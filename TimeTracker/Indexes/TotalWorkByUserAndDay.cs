using System;
using System.Linq;
using Raven.Client.Indexes;
using TimeTracker.Models;

namespace TimeTracker.Indexes
{
    public class TotalWorkByUserAndDay : AbstractIndexCreationTask<TimeLog, TotalWorkByUserAndDay.Result>
    {
        public TotalWorkByUserAndDay()
        {
            Map = logs => from log in logs
                          where log.Duration > TimeSpan.Zero
                          select new Result
                                     {
                                         UserId = log.UserId,
                                         Date = log.StartTime.Date,
                                         TotalAmountOfWork = log.Duration,
                                     };

            Reduce = timeLogs => from log in timeLogs
                                 group log by new {log.UserId, log.Date}
                                 into g
                                 select new Result
                                            {
                                                UserId = g.Key.UserId,
                                                Date = g.Key.Date,
                                                TotalAmountOfWork = new TimeSpan(g.Sum(x=>x.TotalAmountOfWork.Ticks))
                                            };
        }

        public class Result
        {
            public DateTime Date;
            public TimeSpan TotalAmountOfWork;
            public int UserId;
        }
    }
}