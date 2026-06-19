namespace X4SectorCreator.Helpers
{
    internal static class ImportClusterMetadataResolver
    {
        public static string ResolveBackgroundVisualMapping(string importedContentRef, string vanillaBackgroundVisualMapping)
        {
            return string.IsNullOrWhiteSpace(importedContentRef)
                ? vanillaBackgroundVisualMapping
                : importedContentRef;
        }

        public static string ResolveSoundtrack(string importedMusicRef, string vanillaSoundtrack)
        {
            return string.IsNullOrWhiteSpace(importedMusicRef)
                ? vanillaSoundtrack
                : importedMusicRef;
        }

        public static string ResolveDescription(string importedDescription, string vanillaDescription)
        {
            return string.IsNullOrWhiteSpace(importedDescription)
                ? vanillaDescription
                : importedDescription;
        }
    }
}
