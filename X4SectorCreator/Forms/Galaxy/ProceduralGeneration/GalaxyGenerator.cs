using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.MapAlgorithms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration
{
    internal static class GalaxyGenerator
    {
        public static List<Cluster> CreateClusters(ProceduralGalaxyForm.ProceduralSettings settings)
        {
            return GetMapAlgorithm(settings).Generate().ToList();
        }

        public static void CreateConnections(List<Cluster> clusters)
        {

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

        private static Procedural GetMapAlgorithm(ProceduralGalaxyForm.ProceduralSettings settings)
        {
            var mapAlgorithmCode = settings.MapAlgorithm.ToLower();
            return mapAlgorithmCode switch
            {
                "pure random" => new PureRandom(settings),
                "noise" => new Noise(settings),
                _ => throw new NotImplementedException($"\"{mapAlgorithmCode}\" is not implemented."),
            };
        }
    }
}
