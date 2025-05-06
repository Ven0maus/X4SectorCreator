using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class WaresGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            var wares = CollectWares().ToArray();
            if (wares == null || wares.Length == 0) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    wares
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/wares.xml")));
        }

        private static readonly Dictionary<string, string> _illegalWareMapping = new(StringComparer.OrdinalIgnoreCase)
        {
            {"module_tel_prod_spaceweed_01", "spaceweed" },
            {"module_arg_prod_spacefuel_01", "spacefuel" },
            {"module_par_prod_majasnails_01", "majasnails" },
            {"module_par_prod_majadust_01", "majadust" }
        };

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

        private static IEnumerable<XElement> CollectWares()
        {
            var xml = File.ReadAllText(Constants.DataPaths.WaresMappingPath);
            var allWares = Wares.Deserialize(xml).Ware;
            var illegalWaresCache = new Dictionary<Faction, HashSet<string>>();

            // Add respective owners to wares
            foreach (var ware in allWares)
            {
                var wareElement = new XElement("add", new XAttribute("sel", $"/wares/ware[@id='{ware.Id}']"));
                int count = 0;
                foreach (var faction in FactionsForm.AllCustomFactions)
                {
                    if (ware.OwnerObj != null && ware.OwnerObj
                        .Any(a => a.Faction != null && a.Faction
                            .Equals(faction.Value.Primaryrace, StringComparison.OrdinalIgnoreCase)))
                    {
                        wareElement.Add(new XElement("owner", new XAttribute("faction", faction.Value.Id)));
                        count++;
                    }
                    else
                    {
                        // Include faction in illegal wares if they use them
                        if (_illegalWareMapping.TryGetValue(ware.Id, out var illegalWare))
                        {
                            if (!illegalWaresCache.TryGetValue(faction.Value, out var illegalWares))
                            {
                                illegalWaresCache[faction.Value] = illegalWares = CollectIllegalModules(faction.Value);
                            }
                            if (illegalWares.Contains(illegalWare))
                            {
                                wareElement.Add(new XElement("owner", new XAttribute("faction", faction.Value.Id)));
                                count++;
                            }
                        }
                    }
                }
                if (count > 0)
                    yield return wareElement;
            }

            // Add paintmod wares
            var paintModsElement = new XElement("add", new XAttribute("sel", "//wares"));
            foreach (var faction in FactionsForm.AllCustomFactions)
            {
                paintModsElement.Add(new XElement("ware", new XAttribute("id", $"paintmod_{faction.Value.Id}"),
                    new XAttribute("name", $"{faction.Value.Name} Paint Mod"),
                    new XAttribute("description", ""),
                    new XAttribute("transport", "inventory"),
                    new XAttribute("volume", "1"),
                    new XAttribute("tags", "inventory paintmod"),
                    new XElement("price",
                        new XAttribute("min", "2500"),
                        new XAttribute("average", "3000"),
                        new XAttribute("max", "3500")
                        )
                    )
                );
            }
            yield return paintModsElement;
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
