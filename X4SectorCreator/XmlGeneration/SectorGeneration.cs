using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class SectorGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("macros",
                    GenerateSectors(modPrefix, clusters)
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/xu_ep2_universe/{modPrefix}_sectors.xml")));
        }

        private static IEnumerable<XElement> GenerateSectors(string modPrefix, List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters.Where(a => !a.IsBaseGame).OrderBy(a => a.Id))
            {
                foreach (Sector sector in cluster.Sectors.OrderBy(a => a.Id))
                {
                    yield return new XElement("macro",
                        new XAttribute("name", $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro"),
                        new XAttribute("class", "sector"),
                        new XElement("component",
                            new XAttribute("ref", "standardsector")
                        ),
                        new XElement("connections",
                            GenerateZones(modPrefix, sector, cluster)
                        )
                    );
                }
            }
        }

        private static IEnumerable<XElement> GenerateZones(string modPrefix, Sector sector, Cluster cluster)
        {
            foreach (Zone zone in sector.Zones.OrderBy(a => a.Id))
            {
                yield return new XElement("connection",
                    new XAttribute("name", $"{modPrefix}_ZO_c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}_connection"),
                    new XAttribute("ref", "zones"),
                    new XElement("offset",
                        new XElement("position",
                            new XAttribute("x", zone.Position.X),
                            new XAttribute("y", 0),
                            new XAttribute("z", zone.Position.Y)
                        )
                    ),
                    new XElement("macro",
                        new XAttribute("ref", $"{modPrefix}_ZO_c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}_macro"),
                        new XAttribute("connection", "sector")
                    )
                );
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
