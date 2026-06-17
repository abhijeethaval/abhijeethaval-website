using AbhijeetSite.Api.Features.Profile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AbhijeetSite.Api.Features.Home;

public static class GetHomeSummaryEndpoint
{
    public static IResult Handle()
    {
        ProfileResponse profile = ProfileContentProvider.GetProfile();
        HomeSummaryResponse response = new(profile.Name, profile.Headline, profile.Summary);
        return Results.Ok(response);
    }
}

public static class HomeEndpointsExtension
{
    public static IEndpointRouteBuilder MapHomeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/home/summary", GetHomeSummaryEndpoint.Handle)
           .WithName("GetHomeSummary");

        return app;
    }
}
