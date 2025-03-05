using System.Text.Json;
using System.Text.Json.Serialization;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class ConfigSerializer
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string Serialize(List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            // First order everything correctly before exporting
            clusters = [.. clusters.OrderBy(a => a.Id)];
            foreach (Cluster cluster in clusters)
            {
                cluster.Sectors = [.. cluster.Sectors.OrderBy(a => a.Id)];
                foreach (Sector sector in cluster.Sectors)
                {
                    sector.Regions ??= []; // Support older config saves
                    sector.Regions = [.. sector.Regions];
                    sector.Zones = [.. sector.Zones.OrderBy(a => a.Id)];
                    foreach (Zone zone in sector.Zones)
                    {
                        zone.Gates = [.. zone.Gates.OrderBy(a => a.Id)];
                    }
                }
            }

            ConfigurationObj configObj = new()
            {
                Clusters = clusters,
                RegionDefinitions = RegionDefinitionForm.RegionDefinitions,
                GalaxyName = GalaxySettingsForm.GalaxyName,
                VanillaChanges = vanillaChanges,
                Version = new VersionChecker().CurrentVersion
            };

            return JsonSerializer.Serialize(configObj, _serializerOptions);
        }

        public static (List<Cluster> clusters, VanillaChanges vanillaChanges) Deserialize(string filePath)
        {
            ConfigurationObj configObj = null;
            try
            {
                configObj = JsonSerializer.Deserialize<ConfigurationObj>(filePath, _serializerOptions);
            }
            catch (Exception)
            {
                _ = MessageBox.Show("Unable to import config file, it is likely because the file was exported from an older app version and may be incompatible.");
                return (null, null);
            }

            VersionChecker vc = new();

            // Validate app version, show message if from an older version
            if (!VersionChecker.CompareVersion(vc.CurrentVersion, configObj.Version))
            {
                _ = MessageBox.Show("Please note, if you have any issues after importing your config,\nit is likely because the file was exported from an older app version and may be incompatible.");
            }

            // Set static values
            GalaxySettingsForm.GalaxyName = configObj.GalaxyName;
            GalaxySettingsForm.IsCustomGalaxy = configObj.IsCustomGalaxy;

            // Set stored region definitions
            if (configObj.RegionDefinitions != null && configObj.RegionDefinitions.Count > 0)
            {
                RegionDefinitionForm.RegionDefinitions.AddRange(configObj.RegionDefinitions);
            }

            // First order everything correctly before returning
            List<Cluster> clusters = [.. configObj.Clusters.OrderBy(a => a.Id)];
            foreach (Cluster cluster in clusters)
            {
                cluster.Sectors = [.. cluster.Sectors.OrderBy(a => a.Id)];
                foreach (Sector sector in cluster.Sectors)
                {
                    sector.Regions ??= []; // Support older config saves
                    sector.Regions = [.. sector.Regions.OrderBy(a => a.Id)];
                    sector.Zones = [.. sector.Zones.OrderBy(a => a.Id)];
                    foreach (Zone zone in sector.Zones)
                    {
                        zone.Gates = [.. zone.Gates.OrderBy(a => a.Id)];
                    }
                }
            }

            return (clusters, configObj.VanillaChanges);
        }
    }
}
