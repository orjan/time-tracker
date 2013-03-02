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
            IEnumerable<AggregateWorkViewModel> logs =
                DocumentSession.Query<TimeLog>()
                               .Where(x => x.UserId == Principal.Id)
                               .ToList()
                               .GroupBy(x => ZonedDateTime.FromDateTimeOffset(x.StartTime).LocalDateTime.Date)
                               .Select(x => new AggregateWorkViewModel
                                                {
                                                    Date = x.Key,
                                                    AmountOfWork = Duration.FromTicks(x.Sum(y => y.Duration.Ticks))
                                                }).ToList(); // .ToDictionary(model => model.Date);

            var z = logs.GroupBy(x => x.Date.WeekOfWeekYear).Select(x => new WeekHolder
                                                                     {
                                                                         Week = x.Key, 
                                                                         Work = Duration.FromTicks(x.Sum(w=>w.AmountOfWork.Ticks)),
                                                                         BaseLine = Duration.FromHours(x.Count() * 8),
                                                                         WorkLogs = x.ToDictionary(y=>y.Date),
                                                                     }).ToDictionary(v=> v.Week);


            var lastDay = logs.Max(x => x.Date);
            var firstDay = logs.Min(x => x.Date);

            return View(new Flumm
                            {
                               FirstDay = firstDay,
                                LastDate = lastDay,
                                WorkPerDay = z
                            });
        }

        public class Flumm
        {
            public LocalDate FirstDay { get; set; }
            public LocalDate LastDate { get; set; }
            public Dictionary<int, WeekHolder> WorkPerDay { get; set; }
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

    public class WeekHolder
    {
        public int Week { get; set; }

        public Duration Work { get; set; }

        public Duration BaseLine { get; set; }

        public Duration Balance { get { return Work-BaseLine; } }

        public Dictionary<LocalDate, StatusController.AggregateWorkViewModel> WorkLogs { get; set; }
    }
}