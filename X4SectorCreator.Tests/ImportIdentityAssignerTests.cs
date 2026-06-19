using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class ImportIdentityAssignerTests
{
    [Fact]
    public void AssignImportedIdsPreservingOrder_KeepsCustomClusterImportOrder()
    {
        var firstCluster = new Cluster { Name = "Zulu" };
        var secondCluster = new Cluster { Name = "Alpha" };

        ImportIdentityAssigner.AssignImportedIdsPreservingOrder(
            [firstCluster, secondCluster],
            maxClusterId: 41,
            _ => throw new NotImplementedException(),
            (_, _) => throw new NotImplementedException(),
            (_, _) => throw new NotImplementedException(),
            (_, _, _) => throw new NotImplementedException());

        Assert.Equal(42, firstCluster.Id);
        Assert.Equal(43, secondCluster.Id);
    }

    [Fact]
    public void AssignImportedIdsPreservingOrder_KeepsCustomSectorImportOrderWithinCluster()
    {
        var firstSector = new Sector { Name = "Zulu" };
        var secondSector = new Sector { Name = "Alpha" };
        var cluster = new Cluster
        {
            Id = 7,
            Name = "Test Cluster",
            Sectors = [firstSector, secondSector],
        };

        ImportIdentityAssigner.AssignImportedIdsPreservingOrder(
            [cluster],
            maxClusterId: 100,
            _ => throw new NotImplementedException(),
            (_, _) => throw new NotImplementedException(),
            (_, _) => throw new NotImplementedException(),
            (_, _, _) => throw new NotImplementedException());

        Assert.Equal(1, cluster.Sectors[0].Id);
        Assert.Equal("Zulu", cluster.Sectors[0].Name);
        Assert.Equal(2, cluster.Sectors[1].Id);
        Assert.Equal("Alpha", cluster.Sectors[1].Name);
    }
}
