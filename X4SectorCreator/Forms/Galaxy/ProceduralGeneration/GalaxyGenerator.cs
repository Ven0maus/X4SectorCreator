using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.GateAlgorithms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration
{
    internal static class GalaxyGenerator
    {
        public static void CreateConnections(List<Cluster> clusters, int seed, int minGates, int maxGates)
        {
            // Clear all existing connections
            foreach (var zone in clusters.SelectMany(c => c.Sectors).SelectMany(s => s.Zones))
                zone.Gates.Clear();

            var mst = new GateBuilderMST(seed);
            mst.Generate(clusters);
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
    }
}
