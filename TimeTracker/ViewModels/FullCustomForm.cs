using NodaTime;

namespace TimeTracker.ViewModels
{
    public class FullCustomForm
    {
        public LocalDate Date { get; set; }
        public LocalTime StartTime { get; set; }
        public LocalTime EndTime { get; set; }
    }
}