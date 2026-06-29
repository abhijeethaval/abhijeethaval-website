namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Registers identity middleware.
/// </summary>
public static class IdentityApplicationBuilderExtensions
{
    /// <summary>
    /// Normalizes authentication requests to the configured public origin.
    /// </summary>
    public static IApplicationBuilder UseIdentityPublicOrigin(this IApplicationBuilder app)
    {
        return app.UseMiddleware<PublicOriginRequestMiddleware>();
    }
}
