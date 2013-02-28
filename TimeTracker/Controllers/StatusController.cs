using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NodaTime;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    [Authorize]
    public class StatusController : DocumentController
    {
        //
        // GET: /Status/

        public ActionResult Index()
        {
            // TODO: make use of index when server has been upgraded

            IEnumerable<IGrouping<int, AggregateWorkViewModel>> logs =
                DocumentSession.Query<TimeLog>()
                               .Where(x => x.UserId == Principal.Id)
                               .ToList()
                               .GroupBy(x => ZonedDateTime.FromDateTimeOffset(x.StartTime).LocalDateTime.Date)
                               .Select(
                                   x =>
                                   new AggregateWorkViewModel
                                       {
                                           Date = x.Key,
                                           AmountOfWork = Duration.FromTicks(x.Sum(y => y.Duration.Ticks))
                                       }).GroupBy(x => x.Date.WeekOfWeekYear);
                                       

            return View(logs);
        }


        public class AggregateWorkViewModel
        {
            public LocalDate Date { get; set; }
            public Duration AmountOfWork { get; set; }
        }
    }
}