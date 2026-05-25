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
            "Principal AI Architect | Enterprise AI Agents, Agent Harnesses & Skills | Microsoft Agent Framework, MCP, RAG, LangGraph, Azure AI Foundry | SaaS & Distributed Systems | .NET, Python, TypeScript",
            "Principal Architect with 18 years of experience building enterprise distributed systems across C#/.NET, Python, Azure, and SaaS platforms. Currently leading agentic AI architecture on the Icertis ICI platform, focused on enterprise AI agents, agent harnesses, skills, MCP, RAG, Microsoft Agent Framework, LangGraph, and LLM trust infrastructure. Owner of three production frameworks supporting ~55 engineers across partner platform, GovCon, and agentic AI workstreams. Designed the Contract Composer agent, now in enterprise beta, and established the architecture pattern for five domain agents on ICI Business APIs."
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
