namespace AbhijeetSite.Api.Features.Profile;

public record ProfileResponse(
    string Name,
    string Headline,
    string Summary,
    IReadOnlyList<string> About,
    IReadOnlyList<string> Expertise,
    IReadOnlyList<ExperienceResponse> Experiences,
    IReadOnlyList<EducationResponse> Educations);

public record ExperienceResponse(
    string Company,
    string Location,
    IReadOnlyList<RoleResponse> Roles);

public record RoleResponse(
    string Title,
    string StartDate,
    string EndDate,
    string Summary,
    IReadOnlyList<string> Achievements,
    IReadOnlyList<string> FocusAreas);

public record EducationResponse(
    string Institution,
    string Credential,
    string Years,
    IReadOnlyList<string> Activities);
