using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TimeTracker.Indexes;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    [Authorize]
    public class HomeController : DocumentController
    {
        public ActionResult Index()
        {
            var logs =
                DocumentSession.Query<TotalWorkByUserAndDay.Result, TotalWorkByUserAndDay>()
                               .Where(x => x.UserId == Principal.Id)
                               .OrderByDescending(x => x.Date);

            return View(logs);
        }

        [HttpPost]
        public ActionResult RegisterTime()
        {
            DateTime currentTime = DateTime.Now;

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