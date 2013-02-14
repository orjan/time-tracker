using System;

namespace TimeTracker.Models
{
    public class TimeLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public TimeSpan Time { get; set; }

        public TimeLog()
        {
            StartTime = DateTimeOffset.MinValue;
            Time = TimeSpan.Zero;
        }

        public bool IsOpen()
        {
            return Time.Equals(TimeSpan.Zero);
        }
    }
}