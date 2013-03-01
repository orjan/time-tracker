using System;
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

            Dictionary<LocalDate, AggregateWorkViewModel> logs =
                DocumentSession.Query<TimeLog>()
                               .Where(x => x.UserId == Principal.Id)
                               .ToList()
                               .GroupBy(x => ZonedDateTime.FromDateTimeOffset(x.StartTime).LocalDateTime.Date)
                               .Select(x => new AggregateWorkViewModel
                                                {
                                                    Date = x.Key,
                                                    AmountOfWork = Duration.FromTicks(x.Sum(y => y.Duration.Ticks))
                                                }).ToDictionary(model => model.Date);

            var lastDay = logs.Max(x => x.Key);
            var firstDay = logs.Min(x => x.Key);

            return View(new Flumm
                            {
                               FirstDay = firstDay,
                                LastDate = lastDay,
                                WorkPerDay = logs
                            });
        }

        public class Flumm
        {
            public LocalDate FirstDay { get; set; }
            public LocalDate LastDate { get; set; }
            public IDictionary<LocalDate, AggregateWorkViewModel> WorkPerDay { get; set; }
        }


        public class StatsViewModel
        {
            public int StartWeek { get; set; }
            public int EndWeek { get; set; }

            public ILookup<LocalDate, AggregateWorkViewModel> Data { get; set; }

            public Duration WorkLogFor(int w, int y)
            {
               return  Duration.FromTicks(
                    Data[LocalDate.FromWeekYearWeekAndDay(2013, w, (IsoDayOfWeek) y)].Sum(x => x.AmountOfWork.Ticks));
            }
        }

        public class AggregateWorkViewModel
        {
            public LocalDate Date { get; set; }
            public Duration AmountOfWork { get; set; }
        }
    }
}