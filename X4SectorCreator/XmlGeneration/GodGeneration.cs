using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class GodGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            XElement[] stationsContent = CollectStationsContent(modPrefix, clusters).ToArray();
            XElement[] productsContent = CollectProductsContent(modPrefix).ToArray();

            // Replace entire god file
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                XElement stationsNode = stationsContent.Length == 0 ? null :
                    new XElement("stations", stationsContent);

                XElement productsNode = productsContent.Length == 0 ? null :
                    new XElement("products", productsContent);

                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        new XElement("replace", new XAttribute("sel", "//god"),
                        new XElement("god", new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                            new XAttribute(xsi + "noNamespaceSchemaLocation", "libraries.xsd"),
                            productsNode,
                            stationsNode
                            )
                        )
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/god.xml")));
            }
            else
            {
                XElement diffContent = new("diff");

                if (productsContent.Length > 0)
                {
                    diffContent.Add(new XElement("add",
                                new XAttribute("sel", "/god/products"),
                                productsContent
                            ));
                }

                if (stationsContent.Length > 0)
                {
                    diffContent.Add(new XElement("add",
                                new XAttribute("sel", "/god/stations"),
                                stationsContent
                            ));
                }

                if (!diffContent.IsEmpty)
                {
                    XDocument xmlDocument = new(
                        new XDeclaration("1.0", "utf-8", null),
                        diffContent
                    );
                    xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/god.xml")));
                }
            }
        }

        private static IEnumerable<XElement> CollectProductsContent(string modPrefix)
        {
            foreach (KeyValuePair<string, Factory> factory in FactoriesForm.AllFactories)
            {
                string originalId = factory.Value.Id;
                string originalMacro = factory.Value.Location?.Macro;

                // Fix galaxy macro (can happen when switching galaxy name after factory is defined)
                if (originalMacro != null && GalaxySettingsForm.IsCustomGalaxy && originalMacro != $"{GalaxySettingsForm.GalaxyName}_macro")
                    factory.Value.Location.Macro = originalMacro = $"{GalaxySettingsForm.GalaxyName}_macro";

                // Prepend prefix
                factory.Value.Id = $"{modPrefix}_{factory.Value.Id}";

                // Replace location macro prefix
                if (factory.Value.Location?.Macro != null && factory.Value.Location.Macro.Contains("PREFIX"))
                {
                    factory.Value.Location.Macro = factory.Value.Location.Macro.Replace("PREFIX", modPrefix);
                }

                // Serialize
                string factoryElementXml = factory.Value.SerializeFactory();

                // Reset
                factory.Value.Id = originalId;
                if (factory.Value.Location?.Macro != null)
                {
                    factory.Value.Location.Macro = originalMacro;
                }

                XElement factoryElement = XElement.Parse(factoryElementXml);
                yield return factoryElement;
            }
        }

        private static IEnumerable<XElement> CollectStationsContent(string modPrefix, List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters)
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    foreach (Zone zone in sector.Zones.Where(a => !a.IsBaseGame))
                    {
                        foreach (Station station in zone.Stations)
                        {
                            string clusterPrefix = $"c{cluster.Id:D3}";
                            if (cluster.IsBaseGame)
                            {
                                clusterPrefix = cluster.BaseGameMapping.CapitalizeFirstLetter();
                            }

                            string sectorPrefix = $"s{sector.Id:D3}";
                            if (sector.IsBaseGame)
                            {
                                sectorPrefix = sector.BaseGameMapping.CapitalizeFirstLetter();
                            }

                            string id = $"{modPrefix}_ST_{clusterPrefix}_{sectorPrefix}_st{station.Id:D3}";
                            string zoneMacro = $"{modPrefix}_ZO_{clusterPrefix}_{sectorPrefix}_z{zone.Id:D3}_macro";

                            string faction = (station.Faction ?? string.Empty).ToLower();
                            string owner = station.Owner.ToLower();
                            var type = station.Type.ToLower();
                            faction = CorrectFactionName(faction);
                            owner = CorrectFactionName(owner);

                            string realType = "factory";

                            // Xenon work differently compared to others
                            if (faction.Equals("xenon", StringComparison.OrdinalIgnoreCase) ||
                                owner.Equals("xenon", StringComparison.OrdinalIgnoreCase))
                            {
                                if (type == "defence")
                                    realType = "defence";
                                if (type == "wharf" || type == "shipyard")
                                    realType = "shipyard";
                            }

                            if (type == "tradestation")
                                realType = "tradingstation";
                            if (type == "piratebase")
                                realType = "piratebase";

                            var selectElement = new XElement("select",
                                        new XAttribute("faction", faction),
                                        new XAttribute("tags", $"[{type}]"));
                            XAttribute attributeElement = null;

                            // Handle custom construction plan if available
                            if (!string.IsNullOrWhiteSpace(station.CustomConstructionPlan))
                            {
                                selectElement = null;
                                attributeElement = new XAttribute("constructionplan", $"{modPrefix}_{station.CustomConstructionPlan}");
                            }

                            yield return new XElement("station",
                                new XAttribute("id", id.ToLower()),
                                new XAttribute("race", station.Race.ToLower()),
                                new XAttribute("owner", owner),
                                new XAttribute("type", realType),
                                new XElement("quotas",
                                    new XElement("quota",
                                        new XAttribute("galaxy", 1),
                                        new XAttribute("sector", 1)
                                    )
                                ),
                                new XElement("location",
                                    new XAttribute("class", "zone"),
                                    new XAttribute("macro", zoneMacro.ToLower()),
                                    type == "piratebase" ? new XAttribute("solitary", "true") : null,
                                    new XAttribute("matchextension", "false")
                                ),
                                new XElement("station",
                                    attributeElement,
                                    selectElement,
                                    new XElement("loadout",
                                        new XElement("level",
                                            new XAttribute("exact", "1.0"))
                                    )
                                )
                            );
                        }
                    }
                }
            }
        }

        private static readonly Dictionary<string, string> _factionNameMapping = new(StringComparer.OrdinalIgnoreCase)
        {
            { "vigor", "loanshark" },
            { "riptide", "scavenger" },
            { "quettanauts", "kaori" },
            { "zyarth", "split" },
            { "segaris", "pioneers" },
        };
        private static readonly Dictionary<string, string> _reverseFactionNameMapping = 
            _factionNameMapping.ToDictionary(a => a.Value, a => a.Key);

        /// <summary>
        /// Corrects readable factions names to the X4 names.
        /// </summary>
        /// <param name="faction"></param>
        /// <returns></returns>
        public static string CorrectFactionName(string faction)
        {
            if (_factionNameMapping.TryGetValue(faction, out var correctFactionName))
                return correctFactionName;
            return faction;
        }

        /// <summary>
        /// Corrects X4 names to readable faction names.
        /// </summary>
        /// <param name="faction"></param>
        /// <returns></returns>
        public static string CorrectFactionNameReversed(string faction)
        {
            if (_reverseFactionNameMapping.TryGetValue(faction, out var correctFactionName))
                return correctFactionName;
            return faction;
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
