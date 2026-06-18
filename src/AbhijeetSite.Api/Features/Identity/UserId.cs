namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Strongly typed identity for a local user.
/// </summary>
/// <param name="Value">Underlying UUID value.</param>
public readonly record struct UserId(Guid Value)
{
    /// <summary>
    /// Creates a new user identifier.
    /// </summary>
    public static UserId New()
    {
        return new UserId(Guid.CreateVersion7());
    }
}
