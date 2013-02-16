using NodaTime;
using NodaTime.Text;

namespace TimeTracker.ViewModels.Binders.Noda
{
    public class LocalDateBinder : NodaModelBinder<LocalDate>
    {
        private readonly LocalDatePattern parser;

        public LocalDateBinder()
        {
            parser = LocalDatePattern.IsoPattern;
        }

        protected override LocalDate Parse(string attemptedValue)
        {
            return parser.Parse(attemptedValue).GetValueOrThrow();
        }
    }
}