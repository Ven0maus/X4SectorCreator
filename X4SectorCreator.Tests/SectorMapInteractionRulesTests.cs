using X4SectorCreator.Helpers;
using Xunit;

namespace X4SectorCreator.Tests;

public class SectorMapInteractionRulesTests
{
    [Fact]
    public void GateNodeHitRadius_UsesTighterSelectionWindow()
    {
        float hitRadius = SectorMapInteractionRules.GetGateNodeHitRadius(zoom: 1f, gateSizeRadius: 8f);

        Assert.Equal(14f, hitRadius);
        Assert.True(hitRadius < 42f);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void MultiSectorDragging_UsesCanonicalLayout(int sectorCount)
    {
        Assert.True(SectorMapInteractionRules.UseCanonicalSectorDragLayout(sectorCount));
    }

    [Fact]
    public void ClusterMove_PreservesExistingChildSectorLayout()
    {
        Assert.True(SectorMapInteractionRules.PreserveChildSectorLayoutAfterClusterMove());
    }
}
