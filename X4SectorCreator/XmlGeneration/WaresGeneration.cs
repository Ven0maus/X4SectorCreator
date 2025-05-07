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
            {"module_par_prod_majadust_01", "majadust" },
            {"module_par_prod_sojahusk_01", "sojahusk" },
            {"module_par_prod_sojabeans_01", "sojabeans" }
        };

        private static HashSet<string> CollectSpecialModules(Faction faction)
        {
            var illegalWares = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "spaceweed", "majadust", "majasnails", "spacefuel", "sojahusk", "sojabeans" };
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
            // TODO: Improve returning only 1 ware edit instead of multiple of same ware id
            var xml = File.ReadAllText(Constants.DataPaths.WaresMappingPath);
            var allWares = Wares.Deserialize(xml).Ware;
            var illegalWaresCache = new Dictionary<Faction, HashSet<string>>();

            var trackedWares = new Dictionary<Ware, HashSet<Faction>>();

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
                        TrackWare(trackedWares, ware, faction.Value);
                        count++;
                    }
                    else
                    {
                        // Include faction in illegal wares if they use them
                        if (_illegalWareMapping.TryGetValue(ware.Id, out var illegalWare))
                        {
                            if (!illegalWaresCache.TryGetValue(faction.Value, out var illegalWares))
                            {
                                illegalWaresCache[faction.Value] = illegalWares = CollectSpecialModules(faction.Value);
                            }
                            if (illegalWares.Contains(illegalWare))
                            {
                                wareElement.Add(new XElement("owner", new XAttribute("faction", faction.Value.Id)));
                                TrackWare(trackedWares, ware, faction.Value);
                                count++;
                            }
                        }
                    }
                }
                if (count > 0)
                    yield return wareElement;
            }

            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                var usedShipMacros = faction.ShipGroups
                    .SelectMany(a => a.SelectObj)
                    .Select(a => a.Macro)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var matchingShipWares = usedShipMacros
                    .Select(a => allWares.FirstOrDefault(b => b.ComponentObj?.Ref != null && b.ComponentObj.Ref.Equals(a, StringComparison.OrdinalIgnoreCase)))
                    .Where(a => a != null)
                    .ToArray();

                // Add faction owner to all matching ship wares
                foreach (var matchingShipWare in matchingShipWares)
                {
                    if (trackedWares.TryGetValue(matchingShipWare, out var factions))
                    {
                        if (factions.Contains(faction)) continue;
                    }

                    var wareElement = new XElement("add", new XAttribute("sel", $"/wares/ware[@id='{matchingShipWare.Id}']"));
                    wareElement.Add(new XElement("owner", new XAttribute("faction", faction.Id)));
                    TrackWare(trackedWares, matchingShipWare, faction);

                    yield return wareElement;
                }

                var equipmentRequiredFromFactions = matchingShipWares
                    .Where(a => a.OwnerObj != null)
                    .SelectMany(a => a.OwnerObj)
                    .Select(a => a.Faction)
                    .Where(a => a != null)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                // Collect all equipment (turrets, shields, weapons, engines) for the given factions, and add the current custom faction to it
                foreach (var eqFaction in equipmentRequiredFromFactions)
                {
                    // Collect equipment wares for eqFaction
                    var turrets = CollectGroupWaresFromFaction(allWares, "turrets", eqFaction);
                    var shields = CollectGroupWaresFromFaction(allWares, "shields", eqFaction);
                    var weapons = CollectGroupWaresFromFaction(allWares, "weapons", eqFaction);
                    var engines = CollectGroupWaresFromFaction(allWares, "engines", eqFaction);

                    var allEquipmentWares = turrets
                        .Concat(shields)
                        .Concat(weapons)
                        .Concat(engines)
                        .ToHashSet();
                    foreach (var equipmentWare in allEquipmentWares)
                    {
                        if (trackedWares.TryGetValue(equipmentWare, out var factions))
                        {
                            if (factions.Contains(faction)) continue;
                        }
                        var wareElement = new XElement("add", new XAttribute("sel", $"/wares/ware[@id='{equipmentWare.Id}']"));
                        wareElement.Add(new XElement("owner", new XAttribute("faction", faction.Id)));
                        TrackWare(trackedWares, equipmentWare, faction);

                        yield return wareElement;
                    }
                }
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

        private static IEnumerable<Ware> CollectGroupWaresFromFaction(List<Ware> wares, string groupName, string faction)
        {
            foreach (var ware in wares)
            {
                if (ware.Group != null && ware.Group.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                {
                    if (ware.OwnerObj != null && ware.OwnerObj.Any(a => a.Faction != null && a.Faction.Equals(faction, StringComparison.OrdinalIgnoreCase)))
                        yield return ware;
                }
            }
        }

        private static void TrackWare(Dictionary<Ware, HashSet<Faction>> trackedWares, Ware ware, Faction faction)
        {
            if (!trackedWares.TryGetValue(ware, out var factions))
                trackedWares.Add(ware, factions = []);
            factions.Add(faction);
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
