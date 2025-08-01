using DevConsole.Extractors;
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
            /*
            Console.WriteLine("Enter clusters.xml directory path: ");
            var path = Console.ReadLine();
            var clustersPath = Path.Combine(path, "clusters.xml");
            var regionDefinitionsPath = Path.Combine(path, "region_definitions.xml");
            if (File.Exists(clustersPath))
            {
                RegionExtractor.ExtractRegions(clustersPath, regionDefinitionsPath);
            }
            */
            
            //MapPatcher.Patch();

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
