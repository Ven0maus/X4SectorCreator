using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class ReversePathFixService
    {
        internal sealed record ReversePathFixSummary(int PathsNormalized, int ReverseGatesCreated, int BlankDestinationsIgnored);

        public static ReversePathFixSummary Apply(List<Cluster> clusters)
        {
            int normalizedCount = 0;
            int reverseGatesCreated = 0;
            int blankDestinationsIgnored = 0;

            var gateEntries = clusters
                .SelectMany(cluster => cluster.Sectors, (cluster, sector) => (cluster, sector))
                .SelectMany(pair => pair.sector.Zones, (pair, zone) => (pair.cluster, pair.sector, zone))
                .SelectMany(pair => pair.zone.Gates, (pair, gate) => (pair.cluster, pair.sector, pair.zone, gate))
                .ToList();

            foreach (var entry in gateEntries)
            {
                string normalizedSourcePath = GateConnectionResolver.NormalizePath(entry.gate.SourcePath);
                if (!string.Equals(entry.gate.SourcePath, normalizedSourcePath, StringComparison.Ordinal))
                {
                    entry.gate.SourcePath = normalizedSourcePath;
                    normalizedCount++;
                }

                string normalizedDestinationPath = GateConnectionResolver.NormalizePath(entry.gate.DestinationPath);
                if (!string.Equals(entry.gate.DestinationPath, normalizedDestinationPath, StringComparison.Ordinal))
                {
                    entry.gate.DestinationPath = normalizedDestinationPath;
                    normalizedCount++;
                }

                if (string.IsNullOrWhiteSpace(entry.gate.DestinationPath))
                {
                    blankDestinationsIgnored++;
                }
            }

            var zoneLookup = gateEntries
                .GroupBy(a => BuildZonePath(a.cluster, a.sector, a.zone), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(a => a.Key, a => a.First(), StringComparer.OrdinalIgnoreCase);

            var gateLookup = gateEntries
                .Where(a => !string.IsNullOrWhiteSpace(a.gate.SourcePath))
                .GroupBy(a => a.gate.SourcePath, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(a => a.Key, a => a.First(), StringComparer.OrdinalIgnoreCase);

            foreach (var entry in gateEntries.ToList())
            {
                if (string.IsNullOrWhiteSpace(entry.gate.DestinationPath) || gateLookup.ContainsKey(entry.gate.DestinationPath))
                    continue;

                string destinationPath = GateConnectionResolver.NormalizePath(entry.gate.DestinationPath);
                string targetZonePath = destinationPath?.Contains('/') == true
                    ? destinationPath[..destinationPath.LastIndexOf('/')]
                    : null;
                if (string.IsNullOrWhiteSpace(targetZonePath) || !zoneLookup.TryGetValue(targetZonePath, out var target))
                    continue;

                Gate reverseGate = new()
                {
                    Id = target.sector.Zones.SelectMany(a => a.Gates).DefaultIfEmpty(new Gate { Id = 0 }).Max(a => a.Id) + 1,
                    ConnectionName = destinationPath.Split('/').LastOrDefault(),
                    ParentSectorName = target.sector.Name,
                    DestinationSectorName = entry.sector.Name,
                    Source = BuildGateLocation(target.cluster, target.sector, target.zone),
                    Destination = entry.gate.Source,
                    SourcePath = destinationPath,
                    DestinationPath = entry.gate.SourcePath,
                    Type = entry.gate.Type,
                    Position = Point.Empty,
                    Yaw = (entry.gate.Yaw + 180) % 360,
                    Pitch = entry.gate.Pitch,
                    Roll = entry.gate.Roll,
                    IsHighwayGate = entry.gate.IsHighwayGate
                };

                target.zone.Gates.Add(reverseGate);
                gateLookup[reverseGate.SourcePath] = (target.cluster, target.sector, target.zone, reverseGate);
                reverseGatesCreated++;
            }

            return new ReversePathFixSummary(normalizedCount, reverseGatesCreated, blankDestinationsIgnored);
        }

        private static string BuildZonePath(Cluster cluster, Sector sector, Zone zone)
        {
            bool isFullBaseGame = cluster.IsBaseGame && sector.IsBaseGame;
            bool isHalfBaseGame = cluster.IsBaseGame && !sector.IsBaseGame;

            if (isFullBaseGame)
            {
                string clusterConnection = $"{cluster.BaseGameMapping.CapitalizeFirstLetter()}_connection";
                string sectorConnection = $"{cluster.BaseGameMapping.CapitalizeFirstLetter()}_{sector.BaseGameMapping.CapitalizeFirstLetter()}_connection";
                string zoneConnection = !string.IsNullOrWhiteSpace(zone.Name)
                    ? $"{zone.Name}_connection"
                    : $"PREFIX_ZO_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_{sector.BaseGameMapping.CapitalizeFirstLetter()}_z{zone.Id:D3}_connection";
                return $"{clusterConnection}/{sectorConnection}/{zoneConnection}";
            }

            if (isHalfBaseGame)
            {
                string clusterConnection = $"{cluster.BaseGameMapping.CapitalizeFirstLetter()}_connection";
                string sectorConnection = $"PREFIX_SE_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_s{sector.Id:D3}_connection";
                string zoneConnection = $"PREFIX_ZO_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_s{sector.Id:D3}_z{zone.Id:D3}_connection";
                return $"{clusterConnection}/{sectorConnection}/{zoneConnection}";
            }

            string customClusterConnection = $"PREFIX_CL_c{cluster.Id:D3}_connection";
            string customSectorConnection = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_connection";
            string customZoneConnection = $"PREFIX_ZO_c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}_connection";
            return $"{customClusterConnection}/{customSectorConnection}/{customZoneConnection}";
        }

        private static string BuildGateLocation(Cluster cluster, Sector sector, Zone zone)
        {
            string clusterPart = cluster.IsBaseGame ? cluster.BaseGameMapping.CapitalizeFirstLetter() : $"c{cluster.Id:D3}";
            string sectorPart = sector.IsBaseGame ? sector.BaseGameMapping.CapitalizeFirstLetter() : $"s{sector.Id:D3}";
            return $"{clusterPart}_{sectorPart}_z{zone.Id:D3}";
        }
    }
}
