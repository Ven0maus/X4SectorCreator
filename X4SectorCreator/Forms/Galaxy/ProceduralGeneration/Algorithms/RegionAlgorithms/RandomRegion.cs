using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Helpers;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Region = X4SectorCreator.Objects.Region;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.RegionAlgorithms
{
    internal class RandomRegion
    {
        private readonly List<(string Resource, float Weight)> _weightedResources = new();
        private readonly Random _random;
        private readonly ProceduralSettings _settings;

        public RandomRegion(ProceduralSettings settings, Dictionary<string, string> resources)
        {
            _settings = settings;
            _random = new(settings.Seed);
            _weightedResources = resources
                .Select(kvp => (kvp.Key, float.Parse(kvp.Value, CultureInfo.InvariantCulture)))
                .ToList();
        }

        public void GenerateMinerals(Cluster cluster, Sector sector)
        {
            var sectorPosition = cluster.Position.Add(sector.PlacementDirection);
            float richness = OpenSimplex2.Noise2(_settings.Seed, sectorPosition.X * 0.01f, sectorPosition.Y * 0.01f);
            richness = Math.Clamp((richness + 1f) / 2f, 0f, 1f); // Normalize noise [-1,1] -> [0,1]

            if (richness < 0.3f)
                return; // No minerals

            int nodeCount = 0;
            if (richness < 0.6f)
                nodeCount = _random.Next(1, 3); // Sparse
            else
                nodeCount = _random.Next(4, 8); // Rich

            for (int i = 0; i < nodeCount; i++)
            {
                var position = GenerateClusteredPoint(sector);
                var region = new Region { Position = position };
                sector.Regions.Add(region);
            }
        }

        private Point GenerateClusteredPoint(Sector sector)
        {
            var radius = sector.DiameterRadius / 2;
            double angle = _random.NextDouble() * 2 * Math.PI;
            double dist = _random.NextDouble() * radius * 0.8;

            int x = (int)(Math.Cos(angle) * dist);
            int y = (int)(Math.Sin(angle) * dist);
            return new Point(x, y);
        }

        private string PickResource()
        {
            float totalWeight = _weightedResources.Sum(r => r.Weight);
            float roll = (float)(_random.NextDouble() * totalWeight);

            float cumulative = 0f;
            foreach (var (resource, weight) in _weightedResources)
            {
                cumulative += weight;
                if (roll < cumulative)
                    return resource;
            }

            // Fallback (should never hit)
            return _weightedResources.Last().Resource;
        }
    }
}
