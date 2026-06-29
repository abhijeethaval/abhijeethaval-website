using AbhijeetSite.Api.SharedKernel.Result;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Registers identity endpoints.
/// </summary>
public static class IdentityEndpointsExtension
{
    /// <summary>
    /// Maps identity routes.
    /// </summary>
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/auth");

        group.MapGet("/login/google", GoogleLoginEndpoint.Handle)
            .AllowAnonymous()
            .WithName("LoginWithGoogle");
        group.MapGet("/me", CurrentUserEndpoint.Handle)
            .AllowAnonymous()
            .WithName("GetCurrentUser");
        group.MapPost("/logout", async (HttpContext httpContext) =>
                await LogoutEndpoint.HandleAsync(httpContext))
            .AllowAnonymous()
            .WithName("Logout");

        return app;
    }
}

internal static class GoogleLoginEndpoint
{
    private const string DefaultReturnPath = "/";

    public static IResult Handle(
        string? returnUrl,
        IOptions<IdentityAuthenticationOptions> options)
    {
        if (!options.Value.HasGoogleCredentials)
        {
            return Results.Problem(
                "Google login is not configured. Set Auth:Google:ClientId and Auth:Google:ClientSecret.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        AuthenticationProperties properties = new()
        {
            RedirectUri = NormalizeReturnUrl(returnUrl)
        };

        return Results.Challenge(properties, [IdentityAuthenticationSchemes.Google]);
    }

    private static string NormalizeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl.StartsWith("//", StringComparison.Ordinal))
        {
            return DefaultReturnPath;
        }

        return Uri.TryCreate(returnUrl, UriKind.Relative, out Uri? uri)
            ? uri.ToString()
            : DefaultReturnPath;
    }
}

internal static class CurrentUserEndpoint
{
    public static IResult Handle(HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            return Results.Ok(CurrentUserResponse.Anonymous());
        }

        Result<CurrentUserResponse> result = CurrentUserResponse.Create(httpContext.User);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error?.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
}

internal static class LogoutEndpoint
{
    public static async Task<IResult> HandleAsync(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(IdentityAuthenticationSchemes.ApplicationCookie);
        return Results.NoContent();
    }
}
