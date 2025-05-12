using System.Globalization;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    internal class FactionRelationDistribution(Random random)
    {
        private readonly Random _random = random;

        public enum RelationType
        {
            Self,
            Ally,
            Member,
            Friend,
            Neutral,
            Enemy,
            KillMilitary,
            Kill,
            Nemesis
        }

        private static readonly Dictionary<RelationType, float> RelationRanges = new()
        {
            [RelationType.Self] = 1.0f,
            [RelationType.Ally] = 0.5f,
            [RelationType.Member] = 0.1f,
            [RelationType.Friend] = 0.01f,
            [RelationType.Neutral] = 0f,
            [RelationType.Enemy] = -0.01f,
            [RelationType.KillMilitary] = -0.1f,
            [RelationType.Kill] = -0.32f,
            [RelationType.Nemesis] = -1f,
        };

        private static readonly RelationType[] _balancedRelationPool = [RelationType.Neutral, RelationType.Enemy];

        private RelationType GetRandomRelationType() =>
            _balancedRelationPool[_random.Next(_balancedRelationPool.Length)];

        public void DefineFactionRelations(List<Faction> mainFactions, List<Faction> pirateFactions)
        {
            var allFactions = mainFactions.Concat(pirateFactions).ToList();

            // Mark 30% of non-pirate factions as "hostile"
            int hostileCount = (int)Math.Ceiling(mainFactions.Count / 100f * 30);
            var hostileFactions = mainFactions
                .TakeRandom(hostileCount, _random)
                .ToHashSet();
            var nemesisFaction = hostileFactions.FirstOrDefault();
            if (nemesisFaction != null)
                hostileFactions.Remove(nemesisFaction);

            // Ensure all factions have initialized relations
            foreach (var faction in allFactions)
            {
                // Lock pirate faction relations
                faction.Relations ??= new Faction.RelationsObj { Relation = [], Locked = pirateFactions.Contains(faction) ? "1" : null };

                // Add default relations for criminal and smuggler
                faction.Relations.Relation.Add(new Faction.Relation { Faction = "criminal", RelationValue = "-0.5" });
                faction.Relations.Relation.Add(new Faction.Relation { Faction = "smuggler", RelationValue = "-0.06" });
            }

            // Ensure player relations are set too
            SetPlayerRelations(pirateFactions, allFactions, nemesisFaction);

            for (int i = 0; i < allFactions.Count; i++)
            {
                for (int j = i + 1; j < allFactions.Count; j++)
                {
                    var a = allFactions[i];
                    var b = allFactions[j];

                    float value;

                    // Special case: if either is a pirate
                    bool isPirateA = pirateFactions.Contains(a);
                    bool isPirateB = pirateFactions.Contains(b);

                    if (nemesisFaction != null && (a == nemesisFaction || b == nemesisFaction))
                    {
                        value = RelationRanges[RelationType.Nemesis];
                    }
                    else if (isPirateA || isPirateB)
                    {
                        if (isPirateA && isPirateB)
                            value = 0.0032f; // pirates are friendly to each other
                        else
                            value = -0.0032f;
                    }
                    else if (hostileFactions.Contains(a) || hostileFactions.Contains(b))
                    {
                        value = RelationRanges[RelationType.KillMilitary];
                    }
                    else
                    {
                        var type = GetRandomRelationType();
                        value = RelationRanges[type];
                    }

                    // Assign symmetric relation
                    a.Relations.Relation.Add(new Faction.Relation { Faction = b.Id, RelationValue = value.ToString(CultureInfo.InvariantCulture) });
                    b.Relations.Relation.Add(new Faction.Relation { Faction = a.Id, RelationValue = value.ToString(CultureInfo.InvariantCulture) });
                }
            }
        }

        private void SetPlayerRelations(List<Faction> pirateFactions, List<Faction> allFactions, Faction nemesis)
        {
            // Add player relations for pirates
            foreach (var pirate in pirateFactions)
            {
                pirate.Relations.Relation.Add(new Faction.Relation { Faction = "player", RelationValue = "-0.0032" }); // -5 pirate range
            }

            // Set nemesis relation
            nemesis?.Relations.Relation.Add(new Faction.Relation { Faction = "player", RelationValue = RelationRanges[RelationType.Nemesis].ToString(CultureInfo.InvariantCulture) });

            foreach (var faction in allFactions)
            {
                if (pirateFactions.Contains(faction) || (nemesis != null && nemesis == faction))
                {
                    continue;
                }

                var type = GetRandomRelationType();
                var value = RelationRanges[type];

                faction.Relations.Relation.Add(new Faction.Relation { Faction = "player", RelationValue = value.ToString(CultureInfo.InvariantCulture) });
            }
        }
    }
}
