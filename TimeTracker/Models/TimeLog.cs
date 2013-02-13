using System;

namespace TimeTracker.Models
{
    public class TimeLog
    {
        public TimeLog()
        {
            StartTime = DateTime.MinValue;
            Time = TimeSpan.Zero;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        
        public DateTime StartTime { get; set; }
        public TimeSpan Time { get; set; }

        public void Stop(DateTime stopTime)
        {
            Time = stopTime.Subtract(StartTime);
        }

        public bool IsOpen()
        {
            return Time.Equals(TimeSpan.Zero);
        }
    }
}