using System.Diagnostics;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration
{
    internal static class GalaxyGenerator
    {
        public static void CreateConnections(List<Cluster> clusters, int minGates, int maxGates)
        {
            // Clear all existing connections
            foreach (var zone in clusters.SelectMany(c => c.Sectors).SelectMany(s => s.Zones))
                zone.Gates.Clear();

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
                    AddGate(a, b);
                }
            }

            // Step 3: Add extra gates up to maxGatesPerCluster
            /*
            Random rng = new Random();
            foreach (var cluster in clusters)
            {
                while (cluster.Gates.Count < minGates)
                {
                    // Get nearest eligible neighbors not already connected
                    var possibleTargets = clusters
                        .Where(c => c != cluster && !cluster.Gates.Any(g => g.Target == c) && c.Gates.Count < maxGates)
                        .OrderBy(c => Distance(cluster, c))
                        .ToList();

                    if (possibleTargets.Count == 0) break;

                    var target = possibleTargets[0]; // Closest one
                    float dist = Distance(cluster, target);
                    AddGate(cluster, target, dist);
                    AddGate(target, cluster, dist);
                }
            }
            */
        }

        private static void AddGate(Cluster from, Cluster to)
        {
            // Source
            var sourceSector = from.Sectors.First();
            var sourceZone = new Zone();
            var sourceGate = new Gate 
            { 
                DestinationSectorName = to.Name, 
                ParentSectorName = from.Name
            };

            // Target
            var targetSector = to.Sectors.First();
            var targetZone = new Zone();
            var targetGate = new Gate
            {
                DestinationSectorName = from.Name,
                ParentSectorName = to.Name
            };

            targetGate.SetSourcePath("PREFIX", to, targetSector, targetZone);
            targetGate.SetDestinationPath("PREFIX", from, sourceSector, sourceZone, sourceGate);
            sourceGate.SetSourcePath("PREFIX", from, sourceSector, sourceZone);
            sourceGate.SetDestinationPath("PREFIX", to, targetSector, targetZone, targetGate);

            targetSector.Zones.Add(targetZone);
            targetZone.Gates.Add(targetGate);
            sourceSector.Zones.Add(sourceZone);
            sourceZone.Gates.Add(sourceGate);   
        }

        public static void CreateRegions(List<Cluster> clusters)
        {

        }

        public static void CreateCustomFactions(List<Cluster> clusters)
        {

        }

        public static void CreateVanillaFactions(List<Cluster> clusters)
        {

        }

        private static float Distance(Point a, Point b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

    }
}
