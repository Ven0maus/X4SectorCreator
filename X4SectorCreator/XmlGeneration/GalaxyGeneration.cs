using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class GalaxyGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, VanillaChanges vanillaChanges, ClusterCollection nonModifiedBaseGameData)
        {
            List<Cluster> orderedClusters = [.. clusters.OrderBy(a => a.Id)];

            XDocument xmlDocument = null;
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                var galaxyElements = GenerateClusters(modPrefix, orderedClusters)
                    .Concat(GenerateGateConnections(modPrefix, orderedClusters, nonModifiedBaseGameData))
                    .ToArray();
                if (galaxyElements.Length > 0)
                {
                    xmlDocument = new(new XDeclaration("1.0", "utf-8", null),
                        new XElement("macros",
                            new XElement("macro",
                                new XAttribute("name", $"{GalaxySettingsForm.GalaxyName}_macro"),
                                new XAttribute("class", "galaxy"),
                                    new XElement("component", new XAttribute("ref", "standardgalaxy")),
                                    new XElement("connections",
                                    galaxyElements
                                )
                            )
                        )
                    );

                    xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{GalaxySettingsForm.GalaxyName}/galaxy.xml")));
                }
            }
            else
            {
                var groups = GenerateVanillaChanges(vanillaChanges)
                    .Append((null, GenerateNewContent(modPrefix, clusters, nonModifiedBaseGameData)))
                    .Where(a => a.element != null)
                    .GroupBy(a => a.dlc)
                    .ToArray();
                if (groups.Length > 0)
                {
                    foreach (var group in groups)
                    {
                        string dlcMapping = group.Key == null ? null : $"{MainForm.Instance.DlcMappings[group.Key]}_";
                        xmlDocument = new(new XDeclaration("1.0", "utf-8", null),
                            new XElement("diff", group.Select(a => a.element))
                        );

                        if (dlcMapping == null)
                        {
                            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{GalaxySettingsForm.GalaxyName}/galaxy.xml")));
                        }
                        else
                        {
                            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"extensions/{group.Key}/maps/{GalaxySettingsForm.GalaxyName}/galaxy.xml")));
                        }
                    }
                }
            }
        }

        private static XElement GenerateNewContent(string modPrefix, List<Cluster> clusters, ClusterCollection nonModifiedBaseGameData)
        {
            var addElement = new XElement("add",
                                new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections"));

            var newClusters = GenerateClusters(modPrefix, clusters);
            foreach (var element in newClusters)
                addElement.Add(element);

            var newGateConnections = GenerateGateConnections(modPrefix, clusters, nonModifiedBaseGameData);
            foreach (var element in newGateConnections)
                addElement.Add(element);

            return addElement.IsEmpty ? null : addElement;
        }

        private static IEnumerable<XElement> GenerateClusters(string modPrefix, List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters.Where(a => !a.IsBaseGame))
            {
                yield return new XElement("connection",
                    new XAttribute("name", $"{modPrefix}_CL_c{cluster.Id:D3}_connection"),
                    new XAttribute("ref", "clusters"),
                    new XElement("offset",
                        new XElement("position",
                            new XAttribute("x", cluster.Position.X * 15000 * 1000),
                            new XAttribute("y", 0),
                            new XAttribute("z", cluster.Position.Y * 8660 * 1000)
                        )
                    ),
                    new XElement("macro",
                        new XAttribute("ref", $"{modPrefix}_CL_c{cluster.Id:D3}_macro"),
                        new XAttribute("connection", "galaxy")
                    )
                );
            }
        }

        private static IEnumerable<XElement> GenerateGateConnections(string modPrefix, List<Cluster> clusters, ClusterCollection nonModifiedBaseGameData)
        {
            // Create a mapping of all vanilla gates
            var gatesCache = nonModifiedBaseGameData.Clusters
                .SelectMany(a => a.Sectors)
                .SelectMany(a => a.Zones)
                .SelectMany(a => a.Gates)
                .Select(a => a.ConnectionName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            HashSet<Gate> destinationGatesToBeSkipped = [];
            foreach (Cluster cluster in clusters)
            {
                foreach (Sector sector in cluster.Sectors.OrderBy(a => a.Id))
                {
                    foreach (Zone zone in sector.Zones.OrderBy(a => a.Id))
                    {
                        foreach (Gate gate in zone.Gates.OrderBy(a => a.Id))
                        {
                            if ((gate.ConnectionName != null && gatesCache.Contains(gate.ConnectionName)) || destinationGatesToBeSkipped.Contains(gate))
                                continue;

                            if (string.IsNullOrWhiteSpace(gate.SourcePath) ||
                                string.IsNullOrWhiteSpace(gate.DestinationPath))
                            {
                                throw new Exception($"Gate \"{cluster.Name}/{sector.Name}/z{zone.Id}/g{gate.Id:D3}\" source/destination path is not set.");
                            }

                            Sector sourceSector = clusters
                                .SelectMany(a => a.Sectors)
                                .First(a => a.Name.Equals(gate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                            Zone sourceZone = sourceSector.Zones
                                .First(a => a.Gates
                                    .Any(a => a.DestinationSectorName
                                        .Equals(gate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
                            Gate sourceGate = sourceZone.Gates.First(a => a.DestinationSectorName.Equals(gate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
                            _ = destinationGatesToBeSkipped.Add(sourceGate);

                            yield return new XElement("connection",
                                new XAttribute("name", $"{modPrefix}_GA_g{gate.Id:D3}_{gate.Source}_{gate.Destination}_connection"),
                                new XAttribute("ref", "destination"),
                                new XAttribute("path", $"../{gate.SourcePath.Replace("PREFIX", modPrefix)}"),
                                new XElement("macro",
                                    new XAttribute("connection", "destination"),
                                    new XAttribute("path", $"../../../../../{gate.DestinationPath.Replace("PREFIX", modPrefix)}")
                                )
                            );
                        }
                    }
                }
            }
        }

        private static IEnumerable<(string dlc, XElement element)> GenerateVanillaChanges(VanillaChanges vanillaChanges)
        {
            foreach (var cluster in vanillaChanges.RemovedClusters)
            {
                yield return (cluster.Dlc, new XElement("remove",
                    new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{cluster.BaseGameMapping.CapitalizeFirstLetter()}_connection']")));
            }
            foreach (var modification in vanillaChanges.ModifiedClusters)
            {
                var Old = modification.Old;
                var New = modification.New;
                if (Old.Position != New.Position) 
                {
                    // Exceptional case for cluster 0, 0 it has no offset properties defined
                    if (Old.BaseGameMapping.Equals("cluster_01", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return (Old.Dlc, new XElement("add",
                            new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{Old.BaseGameMapping.CapitalizeFirstLetter()}_connection']"),
                                new XElement("offset",
                                    new XElement("position",
                                        new XAttribute("x", New.Position.X * 15000 * 1000),
                                        new XAttribute("y", "0"),
                                        new XAttribute("z", New.Position.Y * 8660 * 1000)
                                    )
                                )
                            )
                        );
                    }
                    else
                    {
                        yield return (Old.Dlc, new XElement("replace",
                            new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{Old.BaseGameMapping.CapitalizeFirstLetter()}_connection']/offset/position/@x"),
                            New.Position.X * 15000 * 1000
                            )
                        );
                        yield return (Old.Dlc, new XElement("replace",
                            new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{Old.BaseGameMapping.CapitalizeFirstLetter()}_connection']/offset/position/@z"),
                            New.Position.Y * 8660 * 1000
                            )
                        );
                    }
                }
            }
            foreach (var removedConnection in vanillaChanges.RemovedConnections)
            {
                if (!removedConnection.Gate.IsHighwayGate)
                {
                    string connectionName = removedConnection.Gate.ConnectionName;

                    // Destination gate doesn't have a removeable entry and can be skipped
                    if (removedConnection.Gate.ConnectionName.Equals("destination", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Remove gate connection from galaxy
                    yield return (removedConnection.VanillaCluster.Dlc, new XElement("remove",
                        new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{connectionName}']")
                        )
                    );
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
