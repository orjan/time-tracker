using NodaTime;

namespace TimeTracker.ViewModels
{
    public class CloseTimeLog
    {
        public int TimeLogId { get; set; }
        public LocalTime EndTime { get; set; }
    }
}