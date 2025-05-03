using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.MapAlgorithms
{
    internal class PureRandom(ProceduralSettings settings) : Procedural(settings)
    {
        public override IEnumerable<Cluster> Generate()
        {
            foreach (var coordinate in Coordinates)
            {
                if (Random.Next(100) < Settings.ClusterChance)
                {
                    yield return CreateClusterAndSectors(coordinate);
                }
            }
        }
    }
}
