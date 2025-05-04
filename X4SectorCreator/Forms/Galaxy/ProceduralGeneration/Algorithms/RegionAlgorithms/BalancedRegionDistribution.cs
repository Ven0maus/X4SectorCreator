using System.Diagnostics;
using System.Globalization;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Helpers;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Region = X4SectorCreator.Objects.Region;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.RegionAlgorithms
{
    internal class BalancedRegionDistribution(ProceduralSettings settings, Dictionary<string, string> resources)
    {
        private readonly List<(string Resource, float Weight)> _weightedResources = resources
                .Select(kvp => (kvp.Key, float.Parse(kvp.Value, CultureInfo.InvariantCulture)))
                .ToList();
        private readonly Random _random = new(settings.Seed);
        private readonly ProceduralSettings _settings = settings;

        private static readonly Dictionary<string, double> _yieldDensities = new(StringComparer.OrdinalIgnoreCase)
        {
            ["lowest"] = 0.026,
            ["verylow"] = 0.06,
            ["lowminus"] = 0.2,
            ["low"] = 0.6,
            ["lowplus"] = 1.8,
            ["medlow"] = 4,
            ["medium"] = 6,
            ["medplus"] = 16,
            ["medhigh"] = 32,
            ["highlow"] = 48,
            ["high"] = 60,
            ["highplus"] = 120,
            ["veryhigh"] = 3600,
            ["highest"] = 60000
        };

        private readonly Dictionary<string, RegionDefinition> _regionDefinitions = new(StringComparer.OrdinalIgnoreCase);

        public void GenerateMinerals(List<Cluster> clusters, Cluster cluster, Sector sector)
        {
            var sectorPosition = cluster.Position.Add(sector.PlacementDirection);
            float richness = OpenSimplex2.Noise2(_settings.Seed, sectorPosition.X * 0.01f, sectorPosition.Y * 0.01f);
            richness = Math.Clamp((richness + 1f) / 2f, 0f, 1f); // Normalize noise [-1,1] -> [0,1]

            if (richness < 0.15f)
                return; // No resources

            int nodeCount;
            if (richness < 0.4f)
                nodeCount = _random.Next(0, 3); // Sparse
            else if (richness < 0.7f)
                nodeCount = _random.Next(1, 5); // Rich
            else
                nodeCount = _random.Next(2, 7); // Rich

            if (nodeCount == 0) return;

            var nearbyResources = GetNearbyClusters(clusters, cluster, 4)
                .SelectMany(a => a.Sectors)
                .SelectMany(a => a.Regions)
                .SelectMany(a => a.Definition.Resources)
                .Select(a => a.Ware)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < nodeCount; i++)
            {
                var position = GenerateClusteredPoint(sector);
                var resource = PickResource(nearbyResources);
                int attempts = 0;
                while (nearbyResources.Contains(resource))
                {
                    if (attempts >= 50)
                    {
                        nearbyResources.Clear();
                        break;
                    }

                    resource = PickResource(nearbyResources);
                    attempts++;
                }

                var yield = PickYield(richness);
                var regionName = $"{resource}_{yield}";
                if (!_regionDefinitions.TryGetValue(regionName, out var definition))
                {
                    _regionDefinitions[regionName] = definition = new RegionDefinition
                    {
                        Guid = new Guid().ToString(),
                        Name = regionName,
                        Resources =
                       [
                           new()
                            {
                                Ware = resource,
                                Yield = yield
                            }
                       ]
                    };
                }

                var region = new Region
                {
                    Name = $"{resource}_{i + 1}",
                    Position = position,
                    Definition = definition,
                    BoundaryLinear = "5000",
                    BoundaryRadius = "10000"
                };
                sector.Regions.Add(region);
                nearbyResources.Add(resource);
            }
        }

        private string PickYield(double richness)
        {
            if (richness < 0.4f)
                return _yieldDensities.Where(a => a.Value >= 0.2 && a.Value <= 16).Select(a => a.Key).RandomOrDefault(1, _random);
            else if (richness < 0.75f)
                return _yieldDensities.Where(a => a.Value >= 16 && a.Value <= 32).Select(a => a.Key).RandomOrDefault(1, _random);
            else 
                return _yieldDensities.Where(a => a.Value >= 32 && a.Value <= 60000).Select(a => a.Key).RandomOrDefault(1, _random);
        }

        private static List<Cluster> GetNearbyClusters(List<Cluster> clusters, Cluster targetCluster, float range)
        {
            // First, determine the range squared for efficient comparison
            float rangeSquared = range * range;

            // Assume each cluster has a Position (e.g., Point or PointF). Adjust if needed.
            var nearby = new List<Cluster>();

            foreach (var cluster in clusters)
            {
                if (cluster == targetCluster)
                    continue;

                var squarePosSource = cluster.Position.HexToSquareGridCoordinate();
                var squarePosTarget = targetCluster.Position.HexToSquareGridCoordinate();

                float dx = squarePosSource.X - squarePosTarget.X;
                float dy = squarePosSource.Y - squarePosTarget.Y;
                float distSquared = dx * dx + dy * dy;

                if (distSquared <= rangeSquared)
                {
                    nearby.Add(cluster);
                }
            }

            return nearby;
        }

        private Point GenerateClusteredPoint(Sector sector)
        {
            var radius = sector.DiameterRadius / 2;
            double angle = _random.NextDouble() * 2 * Math.PI;
            double dist = _random.NextDouble() * radius * 0.5;

            int x = (int)(Math.Cos(angle) * dist);
            int y = (int)(Math.Sin(angle) * dist);
            return new Point(x, y);
        }

        private string PickResource(HashSet<string> nearbyResources)
        {
            const float nearbyPenaltyMultiplier = 0.10f; // Reduce weight of nearby resources to 10%

            // Step 1: Adjust weights based on nearby presence
            var adjustedResources = _weightedResources
                .Select(r => new
                {
                    Resource = r.Resource,
                    Weight = nearbyResources.Contains(r.Resource) ? r.Weight * nearbyPenaltyMultiplier : r.Weight
                })
                .ToList();

            // Step 2: Weighted random selection as before
            float totalWeight = adjustedResources.Sum(r => r.Weight);
            float roll = (float)(_random.NextDouble() * totalWeight);

            float cumulative = 0f;
            foreach (var r in adjustedResources)
            {
                cumulative += r.Weight;
                if (roll < cumulative)
                    return r.Resource;
            }

            // Fallback (should never hit)
            return adjustedResources.Last().Resource;
        }

        public void PreventRegionStarvedSectors(List<Cluster> clusters)
        {
            int count = 0;
            foreach (var cluster in clusters)
            {
                foreach (var sector in cluster.Sectors)
                {
                    if (sector.Regions.Count == 0)
                    {
                        var nearbyResources = GetNearbyClusters(clusters, cluster, 3)
                            .SelectMany(a => a.Sectors)
                            .SelectMany(a => a.Regions)
                            .SelectMany(a => a.Definition.Resources)
                            .Select(a => a.Ware)
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);
                        if (nearbyResources.Count < 3)
                        {
                            var chance = _random.Next(100);
                            var totalRegions = chance < 15 ? 3 : chance < 40 ? 2 : 1;
                            for (int i = 0; i < totalRegions; i++)
                            {
                                var position = GenerateClusteredPoint(sector);
                                var resource = PickResource(nearbyResources);
                                int attempts = 0;
                                while (nearbyResources.Contains(resource))
                                {
                                    if (attempts >= 50)
                                    {
                                        nearbyResources.Clear();
                                        break;
                                    }

                                    resource = PickResource(nearbyResources);
                                    attempts++;
                                }

                                var yield = PickYield(_random.NextDouble());
                                var regionName = $"{resource}_{yield}";
                                if (!_regionDefinitions.TryGetValue(regionName, out var definition))
                                {
                                    _regionDefinitions[regionName] = definition = new RegionDefinition
                                    {
                                        Guid = new Guid().ToString(),
                                        Name = regionName,
                                        Resources =
                                        [
                                            new()
                                            {
                                                Ware = resource,
                                                Yield = yield
                                            }
                                        ]
                                    };
                                }

                                var region = new Region
                                {
                                    Name = $"{resource}_{yield}_{sector.Regions.Count + 1}",
                                    Position = position,
                                    Definition = definition,
                                    BoundaryLinear = "5000",
                                    BoundaryRadius = "10000"
                                };
                                nearbyResources.Add(resource);
                                sector.Regions.Add(region);
                                count++;
                            }
                        }
                    }
                }
            }
            Debug.WriteLine("Regions added for starved sectors: " + count);
        }
    }
}
