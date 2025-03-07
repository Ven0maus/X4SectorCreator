using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class SectorGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, ClusterCollection nonModifiedBaseGameData, VanillaChanges vanillaChanges)
        {
            #region Custom Sector File
            // Save new sectors in custom clusters
            XElement[] sectors = GenerateSectors(modPrefix, clusters.Where(a => !a.IsBaseGame).ToList()).ToArray();
            if (sectors.Length > 0)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("macros",
                        sectors
                    )
                );

                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{GalaxySettingsForm.GalaxyName}/{modPrefix}_sectors.xml")));
            }
            #endregion

            #region BaseGame Sector File
            // Save new zones in existing sectors
            List<IGrouping<string, (string dlc, XElement element)>> diffData = GenerateVanillaChanges(vanillaChanges)
                .Concat(GenerateSectorAdds(modPrefix, clusters, nonModifiedBaseGameData))
                .GroupBy(a => a.dlc)
                .ToList();
            if (diffData.Count > 0)
            {
                foreach (IGrouping<string, (string dlc, XElement element)> group in diffData)
                {
                    string dlcMapping = group.Key == null ? null : $"{MainForm.Instance.DlcMappings[group.Key]}_";
                    XDocument xmlSectorDiff = new(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("diff",
                            group.Select(a => a.element)
                        )
                    );

                    if (dlcMapping == null)
                    {
                        xmlSectorDiff.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{GalaxySettingsForm.GalaxyName}/sectors.xml")));
                    }
                    else
                    {
                        xmlSectorDiff.Save(EnsureDirectoryExists(Path.Combine(folder, $"extensions/{group.Key}/maps/{GalaxySettingsForm.GalaxyName}/{dlcMapping}sectors.xml")));
                    }
                }
            }
            #endregion
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

        private static IEnumerable<(string dlc, XElement element)> GenerateSectorAdds(string modPrefix, List<Cluster> clusters, ClusterCollection nonModifiedBaseGameData)
        {
            HashSet<string> zoneCache = nonModifiedBaseGameData.Clusters
                .SelectMany(a => a.Sectors)
                .SelectMany(a => a.Zones)
                .Select(a => a.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (Cluster cluster in clusters)
            {
                if (!cluster.IsBaseGame)
                {
                    continue;
                }

                foreach (Sector sector in cluster.Sectors)
                {
                    if (sector.Zones.Count == 0)
                    {
                        continue;
                    }

                    XElement[] zoneElements = GenerateExistingSectorZoneConnections(modPrefix, cluster, sector, sector.Zones, zoneCache).ToArray();
                    if (zoneElements.Length > 0)
                    {
                        yield return (cluster.Dlc, new XElement("add",
                            new XAttribute("sel", $"//macros/macro[@name='{cluster.BaseGameMapping.CapitalizeFirstLetter()}_{sector.BaseGameMapping.CapitalizeFirstLetter()}_macro']/connections"),
                            zoneElements)
                        );
                    }
                }
            }
        }

        private static IEnumerable<XElement> GenerateExistingSectorZoneConnections(string modPrefix, Cluster cluster, Sector sector, List<Zone> zones, HashSet<string> zoneCache)
        {
            foreach (Zone zone in zones.OrderBy(a => a.Id))
            {
                // if zone is not part of base game
                if (zoneCache.Contains(zone.Name))
                {
                    continue;
                }

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

        private static IEnumerable<(string dlc, XElement element)> GenerateVanillaChanges(VanillaChanges vanillaChanges)
        {
            foreach (RemovedSector sector in vanillaChanges.RemovedSectors)
            {
                string macro = $"{sector.VanillaCluster.BaseGameMapping.CapitalizeFirstLetter()}_{sector.Sector.BaseGameMapping.CapitalizeFirstLetter()}";
                yield return (sector.VanillaCluster.Dlc, new XElement("remove",
                    new XAttribute("sel", $"//macros/macro[@name='{macro}_macro']")));
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
