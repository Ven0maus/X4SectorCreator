using DevConsole.PatchHelpers;
using X4SectorCreator.Configuration;

namespace DevConsole
{
    /// <summary>
    /// This console is not included in the X4SectorCreator release, its just a helper tool for development.
    /// </summary>
    internal class Program
    {
        private static readonly string _readPath = "DevData";
        private static readonly string _resultsPath = $"{_readPath}/Results";

        private static void Main()
        {
            MapPatcher.Patch();
            return;

            EnsureDirectoriesExist();

            // Vanilla gate connection mappings
            VanillaGateConnectionParser.GenerateGateConnectionMappings(_readPath, _resultsPath);
        }

        private static void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(_readPath))
            {
                _ = Directory.CreateDirectory(_readPath);
            }

            if (!Directory.Exists(_resultsPath))
            {
                _ = Directory.CreateDirectory(_resultsPath);
            }
        }
    }
}
