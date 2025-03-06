using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    /// <summary>
    /// Parses the vanilla connection mappings into a valid gate objects into the correct sectors
    /// </summary>
    public static class VanillaGateConnectionParser
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

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

            var filePaths = new (string prefix, string path)[]
            {
                (null, GetFile(null)),
                ("dlc4_", GetFile(SectorMapForm.DlcMapping["Split Vendetta"])),
                ("dlc_pirate_", GetFile(SectorMapForm.DlcMapping["Tides Of Avarice"])),
                ("dlc_terran_", GetFile(SectorMapForm.DlcMapping["Cradle Of Humanity"])),
                ("dlc_boron_", GetFile(SectorMapForm.DlcMapping["Kingdom End"])),
                ("dlc7_", GetFile(SectorMapForm.DlcMapping["Timelines"])),
                //("dlc_mini01_", GetFile(SectorMapForm.DlcMapping["Hyperion Pack"])),
            };

            // Verify existance
            var nonExistant = filePaths
                .Where(a => !Directory.Exists(Path.GetDirectoryName(a.path)))
                .ToArray();
            if (nonExistant.Length > 0)
            {
                Console.WriteLine("Unable to generate vanilla gate connection mappings file. Missing directories:\n- " +
                    string.Join("\n- ", filePaths.Select(a => a.path)));
                return;
            }

            var connections = CollectGateConnections(filePaths.Select(a => a.path)).ToList();
            var zoneGateInfos = CollectZoneAndGateInformation(filePaths).ToList();
            var mapping = new VanilaConnectionMapping
            {
                Connections = connections,
                ZoneGateInfos = zoneGateInfos
            };

            // Generate the mappings based on these connections
            var json = JsonSerializer.Serialize(mapping, _serializerOptions);
            File.WriteAllText($"{_resultsPath}/vanilla_connection_mappings.json", json);
        }

        public static void CreateVanillaGateConnections(Dictionary<(int x, int y), Cluster> allClusters)
        {
            try
            {
                var mappingsPath = Path.Combine(Application.StartupPath, "Mappings/vanilla_connection_mappings.json");
                var json = File.ReadAllText(mappingsPath);
                var connections = JsonSerializer.Deserialize<List<Connection>>(json);

                var baseGameClusters = allClusters.Values
                    .Where(a => a.IsBaseGame)
                    .ToArray();

                foreach (var connection in connections)
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
                    var sourceCluster = baseGameClusters.First(a => a.BaseGameMapping.Equals(connection.Source.Cluster, StringComparison.OrdinalIgnoreCase));
                    var sourceSector = sourceCluster.Sectors.First(a => a.BaseGameMapping.Equals(connection.Source.Sector, StringComparison.OrdinalIgnoreCase));
                    var sourceZone = sourceSector.Zones.FirstOrDefault(a => a.Name.Equals(connection.Source.Zone, StringComparison.OrdinalIgnoreCase));

                    // Target
                    var targetCluster = baseGameClusters.First(a => a.BaseGameMapping.Equals(connection.Destination.Cluster, StringComparison.OrdinalIgnoreCase));
                    var targetSector = targetCluster.Sectors.First(a => a.BaseGameMapping.Equals(connection.Destination.Sector, StringComparison.OrdinalIgnoreCase));
                    var targetZone = targetSector.Zones.FirstOrDefault(a => a.Name.Equals(connection.Destination.Zone, StringComparison.OrdinalIgnoreCase));

                    // Source & Target connection
                    CreateGateConnection(sourceSector, sourceZone, connection.Source, connection.Destination, targetSector);
                    CreateGateConnection(targetSector, targetZone, connection.Destination, connection.Source, sourceSector);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Unable to create vanilla gate connections, the mapping file must be outdated or corrupted:\n{ex.Message}");
                return;
            }
        }

        private static void CreateGateConnection(Sector sector, Zone zone, ConnectionInfo sourceInfo, ConnectionInfo targetInfo, Sector targetSector)
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

            // Find dest sector name

            zone.Gates.Add(new Gate
            {
                ParentSectorName = sector.Name,
                DestinationSectorName = targetSector.Name,
                ConnectionName = sourceInfo.Name,
                SourcePath = sourceInfo.Path,
                DestinationPath = targetInfo.Path,
            });
        }

        private static string GetFile(string dlc)
        {
            return dlc == null ? $"{_readPath}/maps/xu_ep2_universe/" : $"{_readPath}/extensions/{dlc}/maps/xu_ep2_universe/";
        }

        private static IEnumerable<Connection> CollectGateConnections(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                var xDocument = XDocument.Load(filePath + "galaxy.xml");
                // Query all <connection> elements that contain a <macro> child with a 'connection' attribute
                var elements = xDocument.Descendants("connection")
                    .Where(c => c.Element("macro") != null && !c.Element("macro").Attribute("connection").Value.Equals("galaxy", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Output the connections that have a macro
                foreach (var element in elements)
                {
                    yield return ParseConnection(element);
                }
            }
        }

        private static IEnumerable<ZoneGateInfo> CollectZoneAndGateInformation(IEnumerable<(string prefix, string path)> filePaths)
        {
            foreach (var (prefix, path) in filePaths)
            {
                // TODO add right prefix per dlc
                var zonesXDoc = XDocument.Load($"{path}{prefix ?? ""}zones.xml");
                var zoneElements = zonesXDoc.Descendants("macro")
                    .Where(c => c.Attribute("class") != null && c.Attribute("class").Value
                        .Equals("zone", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // TODO add right prefix per dlc
                var sectorXDoc = XDocument.Load($"{path}{prefix ?? ""}sectors.xml");
                var sectorElements = sectorXDoc.Descendants("connection")
                    .Where(c => c.Attribute("ref") != null && c.Attribute("ref").Value.Equals("zones", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(a => a.Attribute("name").Value, a => a, StringComparer.OrdinalIgnoreCase);

                foreach (var zoneElement in zoneElements)
                {
                    // Gate inside the zone
                    var zoneGateInfo = ParseGate(zoneElement);
                    if (zoneGateInfo == null) continue;

                    // Find matching zone sector element
                    var matchingElementName = zoneElement.Attribute("name").Value.Replace("_macro", "_connection");
                    if (!sectorElements.TryGetValue(matchingElementName, out var sectorZoneElement))
                        throw new Exception("Unable to find: " + matchingElementName);

                    zoneGateInfo.ZoneName = matchingElementName;

                    if (sectorZoneElement.Element("offset")?.Element("position") != null)
                    {
                        var positionElement = sectorZoneElement.Element("offset").Element("position");
                        var x = (int)Math.Round(float.Parse(positionElement.Attribute("x").Value, CultureInfo.InvariantCulture));
                        var z = (int)Math.Round(float.Parse(positionElement.Attribute("z").Value, CultureInfo.InvariantCulture));
                        zoneGateInfo.ZonePosition = new CustomPoint(x, z);
                    }

                    yield return zoneGateInfo;
                }
            }
        }

        private static ZoneGateInfo ParseGate(XElement element)
        {
            var gateConnectionElement = element
                .Element("connections")?
                .Elements("connection")?
                .FirstOrDefault(a => a.Attribute("ref").Value.Equals("gates", StringComparison.OrdinalIgnoreCase));
            if (gateConnectionElement != null)
            {
                var gateTypeElement = gateConnectionElement.Element("macro")?.Attribute("ref").Value;
                if (gateConnectionElement.Element("offset")?.Element("position") != null)
                {
                    var positionElement = gateConnectionElement.Element("offset").Element("position");
                    var x = (int)Math.Round(float.Parse(positionElement.Attribute("x").Value, CultureInfo.InvariantCulture));
                    var z = (int)Math.Round(float.Parse(positionElement.Attribute("z").Value, CultureInfo.InvariantCulture));
                    return new ZoneGateInfo 
                    { 
                        GatePosition = new CustomPoint(x, z), 
                        GateType = gateTypeElement ?? "props_gates_anc_gate_macro", 
                        GateName = gateConnectionElement.Attribute("name").Value
                    };
                }
                return new ZoneGateInfo 
                { 
                    GateType = gateTypeElement ?? "props_gates_anc_gate_macro",
                    GateName = gateConnectionElement.Attribute("name").Value
                };
            }
            return null;
        }

        private static Connection ParseConnection(XElement connectionElement)
        {
            // Create the main connection object
            var connection = new Connection
            {
                Source = ParseConnectionInfo(connectionElement), // Parse source
                Destination = ParseMacroConnection(connectionElement) // Parse destination
            };

            return connection;
        }

        private static ConnectionInfo ParseConnectionInfo(XElement connectionElement)
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
            var macroElement = connectionElement.Element("macro");

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
        private static string ExtractCluster(string path) => ExtractPart(path, "Cluster");
        private static string ExtractSector(string path) => ExtractPart(path, "Sector");
        private static string ExtractZone(string path) => ExtractPart(path, "Zone");
        private static string ExtractGate(string path) => ExtractPart(path, "connection");

        private static string ExtractPart(string path, string keyword)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var parts = path.Split('/');
            foreach (var part in parts)
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
        public CustomPoint GatePosition { get; set; }
        public CustomPoint ZonePosition { get; set; }
        public string GateType { get; set; }
        public string GateName { get; set; }
        public string ZoneName { get; set; }
    }

    public readonly struct CustomPoint(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
    }
}
