using System.Diagnostics;
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

        internal static void ExtractRegions(string clustersPath, string definitionsPath)
        {
            // Collect all clusters
            var xdoc = XDocument.Load(clustersPath);

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
                            {
                                sector.SectorRealOffset = ((long)offset.X, (long)offset.Z);
                            }
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
                                long dx = X - sector.SectorRealOffset.X;
                                long dz = Z - sector.SectorRealOffset.Y;
                                long distanceSquared = dx * dx + dz * dz;

                                if (distanceSquared < minDistanceSquared)
                                {
                                    minDistanceSquared = distanceSquared;
                                    closestSector = sector;
                                }
                            }

                            if (closestSector != null)
                            {
                                // Create new region for sector
                                var region = new Region
                                {
                                    Name = connection.Name,
                                    Position = new System.Drawing.Point((int)X, (int)Z)
                                };

                                // Collect cached region definition
                                if (!_regionDefintions.TryGetValue(connection.RegionRef, out var regionDefinition))
                                {
                                    _regionDefintions[connection.RegionRef] = regionDefinition = CreateRegionDefinition(connection.RegionRef);
                                }
   
                                region.Definition = regionDefinition;
                                closestSector.Regions.Add(region);
                            }
                        }
                    }
                }
            }

            xdoc = XDocument.Load(definitionsPath);
            var definitions = ReadRegionDefintions(xdoc)
                .Where(a => !string.IsNullOrWhiteSpace(a.Name))
                .ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);

            var invalid = new HashSet<RegionDefinition>();
            foreach (var definition in _regionDefintions.Values)
            {
                if (!definitions.TryGetValue(definition.Name, out var regionObj))
                {
                    invalid.Add(definition);
                    continue;
                }

                definition.Resources = regionObj.Resources;
            }

            // Remove all regions that are invalid
            foreach (var invalidDefinition in invalid)
            {
                foreach (var cluster in clusters.Values)
                {
                    foreach (var sector in cluster.Sectors)
                    {
                        var matchingRegions = sector.Regions.Where(a => a.Definition == invalidDefinition).ToArray();
                        foreach (var region in matchingRegions)
                            sector.Regions.Remove(region);
                    }
                }
                _regionDefintions.Remove(invalidDefinition.Name);
            }

            // Set region boundary radius
            foreach (var cluster in clusters.Values)
            {
                foreach (var sector in cluster.Sectors)
                {
                    foreach (var region in sector.Regions)
                    {
                        if (definitions.TryGetValue(region.Definition.Name, out var def))
                        {
                            region.BoundaryRadius = def.BoundaryRadius;
                        }
                    }
                }
            }

            Console.WriteLine($"Exported {clusters.Values.SelectMany(a => a.Sectors).SelectMany(a => a.Regions).Count()} regions.");

            // Store region extraction file
            var clusterCollection = new ClusterCollection { Clusters = [.. clusters.Values] };
            var xml = JsonSerializer.Serialize(clusterCollection, ConfigSerializer.JsonSerializerOptions);
            if (!Directory.Exists(Path.GetDirectoryName(Path.Combine("Extractions", "ExtractedRegions.xml"))))
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine("Extractions", "ExtractedRegions.xml")));
            File.WriteAllText(Path.Combine("Extractions", "ExtractedRegions.xml"), xml);
        }

        private static RegionDefinition CreateRegionDefinition(string regionRef)
        {
            var regionDefinition = new RegionDefinition()
            {
                Name = regionRef,
                Guid = Guid.NewGuid().ToString()
            };
            return regionDefinition;
        }

        public static IEnumerable<Macro> ReadClusterMacros(XDocument doc)
        {
            var macroElements = doc.Element("macros").Elements("macro");
            foreach (var macroElement in macroElements)
            {
                yield return new Macro
                {
                    Name = macroElement.Attribute("name")?.Value,
                    Class = macroElement.Attribute("class")?.Value,
                    Connections = (macroElement
                        .Element("connections")?
                        .Elements("connection") ?? [])
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

        private static string ExtractBoundaryRadius(XElement region)
        {
            var sizeElement = region.Element("boundary")?.Element("size");
            var boundary = sizeElement?.Attribute("r")?.Value;
            if (boundary == null)
            {
                // Try multiple variant
                boundary = region.Element("boundaries")?
                    .Elements("boundary")?
                    .Select(a => a.Element("size")?.Attribute("r")?.Value)?
                    .Where(a => !string.IsNullOrWhiteSpace(a))?
                    .FirstOrDefault();

                if (boundary == null && sizeElement != null)
                {
                    // Try box type variant
                    var widthStr = sizeElement.Attribute("x")?.Value;
                    var heightStr = sizeElement.Attribute("z")?.Value;
                    if (widthStr != null && heightStr != null && 
                        float.TryParse(widthStr, out var width) && 
                        float.TryParse(heightStr, out var height))
                    {
                        float radius = (float)Math.Sqrt((width * width + height * height)) / 2f;
                        boundary = ((int)Math.Round(radius, 0)).ToString(CultureInfo.InvariantCulture);
                    }
                }

                if (boundary == null)
                {
                    Debug.WriteLine($"No boundary \"{region.Name}\".");
                }
            }
            return boundary;
        }

        private static IEnumerable<RegionObj> ReadRegionDefintions(XDocument doc)
        {
            var regions = doc.Element("regions").Elements("region");
            foreach (var region in regions)
            {
                var regionObj = new RegionObj
                {
                    Name = region.Attribute("name")?.Value,
                    Resources = [.. (region.Element("resources")?.Elements("resource") ?? [])
                        .Select(a => new Resource
                        {
                            Ware = a.Attribute("ware")?.Value,
                            Yield = a.Attribute("yield")?.Value
                        })]
                };

                // Skip resource less regions
                if (regionObj.Resources.Count == 0)
                    continue;

                // Extract also radius
                regionObj.BoundaryRadius = ExtractBoundaryRadius(region);

                yield return regionObj;
            }
        }

        public class RegionObj
        {
            public string Name { get; set; }
            public string BoundaryRadius { get; set; }
            public List<Resource> Resources { get; set; }
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
