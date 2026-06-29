using System.Security.Claims;
using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Current authentication state returned to the browser.
/// </summary>
/// <param name="IsAuthenticated">Whether the request has a valid local session.</param>
/// <param name="User">Authenticated user details when signed in.</param>
public sealed record CurrentUserResponse(bool IsAuthenticated, AuthenticatedUserResponse? User)
{
    /// <summary>
    /// Creates an anonymous current-user response.
    /// </summary>
    public static CurrentUserResponse Anonymous()
    {
        return new CurrentUserResponse(false, null);
    }

    /// <summary>
    /// Creates an authenticated current-user response from local session claims.
    /// </summary>
    public static Result<CurrentUserResponse> Create(ClaimsPrincipal principal)
    {
        string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return MissingClaimFailure(ClaimTypes.NameIdentifier);
        }

        string? displayName = principal.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return MissingClaimFailure(ClaimTypes.Name);
        }

        string? email = principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            return MissingClaimFailure(ClaimTypes.Email);
        }

        AuthenticatedUserResponse user = new(
            userId,
            displayName,
            email,
            principal.FindFirstValue(IdentityClaimTypes.AvatarUrl),
            IsAdmin(principal));

        return Result<CurrentUserResponse>.Success(new CurrentUserResponse(true, user));
    }

    private static Result<CurrentUserResponse> MissingClaimFailure(string claimName)
    {
        return Result<CurrentUserResponse>.Failure(IdentityErrors.SessionClaimMissing(claimName));
    }

    private static bool IsAdmin(ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(IdentityClaimTypes.IsAdmin) == bool.TrueString;
    }
}

/// <summary>
/// Authenticated local user details returned to the browser.
/// </summary>
/// <param name="Id">Local user identifier.</param>
/// <param name="DisplayName">Display name.</param>
/// <param name="Email">Verified email address.</param>
/// <param name="AvatarUrl">Provider avatar URL when supplied.</param>
/// <param name="IsAdmin">Whether the user has local administrator privileges.</param>
public sealed record AuthenticatedUserResponse(
    string Id,
    string DisplayName,
    string Email,
    string? AvatarUrl,
    bool IsAdmin);
