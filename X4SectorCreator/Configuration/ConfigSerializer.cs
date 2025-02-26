using System.Text.Json;
using System.Text.Json.Serialization;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class ConfigSerializer
    {
        public static string Serialize(List<Cluster> clusters)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            // First order everything correctly before exporting
            clusters = clusters.OrderBy(a => a.Id).ToList();
            foreach (var cluster in clusters)
            {
                cluster.Sectors = cluster.Sectors.OrderBy(a => a.Id).ToList();
                foreach (var sector in cluster.Sectors)
                {
                    sector.Zones = sector.Zones.OrderBy(a => a.Id).ToList();
                    foreach (var zone in sector.Zones)
                    {
                        zone.Gates = zone.Gates.OrderBy(a => a.Id).ToList();
                    }
                }
            }

            return JsonSerializer.Serialize(clusters, options);
        }

        public static List<Cluster> Deserialize(string filePath)
        {
            var clusters = JsonSerializer.Deserialize<List<Cluster>>(filePath);

            // First order everything correctly before returning
            clusters = clusters.OrderBy(a => a.Id).ToList();
            foreach (var cluster in clusters)
            {
                cluster.Sectors = cluster.Sectors.OrderBy(a => a.Id).ToList();
                foreach (var sector in cluster.Sectors)
                {
                    sector.Zones = sector.Zones.OrderBy(a => a.Id).ToList();
                    foreach (var zone in sector.Zones)
                    {
                        zone.Gates = zone.Gates.OrderBy(a => a.Id).ToList();
                    }
                }
            }

            return clusters;
        }
    }
}
