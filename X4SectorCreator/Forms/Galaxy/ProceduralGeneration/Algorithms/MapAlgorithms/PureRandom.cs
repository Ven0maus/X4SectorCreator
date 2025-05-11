using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.MapAlgorithms
{
    internal class PureRandom(ProceduralSettings settings) : Procedural(settings)
    {
        public override IEnumerable<Cluster> Generate()
        {
            int count = 0;
            foreach (var coordinate in Coordinates)
            {
                if (Random.Next(100) < Settings.ClusterChance)
                {
                    yield return CreateClusterAndSectors(coordinate, ++count);
                }
            }
        }
    }
}
