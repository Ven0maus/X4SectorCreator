using System.Text.Json;
using System.Text.Json.Serialization;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    internal static class ConfigSerializer
    {
        public static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string Serialize(List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            // Make a deep copy so we don't modify anything
            List<Cluster> clonedClusters = [.. clusters.Select(a => (Cluster)a.Clone())];

            // First order everything correctly before exporting
            clonedClusters = [.. clonedClusters.OrderBy(a => a.Id)];
            foreach (Cluster cluster in clonedClusters)
            {
                cluster.Sectors = [.. cluster.Sectors.OrderBy(a => a.Id)];
                foreach (Sector sector in cluster.Sectors)
                {
                    sector.Regions ??= []; // Support older config saves
                    sector.Regions = [.. sector.Regions];

                    if (cluster.IsBaseGame)
                    {
                        // For base game we need to make sure not to serialize everything, only the necessary
                        if (sector.IsBaseGame)
                            sector.Zones = [.. sector.Zones.Where(a => !a.IsBaseGame).OrderBy(a => a.Id)];
                        foreach (Zone zone in sector.Zones)
                        {
                            if (sector.IsBaseGame)
                                zone.Gates = [.. zone.Gates.Where(a => !a.IsBaseGame).OrderBy(a => a.Id)];
                        }
                    }
                    else
                    {
                        sector.Zones = [.. sector.Zones.OrderBy(a => a.Id)];
                        foreach (Zone zone in sector.Zones)
                        {
                            zone.Gates = [.. zone.Gates.OrderBy(a => a.Id)];
                        }
                    }
                }
            }

            // Reduces some object hierarchy like VanillaCluster, we don't need all info exported
            vanillaChanges.RemoveExportBloating();

            ConfigurationObj configObj = new()
            {
                Clusters = clonedClusters,
                RegionDefinitions = RegionDefinitionForm.RegionDefinitions,
                GalaxyName = GalaxySettingsForm.GalaxyName,
                VanillaChanges = vanillaChanges,
                Jobs = JobsForm.AllJobs.Select(a => a.Value).ToList(),
                Baskets = JobsForm.AllBaskets.Select(a => a.Value).ToList(),
                Version = new VersionChecker().CurrentVersion
            };

            return JsonSerializer.Serialize(configObj, SerializerOptions);
        }

        public static (List<Cluster> clusters, VanillaChanges vanillaChanges) Deserialize(string filePath)
        {
            ConfigurationObj configObj = null;
            try
            {
                configObj = JsonSerializer.Deserialize<ConfigurationObj>(filePath, SerializerOptions);
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

            #region Static values
            GalaxySettingsForm.GalaxyName = configObj.GalaxyName;
            GalaxySettingsForm.IsCustomGalaxy = configObj.IsCustomGalaxy;

            JobsForm.AllJobs.Clear();
            if (configObj.Jobs != null && configObj.Jobs.Count > 0)
            {
                foreach (var job in configObj.Jobs)
                    JobsForm.AllJobs.Add(job.Id, job);
            }

            JobsForm.AllBaskets.Clear();
            if (configObj.Baskets != null && configObj.Baskets.Count > 0)
            {
                foreach (var basket in configObj.Baskets)
                    JobsForm.AllBaskets.Add(basket.Id, basket);
            }

            // Set stored region definitions
            RegionDefinitionForm.RegionDefinitions.Clear();
            if (configObj.RegionDefinitions != null && configObj.RegionDefinitions.Count > 0)
            {
                RegionDefinitionForm.RegionDefinitions.AddRange(configObj.RegionDefinitions);
            }
            #endregion

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
