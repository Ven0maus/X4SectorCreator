using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    internal class BalancedFactionDistribution(ProceduralSettings settings)
    {
        private readonly ProceduralSettings _settings = settings;
        private readonly Random _random = new(settings.Seed);

        public void GenerateFactions(List<Cluster> clusters)
        {
            var factionCreator = new FactionCreator(_settings.Seed);
            var pirates = _random.Next(_settings.MinPirateFactions, _settings.MaxPirateFactions + 1);
            var mainFactions = _random.Next(_settings.MinMainFactions, _settings.MaxMainFactions + 1);

            // Create main factions
            for (int i = 0; i < mainFactions; i++)
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

        public void GenerateStations(List<Cluster> clusters)
        {
            var stationGen = new StationGenAlgorithm(_random, _settings);
            stationGen.GenerateStations(clusters);
        }

        public void GenerateJobs(List<Cluster> clusters)
        {

        }

        public void GenerateFactories(List<Cluster> clusters)
        {

        }
    }
}
