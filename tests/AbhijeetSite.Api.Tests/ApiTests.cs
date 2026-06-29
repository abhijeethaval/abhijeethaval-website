using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AbhijeetSite.Api.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string ArchitectStartDate = "Jan 2020";
    private const string ArchitectTitle = "Architect";
    private const string AssociateArchitectStartDate = "Jun 2018";
    private const string AssociateArchitectTitle = "Associate Architect";
    private const string EnterpriseApiSummarySignal = "enterprise APIs";
    private const string ForwardDeployedHeadlineSignal = "forward-deployed AI solutions";
    private const string IcertisCompany = "Icertis";
    private const string KitInstitutionSignal = "K.I.T";
    private const string KpmgPocSignal = "KPMG document extraction POC";
    private const string LatestExperienceSignal = "17 years";
    private const string LatestHeadlineSignal = "Principal Applied AI Architect";
    private const string McpA2AExpertise = "MCP and A2A interoperability";
    private const string MojibakeArtifact = "\u00C2";
    private const string ProfileName = "Abhijeet Haval";
    private const string VivekanandInstitutionSignal = "Vivekanand";

    private readonly WebApplicationFactory<Program> _factory;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetHomeSummary_CurrentProfile_ReturnsLatestResumeSummary()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/home/summary");

        // Assert
        response.EnsureSuccessStatusCode(); // Assert HTTP 200 OK

        HomeSummaryResponse? content = await response.Content.ReadFromJsonAsync<HomeSummaryResponse>();
        Assert.NotNull(content);
        Assert.Equal(ProfileName, content.Name);
        Assert.Contains(LatestHeadlineSignal, content.Headline);
        Assert.Contains(LatestExperienceSignal, content.Summary);
        Assert.Contains(EnterpriseApiSummarySignal, content.Summary);
    }

    [Fact]
    public async Task GetProfile_CurrentProfile_ReturnsLatestResumeExperience()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/profile");

        // Assert
        response.EnsureSuccessStatusCode();

        ProfileResponse? content = await response.Content.ReadFromJsonAsync<ProfileResponse>();
        Assert.NotNull(content);
        Assert.Equal(ProfileName, content.Name);
        Assert.Contains(ForwardDeployedHeadlineSignal, content.Headline);
        Assert.Contains(content.Expertise, item => item == McpA2AExpertise);
        ExperienceResponse icertis = Assert.Single(
            content.Experiences,
            item => item.Company == IcertisCompany);
        Assert.Contains(
            icertis.Roles,
            role => role.Title == ArchitectTitle && role.StartDate == ArchitectStartDate);
        Assert.Contains(
            icertis.Roles,
            role => role.Title == AssociateArchitectTitle && role.StartDate == AssociateArchitectStartDate);
        Assert.Contains(
            icertis.Roles,
            role => role.Achievements.Any(item => item.Contains(KpmgPocSignal)));
        Assert.Contains(content.Educations, item => item.Institution.Contains(KitInstitutionSignal));
        Assert.Contains(content.Educations, item => item.Institution.Contains(VivekanandInstitutionSignal));
        Assert.DoesNotContain(content.About, item => item.Contains(MojibakeArtifact));
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
