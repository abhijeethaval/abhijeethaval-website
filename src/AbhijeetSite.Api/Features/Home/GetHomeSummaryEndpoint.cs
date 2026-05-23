using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AbhijeetSite.Api.Features.Home;

public static class GetHomeSummaryEndpoint
{
    public static IResult Handle()
    {
        var response = new HomeSummaryResponse(
            "Abhijeet Haval",
            "Principal Software Architect",
            "Personal technical website foundation built with .NET Aspire, ASP.NET Core Minimal APIs, React, and vertical slices."
        );
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
