using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddOutputCache();
builder.Services.AddResponseCompression();

builder.Services.AddSingleton<IPortfolioRepository, JsonPortfolioRepository>();
builder.Services.AddSingleton<IContactRequestQueue, ContactRequestQueue>();
builder.Services.AddHostedService<ContactRequestWorker>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("contact", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var app = builder.Build();

app.UseExceptionHandler();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseResponseCompression();
app.UseRateLimiter();
app.UseOutputCache();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapOpenApi();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            checkedAtUtc = DateTimeOffset.UtcNow
        });
    }
});
app.MapPortfolioApi();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
