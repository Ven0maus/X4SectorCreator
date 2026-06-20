using X4SectorCreator.Helpers;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class SectorIslandReportBuilderTests
{
    [Fact]
    public void GetResolvedIslandsCount_UsesRemainingIslandsNotConnectionCount()
    {
        var remaining = Array.Empty<SectorIslandAnalyzer.SectorIslandResult>();

        int resolved = SectorIslandReportBuilder.GetResolvedIslandsCount(80, remaining);

        Assert.Equal(80, resolved);
    }

    [Fact]
    public void BuildUnresolvedIslandReport_ListsEveryRemainingIslandClearly()
    {
        var remaining = new[]
        {
            new SectorIslandAnalyzer.SectorIslandResult("Cluster A", "Matrix Prototype II", 0, 0),
            new SectorIslandAnalyzer.SectorIslandResult("Cluster B", "Radiant Haven", 0, 0)
        };

        string report = SectorIslandReportBuilder.BuildUnresolvedIslandReport(
            "Uncharted Skies",
            islandsDetected: 12,
            islandsFixed: 10,
            remaining);

        Assert.Contains("Mod: Uncharted Skies", report);
        Assert.Contains("Islands detected: 12", report);
        Assert.Contains("Islands fixed: 10", report);
        Assert.Contains("Unresolved islands: 2", report);
        Assert.Contains("- Matrix Prototype II (cluster: Cluster A)", report);
        Assert.Contains("- Radiant Haven (cluster: Cluster B)", report);
    }
}
