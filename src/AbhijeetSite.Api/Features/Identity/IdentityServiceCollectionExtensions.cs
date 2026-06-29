using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpOverrides;

namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Registers identity services.
/// </summary>
public static class IdentityServiceCollectionExtensions
{
    private const int SessionLifetimeHours = 8;

    /// <summary>
    /// Adds local cookie authentication and Google external login.
    /// </summary>
    public static IServiceCollection AddIdentityAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        IConfigurationSection section = configuration.GetSection(IdentityAuthenticationOptions.SectionName);
        GoogleExternalLoginOptions google = GetGoogleOptions(section);
        services.Configure<IdentityAuthenticationOptions>(section);
        services.ConfigureForwardedHeaders();
        services.ConfigureDataProtection(section, environment, google);
        services.AddScoped<ExternalLoginUpsertHandler>();
        AuthenticationBuilder authentication = services
            .AddAuthentication(IdentityAuthenticationSchemes.ApplicationCookie)
            .AddCookie(IdentityAuthenticationSchemes.ApplicationCookie, options =>
                ConfigureCookie(options, environment));
        AddGoogleIfConfigured(authentication, google);
        services.AddIdentityAuthorization();

        return services;
    }

    private static void ConfigureForwardedHeaders(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
                | ForwardedHeaders.XForwardedHost
                | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }

    private static void ConfigureDataProtection(
        this IServiceCollection services,
        IConfigurationSection authSection,
        IWebHostEnvironment environment,
        GoogleExternalLoginOptions google)
    {
        string keysPath = authSection[nameof(IdentityAuthenticationOptions.DataProtectionKeysPath)]
            ?? string.Empty;
        IDataProtectionBuilder builder = services.AddDataProtection()
            .SetApplicationName("AbhijeetSite.Api");

        if (string.IsNullOrWhiteSpace(keysPath))
        {
            EnsureProductionKeyPathIsConfigured(environment, google);
            return;
        }

        string resolvedPath = ResolveKeysPath(environment, keysPath);
        Directory.CreateDirectory(resolvedPath);
        builder.PersistKeysToFileSystem(new DirectoryInfo(resolvedPath));
    }

    private static void EnsureProductionKeyPathIsConfigured(
        IWebHostEnvironment environment,
        GoogleExternalLoginOptions google)
    {
        if (!environment.IsDevelopment() && google.HasCredentials)
        {
            string message = "Auth:DataProtectionKeysPath must be configured before production Google login.";
            throw new InvalidOperationException(message);
        }
    }

    private static string ResolveKeysPath(IWebHostEnvironment environment, string keysPath)
    {
        if (Path.IsPathRooted(keysPath))
        {
            return keysPath;
        }

        return Path.GetFullPath(Path.Combine(environment.ContentRootPath, keysPath));
    }

    private static void ConfigureCookie(CookieAuthenticationOptions options, IWebHostEnvironment environment)
    {
        options.Cookie.Name = "abhijeetsite.session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = GetSecurePolicy(environment);
        options.ExpireTimeSpan = TimeSpan.FromHours(SessionLifetimeHours);
        options.SlidingExpiration = true;
        options.LoginPath = "/api/auth/login/google";
        options.AccessDeniedPath = "/api/auth/forbidden";
    }

    private static CookieSecurePolicy GetSecurePolicy(IWebHostEnvironment environment)
    {
        return environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    }

    private static void AddGoogleIfConfigured(
        AuthenticationBuilder authentication,
        GoogleExternalLoginOptions google)
    {
        if (google.HasCredentials)
        {
            authentication.AddOAuth(IdentityAuthenticationSchemes.Google, options =>
                ConfigureGoogle(options, google));
        }
    }

    private static void ConfigureGoogle(OAuthOptions options, GoogleExternalLoginOptions google)
    {
        options.ClientId = google.ClientId;
        options.ClientSecret = google.ClientSecret;
        options.CallbackPath = "/api/auth/callback/google";
        options.AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        options.TokenEndpoint = "https://oauth2.googleapis.com/token";
        options.UserInformationEndpoint = "https://openidconnect.googleapis.com/v1/userinfo";
        options.SaveTokens = false;
        options.SignInScheme = IdentityAuthenticationSchemes.ApplicationCookie;
        options.UsePkce = true;
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Events.OnCreatingTicket = GoogleOAuthTicketHandler.HandleCreatingTicketAsync;
    }

    private static GoogleExternalLoginOptions GetGoogleOptions(IConfigurationSection authSection)
    {
        GoogleExternalLoginOptions google = new();
        authSection.GetSection(nameof(IdentityAuthenticationOptions.Google)).Bind(google);
        return google;
    }

    private static void AddIdentityAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.AuthenticatedUser, policy => policy.RequireAuthenticatedUser())
            .AddPolicy(AuthPolicies.AdminOnly, policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim(IdentityClaimTypes.IsAdmin, bool.TrueString));
    }
}
