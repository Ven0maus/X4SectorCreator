using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ModulesGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            var moduleEditsElements = CollectModuleEdits().ToArray();
            if (moduleEditsElements == null || moduleEditsElements.Length == 0) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    moduleEditsElements
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/modules.xml")));
        }

        private static IEnumerable<XElement> CollectModuleEdits()
        {
            var raceModules = CollectRaceModules();
            Module[] allModulesFlat = null;

            // Foreach new faction, add the faction to the modules of the matching race
            foreach (var faction in FactionsForm.AllCustomFactions)
            {
                var race = faction.Value.Primaryrace;
                var modules = raceModules[race].ToHashSet();

                // Check if faction owns factory that produces spaceweed, spacefuel, maja snails, maja dust
                // Then add these missing modules if they don't exist yet
                var illegalModules = CollectIllegalModules(faction.Value);
                if (illegalModules.Count > 0)
                {
                    var missingModules = illegalModules
                        .Where(a => !modules.Any(b => b.CategoryObj?.Ware != null && b.CategoryObj.Ware.Equals(a, StringComparison.OrdinalIgnoreCase)))
                        .ToArray();
                    if (missingModules.Length > 0)
                    {
                        allModulesFlat ??= raceModules.Values.SelectMany(a => a).ToArray();
                        foreach (var missingModule in missingModules)
                        {
                            var misModule = allModulesFlat.First(a => a.CategoryObj?.Ware != null && a.CategoryObj.Ware.Equals(missingModule, StringComparison.OrdinalIgnoreCase));
                            modules.Add(misModule);
                        }
                    }
                }

                // Include sojabeans and sojahusk if faction has a freeport or piratedock station
                var requiresSojabeansAndHusk = MainForm.Instance.AllClusters.Values
                    .SelectMany(a => a.Sectors)
                    .SelectMany(a => a.Zones)
                    .SelectMany(a => a.Stations)
                    .Where(a => a.Owner == faction.Value.Id)
                    .Any(a => a.Type.Equals("piratedock", StringComparison.OrdinalIgnoreCase) || 
                        a.Type.Equals("freeport", StringComparison.OrdinalIgnoreCase));
                if (requiresSojabeansAndHusk)
                {
                    allModulesFlat ??= raceModules.Values.SelectMany(a => a).ToArray();
                    var sojaBeansModule = allModulesFlat.First(a => a.CategoryObj?.Ware != null && a.CategoryObj.Ware.Equals("sojabeans", StringComparison.OrdinalIgnoreCase));
                    var sojaHuskModule = allModulesFlat.First(a => a.CategoryObj?.Ware != null && a.CategoryObj.Ware.Equals("sojahusk", StringComparison.OrdinalIgnoreCase));
                    modules.Add(sojaBeansModule);
                    modules.Add(sojaHuskModule);
                }

                foreach (var module in modules)
                {
                    // Skip player only modules
                    if (module.CategoryObj.Faction.Equals("[player]") ||
                        module.CategoryObj.Faction.Equals("player"))
                    {
                        continue;
                    }

                    // Create a new extension element for the faction attribute
                    yield return new XElement("add", 
                        new XAttribute("sel", $"/modules/module[@id='{module.Id}']/category"),
                        new XElement("extension", 
                        new XAttribute("faction", $"[{faction.Value.Id}]")));
                }
            }
        }

        private static HashSet<string> CollectIllegalModules(Faction faction)
        {
            var illegalWares = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "spaceweed", "majadust", "majasnails", "spacefuel" };
            var foundWares = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var factory in FactoriesForm.AllFactories.Values)
            {
                if (factory.Owner.Equals(faction.Id, StringComparison.OrdinalIgnoreCase))
                {
                    if (factory.Module?.Select?.Ware != null && illegalWares.Contains(factory.Module.Select.Ware))
                    {
                        foundWares.Add(factory.Module.Select.Ware);
                    }
                }
            }
            return foundWares;
        }

        private static Dictionary<string, HashSet<Module>> CollectRaceModules()
        {
            var factionModules = new Dictionary<string, HashSet<Module>>(StringComparer.OrdinalIgnoreCase);

            // Load all data from modules_mappings
            var xml = File.ReadAllText(Constants.DataPaths.ModulesMappingsPath);
            var modules = Modules.Deserialize(xml).Module;
            
            // For each race, combine their modules in a dictionary
            foreach (var module in modules)
            {
                var races = ParseMultiField(module.CategoryObj.Race);
                var extensionRaces = module.CategoryObj.Extension?.SelectMany(a => ParseMultiField(a.Race)) ?? [];
                foreach (var race in extensionRaces)
                    races.Add(race);

                foreach (var race in races)
                {
                    if (!factionModules.TryGetValue(race, out var moduleList))
                    {
                        factionModules[race] = moduleList = [];
                    }
                    moduleList.Add(module);
                }
            }

            return factionModules;
        }

        private static HashSet<string> ParseMultiField(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return [];

            // Remove brackets if present
            value = value.Trim();
            if (value.StartsWith('[') && value.EndsWith(']'))
            {
                value = value[1..^1];
            }

            // Split and add to HashSet
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in value.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                result.Add(r.Trim());
            }

            return result;
        }

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }
    }
}
