using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    internal class BalancedFactionDistribution(ProceduralSettings settings)
    {
        private readonly ProceduralSettings _settings = settings;

        public void GenerateFactions(List<Cluster> clusters)
        {
            if (_settings.GenerateCustomFactions)
            {

            }

            // Vanilla factions already exist
        }

        public void GenerateJobs(List<Cluster> clusters)
        {
            if (_settings.GenerateCustomFactions)
            {

            }

            if (_settings.GenerateVanillaFactions)
            {

            }
        }

        public void GenerateFactories(List<Cluster> clusters)
        {
            if (_settings.GenerateCustomFactions)
            {

            }

            if (_settings.GenerateVanillaFactions)
            {

            }
        }

        public void GenerateStations(List<Cluster> clusters)
        {
            if (_settings.GenerateCustomFactions)
            {

            }

            if (_settings.GenerateVanillaFactions)
            {

            }
        }
    }
}
