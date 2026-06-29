namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Local user linked to one or more external login providers.
/// </summary>
public sealed class User
{
    private User()
    {
        DisplayName = string.Empty;
        Email = string.Empty;
    }

    private User(
        UserId id,
        string displayName,
        string email,
        string? avatarUrl,
        DateTimeOffset createdAt,
        bool isAdmin)
    {
        Id = id;
        DisplayName = displayName;
        Email = email;
        AvatarUrl = avatarUrl;
        CreatedAt = createdAt;
        LastSignedInAt = createdAt;
        IsAdmin = isAdmin;
    }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public UserId Id { get; private set; }

    /// <summary>
    /// Gets the user's display name.
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Gets the user's avatar URL when supplied by the provider.
    /// </summary>
    public string? AvatarUrl { get; private set; }

    /// <summary>
    /// Gets when the user was first created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets when the user last signed in.
    /// </summary>
    public DateTimeOffset LastSignedInAt { get; private set; }

    /// <summary>
    /// Gets whether the user has administrator privileges.
    /// </summary>
    public bool IsAdmin { get; private set; }

    /// <summary>
    /// Creates a local user.
    /// </summary>
    public static User Create(
        UserId id,
        string displayName,
        string email,
        string? avatarUrl,
        DateTimeOffset createdAt,
        bool isAdmin)
    {
        return new User(id, displayName, email, avatarUrl, createdAt, isAdmin);
    }

    /// <summary>
    /// Records a successful external sign-in.
    /// </summary>
    public void RecordSignIn(
        string displayName,
        string email,
        string? avatarUrl,
        DateTimeOffset signedInAt,
        bool isAdmin)
    {
        DisplayName = displayName;
        Email = email;
        AvatarUrl = avatarUrl;
        LastSignedInAt = signedInAt;
        IsAdmin = isAdmin;
    }
}
