using System.Net.Http.Json;
using System.Net;
using System.Security.Claims;
using AbhijeetSite.Api.Features.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AbhijeetSite.Api.Tests;

public sealed class AuthApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string AdminEmail = "abhijeethaval@gmail.com";
    private const string AdminName = "Abhijeet Haval";
    private const string AdminUserId = "018f5b5d-27f7-7b5f-9c9f-3564f4d3f9f0";

    private readonly WebApplicationFactory<Program> _factory;

    public AuthApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCurrentUser_AnonymousRequest_ReturnsAnonymousSession()
    {
        HttpClient client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/auth/me");

        response.EnsureSuccessStatusCode();
        CurrentUserResponse? content = await response.Content.ReadFromJsonAsync<CurrentUserResponse>();
        Assert.NotNull(content);
        Assert.False(content.IsAuthenticated);
        Assert.Null(content.User);
    }

    [Fact]
    public async Task GetCurrentUser_DevelopmentRequest_DoesNotRedirectToHttps()
    {
        HttpClient client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        HttpResponseMessage response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AuthorizeAsync_AdminClaim_SucceedsForAdminOnlyPolicy()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        IAuthorizationService authorizationService =
            scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        ClaimsPrincipal principal = CreateAdminPrincipal();

        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            principal,
            resource: null,
            AuthPolicies.AdminOnly);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task AuthorizeAsync_AnonymousUser_FailsForAdminOnlyPolicy()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        IAuthorizationService authorizationService =
            scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        ClaimsPrincipal principal = new(new ClaimsIdentity());

        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            principal,
            resource: null,
            AuthPolicies.AdminOnly);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task GetAdminArticleDrafts_AnonymousRequest_ReturnsUnauthorized()
    {
        HttpClient client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        HttpResponseMessage response = await client.GetAsync("/api/admin/articles/drafts");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static ClaimsPrincipal CreateAdminPrincipal()
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, AdminUserId),
            new(ClaimTypes.Name, AdminName),
            new(ClaimTypes.Email, AdminEmail),
            new(IdentityClaimTypes.IsAdmin, bool.TrueString)
        ];

        ClaimsIdentity identity = new(claims, IdentityAuthenticationSchemes.ApplicationCookie);
        return new ClaimsPrincipal(identity);
    }

    private sealed record CurrentUserResponse(bool IsAuthenticated, AuthenticatedUserResponse? User);

    private sealed record AuthenticatedUserResponse(
        string Id,
        string DisplayName,
        string Email,
        string? AvatarUrl,
        bool IsAdmin);
}
