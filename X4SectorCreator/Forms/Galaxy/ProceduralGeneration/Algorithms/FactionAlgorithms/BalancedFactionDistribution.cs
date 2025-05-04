using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    internal class BalancedFactionDistribution(ProceduralSettings settings)
    {
        private readonly ProceduralSettings _settings = settings;
        private readonly Random _random = new(settings.Seed);

        public void GenerateFactions(List<Cluster> clusters)
        {
            var total = _random.Next(_settings.MinTotalFactions, _settings.MaxTotalFactions + 1);
            var factionCreator = new FactionCreator(_settings.Seed);

            // 20% of custom factions are pirates
            var pirates = (int)Math.Ceiling(total / 100f * 20f);
            var mainFactions = total - pirates;

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
            var allFactions = FactionsForm.AllCustomFactions;
            var pirates = allFactions.Where(a => a.Value.Tags.Contains("plunder")).ToArray();
            var main = allFactions.Except(pirates).ToArray();

            // For each main faction, define a starting cluster

            // Place wharf and shipyard in starting cluster (both in 1 sector, or divide over multiple sectors, if cluster is a multi sector)

            // Expand in a realistic manner to neighboring clusters / sectors

            // Try to guide expansion towards available sought-after resources (lacking resources in starting cluster)

            // Place defense stations near gates (to claim ownership of sectors)

            // Place trade station and equipment dock in a random owned cluster

            // Pirate factions will just spawn in left over unowned clusters (random chance for a piratedock/piratebase or freeport to spawn)
        }

        public void GenerateJobs(List<Cluster> clusters)
        {

        }

        public void GenerateFactories(List<Cluster> clusters)
        {

        }
    }
}
