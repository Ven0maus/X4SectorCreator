using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.GateAlgorithms;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.RegionAlgorithms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration
{
    internal static class GalaxyGenerator
    {
        public static void CreateConnections(List<Cluster> clusters, ProceduralSettings settings)
        {
            // Clear all existing connections
            foreach (var zone in clusters.SelectMany(c => c.Sectors).SelectMany(s => s.Zones))
                zone.Gates.Clear();

            var mst = new GateBuilderMST(settings);
            mst.Generate(clusters);
        }

        public static void CreateRegions(List<Cluster> clusters, ProceduralSettings settings)
        {
            // Clear all existing regions and region definitions
            RegionDefinitionForm.RegionDefinitions.Clear();
            foreach (var zone in clusters.SelectMany(c => c.Sectors))
                zone.Regions.Clear();

            var randomGen = new BalancedRegionDistribution(settings, settings.Resources);
            foreach (var cluster in clusters)
                foreach (var sector in cluster.Sectors)
                    randomGen.GenerateMinerals(clusters, cluster, sector);

            // Prevent sectors that have no regions and nearby neighbors have too little resources
            randomGen.PreventRegionStarvedSectors(clusters);
        }

        public static void CreateCustomFactions(List<Cluster> clusters)
        {
            Forms.FactoriesForm.AllFactories.Clear();
            Forms.JobsForm.AllJobs.Clear();
            Forms.FactionsForm.AllCustomFactions.Clear();
        }

        public static void CreateVanillaFactions(List<Cluster> clusters)
        {

        }
    }
}
