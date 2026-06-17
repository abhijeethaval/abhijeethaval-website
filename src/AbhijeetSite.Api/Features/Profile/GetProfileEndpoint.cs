using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AbhijeetSite.Api.Features.Profile;

public static class GetProfileEndpoint
{
    public static IResult Handle()
    {
        ProfileResponse response = ProfileContentProvider.GetProfile();
        return Results.Ok(response);
    }
}

public static class ProfileEndpointsExtension
{
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/profile", GetProfileEndpoint.Handle)
           .WithName("GetProfile");

        return app;
    }
}
