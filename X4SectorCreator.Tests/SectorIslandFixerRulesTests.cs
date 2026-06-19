using System.Drawing;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Xunit;

namespace X4SectorCreator.Tests;

public class SectorIslandFixerRulesTests
{
    [Fact]
    public void UsesAcceleratorWithinClusterAndGateAcrossClusters()
    {
        Assert.Equal(Gate.GateType.props_gates_orb_accelerator_01_macro, SectorIslandFixerRules.GetConnectionType(sameCluster: true));
        Assert.Equal(Gate.GateType.props_gates_anc_gate_macro, SectorIslandFixerRules.GetConnectionType(sameCluster: false));
    }

    [Fact]
    public void TravelNodeCandidates_ReturnsExpectedCandidateCount()
    {
        var candidates = SectorIslandFixerRules.GetTravelNodeCandidates(2_000_000);

        Assert.Equal(12, candidates.Count);
    }

    [Fact]
    public void YawPointsTowardTargetDirection()
    {
        int eastYaw = SectorIslandFixerRules.CalculateYaw(new Point(0, 0), new Point(100, 0));
        int northYaw = SectorIslandFixerRules.CalculateYaw(new Point(0, 0), new Point(0, 100));

        Assert.Equal(90, eastYaw);
        Assert.Equal(0, northYaw);
    }
}
