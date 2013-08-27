using NodaTime;
using TimeTracker.Models;

namespace TimeTracker.ViewModels
{
    public class FullCustomFormTimeLogConverter
    {
        private readonly IClock clock;
        private readonly DateTimeZone dateTimeZone;

        public FullCustomFormTimeLogConverter(IClock clock)
        {
            this.clock = clock;
            dateTimeZone = DateTimeZoneProviders.Tzdb["Europe/Stockholm"];
        }

        public TimeLog Convert(FullCustomForm form)
        {
            var timeLog = new TimeLog();



            if (form.StartTime.Equals(new LocalTime()))
            {
                form.StartTime = clock.Now.InZone(dateTimeZone).LocalDateTime.TimeOfDay;
            }

            LocalDateTime startDateTime = (form.Date + form.StartTime);
            timeLog.StartTime = startDateTime.InZoneStrictly(dateTimeZone).ToDateTimeOffset();

            Duration between = Period.Between(form.StartTime, form.EndTime).ToDuration();
            if (between > Duration.Zero)
            {
                timeLog.Duration = between.ToTimeSpan();
            }

            return timeLog;
        }
    }
}