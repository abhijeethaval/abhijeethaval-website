using AbhijeetSite.Api.SharedKernel.Time;

namespace AbhijeetSite.Api.Tests.Support;

public sealed class ManualApplicationClock : IApplicationClock
{
    public ManualApplicationClock(DateTimeOffset utcNow)
    {
        UtcNow = utcNow;
    }

    public DateTimeOffset UtcNow { get; }
}
