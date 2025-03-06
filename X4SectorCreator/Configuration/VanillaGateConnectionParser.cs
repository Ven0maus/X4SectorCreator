using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace X4SectorCreator.Configuration
{
    /// <summary>
    /// This is a tool, only for development use to create a mapping file from the game's vanilla gate connections.
    /// </summary>
    public class VanillaGateConnectionParser
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        private static string _readPath;
        private static string _resultsPath;

        public static void GenerateGateConnectionMappings(string readPath, string resultsPath)
        {
            _readPath = readPath;
            _resultsPath = resultsPath;

            var mainPath = Path.Combine(Application.StartupPath);
            var filePaths = new string[]
            {
                GetFile(null),
                GetFile(SectorMapForm.DlcMapping["Split Vendetta"]),
                GetFile(SectorMapForm.DlcMapping["Tides Of Avarice"]),
                GetFile(SectorMapForm.DlcMapping["Cradle Of Humanity"]),
                GetFile(SectorMapForm.DlcMapping["Kingdom End"]),
                GetFile(SectorMapForm.DlcMapping["Timelines"]),
                //GetFile(SectorMapForm.DlcMapping["Hyperion Pack"]),
            };

            // Verify existance
            var nonExistant = filePaths
                .Where(a => !File.Exists(a))
                .ToArray();
            if (nonExistant.Length > 0)
            {
                Console.WriteLine("Unable to generate vanilla gate connection mappings file. Missing info files:\n- " +
                    string.Join("\n- ", filePaths));
                return;
            }

            var connections = CollectGateConnections(filePaths);

            // Generate the mappings based on these connections
            var json = JsonSerializer.Serialize(connections, _serializerOptions);
            File.WriteAllText($"{_resultsPath}/vanilla_connection_mappings.json", json);
        }

        private static string GetFile(string dlc)
        {
            return dlc == null ? $"{_readPath}/maps/xu_ep2_universe/galaxy.xml" : $"{_readPath}/extensions/{dlc}/maps/xu_ep2_universe/galaxy.xml";
        }

        private static IEnumerable<Connection> CollectGateConnections(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                var xDocument = XDocument.Load(filePath);
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
}
