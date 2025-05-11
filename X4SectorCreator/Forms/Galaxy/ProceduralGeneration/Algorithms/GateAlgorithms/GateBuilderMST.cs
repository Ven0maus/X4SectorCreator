using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Helpers;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.GateAlgorithms
{
    internal class GateBuilderMST(ProceduralSettings settings)
    {
        private readonly Random _random = new(settings.Seed);
        private readonly ProceduralSettings _settings = settings;

        public void Generate(List<Cluster> clusters)
        {
            // Generate based on selected distribution
            int count = clusters.Count;

            // Step 1: Build all pairwise distances
            var edges = new List<(Cluster A, Cluster B, float Distance)>();
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    float dist = clusters[i].Position.DistanceSquared(clusters[j].Position);
                    edges.Add((clusters[i], clusters[j], dist));
                }
            }

            // Step 2: Build MST using Kruskal's algorithm
            edges.Sort((a, b) => a.Distance.CompareTo(b.Distance));
            var mstEdges = new List<(Cluster A, Cluster B, float Distance)>();
            var uf = new UnionFind<int>(Enumerable.Range(0, clusters.Count));

            for (int i = 0; i < edges.Count; i++)
            {
                var (a, b, dist) = edges[i];
                int idxA = clusters.IndexOf(a);
                int idxB = clusters.IndexOf(b);

                if (uf.Union(idxA, idxB))
                {
                    mstEdges.Add((a, b, dist));
                    AddGate(a, a.Sectors.First(), b, b.Sectors.First());
                }
            }

            // Step 3: Connect any missing sectors that didn't cut the initial generation
            foreach (var cluster in clusters)
                ConnectMissingSectors(cluster);

            // Step 4: Add extra gates up to maxGatesPerCluster by chance
            AddExtraGates(clusters, edges, mstEdges);
        }

        private void AddExtraGates(List<Cluster> clusters, List<(Cluster A, Cluster B, float Distance)> edges, List<(Cluster A, Cluster B, float Distance)> mstEdges)
        {
            if (_settings.MaxGatesPerSector == 1) return;

            // Precompute distance-based neighbor map
            var clusterIndex = clusters.ToDictionary(c => c, clusters.IndexOf);
            var neighborMap = clusters.ToDictionary(
                c => c,
                c => edges
                        .Where(e => e.A == c || e.B == c)
                        .Select(e => e.A == c ? e.B : e.A)
                        .OrderBy(n => c.Position.DistanceSquared(n.Position))
                        .ToList()
            );

            // Track connections to avoid duplicates
            var connectedPairs = new HashSet<(Cluster, Cluster)>(
                mstEdges.Select(e => (e.A, e.B)).Concat(mstEdges.Select(e => (e.B, e.A)))
            );

            int count = 0;
            foreach (var cluster in clusters)
            {
                foreach (var sector in cluster.Sectors)
                {
                    if (_random.Next(100) > _settings.GateMultiChancePerSector)
                        continue;

                    int currentGates = sector.Zones.Sum(z => z.Gates.Count);
                    int maxExtra = _settings.MaxGatesPerSector - currentGates;

                    for (int i = 0; i < maxExtra; i++)
                    {
                        // Try to find a nearby unconnected cluster
                        foreach (var neighbor in neighborMap[cluster])
                        {
                            var key = (cluster, neighbor);
                            if (connectedPairs.Contains(key))
                                continue;

                            var neighborSector = neighbor.Sectors.Where(s => s.Zones.All(z => z.Gates.Count < _settings.MaxGatesPerSector)).RandomOrDefault(_random);
                            if (neighborSector == null)
                                continue;

                            // Connect them
                            AddGate(cluster, sector, neighbor, neighborSector);
                            connectedPairs.Add((cluster, neighbor));
                            connectedPairs.Add((neighbor, cluster));
                            count++;
                            break;
                        }
                    }
                }
            }
        }

        private void ConnectMissingSectors(Cluster cluster)
        {
            var freeSectors = cluster.Sectors
                .Where(s => s.Zones.All(z => z.Gates.Count == 0))
                .ToList();

            if (freeSectors.Count == 0) return;

            foreach (var freeSector in freeSectors)
            {
                var connectedSectors = cluster.Sectors
                    .Where(s => s.Zones.Any(z => z.Gates.Count > 0))
                    .ToList();

                var randomOtherSector = connectedSectors[_random.Next(connectedSectors.Count)];
                AddGate(cluster, randomOtherSector, cluster, freeSector);
            }
        }

        private void AddGate(Cluster sourceCluster, Sector sourceSector, Cluster targetCluster, Sector targetSector)
        {
            var directionSource = sourceCluster.Position.Add(sourceSector.PlacementDirection);
            if (sourceCluster.Sectors.Count == 1)
                directionSource = sourceCluster.Position;

            var directionTarget = targetCluster.Position.Add(targetSector.PlacementDirection);
            if (targetCluster.Sectors.Count == 1)
                directionTarget = targetCluster.Position;

            // Source
            var sourceZone = new Zone 
            { 
                Id = sourceSector.Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
                Position = CalculateValidGatePosition(sourceSector, directionTarget.GetDirection(directionSource)) 
            };
            var sourceGate = new Gate
            {
                Id = sourceSector.Zones.SelectMany(a => a.Gates).DefaultIfEmpty(new Gate()).Max(a => a.Id) + 1,
                DestinationSectorName = targetSector.Name,
                ParentSectorName = sourceSector.Name,
                Yaw = (int)sourceZone.Position.GetDirectionAngleCompassStyle(new Point(0, 0)) // Point towards center
            };
            sourceZone.Gates.Add(sourceGate);
            sourceSector.Zones.Add(sourceZone);

            // Target
            var targetZone = new Zone 
            {
                Id = targetSector.Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
                Position = CalculateValidGatePosition(targetSector, directionSource.GetDirection(directionTarget))
            };

            var targetGate = new Gate
            {
                Id = targetSector.Zones.SelectMany(a => a.Gates).DefaultIfEmpty(new Gate()).Max(a => a.Id) + 1,
                DestinationSectorName = sourceSector.Name,
                ParentSectorName = targetSector.Name,
                Yaw = (int)targetZone.Position.GetDirectionAngleCompassStyle(new Point(0, 0)) // Point towards center
            };
            targetZone.Gates.Add(targetGate);
            targetSector.Zones.Add(targetZone);

            sourceGate.Source = ConvertToPath(sourceCluster, sourceSector, sourceZone);
            sourceGate.Destination = ConvertToPath(targetCluster, targetSector, targetZone);
            targetGate.Source = sourceGate.Destination;
            targetGate.Destination = sourceGate.Source;

            targetGate.SetSourcePath("PREFIX", targetCluster, targetSector, targetZone);
            targetGate.SetDestinationPath("PREFIX", sourceCluster, sourceSector, sourceZone, sourceGate);
            sourceGate.SetSourcePath("PREFIX", sourceCluster, sourceSector, sourceZone);
            sourceGate.SetDestinationPath("PREFIX", targetCluster, targetSector, targetZone, targetGate);
        }

        private static string ConvertToPath(Cluster cluster, Sector sector, Zone zone)
        {
            return $"c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}";
        }

        private Point CalculateValidGatePosition(Sector sector, Point direction)
        {
            const int maxAttempts = 500;

            // We don't want diameter but radius so half the diameter
            var radius = (int)(sector.DiameterRadius / 2f);
            int minDistance = Math.Min(75000, (int)(radius / 2f));

            // Zone position determines gate's position (1 gate per zone)
            var existingPositions = sector.Zones
                .Where(a => a.Gates.Count > 0)
                .Select(g => g.Position)
                .ToList();

            // Get angle from center (0,0) toward direction
            double targetAngle = Math.Atan2(direction.Y, direction.X);

            for (int i = 0; i < maxAttempts; i++)
            {
                // Bias angle toward target direction ±30 degrees
                double angleOffset = (_random.NextDouble() - 0.5) * (Math.PI / 3); // ±30 degrees
                double angle = targetAngle + angleOffset;

                // Bias distance toward outer 35-80% of radius
                double distance = radius * (0.35 + 0.4 * _random.NextDouble());

                int x = (int)Math.Round(Math.Cos(angle) * distance);
                int y = (int)Math.Round(Math.Sin(angle) * distance);
                var candidate = new Point(x, y);

                if (!IsPointInsideFlatToppedHex(candidate, radius, 0.3f))
                    continue;

                bool tooClose = existingPositions.Any(pos => candidate.Distance(pos) < minDistance);

                if (!tooClose)
                    return candidate;
            }

            // Fallback if all attempts fail
            return new Point(_random.Next(0, minDistance), _random.Next(0, minDistance));
        }

        private static bool IsPointInsideFlatToppedHex(Point point, int radius, float marginPercent)
        {
            // Shrink hex radius by margin to stay clear of borders
            float r = radius * (1 - marginPercent);

            float px = point.X;
            float py = point.Y;

            float q2 = Math.Abs(px) * 0.57735f; // tan(30°)
            return Math.Abs(px) <= r &&
                   Math.Abs(py) <= r * 0.866f &&   // sin(60°)
                   Math.Abs(py) <= -q2 + r * 0.866f;
        }
    }
}
