using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Xml.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    /// <summary>
    /// Parses the vanilla connection mappings into a valid gate objects into the correct sectors
    /// </summary>
    public static class VanillaGateConnectionParser
    {
        private static string _readPath;
        private static string _resultsPath;

        /// <summary>
        /// This is a method, only for development use to create a mapping file from the game's vanilla gate connections.
        /// </summary>
        /// <param name="readPath"></param>
        /// <param name="resultsPath"></param>
        public static void GenerateGateConnectionMappings(string readPath, string resultsPath)
        {
            _readPath = readPath;
            _resultsPath = resultsPath;

            (string prefix, string path)[] filePaths =
            [
                (null, GetFile(null)),
                ("dlc4_", GetFile(SectorMapForm.DlcMapping["Split Vendetta"])),
                ("dlc_pirate_", GetFile(SectorMapForm.DlcMapping["Tides Of Avarice"])),
                ("dlc_terran_", GetFile(SectorMapForm.DlcMapping["Cradle Of Humanity"])),
                ("dlc_boron_", GetFile(SectorMapForm.DlcMapping["Kingdom End"])),
                ("dlc7_", GetFile(SectorMapForm.DlcMapping["Timelines"])),
                ("dlc_mini_01_", GetFile(SectorMapForm.DlcMapping["Hyperion Pack"])),
            ];

            // Verify existance
            (string prefix, string path)[] nonExistant = [.. filePaths.Where(a => !Directory.Exists(Path.GetDirectoryName(a.path)))];
            if (nonExistant.Length > 0)
            {
                Console.WriteLine("Unable to generate vanilla gate connection mappings file. Missing directories:\n- " +
                    string.Join("\n- ", filePaths.Select(a => a.path)));
                return;
            }

            List<Connection> connections = CollectGateConnections(filePaths.Select(a => a.path))
                .Concat(CollectHighwayConnections(filePaths))
                .ToList();
            List<ZoneGateInfo> zoneGateInfos = CollectZoneAndGateInformation(filePaths).ToList();
            VanilaConnectionMapping mapping = new()
            {
                Connections = connections,
                ZoneGateInfos = zoneGateInfos
            };

            // Generate the mappings based on these connections
            string json = JsonSerializer.Serialize(mapping, ConfigSerializer.JsonSerializerOptions);
            File.WriteAllText($"{_resultsPath}/vanilla_connection_mappings.json", json);
        }

        public static void CreateVanillaGateConnections(Dictionary<(int x, int y), Cluster> allClusters)
        {
            try
            {
                string json = File.ReadAllText(Constants.DataPaths.VanillaConnectionMappingFilePath);
                VanilaConnectionMapping vanillaMapping = JsonSerializer.Deserialize<VanilaConnectionMapping>(json, ConfigSerializer.JsonSerializerOptions);

                Cluster[] baseGameClusters = [.. allClusters.Values.Where(a => a.IsBaseGame)];

                foreach (Connection connection in vanillaMapping.Connections)
                {
                    if (connection.IsHighway)
                    {
                        SetupHighwayConnection(vanillaMapping, baseGameClusters, connection);
                    }
                    else
                    {
                        SetupGateConnection(vanillaMapping, baseGameClusters, connection);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Unable to create vanilla connections, the mapping file must be outdated or corrupted:\n{ex.Message}");
                throw;
            }
        }

        private static void SetupHighwayConnection(VanilaConnectionMapping vanillaMapping, Cluster[] baseGameClusters, Connection connection)
        {
            // Cleanup the data
            connection.Source.Zone = connection.Source.Zone.Replace("_connection", string.Empty);
            connection.Destination.Zone = connection.Destination.Zone.Replace("_connection", string.Empty);

            // Source
            Cluster sourceCluster = baseGameClusters.First(a => a.BaseGameMapping.Equals(connection.Source.Cluster, StringComparison.OrdinalIgnoreCase));
            Sector sourceSector = sourceCluster.Sectors.First(a => a.BaseGameMapping.Equals(connection.Source.Sector, StringComparison.OrdinalIgnoreCase));
            Zone sourceZone = sourceSector.Zones.FirstOrDefault(a => a.Name.Equals(connection.Source.Zone, StringComparison.OrdinalIgnoreCase));

            // Target
            Cluster targetCluster = baseGameClusters.First(a => a.BaseGameMapping.Equals(connection.Destination.Cluster, StringComparison.OrdinalIgnoreCase));
            Sector targetSector = targetCluster.Sectors.First(a => a.BaseGameMapping.Equals(connection.Destination.Sector, StringComparison.OrdinalIgnoreCase));
            Zone targetZone = targetSector.Zones.FirstOrDefault(a => a.Name.Equals(connection.Destination.Zone, StringComparison.OrdinalIgnoreCase));

            // Can't filter out connections we already made (highways have always two 1to1 connections)
            // Because the user needs option to delete both of them.. These are rendered out on the sector map atleast.

            // Collect known zone and gate infos for the given source & target
            ZoneGateInfo[] sourceZoneGateInfo = CollectZoneGateInfoForHighwaySector(vanillaMapping.ZoneGateInfos, sourceCluster, sourceSector).ToArray();
            ZoneGateInfo[] targetZoneGateInfo = CollectZoneGateInfoForHighwaySector(vanillaMapping.ZoneGateInfos, targetCluster, targetSector).ToArray();

            // We use the regular gates to determine the radius, including the highways
            ZoneGateInfo[] sourceRegularGateInfo = CollectZoneGateInfoForSector(vanillaMapping.ZoneGateInfos, sourceCluster, sourceSector).Concat(sourceZoneGateInfo).ToArray();
            ZoneGateInfo[] targetRegularGateInfo = CollectZoneGateInfoForSector(vanillaMapping.ZoneGateInfos, targetCluster, targetSector).Concat(targetZoneGateInfo).ToArray();

            // Determine radius for source & target sector based on known zone and gate positions
            sourceSector.DiameterRadius = DetermineRadius(sourceRegularGateInfo) * 2;
            targetSector.DiameterRadius = DetermineRadius(sourceRegularGateInfo) * 2;

            // Source & Target connection
            CreateGateConnection(sourceSector, sourceZone, connection.Source, sourceZoneGateInfo, connection.Destination, targetSector, true);
            CreateGateConnection(targetSector, targetZone, connection.Destination, targetZoneGateInfo, connection.Source, sourceSector, true);
        }

        private static void SetupGateConnection(VanilaConnectionMapping vanillaMapping, Cluster[] baseGameClusters, Connection connection)
        {
            // Cleanup the data
            connection.Source.Cluster = connection.Source.Cluster.Replace("_connection", string.Empty);
            connection.Source.Sector = connection.Source.Sector.Replace("_connection", string.Empty).Replace(connection.Source.Cluster + "_", string.Empty);
            connection.Source.Zone = connection.Source.Zone.Replace("_connection", string.Empty);
            connection.Source.Gate = connection.Source.Gate.Replace("_connection", string.Empty);

            connection.Destination.Cluster = connection.Destination.Cluster.Replace("_connection", string.Empty);
            connection.Destination.Sector = connection.Destination.Sector.Replace("_connection", string.Empty).Replace(connection.Destination.Cluster + "_", string.Empty);
            connection.Destination.Zone = connection.Destination.Zone.Replace("_connection", string.Empty);
            connection.Destination.Gate = connection.Destination.Gate.Replace("_connection", string.Empty);

            // Source
            Cluster sourceCluster = baseGameClusters.First(a => a.BaseGameMapping.Equals(connection.Source.Cluster, StringComparison.OrdinalIgnoreCase));
            Sector sourceSector = sourceCluster.Sectors.First(a => a.BaseGameMapping.Equals(connection.Source.Sector, StringComparison.OrdinalIgnoreCase));
            Zone sourceZone = sourceSector.Zones.FirstOrDefault(a => a.Name.Equals(connection.Source.Zone, StringComparison.OrdinalIgnoreCase));

            // Target
            Cluster targetCluster = baseGameClusters.First(a => a.BaseGameMapping.Equals(connection.Destination.Cluster, StringComparison.OrdinalIgnoreCase));
            Sector targetSector = targetCluster.Sectors.First(a => a.BaseGameMapping.Equals(connection.Destination.Sector, StringComparison.OrdinalIgnoreCase));
            Zone targetZone = targetSector.Zones.FirstOrDefault(a => a.Name.Equals(connection.Destination.Zone, StringComparison.OrdinalIgnoreCase));

            // Collect known zone and gate infos for the given source & target
            ZoneGateInfo[] sourceZoneGateInfo = CollectZoneGateInfoForSector(vanillaMapping.ZoneGateInfos, sourceCluster, sourceSector).ToArray();
            ZoneGateInfo[] targetZoneGateInfo = CollectZoneGateInfoForSector(vanillaMapping.ZoneGateInfos, targetCluster, targetSector).ToArray();

            // Determine radius for source & target sector based on known zone and gate positions
            sourceSector.DiameterRadius = DetermineRadius(sourceZoneGateInfo) * 2;
            targetSector.DiameterRadius = DetermineRadius(targetZoneGateInfo) * 2;

            // Source & Target connection
            CreateGateConnection(sourceSector, sourceZone, connection.Source, sourceZoneGateInfo, connection.Destination, targetSector, false);
            CreateGateConnection(targetSector, targetZone, connection.Destination, targetZoneGateInfo, connection.Source, sourceSector, false);
        }

        private static int DetermineRadius(ZoneGateInfo[] zoneGateInfos)
        {
            return zoneGateInfos
                .Select(z => Math.Max(Math.Abs(z.ZonePosition.X), Math.Abs(z.ZonePosition.Y))) // Get max of abs(X) or abs(Z)
                .DefaultIfEmpty(0) // Ensure there's a default value if empty
                .Max() + 250000; // 250km padding
        }

        private static IEnumerable<ZoneGateInfo> CollectZoneGateInfoForSector(List<ZoneGateInfo> zoneGateInfos, Cluster cluster, Sector sector)
        {
            foreach (ZoneGateInfo zoneGateInfo in zoneGateInfos)
            {
                if (zoneGateInfo.ZoneName.EndsWith(cluster.BaseGameMapping + "_" + sector.BaseGameMapping + "_connection", StringComparison.OrdinalIgnoreCase))
                {
                    yield return zoneGateInfo;
                }
            }
        }

        private static IEnumerable<ZoneGateInfo> CollectZoneGateInfoForHighwaySector(List<ZoneGateInfo> zoneGateInfos, Cluster cluster, Sector sector)
        {
            foreach (ZoneGateInfo zoneGateInfo in zoneGateInfos)
            {
                if (zoneGateInfo.ZoneName.StartsWith("tzone" + cluster.BaseGameMapping + "_" + sector.BaseGameMapping, StringComparison.OrdinalIgnoreCase))
                {
                    yield return zoneGateInfo;
                }
            }
        }

        private static void CreateGateConnection(Sector sector, Zone zone, ConnectionInfo sourceInfo,
            ZoneGateInfo[] sourceZoneGateInfo, ConnectionInfo targetInfo, Sector targetSector, bool isHighwayGate)
        {
            if (zone == null)
            {
                zone = new Zone
                {
                    Name = sourceInfo.Zone,
                    Gates = []
                };
                sector.Zones.Add(zone);
            }

            // Find the matching zoneGateInfo for the source zone + set position
            ZoneGateInfo zoneGateInfo = sourceZoneGateInfo.First(a => a.ZoneName.Replace("_connection", string.Empty).Equals(sourceInfo.Zone, StringComparison.OrdinalIgnoreCase));
            zone.Position = new Point(zoneGateInfo.ZonePosition.X, zoneGateInfo.ZonePosition.Y);

            zone.Gates.Add(new Gate
            {
                ParentSectorName = sector.Name,
                DestinationSectorName = targetSector.Name,
                ConnectionName = sourceInfo.Name,
                SourcePath = sourceInfo.Path,
                DestinationPath = targetInfo.Path,
                Position = new Point(zoneGateInfo.GatePosition?.X ?? 0, zoneGateInfo.GatePosition?.Y ?? 0),
                Roll = zoneGateInfo.Rotation?.X ?? 0,
                Pitch = zoneGateInfo.Rotation?.Y ?? 0,
                Yaw = zoneGateInfo.Rotation?.Z ?? 0,
                IsHighwayGate = isHighwayGate
            });
        }

        private static string GetFile(string dlc)
        {
            return dlc == null ? $"{_readPath}/maps/xu_ep2_universe/" : $"{_readPath}/extensions/{dlc}/maps/xu_ep2_universe/";
        }

        private static IEnumerable<Connection> CollectHighwayConnections(IEnumerable<(string dlc, string path)> filePaths)
        {
            foreach ((string prefix, string path) in filePaths)
            {
                XDocument xDocument = XDocument.Load($"{path}{prefix ?? ""}clusters.xml");
                // Query all <connection> elements that contain a <macro> child with a 'connection' attribute
                List<XElement> elements = xDocument.Descendants("connection")
                    .Where(c => c.Attribute("ref") != null && c.Attribute("ref").Value.Equals("sechighways", StringComparison.OrdinalIgnoreCase))
                    .Where(c => c.Element("macro") != null)
                    .ToList();

                // Output the connections that have a macro
                foreach (XElement element in elements)
                {
                    Connection highwayConnection = ParseHighwayConnection(element);
                    if (highwayConnection != null)
                    {
                        yield return highwayConnection;
                    }
                }
            }
        }

        private static IEnumerable<Connection> CollectGateConnections(IEnumerable<string> filePaths)
        {
            foreach (string filePath in filePaths)
            {
                XDocument xDocument = XDocument.Load(filePath + "galaxy.xml");
                // Query all <connection> elements that contain a <macro> child with a 'connection' attribute
                List<XElement> elements = xDocument.Descendants("connection")
                    .Where(c => c.Element("macro") != null && !c.Element("macro").Attribute("connection").Value.Equals("galaxy", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Output the connections that have a macro
                foreach (XElement element in elements)
                {
                    yield return ParseGateConnection(element);
                }
            }
        }

        private static IEnumerable<ZoneGateInfo> CollectZoneAndGateInformation(IEnumerable<(string prefix, string path)> filePaths)
        {
            foreach ((string prefix, string path) in filePaths)
            {
                XDocument zonesXDoc = XDocument.Load($"{path}{prefix ?? ""}zones.xml");
                List<XElement> zoneElements = zonesXDoc.Descendants("macro")
                    .Where(c => c.Attribute("class") != null && c.Attribute("class").Value
                        .Equals("zone", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                XDocument sectorXDoc = XDocument.Load($"{path}{prefix ?? ""}sectors.xml");
                Dictionary<string, XElement> sectorElements = sectorXDoc.Descendants("connection")
                    .Where(c => c.Attribute("ref") != null && c.Attribute("ref").Value.Equals("zones", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(a => a.Attribute("name").Value, a => a, StringComparer.OrdinalIgnoreCase);

                foreach (XElement zoneElement in zoneElements)
                {
                    // Gate inside the zone
                    ZoneGateInfo zoneGateInfo = ParseGate(zoneElement);

                    // Find matching zone sector element
                    string matchingElementName = zoneElement.Attribute("name").Value.Replace("_macro", "_connection");
                    if (!sectorElements.TryGetValue(matchingElementName, out XElement sectorZoneElement))
                    {
                        throw new Exception("Unable to find: " + matchingElementName);
                    }

                    zoneGateInfo.ZoneName = matchingElementName;

                    if (sectorZoneElement.Element("offset")?.Element("position") != null)
                    {
                        XElement positionElement = sectorZoneElement.Element("offset").Element("position");
                        int x = (int)Math.Round(float.Parse(positionElement.Attribute("x").Value, CultureInfo.InvariantCulture));
                        int z = (int)Math.Round(float.Parse(positionElement.Attribute("z").Value, CultureInfo.InvariantCulture));
                        zoneGateInfo.ZonePosition = new CustomPoint(x, z);
                    }

                    yield return zoneGateInfo;
                }
            }
        }

        private static ZoneGateInfo ParseGate(XElement element)
        {
            ZoneGateInfo zoneGateInfo = new();
            XElement gateConnectionElement = element
                .Element("connections")?
                .Elements("connection")?
                .FirstOrDefault(a => a.Attribute("ref").Value.Equals("gates", StringComparison.OrdinalIgnoreCase));
            if (gateConnectionElement != null)
            {
                string gateTypeElement = gateConnectionElement.Element("macro")?.Attribute("ref").Value;

                zoneGateInfo = new ZoneGateInfo
                {
                    GateName = gateConnectionElement.Attribute("name").Value,
                    GateType = gateTypeElement ?? "props_gates_anc_gate_macro"
                };

                if (gateConnectionElement.Element("offset")?.Element("position") != null)
                {
                    XElement positionElement = gateConnectionElement.Element("offset").Element("position");
                    int x = (int)Math.Round(float.Parse(positionElement.Attribute("x").Value, CultureInfo.InvariantCulture));
                    int z = (int)Math.Round(float.Parse(positionElement.Attribute("z").Value, CultureInfo.InvariantCulture));
                    zoneGateInfo.GatePosition = new CustomPoint(x, z);
                }
                if (gateConnectionElement.Element("offset")?.Element("quaternion") != null)
                {
                    XElement positionElement = gateConnectionElement.Element("offset").Element("quaternion");
                    float qx = float.Parse(positionElement.Attribute("qx").Value, CultureInfo.InvariantCulture);
                    float qy = float.Parse(positionElement.Attribute("qy").Value, CultureInfo.InvariantCulture);
                    float qz = float.Parse(positionElement.Attribute("qz").Value, CultureInfo.InvariantCulture);
                    float qw = float.Parse(positionElement.Attribute("qw").Value, CultureInfo.InvariantCulture);
                    Quaternion quaternion = new(qx, qy, qz, qw);
                    zoneGateInfo.Rotation = quaternion.ToEulerAngles();
                }
            }
            return zoneGateInfo;
        }

        private static Connection ParseHighwayConnection(XElement connectionElement)
        {
            IEnumerable<IGrouping<string, XElement>> groups = connectionElement.Element("macro").Descendants("connection")
                .Where(a => a.Attribute("ref") != null)
                .GroupBy(a => a.Attribute("ref").Value);

            XElement entryPoint = null, exitPoint = null;
            foreach (IGrouping<string, XElement> group in groups)
            {
                if (group.Key.Equals("entrypoint", StringComparison.OrdinalIgnoreCase))
                {
                    entryPoint = group.First();
                }
                else if (group.Key.Equals("exitpoint", StringComparison.OrdinalIgnoreCase))
                {
                    exitPoint = group.First();
                }
            }

            if (entryPoint == null || exitPoint == null)
            {
                return null;
            }

            // Create the main connection object
            Connection connection = new()
            {
                Source = ParseHighwayConnectionInfo(entryPoint, connectionElement), // Parse source
                Destination = ParseHighwayConnectionInfo(exitPoint, connectionElement), // Parse destination
                IsHighway = true
            };

            return connection;
        }

        private static Connection ParseGateConnection(XElement connectionElement)
        {
            // Create the main connection object
            Connection connection = new()
            {
                Source = ParseGateConnectionInfo(connectionElement), // Parse source
                Destination = ParseMacroConnection(connectionElement), // Parse destination
                IsHighway = false
            };

            return connection;
        }

        private static ConnectionInfo ParseHighwayConnectionInfo(XElement point, XElement connectionElement)
        {
            XElement macro = point.Element("macro");
            string clusterString = ExtractCluster(macro.Attribute("path")?.Value);

            // Split cluster and sector from the main clusterString
            string cluster = null, sector = null;
            if (clusterString != null)
            {
                string[] parts = clusterString.Replace("_connection", string.Empty).Split('_');
                if (parts.Length >= 3)
                {
                    cluster = $"{parts[0]}_{parts[1]}";
                    sector = parts[2];
                }
            }

            return new ConnectionInfo
            {
                Name = connectionElement.Attribute("name")?.Value,
                Path = macro.Attribute("path")?.Value,
                Cluster = cluster,
                Sector = sector,
                Zone = ExtractZone(macro.Attribute("path")?.Value)
            };
        }

        private static ConnectionInfo ParseGateConnectionInfo(XElement connectionElement)
        {
            return new ConnectionInfo
            {
                Name = connectionElement.Attribute("name")?.Value,
                Path = connectionElement.Attribute("path")?.Value,
                Cluster = ExtractCluster(connectionElement.Attribute("path")?.Value),
                Sector = ExtractSector(connectionElement.Attribute("path")?.Value),
                Zone = ExtractZone(connectionElement.Attribute("path")?.Value),
                Gate = ExtractGate(connectionElement.Attribute("path")?.Value)
            };
        }

        private static ConnectionInfo ParseMacroConnection(XElement connectionElement)
        {
            XElement macroElement = connectionElement.Element("macro");

            if (macroElement != null)
            {
                return new ConnectionInfo
                {
                    Name = macroElement.Attribute("connection")?.Value,  // For destination connection name
                    Path = macroElement.Attribute("path")?.Value,
                    Cluster = ExtractCluster(macroElement.Attribute("path")?.Value),
                    Sector = ExtractSector(macroElement.Attribute("path")?.Value),
                    Zone = ExtractZone(macroElement.Attribute("path")?.Value),
                    Gate = ExtractGate(macroElement.Attribute("path")?.Value)
                };
            }

            return null; // In case no macro connection exists
        }

        // Helper methods to extract components from the path
        private static string ExtractCluster(string path)
        {
            return ExtractPart(path, "Cluster");
        }

        private static string ExtractSector(string path)
        {
            return ExtractPart(path, "Sector");
        }

        private static string ExtractZone(string path)
        {
            return ExtractPart(path, "Zone");
        }

        private static string ExtractGate(string path)
        {
            return ExtractPart(path, "connection");
        }

        private static string ExtractPart(string path, string keyword)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            string[] parts = path.Split('/');
            foreach (string part in parts)
            {
                if (part.Contains(keyword))
                {
                    return part;
                }
            }
            return null;
        }
    }

    public class VanilaConnectionMapping
    {
        public List<Connection> Connections { get; set; }
        public List<ZoneGateInfo> ZoneGateInfos { get; set; }
    }

    public class Connection
    {
        public ConnectionInfo Source { get; set; }
        public ConnectionInfo Destination { get; set; }
        public bool IsHighway { get; set; }
    }

    public class ConnectionInfo
    {
        // Name is only filled in for the source
        public string Name { get; set; }
        public string Path { get; set; }
        public string Cluster { get; set; }
        public string Sector { get; set; }
        public string Zone { get; set; }
        public string Gate { get; set; }
    }

    public class ZoneGateInfo
    {
        public CustomPoint? GatePosition { get; set; }
        public CustomPoint ZonePosition { get; set; }
        public CustomVector? Rotation { get; set; }
        public string GateType { get; set; }
        public string GateName { get; set; }
        public string ZoneName { get; set; }
    }

    public struct CustomPoint(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
    }

    public struct CustomVector(int x, int y, int z)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public int Z { get; set; } = z;
    }
}
