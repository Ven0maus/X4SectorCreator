using System.Text.Json;
using System.Text.Json.Serialization;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class ConfigSerializer
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string Serialize(List<Cluster> clusters)
        {
            // First order everything correctly before exporting
            clusters = [.. clusters.OrderBy(a => a.Id)];
            foreach (Cluster cluster in clusters)
            {
                cluster.Sectors = [.. cluster.Sectors.OrderBy(a => a.Id)];
                foreach (Sector sector in cluster.Sectors)
                {
                    sector.Zones = [.. sector.Zones.OrderBy(a => a.Id)];
                    foreach (Zone zone in sector.Zones)
                    {
                        zone.Gates = [.. zone.Gates.OrderBy(a => a.Id)];
                    }
                }
            }

            return JsonSerializer.Serialize(clusters, _serializerOptions);
        }

        public static List<Cluster> Deserialize(string filePath)
        {
            List<Cluster> clusters = JsonSerializer.Deserialize<List<Cluster>>(filePath, _serializerOptions);

            // First order everything correctly before returning
            clusters = [.. clusters.OrderBy(a => a.Id)];
            foreach (Cluster cluster in clusters)
            {
                cluster.Sectors = [.. cluster.Sectors.OrderBy(a => a.Id)];
                foreach (Sector sector in cluster.Sectors)
                {
                    sector.Zones = [.. sector.Zones.OrderBy(a => a.Id)];
                    foreach (Zone zone in sector.Zones)
                    {
                        zone.Gates = [.. zone.Gates.OrderBy(a => a.Id)];
                    }
                }
            }

            return clusters;
        }
    }
}
