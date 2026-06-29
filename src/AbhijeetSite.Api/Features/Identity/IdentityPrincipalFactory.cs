using System.Security.Claims;

namespace AbhijeetSite.Api.Features.Identity;

internal static class IdentityPrincipalFactory
{
    public static ClaimsPrincipal Create(SignInUserResult user)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.UserId.Value.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(IdentityClaimTypes.IsAdmin, user.IsAdmin.ToString())
        ];

        AddAvatarClaim(claims, user.AvatarUrl);

        ClaimsIdentity identity = new(claims, IdentityAuthenticationSchemes.ApplicationCookie);
        return new ClaimsPrincipal(identity);
    }

    private static void AddAvatarClaim(List<Claim> claims, string? avatarUrl)
    {
        if (!string.IsNullOrWhiteSpace(avatarUrl))
        {
            claims.Add(new Claim(IdentityClaimTypes.AvatarUrl, avatarUrl));
        }
    }
}
