namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Authorization policy names owned by the identity module.
/// </summary>
public static class AuthPolicies
{
    /// <summary>
    /// Policy requiring any authenticated local user.
    /// </summary>
    public const string AuthenticatedUser = "AuthenticatedUser";

    /// <summary>
    /// Policy requiring local administrator privileges.
    /// </summary>
    public const string AdminOnly = "AdminOnly";
}
