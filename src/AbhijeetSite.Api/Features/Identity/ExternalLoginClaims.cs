using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Normalized claims returned by an external identity provider.
/// </summary>
public sealed record ExternalLoginClaims(
    ExternalLoginProvider Provider,
    string ProviderSubject,
    string DisplayName,
    string Email,
    bool IsEmailVerified,
    string? AvatarUrl)
{
    /// <summary>
    /// Creates normalized external login claims after validating required provider fields.
    /// </summary>
    public static Result<ExternalLoginClaims> Create(
        ExternalLoginProvider provider,
        string? providerSubject,
        string? displayName,
        string? email,
        bool isEmailVerified,
        string? avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(providerSubject))
        {
            return Result<ExternalLoginClaims>.Failure(IdentityErrors.MissingExternalClaim("sub"));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result<ExternalLoginClaims>.Failure(IdentityErrors.MissingExternalClaim("name"));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<ExternalLoginClaims>.Failure(IdentityErrors.MissingExternalClaim("email"));
        }

        ExternalLoginClaims claims = new(
            provider,
            providerSubject,
            displayName,
            email,
            isEmailVerified,
            avatarUrl);

        return Result<ExternalLoginClaims>.Success(claims);
    }
}
