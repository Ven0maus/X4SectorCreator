using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static partial class ModImportService
    {
        private const int ClusterPositionScaleX = 15000 * 1000;
        private const int ClusterPositionScaleY = 8660 * 1000;

        public static ModImportResult Import(string modDirectory, ClusterCollection vanillaClusterData)
        {
            if (string.IsNullOrWhiteSpace(modDirectory) || !Directory.Exists(modDirectory))
            {
                throw new DirectoryNotFoundException("The selected mod directory does not exist.");
            }

            var contentPath = Path.Combine(modDirectory, "content.xml");
            if (!File.Exists(contentPath))
            {
                throw new FileNotFoundException("The selected folder is not an X4 extension. Missing content.xml.", contentPath);
            }

            var mapsRoot = Path.Combine(modDirectory, "maps");
            if (!Directory.Exists(mapsRoot))
            {
                throw new DirectoryNotFoundException("The selected extension does not contain a maps folder.");
            }

            var modName = ReadModName(contentPath) ?? Path.GetFileName(modDirectory);

            var macros = new Dictionary<string, MacroDefinition>(StringComparer.OrdinalIgnoreCase);
            var galaxyConnections = new List<ConnectionDefinition>();
            foreach (var xmlPath in Directory.GetFiles(mapsRoot, "*.xml", SearchOption.AllDirectories))
            {
                CollectDefinitions(xmlPath, macros, galaxyConnections);
            }

            if (macros.Count == 0 && galaxyConnections.Count == 0)
            {
                throw new InvalidOperationException("No supported map XML content was found in the selected extension.");
            }

            var vanillaLookup = VanillaLookup.Create(vanillaClusterData);
            var customClusterPositions = galaxyConnections
                .Where(a => a.IsClusterConnection && !string.IsNullOrWhiteSpace(a.MacroRef))
                .GroupBy(a => a.MacroRef, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(a => a.Key, a => a.First().Offset, StringComparer.OrdinalIgnoreCase);

            var importedClusters = BuildImportedClusters(macros, vanillaLookup, customClusterPositions);
            if (importedClusters.Count == 0)
            {
                throw new InvalidOperationException("The extension did not contain any importable clusters, sectors, or zones.");
            }

            ResolveCustomClusterPositionCollisions(importedClusters, vanillaLookup);
            PairImportedGates(galaxyConnections, importedClusters);
            AssignImportedIds(importedClusters, vanillaLookup);
            RebuildImportedGatePaths(importedClusters);

            return new ModImportResult(modName, importedClusters);
        }

        private static List<Cluster> BuildImportedClusters(
            Dictionary<string, MacroDefinition> macros,
            VanillaLookup vanillaLookup,
            Dictionary<string, PositionDefinition> customClusterPositions)
        {
            var importedClusters = new Dictionary<string, Cluster>(StringComparer.OrdinalIgnoreCase);

            var clusterMacroNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var macro in macros.Values.Where(a => a.IsCluster))
            {
                clusterMacroNames.Add(macro.Name);
            }

            foreach (var macroName in customClusterPositions.Keys)
            {
                clusterMacroNames.Add(macroName);
            }

            foreach (var macro in macros.Values.Where(a => vanillaLookup.ClustersByMacroName.ContainsKey(a.Name) && a.Connections.Count > 0))
            {
                clusterMacroNames.Add(macro.Name);
            }

            foreach (var clusterMacroName in clusterMacroNames.OrderBy(a => a, StringComparer.OrdinalIgnoreCase))
            {
                var isBaseCluster = vanillaLookup.ClustersByMacroName.TryGetValue(clusterMacroName, out var vanillaCluster);
                var clusterMacro = macros.GetValueOrDefault(clusterMacroName);
                var cluster = isBaseCluster
                    ? new Cluster
                    {
                        BaseGameMapping = vanillaCluster.BaseGameMapping,
                        Name = vanillaCluster.Name,
                        Description = vanillaCluster.Description,
                        Dlc = vanillaCluster.Dlc,
                        Position = vanillaCluster.Position,
                        Sectors = []
                    }
                    : new Cluster
                    {
                        Name = clusterMacroName.Replace("_macro", string.Empty, StringComparison.OrdinalIgnoreCase),
                        Position = ConvertClusterPosition(customClusterPositions.GetValueOrDefault(clusterMacroName)),
                        Sectors = []
                    };

                var touchedBaseSectors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (clusterMacro != null)
                {
                    foreach (var connection in clusterMacro.Connections.Where(a => a.IsSectorConnection && !string.IsNullOrWhiteSpace(a.MacroRef)))
                    {
                        touchedBaseSectors.Add(connection.MacroRef);
                    }
                }

                foreach (var sectorMacroName in macros.Values
                    .Where(a => vanillaLookup.SectorsByMacroName.TryGetValue(a.Name, out var info) &&
                                info.Cluster.BaseGameMapping.Equals(cluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase) &&
                                a.Connections.Count > 0)
                    .Select(a => a.Name))
                {
                    touchedBaseSectors.Add(sectorMacroName);
                }

                foreach (var sectorMacroName in macros.Values
                    .Where(a => vanillaLookup.ZonesByMacroName.TryGetValue(a.Name, out var zoneInfo) &&
                                zoneInfo.Cluster.BaseGameMapping.Equals(cluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase) &&
                                a.Connections.Count > 0)
                    .Select(a => $"{cluster.BaseGameMapping.CapitalizeFirstLetter()}_{vanillaLookup.ZonesByMacroName[a.Name].Sector.BaseGameMapping.CapitalizeFirstLetter()}_macro"))
                {
                    touchedBaseSectors.Add(sectorMacroName);
                }

                if (clusterMacro != null)
                {
                    foreach (var sectorConnection in clusterMacro.Connections.Where(a => a.IsSectorConnection && !string.IsNullOrWhiteSpace(a.MacroRef)))
                    {
                        var sector = CreateImportedSector(sectorConnection.MacroRef, sectorConnection.Offset, cluster, macros, vanillaLookup);
                        if (sector != null)
                        {
                            cluster.Sectors.Add(sector);
                        }
                    }
                }

                if (isBaseCluster)
                {
                    foreach (var sectorMacroName in touchedBaseSectors)
                    {
                        if (cluster.Sectors.Any(a => a.BaseGameMapping != null &&
                                vanillaLookup.SectorsByMacroName.TryGetValue(sectorMacroName, out var sectorInfo) &&
                                a.BaseGameMapping.Equals(sectorInfo.Sector.BaseGameMapping, StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }

                        var sector = CreateImportedSector(sectorMacroName, null, cluster, macros, vanillaLookup);
                        if (sector != null)
                        {
                            cluster.Sectors.Add(sector);
                        }
                    }
                }

                if (cluster.Sectors.Count == 0)
                {
                    continue;
                }

                cluster.CustomSectorPositioning = cluster.Sectors.Count > 1;
                importedClusters[clusterMacroName] = cluster;
            }

            return importedClusters.Values.ToList();
        }

        private static Sector CreateImportedSector(
            string sectorMacroName,
            PositionDefinition sectorOffset,
            Cluster cluster,
            Dictionary<string, MacroDefinition> macros,
            VanillaLookup vanillaLookup)
        {
            var isBaseSector = vanillaLookup.SectorsByMacroName.TryGetValue(sectorMacroName, out var vanillaSectorInfo);
            var sectorMacro = macros.GetValueOrDefault(sectorMacroName);

            if (!isBaseSector && sectorMacro == null)
            {
                return null;
            }

            var sector = isBaseSector
                ? new Sector
                {
                    BaseGameMapping = vanillaSectorInfo.Sector.BaseGameMapping,
                    Name = vanillaSectorInfo.Sector.Name,
                    Description = vanillaSectorInfo.Sector.Description,
                    Placement = vanillaSectorInfo.Sector.Placement,
                    SectorRealOffset = vanillaSectorInfo.Sector.SectorRealOffset,
                    Zones = [],
                    Regions = [],
                    ResourceAreas = []
                }
                : new Sector
                {
                    Name = sectorMacroName.Replace("_macro", string.Empty, StringComparison.OrdinalIgnoreCase),
                    Placement = InferPlacement(sectorOffset),
                    CustomOffset = sectorOffset == null ? null : new Point((int)Math.Round(sectorOffset.X), (int)Math.Round(sectorOffset.Z)),
                    Zones = [],
                    Regions = [],
                    ResourceAreas = []
                };

            var zoneCandidates = new List<(string macroName, PositionDefinition offset)>();
            if (sectorMacro != null)
            {
                zoneCandidates.AddRange(sectorMacro.Connections
                    .Where(a => a.IsZoneConnection && !string.IsNullOrWhiteSpace(a.MacroRef))
                    .Select(a => (a.MacroRef, a.Offset)));
            }

            foreach (var zoneMacroName in macros.Values
                .Where(a => vanillaLookup.ZonesByMacroName.TryGetValue(a.Name, out var zoneInfo) &&
                            MatchesSector(zoneInfo, cluster, sector))
                .Select(a => a.Name))
            {
                if (zoneCandidates.Any(a => a.macroName.Equals(zoneMacroName, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                zoneCandidates.Add((zoneMacroName, null));
            }

            foreach (var (zoneMacroName, zoneOffset) in zoneCandidates
                .DistinctBy(a => a.macroName, StringComparer.OrdinalIgnoreCase))
            {
                var zone = CreateImportedZone(zoneMacroName, zoneOffset, cluster, sector, macros, vanillaLookup);
                if (zone != null)
                {
                    sector.Zones.Add(zone);
                }
            }

            return sector;
        }

        private static Zone CreateImportedZone(
            string zoneMacroName,
            PositionDefinition zoneOffset,
            Cluster cluster,
            Sector sector,
            Dictionary<string, MacroDefinition> macros,
            VanillaLookup vanillaLookup)
        {
            var isBaseZone = vanillaLookup.ZonesByMacroName.TryGetValue(zoneMacroName, out var vanillaZoneInfo) &&
                             MatchesSector(vanillaZoneInfo, cluster, sector);
            var zoneMacro = macros.GetValueOrDefault(zoneMacroName);

            if (!isBaseZone && zoneMacro == null)
            {
                return null;
            }

            var zone = new Zone
            {
                Name = isBaseZone ? vanillaZoneInfo.Zone.Name : null,
                Position = ConvertZonePosition(zoneOffset),
                Gates = []
            };

            if (zoneMacro == null)
            {
                return zone;
            }

            foreach (var gateConnection in zoneMacro.Connections.Where(a => a.IsGateConnection))
            {
                var gate = new Gate
                {
                    ConnectionName = gateConnection.Name,
                    Type = ParseGateType(gateConnection.MacroRef),
                    Pitch = gateConnection.Rotation?.Pitch ?? 0,
                    Roll = gateConnection.Rotation?.Roll ?? 0,
                    Yaw = gateConnection.Rotation?.Yaw ?? 0,
                    Position = gateConnection.Offset == null ? Point.Empty : new Point((int)Math.Round(gateConnection.Offset.X), (int)Math.Round(gateConnection.Offset.Z))
                };

                zone.Gates.Add(gate);
            }

            return zone;
        }

        private static void PairImportedGates(List<ConnectionDefinition> galaxyConnections, List<Cluster> importedClusters)
        {
            var gateMap = new Dictionary<string, PendingGate>(StringComparer.OrdinalIgnoreCase);
            foreach (var cluster in importedClusters)
            {
                foreach (var sector in cluster.Sectors)
                {
                    foreach (var zone in sector.Zones)
                    {
                        foreach (var gate in zone.Gates)
                        {
                            if (!string.IsNullOrWhiteSpace(gate.ConnectionName))
                            {
                                gateMap[gate.ConnectionName] = new PendingGate(cluster, sector, zone, gate, gate.ConnectionName);
                            }
                        }
                    }
                }
            }

            var pairedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var connection in galaxyConnections.Where(a => a.IsGatePair))
            {
                var sourceKey = ExtractConnectionName(connection.Path);
                var destinationKey = ExtractConnectionName(connection.MacroPath);
                if (sourceKey == null || destinationKey == null)
                {
                    continue;
                }

                if (!gateMap.TryGetValue(sourceKey, out var sourceGate) || !gateMap.TryGetValue(destinationKey, out var destinationGate))
                {
                    continue;
                }

                var pairKey = string.Compare(sourceKey, destinationKey, StringComparison.OrdinalIgnoreCase) < 0
                    ? $"{sourceKey}|{destinationKey}"
                    : $"{destinationKey}|{sourceKey}";
                if (!pairedKeys.Add(pairKey))
                {
                    continue;
                }

                sourceGate.Gate.ConnectionName = sourceGate.Name;
                sourceGate.Gate.ParentSectorName = sourceGate.Sector.Name;
                sourceGate.Gate.DestinationSectorName = destinationGate.Sector.Name;
                sourceGate.Gate.Destination = destinationGate.Name;

                destinationGate.Gate.ConnectionName = destinationGate.Name;
                destinationGate.Gate.ParentSectorName = destinationGate.Sector.Name;
                destinationGate.Gate.DestinationSectorName = sourceGate.Sector.Name;
                destinationGate.Gate.Destination = sourceGate.Name;
            }

            foreach (var zone in importedClusters.SelectMany(a => a.Sectors).SelectMany(a => a.Zones))
            {
                zone.Gates.RemoveAll(a => string.IsNullOrWhiteSpace(a.DestinationSectorName));
            }
        }

        private static void AssignImportedIds(List<Cluster> importedClusters, VanillaLookup vanillaLookup)
        {
            var nextClusterId = vanillaLookup.MaxClusterId + 1;
            foreach (var cluster in importedClusters.Where(a => !a.IsBaseGame).OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase))
            {
                cluster.Id = nextClusterId++;
            }

            foreach (var cluster in importedClusters)
            {
                var nextSectorId = cluster.IsBaseGame
                    ? vanillaLookup.GetNextSectorId(cluster.BaseGameMapping)
                    : 1;

                foreach (var sector in cluster.Sectors.Where(a => !a.IsBaseGame).OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase))
                {
                    sector.Id = nextSectorId++;
                }

                foreach (var sector in cluster.Sectors.Where(a => a.IsBaseGame))
                {
                    sector.Id = vanillaLookup.GetSectorId(cluster.BaseGameMapping, sector.BaseGameMapping);
                }

                foreach (var sector in cluster.Sectors)
                {
                    var nextZoneId = cluster.IsBaseGame && sector.IsBaseGame
                        ? vanillaLookup.GetNextZoneId(cluster.BaseGameMapping, sector.BaseGameMapping)
                        : 1;

                    foreach (var zone in sector.Zones.Where(a => !a.IsBaseGame))
                    {
                        zone.Id = nextZoneId++;
                    }

                    foreach (var zone in sector.Zones.Where(a => a.IsBaseGame))
                    {
                        zone.Id = vanillaLookup.GetZoneId(cluster.BaseGameMapping, sector.BaseGameMapping, zone.Name);
                    }

                    var nextGateId = 1;
                    foreach (var gate in sector.Zones.SelectMany(a => a.Gates))
                    {
                        gate.Id = nextGateId++;
                    }
                }

                cluster.Sectors = cluster.Sectors
                    .OrderBy(a => a.IsBaseGame ? 0 : 1)
                    .ThenBy(a => a.Id)
                    .ToList();
            }
        }

        private static void ResolveCustomClusterPositionCollisions(List<Cluster> importedClusters, VanillaLookup vanillaLookup)
        {
            var occupiedPositions = vanillaLookup.ClustersByMacroName.Values
                .Select(a => (a.Position.X, a.Position.Y))
                .ToHashSet();

            foreach (var cluster in importedClusters.Where(a => a.IsBaseGame))
            {
                occupiedPositions.Add((cluster.Position.X, cluster.Position.Y));
            }

            foreach (var cluster in importedClusters.Where(a => !a.IsBaseGame).OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase))
            {
                var desired = (cluster.Position.X, cluster.Position.Y);
                if (IsValidClusterGridPosition(desired) && occupiedPositions.Add(desired))
                {
                    continue;
                }

                var resolved = FindNearestFreePosition(desired, occupiedPositions);
                cluster.Position = new Point(resolved.X, resolved.Y);
                occupiedPositions.Add(resolved);
            }
        }

        private static (int X, int Y) FindNearestFreePosition((int X, int Y) desired, HashSet<(int X, int Y)> occupiedPositions)
        {
            for (var radius = 1; radius < 512; radius++)
            {
                var candidates = new List<(int X, int Y)>();
                for (var x = desired.X - radius; x <= desired.X + radius; x++)
                {
                    candidates.Add((x, desired.Y - radius));
                    candidates.Add((x, desired.Y + radius));
                }

                for (var y = desired.Y - radius + 1; y <= desired.Y + radius - 1; y++)
                {
                    candidates.Add((desired.X - radius, y));
                    candidates.Add((desired.X + radius, y));
                }

                foreach (var candidate in candidates
                    .Distinct()
                    .Where(IsValidClusterGridPosition)
                    .OrderBy(a => Math.Abs(a.X - desired.X) + Math.Abs(a.Y - desired.Y))
                    .ThenBy(a => a.Y)
                    .ThenBy(a => a.X))
                {
                    if (!occupiedPositions.Contains(candidate))
                    {
                        return candidate;
                    }
                }
            }

            throw new InvalidOperationException("Unable to find a free map position for an imported cluster.");
        }

        private static bool IsValidClusterGridPosition((int X, int Y) position)
        {
            return Math.Abs(position.X % 2) == Math.Abs(position.Y % 2);
        }

        private static void RebuildImportedGatePaths(List<Cluster> importedClusters)
        {
            var gateLookup = importedClusters
                .SelectMany(a => a.Sectors, (cluster, sector) => (cluster, sector))
                .SelectMany(a => a.sector.Zones, (a, zone) => (a.cluster, a.sector, zone))
                .SelectMany(a => a.zone.Gates, (a, gate) => (a.cluster, a.sector, a.zone, gate))
                .ToList();

            foreach (var entry in gateLookup)
            {
                entry.gate.ParentSectorName = entry.sector.Name;
                entry.gate.Source = BuildGateLocation(entry.cluster, entry.sector, entry.zone);
                entry.gate.SetSourcePath("PREFIX", entry.cluster, entry.sector, entry.zone);
            }

            foreach (var entry in gateLookup)
            {
                var target = gateLookup.FirstOrDefault(a =>
                    !ReferenceEquals(a.gate, entry.gate) &&
                    a.gate.ConnectionName.Equals(entry.gate.Destination, StringComparison.OrdinalIgnoreCase));
                if (target.gate == null)
                {
                    continue;
                }

                entry.gate.Destination = BuildGateLocation(target.cluster, target.sector, target.zone);
                entry.gate.SetDestinationPath("PREFIX", target.cluster, target.sector, target.zone, target.gate);
            }
        }

        private static void CollectDefinitions(string xmlPath, Dictionary<string, MacroDefinition> macros, List<ConnectionDefinition> galaxyConnections)
        {
            var document = XDocument.Load(xmlPath);
            if (document.Root == null)
            {
                return;
            }

            if (document.Root.Name.LocalName.Equals("macros", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var macroElement in document.Root.Elements("macro"))
                {
                    UpsertMacro(macros, macroElement);
                }

                foreach (var galaxyMacro in document.Root.Elements("macro").Where(IsGalaxyMacro))
                {
                    galaxyConnections.AddRange(ParseConnections(galaxyMacro));
                }

                return;
            }

            if (!document.Root.Name.LocalName.Equals("diff", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            foreach (var addElement in document.Root.Elements("add"))
            {
                var selector = (string)addElement.Attribute("sel") ?? string.Empty;
                if (IsMacrosSelector(selector))
                {
                    foreach (var macroElement in addElement.Elements("macro"))
                    {
                        UpsertMacro(macros, macroElement);
                    }
                }

                if (!selector.Contains("/connections", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var macroName = ExtractMacroName(selector);
                if (string.IsNullOrWhiteSpace(macroName))
                {
                    continue;
                }

                var definition = GetOrCreateMacro(macros, macroName);
                var parsedConnections = addElement.Elements("connection").Select(ParseConnection).ToList();
                definition.Connections.AddRange(parsedConnections);

                if (IsGalaxySelector(selector))
                {
                    galaxyConnections.AddRange(parsedConnections);
                }
            }
        }

        private static void UpsertMacro(Dictionary<string, MacroDefinition> macros, XElement macroElement)
        {
            var name = (string)macroElement.Attribute("name");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var definition = GetOrCreateMacro(macros, name);
            definition.Class = (string)macroElement.Attribute("class") ?? definition.Class;
            definition.Connections.AddRange(ParseConnections(macroElement));
        }

        private static MacroDefinition GetOrCreateMacro(Dictionary<string, MacroDefinition> macros, string name)
        {
            if (!macros.TryGetValue(name, out var definition))
            {
                macros[name] = definition = new MacroDefinition(name);
            }

            return definition;
        }

        private static List<ConnectionDefinition> ParseConnections(XElement macroElement)
        {
            return macroElement.Element("connections")?
                .Elements("connection")
                .Select(ParseConnection)
                .ToList() ?? [];
        }

        private static ConnectionDefinition ParseConnection(XElement connectionElement)
        {
            var rotationElement = connectionElement.Element("offset")?.Element("rotation");
            return new ConnectionDefinition
            {
                Name = (string)connectionElement.Attribute("name"),
                Ref = (string)connectionElement.Attribute("ref"),
                Path = NormalizeConnectionPath((string)connectionElement.Attribute("path")),
                MacroRef = (string)connectionElement.Element("macro")?.Attribute("ref"),
                MacroPath = NormalizeConnectionPath((string)connectionElement.Element("macro")?.Attribute("path")),
                Offset = ParsePosition(connectionElement.Element("offset")?.Element("position")),
                Rotation = rotationElement == null ? null : new RotationDefinition(
                    ParseInt(rotationElement.Attribute("yaw")?.Value),
                    ParseInt(rotationElement.Attribute("pitch")?.Value),
                    ParseInt(rotationElement.Attribute("roll")?.Value))
            };
        }

        private static bool MatchesSector(VanillaZoneInfo zoneInfo, Cluster cluster, Sector sector)
        {
            return cluster.IsBaseGame && sector.IsBaseGame &&
                   zoneInfo.Cluster.BaseGameMapping.Equals(cluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase) &&
                   zoneInfo.Sector.BaseGameMapping.Equals(sector.BaseGameMapping, StringComparison.OrdinalIgnoreCase);
        }

        private static Gate.GateType ParseGateType(string macroRef)
        {
            return Enum.TryParse<Gate.GateType>(macroRef ?? string.Empty, out var gateType)
                ? gateType
                : Gate.GateType.props_gates_anc_gate_macro;
        }

        private static PositionDefinition ParsePosition(XElement positionElement)
        {
            if (positionElement == null)
            {
                return null;
            }

            return new PositionDefinition(
                ParseDouble(positionElement.Attribute("x")?.Value),
                ParseDouble(positionElement.Attribute("y")?.Value),
                ParseDouble(positionElement.Attribute("z")?.Value));
        }

        private static Point ConvertClusterPosition(PositionDefinition position)
        {
            if (position == null)
            {
                return Point.Empty;
            }

            var scaledX = position.X / ClusterPositionScaleX;
            var scaledY = position.Z / ClusterPositionScaleY;
            var x = (int)Math.Round(scaledX, MidpointRounding.AwayFromZero);
            var y = (int)Math.Round(scaledY, MidpointRounding.AwayFromZero);

            if (Math.Abs(x % 2) != Math.Abs(y % 2))
            {
                var lower = y - 1;
                var upper = y + 1;

                if (Math.Abs(x % 2) != Math.Abs(lower % 2))
                {
                    lower -= 1;
                }

                if (Math.Abs(x % 2) != Math.Abs(upper % 2))
                {
                    upper += 1;
                }

                y = Math.Abs(scaledY - lower) <= Math.Abs(scaledY - upper)
                    ? lower
                    : upper;
            }

            return new Point(x, y);
        }

        private static Point ConvertZonePosition(PositionDefinition position)
        {
            if (position == null)
            {
                return Point.Empty;
            }

            return new Point((int)Math.Round(position.X), (int)Math.Round(position.Z));
        }

        private static SectorPlacement InferPlacement(PositionDefinition sectorOffset)
        {
            if (sectorOffset == null)
            {
                return SectorPlacement.TopLeft;
            }

            var x = sectorOffset.X;
            var z = sectorOffset.Z;
            const double tolerance = 1000;

            if (Math.Abs(x) <= tolerance)
            {
                return z >= 0 ? SectorPlacement.MiddleTop : SectorPlacement.MiddleBottom;
            }

            if (Math.Abs(z) <= tolerance)
            {
                return x >= 0 ? SectorPlacement.MiddleRight : SectorPlacement.MiddleLeft;
            }

            if (x >= 0 && z >= 0) return SectorPlacement.TopRight;
            if (x >= 0 && z < 0) return SectorPlacement.BottomRight;
            if (x < 0 && z >= 0) return SectorPlacement.TopLeft;
            return SectorPlacement.BottomLeft;
        }

        private static string BuildGateLocation(Cluster cluster, Sector sector, Zone zone)
        {
            var clusterPart = cluster.IsBaseGame ? cluster.BaseGameMapping.CapitalizeFirstLetter() : $"c{cluster.Id:D3}";
            var sectorPart = sector.IsBaseGame ? sector.BaseGameMapping.CapitalizeFirstLetter() : $"s{sector.Id:D3}";
            return $"{clusterPart}_{sectorPart}_z{zone.Id:D3}";
        }

        private static string NormalizeConnectionPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            var value = path.Trim();
            while (value.StartsWith("../", StringComparison.Ordinal))
            {
                value = value[3..];
            }

            return value;
        }

        private static string ExtractConnectionName(string path)
        {
            return NormalizeConnectionPath(path)?.Split('/').LastOrDefault();
        }

        private static bool IsMacrosSelector(string selector)
        {
            return selector.Equals("/macros", StringComparison.OrdinalIgnoreCase) ||
                   selector.Equals("//macros", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsGalaxySelector(string selector)
        {
            var macroName = ExtractMacroName(selector);
            return !string.IsNullOrWhiteSpace(macroName) &&
                   (macroName.EndsWith("universe_macro", StringComparison.OrdinalIgnoreCase) ||
                    macroName.EndsWith("galaxy_macro", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsGalaxyMacro(XElement macroElement)
        {
            return ((string)macroElement.Attribute("class"))?.Equals("galaxy", StringComparison.OrdinalIgnoreCase) == true;
        }

        private static string ExtractMacroName(string selector)
        {
            return SelectorMacroRegex().Match(selector).Groups[1].Value;
        }

        private static string ReadModName(string contentPath)
        {
            var document = XDocument.Load(contentPath);
            return document.Root?.Attribute("name")?.Value;
        }

        private static double ParseDouble(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? 0
                : double.Parse(value, CultureInfo.InvariantCulture);
        }

        private static int ParseInt(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? 0
                : (int)Math.Round(double.Parse(value, CultureInfo.InvariantCulture), MidpointRounding.AwayFromZero);
        }

        [GeneratedRegex("@name='([^']+)'", RegexOptions.IgnoreCase)]
        private static partial Regex SelectorMacroRegex();

        private sealed class MacroDefinition(string name)
        {
            public string Name { get; } = name;
            public string Class { get; set; }
            public List<ConnectionDefinition> Connections { get; } = [];
            public bool IsCluster => Class != null && Class.Equals("cluster", StringComparison.OrdinalIgnoreCase);
        }

        private sealed class ConnectionDefinition
        {
            public string Name { get; set; }
            public string Ref { get; set; }
            public string Path { get; set; }
            public string MacroRef { get; set; }
            public string MacroPath { get; set; }
            public PositionDefinition Offset { get; set; }
            public RotationDefinition Rotation { get; set; }
            public bool IsClusterConnection => Ref != null && Ref.Equals("clusters", StringComparison.OrdinalIgnoreCase);
            public bool IsSectorConnection => Ref != null && Ref.Equals("sectors", StringComparison.OrdinalIgnoreCase);
            public bool IsZoneConnection => Ref != null && Ref.Equals("zones", StringComparison.OrdinalIgnoreCase);
            public bool IsGateConnection => Ref != null && Ref.Equals("gates", StringComparison.OrdinalIgnoreCase);
            public bool IsGatePair => Ref != null && Ref.Equals("destination", StringComparison.OrdinalIgnoreCase);
        }

        private sealed record PositionDefinition(double X, double Y, double Z);
        private sealed record RotationDefinition(int Yaw, int Pitch, int Roll);
        private sealed record PendingGate(Cluster Cluster, Sector Sector, Zone Zone, Gate Gate, string Name);

        private sealed class VanillaLookup
        {
            public int MaxClusterId { get; private init; }
            public Dictionary<string, Cluster> ClustersByMacroName { get; private init; }
            public Dictionary<string, VanillaSectorInfo> SectorsByMacroName { get; private init; }
            public Dictionary<string, VanillaZoneInfo> ZonesByMacroName { get; private init; }
            public Dictionary<string, int> NextSectorIds { get; private init; }
            public Dictionary<(string cluster, string sector), int> NextZoneIds { get; private init; }

            public int GetNextSectorId(string clusterBaseGameMapping)
            {
                return NextSectorIds.TryGetValue(clusterBaseGameMapping, out var value) ? value : 1;
            }

            public int GetSectorId(string clusterBaseGameMapping, string sectorBaseGameMapping)
            {
                return SectorsByMacroName.Values
                    .First(a => a.Cluster.BaseGameMapping.Equals(clusterBaseGameMapping, StringComparison.OrdinalIgnoreCase) &&
                                a.Sector.BaseGameMapping.Equals(sectorBaseGameMapping, StringComparison.OrdinalIgnoreCase))
                    .Sector.Id;
            }

            public int GetNextZoneId(string clusterBaseGameMapping, string sectorBaseGameMapping)
            {
                return NextZoneIds.TryGetValue((clusterBaseGameMapping, sectorBaseGameMapping), out var value) ? value : 1;
            }

            public int GetZoneId(string clusterBaseGameMapping, string sectorBaseGameMapping, string zoneName)
            {
                return ZonesByMacroName.Values
                    .First(a => a.Cluster.BaseGameMapping.Equals(clusterBaseGameMapping, StringComparison.OrdinalIgnoreCase) &&
                                a.Sector.BaseGameMapping.Equals(sectorBaseGameMapping, StringComparison.OrdinalIgnoreCase) &&
                                a.Zone.Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase))
                    .Zone.Id;
            }

            public static VanillaLookup Create(ClusterCollection vanillaClusterData)
            {
                var clusters = vanillaClusterData.Clusters.Where(a => a.IsBaseGame).ToList();
                var clustersByMacro = clusters.ToDictionary(
                    a => $"{a.BaseGameMapping.CapitalizeFirstLetter()}_macro",
                    a => a,
                    StringComparer.OrdinalIgnoreCase);

                var sectorsByMacro = new Dictionary<string, VanillaSectorInfo>(StringComparer.OrdinalIgnoreCase);
                var zonesByMacro = new Dictionary<string, VanillaZoneInfo>(StringComparer.OrdinalIgnoreCase);
                foreach (var cluster in clusters)
                {
                    foreach (var sector in cluster.Sectors.Where(a => a.IsBaseGame))
                    {
                        sectorsByMacro[$"{cluster.BaseGameMapping.CapitalizeFirstLetter()}_{sector.BaseGameMapping.CapitalizeFirstLetter()}_macro"] = new VanillaSectorInfo(cluster, sector);
                        foreach (var zone in sector.Zones.Where(a => a.IsBaseGame))
                        {
                            zonesByMacro[$"{zone.Name}_macro"] = new VanillaZoneInfo(cluster, sector, zone);
                        }
                    }
                }

                return new VanillaLookup
                {
                    MaxClusterId = clusters.Max(a => a.Id),
                    ClustersByMacroName = clustersByMacro,
                    SectorsByMacroName = sectorsByMacro,
                    ZonesByMacroName = zonesByMacro,
                    NextSectorIds = clusters.ToDictionary(a => a.BaseGameMapping, a => a.Sectors.Max(b => b.Id) + 1, StringComparer.OrdinalIgnoreCase),
                    NextZoneIds = clusters
                        .SelectMany(a => a.Sectors.Where(b => b.IsBaseGame), (cluster, sector) => (cluster, sector))
                        .ToDictionary(
                            a => (a.cluster.BaseGameMapping, a.sector.BaseGameMapping),
                            a => a.sector.Zones.DefaultIfEmpty(new Zone { Id = 0 }).Max(b => b.Id) + 1)
                };
            }
        }

        private sealed record VanillaSectorInfo(Cluster Cluster, Sector Sector);
        private sealed record VanillaZoneInfo(Cluster Cluster, Sector Sector, Zone Zone);
    }

    internal sealed record ModImportResult(string ModName, List<Cluster> Clusters);
}
