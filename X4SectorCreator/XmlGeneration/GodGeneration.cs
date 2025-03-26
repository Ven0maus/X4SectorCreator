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

                // Prepend prefix
                factory.Value.Id = $"{modPrefix}_{factory.Value.Id}";

                // Serialize
                string factoryElementXml = factory.Value.SerializeFactory();

                // Reset
                factory.Value.Id = originalId;

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
                    foreach (Zone zone in sector.Zones)
                    {
                        foreach (Station station in zone.Stations)
                        {
                            string clusterPrefix = $"c{cluster.Id:D3}";
                            if (cluster.IsBaseGame)
                            {
                                clusterPrefix = cluster.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "");
                            }

                            string sectorPrefix = $"s{sector.Id:D3}";
                            if (sector.IsBaseGame)
                            {
                                sectorPrefix = sector.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "");
                            }

                            string id = $"{modPrefix}_ST_{clusterPrefix}_{sectorPrefix}_st{station.Id:D3}";
                            string zoneMacro = $"{modPrefix}_ZO_{clusterPrefix}_{sectorPrefix}_z{zone.Id:D3}_macro";

                            string faction = station.Faction.ToLower();
                            string owner = station.Owner.ToLower();
                            faction = CorrectFactionNames(faction);
                            owner = CorrectFactionNames(owner);

                            yield return new XElement("station",
                                new XAttribute("id", id.ToLower()),
                                new XAttribute("race", station.Race.ToLower()),
                                new XAttribute("owner", owner),
                                new XAttribute("type", station.Type.Equals("tradestation", StringComparison.OrdinalIgnoreCase) ? "tradingstation" : "factory"),
                                new XElement("quotas",
                                    new XElement("quota",
                                        new XAttribute("galaxy", 1),
                                        new XAttribute("sector", 1)
                                    )
                                ),
                                new XElement("location",
                                    new XAttribute("class", "zone"),
                                    new XAttribute("macro", zoneMacro.ToLower()),
                                    new XAttribute("matchextension", "false")
                                ),
                                new XElement("station",
                                    new XElement("select",
                                        new XAttribute("faction", faction),
                                        new XAttribute("tags", $"[{station.Type.ToLower()}]")),
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

        private static string CorrectFactionNames(string faction)
        {
            switch (faction)
            {
                case "vigor":
                    faction = "loanshark";
                    break;
                case "riptide":
                    faction = "scavenger";
                    break;
                case "quettanauts":
                    faction = "kaori";
                    break;
                case "zyarth":
                    faction = "split";
                    break;
                case "segaris":
                    faction = "pioneers";
                    break;
            }

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
