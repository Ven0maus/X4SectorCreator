using X4SectorCreator.Helpers;
using Xunit;

namespace X4SectorCreator.Tests;

public class SectorIslandAnalyzerTests
{
    [Fact]
    public void Finds_CustomSector_WithNoIncomingOrOutgoingConnections()
    {
        var sectors = new[]
        {
            new SectorIslandAnalyzer.SectorConnectivityEntry("Cluster A", "Sector A", false, ["Sector B"]),
            new SectorIslandAnalyzer.SectorConnectivityEntry("Cluster B", "Sector B", false, ["Sector A"]),
            new SectorIslandAnalyzer.SectorConnectivityEntry("Cluster C", "Sector C", false, Array.Empty<string>())
        };

        var islands = SectorIslandAnalyzer.FindIsolatedSectors(sectors);

        var island = Assert.Single(islands);
        Assert.Equal("Cluster C", island.ClusterName);
        Assert.Equal("Sector C", island.SectorName);
        Assert.Equal(0, island.IncomingConnections);
        Assert.Equal(0, island.OutgoingConnections);
    }

    [Fact]
    public void Ignores_BaseGameSector_WhenCheckingCustomSectorsOnly()
    {
        var sectors = new[]
        {
            new SectorIslandAnalyzer.SectorConnectivityEntry("Vanilla Cluster", "Vanilla Sector", true, Array.Empty<string>())
        };

        var islands = SectorIslandAnalyzer.FindIsolatedSectors(sectors, customSectorsOnly: true);

        Assert.Empty(islands);
    }

    [Fact]
    public void Counts_IncomingConnections_FromOtherSectors()
    {
        var sectors = new[]
        {
            new SectorIslandAnalyzer.SectorConnectivityEntry("Cluster A", "Sector A", false, ["Sector B"]),
            new SectorIslandAnalyzer.SectorConnectivityEntry("Cluster B", "Sector B", false, Array.Empty<string>())
        };

        var islands = SectorIslandAnalyzer.FindIsolatedSectors(sectors);

        Assert.Empty(islands);
    }
}
