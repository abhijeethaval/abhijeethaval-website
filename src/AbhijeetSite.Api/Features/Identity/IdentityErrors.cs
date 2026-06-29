using AbhijeetSite.Api.SharedKernel.Result;

namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Error catalog owned by the identity module.
/// </summary>
public static class IdentityErrors
{
    /// <summary>
    /// Stable email-not-verified error code.
    /// </summary>
    public const string EmailNotVerifiedCode = "identity.email.notVerified";

    private const string MissingClaimCode = "identity.externalClaim.missing";
    private const string MissingLocalUserCode = "identity.localUser.missing";
    private const string PersistenceFailureCode = "identity.persistence.failure";
    private const string SessionClaimMissingCode = "identity.sessionClaim.missing";

    /// <summary>
    /// Creates a missing external provider claim validation error.
    /// </summary>
    public static Error MissingExternalClaim(string claimName)
    {
        string message = $"The external provider did not return required claim '{claimName}'.";
        return new Error(MissingClaimCode, message, ErrorCategory.Validation);
    }

    /// <summary>
    /// Creates an email-not-verified business error.
    /// </summary>
    public static Error EmailNotVerified(string email)
    {
        string message = $"The provider did not verify email '{email}'. Use a verified Google account.";
        return new Error(EmailNotVerifiedCode, message, ErrorCategory.Business);
    }

    /// <summary>
    /// Creates a missing local user infrastructure error.
    /// </summary>
    public static Error MissingLocalUser(UserId userId)
    {
        string message = $"External login references missing local user '{userId.Value}'.";
        return new Error(MissingLocalUserCode, message, ErrorCategory.Infrastructure);
    }

    /// <summary>
    /// Creates an identity persistence failure error.
    /// </summary>
    public static Error PersistenceFailure()
    {
        string message = "Identity changes could not be saved. Verify PostgreSQL connectivity and retry.";
        return new Error(PersistenceFailureCode, message, ErrorCategory.Infrastructure);
    }

    /// <summary>
    /// Creates a missing local session claim error.
    /// </summary>
    public static Error SessionClaimMissing(string claimName)
    {
        string message = $"Authenticated session is missing required claim '{claimName}'. Sign in again.";
        return new Error(SessionClaimMissingCode, message, ErrorCategory.Infrastructure);
    }
}
