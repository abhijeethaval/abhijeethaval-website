using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using AbhijeetSite.Api.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Creates or updates local identity records from a verified external login.
/// </summary>
public sealed class ExternalLoginUpsertHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationClock _clock;
    private readonly IOptions<IdentityAuthenticationOptions> _options;

    /// <summary>
    /// Creates the handler.
    /// </summary>
    public ExternalLoginUpsertHandler(
        AppDbContext dbContext,
        IApplicationClock clock,
        IOptions<IdentityAuthenticationOptions> options)
    {
        _dbContext = dbContext;
        _clock = clock;
        _options = options;
    }

    /// <summary>
    /// Upserts local user and external login records.
    /// </summary>
    public async Task<Result<SignInUserResult>> HandleAsync(
        ExternalLoginClaims claims,
        CancellationToken cancellationToken)
    {
        if (!claims.IsEmailVerified)
        {
            return Result<SignInUserResult>.Failure(IdentityErrors.EmailNotVerified(claims.Email));
        }

        ExternalLogin? login = await FindExternalLoginAsync(claims, cancellationToken);
        if (login is not null)
        {
            return await UpdateExistingLoginAsync(login, claims, cancellationToken);
        }

        User? user = await FindVerifiedEmailUserAsync(claims.Email, cancellationToken);
        return user is null
            ? await CreateUserAndLoginAsync(claims, cancellationToken)
            : await LinkExistingUserAsync(user, claims, cancellationToken);
    }

    private async Task<ExternalLogin?> FindExternalLoginAsync(
        ExternalLoginClaims claims,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ExternalLogins.SingleOrDefaultAsync(
            login => login.Provider == claims.Provider
                && login.ProviderSubject == claims.ProviderSubject,
            cancellationToken);
    }

    private async Task<User?> FindVerifiedEmailUserAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    private async Task<Result<SignInUserResult>> UpdateExistingLoginAsync(
        ExternalLogin login,
        ExternalLoginClaims claims,
        CancellationToken cancellationToken)
    {
        User? user = await _dbContext.Users.SingleOrDefaultAsync(
            item => item.Id == login.UserId,
            cancellationToken);

        if (user is null)
        {
            return Result<SignInUserResult>.Failure(IdentityErrors.MissingLocalUser(login.UserId));
        }

        ApplySignIn(user, claims);
        login.RecordSignIn(claims.Email);
        return await SaveAsync(user, cancellationToken);
    }

    private async Task<Result<SignInUserResult>> CreateUserAndLoginAsync(
        ExternalLoginClaims claims,
        CancellationToken cancellationToken)
    {
        DateTimeOffset signedInAt = _clock.UtcNow;
        bool isAdmin = _options.Value.IsAdminEmail(claims.Email);
        User user = User.Create(
            UserId.New(),
            claims.DisplayName,
            claims.Email,
            claims.AvatarUrl,
            signedInAt,
            isAdmin);

        _dbContext.Users.Add(user);
        AddExternalLogin(user, claims);
        return await SaveAsync(user, cancellationToken);
    }

    private async Task<Result<SignInUserResult>> LinkExistingUserAsync(
        User user,
        ExternalLoginClaims claims,
        CancellationToken cancellationToken)
    {
        ApplySignIn(user, claims);
        AddExternalLogin(user, claims);
        return await SaveAsync(user, cancellationToken);
    }

    private void ApplySignIn(User user, ExternalLoginClaims claims)
    {
        bool isAdmin = _options.Value.IsAdminEmail(claims.Email);
        user.RecordSignIn(claims.DisplayName, claims.Email, claims.AvatarUrl, _clock.UtcNow, isAdmin);
    }

    private void AddExternalLogin(User user, ExternalLoginClaims claims)
    {
        ExternalLogin login = ExternalLogin.Create(
            ExternalLoginId.New(),
            user.Id,
            claims.Provider,
            claims.ProviderSubject,
            claims.Email);

        _dbContext.ExternalLogins.Add(login);
    }

    private async Task<Result<SignInUserResult>> SaveAsync(
        User user,
        CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Result<SignInUserResult>.Failure(IdentityErrors.PersistenceFailure());
        }

        return Result<SignInUserResult>.Success(ToSignInResult(user));
    }

    private static SignInUserResult ToSignInResult(User user)
    {
        return new SignInUserResult(
            user.Id,
            user.DisplayName,
            user.Email,
            user.AvatarUrl,
            user.IsAdmin);
    }
}
