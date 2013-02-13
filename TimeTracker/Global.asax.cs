using System;
using System.Globalization;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Raven.Client.Document;
using Raven.Client.Indexes;
using TimeTracker.Models;

namespace TimeTracker
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        public static DocumentStore DocumentStore { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof (TimeSpan), new TimeBinder());


            DocumentStore = new DocumentStore
                                {
                                    ConnectionStringName = "RavenDB"
                                };

            DocumentStore.Initialize();
            IndexCreation.CreateIndexes(typeof (MvcApplication).Assembly, DocumentStore);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (String.IsNullOrEmpty(authTicket.UserData))
                {
                    FormsAuthentication.SignOut();
                }
                else
                {
                    HttpContext.Current.User = CustomPrincipal.Deserialize(authTicket.UserData);
                }
            }
        }
    }

    public class TimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Ensure there's incomming data
            string key = bindingContext.ModelName;
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(key);

            if (valueProviderResult == null || string.IsNullOrEmpty(valueProviderResult.AttemptedValue))
            {
                return null;
            }

            // Preserve it in case we need to redisplay the form
            bindingContext.ModelState.SetModelValue(key, valueProviderResult);

            return TimeSpan.ParseExact(valueProviderResult.AttemptedValue, "hhmm", CultureInfo.InvariantCulture);
        }
    }
}