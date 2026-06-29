namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Local user data used to issue an application session.
/// </summary>
/// <param name="UserId">Local user identifier.</param>
/// <param name="DisplayName">Display name shown in authenticated UI.</param>
/// <param name="Email">Verified email address.</param>
/// <param name="AvatarUrl">Provider avatar URL when supplied.</param>
/// <param name="IsAdmin">Whether the user has local administrator privileges.</param>
public sealed record SignInUserResult(
    UserId UserId,
    string DisplayName,
    string Email,
    string? AvatarUrl,
    bool IsAdmin);
