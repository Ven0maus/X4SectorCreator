using System.Xml.Linq;
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
                    XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
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
                    foreach (var station in sector.Stations)
                    {
                        var id = cluster.IsBaseGame ? $"{modPrefix}_ST_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_s{sector.Id:D3}_st{station.Id:D3}" :
                            $"{modPrefix}_ST_c{cluster.Id:D3}_s{sector.Id:D3}_st{station.Id:D3}";

                        var sectorMacro = cluster.IsBaseGame ? $"{modPrefix}_SE_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_s{sector.Id:D3}_macro" :
                            $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";

                        // TODO: Set zone for station based on location instead of sector

                        yield return new XElement("station",
                            new XAttribute("id", id.ToLower()),
                            new XAttribute("race", station.Race.ToLower()),
                            new XAttribute("owner", station.Faction.ToLower()),
                            new XAttribute("type", "factory"),
                            new XElement("quotas",
                                new XElement("quota",
                                    new XAttribute("galaxy", 1),
                                    new XAttribute("sector", 1)
                                )
                            ),
                            new XElement("location",
                                new XAttribute("class", "sector"),
                                new XAttribute("macro", sectorMacro.ToLower()),
                                new XAttribute("matchextension", "false")
                            ),
                            new XElement("station",
                                new XElement("select",
                                    new XAttribute("faction", station.Faction.ToLower()),
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
