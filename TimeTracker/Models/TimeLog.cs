using System;

namespace TimeTracker.Models
{
    public class TimeLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public TimeSpan Duration { get; set; }

        public TimeLog()
        {
            StartTime = DateTimeOffset.MinValue;
            Duration = TimeSpan.Zero;
        }

        public bool IsOpen()
        {
            return Duration.Equals(TimeSpan.Zero);
        }
    }
}