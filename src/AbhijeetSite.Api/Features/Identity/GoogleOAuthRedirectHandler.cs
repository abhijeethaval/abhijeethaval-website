using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace AbhijeetSite.Api.Features.Identity;

/// <summary>
/// Rewrites Google authorization redirects to use the configured public callback origin.
/// </summary>
public static class GoogleOAuthRedirectHandler
{
    private const string RedirectUriParameterName = "redirect_uri";

    /// <summary>
    /// Rewrites the authorization URL redirect URI when a public origin is configured.
    /// </summary>
    public static string RewriteRedirectUri(
        string authorizationRedirectUri,
        string publicOrigin,
        PathString callbackPath)
    {
        if (string.IsNullOrWhiteSpace(publicOrigin))
        {
            return authorizationRedirectUri;
        }

        string publicCallbackUri = BuildPublicCallbackUri(publicOrigin, callbackPath);
        UriBuilder authorizationUri = new(authorizationRedirectUri);
        IReadOnlyList<KeyValuePair<string, string?>> parameters =
            RewriteQueryParameters(authorizationUri.Query, publicCallbackUri);
        QueryString queryString = QueryString.Create(parameters);
        authorizationUri.Query = TrimQueryPrefix(queryString);
        return authorizationUri.Uri.ToString();
    }

    private static string BuildPublicCallbackUri(string publicOrigin, PathString callbackPath)
    {
        if (!Uri.TryCreate(publicOrigin, UriKind.Absolute, out Uri? origin))
        {
            throw new InvalidOperationException("Auth:PublicOrigin must be an absolute URI.");
        }

        UriBuilder builder = new(origin)
        {
            Path = callbackPath.Value ?? string.Empty,
            Query = string.Empty,
            Fragment = string.Empty
        };

        return builder.Uri.ToString();
    }

    private static IReadOnlyList<KeyValuePair<string, string?>> RewriteQueryParameters(
        string query,
        string publicCallbackUri)
    {
        List<KeyValuePair<string, string?>> parameters = [];
        foreach (KeyValuePair<string, StringValues> parameter in QueryHelpers.ParseQuery(query))
        {
            AddParameterValues(parameters, parameter, publicCallbackUri);
        }

        return parameters;
    }

    private static void AddParameterValues(
        List<KeyValuePair<string, string?>> parameters,
        KeyValuePair<string, StringValues> parameter,
        string publicCallbackUri)
    {
        foreach (string? value in parameter.Value)
        {
            string? rewrittenValue = parameter.Key == RedirectUriParameterName
                ? publicCallbackUri
                : value;
            parameters.Add(new KeyValuePair<string, string?>(parameter.Key, rewrittenValue));
        }
    }

    private static string TrimQueryPrefix(QueryString queryString)
    {
        string query = queryString.Value ?? string.Empty;
        return query.StartsWith("?", StringComparison.Ordinal) ? query[1..] : query;
    }
}
