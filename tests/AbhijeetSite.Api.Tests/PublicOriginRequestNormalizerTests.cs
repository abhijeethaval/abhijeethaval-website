using AbhijeetSite.Api.Features.Identity;
using Microsoft.AspNetCore.Http;

namespace AbhijeetSite.Api.Tests;

public sealed class PublicOriginRequestNormalizerTests
{
    private const string PublicOrigin = "https://abhijeethaval.com";
    private const string InternalHost = "abhijeetsite-api.internal:8080";

    [Fact]
    public void Normalize_AuthRequestWithPublicOrigin_UsesConfiguredOrigin()
    {
        DefaultHttpContext context = CreateContext("/api/auth/callback/google");

        PublicOriginRequestNormalizer.Normalize(context.Request, PublicOrigin);

        Assert.Equal("https", context.Request.Scheme);
        Assert.Equal("abhijeethaval.com", context.Request.Host.Value);
    }

    [Fact]
    public void Normalize_PublicOriginWithPort_PreservesConfiguredPort()
    {
        DefaultHttpContext context = CreateContext("/api/auth/login/google");

        PublicOriginRequestNormalizer.Normalize(context.Request, "https://localhost:9443");

        Assert.Equal("https", context.Request.Scheme);
        Assert.Equal("localhost:9443", context.Request.Host.Value);
    }

    [Fact]
    public void Normalize_NonAuthRequestWithPublicOrigin_KeepsOriginalOrigin()
    {
        DefaultHttpContext context = CreateContext("/api/profile");

        PublicOriginRequestNormalizer.Normalize(context.Request, PublicOrigin);

        Assert.Equal("http", context.Request.Scheme);
        Assert.Equal(InternalHost, context.Request.Host.Value);
    }

    private static DefaultHttpContext CreateContext(string path)
    {
        DefaultHttpContext context = new();
        context.Request.Scheme = "http";
        context.Request.Host = HostString.FromUriComponent(InternalHost);
        context.Request.Path = path;
        return context;
    }
}
