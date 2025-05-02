using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGenerators
{
    internal class GalaxyGenerator(ProceduralGalaxyForm.ProceduralSettings settings) : Procedural(settings)
    {
        public List<Cluster> GenerateGalaxy()
        {
            var clusters = new List<Cluster>();

            // Generate all possible hex positions within the width and height
            var hexPositions = GenerateGrid(Settings.Width, Settings.Height);

            foreach (var position in hexPositions)
            {
                if (Random.Next(100) < Settings.ClusterChance)
                {
                    Cluster cluster = new() { Name = "test", Position = position, Sectors = [] };

                    // 2. Generate sectors in this cluster (0–3)
                    int numSectors = Random.Next(100) < Settings.MultiSectorChance ? Random.Next(1, 4) : 1;
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
