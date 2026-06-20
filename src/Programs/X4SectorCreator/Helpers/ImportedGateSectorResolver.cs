using X4SectorCreator.Objects;

namespace X4SectorCreator.Helpers
{
    internal static class ImportedGateSectorResolver
    {
        public sealed record GateReference(Cluster Cluster, Sector Sector, Zone Zone, Gate Gate);

        public static bool TryFindGateReferenceBySourcePath(IEnumerable<Cluster> clusters, string sourcePath, out GateReference reference)
        {
            reference = null;
            string normalizedSourcePath = GateConnectionResolver.NormalizePath(sourcePath);
            if (string.IsNullOrWhiteSpace(normalizedSourcePath))
                return false;

            reference = clusters
                .SelectMany(cluster => cluster.Sectors, (cluster, sector) => (cluster, sector))
                .SelectMany(pair => pair.sector.Zones, (pair, zone) => (pair.cluster, pair.sector, zone))
                .SelectMany(pair => pair.zone.Gates, (pair, gate) => new GateReference(pair.cluster, pair.sector, pair.zone, gate))
                .FirstOrDefault(a => string.Equals(
                    GateConnectionResolver.NormalizePath(a.Gate.SourcePath),
                    normalizedSourcePath,
                    StringComparison.OrdinalIgnoreCase));

            return reference != null;
        }

        public static bool TryFindCounterpartReference(IEnumerable<Cluster> clusters, Gate gate, out GateReference reference)
        {
            reference = null;
            return gate != null &&
                   !string.IsNullOrWhiteSpace(gate.DestinationPath) &&
                   TryFindGateReferenceBySourcePath(clusters, gate.DestinationPath, out reference);
        }

        public static bool GateTargetsSector(IEnumerable<Cluster> clusters, Gate gate, Sector sector)
        {
            if (gate == null || sector == null)
                return false;

            return TryFindCounterpartReference(clusters, gate, out GateReference reference) &&
                   ReferenceEquals(reference.Sector, sector);
        }
    }
}
