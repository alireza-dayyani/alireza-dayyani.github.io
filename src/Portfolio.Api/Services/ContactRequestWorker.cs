namespace Portfolio.Api.Services;

public sealed class ContactRequestWorker(
    IContactRequestQueue queue,
    ILogger<ContactRequestWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in queue.ReadAllAsync(stoppingToken))
        {
            logger.LogInformation(
                "Portfolio contact request {RequestId} from {SenderName} queued at {SubmittedAtUtc}",
                request.Id,
                request.Name,
                request.SubmittedAtUtc);
        }
    }
}
