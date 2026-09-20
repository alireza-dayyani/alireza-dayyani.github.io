using System.Text.Json;
using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public interface IPortfolioRepository
{
    PortfolioContent Get();
}

public sealed class JsonPortfolioRepository : IPortfolioRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly PortfolioContent _content;

    public JsonPortfolioRepository(IWebHostEnvironment environment)
    {
        var dataPath = Path.Combine(environment.WebRootPath, "data", "portfolio.json");

        using var stream = File.OpenRead(dataPath);
        _content = Deserialize(stream);
    }

    public PortfolioContent Get() => _content;

    public static PortfolioContent Deserialize(Stream stream) =>
        JsonSerializer.Deserialize<PortfolioContent>(stream, SerializerOptions)
        ?? throw new InvalidDataException("Portfolio data could not be loaded.");
}
