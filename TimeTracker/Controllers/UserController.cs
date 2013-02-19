using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.Mvc;
using NodaTime;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    [Authorize]
    public class UserController : DocumentController
    {
        private readonly IDateTimeZoneProvider dateTimeZoneProvider;

        public UserController()
        {
            dateTimeZoneProvider = DateTimeZoneProviders.Default;
        }

        public ActionResult Details()
        {
            var user = DocumentSession.Load<User>(Principal.Id);

            return View(new UserDetailsViewModel 
                { 
                    FullName = user.FullName, 
                    CurrentDateTimeZone = user.DateTimeZone,
                    TimeZoneIds = dateTimeZoneProvider.Ids
                });
        }

        [HttpPost]
        public ActionResult UpdateUserDetails(UserDetailsViewModel userDetails)
        {
            var user = DocumentSession.Load<User>(Principal.Id);

            user.FullName = userDetails.FullName;
            user.DateTimeZone = userDetails.CurrentDateTimeZone;

            DocumentSession.Store(user);

            return RedirectToAction("Index", "Home");
        }
    }

    public class UserDetailsViewModel
    {
        [DisplayName("Select time zone")]
        public string CurrentDateTimeZone { get; set; }
        
        [DisplayName("Full name")]
        public string FullName { get; set; }
        
        public ReadOnlyCollection<string> TimeZoneIds { get; set; }
    }
}
