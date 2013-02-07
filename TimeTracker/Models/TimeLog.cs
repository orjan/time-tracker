using System;

namespace TimeTracker.Models
{
    public class TimeLog
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}