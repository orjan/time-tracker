using System;
using System.Linq;
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
            IOrderedQueryable<TotalWorkByUserAndDay.Result> logs =
                DocumentSession.Query<TotalWorkByUserAndDay.Result, TotalWorkByUserAndDay>()
                               .Where(x => x.UserId == Principal.Id)
                               .OrderByDescending(x => x.Date);

            return View(logs);
        }

        [HttpPost]
        public ActionResult RegisterTime(ShortTimeLogForm timeLogFormat)
        {
            DateTime currentTime = timeLogFormat.Parse().CalculateDateTime();

            TimeLog timeLog = DocumentSession.Query<TimeLog>()
                                             .SingleOrDefault(
                                                 t => t.UserId == Principal.Id && t.StopTime.Equals(DateTime.MinValue));

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
    }
}