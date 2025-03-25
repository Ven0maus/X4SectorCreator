using System.Xml.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class GodGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            var stationsContent = CollectStationsContent(modPrefix, clusters).ToArray();

            // Replace entire god file
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                var stationsNode = stationsContent.Length == 0 ? null :
                    new XElement("stations", stationsContent);

                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        new XElement("replace", new XAttribute("sel", "//god"),
                        new XElement("god", new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                            new XAttribute(xsi + "noNamespaceSchemaLocation", "libraries.xsd"),
                            stationsNode
                            )
                        )
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/god.xml")));
            }
            else
            {
                if (stationsContent.Length > 0)
                {
                    XDocument xmlDocument = new(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("diff",
                            new XElement("add", 
                                new XAttribute("sel", "/god/stations"),
                                stationsContent
                            )
                        )
                    );
                    xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/god.xml")));
                }
            }
        }

        private static IEnumerable<XElement> CollectStationsContent(string modPrefix, List<Cluster> clusters)
        {
            foreach (var cluster in clusters)
            {
                foreach (var sector in cluster.Sectors)
                {
                    foreach (var zone in sector.Zones)
                    {
                        foreach (var station in zone.Stations)
                        {
                            string clusterPrefix = $"c{cluster.Id:D3}";
                            if (cluster.IsBaseGame)
                                clusterPrefix = cluster.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "");

                            string sectorPrefix = $"s{sector.Id:D3}";
                            if (sector.IsBaseGame)
                                sectorPrefix = sector.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "");

                            var id = $"{modPrefix}_ST_{clusterPrefix}_{sectorPrefix}_st{station.Id:D3}";
                            var zoneMacro = $"{modPrefix}_ZO_{clusterPrefix}_{sectorPrefix}_z{zone.Id:D3}_macro";

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
