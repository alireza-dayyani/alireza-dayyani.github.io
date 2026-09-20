namespace Portfolio.Api.Models;

public sealed record ContactRequest(string? Name, string? Email, string? Message);

public sealed record QueuedContactRequest(
    Guid Id,
    string Name,
    string Email,
    string Message,
    DateTimeOffset SubmittedAtUtc);

public sealed record ContactAcceptedResponse(Guid Id, string Status);
