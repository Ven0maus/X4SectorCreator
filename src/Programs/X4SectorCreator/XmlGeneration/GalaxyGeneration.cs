using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Helpers;
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
                XElement[] galaxyElements = GenerateClusters(modPrefix, orderedClusters)
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
                XElement[] elements = GenerateVanillaChanges(vanillaChanges)
                    .Append(GenerateNewContent(modPrefix, clusters, nonModifiedBaseGameData))
                    .Where(a => a != null)
                    .ToArray();
                if (elements.Length > 0)
                {
                    xmlDocument = new(new XDeclaration("1.0", "utf-8", null),
                        new XElement("diff", elements.Select(a => a))
                    );

                    xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/{GalaxySettingsForm.GalaxyName}/galaxy.xml")));
                }
            }
        }

        private static XElement GenerateNewContent(string modPrefix, List<Cluster> clusters, ClusterCollection nonModifiedBaseGameData)
        {
            XElement addElement = new("add",
                                new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections"));

            IEnumerable<XElement> newClusters = GenerateClusters(modPrefix, clusters);
            foreach (XElement element in newClusters)
            {
                addElement.Add(element);
            }

            IEnumerable<XElement> newGateConnections = GenerateGateConnections(modPrefix, clusters, nonModifiedBaseGameData);
            foreach (XElement element in newGateConnections)
            {
                addElement.Add(element);
            }

            return addElement.IsEmpty ? null : addElement;
        }

        private static IEnumerable<XElement> GenerateClusters(string modPrefix, List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters.Where(a => !a.IsBaseGame))
            {
                string connectionName = GetClusterConnectionName(modPrefix, cluster);
                string macroName = GetClusterMacroName(modPrefix, cluster);

                yield return new XElement("connection",
                    new XAttribute("name", connectionName),
                    new XAttribute("ref", "clusters"),
                    new XElement("offset",
                        new XElement("position",
                            new XAttribute("x", cluster.Position.X * 15000 * 1000),
                            new XAttribute("y", 0),
                            new XAttribute("z", cluster.Position.Y * 8660 * 1000)
                        )
                    ),
                    new XElement("macro",
                        new XAttribute("ref", macroName),
                        new XAttribute("connection", "galaxy")
                    )
                );
            }
        }

        private static string GetClusterConnectionName(string modPrefix, Cluster cluster)
        {
            if (!string.IsNullOrWhiteSpace(cluster.ImportedConnectionName))
                return cluster.ImportedConnectionName.Replace("PREFIX", modPrefix);

            return $"{modPrefix}_CL_c{cluster.Id:D3}_connection";
        }

        private static string GetClusterMacroName(string modPrefix, Cluster cluster)
        {
            if (!string.IsNullOrWhiteSpace(cluster.ImportedMacroName))
                return cluster.ImportedMacroName.Replace("PREFIX", modPrefix);

            return $"{modPrefix}_CL_c{cluster.Id:D3}_macro";
        }

        private static IEnumerable<XElement> GenerateGateConnections(string modPrefix, List<Cluster> clusters, ClusterCollection nonModifiedBaseGameData)
        {
            // Create a mapping of all vanilla gates
            HashSet<string> gatesCache = nonModifiedBaseGameData.Clusters
                .SelectMany(a => a.Sectors)
                .SelectMany(a => a.Zones)
                .SelectMany(a => a.Gates)
                .Select(a => a.ConnectionName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var gateLookup = clusters
                .SelectMany(cluster => cluster.Sectors, (cluster, sector) => (cluster, sector))
                .SelectMany(pair => pair.sector.Zones, (pair, zone) => (pair.cluster, pair.sector, zone))
                .SelectMany(pair => pair.zone.Gates, (pair, gate) => new GateReference(pair.cluster, pair.sector, pair.zone, gate))
                .ToList();
            var sourcePathLookup = GateConnectionResolver.BuildSourcePathLookup(gateLookup, a => a.Gate.SourcePath);

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
                            {
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(gate.SourcePath) ||
                                string.IsNullOrWhiteSpace(gate.DestinationPath))
                            {
                                throw new Exception($"Gate \"{cluster.Name}/{sector.Name}/z{zone.Id}/g{gate.Id:D3}\" source/destination path is not set.");
                            }

                            if (!GateConnectionResolver.TryResolveTarget(sourcePathLookup, gate.DestinationPath, out GateReference sourceReference))
                            {
                                throw new Exception(
                                    $"Gate \"{cluster.Name}/{sector.Name}/z{zone.Id}/g{gate.Id:D3}\" could not resolve a reciprocal gate for destination path " +
                                    $"\"{gate.DestinationPath}\".");
                            }

                            _ = destinationGatesToBeSkipped.Add(sourceReference.Gate);

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

        private sealed record GateReference(Cluster Cluster, Sector Sector, Zone Zone, Gate Gate);

        private static IEnumerable<XElement> GenerateVanillaChanges(VanillaChanges vanillaChanges)
        {
            foreach (Cluster cluster in vanillaChanges.RemovedClusters)
            {
                yield return new XElement("remove",
                    new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{cluster.BaseGameMapping.CapitalizeFirstLetter()}_connection']"));
            }
            foreach (ModifiedCluster modification in vanillaChanges.ModifiedClusters)
            {
                Cluster Old = modification.Old;
                Cluster New = modification.New;
                if (Old.Position != New.Position)
                {
                    // Exceptional case for cluster 0, 0 it has no offset properties defined
                    if (Old.BaseGameMapping.Equals("cluster_01", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new XElement("add",
                            new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{Old.BaseGameMapping.CapitalizeFirstLetter()}_connection']"),
                                new XElement("offset",
                                    new XElement("position",
                                        new XAttribute("x", New.Position.X * 15000 * 1000),
                                        new XAttribute("y", "0"),
                                        new XAttribute("z", New.Position.Y * 8660 * 1000)
                                    )
                                )
                            );
                    }
                    else
                    {
                        yield return new XElement("replace",
                            new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{Old.BaseGameMapping.CapitalizeFirstLetter()}_connection']/offset/position/@x"),
                            New.Position.X * 15000 * 1000
                            );
                        yield return new XElement("replace",
                            new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{Old.BaseGameMapping.CapitalizeFirstLetter()}_connection']/offset/position/@z"),
                            New.Position.Y * 8660 * 1000
                            );
                    }
                }
            }
            foreach (RemovedConnection removedConnection in vanillaChanges.RemovedConnections)
            {
                if (!removedConnection.Gate.IsHighwayGate)
                {
                    string connectionName = removedConnection.Gate.ConnectionName;

                    // Destination gate doesn't have a removeable entry and can be skipped
                    if (removedConnection.Gate.ConnectionName.Equals("destination", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // Remove gate connection from galaxy
                    yield return new XElement("remove",
                        new XAttribute("sel", $"/macros/macro[@name='XU_EP2_universe_macro']/connections/connection[@name='{connectionName}']")
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
