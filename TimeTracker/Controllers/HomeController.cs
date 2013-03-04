using System;
using System.Linq;
using System.Web.Mvc;
using NodaTime;
using TimeTracker.Models;
using TimeTracker.ViewModels;

namespace TimeTracker.Controllers
{
    public class IndexViewModel
    {
        public IOrderedQueryable<TimeLog> Logs { get; set; }

        public FullCustomForm Form { get; set; }
    }

    public class HomeController : DocumentController
    {
        private readonly IClock clock;

        public HomeController(IClock clock)
        {
            this.clock = clock;
        }

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("StartPage");
            }

            var logs =
                DocumentSession.Query<TimeLog>()
                               .Where(x => (x.UserId == Principal.Id)).OrderByDescending(z=>z.StartTime);

            var zonedDateTime = new ZonedDateTime(clock.Now, CurrentTimeZone);
            var localDate = TempData["prev-date"] is LocalDate ? (LocalDate)TempData["prev-date"] : zonedDateTime.LocalDateTime.Date;

            return View(new IndexViewModel
                            {
                                Logs = logs,
                                Form = new FullCustomForm() {Date = localDate}
                            });
        }

        
        [HttpPost]
        [Authorize]
        public ActionResult FullCustom(FullCustomForm fullCustomForm)
        {
            var fullCustomFormTimeLogConverter = new FullCustomFormTimeLogConverter(clock);

            var timeLog = fullCustomFormTimeLogConverter.Convert(fullCustomForm);
            timeLog.UserId = Principal.Id;

            DocumentSession.Store(timeLog);

            TempData["prev-date"] = fullCustomForm.Date;

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult CloseTimeLog(CloseTimeLog closeTimeLog)
        {
            var timeLog = DocumentSession.Load<TimeLog>(closeTimeLog.TimeLogId);

            if (timeLog.UserId != Principal.Id)
            {
                throw new Exception("It's not possible to delete another users timelog...");
            }

            var duration = Duration(closeTimeLog, timeLog.StartTime);

            timeLog.Duration = duration;

            DocumentSession.Store(timeLog);

            return RedirectToAction("Index");
        }

        private TimeSpan Duration(CloseTimeLog closeTimeLog, DateTimeOffset timeLog)
        {
            var fromDateTimeOffset = ZonedDateTime.FromDateTimeOffset(timeLog);
            var localTime = closeTimeLog.EndTime;
            
            if (localTime.Equals(LocalTime.Midnight))
            {
                var zonedDateTime = new ZonedDateTime(clock.Now, CurrentTimeZone);
                localTime = zonedDateTime.LocalDateTime.TimeOfDay;
            }

            var between = Period.Between(fromDateTimeOffset.LocalDateTime.TimeOfDay, localTime);
            var duration = between.ToDuration().ToTimeSpan();
            return duration;
        }

        [HttpPost]
        [Authorize]
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