using AbhijeetSite.Api.Features.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace AbhijeetSite.Api.Tests;

public sealed class GoogleOAuthRedirectHandlerTests
{
    private const string AuthorizationUri =
        "https://accounts.google.com/o/oauth2/v2/auth"
        + "?client_id=client"
        + "&redirect_uri=http%3A%2F%2Fabhijeethaval.com%2Fapi%2Fauth%2Fcallback%2Fgoogle"
        + "&response_type=code";
    private const string CallbackPath = "/api/auth/callback/google";
    private const string PublicCallbackUri = "https://abhijeethaval.com/api/auth/callback/google";
    private const string PublicOrigin = "https://abhijeethaval.com";

    [Fact]
    public void RewriteRedirectUri_PublicOriginConfigured_UsesHttpsPublicCallback()
    {
        string rewrittenUri = GoogleOAuthRedirectHandler.RewriteRedirectUri(
            AuthorizationUri,
            PublicOrigin,
            new PathString(CallbackPath));

        string redirectUri = ReadQueryParameter(rewrittenUri, "redirect_uri");
        Assert.Equal(PublicCallbackUri, redirectUri);
    }

    [Fact]
    public void RewriteRedirectUri_PublicOriginMissing_ReturnsOriginalUri()
    {
        string rewrittenUri = GoogleOAuthRedirectHandler.RewriteRedirectUri(
            AuthorizationUri,
            string.Empty,
            new PathString(CallbackPath));

        Assert.Equal(AuthorizationUri, rewrittenUri);
    }

    private static string ReadQueryParameter(string uri, string parameterName)
    {
        Uri parsedUri = new(uri);
        Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(parsedUri.Query);
        return query[parameterName].ToString();
    }
}
