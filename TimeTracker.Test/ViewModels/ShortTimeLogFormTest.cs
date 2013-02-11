using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;

namespace TimeTracker.ViewModels
{
    public class ShortTimeLogFormTest
    {
        public static IEnumerable<object[]> ShortFormData
        {
            get
            {
                return new[]
                           {
                               new object[] {"0700", TimeSpan.FromHours(7)},
                               new object[] {"0500", TimeSpan.FromHours(5)},
                               new object[] {"0520", new TimeSpan(0, 5, 20, 0)},
                               new object[] {"0433", new TimeSpan(0, 4, 33, 0)},
                           };
            }
        }

        [Theory, PropertyData("ShortFormData")]
        public void Should_be_able_to_parse_time(string input, TimeSpan expectedTime)
        {
            var shortForm = new ShortTimeLogForm {Input = input};

            Assert.Equal(expectedTime, shortForm.Parse().Time);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Should_be_able_to_pass_null(string input)
        {
            var shortTimeLogForm = new ShortTimeLogForm() { Input = input };

            var datePart = shortTimeLogForm.Parse();

            datePart.Time.Should().Be(TimeSpan.Zero);
        }

        public void Should_be_able_to_get_date()
        {
            var shortTimeLogForm = new ShortTimeLogForm() { Input = "0700" };

            var datePart = shortTimeLogForm.Parse();

            datePart.CalculateDateTime();
        }
    }
}