using AbhijeetSite.Api.Features.Identity;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Result;
using AbhijeetSite.Api.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AbhijeetSite.Api.Tests;

public sealed class ExternalLoginUpsertTests : IClassFixture<PostgreSqlDatabaseFixture>
{
    private const string AdminEmail = "abhijeethaval@gmail.com";
    private const string AvatarUrl = "https://lh3.googleusercontent.com/avatar";
    private const string DisplayName = "Abhijeet Haval";
    private const string GoogleSubject = "google-subject-123";
    private const string RegularEmail = "reader@example.com";

    private static readonly DateTimeOffset SignedInAt =
        new(2026, 06, 29, 10, 00, 00, TimeSpan.Zero);

    private readonly PostgreSqlDatabaseFixture _fixture;

    public ExternalLoginUpsertTests(PostgreSqlDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [DockerRequiredFact]
    public async Task HandleAsync_NewVerifiedGoogleAccount_CreatesUserAndExternalLogin()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        ExternalLoginUpsertHandler handler = CreateHandler(dbContext, []);
        ExternalLoginClaims claims = CreateClaims(UniqueEmail());

        Result<SignInUserResult> result = await handler.HandleAsync(claims, CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        User user = await dbContext.Users.SingleAsync(item => item.Id == result.Value.UserId);
        ExternalLogin login = await dbContext.ExternalLogins.SingleAsync(item => item.UserId == user.Id);
        Assert.Equal(claims.Email, user.Email);
        Assert.Equal(claims.ProviderSubject, login.ProviderSubject);
        Assert.False(user.IsAdmin);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_AdminAllowlistEmail_CreatesAdminUser()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        ExternalLoginUpsertHandler handler = CreateHandler(dbContext, [AdminEmail]);
        ExternalLoginClaims claims = CreateClaims(AdminEmail, UniqueSubject());

        Result<SignInUserResult> result = await handler.HandleAsync(claims, CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        User user = await dbContext.Users.SingleAsync(item => item.Id == result.Value.UserId);
        Assert.True(user.IsAdmin);
        Assert.True(result.Value.IsAdmin);
    }

    [DockerRequiredFact]
    public async Task HandleAsync_UnverifiedEmail_ReturnsBusinessError()
    {
        await using AppDbContext dbContext = _fixture.CreateDbContext();
        ExternalLoginUpsertHandler handler = CreateHandler(dbContext, []);
        ExternalLoginClaims claims = CreateClaims(UniqueEmail(), isEmailVerified: false);

        Result<SignInUserResult> result = await handler.HandleAsync(claims, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCategory.Business, result.Error?.Category);
        Assert.Equal(IdentityErrors.EmailNotVerifiedCode, result.Error?.Code);
    }

    private static ExternalLoginUpsertHandler CreateHandler(
        AppDbContext dbContext,
        string[] adminEmails)
    {
        IdentityAuthenticationOptions options = new()
        {
            AdminEmails = adminEmails
        };

        ManualApplicationClock clock = new(SignedInAt);
        return new ExternalLoginUpsertHandler(dbContext, clock, Options.Create(options));
    }

    private static ExternalLoginClaims CreateClaims(
        string email,
        string providerSubject = GoogleSubject,
        bool isEmailVerified = true)
    {
        return ExternalLoginClaims.Create(
            ExternalLoginProvider.Google,
            providerSubject,
            DisplayName,
            email,
            isEmailVerified,
            AvatarUrl).Value;
    }

    private static string UniqueEmail()
    {
        return $"{Guid.CreateVersion7():N}-{RegularEmail}";
    }

    private static string UniqueSubject()
    {
        return $"{GoogleSubject}-{Guid.CreateVersion7():N}";
    }
}
