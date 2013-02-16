using System;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using NodaTime;
using TimeTracker.Indexes;
using TimeTracker.Models;
using TimeTracker.ViewModels;

namespace TimeTracker.Controllers
{
    public class IndexViewModel
    {
        public IOrderedQueryable<TimeLog> Logs { get; set; }

        public FullCustomForm Form { get; set; }
    }

    [Authorize]
    public class HomeController : DocumentController
    {
        private NodaTime.DateTimeZone defaultTimeZone = DateTimeZoneProviders.Default["Europe/Stockholm"];

        public ActionResult Index()
        {
            var logs =
                DocumentSession.Query<TimeLog>()
                               .Where(x => (x.UserId == Principal.Id)).OrderByDescending(z=>z.StartTime);

            // TODO: setup fullcustom form as a model
            var zonedDateTime = new ZonedDateTime(SystemClock.Instance.Now, defaultTimeZone);
            

            return View(new IndexViewModel
                            {
                                Logs = logs,
                                Form = new FullCustomForm() {Date = zonedDateTime.LocalDateTime.Date}
                            });
        }

        [HttpPost]
        public ActionResult FullCustom(FullCustomForm fullCustomForm)
        {
            var fullCustomFormTimeLogConverter = new FullCustomFormTimeLogConverter(SystemClock.Instance);

            var timeLog = fullCustomFormTimeLogConverter.Convert(fullCustomForm);
            timeLog.UserId = Principal.Id;

            DocumentSession.Store(timeLog);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var timeLog = DocumentSession.Load<TimeLog>(id);

            if (timeLog.UserId != Principal.Id)
            {
                throw new Exception("It's not possible to delete another users timelog...");
            }

            DocumentSession.Delete(timeLog);

            return RedirectToAction("Index");
        }
    }
}