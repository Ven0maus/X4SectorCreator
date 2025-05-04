using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms;
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

            // Cleanup any unused region definitions from generation
            var definitions = clusters
                .SelectMany(a => a.Sectors)
                .SelectMany(a => a.Regions)
                .Select(a => a.Definition)
                .ToHashSet();
            RegionDefinitionForm.RegionDefinitions.RemoveAll(a => !definitions.Contains(a));
        }

        public static void CreateFactions(List<Cluster> clusters, ProceduralSettings settings)
        {
            Forms.FactoriesForm.AllFactories.Clear();
            Forms.JobsForm.AllJobs.Clear();
            Forms.FactionsForm.AllCustomFactions.Clear();

            var balancedGen = new BalancedFactionDistribution(settings);
            balancedGen.GenerateFactions(clusters);
            balancedGen.GenerateJobs(clusters);
            balancedGen.GenerateFactories(clusters);
            balancedGen.GenerateStations(clusters);
        }
    }
}
