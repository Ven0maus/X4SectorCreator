﻿using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ZoneGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            XDocument xmlDocument = new(
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
            // This one needs to include base game, because they don't have zones initialized, only the ones with gates do
            foreach (Cluster cluster in clusters.OrderBy(a => a.Id))
            {
                foreach (Sector sector in cluster.Sectors.OrderBy(a => a.Id))
                {
                    foreach (Zone zone in sector.Zones.OrderBy(a => a.Id))
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
            foreach (Gate gate in zone.Gates.OrderBy(a => a.Id))
            {
                // General rule: don't place anything more than 50km away within a zone
                yield return new XElement("connection",
                    new XAttribute("name", $"{modPrefix}_GA_g{gate.Id:D3}_{gate.Source}_{gate.Destination}_connection"),
                    new XAttribute("ref", "gates"),
                    new XElement("offset",
                        new XElement("position",
                            new XAttribute("x", 0),
                            new XAttribute("y", 1000), // 1000 to avoid bugs at (0,0,0)
                            new XAttribute("z", 0)
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
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }
    }
}
