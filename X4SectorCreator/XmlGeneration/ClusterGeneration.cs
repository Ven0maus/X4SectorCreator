using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ClusterGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            #region Custom Clusters File
            XElement[] elements = GenerateClusters(modPrefix, clusters
                    .Where(a => !a.IsBaseGame)
                    .ToList())
                .ToArray();
            if (elements.Length > 0)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("macros",
                        elements
                    )
                );

                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{GalaxySettingsForm.GalaxyName}/{modPrefix}_clusters.xml")));
            }
            #endregion

            #region BaseGame Clusters File
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                return; // No need to process these
            }

            IGrouping<string, (string dlc, XElement element)>[] vanillaChangeElements = GenerateVanillaChanges(modPrefix, clusters, vanillaChanges)
                .GroupBy(a => a.dlc)
                .ToArray();
            if (vanillaChangeElements.Length > 0)
            {
                foreach (IGrouping<string, (string dlc, XElement element)> group in vanillaChangeElements)
                {
                    string dlcMapping = group.Key == null ? null : $"{MainForm.Instance.DlcMappings[group.Key]}_";
                    XDocument xmlDocument = new(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("diff",
                            group.Select(a => a.element)
                        )
                    );

                    if (dlcMapping == null)
                    {
                        xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{GalaxySettingsForm.GalaxyName}/clusters.xml")));
                    }
                    else
                    {
                        xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"extensions/{group.Key}/maps/{GalaxySettingsForm.GalaxyName}/{dlcMapping}clusters.xml")));
                    }
                }
            }

            #endregion
        }

        private static IEnumerable<XElement> GenerateClusters(string modPrefix, List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters.OrderBy(a => a.Id))
            {
                yield return new XElement("macro",
                    new XAttribute("name", $"{modPrefix}_CL_c{cluster.Id:D3}_macro"),
                    new XAttribute("class", "cluster"),
                    new XElement("component",
                        new XAttribute("ref", "standardcluster")
                    ),
                    new XElement("connections",
                        GenerateConnections(modPrefix, cluster)
                    )
                );
            }
        }

        private static IEnumerable<XElement> GenerateConnections(string modPrefix, Cluster cluster)
        {
            foreach (Sector sector in cluster.Sectors.OrderBy(a => a.Id))
            {
                yield return new XElement("connection",
                    new XAttribute("name", $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_connection"),
                    new XAttribute("ref", "sectors"),
                    new XElement("offset",
                        new XElement("position",
                            new XAttribute("x", sector.Offset.X),
                            new XAttribute("y", 0),
                            new XAttribute("z", sector.Offset.Y)
                        )
                    ),
                    new XElement("macro",
                        new XAttribute("ref", $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro"),
                        new XAttribute("connection", "cluster")
                    )
                );

                // Return regions after sector connection
                foreach (Objects.Region region in sector.Regions)
                {
                    if (region.IsBaseGame) continue;

                    string name = $"c{cluster.Id:D3}_s{sector.Id:D3}_r{region.Id:D3}";
                    if (cluster.IsBaseGame && sector.IsBaseGame)
                        name = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_r{region.Id:D3}";
                    else if (cluster.IsBaseGame)
                        name = $"{cluster.BaseGameMapping}_s{sector.Id:D3}_r{region.Id:D3}";

                    string identifier = region.GetIdentifier(modPrefix);

                    yield return new XElement("connection",
                        new XAttribute("name", $"{name}_{identifier}_connection"),
                        new XAttribute("ref", "regions"),
                        new XElement("offset",
                            new XElement("position",
                                new XAttribute("x", (sector.IsBaseGame ? sector.SectorRealOffset.X : sector.Offset.X) + region.Position.X),
                                new XAttribute("y", 0),
                                new XAttribute("z", (sector.IsBaseGame ? sector.SectorRealOffset.Y : sector.Offset.Y) + region.Position.Y)
                            )
                        ),
                        new XElement("macro",
                            new XAttribute("name", $"{name}_{identifier}_macro"),
                            new XElement("component",
                                new XAttribute("connection", "cluster"),
                                new XAttribute("ref", "standardregion")
                            ),
                            // Region definition name needs to be fully lowercase else it will NOT work!!!!!!!!
                            new XElement("properties",
                                new XElement("region",
                                    new XAttribute("ref", identifier.ToLower())
                                )
                            )
                        )
                    );
                }
            }

            // Adding content connection
            yield return new XElement("connection",
                new XAttribute("ref", "content"),
                new XElement("macro",
                    new XElement("component",
                        new XAttribute("connection", "space"),
                        new XAttribute("ref", string.IsNullOrWhiteSpace(cluster.CustomClusterXml) ? 
                            cluster.BackgroundVisualMapping : GetClusterCode(cluster).Replace("PREFIX", modPrefix))
                    )
                )
            );
        }

        private static string GetClusterCode(Cluster cluster)
        {
            // Used for custom cluster xml referencing
            return $"PREFIX_CL_c{cluster.Id:D3}";
        }

        private static IEnumerable<(string dlc, XElement element)> GenerateVanillaChanges(string modPrefix, List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            foreach (Cluster cluster in vanillaChanges.RemovedClusters)
            {
                string clusterCode = cluster.BaseGameMapping.CapitalizeFirstLetter();
                yield return (cluster.Dlc, new XElement("remove", new XAttribute("sel", $"//macros/macro[@name='{clusterCode}_macro']")));
            }
            foreach (ModifiedCluster modification in vanillaChanges.ModifiedClusters)
            {
                Cluster Old = modification.Old;
                Cluster New = modification.New;
                if (!Old.BackgroundVisualMapping.Equals(New.BackgroundVisualMapping, StringComparison.OrdinalIgnoreCase))
                {
                    string clusterCode = Old.BaseGameMapping.CapitalizeFirstLetter();
                    yield return (Old.Dlc, new XElement("replace",
                        new XAttribute("sel", $"/macros/macro[@name='{clusterCode}_macro']/connections/connection[@ref='content']/macro/component/@ref"),
                        New.BackgroundVisualMapping
                        ));
                }
            }
            foreach (RemovedSector sector in vanillaChanges.RemovedSectors)
            {
                // Check if the cluster was removed, then skip this sector as its already part of the cluster deletion in galaxy.xml
                if (vanillaChanges.RemovedClusters.Contains(sector.VanillaCluster))
                {
                    continue;
                }

                string clusterCode = sector.VanillaCluster.BaseGameMapping.CapitalizeFirstLetter();
                string sectorCode = $"{clusterCode}_{sector.Sector.BaseGameMapping.CapitalizeFirstLetter()}";
                yield return (sector.VanillaCluster.Dlc, new XElement("remove",
                    new XAttribute("sel", $"/macros/macro[@name='{clusterCode}_macro']/connections/connection[@name='{sectorCode}_connection']")));
            }
            HashSet<string> handledConnections = new(StringComparer.OrdinalIgnoreCase);
            foreach (RemovedConnection removedConnection in vanillaChanges.RemovedConnections)
            {
                if (removedConnection.Gate.IsHighwayGate && !handledConnections.Contains(removedConnection.Gate.ConnectionName))
                {
                    _ = handledConnections.Add(removedConnection.Gate.ConnectionName);
                    string clusterCode = removedConnection.VanillaCluster.BaseGameMapping.CapitalizeFirstLetter();
                    yield return (removedConnection.VanillaCluster.Dlc, new XElement("remove",
                    new XAttribute("sel", $"/macros/macro[@name='{clusterCode}_macro']/connections/connection[@name='{removedConnection.Gate.ConnectionName}']")));
                }
            }

            // Added sectors in vanilla clusters
            foreach (Cluster cluster in clusters)
            {
                if (!cluster.IsBaseGame)
                {
                    continue;
                }

                foreach (Sector sector in cluster.Sectors)
                {
                    string macro = cluster.IsBaseGame ? $"{modPrefix}_SE_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_s{sector.Id:D3}_macro" :
                        $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
                    string connection = cluster.IsBaseGame ? $"{modPrefix}_SE_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_s{sector.Id:D3}_connection" :
                        $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_connection";

                    var addElement = new XElement("add", new XAttribute("sel", $"/macros/macro[@name='{cluster.BaseGameMapping.CapitalizeFirstLetter()}_macro']/connections"));

                    if (sector.IsBaseGame)
                    {
                        // Check if regions exist, then generate these too
                        foreach (Objects.Region region in sector.Regions)
                        {
                            if (region.IsBaseGame) continue;

                            string name = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_r{region.Id:D3}";
                            string identifier = region.GetIdentifier(modPrefix);

                            addElement.Add(new XElement("connection",
                                new XAttribute("name", $"{name}_{identifier}_connection"),
                                new XAttribute("ref", "regions"),
                                new XElement("offset",
                                    new XElement("position",
                                        new XAttribute("x", sector.Offset.X + region.Position.X),
                                        new XAttribute("y", 0),
                                        new XAttribute("z", sector.Offset.Y + region.Position.Y)
                                    )
                                ),
                                new XElement("macro",
                                    new XAttribute("name", $"{name}_{identifier}_macro"),
                                    new XElement("component",
                                        new XAttribute("connection", "cluster"),
                                        new XAttribute("ref", "standardregion")
                                    ),
                                    // Region definition name needs to be fully lowercase else it will NOT work!!!!!!!!
                                    new XElement("properties",
                                        new XElement("region",
                                            new XAttribute("ref", identifier.ToLower())
                                        )
                                    )
                                )
                            ));
                        }
                        if (!addElement.IsEmpty)
                            yield return (cluster.Dlc, addElement);
                        continue;
                    }

                    // Add new sector
                    addElement.Add(
                            new XElement("connection",
                            new XAttribute("name", connection),
                            new XAttribute("ref", "sectors"),
                            new XElement("offset",
                                new XElement("position",
                                    new XAttribute("x", sector.Offset.X),
                                    new XAttribute("y", 0),
                                    new XAttribute("z", sector.Offset.Y)
                                )
                            ),
                            new XElement("macro",
                                new XAttribute("ref", macro),
                                new XAttribute("connection", "cluster")
                            )
                        )
                    );

                    if (sector.Regions != null && sector.Regions.Count > 0)
                    {
                        // Add regions after sector connection
                        foreach (Objects.Region region in sector.Regions)
                        {
                            if (region.IsBaseGame) continue;

                            string name = cluster.IsBaseGame ? $"{cluster.BaseGameMapping}_s{sector.Id:D3}_r{region.Id:D3}" :
                                $"c{cluster.Id:D3}_s{sector.Id:D3}_r{region.Id:D3}";
                            string identifier = region.GetIdentifier(modPrefix);

                            addElement.Add(new XElement("connection",
                                new XAttribute("name", $"{name}_{identifier}_connection"),
                                new XAttribute("ref", "regions"),
                                new XElement("offset",
                                    new XElement("position",
                                        new XAttribute("x", sector.Offset.X + region.Position.X),
                                        new XAttribute("y", 0),
                                        new XAttribute("z", sector.Offset.Y + region.Position.Y)
                                    )
                                ),
                                new XElement("macro",
                                    new XAttribute("name", $"{name}_{identifier}_macro"),
                                    new XElement("component",
                                        new XAttribute("connection", "cluster"),
                                        new XAttribute("ref", "standardregion")
                                    ),
                                    // Region definition name needs to be fully lowercase else it will NOT work!!!!!!!!
                                    new XElement("properties",
                                        new XElement("region",
                                            new XAttribute("ref", identifier.ToLower())
                                        )
                                    )
                                )
                            ));
                        }
                    }

                    yield return (cluster.Dlc, addElement);
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
