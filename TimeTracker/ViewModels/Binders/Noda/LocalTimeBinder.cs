using NodaTime;
using NodaTime.Text;

namespace TimeTracker.ViewModels.Binders.Noda
{
    public class LocalTimeBinder : NodaModelBinder<LocalTime>
    {
        private readonly LocalTimePattern parser;

        public LocalTimeBinder()
        {
            parser = LocalTimePattern.CreateWithInvariantCulture("HHmm");
        }

        protected override LocalTime Parse(string attemptedValue)
        {
            return parser.Parse(attemptedValue).GetValueOrThrow();
        }
    }
}