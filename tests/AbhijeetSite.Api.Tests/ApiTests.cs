using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AbhijeetSite.Api.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetHomeSummary_ReturnsSuccessAndCorrectJson()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/home/summary");

        // Assert
        response.EnsureSuccessStatusCode(); // Assert HTTP 200 OK

        var content = await response.Content.ReadFromJsonAsync<HomeSummaryResponse>();
        Assert.NotNull(content);
        Assert.Equal("Abhijeet Haval", content.Name);
        Assert.Equal("Principal AI Architect | Enterprise AI Agents, Agent Harnesses & Skills | Microsoft Agent Framework, MCP, RAG, LangGraph, Azure AI Foundry | SaaS & Distributed Systems | .NET, Python, TypeScript", content.Headline);
        Assert.Contains("Principal Architect with 18 years of experience", content.Summary);
    }

    // Helper DTO to deserialize response for assertions
    private record HomeSummaryResponse(string Name, string Headline, string Summary);
}
