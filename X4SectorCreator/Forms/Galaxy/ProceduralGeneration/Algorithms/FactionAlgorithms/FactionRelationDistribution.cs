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

        private static readonly RelationType[] RandomRelationPool = new[]
        {
            RelationType.Friend,
            RelationType.Neutral,
            RelationType.Enemy,
            RelationType.Kill,
            RelationType.Member,
            RelationType.Ally,
            RelationType.KillMilitary
        };

        private float RandomFloat(float min, float max) =>
            (float)(_random.NextDouble() * (max - min) + min);

        private RelationType GetRandomRelationType() =>
            RandomRelationPool[_random.Next(RandomRelationPool.Length)];

        public void DefineFactionRelations(List<Faction> mainFactions, List<Faction> pirateFactions)
        {
            var allFactions = mainFactions.Concat(pirateFactions).ToList();

            // Mark 50% of non-pirate factions as "hostile"
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
                faction.Relations ??= new Faction.RelationsObj { Relation = new() };
            }

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
    }
}
