using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.MapAlgorithms
{
    internal class PureRandom(ProceduralGalaxyForm.ProceduralSettings settings) : Procedural(settings)
    {
        public override IEnumerable<Cluster> Generate()
        {
            foreach (var coordinate in Coordinates)
            {
                if (Random.Next(100) < Settings.ClusterChance)
                {
                    Cluster cluster = new() { Name = "test", Position = coordinate, Sectors = [] };

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
                    yield return cluster;
                }
            }
        }
    }
}
