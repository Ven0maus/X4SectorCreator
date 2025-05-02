using X4SectorCreator.Forms.Galaxy.ProceduralGenerators.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGenerators
{
    internal class GalaxyGenerator(int seed) : Procedural(seed)
    {
        public List<Cluster> GenerateGalaxy(int width, int height)
        {
            var clusters = new List<Cluster>();

            // 1. Generate Clusters on a hex grid
            var sectorCounts = new[] { 1, 2, 3 };
            var sectorCountWeights = new[] { 0.95, 0.15, 0.05 };

            // Generate all possible hex positions within the width and height
            var hexPositions = GenerateGrid(width, height);

            double scale = 0.3;
            double threshold = 0.1;

            foreach (var hexPosition in hexPositions)
            {
                double noiseValue = OpenSimplex2.Noise2(Seed, hexPosition.X * scale, hexPosition.Y * scale);
                if (noiseValue > threshold)
                {
                    Cluster cluster = new() { Name = "test", Position = hexPosition, Sectors = [] };

                    // 2. Generate sectors in this cluster (0–3)
                    int numSectors = WeightedRandom(sectorCounts, sectorCountWeights);
                    for (int i = 0; i < numSectors; i++)
                    {
                        var sector = new Sector
                        {
                            Id = cluster.Sectors.Count,
                            Name = "test"
                        };
                        cluster.Sectors.Add(sector);
                    }

                    cluster.AutoPositionSectors();
                    clusters.Add(cluster);
                }
            }

            return clusters;
        }
    }
}
