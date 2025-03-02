using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class SectorGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            var galaxyName = GalaxySettingsForm.IsCustomGalaxy ?
                $"{modPrefix}_{GalaxySettingsForm.GalaxyName}" : GalaxySettingsForm.GalaxyName;

            // Save new sectors in custom clusters
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("macros",
                    GenerateSectors(modPrefix, clusters.Where(a => !a.IsBaseGame).ToList())
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{galaxyName}/{modPrefix}_sectors.xml")));

            // Save new zones in existing sectors
            var diffData = GenerateSectorAdds(modPrefix, clusters).ToList();
            if (diffData.Count > 0)
            {
                var xmlSectorDiff = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        diffData
                    )
                );

                xmlSectorDiff.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{galaxyName}/sectors.xml")));
            }
        }

        private static IEnumerable<XElement> GenerateSectors(string modPrefix, List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters.OrderBy(a => a.Id))
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

        private static IEnumerable<XElement> GenerateSectorAdds(string modPrefix, List<Cluster> clusters)
        {
            foreach (var cluster in clusters)
            {
                if (!cluster.IsBaseGame) continue;

                foreach (var sector in cluster.Sectors)
                {
                    if (sector.Zones.Count == 0) continue;

                    yield return new XElement("add",
                        new XAttribute("sel", $"//macros/macro[@name='{cluster.BaseGameMapping.CapitalizeFirstLetter()}_{sector.BaseGameMapping.CapitalizeFirstLetter()}_macro']/connections"),
                        GenerateExistingSectorZoneConnections(modPrefix, cluster, sector, sector.Zones)
                    );
                }
            }
        }

        private static IEnumerable<XElement> GenerateExistingSectorZoneConnections(string modPrefix, Cluster cluster, Sector sector, List<Zone> zones)
        {
            foreach (var zone in zones.OrderBy(a => a.Id))
            {
                yield return new XElement("connection",
                    new XAttribute("name", $"{modPrefix}_ZO_{cluster.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "")}_{sector.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "")}_z{zone.Id:D3}_connection"),
                    new XAttribute("ref", "zones"),
                    new XElement("offset",
                        new XElement("position",
                            new XAttribute("x", zone.Position.X),
                            new XAttribute("y", 0),
                            new XAttribute("z", zone.Position.Y)
                        )
                    ),
                    new XElement("macro",
                        new XAttribute("ref", $"{modPrefix}_ZO_{cluster.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "")}_{sector.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "")}_z{zone.Id:D3}_macro"),
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
