using System.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    internal class StationGenAlgorithm(Random random, ProceduralSettings settings)
    {
        private readonly Random _random = random;
        private readonly ProceduralSettings _settings = settings;

        public void GenerateStations(List<Cluster> clusters)
        {
            var allFactions = FactionsForm.AllCustomFactions;
            var pirates = allFactions
                .Select(a => a.Value)
                .Where(a => a.Tags.Contains("plunder"))
                .ToArray();
            var main = allFactions
                .Select(a => a.Value)
                .Except(pirates)
                .ToArray();

            // For each main faction, define a starting cluster
            var allSectors = clusters.SelectMany(a => a.Sectors).ToArray();
            var availableSectors = new List<Sector>(allSectors);
            var factionSectors = new Dictionary<Sector, Faction>();

            foreach (var faction in main)
            {
                var sector = GetBestStartingSector(availableSectors, clusters, factionSectors, faction);
                factionSectors[sector] = faction;
                availableSectors.Remove(sector);
            }

            // Place wharf and shipyard in starting cluster (both in 1 sector, or divide over multiple sectors, if cluster is a multi sector)
            foreach (var kvp in factionSectors)
            {
                var sector = kvp.Key;
                var faction = kvp.Value;

                // Random sector or first sector
                CreateStation(sector, "wharf", faction);
                CreateStation(sector, "shipyard", faction);

                // Set prefered HQ space
                var cluster = clusters.First(a => a.Sectors.Contains(sector));
                faction.PrefferedHqSpace = cluster.Sectors.Count == 1 ? GetClusterMacro(cluster) : GetSectorMacro(cluster, sector);
            }

            // Expand in a realistic manner to neighboring clusters / sectors
            foreach (var faction in main)
            {
                ExpandFactionTerritory(faction, factionSectors, availableSectors, clusters);
            }

            // Place trade station and equipment dock in a random owned cluster
            foreach (var faction in main)
            {
                var ownedSectors = factionSectors
                    .Where(a => a.Value == faction)
                    .Select(a => a.Key)
                    .ToArray();
                if (ownedSectors.Length != 0)
                {
                    var sector = ownedSectors.TakeRandom(2, _random).ToArray();
                    var tradeStationSector = sector.Length >= 2 ? sector[1] : sector[0];
                    var equipmentDockSector = sector[0];

                    CreateStation(tradeStationSector, "tradestation", faction);
                    CreateStation(equipmentDockSector, "equipmentdock", faction);
                }
            }

            // Place defense stations near gates (to claim ownership of sectors)
            foreach (var kvp in factionSectors)
            {
                var sector = kvp.Key;
                var faction = kvp.Value;

                // Exclude factions that don't claimspace
                if (!faction.Tags.Contains("claimspace", StringComparison.OrdinalIgnoreCase)) continue;

                var stations = 0;
                foreach (var zone in sector.Zones.ToList())
                {
                    if (stations > 0) break;
                    
                    foreach (var gate in zone.Gates)
                    {
                        if (stations > 0) break;

                        // Get random position x distance away in any direction from the gate
                        double angle = _random.NextDouble() * 2 * Math.PI; // Random angle in radians
                        int distance = _random.Next(30000, 70000); // Distance from the original point

                        int newX = zone.Position.X + (int)(Math.Cos(angle) * distance);
                        int newY = zone.Position.Y + (int)(Math.Sin(angle) * distance);

                        Point newPosition = new(newX, newY);

                        CreateStation(sector, "defence", faction, newPosition);
                        stations++;
                    }
                }
            }

            // Pirate factions will just spawn in left-over unowned clusters (random chance for a piratedock/piratebase or freeport to spawn)
            var unOwnedSectors = allSectors
                .Where(c => !factionSectors.ContainsKey(c))
                .ToList();

            foreach (var pirate in pirates)
            {
                // 2 - 5 pirate stations per pirate faction
                var amountOfStations = _random.Next(2, 6);
                Sector firstSector = null;
                for (int i = 0; i < amountOfStations; i++)
                {
                    // Make sure there are always sectors for pirates to spawn in
                    if (unOwnedSectors.Count == 0)
                        unOwnedSectors = [.. factionSectors.Keys];

                    var sector = unOwnedSectors[_random.Next(unOwnedSectors.Count)];
                    firstSector ??= sector;

                    unOwnedSectors.Remove(sector);

                    var type = _random.Next(3) switch
                    {
                        0 => "piratedock",
                        1 => "piratebase",
                        _ => "freeport"
                    };

                    CreateStation(sector, type, pirate);
                }

                // Set prefered HQ space
                var cluster = clusters.First(a => a.Sectors.Contains(firstSector));
                pirate.PrefferedHqSpace = cluster.Sectors.Count == 1 ? GetClusterMacro(cluster) : GetSectorMacro(cluster, firstSector);
            }
        }

        private static string GetClusterMacro(Cluster cluster)
        {
            return $"PREFIX_CL_c{cluster.Id:D3}_macro";
        }

        private static string GetSectorMacro(Cluster cluster, Sector sector)
        {
            return $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
        }

        private void CreateStation(Sector sector, string type, Faction faction, Point? position = null)
        {
            var zone = new Zone
            {
                Id = sector.Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
                Position = position ?? GetValidStationPosition(sector)
            };
            sector.Zones.Add(zone);

            var station = new Station
            {
                Faction = faction.Id,
                Id = sector.Zones.SelectMany(a => a.Stations).DefaultIfEmpty(new Station()).Max(a => a.Id) + 1,
                Owner = faction.Id,
                Race = faction.Primaryrace,
                Type = type,
                Position = zone.Position
            };
            station.Name = $"s{station.Id}_{type}";

            zone.Stations.Add(station);
        }

        private Point GetValidStationPosition(Sector sector)
        {
            var radius = sector.DiameterRadius / 2f;
            radius -= radius * 0.5f; // Reduce by 50%

            var positions = sector.Zones
                .Where(a => a.Stations.Count > 0)
                .SelectMany(a => a.Stations)
                .Select(a => a.Position)
                .ToHashSet();

            // squared dist
            long minDistance = 100000L * 100000L;

            var point = new Point(_random.Next(-(int)radius, (int)radius), _random.Next(-(int)radius, (int)radius));
            int attempts = 500;
            while (positions.Any(a => point.DistanceSquared(a) < minDistance))
            {
                if (attempts <= 0) break;
                point = new Point(_random.Next(-(int)radius, (int)radius), _random.Next(-(int)radius, (int)radius));
                attempts--;
            }
            return point;
        }

        private Sector GetBestStartingSector(List<Sector> sectors, List<Cluster> clusters, Dictionary<Sector, Faction> factionSectors, Faction faction)
        {
            // For simplicity, pick the sector with the most resources, adding randomness
            var scoredSectors = sectors
                .Where(a => !IsNearbyOtherFaction(clusters, a, faction, factionSectors, 5))
                .Select(c => new
                {
                    Sector = c,
                    Score = ScoreCluster(c)
                })
                .OrderByDescending(x => x.Score + _random.NextDouble())
                .Select(a => a.Sector);

            return scoredSectors.FirstOrDefault()
                ?? sectors
                .Where(a => !IsNearbyOtherFaction(clusters, a, faction, factionSectors, 2))
                .Select(c => new
                {
                    Sector = c,
                    Score = ScoreCluster(c)
                })
                .OrderByDescending(x => x.Score + _random.NextDouble())
                .Select(a => a.Sector)
                .FirstOrDefault()
                ?? sectors
                .Where(a => !IsNearbyOtherFaction(clusters, a, faction, factionSectors, 1))
                .Select(c => new
                {
                    Sector = c,
                    Score = ScoreCluster(c)
                })
                .OrderByDescending(x => x.Score + _random.NextDouble())
                .Select(a => a.Sector)
                .FirstOrDefault()
                ?? sectors.RandomOrDefault(_random);
        }

        private static int ScoreCluster(Sector sector)
        {
            int resourceScore = sector.Regions
                .SelectMany(a => a.Definition.Resources)
                .Select(a => a.Ware)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
            return resourceScore;
        }

        private void ExpandFactionTerritory(Faction faction, Dictionary<Sector, Faction> factionSectors, List<Sector> sectors, List<Cluster> clusters)
        {
            var startSector = factionSectors.First(a => a.Value == faction).Key;
            var expansionQueue = new Queue<Sector>();
            var ownedSectors = new HashSet<Sector> { startSector };
            expansionQueue.Enqueue(startSector);
            int maxExpansion = _random.Next(_settings.MinSectorOwnership, _settings.MaxSectorOwnership + 1);

            var sectorMapping = clusters.SelectMany(a => a.Sectors).ToDictionary(a => a.Name, a => a);
            while (expansionQueue.Count > 0 && ownedSectors.Count < maxExpansion)
            {
                var current = expansionQueue.Dequeue();

                // Score neighbors
                var neighbors = GetNeighbors(current, sectorMapping)
                    .Where(a => sectors.Contains(a))
                    .Select(n => (Sector: n, Score: GetResourceScore(n, ownedSectors, IsNearbyOtherFaction(clusters, n, faction, factionSectors, 3))))
                    .ToList();

                if (neighbors.Count == 0)
                    continue;

                var selected = WeightedRandomChoice(neighbors);
                if (selected != null)
                {
                    sectors.Remove(selected);
                    ownedSectors.Add(selected);
                    expansionQueue.Enqueue(selected);
                    factionSectors[selected] = faction;
                }
            }
        }

        private static bool IsNearbyOtherFaction(List<Cluster> clusters, Sector sector, Faction faction, Dictionary<Sector, Faction> factionSectors, int distance)
        {
            var cluster = clusters.First(a => a.Sectors.Contains(sector));
            var clusterPos = cluster.Position.HexToSquareGridCoordinate();
            var otherSectors = factionSectors.Where(a => a.Value != faction).Select(a => a.Key).ToHashSet();

            foreach (var otherSector in otherSectors)
            {
                var otherCluster = clusters.First(a => a.Sectors.Contains(otherSector));
                var pos = otherCluster.Position.HexToSquareGridCoordinate();
                if (pos.DistanceSquared(clusterPos) < (distance * distance))
                {
                    return true;
                }
            }
            return false;
        }

        private static IEnumerable<Sector> GetNeighbors(Sector sector, Dictionary<string, Sector> sectorMap)
        {
            foreach (var gate in sector.Zones.SelectMany(a => a.Gates))
            {
                if (sectorMap.TryGetValue(gate.DestinationSectorName, out var destSector))
                {
                    yield return destSector;
                }
            }
        }

        private static int GetResourceScore(Sector sector, HashSet<Sector> ownedSectors, bool isNearbyOtherFaction)
        {
            var ownedResources = ownedSectors
                .SelectMany(s => s.Regions)
                .SelectMany(a => a.Definition.Resources)
                .Select(a => a.Ware)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToHashSet();

            var score = sector.Regions
                .SelectMany(a => a.Definition.Resources)
                .Select(a => a.Ware)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count(r => !ownedResources.Contains(r));

            if (!isNearbyOtherFaction)
                score += 1;

            return score;
        }

        private Sector WeightedRandomChoice(List<(Sector Sector, int Score)> scoredSectors)
        {
            int totalScore = scoredSectors.Sum(x => x.Score);
            if (totalScore == 0) return scoredSectors.Select(a => a.Sector).RandomOrDefault(_random);

            int roll = _random.Next(totalScore);
            int cumulative = 0;

            foreach (var entry in scoredSectors)
            {
                cumulative += entry.Score;
                if (roll < cumulative)
                    return entry.Sector;
            }

            return scoredSectors.Select(a => a.Sector).RandomOrDefault(_random); // fallback
        }
    }
}
