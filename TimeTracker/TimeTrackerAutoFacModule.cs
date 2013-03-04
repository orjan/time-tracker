using Autofac;
using NodaTime;

namespace TimeTracker
{
    public class TimeTrackerAutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => SystemClock.Instance).As<IClock>();
        }
    }
}