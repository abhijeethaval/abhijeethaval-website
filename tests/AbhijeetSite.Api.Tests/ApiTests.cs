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
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/home/summary");

        // Assert
        response.EnsureSuccessStatusCode(); // Assert HTTP 200 OK

        HomeSummaryResponse? content = await response.Content.ReadFromJsonAsync<HomeSummaryResponse>();
        Assert.NotNull(content);
        Assert.Equal("Abhijeet Haval", content.Name);
        Assert.Contains("Agentic AI", content.Headline);
        Assert.Contains("enterprise-scale distributed systems", content.Summary);
    }

    [Fact]
    public async Task GetProfile_ReturnsCuratedProfileContent()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/profile");

        // Assert
        response.EnsureSuccessStatusCode();

        ProfileResponse? content = await response.Content.ReadFromJsonAsync<ProfileResponse>();
        Assert.NotNull(content);
        Assert.Equal("Abhijeet Haval", content.Name);
        Assert.Contains("Generative AI", content.Headline);
        Assert.Contains(content.Experiences, item => item.Company == "Icertis");
        Assert.Contains(content.Educations, item => item.Institution.Contains("K.I.T"));
        Assert.Contains(content.Educations, item => item.Institution.Contains("Vivekanand"));
        Assert.DoesNotContain(content.About, item => item.Contains("Â"));
    }

    // Helper DTO to deserialize response for assertions
    private record HomeSummaryResponse(string Name, string Headline, string Summary);

    private record ProfileResponse(
        string Name,
        string Headline,
        string Summary,
        string[] About,
        string[] Expertise,
        ExperienceResponse[] Experiences,
        EducationResponse[] Educations);

    private record ExperienceResponse(
        string Company,
        string Location,
        RoleResponse[] Roles);

    private record RoleResponse(
        string Title,
        string StartDate,
        string EndDate,
        string Summary,
        string[] Achievements,
        string[] FocusAreas);

    private record EducationResponse(
        string Institution,
        string Credential,
        string Years,
        string[] Activities);
}
