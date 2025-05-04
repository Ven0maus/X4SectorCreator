using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.NameAlgorithms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    internal class BalancedFactionDistribution(ProceduralSettings settings)
    {
        private readonly ProceduralSettings _settings = settings;
        private readonly Random _random = new(settings.Seed);

        public void GenerateFactions(List<Cluster> clusters)
        {
            if (_settings.GenerateCustomFactions)
            {
                var total = _random.Next(_settings.MinTotalFactions, _settings.MaxTotalFactions + 1);

                // Define how many custom factions will be created
                int totalCustom;
                if (_settings.GenerateVanillaFactions)
                {
                    // Vanilla factions will take the flooring
                    totalCustom = (int)Math.Ceiling(total / 2f);
                }
                else
                {
                    totalCustom = total;
                }

                var factionCreator = new FactionCreator(_settings.Seed);

                // 20% of custom factions are pirates
                var pirates = (int)(totalCustom / 100f * 20f);
                var mainFactions = totalCustom - pirates;

                // Create main factions
                for (int i=0; i < mainFactions; i++)
                {
                    var faction = factionCreator.Generate(false);
                    FactionsForm.AllCustomFactions.Add(faction.Id, faction);
                }

                // Create pirate factions
                for (int i = 0; i < pirates; i++)
                {
                    var faction = factionCreator.Generate(true);
                    FactionsForm.AllCustomFactions.Add(faction.Id, faction);
                }
            }
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
