using System.Globalization;
using System.Text.Json;
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
        private static readonly TextInfo EnglishTextInfo = CultureInfo.InvariantCulture.TextInfo;

        public static ModImportResult Import(string modDirectory, ClusterCollection vanillaClusterData)
        {
            return ImportInternal([Path.GetFullPath(modDirectory)], vanillaClusterData, mergeDisplayName: null);
        }

        public static ModImportResult ImportMerged(string rootDirectory, ClusterCollection vanillaClusterData)
        {
            List<string> modDirectories = DiscoverMergeImportDirectories(rootDirectory);
            if (modDirectories.Count == 0)
            {
                throw new InvalidOperationException("No importable X4 extension folders were found in the selected directory.");
            }

            string mergeDisplayName = modDirectories.Count == 1
                ? null
                : $"Merged import ({modDirectories.Count} mods)";

            return ImportInternal(modDirectories, vanillaClusterData, mergeDisplayName);
        }

        public static List<string> DiscoverMergeImportDirectories(string rootDirectory)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory) || !Directory.Exists(rootDirectory))
            {
                throw new DirectoryNotFoundException("The selected mod directory does not exist.");
            }

            string fullRoot = Path.GetFullPath(rootDirectory);
            if (IsImportableModDirectory(fullRoot))
            {
                return [fullRoot];
            }

            List<string> candidates = Directory
                .GetFiles(fullRoot, "content.xml", SearchOption.AllDirectories)
                .Select(Path.GetDirectoryName)
                .Where(a => !string.IsNullOrWhiteSpace(a) && IsImportableModDirectory(a))
                .Select(Path.GetFullPath)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(a => a.Length)
                .ThenBy(a => a, StringComparer.OrdinalIgnoreCase)
                .ToList();

            List<string> selected = [];
            foreach (string candidate in candidates)
            {
                if (selected.Any(parent => IsChildDirectory(candidate, parent)))
                {
                    continue;
                }

                selected.Add(candidate);
            }

            return selected;
        }

        public static bool IsImportableModDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
                return false;

            return File.Exists(Path.Combine(directory, "content.xml")) &&
                   Directory.Exists(Path.Combine(directory, "maps"));
        }

        private static ModImportResult ImportInternal(IReadOnlyList<string> modDirectories, ClusterCollection vanillaClusterData, string mergeDisplayName)
        {
            if (modDirectories == null || modDirectories.Count == 0)
            {
                throw new InvalidOperationException("No importable X4 extension folders were provided.");
            }

            foreach (string modDirectory in modDirectories)
            {
                if (!Directory.Exists(modDirectory))
                {
                    throw new DirectoryNotFoundException($"The selected mod directory does not exist: {modDirectory}");
                }

                string contentPath = Path.Combine(modDirectory, "content.xml");
                if (!File.Exists(contentPath))
                {
                    throw new FileNotFoundException("The selected folder is not an X4 extension. Missing content.xml.", contentPath);
                }

                string mapsRoot = Path.Combine(modDirectory, "maps");
                if (!Directory.Exists(mapsRoot))
                {
                    throw new DirectoryNotFoundException($"The selected extension does not contain a maps folder: {modDirectory}");
                }
            }

            string modName = mergeDisplayName ??
                (ReadModName(Path.Combine(modDirectories[0], "content.xml")) ?? Path.GetFileName(modDirectories[0]));

            var macros = new Dictionary<string, MacroDefinition>(StringComparer.OrdinalIgnoreCase);
            var galaxyConnections = new List<ConnectionDefinition>();
            List<string> importWarnings = [];

            foreach (string modDirectory in modDirectories)
            {
                string mapsRoot = Path.Combine(modDirectory, "maps");
                foreach (string xmlPath in Directory.GetFiles(mapsRoot, "*.xml", SearchOption.AllDirectories))
                {
                    CollectDefinitions(xmlPath, macros, galaxyConnections);
                }

                ApplyMapDefaultMetadata(modDirectory, macros, importWarnings);
            }

            if (macros.Count == 0 && galaxyConnections.Count == 0)
            {
                throw new InvalidOperationException("No supported map XML content was found in the selected extension.");
            }

            var vanillaLookup = VanillaLookup.Create(vanillaClusterData);
            var referencedVanillaEndpoints = CollectReferencedVanillaEndpoints(galaxyConnections, vanillaLookup);
            var customClusterPositions = galaxyConnections
                .Where(a => a.IsClusterConnection && !string.IsNullOrWhiteSpace(a.MacroRef))
                .GroupBy(a => a.MacroRef, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(a => a.Key, a => a.First().Offset, StringComparer.OrdinalIgnoreCase);

            var importedClusters = BuildImportedClusters(macros, vanillaLookup, customClusterPositions, referencedVanillaEndpoints);
            if (importedClusters.Count == 0)
            {
                throw new InvalidOperationException("The extension did not contain any importable clusters, sectors, or zones.");
            }

            foreach (string modDirectory in modDirectories)
            {
                ClusterCollection sidecarMetadata = LoadSidecarClusterMetadata(modDirectory);
                if (sidecarMetadata?.Clusters?.Count > 0)
                {
                    ApplySidecarMetadata(importedClusters, sidecarMetadata.Clusters);
                }

                ApplyGodOwnership(modDirectory, importedClusters);
            }

            HydrateReferencedVanillaGates(importedClusters, galaxyConnections, vanillaLookup);
            ImportNameNormalizer.EnsureImportedSectorNamesPreservingIdentity(importedClusters);
            ResolveCustomClusterPositionCollisions(importedClusters, vanillaLookup);
            PairImportedGates(galaxyConnections, importedClusters);
            AssignImportedIds(importedClusters, vanillaLookup);
            RebuildImportedGatePaths(importedClusters);

            return new ModImportResult(modName, importedClusters, importWarnings.Distinct(StringComparer.OrdinalIgnoreCase).ToList());
        }

        private static bool IsChildDirectory(string candidate, string parent)
        {
            string normalizedParent = Path.TrimEndingDirectorySeparator(Path.GetFullPath(parent));
            string normalizedCandidate = Path.TrimEndingDirectorySeparator(Path.GetFullPath(candidate));

            return normalizedCandidate.Length > normalizedParent.Length &&
                   normalizedCandidate.StartsWith(normalizedParent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        private static List<Cluster> BuildImportedClusters(
            Dictionary<string, MacroDefinition> macros,
            VanillaLookup vanillaLookup,
            Dictionary<string, PositionDefinition> customClusterPositions,
            ReferencedVanillaEndpoints referencedVanillaEndpoints)
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
                        ImportedMacroName = clusterMacroName,
                        Name = vanillaCluster.Name,
                        Description = ImportClusterMetadataResolver.ResolveDescription(clusterMacro?.Description, vanillaCluster.Description),
                        BackgroundVisualMapping = ImportClusterMetadataResolver.ResolveBackgroundVisualMapping(clusterMacro?.ContentRef, vanillaCluster.BackgroundVisualMapping),
                        Soundtrack = ImportClusterMetadataResolver.ResolveSoundtrack(clusterMacro?.MusicRef, vanillaCluster.Soundtrack),
                        Dlc = vanillaCluster.Dlc,
                        Position = vanillaCluster.Position,
                        Sectors = []
                    }
                    : new Cluster
                    {
                        ImportedMacroName = clusterMacroName,
                        Name = clusterMacro?.DisplayName ?? NormalizeMacroFallbackName(clusterMacroName),
                        Description = clusterMacro?.Description,
                        BackgroundVisualMapping = clusterMacro?.ContentRef,
                        Soundtrack = clusterMacro?.MusicRef,
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

                if (isBaseCluster)
                {
                    foreach (string sectorMacroName in referencedVanillaEndpoints.SectorMacroNames)
                    {
                        if (vanillaLookup.SectorsByMacroName.TryGetValue(sectorMacroName, out var sectorInfo) &&
                            sectorInfo.Cluster.BaseGameMapping.Equals(cluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase))
                        {
                            touchedBaseSectors.Add(sectorMacroName);
                        }
                    }
                }

                if (clusterMacro != null)
                {
                    foreach (var sectorConnection in clusterMacro.Connections.Where(a => a.IsSectorConnection && !string.IsNullOrWhiteSpace(a.MacroRef)))
                    {
                        var sector = CreateImportedSector(sectorConnection.MacroRef, sectorConnection.Offset, cluster, macros, vanillaLookup, referencedVanillaEndpoints);
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

                        var sector = CreateImportedSector(sectorMacroName, null, cluster, macros, vanillaLookup, referencedVanillaEndpoints);
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
            VanillaLookup vanillaLookup,
            ReferencedVanillaEndpoints referencedVanillaEndpoints)
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
                    ImportedMacroName = sectorMacroName,
                    Name = vanillaSectorInfo.Sector.Name,
                    Description = vanillaSectorInfo.Sector.Description,
                    DisableFactionLogic = vanillaSectorInfo.Sector.DisableFactionLogic,
                    Owner = vanillaSectorInfo.Sector.Owner,
                    Sunlight = vanillaSectorInfo.Sector.Sunlight,
                    Economy = vanillaSectorInfo.Sector.Economy,
                    Security = vanillaSectorInfo.Sector.Security,
                    AllowRandomAnomalies = vanillaSectorInfo.Sector.AllowRandomAnomalies,
                    Placement = vanillaSectorInfo.Sector.Placement,
                    SectorRealOffset = vanillaSectorInfo.Sector.SectorRealOffset,
                    Zones = [],
                    Regions = [],
                    ResourceAreas = vanillaSectorInfo.Sector.ResourceAreas.Select(a => (Resource)a.Clone()).ToList()
                }
                : new Sector
                {
                    ImportedMacroName = sectorMacroName,
                    Name = sectorMacro?.DisplayName ?? NormalizeMacroFallbackName(sectorMacroName),
                    Description = sectorMacro?.Description,
                    Owner = sectorMacro?.Owner,
                    DisableFactionLogic = sectorMacro?.DisableFactionLogic ?? false,
                    Sunlight = sectorMacro?.Sunlight ?? 1.0f,
                    Economy = sectorMacro?.Economy ?? 1.0f,
                    Security = sectorMacro?.Security ?? 1.0f,
                    AllowRandomAnomalies = sectorMacro?.AllowRandomAnomalies ?? true,
                    Placement = InferPlacement(sectorOffset),
                    CustomOffset = sectorOffset == null ? null : new Point((int)Math.Round(sectorOffset.X), (int)Math.Round(sectorOffset.Z)),
                    Zones = [],
                    Regions = [],
                    ResourceAreas = sectorMacro?.ResourceAreas.Select(a => (Resource)a.Clone()).ToList() ?? []
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

            if (isBaseSector)
            {
                foreach (string zoneMacroName in referencedVanillaEndpoints.ZoneMacroNames)
                {
                    if (!vanillaLookup.ZonesByMacroName.TryGetValue(zoneMacroName, out var zoneInfo) ||
                        !MatchesSector(zoneInfo, cluster, sector) ||
                        zoneCandidates.Any(a => a.macroName.Equals(zoneMacroName, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    zoneCandidates.Add((zoneMacroName, null));
                }
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
                    ImportedMacroName = zoneMacroName,
                    Position = zoneOffset == null && isBaseZone ? vanillaZoneInfo.Zone.Position : ConvertZonePosition(zoneOffset),
                    Gates = isBaseZone ? vanillaZoneInfo.Zone.Gates.Select(a => (Gate)a.Clone()).ToList() : []
                };

            if (zoneMacro == null)
            {
                return zone;
            }

            foreach (var gateConnection in zoneMacro.Connections.Where(a => a.IsGateConnection))
            {
                var gate = zone.Gates.FirstOrDefault(a => a.ConnectionName != null && a.ConnectionName.Equals(gateConnection.Name, StringComparison.OrdinalIgnoreCase));
                if (gate == null)
                {
                    gate = new Gate
                    {
                        ConnectionName = gateConnection.Name,
                    };
                    zone.Gates.Add(gate);
                }

                gate.Type = ParseGateType(gateConnection.MacroRef);
                gate.Pitch = gateConnection.Rotation?.Pitch ?? gate.Pitch;
                gate.Roll = gateConnection.Rotation?.Roll ?? gate.Roll;
                gate.Yaw = gateConnection.Rotation?.Yaw ?? gate.Yaw;
                gate.Position = gateConnection.Offset == null
                    ? gate.Position
                    : new Point((int)Math.Round(gateConnection.Offset.X), (int)Math.Round(gateConnection.Offset.Z));
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
            ImportIdentityAssigner.AssignImportedIdsPreservingOrder(
                importedClusters,
                vanillaLookup.MaxClusterId,
                vanillaLookup.GetNextSectorId,
                vanillaLookup.GetSectorId,
                vanillaLookup.GetNextZoneId,
                vanillaLookup.GetZoneId);
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

        private static void HydrateReferencedVanillaGates(List<Cluster> importedClusters, List<ConnectionDefinition> galaxyConnections, VanillaLookup vanillaLookup)
        {
            if (!File.Exists(Constants.DataPaths.VanillaConnectionMappingFilePath))
                return;

            VanilaConnectionMapping vanillaMapping = JsonSerializer.Deserialize<VanilaConnectionMapping>(
                File.ReadAllText(Constants.DataPaths.VanillaConnectionMappingFilePath),
                ConfigSerializer.JsonSerializerOptions);
            if (vanillaMapping?.Connections == null || vanillaMapping.ZoneGateInfos == null)
                return;

            HashSet<string> referencedPaths = galaxyConnections
                .Where(a => a.IsGatePair)
                .SelectMany(a => new[] { NormalizeConnectionPath(a.Path), NormalizeConnectionPath(a.MacroPath) })
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            Dictionary<string, Cluster> importedBaseClusters = importedClusters
                .Where(a => a.IsBaseGame)
                .ToDictionary(a => a.BaseGameMapping, StringComparer.OrdinalIgnoreCase);

            foreach (Connection connection in vanillaMapping.Connections.Where(a => !a.IsHighway))
            {
                TryHydrateVanillaGateEndpoint(connection.Source, connection.Destination, vanillaMapping.ZoneGateInfos, referencedPaths, importedBaseClusters, vanillaLookup);
                TryHydrateVanillaGateEndpoint(connection.Destination, connection.Source, vanillaMapping.ZoneGateInfos, referencedPaths, importedBaseClusters, vanillaLookup);
            }
        }

        private static void TryHydrateVanillaGateEndpoint(
            ConnectionInfo endpoint,
            ConnectionInfo opposite,
            List<ZoneGateInfo> zoneGateInfos,
            HashSet<string> referencedPaths,
            Dictionary<string, Cluster> importedBaseClusters,
            VanillaLookup vanillaLookup)
        {
            string endpointPath = NormalizeConnectionPath(endpoint?.Path);
            if (string.IsNullOrWhiteSpace(endpointPath) || !referencedPaths.Contains(endpointPath))
                return;

            string clusterBaseGameMapping = endpoint.Cluster?.Replace("_connection", string.Empty, StringComparison.OrdinalIgnoreCase);
            string sectorConnectionName = endpoint.Sector;
            string sectorMacroName = sectorConnectionName?.Replace("_connection", "_macro", StringComparison.OrdinalIgnoreCase);
            string zoneName = endpoint.Zone?.Replace("_connection", string.Empty, StringComparison.OrdinalIgnoreCase);
            string gateConnectionName = endpointPath.Split('/').LastOrDefault();

            if (string.IsNullOrWhiteSpace(clusterBaseGameMapping) ||
                string.IsNullOrWhiteSpace(sectorMacroName) ||
                !importedBaseClusters.TryGetValue(clusterBaseGameMapping, out Cluster cluster) ||
                !vanillaLookup.SectorsByMacroName.TryGetValue(sectorMacroName, out VanillaSectorInfo vanillaSectorInfo))
            {
                return;
            }

            Sector sector = cluster.Sectors.FirstOrDefault(a => a.IsBaseGame && a.BaseGameMapping.Equals(vanillaSectorInfo.Sector.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
            if (sector == null)
                return;

            Zone zone = sector.Zones.FirstOrDefault(a => a.Name != null && a.Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase));
            ZoneGateInfo zoneGateInfo = zoneGateInfos.FirstOrDefault(a =>
                a.GateName.Equals(gateConnectionName, StringComparison.OrdinalIgnoreCase) ||
                a.ZoneName.Replace("_connection", string.Empty, StringComparison.OrdinalIgnoreCase).Equals(zoneName, StringComparison.OrdinalIgnoreCase));

            if (zone == null)
            {
                zone = new Zone
                {
                    Name = zoneName,
                    Gates = [],
                    Position = zoneGateInfo == null ? Point.Empty : new Point(zoneGateInfo.ZonePosition.X, zoneGateInfo.ZonePosition.Y)
                };
                sector.Zones.Add(zone);
            }
            else if (zoneGateInfo != null)
            {
                zone.Position = new Point(zoneGateInfo.ZonePosition.X, zoneGateInfo.ZonePosition.Y);
            }

            if (zone.Gates.Any(a => !string.IsNullOrWhiteSpace(a.SourcePath) && a.SourcePath.Equals(endpointPath, StringComparison.OrdinalIgnoreCase)))
                return;

            string oppositeClusterBaseGameMapping = opposite.Cluster?.Replace("_connection", string.Empty, StringComparison.OrdinalIgnoreCase);
            string oppositeSectorMacroName = opposite.Sector?.Replace("_connection", "_macro", StringComparison.OrdinalIgnoreCase);
            string destinationSectorName = null;
            if (!string.IsNullOrWhiteSpace(oppositeClusterBaseGameMapping) &&
                !string.IsNullOrWhiteSpace(oppositeSectorMacroName) &&
                vanillaLookup.SectorsByMacroName.TryGetValue(oppositeSectorMacroName, out VanillaSectorInfo oppositeSectorInfo))
            {
                destinationSectorName = oppositeSectorInfo.Sector.Name;
            }

            zone.Gates.Add(new Gate
            {
                ConnectionName = gateConnectionName,
                ParentSectorName = sector.Name,
                DestinationSectorName = destinationSectorName,
                SourcePath = endpointPath,
                DestinationPath = NormalizeConnectionPath(opposite.Path),
                Type = ParseGateType(zoneGateInfo?.GateType),
                Position = zoneGateInfo?.GatePosition == null ? Point.Empty : new Point(zoneGateInfo.GatePosition.Value.X, zoneGateInfo.GatePosition.Value.Y),
                Roll = zoneGateInfo?.Rotation?.X ?? 0,
                Pitch = zoneGateInfo?.Rotation?.Y ?? 0,
                Yaw = zoneGateInfo?.Rotation?.Z ?? 0,
                IsHighwayGate = false
            });
        }

        private static ClusterCollection LoadSidecarClusterMetadata(string modDirectory)
        {
            foreach (string file in Directory.GetFiles(modDirectory, "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(a => a, StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    using JsonDocument document = JsonDocument.Parse(File.ReadAllText(file));
                    if (!document.RootElement.TryGetProperty("Clusters", out JsonElement clustersElement) ||
                        clustersElement.ValueKind != JsonValueKind.Array)
                    {
                        continue;
                    }

                    ClusterCollection clusters = JsonSerializer.Deserialize<ClusterCollection>(File.ReadAllText(file), ConfigSerializer.JsonSerializerOptions);
                    if (clusters?.Clusters?.Count > 0)
                        return clusters;
                }
                catch
                {
                    // Ignore sidecar files that are not cluster exports.
                }
            }

            return null;
        }

        private static void ApplySidecarMetadata(List<Cluster> importedClusters, List<Cluster> metadataClusters)
        {
            foreach (Cluster importedCluster in importedClusters)
            {
                Cluster metadataCluster = FindMetadataCluster(importedCluster, metadataClusters);
                if (metadataCluster == null)
                    continue;

                importedCluster.Name ??= metadataCluster.Name;
                importedCluster.Description ??= metadataCluster.Description;
                importedCluster.BackgroundVisualMapping ??= metadataCluster.BackgroundVisualMapping;
                importedCluster.Soundtrack ??= metadataCluster.Soundtrack;
                importedCluster.Dlc ??= metadataCluster.Dlc;

                foreach (Sector importedSector in importedCluster.Sectors)
                {
                    Sector metadataSector = FindMetadataSector(importedCluster, importedSector, metadataCluster);
                    if (metadataSector == null)
                        continue;

                    importedSector.Name ??= metadataSector.Name;
                    importedSector.Description ??= metadataSector.Description;
                    importedSector.Owner ??= metadataSector.Owner;
                    importedSector.Tags ??= metadataSector.Tags;

                    if (metadataSector.ResourceAreas?.Count > 0)
                    {
                        importedSector.ResourceAreas ??= [];
                        if (importedSector.ResourceAreas.Count == 0)
                        {
                            importedSector.ResourceAreas = metadataSector.ResourceAreas.Select(a => (Resource)a.Clone()).ToList();
                        }
                    }

                    if (!importedSector.CustomOffset.HasValue && metadataSector.CustomOffset.HasValue)
                        importedSector.CustomOffset = metadataSector.CustomOffset;
                }
            }
        }

        private static void ApplyGodOwnership(string modDirectory, List<Cluster> importedClusters)
        {
            string godPath = Path.Combine(modDirectory, "libraries", "god.xml");
            if (!File.Exists(godPath))
                return;

            XDocument document = XDocument.Load(godPath);
            Dictionary<Sector, Dictionary<string, int>> sectorVotes = new();

            foreach (XElement element in document.Descendants())
            {
                string owner = (string)element.Attribute("owner");
                if (string.IsNullOrWhiteSpace(owner))
                    continue;

                XElement location = element.Element("location");
                if (location == null)
                    continue;

                string locationClass = (string)location.Attribute("class");
                string macro = (string)location.Attribute("macro");
                if (string.IsNullOrWhiteSpace(locationClass) || string.IsNullOrWhiteSpace(macro))
                    continue;

                IEnumerable<Sector> sectors = ResolveSectorsForGodLocation(importedClusters, locationClass, macro);
                foreach (Sector sector in sectors)
                {
                    if (!sectorVotes.TryGetValue(sector, out Dictionary<string, int> counter))
                    {
                        counter = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                        sectorVotes[sector] = counter;
                    }

                    counter[owner] = counter.TryGetValue(owner, out int current) ? current + 1 : 1;
                }
            }

            foreach ((Sector sector, Dictionary<string, int> counter) in sectorVotes)
            {
                if (!string.IsNullOrWhiteSpace(sector.Owner))
                    continue;

                sector.Owner = NormalizeSectorOwner(counter
                    .OrderByDescending(a => a.Value)
                    .ThenBy(a => a.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(a => a.Key)
                    .FirstOrDefault());
            }
        }

        private static IEnumerable<Sector> ResolveSectorsForGodLocation(List<Cluster> importedClusters, string locationClass, string macro)
        {
            switch (locationClass.ToLowerInvariant())
            {
                case "sector":
                    return importedClusters
                        .SelectMany(a => a.Sectors)
                        .Where(a => string.Equals(a.ImportedMacroName, macro, StringComparison.OrdinalIgnoreCase));

                case "zone":
                    return importedClusters
                        .SelectMany(a => a.Sectors)
                        .Where(sector => sector.Zones.Any(zone => string.Equals(zone.ImportedMacroName, macro, StringComparison.OrdinalIgnoreCase)));

                case "cluster":
                    return importedClusters
                        .Where(a => string.Equals(a.ImportedMacroName, macro, StringComparison.OrdinalIgnoreCase))
                        .SelectMany(a => a.Sectors);

                default:
                    return [];
            }
        }

        private static string NormalizeSectorOwner(string owner)
        {
            if (string.IsNullOrWhiteSpace(owner))
                return owner;

            string normalized = owner.Trim();
            return normalized.Length == 0
                ? normalized
                : char.ToUpperInvariant(normalized[0]) + normalized[1..];
        }

        private static Cluster FindMetadataCluster(Cluster importedCluster, List<Cluster> metadataClusters)
        {
            if (importedCluster.IsBaseGame)
            {
                return metadataClusters.FirstOrDefault(a =>
                    !string.IsNullOrWhiteSpace(a.BaseGameMapping) &&
                    a.BaseGameMapping.Equals(importedCluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
            }

            return metadataClusters.FirstOrDefault(a =>
                       string.IsNullOrWhiteSpace(a.BaseGameMapping) &&
                       !string.IsNullOrWhiteSpace(a.Name) &&
                       a.Name.Equals(importedCluster.Name, StringComparison.OrdinalIgnoreCase))
                   ?? metadataClusters.FirstOrDefault(a =>
                       string.IsNullOrWhiteSpace(a.BaseGameMapping) &&
                       a.Position == importedCluster.Position &&
                       a.Sectors.Count == importedCluster.Sectors.Count);
        }

        private static Sector FindMetadataSector(Cluster importedCluster, Sector importedSector, Cluster metadataCluster)
        {
            if (importedSector.IsBaseGame)
            {
                return metadataCluster.Sectors.FirstOrDefault(a =>
                    !string.IsNullOrWhiteSpace(a.BaseGameMapping) &&
                    a.BaseGameMapping.Equals(importedSector.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(importedSector.ImportedMacroName))
            {
                Sector macroMatch = metadataCluster.Sectors.FirstOrDefault(a =>
                    string.IsNullOrWhiteSpace(a.BaseGameMapping) &&
                    TryExtractCustomSectorSignature(importedSector.ImportedMacroName, out string importedSignature) &&
                    TryExtractCustomSectorSignature(a.ImportedMacroName, out string metadataSignature) &&
                    importedSignature.Equals(metadataSignature, StringComparison.OrdinalIgnoreCase));
                if (macroMatch != null)
                {
                    return macroMatch;
                }
            }

            string importedLayoutSignature = BuildSectorLayoutSignature(importedSector);
            if (!string.IsNullOrWhiteSpace(importedLayoutSignature))
            {
                Sector[] layoutMatches = metadataCluster.Sectors
                    .Where(a => string.IsNullOrWhiteSpace(a.BaseGameMapping) &&
                                BuildSectorLayoutSignature(a).Equals(importedLayoutSignature, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                if (layoutMatches.Length == 1)
                {
                    return layoutMatches[0];
                }
            }

            return metadataCluster.Sectors.FirstOrDefault(a =>
                       string.IsNullOrWhiteSpace(a.BaseGameMapping) &&
                       !string.IsNullOrWhiteSpace(a.Name) &&
                       a.Name.Equals(importedSector.Name, StringComparison.OrdinalIgnoreCase))
                   ?? metadataCluster.Sectors
                       .Where(a => string.IsNullOrWhiteSpace(a.BaseGameMapping))
                       .ElementAtOrDefault(importedCluster.Sectors
                           .Where(a => !a.IsBaseGame)
                           .ToList()
                           .IndexOf(importedSector));
        }

        private static string BuildSectorLayoutSignature(Sector sector)
        {
            if (sector?.Zones == null || sector.Zones.Count == 0)
                return string.Empty;

            return string.Join(",",
                sector.Zones
                    .OrderBy(a => a.Position.X)
                    .ThenBy(a => a.Position.Y)
                    .Select(a => $"{a.Position.X}:{a.Position.Y}"));
        }

        private static bool TryExtractCustomSectorSignature(string macroName, out string signature)
        {
            signature = null;
            if (string.IsNullOrWhiteSpace(macroName))
                return false;

            Match match = CustomSectorSignatureRegex().Match(macroName);
            if (!match.Success)
                return false;

            signature = $"c{int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture):D3}_s{int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture):D3}";
            return true;
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

            CollectScriptNameHints(document, macros);
            CollectCommentSelectorNameHints(document, macros);

            if (document.Root.Name.LocalName.Equals("macros", StringComparison.OrdinalIgnoreCase))
            {
                CollectMacroDefinitions(document.Root, macros);

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
                    CollectMacroDefinitions(addElement, macros);
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

        private static void CollectMacroDefinitions(XContainer container, Dictionary<string, MacroDefinition> macros)
        {
            string pendingComment = null;

            foreach (var node in container.Nodes())
            {
                if (node is XComment comment)
                {
                    pendingComment = comment.Value;
                    continue;
                }

                if (node is XElement element && element.Name.LocalName.Equals("macro", StringComparison.OrdinalIgnoreCase))
                {
                    UpsertMacro(macros, element, pendingComment);
                    pendingComment = null;
                    continue;
                }

                pendingComment = null;
            }
        }

        private static void CollectScriptNameHints(XDocument document, Dictionary<string, MacroDefinition> macros)
        {
            foreach (var element in document.Descendants().Where(a =>
                         a.Name.LocalName.Equals("find_sector", StringComparison.OrdinalIgnoreCase) ||
                         a.Name.LocalName.Equals("find_cluster", StringComparison.OrdinalIgnoreCase)))
            {
                string macroRef = (string)element.Attribute("macro");
                if (string.IsNullOrWhiteSpace(macroRef))
                    continue;

                macroRef = macroRef.Replace("macro.", string.Empty, StringComparison.OrdinalIgnoreCase);
                var nameHint = NormalizeDisplayName((element.PreviousNode as XComment)?.Value);
                if (string.IsNullOrWhiteSpace(nameHint))
                {
                    nameHint = NormalizeScriptVariableName((string)element.Attribute("name"));
                }

                if (string.IsNullOrWhiteSpace(nameHint))
                    continue;

                var definition = GetOrCreateMacro(macros, macroRef);
                definition.DisplayName ??= nameHint;
            }
        }

        private static void CollectCommentSelectorNameHints(XDocument document, Dictionary<string, MacroDefinition> macros)
        {
            string pendingDisplayName = null;

            foreach (XNode node in document.Root?.Nodes() ?? [])
            {
                if (node is XComment comment)
                {
                    pendingDisplayName = ExtractDisplayNameFromComment(comment.Value) ?? pendingDisplayName;
                    continue;
                }

                if (node is XElement element)
                {
                    string selector = (string)element.Attribute("sel") ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(pendingDisplayName) && selector.Contains("macro[@name='", StringComparison.OrdinalIgnoreCase))
                    {
                        string macroName = ExtractMacroName(selector);
                        if (!string.IsNullOrWhiteSpace(macroName))
                        {
                            GetOrCreateMacro(macros, macroName).DisplayName ??= pendingDisplayName;
                        }
                    }

                    pendingDisplayName = null;
                }
            }
        }

        private static void ApplyMapDefaultMetadata(string modDirectory, Dictionary<string, MacroDefinition> macros, List<string> importWarnings)
        {
            string mapDefaultsPath = Path.Combine(modDirectory, "libraries", "mapdefaults.xml");
            if (!File.Exists(mapDefaultsPath))
                return;

            var translations = LoadTranslations(modDirectory);
            var document = XDocument.Load(mapDefaultsPath);
            if (document.Root == null)
                return;

            foreach (XElement dataset in document.Root.Elements("dataset"))
            {
                string macroName = (string)dataset.Attribute("macro");
                if (string.IsNullOrWhiteSpace(macroName))
                    continue;

                var definition = GetOrCreateMacro(macros, macroName);
                XElement identification = dataset.Element("properties")?.Element("identification");
                if (identification != null)
                {
                    string nameRef = (string)identification.Attribute("name");
                    string descriptionRef = (string)identification.Attribute("description");

                    string resolvedName = ResolveTranslationReference(nameRef, translations);
                    if (string.IsNullOrWhiteSpace(resolvedName))
                    {
                        resolvedName = nameRef;
                        if (!string.IsNullOrWhiteSpace(nameRef))
                        {
                            importWarnings.Add($"Unresolved sector/cluster name reference for macro '{macroName}': {nameRef}");
                        }
                    }

                    definition.DisplayName ??= NormalizeDisplayName(resolvedName);

                    string resolvedDescription = ResolveTranslationReference(descriptionRef, translations)
                        ?? ResolveTranslationPageTitle(descriptionRef, translations);
                    if (IsPlaceholderDescription(descriptionRef, resolvedDescription))
                    {
                        resolvedDescription = ExtractPrecedingDatasetComment(dataset);
                    }

                    definition.Description ??= resolvedDescription;
                    definition.ImageRef ??= (string)identification.Attribute("image");
                }

                XElement properties = dataset.Element("properties");
                XElement area = properties?.Element("area");
                XElement music = properties?.Element("music")
                    ?? properties?.Element("sounds")?.Element("music")
                    ?? properties?.Element("system")?.Element("music");
                if (music != null)
                {
                    definition.MusicRef ??= (string)music.Attribute("ref");
                }

                if (area == null)
                {
                    definition.ResourceAreas = ImportSectorMetadataResolver.ParseResourceAreas(properties);
                    continue;
                }

                if (TryParseFloat((string)area.Attribute("sunlight"), out float sunlight))
                    definition.Sunlight ??= sunlight;
                if (TryParseFloat((string)area.Attribute("economy"), out float economy))
                    definition.Economy ??= economy;
                if (TryParseFloat((string)area.Attribute("security"), out float security))
                    definition.Security ??= security;

                string factionLogic = (string)area.Attribute("factionlogic");
                if (bool.TryParse(factionLogic, out bool factionLogicEnabled))
                    definition.DisableFactionLogic ??= !factionLogicEnabled;

                string tags = (string)area.Attribute("tags") ?? string.Empty;
                if (tags.Length > 0)
                    definition.AllowRandomAnomalies ??= tags.Contains("allowrandomanomaly", StringComparison.OrdinalIgnoreCase);

                definition.Owner ??= ImportSectorMetadataResolver.ResolveOwner(area);
                definition.ResourceAreas = ImportSectorMetadataResolver.ParseResourceAreas(properties);
            }
        }

        private static bool TryParseFloat(string value, out float result)
        {
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        private static TranslationLookup LoadTranslations(string modDirectory)
        {
            var titles = new Dictionary<int, string>();
            var entries = new Dictionary<(int pageId, int textId), string>();

            string tRoot = Path.Combine(modDirectory, "t");
            if (!Directory.Exists(tRoot))
                return new TranslationLookup(titles, entries);

            foreach (string file in Directory
                .GetFiles(tRoot, "*.xml", SearchOption.TopDirectoryOnly)
                .OrderBy(a => a, StringComparer.OrdinalIgnoreCase))
            {
                var document = XDocument.Load(file);
                if (document.Root == null)
                    continue;

                foreach (XElement page in document.Descendants("page"))
                {
                    if (!int.TryParse((string)page.Attribute("id"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int pageId))
                        continue;

                    string title = NormalizeDisplayName((string)page.Attribute("title"));
                    if (!string.IsNullOrWhiteSpace(title))
                        titles[pageId] = title;

                    foreach (XElement text in page.Elements("t"))
                    {
                        if (!int.TryParse((string)text.Attribute("id"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int textId))
                            continue;

                        string value = text.Value;
                        if (!string.IsNullOrWhiteSpace(value))
                            entries[(pageId, textId)] = value;
                    }
                }
            }

            return new TranslationLookup(titles, entries);
        }

        private static string ResolveTranslationReference(string reference, TranslationLookup translations)
        {
            if (!TryParseTranslationReference(reference, out int pageId, out int textId))
                return NormalizeDisplayName(reference);

            return translations.TryResolveEntry(pageId, textId, out string value) ? value : null;
        }

        private static string ResolveTranslationPageTitle(string reference, TranslationLookup translations)
        {
            if (!TryParseTranslationReference(reference, out int pageId, out _))
                return null;

            if (!translations.TryGetTitle(pageId, out string title))
                return null;

            return IsPlaceholderTranslationValue(title) ? null : title;
        }

        private static bool TryParseTranslationReference(string reference, out int pageId, out int textId)
        {
            pageId = 0;
            textId = 0;
            if (string.IsNullOrWhiteSpace(reference))
                return false;

            var match = TranslationReferenceRegex().Match(reference.Trim());
            if (!match.Success)
                return false;

            return int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out pageId) &&
                   int.TryParse(match.Groups[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out textId);
        }

        private static string ExtractPrecedingDatasetComment(XElement dataset)
        {
            for (XNode node = dataset.PreviousNode; node != null; node = node.PreviousNode)
            {
                if (node is XComment comment)
                    return NormalizeDisplayName(comment.Value);

                if (node is XText text && string.IsNullOrWhiteSpace(text.Value))
                    continue;

                break;
            }

            return null;
        }

        private static bool IsPlaceholderDescription(string descriptionRef, string resolvedDescription)
        {
            if (string.Equals(NormalizeDisplayName(resolvedDescription), "None", StringComparison.OrdinalIgnoreCase))
                return true;

            return string.Equals(descriptionRef?.Replace(" ", string.Empty), "{8888892,8004}", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPlaceholderTranslationValue(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.StartsWith("import_fallback_", StringComparison.OrdinalIgnoreCase);
        }

        private static void UpsertMacro(Dictionary<string, MacroDefinition> macros, XElement macroElement, string displayNameHint)
        {
            var name = (string)macroElement.Attribute("name");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var definition = GetOrCreateMacro(macros, name);
            definition.Class = (string)macroElement.Attribute("class") ?? definition.Class;
            definition.DisplayName ??= NormalizeDisplayName(displayNameHint);
            definition.ContentRef ??= macroElement
                .Elements("connections")
                .Elements("connection")
                .FirstOrDefault(a => string.Equals((string)a.Attribute("ref"), "content", StringComparison.OrdinalIgnoreCase))
                ?.Element("macro")
                ?.Element("component")
                ?.Attribute("ref")
                ?.Value;
            definition.Connections.AddRange(ParseConnections(macroElement));
        }

        private static string NormalizeDisplayName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            string normalized = value.Replace('_', ' ');
            normalized = Regex.Replace(normalized, "(?<=[a-z0-9])(?=[A-Z])", " ");
            normalized = Regex.Replace(normalized, @"(?<=[A-Za-z])(?=\d)", " ");
            normalized = Regex.Replace(normalized, @"(?<=\d)(?=[A-Za-z])", " ");
            normalized = Regex.Replace(normalized, "\\s+", " ").Trim();
            normalized = normalized.Trim('-', ' ');

            if (normalized.StartsWith("Sector ", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized[7..].Trim();
            }

            var parts = normalized.Split(" - ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length >= 2 && Regex.IsMatch(parts[0], "^[A-Za-z#]+[A-Za-z0-9#]*$"))
            {
                normalized = string.Join(" - ", parts.Skip(1));
            }

            normalized = Regex.Replace(normalized, "\\bSector\\s*(\\d{3})$", a => $" {ToRomanNumeral(int.Parse(a.Groups[1].Value, CultureInfo.InvariantCulture))}", RegexOptions.IgnoreCase).Trim();
            normalized = Regex.Replace(normalized, "\\bCluster\\s*(\\d+)\\s+Sector\\s*(\\d{3})$", a => $"Cluster {a.Groups[1].Value} Sector {ToRomanNumeral(int.Parse(a.Groups[2].Value, CultureInfo.InvariantCulture))}", RegexOptions.IgnoreCase).Trim();
            normalized = Regex.Replace(normalized, "\\s+(Sector|Cluster)$", string.Empty, RegexOptions.IgnoreCase).Trim();
            normalized = Regex.Replace(normalized, @"\b([A-Za-z]+)\s+#\s*(\d+)\b", "$1 #$2");
            normalized = normalized.Replace("  ", " ");
            normalized = ToDisplayCase(normalized);

            return string.IsNullOrWhiteSpace(normalized) || normalized == "#" ? null : normalized;
        }

        private static string ExtractDisplayNameFromComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                return null;

            string normalized = comment.Replace('\t', ' ').Trim();
            normalized = Regex.Replace(normalized, "\\s+", " ");
            var match = SectorCommentRegex().Match(normalized);
            if (match.Success)
            {
                string value = NormalizeDisplayName(match.Groups[1].Value);
                return value == "#" ? null : value;
            }

            return null;
        }

        private static string NormalizeMacroFallbackName(string macroName)
        {
            if (string.IsNullOrWhiteSpace(macroName))
                return null;

            string normalized = macroName.Replace("_macro", string.Empty, StringComparison.OrdinalIgnoreCase);
            normalized = normalized.Replace("sectoe", "sector", StringComparison.OrdinalIgnoreCase);
            normalized = Regex.Replace(normalized, "^(thedeep_sector_|thedeep_cluster_)", string.Empty, RegexOptions.IgnoreCase);
            normalized = Regex.Replace(normalized, "^(homebrew_|cluster_|sector_|xpan_cluster)", string.Empty, RegexOptions.IgnoreCase);
            return NormalizeDisplayName(normalized);
        }

        private static string ToDisplayCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (Regex.IsMatch(word, "^(I|II|III|IV|V|VI|VII|VIII|IX|X|XI|XII|XIII|XIV|XV|XVI)$", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(word, @"^#?\d+$") ||
                    word.Contains('\''))
                {
                    words[i] = CapitalizeWord(word);
                    continue;
                }

                words[i] = EnglishTextInfo.ToTitleCase(word.ToLowerInvariant());
            }

            return string.Join(' ', words);
        }

        private static string CapitalizeWord(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var parts = value.Split('\'', StringSplitOptions.None);
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (string.IsNullOrEmpty(part))
                    continue;

                if (Regex.IsMatch(part, "^(I|II|III|IV|V|VI|VII|VIII|IX|X|XI|XII|XIII|XIV|XV|XVI)$", RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(part, @"^#?\d+$"))
                {
                    parts[i] = part.ToUpperInvariant();
                }
                else
                {
                    parts[i] = EnglishTextInfo.ToTitleCase(part.ToLowerInvariant());
                }
            }

            return string.Join("'", parts);
        }

        private static string NormalizeTranslationText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            string normalized = Regex.Replace(value, @"\{\d+,\d+\}", string.Empty);
            normalized = normalized.Replace("\t", " ");
            normalized = Regex.Replace(normalized, "\\s+", " ").Trim();
            return NormalizeDisplayName(normalized);
        }

        private static string NormalizeScriptVariableName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            string normalized = value.Trim().TrimStart('$');
            normalized = Regex.Replace(normalized, "_(Sector|Cluster)$", string.Empty, RegexOptions.IgnoreCase);
            normalized = Regex.Replace(normalized, "(Sector|Cluster)$", string.Empty, RegexOptions.IgnoreCase);
            normalized = normalized.Replace('_', ' ');
            normalized = Regex.Replace(normalized, "(?<=[a-z0-9])(?=[A-Z])", " ");
            normalized = Regex.Replace(normalized, "\\s+", " ").Trim();
            return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
        }

        private static string ToRomanNumeral(int value)
        {
            if (value <= 0)
                return value.ToString(CultureInfo.InvariantCulture);

            var numerals = new (int value, string numeral)[]
            {
                (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
                (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
                (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I")
            };

            var result = new System.Text.StringBuilder();
            int remaining = value;
            foreach (var (numeralValue, numeralText) in numerals)
            {
                while (remaining >= numeralValue)
                {
                    result.Append(numeralText);
                    remaining -= numeralValue;
                }
            }

            return result.ToString();
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

        private static ReferencedVanillaEndpoints CollectReferencedVanillaEndpoints(List<ConnectionDefinition> galaxyConnections, VanillaLookup vanillaLookup)
        {
            HashSet<string> sectorMacroNames = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> zoneMacroNames = new(StringComparer.OrdinalIgnoreCase);

            foreach (ConnectionDefinition connection in galaxyConnections.Where(a => a.IsGatePair))
            {
                CollectReferencedVanillaEndpoint(connection.Path, vanillaLookup, sectorMacroNames, zoneMacroNames);
                CollectReferencedVanillaEndpoint(connection.MacroPath, vanillaLookup, sectorMacroNames, zoneMacroNames);
            }

            return new ReferencedVanillaEndpoints(sectorMacroNames, zoneMacroNames);
        }

        private static void CollectReferencedVanillaEndpoint(
            string path,
            VanillaLookup vanillaLookup,
            HashSet<string> sectorMacroNames,
            HashSet<string> zoneMacroNames)
        {
            string normalizedPath = NormalizeConnectionPath(path);
            if (string.IsNullOrWhiteSpace(normalizedPath))
                return;

            string[] segments = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 3)
                return;

            string sectorConnectionName = segments[1];
            string zoneConnectionName = segments[2];
            string sectorMacroName = sectorConnectionName.Replace("_connection", "_macro", StringComparison.OrdinalIgnoreCase);
            string zoneMacroName = zoneConnectionName.Replace("_connection", "_macro", StringComparison.OrdinalIgnoreCase);

            if (vanillaLookup.SectorsByMacroName.ContainsKey(sectorMacroName))
                sectorMacroNames.Add(sectorMacroName);
            if (vanillaLookup.ZonesByMacroName.ContainsKey(zoneMacroName))
                zoneMacroNames.Add(zoneMacroName);
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

        [GeneratedRegex(@"^Sector\s+[^-]+-\s*(.+?)\s*$", RegexOptions.IgnoreCase)]
        private static partial Regex SectorCommentRegex();

        [GeneratedRegex(@"^\s*\{\s*(\d+)\s*,\s*(\d+)\s*\}\s*$", RegexOptions.IgnoreCase)]
        private static partial Regex TranslationReferenceRegex();

        [GeneratedRegex(@"\{\s*(\d+)\s*,\s*(\d+)\s*\}", RegexOptions.IgnoreCase)]
        private static partial Regex TranslationReferenceSearchRegex();

        [GeneratedRegex(@"_c(\d+)_s(\d+)", RegexOptions.IgnoreCase)]
        private static partial Regex CustomSectorSignatureRegex();

        private sealed class MacroDefinition(string name)
        {
            public string Name { get; } = name;
            public string Class { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public string ImageRef { get; set; }
            public string ContentRef { get; set; }
            public string MusicRef { get; set; }
            public string Owner { get; set; }
            public float? Sunlight { get; set; }
            public float? Economy { get; set; }
            public float? Security { get; set; }
            public bool? DisableFactionLogic { get; set; }
            public bool? AllowRandomAnomalies { get; set; }
            public List<Resource> ResourceAreas { get; set; } = [];
            public List<ConnectionDefinition> Connections { get; } = [];
            public bool IsCluster => Class != null && Class.Equals("cluster", StringComparison.OrdinalIgnoreCase);
        }

        private sealed class TranslationLookup(
            Dictionary<int, string> titles,
            Dictionary<(int pageId, int textId), string> entries)
        {
            public bool TryResolveEntry(int pageId, int textId, out string value)
            {
                return TryResolveEntry(pageId, textId, new HashSet<(int pageId, int textId)>(), out value);
            }

            public bool TryGetTitle(int pageId, out string value) => titles.TryGetValue(pageId, out value);

            private bool TryResolveEntry(int pageId, int textId, HashSet<(int pageId, int textId)> seen, out string value)
            {
                value = null;
                var key = (pageId, textId);
                if (!entries.TryGetValue(key, out string rawValue) || !seen.Add(key))
                    return false;

                string resolved = TranslationReferenceSearchRegex().Replace(rawValue, match =>
                {
                    if (!int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int nestedPageId) ||
                        !int.TryParse(match.Groups[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int nestedTextId))
                    {
                        return string.Empty;
                    }

                    return TryResolveEntry(nestedPageId, nestedTextId, seen, out string nestedValue)
                        ? nestedValue
                        : TryGetTitle(nestedPageId, out string nestedTitle)
                            ? nestedTitle
                            : string.Empty;
                });

                value = NormalizeTranslationText(resolved);
                return !string.IsNullOrWhiteSpace(value);
            }
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
        private sealed record ReferencedVanillaEndpoints(HashSet<string> SectorMacroNames, HashSet<string> ZoneMacroNames);

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

    internal sealed record ModImportResult(string ModName, List<Cluster> Clusters, List<string> Warnings);
}
