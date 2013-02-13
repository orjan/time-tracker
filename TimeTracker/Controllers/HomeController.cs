using System;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using TimeTracker.Indexes;
using TimeTracker.Models;
using TimeTracker.ViewModels;

namespace TimeTracker.Controllers
{
    [Authorize]
    public class HomeController : DocumentController
    {
        public ActionResult Index()
        {
            var logs =
                DocumentSession.Query<TimeLog>()
                               .Where(x => (x.UserId == Principal.Id)).OrderByDescending(z=>z.StartTime);


            return View(logs);
        }

        [HttpPost]
        public ActionResult RegisterTime(ShortTimeLogForm timeLogFormat)
        {
            DateTime currentTime = timeLogFormat.Parse().CalculateDateTime();

            TimeLog timeLog = DocumentSession.Query<TimeLog>()
                                             .SingleOrDefault(
                                                 t => t.UserId == Principal.Id && t.Time.Equals(TimeSpan.Zero));

            if (timeLog == null)
            {
                timeLog = new TimeLog
                              {
                                  StartTime = currentTime,
                                  UserId = Principal.Id
                              };
            }
            else
            {
                timeLog.Stop(currentTime);
            }

            DocumentSession.Store(timeLog);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult FullCustom(FullCustomForm fullCustomForm)
        {
            var startTime = fullCustomForm.StartDate.Add(StartTime(fullCustomForm));

            var timeLog = new TimeLog()
                              {
                                  UserId = Principal.Id,
                                  StartTime = startTime
                              };

            if (!fullCustomForm.EndTime.Equals(TimeSpan.Zero))
            {
                timeLog.Time = fullCustomForm.EndTime.Subtract(fullCustomForm.StartTime);
            }

            DocumentSession.Store(timeLog);

            return RedirectToAction("Index");
        }

        private static TimeSpan StartTime(FullCustomForm fullCustomForm)
        {
            if (!fullCustomForm.StartTime.Equals(TimeSpan.Zero))
            {
                return fullCustomForm.StartTime;
            }
            
            var now = DateTime.Now;
            var timeSpan = now.Subtract(now.Date);
            return timeSpan;
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