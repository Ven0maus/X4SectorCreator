using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using X4SectorCreator.Configuration;
using X4SectorCreator.Objects;
using Region = X4SectorCreator.Objects.Region;

namespace DevConsole.Extractors
{
    internal class RegionExtractor
    {
        private static readonly Dictionary<string, RegionDefinition> _regionDefintions = new(StringComparer.OrdinalIgnoreCase);

        internal static IEnumerable<Region> ExtractRegions(string xmlPath)
        {
            // Collect all clusters
            var xdoc = XDocument.Load(xmlPath);

            var clusters = new Dictionary<string, Cluster>(StringComparer.OrdinalIgnoreCase);
            var clusterMacros = ReadClusterMacros(xdoc)
                .Where(a => !string.IsNullOrWhiteSpace(a.Name))
                .ToList();

            // For each sector get the position
            foreach (var macro in clusterMacros)
            {
                var clusterName = macro.Name.Replace("_macro", string.Empty, StringComparison.OrdinalIgnoreCase);

                // Create cluster obj
                var cluster = new Cluster
                {
                    BaseGameMapping = clusterName,
                    Sectors = []
                };
                clusters[clusterName] = cluster;

                foreach (var group in macro.Connections
                    .Where(a => !string.IsNullOrWhiteSpace(a.Ref) && !string.IsNullOrWhiteSpace(a.Name))
                    .GroupBy(a => a.Ref)
                    .OrderBy(g => g.Key.Equals("sectors", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                    .ThenBy(g => g.Key))
                {
                    foreach (var connection in group)
                    {
                        if (group.Key.Equals("sectors", StringComparison.OrdinalIgnoreCase))
                        {
                            var sectorName = connection.Name
                                .Replace("_connection", string.Empty, StringComparison.OrdinalIgnoreCase)
                                .Replace(clusterName + "_", string.Empty, StringComparison.OrdinalIgnoreCase);

                            var sector = new Sector
                            {
                                BaseGameMapping = sectorName,
                                Regions = []
                            };
                            cluster.Sectors.Add(sector);

                            // Find offset
                            var offset = connection.Offset;
                            if (offset != null)
                                sector.Offset = ((long)offset.X, (long)offset.Z);
                        }
                        else if (group.Key.Equals("regions", StringComparison.OrdinalIgnoreCase))
                        {
                            // Skip invalid regions & audio regions
                            if (connection.Offset == null || string.IsNullOrWhiteSpace(connection.RegionRef) ||
                                connection.RegionRef.StartsWith("audio", StringComparison.OrdinalIgnoreCase))
                                continue;

                            var (X, Z) = ((long)connection.Offset.X, (long)connection.Offset.Z);

                            // Find the closest sector based on 2D XZ distance
                            Sector closestSector = null;
                            long minDistanceSquared = long.MaxValue;

                            foreach (var sector in cluster.Sectors)
                            {
                                long dx = X - sector.Offset.X;
                                long dz = Z - sector.Offset.Y;
                                long distanceSquared = dx * dx + dz * dz;

                                if (distanceSquared < minDistanceSquared)
                                {
                                    minDistanceSquared = distanceSquared;
                                    closestSector = sector;
                                }
                            }

                            if (closestSector != null)
                            {
                                var region = new Region
                                {
                                    Name = connection.Name,
                                    Position = new System.Drawing.Point((int)X, (int)Z)
                                };

                                if (!_regionDefintions.TryGetValue(connection.RegionRef, out var regionDefinition))
                                {
                                    _regionDefintions[connection.RegionRef] = regionDefinition = new RegionDefinition()
                                    {
                                        Name = connection.RegionRef
                                    };
                                }
   
                                region.Definition = regionDefinition;
                                closestSector.Regions.Add(region);
                            }
                        }
                    }
                }
            }

            // Store region extraction file
            var clusterCollection = new ClusterCollection { Clusters = clusters.Values.ToList() };
            var xml = JsonSerializer.Serialize(clusterCollection, ConfigSerializer.JsonSerializerOptions);
            if (!Directory.Exists(Path.GetDirectoryName(Path.Combine("Extractions", "ExtractedRegions.xml"))))
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine("Extractions", "ExtractedRegions.xml")));
            File.WriteAllText(Path.Combine("Extractions", "ExtractedRegions.xml"), xml);

            // Get all regions in sector

            // Create a definition if not exists

            // Create region object and assign to correct sector based on position

            // Export all 

            return [];
        }

        private static IEnumerable<Macro> ReadClusterMacros(XDocument doc)
        {
            var macroElements = doc.Element("macros").Elements("macro");
            foreach (var macroElement in macroElements)
            {
                yield return new Macro
                {
                    Name = macroElement.Attribute("name")?.Value,
                    Class = macroElement.Attribute("class")?.Value,
                    Connections = macroElement
                        .Element("connections")?
                        .Elements("connection")
                        .Select(conn => new Connection
                        {
                            Name = conn.Attribute("name")?.Value,
                            Ref = conn.Attribute("ref")?.Value,
                            RegionRef = conn.Element("macro")?.Element("properties")?.Element("region")?.Attribute("ref")?.Value,
                            Offset = conn.Element("offset")?.Element("position") is XElement pos
                                ? new Position
                                {
                                    X = double.Parse(pos.Attribute("x")?.Value ?? "0", CultureInfo.InvariantCulture),
                                    Y = double.Parse(pos.Attribute("y")?.Value ?? "0", CultureInfo.InvariantCulture),
                                    Z = double.Parse(pos.Attribute("z")?.Value ?? "0", CultureInfo.InvariantCulture),
                                }
                                : null
                        })
                        .ToList()
                };
            }
        }

        public class Position
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }

        public class Macro
        {
            public string Name { get; set; }
            public string Class { get; set; }
            public List<Connection> Connections { get; set; } = new List<Connection>();
        }

        public class Connection
        {
            public string Name { get; set; }
            public string Ref { get; set; }
            public Position Offset { get; set; }
            public string RegionRef { get; set; }
        }
    }
}
