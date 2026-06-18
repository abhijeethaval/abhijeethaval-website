namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Strongly typed identity for an external login mapping.
/// </summary>
/// <param name="Value">Underlying UUID value.</param>
public readonly record struct ExternalLoginId(Guid Value)
{
    /// <summary>
    /// Creates a new external login identifier.
    /// </summary>
    public static ExternalLoginId New()
    {
        return new ExternalLoginId(Guid.CreateVersion7());
    }
}
