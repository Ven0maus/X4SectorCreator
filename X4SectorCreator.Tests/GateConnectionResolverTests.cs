using X4SectorCreator.Helpers;
using Xunit;

namespace X4SectorCreator.Tests;

public class GateConnectionResolverTests
{
    private sealed record Candidate(string SectorName, string SourcePath);

    [Fact]
    public void ResolvesBySourcePath_WhenSectorNamesAreDuplicated()
    {
        Candidate[] candidates =
        [
            new("President's End", "cluster_a/sector_a/zone_a/gate_a"),
            new("President's End", "cluster_b/sector_b/zone_b/gate_b")
        ];

        var lookup = GateConnectionResolver.BuildSourcePathLookup(candidates, a => a.SourcePath);

        bool found = GateConnectionResolver.TryResolveTarget(lookup, "cluster_b/sector_b/zone_b/gate_b", out Candidate target);

        Assert.True(found);
        Assert.NotNull(target);
        Assert.Equal("cluster_b/sector_b/zone_b/gate_b", target.SourcePath);
    }

    [Fact]
    public void ReturnsFalse_WhenDestinationPathIsMissing()
    {
        Candidate[] candidates = [new("The Deep", "cluster_a/sector_a/zone_a/gate_a")];
        var lookup = GateConnectionResolver.BuildSourcePathLookup(candidates, a => a.SourcePath);

        bool found = GateConnectionResolver.TryResolveTarget(lookup, "cluster_missing/sector_missing/gate", out Candidate target);

        Assert.False(found);
        Assert.Null(target);
    }
}
