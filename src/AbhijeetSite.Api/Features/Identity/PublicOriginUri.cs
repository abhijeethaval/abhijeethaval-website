namespace AbhijeetSite.Api.Features.Identity;

internal static class PublicOriginUri
{
    public static Uri Parse(string publicOrigin)
    {
        if (!Uri.TryCreate(publicOrigin, UriKind.Absolute, out Uri? origin))
        {
            throw new InvalidOperationException("Auth:PublicOrigin must be an absolute URI.");
        }

        EnsureOriginOnly(origin);
        return origin;
    }

    private static void EnsureOriginOnly(Uri origin)
    {
        bool hasPath = origin.AbsolutePath != "/";
        bool hasQueryOrFragment =
            !string.IsNullOrEmpty(origin.Query) || !string.IsNullOrEmpty(origin.Fragment);
        if (hasPath || hasQueryOrFragment)
        {
            throw new InvalidOperationException(
                "Auth:PublicOrigin must include only scheme, host, and optional port.");
        }
    }
}
