using System.Drawing;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class ImportedGateSectorResolverTests
{
    [Fact]
    public void TryFindGateReferenceBySourcePath_ResolvesCorrectSectorWhenNamesDuplicate()
    {
        Gate targetGate = new()
        {
            SourcePath = "cluster_a/sector_shared_a/zone_a/gate_a",
            DestinationPath = "cluster_c/sector_origin/zone_c/gate_c",
            ParentSectorName = "Shared",
            DestinationSectorName = "Origin",
        };
        Gate reverseGate = new()
        {
            SourcePath = "cluster_c/sector_origin/zone_c/gate_c",
            DestinationPath = "cluster_a/sector_shared_a/zone_a/gate_a",
            ParentSectorName = "Origin",
            DestinationSectorName = "Shared",
        };

        Sector duplicateSectorA = new()
        {
            Id = 1,
            Name = "Shared",
            Zones = [new Zone { Id = 1, Position = new Point(0, 0), Gates = [targetGate] }],
        };
        Sector originSector = new()
        {
            Id = 2,
            Name = "Origin",
            Zones = [new Zone { Id = 1, Position = new Point(0, 0), Gates = [reverseGate] }],
        };
        Sector duplicateSectorB = new()
        {
            Id = 3,
            Name = "Shared",
            Zones = [new Zone { Id = 1, Position = new Point(0, 0) }],
        };

        List<Cluster> clusters =
        [
            new() { Id = 1, Name = "C1", Sectors = [duplicateSectorA] },
            new() { Id = 2, Name = "C2", Sectors = [duplicateSectorB] },
            new() { Id = 3, Name = "C3", Sectors = [originSector] },
        ];

        bool found = ImportedGateSectorResolver.TryFindGateReferenceBySourcePath(clusters, targetGate.SourcePath, out ImportedGateSectorResolver.GateReference reference);

        Assert.True(found);
        Assert.Same(duplicateSectorA, reference.Sector);
        Assert.Same(targetGate, reference.Gate);
    }

    [Fact]
    public void GateTargetsSector_UsesDestinationPathInsteadOfDestinationSectorName()
    {
        Gate targetGate = new()
        {
            SourcePath = "cluster_a/sector_shared_a/zone_a/gate_a",
            DestinationPath = "cluster_c/sector_origin/zone_c/gate_c",
            ParentSectorName = "Shared",
            DestinationSectorName = "Origin",
        };
        Gate reverseGate = new()
        {
            SourcePath = "cluster_c/sector_origin/zone_c/gate_c",
            DestinationPath = "cluster_a/sector_shared_a/zone_a/gate_a",
            ParentSectorName = "Origin",
            DestinationSectorName = "Shared",
        };

        Sector actualTarget = new()
        {
            Id = 1,
            Name = "Shared",
            Zones = [new Zone { Id = 1, Position = new Point(0, 0), Gates = [targetGate] }],
        };
        Sector originSector = new()
        {
            Id = 2,
            Name = "Origin",
            Zones = [new Zone { Id = 1, Position = new Point(0, 0), Gates = [reverseGate] }],
        };
        Sector wrongDuplicate = new()
        {
            Id = 3,
            Name = "Shared",
            Zones = [new Zone { Id = 1, Position = new Point(0, 0) }],
        };

        List<Cluster> clusters =
        [
            new() { Id = 1, Name = "C1", Sectors = [actualTarget] },
            new() { Id = 2, Name = "C2", Sectors = [wrongDuplicate] },
            new() { Id = 3, Name = "C3", Sectors = [originSector] },
        ];

        Assert.True(ImportedGateSectorResolver.GateTargetsSector(clusters, targetGate, originSector));
        Assert.False(ImportedGateSectorResolver.GateTargetsSector(clusters, reverseGate, wrongDuplicate));
    }
}
