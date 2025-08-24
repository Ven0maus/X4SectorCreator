using System.Drawing;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using X4SectorCreator.Configuration;
using X4SectorCreator.Objects;

namespace DevConsole.Extractors
{
    internal class StationExtractor
    {
        internal static void ExtractStations(string clustersPath, string sectorsPath, string godPath)
        {
            List<Station> stationObjects = CollectStations(godPath);
            var clusters = CollectClustersAndSectors(clustersPath);
            var zonePositions = CollectZonePositions(sectorsPath);

            foreach (var station in stationObjects)
            {
                // Place station in the correct sector
                bool found = false;
                foreach (var cluster in clusters.Values)
                {
                    foreach (var sector in cluster.Sectors)
                    {
                        if (station.Location.Contains($"{cluster.BaseGameMapping}_{sector.BaseGameMapping}", StringComparison.OrdinalIgnoreCase))
                        {
                            var zone = new Zone
                            {
                                Name = Guid.NewGuid().ToString(),
                                Stations = [station]
                            };

                            if (station.LocationType.Equals("zone", StringComparison.OrdinalIgnoreCase))
                            {
                                // Find the zone's position and include also the custom position if defined
                                if (zonePositions.TryGetValue(station.Location, out var zonePosition))
                                {
                                    zone.Position = new Point(
                                        (int)Math.Round(zonePosition.X, 0), 
                                        (int)Math.Round(zonePosition.Y, 0));
                                }
                            }
                            else
                            {
                                // Use the defined position on the station object
                                zone.Position = station.Position;
                            }

                            sector.Zones ??= [];
                            sector.Zones.Add(zone);
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }

                if (!found)
                    throw new Exception($"No sector found for station \"{station.Id}\": {station.LocationType} | {station.Location}");
            }

            Console.WriteLine($"Exported {clusters.Values.SelectMany(a => a.Sectors).SelectMany(a => a.Zones).SelectMany(a => a.Stations).Count()} stations.");

            // Store stations extraction file
            var clusterCollection = new ClusterCollection { Clusters = [.. clusters.Values] };
            var xml = JsonSerializer.Serialize(clusterCollection, ConfigSerializer.JsonSerializerOptions);
            if (!Directory.Exists(Path.GetDirectoryName(Path.Combine("Extractions", "ExtractedStations.xml"))))
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine("Extractions", "ExtractedStations.xml")));
            File.WriteAllText(Path.Combine("Extractions", "ExtractedStations.xml"), xml);
        }

        private static List<Station> CollectStations(string godPath)
        {
            var xdoc = XDocument.Load(godPath);
            var stations = xdoc.Element("god").Element("stations").Elements("station");

            var stationObjects = new List<Station>();
            foreach (var station in stations)
            {
                var location = station.Element("location");
                if (location == null) continue;

                var id = station.Attribute("id").Value;
                var race = station.Attribute("race").Value;
                var owner = station.Attribute("owner").Value;

                var locationType = location.Attribute("class")?.Value;
                if (locationType == null) continue;

                var locationMacro = location.Attribute("macro").Value;

                var positionElement = station.Element("position");
                Point? position = null;
                if (positionElement != null)
                {
                    var xStr = positionElement.Attribute("x").Value;
                    var zStr = positionElement.Attribute("z").Value;
                    if (float.TryParse(xStr, CultureInfo.InvariantCulture, out var x) && 
                        float.TryParse(zStr, CultureInfo.InvariantCulture, out var z))
                    {
                        position = new Point((int)Math.Round(x, 0), (int)Math.Round(z, 0));
                    }
                }

                var stationElement = station.Element("station");
                if (stationElement == null) continue;

                var select = stationElement.Element("select");
                if (select == null) continue;

                var factionBlueprint = select.Attribute("faction").Value;
                var type = GetTypeFromTags(id, select.Attribute("tags")?.Value ?? string.Empty);

                stationObjects.Add(new Station
                {
                    Faction = factionBlueprint,
                    Name = id,
                    Owner = owner,
                    Position = position ?? Point.Empty,
                    Race = race,
                    Type = type,
                    LocationType = locationType,
                    Location = locationMacro
                });
            }

            var interestedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "wharf", "shipyard",
                "equipmentdock", "tradestation",
                //"defence"
            };

            // Delete stations that we're not interested in
            _ = stationObjects.RemoveAll(a => !interestedTypes.Contains(a.Type));
            return stationObjects;
        }

        private static Dictionary<string, PointF> CollectZonePositions(string sectorsPath)
        {
            var xdoc = XDocument.Load(sectorsPath);
            var zonePositions = new Dictionary<string, PointF>(StringComparer.OrdinalIgnoreCase);

            var zones = xdoc.Element("macros").Elements("macro");
            foreach (var zone in zones)
            {
                var connections = zone.Element("connections").Elements("connection");
                foreach (var connection in connections)
                {
                    var macro = connection.Element("macro");
                    if (macro == null) continue;

                    var zoneRef = macro.Attribute("ref")?.Value;
                    if (string.IsNullOrWhiteSpace(zoneRef)) continue;

                    PointF? position = null;
                    var offset = connection.Element("offset")?.Element("position");
                    if (offset != null)
                    {
                        var xStr = offset.Attribute("x")?.Value;
                        var zStr = offset.Attribute("z")?.Value;
                        if (float.TryParse(xStr, CultureInfo.InvariantCulture, out var x) &&
                            float.TryParse(zStr, CultureInfo.InvariantCulture, out var z))
                        {
                            position = new PointF(x, z);
                        }
                    }

                    zonePositions[zoneRef] = position ?? PointF.Empty;
                }
            }

            return zonePositions;
        }

        private static Dictionary<string, Cluster> CollectClustersAndSectors(string clustersPath)
        {
            var xdoc = XDocument.Load(clustersPath);
            var clusters = new Dictionary<string, Cluster>(StringComparer.OrdinalIgnoreCase);
            var clusterMacros = RegionExtractor.ReadClusterMacros(xdoc)
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
                        }
                    }
                }
            }

            return clusters;
        }

        private static string GetTypeFromTags(string id, string tags)
        {
            var allTags = tags.Trim().Replace("[", string.Empty).Replace("]", string.Empty).Split(',');
            if (allTags.Length > 1)
            {
                throw new Exception($"Found multiple tags in station \"{id}\": {tags}");
            }
            else if (allTags.Length == 0)
            {
                throw new Exception($"No tags found in station \"{id}\": {tags}");
            }
            return allTags.First();
        }
    }
}
