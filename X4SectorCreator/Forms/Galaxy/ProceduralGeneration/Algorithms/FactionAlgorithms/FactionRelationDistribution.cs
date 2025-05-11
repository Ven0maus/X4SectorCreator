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

        private static readonly Dictionary<RelationType, (float Min, float Max)> RelationRanges = new()
        {
            [RelationType.Self] = (1.0f, 1.0f),
            [RelationType.Ally] = (0.5f, 1.0f),
            [RelationType.Member] = (0.1f, 1.0f),
            [RelationType.Friend] = (0.01f, 1.0f),
            [RelationType.Neutral] = (-0.01f, 0.01f),
            [RelationType.Enemy] = (-1.0f, -0.01f),
            [RelationType.KillMilitary] = (-1.0f, -0.1f),
            [RelationType.Kill] = (-1.0f, -0.32f),
            [RelationType.Nemesis] = (-1.0f, -1.0f),
        };

        private static readonly RelationType[] RandomRelationPool =
        [
            RelationType.Friend,
            RelationType.Neutral,
            RelationType.Enemy,
            RelationType.Kill,
            RelationType.Member,
            RelationType.Ally,
            RelationType.KillMilitary
        ];

        private float RandomFloat(float min, float max) =>
            (float)(_random.NextDouble() * (max - min) + min);

        private RelationType GetRandomRelationType() =>
            RandomRelationPool[_random.Next(RandomRelationPool.Length)];

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

                    if (isPirateA || isPirateB)
                    {
                        if (isPirateA && isPirateB)
                            value = 0.0032f; // pirates are friendly to each other
                        else
                            value = -0.0032f;
                    }
                    else if (hostileFactions.Contains(a) || hostileFactions.Contains(b))
                    {
                        // One of the two is hostile — use an enemy-style value
                        var (min, max) = RelationRanges[RelationType.Kill]; // Or Enemy/KillMilitary if preferred
                        value = RandomFloat(min, max);
                    }
                    else if (nemesisFaction != null && (a == nemesisFaction || b == nemesisFaction))
                    {
                        value = -1;
                    }
                    else
                    {
                        // Normal random
                        var type = GetRandomRelationType();
                        var (min, max) = RelationRanges[type];
                        value = RandomFloat(min, max);
                    }

                    // Assign symmetric relation
                    a.Relations.Relation.Add(new Faction.Relation { Faction = b.Id, RelationValue = value.ToString(CultureInfo.InvariantCulture) });
                    b.Relations.Relation.Add(new Faction.Relation { Faction = a.Id, RelationValue = value.ToString(CultureInfo.InvariantCulture) });
                }
            }
        }

        private static readonly RelationType[] _validPlayerRelations = [RelationType.Friend, RelationType.Neutral, RelationType.Enemy];
        private void SetPlayerRelations(List<Faction> pirateFactions, List<Faction> allFactions, Faction nemesis)
        {
            // Add player relations for pirates
            foreach (var pirate in pirateFactions)
            {
                pirate.Relations.Relation.Add(new Faction.Relation { Faction = "player", RelationValue = "-0.0032" });
            }

            // Set nemesis relation
            nemesis?.Relations.Relation.Add(new Faction.Relation { Faction = "player", RelationValue = "-1" });

            foreach (var faction in allFactions)
            {
                if (pirateFactions.Contains(faction) || (nemesis != null && nemesis == faction))
                {
                    continue;
                }

                var type = _validPlayerRelations[_random.Next(_validPlayerRelations.Length)];
                var (min, max) = RelationRanges[type];
                var value = RandomFloat(min, max);

                faction.Relations.Relation.Add(new Faction.Relation { Faction = "player", RelationValue = value.ToString(CultureInfo.InvariantCulture) });
            }
        }
    }
}
