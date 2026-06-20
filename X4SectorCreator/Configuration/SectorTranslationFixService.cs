namespace X4SectorCreator.Configuration
{
    internal static class SectorTranslationFixService
    {
        public static int Run(string targetModDirectory, string attachedBaseModDirectory)
        {
            Console.Error.WriteLine("Unified sector translation fix failed:");
            Console.Error.WriteLine("Unified sector translation now belongs to Generate MOD output and no longer patches imported source mods.");
            Console.Error.WriteLine("Use Generate MOD to produce localized XML instead of mutating the imported extension folders.");
            return 2;
        }
    }
}
