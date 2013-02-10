using System;

namespace TimeTracker.Models
{
    public class TimeLog
    {
        public TimeLog()
        {
            StartTime = DateTime.MinValue;
            StopTime = DateTime.MinValue;
            Time = TimeSpan.Zero;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public TimeSpan Time { get; set; }

        public void Stop(DateTime stopTime)
        {
            StopTime = stopTime;
            Time = stopTime.Subtract(StartTime);
        }

    }
}