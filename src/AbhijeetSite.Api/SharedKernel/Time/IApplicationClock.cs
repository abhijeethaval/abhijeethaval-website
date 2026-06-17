namespace AbhijeetSite.Api.SharedKernel.Time;

/// <summary>
/// Supplies application time for business workflows.
/// </summary>
public interface IApplicationClock
{
    /// <summary>
    /// Gets the current UTC time.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
