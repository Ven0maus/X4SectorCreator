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
        private const string _vanillaFilesPath = "vanillafiles";

        private static void Main()
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Enter path for X4 Foundations extracted folder:");
            Console.ForegroundColor = originalColor;
            var x4Path = Console.ReadLine();
            if (!Directory.Exists(Path.GetDirectoryName(x4Path)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid path.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Started extracting regions");
            Console.ForegroundColor = originalColor;

            // First run map patcher
            MapPatcher.Patch(x4Path);

            // Base files
            var clustersPath = Path.Combine(_vanillaFilesPath, "clusters.xml");
            var sectorsPath = Path.Combine(_vanillaFilesPath, "sectors.xml");
            var regionDefinitionsPath = Path.Combine(_vanillaFilesPath, "region_definitions.xml");
            var godPath = Path.Combine(_vanillaFilesPath, "god.xml");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Started extracting regions");
            Console.ForegroundColor = originalColor;

            if (File.Exists(clustersPath) && File.Exists(regionDefinitionsPath))
            {
                RegionExtractor.ExtractRegions(clustersPath, regionDefinitionsPath);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Started extracting stations");
            Console.ForegroundColor = originalColor;

            if (File.Exists(godPath)) 
            {
                StationExtractor.ExtractStations(clustersPath, sectorsPath, godPath);
            }

            /* TO EXTRACT GATE CONNECTIONS YOU MUST SETUP DevData FOLDER AS SUCH (IF ANY MISSING YOU WILL GET A PROMPT WHICH ONES ARE MISSING): 
             - DevData
               - extensions
                 - ego_dlc_boron
                   - maps
                     - xu_ep2_universe
                       - all vanilla map files of this dlc
                 - and all other dlcs...
               - maps
                 - xu_ep2_universe
                   - base vanilla map files
             */

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Do you wish to create gate mappings for vanilla too, this requires manual setup first (Y/N):");
            Console.ForegroundColor = originalColor;
            var yn = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(yn) && yn.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Started creating vanilla gate connections mapping");
                Console.ForegroundColor = originalColor;

                EnsureDirectoriesExist();

                // Vanilla gate connection mappings
                VanillaGateConnectionParser.GenerateGateConnectionMappings(_readPath, _resultsPath);
            }
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
