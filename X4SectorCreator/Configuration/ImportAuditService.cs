using System.Text.Json;
using System.Xml.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class ImportAuditService
    {
        private static ModImportResult ImportForAudit(string modDirectory, string attachedBaseModDirectory, ClusterCollection vanillaClusterData)
        {
            if (!string.IsNullOrWhiteSpace(attachedBaseModDirectory) &&
                !string.Equals(Path.GetFullPath(attachedBaseModDirectory), Path.GetFullPath(modDirectory), StringComparison.OrdinalIgnoreCase))
                return ModImportService.ImportWithMerge(attachedBaseModDirectory, modDirectory, vanillaClusterData);

            return ModImportService.IsImportableModDirectory(modDirectory)
                ? ModImportService.Import(modDirectory, vanillaClusterData)
                : ModImportService.ImportMerged(modDirectory, vanillaClusterData);
        }

        public static int RunNameResolution(string modDirectory)
        {
            return RunNameResolution(modDirectory, null);
        }

        public static int RunNameResolution(string modDirectory, string attachedBaseModDirectory)
        {
            try
            {
                ClusterCollection vanillaClusterData = LoadVanillaClusters();
                ModImportResult importedMod = ImportForAudit(modDirectory, attachedBaseModDirectory, vanillaClusterData);

                List<string> unresolvedWarnings = importedMod.Warnings
                    .Where(a => a.StartsWith("Unresolved sector/cluster name reference", StringComparison.OrdinalIgnoreCase) ||
                                a.StartsWith("Imported cluster name was null", StringComparison.OrdinalIgnoreCase) ||
                                a.StartsWith("Imported sector name was null", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Console.WriteLine($"Sector/cluster name resolution audit: {importedMod.ModName}");
                Console.WriteLine($"Path: {modDirectory}");
                Console.WriteLine();

                int clusterCount = importedMod.Clusters.Count;
                int sectorCount = importedMod.Clusters.SelectMany(a => a.Sectors).Count();
                Console.WriteLine($"Imported clusters: {clusterCount}");
                Console.WriteLine($"Imported sectors: {sectorCount}");
                Console.WriteLine($"Name warnings: {unresolvedWarnings.Count}");
                Console.WriteLine();

                if (unresolvedWarnings.Count == 0)
                {
                    Console.WriteLine("No sector/cluster name resolution issues detected.");
                    return 0;
                }

                Console.WriteLine("Name warnings:");
                foreach (string warning in unresolvedWarnings)
                {
                    Console.WriteLine($"- {warning}");
                }

                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Sector/cluster name resolution audit failed:");
                Console.Error.WriteLine(ex.Message);
                return 2;
            }
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
                ModImportResult importedMod = ImportForAudit(modDirectory, attachedBaseModDirectory, vanillaClusterData);

                SourceAuditSummary sourceSummary = AnalyzeSourceMod(modDirectory);
                ImportedAuditSummary importedSummary = AnalyzeImportedMod(importedMod);
                GateValidationSummary gateValidation = ValidateImportedGates(importedMod.Clusters);

                Console.WriteLine($"Import audit: {importedMod.ModName}");
                Console.WriteLine($"Path: {modDirectory}");
                Console.WriteLine();

                Console.WriteLine("Imported graph:");
                Console.WriteLine($"- Clusters: {importedSummary.ClusterCount}");
                Console.WriteLine($"- Sectors: {importedSummary.SectorCount}");
                Console.WriteLine($"- Zones: {importedSummary.ZoneCount}");
                Console.WriteLine($"- Gates: {importedSummary.GateCount}");
                Console.WriteLine($"- Custom clusters: {importedSummary.CustomClusterCount}");
                Console.WriteLine($"- Custom sectors: {importedSummary.CustomSectorCount}");
                Console.WriteLine($"- Duplicate sector names preserved: {importedSummary.DuplicateSectorNameCount}");
                Console.WriteLine($"- Clusters with background visuals: {importedSummary.ClustersWithBackgroundVisuals}");
                Console.WriteLine($"- Clusters with soundtrack: {importedSummary.ClustersWithSoundtrack}");
                Console.WriteLine($"- Sectors with owner: {importedSummary.SectorsWithOwner}");
                Console.WriteLine($"- Sectors with resource areas: {importedSummary.SectorsWithResourceAreas}");
                Console.WriteLine($"- Unresolved reverse gate paths: {gateValidation.InvalidGateCount}");
                Console.WriteLine();

                Console.WriteLine("Source metadata detected:");
                Console.WriteLine($"- mapdefaults datasets: {sourceSummary.DatasetCount}");
                Console.WriteLine($"- image refs: {sourceSummary.ImageRefCount}");
                Console.WriteLine($"- music refs: {sourceSummary.MusicRefCount}");
                Console.WriteLine($"- descriptions: {sourceSummary.DescriptionCount}");
                Console.WriteLine($"- owner attrs: {sourceSummary.OwnerCount}");
                Console.WriteLine($"- sunlight attrs: {sourceSummary.SunlightCount}");
                Console.WriteLine($"- economy attrs: {sourceSummary.EconomyCount}");
                Console.WriteLine($"- security attrs: {sourceSummary.SecurityCount}");
                Console.WriteLine($"- factionlogic attrs: {sourceSummary.FactionLogicCount}");
                Console.WriteLine($"- allowrandomanomaly tags: {sourceSummary.AllowRandomAnomalyCount}");
                Console.WriteLine($"- resource areas: {sourceSummary.ResourceAreaCount}");
                Console.WriteLine($"- translation files: {sourceSummary.TranslationFileCount}");
                Console.WriteLine();

                List<string> warnings = BuildWarnings(sourceSummary, importedSummary, gateValidation);
                if (warnings.Count == 0)
                {
                    Console.WriteLine("No obvious import fidelity gaps detected by the built-in audit.");
                    return 0;
                }

                Console.WriteLine("Warnings:");
                foreach (string warning in warnings)
                {
                    Console.WriteLine($"- {warning}");
                }

                if (gateValidation.InvalidGateCount > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Sample unresolved reverse paths:");
                    foreach (Gate gate in gateValidation.InvalidGates.Take(10))
                    {
                        Console.WriteLine($"- {gate.ParentSectorName} -> {gate.DestinationSectorName}");
                        Console.WriteLine($"  Reverse path not found: {gate.DestinationPath}");
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Import audit failed:");
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

        private static SourceAuditSummary AnalyzeSourceMod(string modDirectory)
        {
            string mapDefaultsPath = Path.Combine(modDirectory, "libraries", "mapdefaults.xml");
            int datasets = 0;
            int imageRefs = 0;
            int musicRefs = 0;
            int descriptions = 0;
            int owners = 0;
            int sunlight = 0;
            int economy = 0;
            int security = 0;
            int factionLogic = 0;
            int allowRandomAnomaly = 0;
            int resourceAreas = 0;

            if (File.Exists(mapDefaultsPath))
            {
                XDocument document = XDocument.Load(mapDefaultsPath);
                foreach (XElement dataset in document.Root?.Elements("dataset") ?? [])
                {
                    datasets++;
                    XElement properties = dataset.Element("properties");
                    if (properties == null)
                        continue;

                    XElement identification = properties.Element("identification");
                    if (identification?.Attribute("image") != null)
                        imageRefs++;
                    if (identification?.Attribute("description") != null)
                        descriptions++;

                    XElement music = properties.Element("music") ?? properties.Element("system")?.Element("music");
                    if (music?.Attribute("ref") != null)
                        musicRefs++;

                    XElement area = properties.Element("area");
                    if (area != null)
                    {
                        owners += area.Attribute("owner") != null ? 1 : 0;
                        sunlight += area.Attribute("sunlight") != null ? 1 : 0;
                        economy += area.Attribute("economy") != null ? 1 : 0;
                        security += area.Attribute("security") != null ? 1 : 0;
                        factionLogic += area.Attribute("factionlogic") != null ? 1 : 0;
                        allowRandomAnomaly += ((string)area.Attribute("tags"))?.Contains("allowrandomanomaly", StringComparison.OrdinalIgnoreCase) == true ? 1 : 0;
                    }

                    resourceAreas += properties.Elements("resourceareas").Elements("resourcearea").Count();
                }
            }

            int translationFiles = 0;
            string translationsPath = Path.Combine(modDirectory, "t");
            if (Directory.Exists(translationsPath))
            {
                translationFiles = Directory.GetFiles(translationsPath, "*.xml", SearchOption.TopDirectoryOnly).Length;
            }

            return new SourceAuditSummary(
                datasets,
                imageRefs,
                musicRefs,
                descriptions,
                owners,
                sunlight,
                economy,
                security,
                factionLogic,
                allowRandomAnomaly,
                resourceAreas,
                translationFiles);
        }

        private static ImportedAuditSummary AnalyzeImportedMod(ModImportResult importedMod)
        {
            List<Cluster> clusters = importedMod.Clusters;
            List<Sector> sectors = clusters.SelectMany(a => a.Sectors).ToList();
            List<Zone> zones = sectors.SelectMany(a => a.Zones).ToList();
            List<Gate> gates = zones.SelectMany(a => a.Gates).ToList();

            int duplicateSectorNames = sectors
                .Where(a => !string.IsNullOrWhiteSpace(a.Name))
                .GroupBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
                .Count(a => a.Count() > 1);

            return new ImportedAuditSummary(
                clusters.Count,
                sectors.Count,
                zones.Count,
                gates.Count,
                clusters.Count(a => !a.IsBaseGame),
                sectors.Count(a => !a.IsBaseGame),
                duplicateSectorNames,
                clusters.Count(a => !string.IsNullOrWhiteSpace(a.BackgroundVisualMapping)),
                clusters.Count(a => !string.IsNullOrWhiteSpace(a.Soundtrack)),
                sectors.Count(a => !string.IsNullOrWhiteSpace(a.Owner)),
                sectors.Count(a => a.ResourceAreas.Count > 0));
        }

        private static List<string> BuildWarnings(SourceAuditSummary source, ImportedAuditSummary imported, GateValidationSummary gateValidation)
        {
            List<string> warnings = [];

            if (source.ImageRefCount > 0 && imported.ClustersWithBackgroundVisuals == 0)
                warnings.Add("Source mod contains cluster/sector image metadata, but imported clusters have no background visuals.");

            if (source.MusicRefCount > 0 && imported.ClustersWithSoundtrack == 0)
                warnings.Add("Source mod contains music metadata, but imported clusters have no soundtrack values.");

            if (source.OwnerCount > 0 && imported.SectorsWithOwner == 0)
                warnings.Add("Source mod contains sector owner metadata, but no sector owner values were imported.");

            if (source.ResourceAreaCount > 0 && imported.SectorsWithResourceAreas == 0)
                warnings.Add("Source mod contains resource areas, but no imported sectors have resource area data.");

            if (source.TranslationFileCount > 1)
                warnings.Add("Source mod contains multiple translation files; verify non-English translation selection manually.");

            if (imported.DuplicateSectorNameCount > 0)
                warnings.Add($"Imported graph contains {imported.DuplicateSectorNameCount} duplicate sector name group(s); linkage must remain path-based.");

            if (gateValidation.InvalidGateCount > 0)
                warnings.Add($"Imported graph still contains {gateValidation.InvalidGateCount} gate(s) whose reverse destination path could not be resolved.");

            return warnings;
        }

        private static GateValidationSummary ValidateImportedGates(IEnumerable<Cluster> clusters)
        {
            List<Gate> gates = clusters
                .SelectMany(a => a.Sectors)
                .SelectMany(a => a.Zones)
                .SelectMany(a => a.Gates)
                .Where(a => !string.IsNullOrWhiteSpace(a.SourcePath))
                .ToList();

            Dictionary<string, Gate> sourcePathLookup = GateConnectionResolver.BuildSourcePathLookup(gates, a => a.SourcePath);
            List<Gate> invalid = [];
            HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
            foreach (Gate gate in gates)
            {
                if (string.IsNullOrWhiteSpace(gate.DestinationPath) || !seen.Add(gate.SourcePath))
                    continue;

                if (!GateConnectionResolver.TryResolveTarget(sourcePathLookup, gate.DestinationPath, out Gate _))
                {
                    invalid.Add(gate);
                }
            }

            return new GateValidationSummary(invalid.Count, invalid);
        }

        private sealed record SourceAuditSummary(
            int DatasetCount,
            int ImageRefCount,
            int MusicRefCount,
            int DescriptionCount,
            int OwnerCount,
            int SunlightCount,
            int EconomyCount,
            int SecurityCount,
            int FactionLogicCount,
            int AllowRandomAnomalyCount,
            int ResourceAreaCount,
            int TranslationFileCount);

        private sealed record ImportedAuditSummary(
            int ClusterCount,
            int SectorCount,
            int ZoneCount,
            int GateCount,
            int CustomClusterCount,
            int CustomSectorCount,
            int DuplicateSectorNameCount,
            int ClustersWithBackgroundVisuals,
            int ClustersWithSoundtrack,
            int SectorsWithOwner,
            int SectorsWithResourceAreas);

        private sealed record GateValidationSummary(int InvalidGateCount, List<Gate> InvalidGates);
    }
}
