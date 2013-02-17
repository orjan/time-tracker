using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using Xunit;

namespace TimeTracker.ViewModels
{
    public class ConverterTest
    {
        [Fact]
        public void FactMethodName()
        {
            var dateTimeZone = NodaTime.DateTimeZoneProviders.Default["Europe/Stockholm"];
            // dateTimeZone.
            int j = 1;
        }
        
        [Fact]
        public void FactMethodName2()
        {
            var between = Period.Between(new LocalTime(7, 0), new LocalTime(17, 0));
            var between2 = Period.Between(new LocalTime(7, 0), new LocalTime(6, 0));


            var dateTimeOffset = SystemClock.Instance.Now.ToDateTimeOffset();
            var fromDateTimeOffset = ZonedDateTime.FromDateTimeOffset(dateTimeOffset);

            int j = 1;
        }
    }
}
