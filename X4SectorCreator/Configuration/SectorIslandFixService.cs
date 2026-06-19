using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class SectorIslandFixService
    {
        internal sealed record SectorIslandFixSummary(int IslandsDetected, int IslandsFixed);

        public static SectorIslandFixSummary Apply(List<Cluster> clusters)
        {
            int initiallyDetected = SectorIslandAnalyzer.FindIsolatedSectors(BuildConnectivityEntries(clusters)).Count;
            int fixedCount = 0;

            foreach (SectorRef source in GetAllSectorRefs(clusters).Where(a => !a.Sector.IsBaseGame).ToArray())
            {
                if (!IsCurrentlyIsolated(source, clusters))
                    continue;

                SectorRef target = FindClosestTarget(source, clusters);
                if (target == null)
                    continue;

                CreateApproximateConnection(source, target);
                fixedCount++;
            }

            return new SectorIslandFixSummary(initiallyDetected, fixedCount);
        }

        private static void CreateApproximateConnection(SectorRef source, SectorRef target)
        {
            bool sameCluster = ReferenceEquals(source.Cluster, target.Cluster);
            Gate.GateType gateType = SectorIslandFixerRules.GetConnectionType(sameCluster);

            Point sourceSectorCenter = SectorIslandFixerRules.GetSectorCenter(source.Cluster, source.Sector);
            Point targetSectorCenter = SectorIslandFixerRules.GetSectorCenter(target.Cluster, target.Sector);

            Point sourceLocalPosition = SectorIslandFixerRules.SelectBestTravelNode(source.Sector.DiameterRadius, sourceSectorCenter, targetSectorCenter);
            Point targetLocalPosition = SectorIslandFixerRules.SelectBestTravelNode(target.Sector.DiameterRadius, targetSectorCenter, sourceSectorCenter);

            Zone sourceZone = new()
            {
                Id = source.Sector.Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
                Position = sourceLocalPosition
            };

            Zone targetZone = new()
            {
                Id = target.Sector.Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
                Position = targetLocalPosition
            };

            Gate sourceGate = new()
            {
                Id = 1,
                ParentSectorName = source.Sector.Name,
                DestinationSectorName = target.Sector.Name,
                Type = gateType,
                Position = Point.Empty
            };

            Gate targetGate = new()
            {
                Id = 1,
                ParentSectorName = target.Sector.Name,
                DestinationSectorName = source.Sector.Name,
                Type = gateType,
                Position = Point.Empty
            };

            sourceZone.Gates.Add(sourceGate);
            targetZone.Gates.Add(targetGate);

            sourceGate.Source = BuildGateLocation(source.Cluster, source.Sector, sourceZone);
            sourceGate.Destination = BuildGateLocation(target.Cluster, target.Sector, targetZone);
            targetGate.Source = sourceGate.Destination;
            targetGate.Destination = sourceGate.Source;

            sourceGate.Yaw = SectorIslandFixerRules.CalculateYaw(sourceLocalPosition, new Point(targetSectorCenter.X - sourceSectorCenter.X + targetLocalPosition.X, targetSectorCenter.Y - sourceSectorCenter.Y + targetLocalPosition.Y));
            targetGate.Yaw = SectorIslandFixerRules.CalculateYaw(targetLocalPosition, new Point(sourceSectorCenter.X - targetSectorCenter.X + sourceLocalPosition.X, sourceSectorCenter.Y - targetSectorCenter.Y + sourceLocalPosition.Y));

            sourceGate.SetSourcePath("PREFIX", source.Cluster, source.Sector, sourceZone);
            targetGate.SetSourcePath("PREFIX", target.Cluster, target.Sector, targetZone);
            sourceGate.SetDestinationPath("PREFIX", target.Cluster, target.Sector, targetZone, targetGate);
            targetGate.SetDestinationPath("PREFIX", source.Cluster, source.Sector, sourceZone, sourceGate);

            source.Sector.Zones.Add(sourceZone);
            target.Sector.Zones.Add(targetZone);
        }

        private static SectorRef FindClosestTarget(SectorRef source, List<Cluster> clusters)
        {
            HashSet<string> isolatedSectorNames = SectorIslandAnalyzer.FindIsolatedSectors(BuildConnectivityEntries(clusters))
                .Select(a => a.SectorName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            Point sourceCenter = SectorIslandFixerRules.GetSectorCenter(source.Cluster, source.Sector);

            return GetAllSectorRefs(clusters)
                .Where(a => !ReferenceEquals(a.Sector, source.Sector))
                .OrderBy(a => isolatedSectorNames.Contains(a.Sector.Name))
                .ThenBy(a => DistanceSquared(sourceCenter, SectorIslandFixerRules.GetSectorCenter(a.Cluster, a.Sector)))
                .FirstOrDefault();
        }

        private static bool IsCurrentlyIsolated(SectorRef source, List<Cluster> clusters)
        {
            return SectorIslandAnalyzer.FindIsolatedSectors(BuildConnectivityEntries(clusters))
                .Any(a => a.SectorName.Equals(source.Sector.Name, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<SectorIslandAnalyzer.SectorConnectivityEntry> BuildConnectivityEntries(List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters)
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    string[] outboundSectorNames = sector.Zones
                        .SelectMany(a => a.Gates)
                        .Where(a => a.IsInterSectorGate)
                        .Select(a => a.DestinationSectorName)
                        .Where(a => !string.IsNullOrWhiteSpace(a))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    yield return new SectorIslandAnalyzer.SectorConnectivityEntry(
                        cluster.Name,
                        sector.Name,
                        sector.IsBaseGame,
                        outboundSectorNames);
                }
            }
        }

        private static IEnumerable<SectorRef> GetAllSectorRefs(List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters)
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    yield return new SectorRef(cluster, sector);
                }
            }
        }

        private static long DistanceSquared(Point a, Point b)
        {
            long dx = a.X - b.X;
            long dy = a.Y - b.Y;
            return (dx * dx) + (dy * dy);
        }

        private static string BuildGateLocation(Cluster cluster, Sector sector, Zone zone)
        {
            string clusterPart = cluster.IsBaseGame ? cluster.BaseGameMapping.CapitalizeFirstLetter() : $"c{cluster.Id:D3}";
            string sectorPart = sector.IsBaseGame ? sector.BaseGameMapping.CapitalizeFirstLetter() : $"s{sector.Id:D3}";
            return $"{clusterPart}_{sectorPart}_z{zone.Id:D3}";
        }

        private sealed record SectorRef(Cluster Cluster, Sector Sector);
    }
}
