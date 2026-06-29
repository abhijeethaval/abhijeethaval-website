namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Configuration for local authentication and external providers.
/// </summary>
public sealed class IdentityAuthenticationOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Auth";

    /// <summary>
    /// Google OAuth configuration.
    /// </summary>
    public GoogleExternalLoginOptions Google { get; set; } = new();

    /// <summary>
    /// File-system path used to persist ASP.NET Core Data Protection keys.
    /// </summary>
    public string DataProtectionKeysPath { get; set; } = string.Empty;

    /// <summary>
    /// Verified email addresses that receive local administrator privileges.
    /// </summary>
    public string[] AdminEmails { get; set; } = [];

    /// <summary>
    /// Gets whether Google OAuth credentials are configured.
    /// </summary>
    public bool HasGoogleCredentials => Google.HasCredentials;

    /// <summary>
    /// Returns whether the supplied verified email should be an administrator.
    /// </summary>
    public bool IsAdminEmail(string email)
    {
        return AdminEmails.Any(adminEmail => string.Equals(
            adminEmail,
            email,
            StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Google OAuth client credentials.
/// </summary>
public sealed class GoogleExternalLoginOptions
{
    /// <summary>
    /// Google OAuth client ID.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Google OAuth client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets whether both OAuth credentials are configured.
    /// </summary>
    public bool HasCredentials =>
        !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);
}
