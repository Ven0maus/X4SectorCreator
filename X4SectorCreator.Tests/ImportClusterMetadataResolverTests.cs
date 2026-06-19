using X4SectorCreator.Helpers;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class ImportClusterMetadataResolverTests
{
    [Fact]
    public void ResolveBackgroundVisualMapping_PrefersImportedContentRefForBaseClusterOverrides()
    {
        string resolved = ImportClusterMetadataResolver.ResolveBackgroundVisualMapping(
            importedContentRef: "Cluster_Custom_Background",
            vanillaBackgroundVisualMapping: "Cluster_01");

        Assert.Equal("Cluster_Custom_Background", resolved);
    }

    [Fact]
    public void ResolveBackgroundVisualMapping_FallsBackToVanillaWhenImportHasNoContentRef()
    {
        string resolved = ImportClusterMetadataResolver.ResolveBackgroundVisualMapping(
            importedContentRef: null,
            vanillaBackgroundVisualMapping: "Cluster_01");

        Assert.Equal("Cluster_01", resolved);
    }

    [Fact]
    public void ResolveSoundtrack_PrefersImportedMusicRefForBaseClusterOverrides()
    {
        string resolved = ImportClusterMetadataResolver.ResolveSoundtrack(
            importedMusicRef: "music_custom_theme",
            vanillaSoundtrack: "music_soundtrack_argon");

        Assert.Equal("music_custom_theme", resolved);
    }

    [Fact]
    public void ResolveDescription_PrefersImportedDescriptionWhenPresent()
    {
        string resolved = ImportClusterMetadataResolver.ResolveDescription(
            importedDescription: "Imported cluster description",
            vanillaDescription: "Vanilla cluster description");

        Assert.Equal("Imported cluster description", resolved);
    }
}
