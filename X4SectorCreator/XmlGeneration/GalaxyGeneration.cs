using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class GalaxyGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            var orderedClusters = clusters.OrderBy(a => a.Id).ToList();
            XDocument xmlDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    new XElement("add",
                        new XAttribute("sel", "/macros/macro[@name='XU_EP2_universe_macro']/connections"),
                        GenerateClusters(modPrefix, orderedClusters),
                        GenerateGateConnections(modPrefix, orderedClusters)
                    )
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, "maps/xu_ep2_universe/galaxy.xml")));
        }

        private static IEnumerable<XElement> GenerateClusters(string modPrefix, List<Cluster> clusters)
        {
            foreach (var cluster in clusters)
            {
                yield return new XElement("connection",
                    new XAttribute("name", $"{modPrefix}_CL_c{cluster.Id:D3}_connection"),
                    new XAttribute("ref", "clusters"),
                    new XElement("offset",
                        new XElement("position",
                            new XAttribute("x", cluster.Position.X * 15000 * 1000),
                            new XAttribute("y", 0),
                            new XAttribute("z", cluster.Position.Y * 8860 * 1000)
                        )
                    ),
                    new XElement("macro",
                        new XAttribute("ref", $"{modPrefix}_CL_c{cluster.Id:D3}_macro"),
                        new XAttribute("connection", "galaxy")
                    )
                );
            }
        }

        private static IEnumerable<XElement> GenerateGateConnections(string modPrefix, List<Cluster> clusters)
        {
            var destinationGatesToBeSkipped = new HashSet<Gate>();
            foreach (var cluster in clusters)
            {
                foreach (var sector in cluster.Sectors.OrderBy(a => a.Id))
                {
                    foreach (var zone in sector.Zones.OrderBy(a => a.Id))
                    {
                        foreach (var gate in zone.Gates.OrderBy(a => a.Id))
                        {
                            if (destinationGatesToBeSkipped.Contains(gate)) continue;
                            if (string.IsNullOrWhiteSpace(gate.SourcePath) ||
                                string.IsNullOrWhiteSpace(gate.DestinationPath))
                                throw new Exception($"Gate \"{cluster.Name}/{sector.Name}/{zone.Name}/g{gate.Id:D3}\" source/destination path is not set.");

                            var sourceSector = clusters
                                .SelectMany(a => a.Sectors)
                                .First(a => a.Name.Equals(gate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                            var sourceZone = sourceSector.Zones
                                .First(a => a.Gates
                                    .Any(a => a.DestinationSectorName
                                        .Equals(gate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
                            var sourceGate = sourceZone.Gates.First(a => a.DestinationSectorName.Equals(gate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
                            destinationGatesToBeSkipped.Add(sourceGate);

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

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            return filePath;
        }
    }
}
