using System.Text.Json;
using System.Xml.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class SectorTranslationFixService
    {
        public static int Run(string targetModDirectory, string attachedBaseModDirectory)
        {
            try
            {
                if (!ModImportService.IsImportableModDirectory(targetModDirectory))
                    throw new InvalidOperationException("Sector translation fix target must be a single importable X4 extension folder.");

                ClusterCollection vanillaClusterData = LoadVanillaClusters();
                ModImportResult importedMod = !string.IsNullOrWhiteSpace(attachedBaseModDirectory) &&
                    !string.Equals(Path.GetFullPath(attachedBaseModDirectory), Path.GetFullPath(targetModDirectory), StringComparison.OrdinalIgnoreCase)
                    ? ModImportService.ImportWithMerge(attachedBaseModDirectory, targetModDirectory, vanillaClusterData)
                    : ModImportService.Import(targetModDirectory, vanillaClusterData);

                Dictionary<string, string> resolvedNames = BuildResolvedNameLookup(importedMod.Clusters);
                FixSummary summary = ApplyUnifiedPageFix(targetModDirectory, resolvedNames);

                Console.WriteLine("Unified sector translation fix complete.");
                Console.WriteLine($"Target: {targetModDirectory}");
                if (!string.IsNullOrWhiteSpace(attachedBaseModDirectory) && !string.Equals(Path.GetFullPath(attachedBaseModDirectory), Path.GetFullPath(targetModDirectory), StringComparison.OrdinalIgnoreCase))
                    Console.WriteLine($"Attached base: {attachedBaseModDirectory}");
                Console.WriteLine($"Unified page id: {summary.PageId}");
                Console.WriteLine($"Updated datasets: {summary.UpdatedDatasetCount}");
                Console.WriteLine($"Written translation file: {summary.TranslationFilePath}");

                if (summary.SkippedMacros.Count > 0)
                {
                    Console.WriteLine("Skipped unresolved macros:");
                    foreach (string skipped in summary.SkippedMacros)
                        Console.WriteLine($"- {skipped}");
                }

                return summary.SkippedMacros.Count == 0 ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Unified sector translation fix failed:");
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

        private static Dictionary<string, string> BuildResolvedNameLookup(IEnumerable<Cluster> clusters)
        {
            Dictionary<string, string> lookup = new(StringComparer.OrdinalIgnoreCase);

            foreach (Cluster cluster in clusters)
            {
                if (!string.IsNullOrWhiteSpace(cluster.ImportedMacroName) &&
                    !string.IsNullOrWhiteSpace(cluster.Name) &&
                    !string.Equals(cluster.Name, ModImportService.MissingTranslationDisplayName, StringComparison.Ordinal))
                {
                    lookup[cluster.ImportedMacroName] = cluster.Name;
                }

                foreach (Sector sector in cluster.Sectors)
                {
                    if (!string.IsNullOrWhiteSpace(sector.ImportedMacroName) &&
                        !string.IsNullOrWhiteSpace(sector.Name) &&
                        !string.Equals(sector.Name, ModImportService.MissingTranslationDisplayName, StringComparison.Ordinal))
                    {
                        lookup[sector.ImportedMacroName] = sector.Name;
                    }
                }
            }

            return lookup;
        }

        private static FixSummary ApplyUnifiedPageFix(string targetModDirectory, Dictionary<string, string> resolvedNames)
        {
            string mapDefaultsPath = Path.Combine(targetModDirectory, "libraries", "mapdefaults.xml");
            if (!File.Exists(mapDefaultsPath))
                throw new FileNotFoundException("Missing libraries/mapdefaults.xml in target mod.", mapDefaultsPath);

            XDocument mapDefaultsDocument = XDocument.Load(mapDefaultsPath);
            List<(string MacroName, string Name)> replacements = [];
            List<string> skippedMacros = [];

            foreach (XElement dataset in mapDefaultsDocument.Descendants("dataset"))
            {
                string macroName = (string)dataset.Attribute("macro");
                if (string.IsNullOrWhiteSpace(macroName))
                    continue;

                XElement identification = dataset.Element("properties")?.Element("identification");
                if (identification?.Attribute("name") == null)
                    continue;

                if (resolvedNames.TryGetValue(macroName, out string resolvedName) && !string.IsNullOrWhiteSpace(resolvedName))
                {
                    replacements.Add((macroName, resolvedName));
                }
                else
                {
                    skippedMacros.Add(macroName);
                }
            }

            int pageId = Localisation.GetFnvHash($"{Path.GetFileName(targetModDirectory)} unified sector translations");
            int textId = 1;
            Dictionary<string, int> textIdsByMacro = new(StringComparer.OrdinalIgnoreCase);
            foreach ((string macroName, _) in replacements)
            {
                textIdsByMacro[macroName] = textId++;
            }

            foreach (XElement dataset in mapDefaultsDocument.Descendants("dataset"))
            {
                string macroName = (string)dataset.Attribute("macro");
                if (string.IsNullOrWhiteSpace(macroName) || !textIdsByMacro.TryGetValue(macroName, out int unifiedTextId))
                    continue;

                XElement identification = dataset.Element("properties")?.Element("identification");
                identification?.SetAttributeValue("name", $"{{{pageId},{unifiedTextId}}}");
            }

            mapDefaultsDocument.Save(mapDefaultsPath);

            string tDirectory = Path.Combine(targetModDirectory, "t");
            Directory.CreateDirectory(tDirectory);
            string translationPath = Path.Combine(tDirectory, "sector_name_fix.xml");

            XElement page = new("page",
                new XAttribute("id", pageId),
                new XAttribute("title", "unified_sector_translation_fix"),
                new XAttribute("descr", "Unified sector translation fix"),
                new XAttribute("voice", "no"),
                replacements.Select(a => new XElement("t", new XAttribute("id", textIdsByMacro[a.MacroName]), a.Name)));

            XDocument translationDocument = new(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("language", page));

            translationDocument.Save(translationPath);

            return new FixSummary(pageId, replacements.Count, translationPath, skippedMacros);
        }

        private sealed record FixSummary(int PageId, int UpdatedDatasetCount, string TranslationFilePath, List<string> SkippedMacros);
    }
}
