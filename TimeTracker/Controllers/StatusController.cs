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
            var logs =
                DocumentSession.Query<TimeLog>()
                               .Where(x => x.UserId == Principal.Id)
                               .ToList()
                               .GroupBy(x => ZonedDateTime.FromDateTimeOffset(x.StartTime).LocalDateTime.Date)
                               .Select(x => new AggregateWorkViewModel
                                                {
                                                    Date = x.Key,
                                                    AmountOfWork = Duration.FromTicks(x.Sum(y => y.Duration.Ticks))
                                                }).ToList();

            var workLogPerWeek = logs.GroupBy(x => x.Date.WeekOfWeekYear)
                                    .Select(workDays => new WeekHolder 
                                        {
                                             Week = workDays.Key,
                                             Work = Duration.FromTicks(workDays.Sum(w=>w.AmountOfWork.Ticks)),
                                             BaseLine = Duration.FromHours(workDays.Count()*8),
                                             WorkLogs = workDays.ToDictionary(y => y.Date.IsoDayOfWeek)
                                        })
                                    .OrderByDescending(x=>x.Week);


            LocalDate lastDay = logs.Max(x => x.Date);
            LocalDate firstDay = logs.Min(x => x.Date);


            return View(new WorkLogStats
                            {
                                FirstDay = firstDay,
                                LastDate = lastDay,
                                WorkPerDay = workLogPerWeek
                            });
        }

        public class AggregateWorkViewModel
        {
            public LocalDate Date { get; set; }
            public Duration AmountOfWork { get; set; }
        }

        public class StatsViewModel
        {
            public int StartWeek { get; set; }
            public int EndWeek { get; set; }

            public ILookup<LocalDate, AggregateWorkViewModel> Data { get; set; }

            public Duration WorkLogFor(int w, int y)
            {
                return Duration.FromTicks(
                    Data[LocalDate.FromWeekYearWeekAndDay(2013, w, (IsoDayOfWeek) y)].Sum(x => x.AmountOfWork.Ticks));
            }
        }

        public class WorkLogStats
        {
            public LocalDate FirstDay { get; set; }
            public LocalDate LastDate { get; set; }
            public IOrderedEnumerable<WeekHolder> WorkPerDay { get; set; }
        }
    }

    public class WeekHolder
    {
        public int Week { get; set; }

        public Duration Work { get; set; }

        public Duration BaseLine { get; set; }

        public Duration Balance
        {
            get { return Work - BaseLine; }
        }

        public Dictionary<IsoDayOfWeek, StatusController.AggregateWorkViewModel> WorkLogs { get; set; }
    }
}