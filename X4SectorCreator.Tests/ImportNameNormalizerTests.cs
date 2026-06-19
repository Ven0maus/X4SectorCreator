using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class ImportNameNormalizerTests
{
    [Fact]
    public void EnsureImportedSectorNamesPreservingIdentity_DoesNotRenameDuplicateExistingSectorNames()
    {
        var first = new Sector { Name = "Shared Name" };
        var second = new Sector { Name = "Shared Name" };
        var cluster = new Cluster
        {
            Name = "Test Cluster",
            Sectors = [first, second],
        };

        ImportNameNormalizer.EnsureImportedSectorNamesPreservingIdentity([cluster]);

        Assert.Equal("Shared Name", cluster.Sectors[0].Name);
        Assert.Equal("Shared Name", cluster.Sectors[1].Name);
    }

    [Fact]
    public void EnsureImportedSectorNamesPreservingIdentity_FillsMissingNamesWithoutChangingExistingOnes()
    {
        var first = new Sector { Name = "Kept Name" };
        var second = new Sector { Name = null };
        var cluster = new Cluster
        {
            Name = "Fallback Cluster",
            Sectors = [first, second],
        };

        ImportNameNormalizer.EnsureImportedSectorNamesPreservingIdentity([cluster]);

        Assert.Equal("Kept Name", cluster.Sectors[0].Name);
        Assert.Equal("Fallback Cluster II", cluster.Sectors[1].Name);
    }
}
