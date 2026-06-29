using Microsoft.Extensions.Options;

namespace AbhijeetSite.Api.Features.Identity;

internal sealed class PublicOriginRequestMiddleware
{
    private readonly RequestDelegate _next;

    public PublicOriginRequestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IOptions<IdentityAuthenticationOptions> options)
    {
        PublicOriginRequestNormalizer.Normalize(context.Request, options.Value.PublicOrigin);
        await _next(context);
    }
}

internal static class PublicOriginRequestNormalizer
{
    private static readonly PathString AuthPathPrefix = new("/api/auth");

    public static void Normalize(HttpRequest request, string publicOrigin)
    {
        if (string.IsNullOrWhiteSpace(publicOrigin))
        {
            return;
        }

        if (!request.Path.StartsWithSegments(AuthPathPrefix))
        {
            return;
        }

        Uri origin = PublicOriginUri.Parse(publicOrigin);
        request.Scheme = origin.Scheme;
        request.Host = ToHostString(origin);
    }

    private static HostString ToHostString(Uri origin)
    {
        return origin.IsDefaultPort
            ? new HostString(origin.Host)
            : new HostString(origin.Host, origin.Port);
    }
}
