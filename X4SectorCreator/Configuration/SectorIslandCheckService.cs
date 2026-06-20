using System.Text.Json;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class SectorIslandCheckService
    {
        private static ModImportResult ImportForCheck(string modDirectory, string attachedBaseModDirectory, ClusterCollection vanillaClusterData)
        {
            if (!string.IsNullOrWhiteSpace(attachedBaseModDirectory) &&
                !string.Equals(Path.GetFullPath(attachedBaseModDirectory), Path.GetFullPath(modDirectory), StringComparison.OrdinalIgnoreCase))
                return ModImportService.ImportWithMerge(attachedBaseModDirectory, modDirectory, vanillaClusterData);

            return ModImportService.IsImportableModDirectory(modDirectory)
                ? ModImportService.Import(modDirectory, vanillaClusterData)
                : ModImportService.ImportMerged(modDirectory, vanillaClusterData);
        }

        public static int Run(string modDirectory)
        {
            return Run(modDirectory, null);
        }

        public static int Run(string modDirectory, string attachedBaseModDirectory)
        {
            try
            {
                ClusterCollection vanillaClusterData = LoadVanillaClusters();
                ModImportResult importedMod = ImportForCheck(modDirectory, attachedBaseModDirectory, vanillaClusterData);

                var isolatedSectors = SectorIslandAnalyzer.FindIsolatedSectors(
                    BuildConnectivityEntries(importedMod.Clusters),
                    customSectorsOnly: true);

                Console.WriteLine($"Checked mod: {importedMod.ModName}");
                Console.WriteLine($"Path: {modDirectory}");
                Console.WriteLine();

                if (isolatedSectors.Count == 0)
                {
                    Console.WriteLine("No isolated custom sectors found.");
                    return 0;
                }

                Console.WriteLine($"Found {isolatedSectors.Count} isolated custom sector(s):");
                foreach (var sector in isolatedSectors)
                {
                    Console.WriteLine($"- {sector.SectorName} (cluster: {sector.ClusterName})");
                }

                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Sector island check failed:");
                Console.Error.WriteLine(ex.Message);
                return 2;
            }
        }

        private static ClusterCollection LoadVanillaClusters()
        {
            string json = File.ReadAllText(Constants.DataPaths.SectorMappingFilePath);
            return JsonSerializer.Deserialize<ClusterCollection>(json, ConfigSerializer.JsonSerializerOptions)
                ?? throw new InvalidOperationException("Unable to load vanilla sector mapping data.");
        }

        private static IEnumerable<SectorIslandAnalyzer.SectorConnectivityEntry> BuildConnectivityEntries(IEnumerable<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters)
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    string[] outboundSectorNames = sector.Zones
                        .SelectMany(a => a.Gates)
                        .Where(a =>
                            !string.IsNullOrWhiteSpace(a.DestinationSectorName) &&
                            !a.DestinationSectorName.Equals(sector.Name, StringComparison.OrdinalIgnoreCase))
                        .Select(a => a.DestinationSectorName)
                        .Where(a => !string.IsNullOrWhiteSpace(a))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    yield return new SectorIslandAnalyzer.SectorConnectivityEntry(
                        cluster.Name,
                        sector.Name,
                        sector.IsBaseGame,
                        outboundSectorNames);
                }
            }
        }
    }
}
