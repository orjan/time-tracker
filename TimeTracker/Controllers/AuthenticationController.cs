using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    public class AuthenticationController : DocumentController
    {
        private void LoginUser(User user)
        {
            string userData = user.Serialize();

            var authenticationTicket = new FormsAuthenticationTicket(
                1,
                user.Email,
                DateTime.Now,
                DateTime.Now.AddMinutes(15),
                false,
                userData);

            string ticket = FormsAuthentication.Encrypt(authenticationTicket);
            var formsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticket);
            Response.Cookies.Add(formsCookie);
        }


        public ActionResult LogOn()
        {
            var openid = new OpenIdRelyingParty();
            IAuthenticationResponse response = openid.GetResponse();


            if (response != null)
            {
                var fetchResponse = response.GetExtension<FetchResponse>();
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        var claimedIdentifier = response.ClaimedIdentifier;

                        User user = DocumentSession.Query<User>("UserByClaimedIdentifier").SingleOrDefault(u => u.ClaimedIdentifier.Equals(claimedIdentifier));

                        if (user == null)
                        {
                            user = new User
                                       {
                                           FullName = fetchResponse.GetAttributeValue(WellKnownAttributes.Name.FullName),
                                           Email = fetchResponse.GetAttributeValue(WellKnownAttributes.Contact.Email),
                                           ClaimedIdentifier = claimedIdentifier
                                       };

                            DocumentSession.Store(user);
                        }
                        
                        LoginUser(user);

                        if (string.IsNullOrEmpty(user.FullName))
                        {
                            return RedirectToAction("Details", "User");
                        }

                        return RedirectToAction("Index", "Home");

                    case AuthenticationStatus.Canceled:
                        ModelState.AddModelError("loginIdentifier",
                                                 "Login was cancelled at the provider");
                        break;
                    case AuthenticationStatus.Failed:
                        ModelState.AddModelError("loginIdentifier",
                                                 "Login failed using the provided OpenID identifier");
                        break;
                }
            }

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogOn(string loginIdentifier)
        {
            if (!Identifier.IsValid(loginIdentifier))
            {
                ModelState.AddModelError("loginIdentifier",
                                         "The specified login identifier is invalid");
                return View();
            }
            else
            {
                var openid = new OpenIdRelyingParty();
                IAuthenticationRequest request = openid.CreateRequest(Identifier.Parse(loginIdentifier));
                
                /*
                request.AddExtension(new ClaimsRequest
                                         {
                                             Email = DemandLevel.Require,
                                             FullName = DemandLevel.Require§
                                         });
                */

                var fetch = new FetchRequest();
                fetch.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                fetch.Attributes.AddRequired(WellKnownAttributes.Name.FullName);
                request.AddExtension(fetch);

                return request.RedirectingResponse.AsActionResult();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    }
}