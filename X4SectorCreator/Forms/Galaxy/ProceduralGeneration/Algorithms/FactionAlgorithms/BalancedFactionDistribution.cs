using X4SectorCreator.Forms.Factories;
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
            var pirateFactions = _random.Next(_settings.MinPirateFactions, _settings.MaxPirateFactions + 1);
            var mainFactions = _random.Next(_settings.MinMainFactions, _settings.MaxMainFactions + 1);

            // Create main factions
            for (int i = 0; i < mainFactions; i++)
            {
                var faction = factionCreator.Generate(false);
                FactionsForm.AllCustomFactions.Add(faction.Id, faction);
            }

            var mainFactionObjects = FactionsForm.AllCustomFactions.Values.ToList();

            // Create pirate factions
            for (int i = 0; i < pirateFactions; i++)
            {
                var faction = factionCreator.Generate(true);
                FactionsForm.AllCustomFactions.Add(faction.Id, faction);
            }

            DefineFactionRelations(mainFactionObjects, FactionsForm.AllCustomFactions.Values.Except(mainFactionObjects).ToList());
            GenerateStations(clusters);
            GenerateQuotas(clusters);
        }

        private void DefineFactionRelations(List<Faction> mainFactions, List<Faction> pirateFactions)
        {
            new FactionRelationDistribution(_random).DefineFactionRelations(mainFactions, pirateFactions);
        }

        private void GenerateStations(List<Cluster> clusters)
        {
            var stationGen = new StationGenAlgorithm(_random, _settings);
            stationGen.GenerateStations(clusters);
        }

        private static void GenerateQuotas(List<Cluster> clusters)
        {
            foreach (var faction in FactionsForm.AllCustomFactions.Values) 
            {
                var coverage = clusters
                    .SelectMany(a => a.Sectors)
                    .SelectMany(a => a.Zones)
                    .SelectMany(a => a.Stations)
                    .Where(a => a.Owner == faction.Id)
                    .Count();
                PresetSelectionForm.ExecuteForProcGen(faction, coverage);
            }
        }
    }
}
