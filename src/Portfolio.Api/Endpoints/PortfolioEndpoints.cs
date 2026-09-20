using Microsoft.AspNetCore.Http.HttpResults;
using Portfolio.Api.Models;
using Portfolio.Api.Services;

namespace Portfolio.Api.Endpoints;

public static class PortfolioEndpoints
{
    public static IEndpointRouteBuilder MapPortfolioApi(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api")
            .WithTags("Portfolio");

        api.MapGet("/portfolio", (IPortfolioRepository repository) => repository.Get())
            .WithName("GetPortfolio")
            .WithSummary("Returns the complete portfolio content model.")
            .CacheOutput(policy => policy.Expire(TimeSpan.FromMinutes(10)));

        api.MapGet("/projects", (IPortfolioRepository repository) => repository.Get().Projects)
            .WithName("GetProjects")
            .WithSummary("Returns selected engineering projects.")
            .CacheOutput(policy => policy.Expire(TimeSpan.FromMinutes(10)));

        api.MapPost("/contact", HandleContactAsync)
            .WithName("SubmitContactRequest")
            .WithSummary("Validates and queues a portfolio contact request.")
            .RequireRateLimiting("contact");

        return endpoints;
    }

    private static async Task<Results<Accepted<ContactAcceptedResponse>, ValidationProblem>> HandleContactAsync(
        ContactRequest request,
        IContactRequestQueue queue,
        CancellationToken cancellationToken)
    {
        var errors = ContactRequestValidator.Validate(request);
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var queuedRequest = new QueuedContactRequest(
            Guid.NewGuid(),
            request.Name!.Trim(),
            request.Email!.Trim(),
            request.Message!.Trim(),
            DateTimeOffset.UtcNow);

        await queue.EnqueueAsync(queuedRequest, cancellationToken);

        return TypedResults.Accepted(
            uri: (string?)null,
            value: new ContactAcceptedResponse(queuedRequest.Id, "queued"));
    }
}
