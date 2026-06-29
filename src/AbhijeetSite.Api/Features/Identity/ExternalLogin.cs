namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Maps an external provider subject to a local user.
/// </summary>
public sealed class ExternalLogin
{
    private ExternalLogin()
    {
        ProviderSubject = string.Empty;
        EmailAtLogin = string.Empty;
    }

    private ExternalLogin(
        ExternalLoginId id,
        UserId userId,
        ExternalLoginProvider provider,
        string providerSubject,
        string emailAtLogin)
    {
        Id = id;
        UserId = userId;
        Provider = provider;
        ProviderSubject = providerSubject;
        EmailAtLogin = emailAtLogin;
    }

    /// <summary>
    /// Gets the external login identifier.
    /// </summary>
    public ExternalLoginId Id { get; private set; }

    /// <summary>
    /// Gets the linked local user identifier.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the external provider.
    /// </summary>
    public ExternalLoginProvider Provider { get; private set; }

    /// <summary>
    /// Gets the stable provider subject.
    /// </summary>
    public string ProviderSubject { get; private set; }

    /// <summary>
    /// Gets the email supplied by the provider at login time.
    /// </summary>
    public string EmailAtLogin { get; private set; }

    /// <summary>
    /// Creates an external login mapping.
    /// </summary>
    public static ExternalLogin Create(
        ExternalLoginId id,
        UserId userId,
        ExternalLoginProvider provider,
        string providerSubject,
        string emailAtLogin)
    {
        return new ExternalLogin(id, userId, provider, providerSubject, emailAtLogin);
    }

    /// <summary>
    /// Records the latest provider email observed during sign-in.
    /// </summary>
    public void RecordSignIn(string emailAtLogin)
    {
        EmailAtLogin = emailAtLogin;
    }
}
