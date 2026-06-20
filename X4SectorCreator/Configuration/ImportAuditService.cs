using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class ImportAuditService
    {
        public static int RunIncludedFiles(string targetPath, string attachedBaseModDirectory)
        {
            try
            {
                ImportInclusionReport report = BuildInclusionReport(targetPath, attachedBaseModDirectory);

                Console.WriteLine("Import inclusion audit:");
                Console.WriteLine($"Target: {targetPath}");
                if (!string.IsNullOrWhiteSpace(report.AttachedBaseModDirectory))
                    Console.WriteLine($"Attached base: {report.AttachedBaseModDirectory}");
                Console.WriteLine();

                Console.WriteLine($"Included mod directories: {report.IncludedMods.Count}");
                foreach (IncludedModReport mod in report.IncludedMods)
                {
                    Console.WriteLine($"- {mod.ModDirectory}");
                    if (mod.IsAttachedBase)
                        Console.WriteLine("  role: attached base");
                    Console.WriteLine($"  content.xml: {mod.ContentXmlPath}");
                    Console.WriteLine($"  mapdefaults: {(mod.MapDefaultsPath ?? "<none>")}");
                    Console.WriteLine($"  included map XML files: {mod.MapXmlFiles.Count}");
                    foreach (string file in mod.MapXmlFiles)
                        Console.WriteLine($"    map: {file}");
                    Console.WriteLine($"  included translation XML files: {mod.TranslationXmlFiles.Count}");
                    foreach (string file in mod.TranslationXmlFiles)
                        Console.WriteLine($"    translation: {file}");

                    if (mod.IgnoredXmlFiles.Count > 0)
                    {
                        Console.WriteLine($"  ignored XML files: {mod.IgnoredXmlFiles.Count}");
                        foreach (string file in mod.IgnoredXmlFiles)
                            Console.WriteLine($"    ignored: {file}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"Ignored nested/duplicate mod directories: {report.IgnoredModDirectories.Count}");
                foreach (string ignored in report.IgnoredModDirectories)
                    Console.WriteLine($"- {ignored}");

                return report.IgnoredModDirectories.Count > 0 || report.IncludedMods.Any(a => a.IgnoredXmlFiles.Count > 0) ? 1 : 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Import inclusion audit failed:");
                Console.Error.WriteLine(ex.Message);
                return 2;
            }
        }

        public static int RunSectorNameList(string modDirectory, string attachedBaseModDirectory)
        {
            try
            {
                ClusterCollection vanillaClusterData = LoadVanillaClusters();
                ModImportResult importedMod = ImportForAudit(modDirectory, attachedBaseModDirectory, vanillaClusterData);

                Console.WriteLine($"Imported sector names: {importedMod.ModName}");
                Console.WriteLine($"Path: {modDirectory}");
                if (!string.IsNullOrWhiteSpace(attachedBaseModDirectory) &&
                    !string.Equals(Path.GetFullPath(attachedBaseModDirectory), Path.GetFullPath(modDirectory), StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Attached base: {attachedBaseModDirectory}");
                }
                Console.WriteLine();

                foreach (Cluster cluster in importedMod.Clusters.OrderBy(a => a.Name ?? a.ImportedMacroName, StringComparer.OrdinalIgnoreCase))
                {
                    string clusterDisplayName = cluster.Name ?? ModImportService.MissingTranslationDisplayName;
                    string clusterMacro = cluster.ImportedMacroName ?? cluster.BaseGameMapping ?? "<unknown>";
                    Console.WriteLine($"[Cluster] {clusterDisplayName}");
                    Console.WriteLine($"  macro: {clusterMacro}");

                    foreach (Sector sector in cluster.Sectors.OrderBy(a => a.Name ?? a.ImportedMacroName, StringComparer.OrdinalIgnoreCase))
                    {
                        string sectorDisplayName = sector.Name ?? ModImportService.MissingTranslationDisplayName;
                        string sectorMacro = sector.ImportedMacroName ?? sector.BaseGameMapping ?? "<unknown>";
                        Console.WriteLine($"  - {sectorDisplayName}");
                        Console.WriteLine($"    macro: {sectorMacro}");
                    }

                    Console.WriteLine();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Sector name list failed:");
                Console.Error.WriteLine(ex.Message);
                return 2;
            }
        }

        private static ModImportResult ImportForAudit(string modDirectory, string attachedBaseModDirectory, ClusterCollection vanillaClusterData)
        {
            if (!string.IsNullOrWhiteSpace(attachedBaseModDirectory) &&
                !string.Equals(Path.GetFullPath(attachedBaseModDirectory), Path.GetFullPath(modDirectory), StringComparison.OrdinalIgnoreCase))
                return ModImportService.ImportWithMerge(attachedBaseModDirectory, modDirectory, vanillaClusterData);

            return ModImportService.IsImportableModDirectory(modDirectory)
                ? ModImportService.Import(modDirectory, vanillaClusterData)
                : ModImportService.ImportMerged(modDirectory, vanillaClusterData);
        }

        private static ModImportResult ImportForNameAudit(string modDirectory, string attachedBaseModDirectory, ClusterCollection vanillaClusterData)
        {
            if (!string.IsNullOrWhiteSpace(attachedBaseModDirectory) &&
                !string.Equals(Path.GetFullPath(attachedBaseModDirectory), Path.GetFullPath(modDirectory), StringComparison.OrdinalIgnoreCase))
                return ModImportService.ImportWithMergeForNameResolution(attachedBaseModDirectory, modDirectory, vanillaClusterData);

            return ModImportService.IsImportableModDirectory(modDirectory)
                ? ModImportService.ImportForNameResolution(modDirectory, vanillaClusterData)
                : ModImportService.ImportMergedForNameResolution(modDirectory, vanillaClusterData);
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
                ModImportResult importedMod = ImportForNameAudit(modDirectory, attachedBaseModDirectory, vanillaClusterData);
                Dictionary<string, NameResolutionTrace> traces = BuildNameResolutionTraces(modDirectory);

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

                Console.WriteLine();
                Console.WriteLine("Name resolution trace:");
                foreach ((string macroName, string finalName) in importedMod.Clusters
                    .SelectMany(cluster =>
                        cluster.Sectors.Select(sector => (sector.ImportedMacroName, sector.Name))
                            .Append((cluster.ImportedMacroName, cluster.Name)))
                    .Where(a => !string.IsNullOrWhiteSpace(a.ImportedMacroName))
                    .DistinctBy(a => a.ImportedMacroName, StringComparer.OrdinalIgnoreCase)
                    .OrderBy(a => a.ImportedMacroName, StringComparer.OrdinalIgnoreCase))
                {
                    if (!traces.TryGetValue(macroName, out NameResolutionTrace trace))
                        continue;

                    Console.WriteLine($"- macro: {macroName}");
                    Console.WriteLine($"  name ref: {trace.NameReference ?? "<none>"}");
                    Console.WriteLine($"  local entry: {trace.LocalTranslationEntry ?? "<none>"}");
                    Console.WriteLine($"  resolved text: {trace.ResolvedTranslationText ?? "<none>"}");
                    Console.WriteLine($"  imported/displayed: {finalName ?? "<null>"}");
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
                ModImportResult importedMod = ImportForNameAudit(modDirectory, attachedBaseModDirectory, vanillaClusterData);

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

        private static ImportInclusionReport BuildInclusionReport(string targetPath, string attachedBaseModDirectory)
        {
            string fullTargetPath = Path.GetFullPath(targetPath);
            string fullAttachedBasePath = string.IsNullOrWhiteSpace(attachedBaseModDirectory)
                ? null
                : Path.GetFullPath(attachedBaseModDirectory);

            List<string> targetIncludedMods = ModImportService.DiscoverMergeImportDirectories(fullTargetPath);
            List<string> includedMods = [];
            if (!string.IsNullOrWhiteSpace(fullAttachedBasePath) &&
                ModImportService.IsImportableModDirectory(fullAttachedBasePath) &&
                !targetIncludedMods.Contains(fullAttachedBasePath, StringComparer.OrdinalIgnoreCase))
            {
                includedMods.Add(fullAttachedBasePath);
            }
            includedMods.AddRange(targetIncludedMods);

            List<string> targetCandidates = FindAllImportableModDirectories(fullTargetPath);
            List<string> ignoredModDirectories = targetCandidates
                .Where(a => !targetIncludedMods.Contains(a, StringComparer.OrdinalIgnoreCase))
                .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                .ToList();

            List<IncludedModReport> includedModReports = includedMods
                .Select(a => BuildIncludedModReport(a, string.Equals(a, fullAttachedBasePath, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return new ImportInclusionReport(includedModReports, ignoredModDirectories, fullAttachedBasePath);
        }

        private static IncludedModReport BuildIncludedModReport(string modDirectory, bool isAttachedBase)
        {
            string fullModDirectory = Path.GetFullPath(modDirectory);
            string contentXmlPath = Path.Combine(fullModDirectory, "content.xml");
            string mapDefaultsPath = Path.Combine(fullModDirectory, "libraries", "mapdefaults.xml");
            string mapsRoot = Path.Combine(fullModDirectory, "maps");
            string tRoot = Path.Combine(fullModDirectory, "t");

            List<string> mapXmlFiles = Directory.Exists(mapsRoot)
                ? Directory.GetFiles(mapsRoot, "*.xml", SearchOption.AllDirectories)
                    .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                    .ToList()
                : [];
            List<string> translationXmlFiles = Directory.Exists(tRoot)
                ? Directory.GetFiles(tRoot, "*.xml", SearchOption.AllDirectories)
                    .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                    .ToList()
                : [];

            HashSet<string> includedXmlFiles = new(StringComparer.OrdinalIgnoreCase);
            if (File.Exists(contentXmlPath))
                includedXmlFiles.Add(contentXmlPath);
            if (File.Exists(mapDefaultsPath))
                includedXmlFiles.Add(mapDefaultsPath);
            foreach (string file in mapXmlFiles)
                includedXmlFiles.Add(file);
            foreach (string file in translationXmlFiles)
                includedXmlFiles.Add(file);

            List<string> ignoredXmlFiles = Directory.GetFiles(fullModDirectory, "*.xml", SearchOption.AllDirectories)
                .Where(a => !includedXmlFiles.Contains(a))
                .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new IncludedModReport(
                fullModDirectory,
                isAttachedBase,
                contentXmlPath,
                File.Exists(mapDefaultsPath) ? mapDefaultsPath : null,
                mapXmlFiles,
                translationXmlFiles,
                ignoredXmlFiles);
        }

        private static List<string> FindAllImportableModDirectories(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
                return [];

            List<string> candidates = [];
            if (ModImportService.IsImportableModDirectory(rootDirectory))
                candidates.Add(Path.GetFullPath(rootDirectory));

            candidates.AddRange(Directory
                .EnumerateFiles(rootDirectory, "content.xml", SearchOption.AllDirectories)
                .Select(Path.GetDirectoryName)
                .Where(a => !string.IsNullOrWhiteSpace(a) && ModImportService.IsImportableModDirectory(a))
                .Select(Path.GetFullPath));

            return candidates
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                .ToList();
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

        private static Dictionary<string, NameResolutionTrace> BuildNameResolutionTraces(string modDirectory)
        {
            Dictionary<string, NameResolutionTrace> traces = new(StringComparer.OrdinalIgnoreCase);
            string mapDefaultsPath = Path.Combine(modDirectory, "libraries", "mapdefaults.xml");
            if (!File.Exists(mapDefaultsPath))
                return traces;

            Dictionary<(int pageId, int textId), string> localEntries = LoadLocalTranslationEntries(modDirectory);
            XDocument document = XDocument.Load(mapDefaultsPath);

            foreach (XElement dataset in document.Descendants("dataset"))
            {
                string macroName = (string)dataset.Attribute("macro");
                if (string.IsNullOrWhiteSpace(macroName))
                    continue;

                string nameRef = (string)dataset.Element("properties")?.Element("identification")?.Attribute("name");
                if (string.IsNullOrWhiteSpace(nameRef))
                    continue;

                string localEntry = null;
                string resolvedText = null;
                if (TryParseTranslationReference(nameRef, out int pageId, out int textId) && localEntries.TryGetValue((pageId, textId), out string rawEntry))
                {
                    localEntry = rawEntry;
                    resolvedText = ResolveLocalTranslationEntry(pageId, textId, localEntries);
                }
                else
                {
                    resolvedText = ImportTranslationTextHelper.Normalize(nameRef);
                }

                traces[macroName] = new NameResolutionTrace(nameRef, localEntry, resolvedText);
            }

            return traces;
        }

        private static Dictionary<(int pageId, int textId), string> LoadLocalTranslationEntries(string modDirectory)
        {
            Dictionary<(int pageId, int textId), string> entries = new();
            string translationsPath = Path.Combine(modDirectory, "t");
            if (!Directory.Exists(translationsPath))
                return entries;

            foreach (string file in Directory.GetFiles(translationsPath, "*.xml", SearchOption.TopDirectoryOnly).OrderBy(a => a, StringComparer.OrdinalIgnoreCase))
            {
                XDocument document = XDocument.Load(file);
                foreach (XElement page in document.Descendants("page"))
                {
                    if (!int.TryParse((string)page.Attribute("id"), out int pageId))
                        continue;

                    foreach (XElement text in page.Elements("t"))
                    {
                        if (!int.TryParse((string)text.Attribute("id"), out int textId))
                            continue;

                        string value = string.Concat(text.Nodes().OfType<XText>().Select(a => a.Value));
                        value = text.Value + value;
                        if (!string.IsNullOrWhiteSpace(value))
                            entries[(pageId, textId)] = value;
                    }
                }
            }

            return entries;
        }

        private static string ResolveLocalTranslationEntry(int pageId, int textId, Dictionary<(int pageId, int textId), string> entries)
        {
            return ResolveLocalTranslationEntry(pageId, textId, entries, new HashSet<(int pageId, int textId)>());
        }

        private static string ResolveLocalTranslationEntry(int pageId, int textId, Dictionary<(int pageId, int textId), string> entries, HashSet<(int pageId, int textId)> seen)
        {
            if (!entries.TryGetValue((pageId, textId), out string raw) || !seen.Add((pageId, textId)))
                return null;

            string resolved = Regex.Replace(raw, "\\{\\s*(\\d+)\\s*,\\s*(\\d+)\\s*\\}", match =>
            {
                int nestedPage = int.Parse(match.Groups[1].Value);
                int nestedText = int.Parse(match.Groups[2].Value);
                return ResolveLocalTranslationEntry(nestedPage, nestedText, entries, seen) ?? string.Empty;
            });

            return ImportTranslationTextHelper.Normalize(resolved);
        }

        private static bool TryParseTranslationReference(string reference, out int pageId, out int textId)
        {
            pageId = 0;
            textId = 0;
            Match match = Regex.Match(reference ?? string.Empty, @"^\s*\{\s*(\d+)\s*,\s*(\d+)\s*\}\s*$");
            return match.Success && int.TryParse(match.Groups[1].Value, out pageId) && int.TryParse(match.Groups[2].Value, out textId);
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

        private sealed record NameResolutionTrace(string NameReference, string LocalTranslationEntry, string ResolvedTranslationText);

        private sealed record ImportInclusionReport(List<IncludedModReport> IncludedMods, List<string> IgnoredModDirectories, string AttachedBaseModDirectory);

        private sealed record IncludedModReport(
            string ModDirectory,
            bool IsAttachedBase,
            string ContentXmlPath,
            string MapDefaultsPath,
            List<string> MapXmlFiles,
            List<string> TranslationXmlFiles,
            List<string> IgnoredXmlFiles);
    }
}
