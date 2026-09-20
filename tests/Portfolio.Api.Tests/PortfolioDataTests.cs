using Portfolio.Api.Services;

namespace Portfolio.Api.Tests;

public sealed class PortfolioDataTests
{
    [Fact]
    public void PortfolioJson_DeserializesToCompleteContent()
    {
        var dataPath = Path.Combine(AppContext.BaseDirectory, "TestData", "portfolio.json");
        using var stream = File.OpenRead(dataPath);

        var content = JsonPortfolioRepository.Deserialize(stream);

        Assert.Equal("Alireza Dayyani", content.Profile.Name);
        Assert.NotEmpty(content.Experience);
        Assert.NotEmpty(content.Skills);
        Assert.Contains(content.Projects, project => project.Slug == "tir-transport-layer");
    }

    [Fact]
    public void PortfolioJson_DoesNotUseExcludedTitle()
    {
        var dataPath = Path.Combine(AppContext.BaseDirectory, "TestData", "portfolio.json");
        var json = File.ReadAllText(dataPath);

        var excludedTitle = string.Concat("Sen", "ior");
        Assert.DoesNotContain(excludedTitle, json, StringComparison.OrdinalIgnoreCase);
    }
}
