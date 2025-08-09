using System.Text.Json;
using System.Text.Json.Serialization;
using X4SectorCreator.Configuration.Converters;
using X4SectorCreator.Forms;
using X4SectorCreator.Forms.Stations;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    public static class ConfigSerializer
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(), new ColorJsonConverter(), new LongTupleConverter() }
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
                    sector.Regions = [.. sector.Regions.Where(a => !a.IsBaseGame)];

                    if (cluster.IsBaseGame)
                    {
                        // For base game we need to make sure not to serialize everything, only the necessary
                        if (sector.IsBaseGame)
                        {
                            sector.Zones = [.. sector.Zones.Where(a => !a.IsBaseGame).OrderBy(a => a.Id)];
                        }

                        foreach (Zone zone in sector.Zones)
                        {
                            if (sector.IsBaseGame)
                            {
                                zone.Gates = [.. zone.Gates.Where(a => !a.IsBaseGame).OrderBy(a => a.Id)];
                            }
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
                PlayerHQSector = GalaxySettingsForm.HeadQuartersSector,
                VanillaChanges = vanillaChanges,
                Factories = FactoriesForm.AllFactories.Select(a => a.Value).ToList(),
                Jobs = JobsForm.AllJobs.Select(a => a.Value).ToList(),
                Baskets = JobsForm.AllBaskets.Select(a => a.Value).ToList(),
                ConstructionPlans = ConstructionPlanViewForm.AllCustomConstructionPlans.Select(a => a.Value).ToList(),
                CustomFactions = FactionsForm.AllCustomFactions.Select(a => a.Value).ToList(),
                Version = new VersionChecker().CurrentVersion
            };

            return JsonSerializer.Serialize(configObj, JsonSerializerOptions);
        }

        public static (List<Cluster> clusters, VanillaChanges vanillaChanges) Deserialize(string filePath)
        {
            ConfigurationObj configObj = null;
            try
            {
                configObj = JsonSerializer.Deserialize<ConfigurationObj>(filePath, JsonSerializerOptions);
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
            GalaxySettingsForm.HeadQuartersSector = configObj.PlayerHQSector;
            GalaxySettingsForm.IsCustomGalaxy = configObj.IsCustomGalaxy;

            FactoriesForm.AllFactories.Clear();
            if (configObj.Factories != null && configObj.Factories.Count > 0)
            {
                foreach (Factory factory in configObj.Factories)
                {
                    FactoriesForm.AllFactories.Add(factory.Id, factory);
                }
            }

            JobsForm.AllJobs.Clear();
            if (configObj.Jobs != null && configObj.Jobs.Count > 0)
            {
                foreach (Job job in configObj.Jobs)
                {
                    JobsForm.AllJobs.Add(job.Id, job);
                }
            }

            JobsForm.AllBaskets.Clear();
            if (configObj.Baskets != null && configObj.Baskets.Count > 0)
            {
                foreach (Basket basket in configObj.Baskets)
                {
                    JobsForm.AllBaskets.Add(basket.Id, basket);
                }
            }

            FactionsForm.AllCustomFactions.Clear();
            if (configObj.CustomFactions != null && configObj.CustomFactions.Count > 0)
            {
                foreach (Faction faction in configObj.CustomFactions)
                {
                    FactionsForm.AllCustomFactions.Add(faction.Id, faction);
                }
            }

            RegionDefinitionForm.RegionDefinitions.Clear();
            if (configObj.RegionDefinitions != null && configObj.RegionDefinitions.Count > 0)
            {
                RegionDefinitionForm.RegionDefinitions.AddRange(configObj.RegionDefinitions);
            }

            ConstructionPlanViewForm.AllCustomConstructionPlans.Clear();
            if (configObj.ConstructionPlans != null && configObj.ConstructionPlans.Count > 0)
            {
                foreach (var constructionPlan in configObj.ConstructionPlans)
                {
                    ConstructionPlanViewForm.AllCustomConstructionPlans.Add(constructionPlan.Id, constructionPlan);
                }
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

        public class LongTupleConverter : JsonConverter<(long, long)>
        {
            public override void Write(Utf8JsonWriter writer, (long, long) value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("X", value.Item1);
                writer.WriteNumber("Y", value.Item2);
                writer.WriteEndObject();
            }

            public override (long, long) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                long x = 0;
                long y = 0;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        return (x, y);

                    if (reader.TokenType != JsonTokenType.PropertyName)
                        throw new JsonException();

                    string propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "X":
                            x = reader.GetInt64();
                            break;
                        case "Y":
                            y = reader.GetInt64();
                            break;
                        default:
                            reader.Skip(); // ignore unknown properties
                            break;
                    }
                }

                throw new JsonException("Invalid JSON for tuple (long, long)");
            }
        }
    }
}
