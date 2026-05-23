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
        Assert.Equal("Principal Software Architect", content.Headline);
        Assert.Contains("Personal technical website foundation", content.Summary);
    }

    // Helper DTO to deserialize response for assertions
    private record HomeSummaryResponse(string Name, string Headline, string Summary);
}
