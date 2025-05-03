using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.GateAlgorithms
{
    internal class GateBuilderMST(int seed)
    {
        private readonly Random _random = new(seed);

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
                    float dist = Distance(clusters[i].Position, clusters[j].Position);
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

            // Step 3: Add extra gates up to maxGatesPerCluster by chance
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

        private void AddGate(Cluster from, Sector fromSector, Cluster to, Sector toSector)
        {
            var sourceSector = fromSector;
            var targetSector = toSector;

            // Source
            var sourceZone = new Zone { Position = CalculateValidGatePosition(fromSector) };
            var sourceGate = new Gate
            {
                Id = sourceSector.Zones.SelectMany(a => a.Gates).Count() + 1,
                DestinationSectorName = targetSector.Name,
                ParentSectorName = sourceSector.Name
            };

            // Target
            var targetZone = new Zone { Position = CalculateValidGatePosition(toSector) };
            var targetGate = new Gate
            {
                Id = targetSector.Zones.SelectMany(a => a.Gates).Count() + 1,
                DestinationSectorName = sourceSector.Name,
                ParentSectorName = targetSector.Name
            };

            sourceGate.Source = ConvertToPath(from, sourceSector, sourceZone);
            sourceGate.Destination = ConvertToPath(to, targetSector, targetZone);
            targetGate.Source = sourceGate.Destination;
            targetGate.Destination = sourceGate.Source;

            targetGate.SetSourcePath("PREFIX", to, targetSector, targetZone);
            targetGate.SetDestinationPath("PREFIX", from, sourceSector, sourceZone, sourceGate);
            sourceGate.SetSourcePath("PREFIX", from, sourceSector, sourceZone);
            sourceGate.SetDestinationPath("PREFIX", to, targetSector, targetZone, targetGate);

            targetSector.Zones.Add(targetZone);
            targetZone.Gates.Add(targetGate);
            sourceSector.Zones.Add(sourceZone);
            sourceZone.Gates.Add(sourceGate);
        }

        private static string ConvertToPath(Cluster cluster, Sector sector, Zone zone)
        {
            return $"c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}";
        }

        private Point CalculateValidGatePosition(Sector sector)
        {
            // TODO: Adjust gate placement to border more between source/target sector for improved realism

            const int maxAttempts = 500;

            // We don't want diameter but radius so half the diameter
            var radius = (int)(sector.DiameterRadius / 2f);
            int minDistance = Math.Min(50_000, (int)(radius / 2f));

            // Zone position determines gate's position (1 gate per zone)
            var existingPositions = sector.Zones
                .Where(a => a.Gates.Count > 0)
                .Select(g => g.Position)
                .ToList();

            int numGates = sector.Zones.Count(a => a.Gates.Count > 0) + 1;

            for (int i = 0; i < maxAttempts; i++)
            {
                // Random polar coordinates
                double angle = (2 * Math.PI / numGates) * i + _random.NextDouble() * (2 * Math.PI / numGates);
                double distance = Math.Sqrt(_random.NextDouble()) * radius;

                int x = (int)Math.Round(Math.Cos(angle) * distance);
                int y = (int)Math.Round(Math.Sin(angle) * distance);
                var candidate = new Point(x, y);

                if (!IsPointInsideFlatToppedHex(candidate, radius, 0.5f))
                    continue;

                bool tooClose = existingPositions.Any(pos => Distance(pos, candidate) < minDistance);

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

        private static float Distance(Point a, Point b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }
    }
}
