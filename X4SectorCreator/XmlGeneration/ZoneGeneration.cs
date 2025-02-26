using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ZoneGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            XDocument xmlDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("macros",
                    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                    GenerateZones(modPrefix, clusters)
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/xu_ep2_universe/{modPrefix}_zones.xml")));
        }

        private static IEnumerable<XElement> GenerateZones(string modPrefix, List<Cluster> clusters)
        {
            foreach (var cluster in clusters.OrderBy(a => a.Id))
            {
                foreach (var sector in cluster.Sectors.OrderBy(a => a.Id))
                {
                    foreach (var zone in sector.Zones.OrderBy(a => a.Id))
                    {
                        yield return new XElement("macro",
                            new XAttribute("name", $"{modPrefix}_ZO_c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}_macro"),
                            new XAttribute("class", "zone"),
                            new XElement("component",
                                new XAttribute("ref", "standardzone")
                            ),
                            new XElement("connections",
                                GenerateGates(modPrefix, zone)
                            )
                        );
                    }
                }
            }
        }

        private static IEnumerable<XElement> GenerateGates(string modPrefix, Zone zone)
        {
            foreach (var gate in zone.Gates.OrderBy(a => a.Id))
            {
                yield return new XElement("connection",
                    new XAttribute("name", $"{modPrefix}_GA_g:{gate.Id:D3}_{gate.Source}_{gate.Destination}_connection"),
                    new XAttribute("ref", "gates"),
                    new XElement("offset",
                        new XElement("position",
                            new XAttribute("x", gate.Position.X),
                            new XAttribute("y", 500), // 500 possibly to avoid highways?
                            new XAttribute("z", gate.Position.Y)
                        ),
                        new XElement("rotation",
                            new XAttribute("yaw", gate.Yaw),
                            new XAttribute("pitch", gate.Pitch),
                            new XAttribute("roll", gate.Roll)
                        )
                    ),
                    new XElement("macro",
                        new XAttribute("ref", gate.Type.ToString()),
                        new XAttribute("connection", "space")
                    )
                );
            }
        }

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            return filePath;
        }
    }
}
