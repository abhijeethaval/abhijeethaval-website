namespace AbhijeetSite.Api.SharedKernel.Time;

/// <summary>
/// Application clock backed by the system UTC clock.
/// </summary>
public sealed class SystemApplicationClock : IApplicationClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
