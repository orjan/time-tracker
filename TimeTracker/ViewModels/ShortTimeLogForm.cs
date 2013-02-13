using System;
using System.Globalization;

namespace TimeTracker.ViewModels
{
    public class FullCustomForm
    {
        public FullCustomForm()
        {
            StartDate = DateTime.Now.Date;
            StartTime = TimeSpan.Zero;
            EndTime = TimeSpan.Zero;
        }

        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }

    public class ShortTimeLogForm
    {
        public int Id { get; set; }
        public string Input { get; set; }

        public DatePart Parse()
        {
            if (string.IsNullOrWhiteSpace(Input))
            {
                return new DatePart();
            }

            return new DatePart
                       {
                           Time = TimeSpan.ParseExact(Input, "hhmm", CultureInfo.InvariantCulture)
                       };
        }

        public class DatePart
        {
            public DatePart()
            {
                Time = TimeSpan.Zero;
                Now = DateTime.Now;
            }

            public TimeSpan Time { get; set; }
            public DateTime Now { get; set; }

            public DateTime CalculateDateTime()
            {
                if (Time.Equals(TimeSpan.Zero))
                {
                    return Now;
                }

                return Now.Date.Add(Time);
            }
        }
    }
}