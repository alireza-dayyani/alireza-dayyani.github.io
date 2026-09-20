namespace Portfolio.Api.Models;

public sealed record PortfolioContent(
    Profile Profile,
    IReadOnlyList<Metric> Metrics,
    IReadOnlyList<SkillGroup> Skills,
    IReadOnlyList<Experience> Experience,
    IReadOnlyList<Project> Projects,
    IReadOnlyList<Education> Education,
    IReadOnlyList<Language> Languages);

public sealed record Profile(
    string Name,
    string Title,
    string Location,
    string Email,
    string Phone,
    string Summary,
    string GitHubUrl,
    string? LinkedInUrl,
    string Availability);

public sealed record Metric(string Value, string Label);

public sealed record SkillGroup(string Category, IReadOnlyList<string> Items);

public sealed record Experience(
    string Role,
    string Company,
    string Location,
    string Start,
    string End,
    string Summary,
    IReadOnlyList<string> Achievements);

public sealed record Project(
    string Slug,
    string Name,
    string Eyebrow,
    string Description,
    IReadOnlyList<string> Highlights,
    IReadOnlyList<string> Technologies);

public sealed record Education(string Qualification, string Institution);

public sealed record Language(string Name, string Proficiency);
