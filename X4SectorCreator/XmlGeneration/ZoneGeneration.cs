using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ZoneGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, ClusterCollection nonModifiedBaseGameData, VanillaChanges vanillaChanges)
        {
            #region Custom Zone File
            // Save new zones in custom sectors
            XElement[] zones = GenerateZones(modPrefix, clusters).ToArray();
            if (zones.Length > 0)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("macros",
                        new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                        new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                        zones
                    )
                );

                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{GalaxySettingsForm.GalaxyName}/{modPrefix}_zones.xml")));
            }
            #endregion

            #region BaseGame Zone File
            // Save new zones in existing sectors
            List<IGrouping<string, (string dlc, XElement element)>> diffData = GenerateExistingSectorZones(modPrefix, clusters, nonModifiedBaseGameData)
                .Concat(GenerateVanillaChanges(vanillaChanges))
                .GroupBy(a => a.dlc)
                .ToList();
            if (diffData.Count > 0)
            {
                foreach (IGrouping<string, (string dlc, XElement element)> group in diffData)
                {
                    string dlcMapping = group.Key == null ? null : $"{MainForm.Instance.DlcMappings[group.Key]}_";
                    XDocument xmlDiffDocument = new(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("diff",
                            group.Select(a => a.element)
                        )
                    );

                    if (dlcMapping == null)
                    {
                        xmlDiffDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{GalaxySettingsForm.GalaxyName}/zones.xml")));
                    }
                    else
                    {
                        xmlDiffDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"extensions/{group.Key}/maps/{GalaxySettingsForm.GalaxyName}/{dlcMapping}zones.xml")));
                    }
                }
            }
            #endregion
        }

        private static IEnumerable<(string dlc, XElement element)> GenerateVanillaChanges(VanillaChanges vanillaChanges)
        {
            foreach (RemovedConnection connection in vanillaChanges.RemovedConnections)
            {
                if (connection.Gate.IsHighwayGate)
                {
                    continue;
                }

                string connectionName = $"connection_{connection.Gate.ConnectionName}";
                if (connection.Gate.ConnectionName.Equals("destination", StringComparison.OrdinalIgnoreCase) && !connection.Gate.IsHighwayGate)
                {
                    connectionName = connection.Gate.SourcePath.Split('/').Last();
                }

                yield return (connection.VanillaCluster.Dlc, new XElement("remove", new XAttribute("sel", $"/macros/macro[@name='{connection.Zone.Name}_macro']/connections/connection[@name='{connectionName}']")));
            }
        }

        private static IEnumerable<XElement> GenerateZones(string modPrefix, List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters.OrderBy(a => a.Id))
            {
                foreach (Sector sector in cluster.Sectors.OrderBy(a => a.Id))
                {
                    if (sector.IsBaseGame)
                    {
                        continue;
                    }

                    foreach (Zone zone in sector.Zones.OrderBy(a => a.Id))
                    {
                        string macro = cluster.IsBaseGame ? $"{modPrefix}_ZO_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_s{sector.Id:D3}_z{zone.Id:D3}_macro" :
                            $"{modPrefix}_ZO_c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}_macro";

                        yield return new XElement("macro",
                            new XAttribute("name", macro),
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

        private static IEnumerable<(string dlc, XElement element)> GenerateExistingSectorZones(string modPrefix, List<Cluster> clusters, ClusterCollection nonModifiedBaseGameData)
        {
            HashSet<string> zoneCache = nonModifiedBaseGameData.Clusters
                .SelectMany(a => a.Sectors)
                .SelectMany(a => a.Zones)
                .Select(a => a.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            List<(string dlc, XElement element)> elements = [];


            foreach (Cluster cluster in clusters.OrderBy(a => a.Id))
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    if (!sector.IsBaseGame)
                    {
                        continue;
                    }

                    foreach (Zone zone in sector.Zones.OrderBy(a => a.Id))
                    {
                        if (zoneCache.Contains(zone.Name))
                        {
                            continue;
                        }

                        elements.Add((cluster.Dlc, new XElement("macro",
                            new XAttribute("name", $"{modPrefix}_ZO_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_{sector.BaseGameMapping.CapitalizeFirstLetter()}_z{zone.Id:D3}_macro"),
                            new XAttribute("class", "zone"),
                            new XElement("component", new XAttribute("ref", "standardzone")),
                            new XElement("connections",
                                GenerateExistingSectorGates(modPrefix, zone)
                            )
                        )));
                    }
                }
            }

            foreach (IGrouping<string, (string dlc, XElement element)> group in elements.GroupBy(a => a.dlc))
            {
                XElement addElement = new("add", new XAttribute("sel", "/macros"));
                foreach ((string dlc, XElement element) in group)
                {
                    addElement.Add(element);
                }
                yield return (group.Key, addElement);
            }
        }

        private static IEnumerable<XElement> GenerateExistingSectorGates(string modPrefix, Zone zone)
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
                            new XAttribute("y", 1000),
                            new XAttribute("z", 0)
                        ),
                        new XElement("rotation",
                            new XAttribute("yaw", gate.Yaw),
                            new XAttribute("pitch", gate.Pitch),
                            new XAttribute("roll", gate.Roll)
                        )
                    ),
                    new XElement("macro",
                        new XAttribute("ref", "props_gates_anc_gate_macro"),
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
