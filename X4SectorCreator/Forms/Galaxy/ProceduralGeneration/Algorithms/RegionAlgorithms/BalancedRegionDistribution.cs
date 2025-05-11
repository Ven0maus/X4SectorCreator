using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using X4SectorCreator.Configuration;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Helpers;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Region = X4SectorCreator.Objects.Region;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.RegionAlgorithms
{
    internal class BalancedRegionDistribution
    {
        public BalancedRegionDistribution(ProceduralSettings settings, Dictionary<string, string> resources)
        {
            _settings = settings;
            _random = new(settings.Seed);
            _weightedResources = resources
                .Select(kvp => (kvp.Key, float.Parse(kvp.Value, CultureInfo.InvariantCulture)))
                .ToList();
            _regionDefinitionFieldsCache = InitializeFieldsCache();
        }

        private readonly List<(string Resource, float Weight)> _weightedResources;
        private readonly Random _random;
        private readonly ProceduralSettings _settings;

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
            ["highlow"] = 48
        };

        private readonly Dictionary<string, List<RegionDefinition>> _regionDefinitions = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FieldObj>> _regionDefinitionFieldsCache;

        private Dictionary<string, List<FieldObj>> InitializeFieldsCache()
        {
            var cache = new Dictionary<string, List<FieldObj>>(StringComparer.OrdinalIgnoreCase);
            var json = File.ReadAllText(Constants.DataPaths.PredefinedFieldMappingFilePath);
            var fields = JsonSerializer.Deserialize<List<FieldObj>>(json, ConfigSerializer.JsonSerializerOptions)
                .ToArray();

            foreach (var resource in _weightedResources.Select(a => a.Resource))
            {
                var fieldList = new List<FieldObj>();
                cache[resource] = fieldList;

                foreach (var field in fields)
                {
                    if (resource.Equals("rawscrap", StringComparison.OrdinalIgnoreCase) && field.Type != null && 
                        field.Type.Equals("debris", StringComparison.OrdinalIgnoreCase) && !field.GroupRef.Contains("station"))
                        fieldList.Add(field);
                    else if (field.GroupRef != null && field.GroupRef.Contains(resource, StringComparison.OrdinalIgnoreCase) && !field.GroupRef.Contains("nores"))
                        fieldList.Add(field);
                    else if (field.Resources != null && field.Resources.Contains(resource, StringComparison.OrdinalIgnoreCase))
                        fieldList.Add(field);
                }
            }

            return cache;
        }

        public void GenerateMinerals(Dictionary<string, Sector> sectorMap, Cluster cluster, Sector sector)
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

            var nearbyResources = GetNeighbors(sector, sectorMap, 3)
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

                var yield = PickYield(richness) ?? "low";
                var definition = GetOrCreateRegionDefinition(resource, yield);

                var radius = (int)(sector.DiameterRadius / 2f);
                var region = new Region
                {
                    Id = sector.Regions.DefaultIfEmpty(new Region()).Max(a => a.Id) + 1,
                    Position = position,
                    Definition = definition,
                    BoundaryLinear = _random.Next(2500, 15001).ToString(),
                    BoundaryRadius = ((int)(radius * (0.4 * (0.6 + _random.NextDouble())))).ToString()
                };
                region.Name = $"{resource}_{yield}_{region.Id}";
                sector.Regions.Add(region);
                nearbyResources.Add(resource);
            }
        }

        private RegionDefinition GetOrCreateRegionDefinition(string resource, string yield)
        {
            string definitionName = $"{resource}_{yield}";
            if (!_regionDefinitions.TryGetValue(definitionName, out var definitions))
            {
                _regionDefinitions[definitionName] = definitions = [];

                var uniqueFieldSets = new HashSet<string>(); // To store unique field key signatures

                int maxAttempts = 20; // Prevent infinite loop in case of limited combinations
                int attempts = 0;

                while (definitions.Count < 4 && attempts < maxAttempts)
                {
                    var density = _random.NextDouble() * _random.Next(1, 3);
                    while (density <= 0.001)
                        density = _random.NextDouble() * _random.Next(1, 3);

                    if (resource == "rawscrap")
                    {
                        density = 0.1;
                    }

                    var definition = new RegionDefinition
                    {
                        Guid = Guid.NewGuid().ToString(),
                        Name = definitionName + $"_{definitions.Count + 1}",
                        BoundaryType = "cylinder",
                        Density = density.ToString("#.##"),
                        Rotation = "0",
                        NoiseScale = _random.Next(2500, 10001).ToString(),
                        MinNoiseValue = "0.0",
                        MaxNoiseValue = "1",
                        Seed = _settings.Seed.ToString(),
                        Resources =
                        [
                            new()
                            {
                                Ware = resource,
                                Yield = yield
                            }
                        ]
                    };

                    SetupRegionDefinitionFields(definition);

                    // Create a unique signature for the set of fields (e.g., sorted field names/IDs)
                    string fieldKey = string.Join(",", definition.Fields
                        .Select(f => f.GroupRef ?? f.Ref)
                        .OrderBy(x => x));

                    if (uniqueFieldSets.Add(fieldKey)) // Only add if this field combination is new
                    {
                        definitions.Add(definition);
                        RegionDefinitionForm.RegionDefinitions.Add(definition);
                    }

                    attempts++;
                }
            }

            return definitions.RandomOrDefault(_random); // Assumes caller just wants any definition
        }

        private string PickYield(double richness)
        {
            if (richness < 0.4f)
                return _yieldDensities.Where(a => a.Value >= 0.2 && a.Value <= 1.8).Select(a => a.Key).RandomOrDefault(_random);
            else if (richness < 0.75f)
                return _yieldDensities.Where(a => a.Value >= 1.8 && a.Value <= 6).Select(a => a.Key).RandomOrDefault(_random);
            else 
                return _yieldDensities.Where(a => a.Value >= 6 && a.Value <= 48).Select(a => a.Key).RandomOrDefault(_random);
        }

        private static IEnumerable<Sector> GetNeighbors(Sector sector, Dictionary<string, Sector> sectorMap, int range)
        {
            foreach (var gate in sector.Zones.SelectMany(a => a.Gates))
            {
                if (sectorMap.TryGetValue(gate.DestinationSectorName, out var destSector))
                {
                    yield return destSector;

                    // Continue searching until range exceeds
                    if (range > 1)
                    {
                        foreach (var neighbor in GetNeighbors(destSector, sectorMap, range -1))
                            yield return neighbor;
                    }
                }
            }
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
                    r.Resource,
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

        private void SetupRegionDefinitionFields(RegionDefinition regionDefinition)
        {
            var resources = regionDefinition.Resources
                .Select(a => a.Ware)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var gases = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "methane", "helium", "hydrogen" };
            var fields = new List<FieldObj>();
            foreach (var resource in resources)
            {
                var fieldObjects = _regionDefinitionFieldsCache[resource];
                if (gases.Contains(resource))
                {
                    // Gases can take only one
                    var takenField = fieldObjects.RandomOrDefault(_random) ?? throw new Exception($"Missing field obj for resource \"{resource}\".");
                    fields.Add(takenField);
                }
                else
                {
                    // Others can take multiple
                    var values = fieldObjects.ToList();
                    var amount = _random.Next(1, fieldObjects.Count);

                    if (resource == "rawscrap")
                    {
                        amount = _random.Next(1, fieldObjects.Count / 2);
                    }

                    for (int i=0; i < amount; i++)
                    {
                        var selected = values.RandomOrDefault(_random);
                        if (selected != null)
                        {
                            values.Remove(selected);
                            fields.Add(selected);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            regionDefinition.Fields.AddRange(fields);
        }

        public void PreventRegionStarvedSectors(List<Cluster> clusters, Dictionary<string, Sector> sectorMap)
        {
            int count = 0;
            foreach (var cluster in clusters)
            {
                foreach (var sector in cluster.Sectors)
                {
                    var nearbyResources = GetNeighbors(sector, sectorMap, 3)
                        .SelectMany(a => a.Regions)
                        .SelectMany(a => a.Definition.Resources)
                        .Select(a => a.Ware)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                    if (nearbyResources.Count <= 5)
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

                            var yield = PickYield(_random.NextDouble()) ?? "low";
                            var definition = GetOrCreateRegionDefinition(resource, yield);

                            var radius = (int)(sector.DiameterRadius / 2f);
                            var region = new Region
                            {
                                Id = sector.Regions.DefaultIfEmpty(new Region()).Max(a => a.Id) + 1,
                                Position = position,
                                Definition = definition,
                                BoundaryLinear = _random.Next(2500, 15001).ToString(),
                                BoundaryRadius = ((int)(radius * (0.4 * (0.6 + _random.NextDouble())))).ToString()
                            };
                            region.Name = $"{resource}_{yield}_{region.Id}";
                            nearbyResources.Add(resource);
                            sector.Regions.Add(region);
                            count++;
                        }
                    }
                }
            }
        }
    }
}
