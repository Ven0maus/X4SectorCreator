using System.Drawing;

namespace X4SectorCreator.Forms
{
    public static class GalaxySettingsForm
    {
        public static bool IsCustomGalaxy { get; set; }

        public static string GalaxyName { get; set; } = "xu_ep2_universe";
    }

    public sealed class MainForm
    {
        public static MainForm Instance { get; set; } = new();

        public Dictionary<string, string> DlcMappings { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}

namespace X4SectorCreator.Helpers
{
    public static class SectorGenerationTestExtensions
    {
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return char.ToUpperInvariant(input[0]) + input[1..];
        }
    }
}

namespace X4SectorCreator.Objects
{
}
