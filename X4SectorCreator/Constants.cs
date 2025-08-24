namespace X4SectorCreator
{
    internal static class Constants
    {
        internal static class DataPaths
        {
            public static readonly string SectorMappingFilePath = Path.Combine(Application.StartupPath, "Data/Mappings/sector_mappings.json");
            public static readonly string DlcMappingFilePath = Path.Combine(Application.StartupPath, "Data/Mappings/dlc_mappings.json");
            public static readonly string VanillaRegionsMappingFilePath = Path.Combine(Application.StartupPath, "Data/Mappings/vanilla_regions.json");
            public static readonly string VanillaStationsMappingFilePath = Path.Combine(Application.StartupPath, "Data/Mappings/vanilla_stations.json");
            public static readonly string VanillaConnectionMappingFilePath = Path.Combine(Application.StartupPath, "Data/Mappings/vanilla_connection_mappings.json");
            public static readonly string VanillaBasketsPath = Path.Combine(Application.StartupPath, "Data/Mappings/vanilla_baskets.xml");
            public static readonly string TemplateFactoriesDirectoryPath = Path.Combine(Application.StartupPath, "Data/TemplateFactories");
            public static readonly string TemplateJobsDirectoryPath = Path.Combine(Application.StartupPath, "Data/TemplateJobs");
            public static readonly string PredefinedFieldMappingFilePath = Path.Combine(Application.StartupPath, "Data/Mappings/predefinedfield_mappings.json");
            public static readonly string ClusterSoundtrackMappings = Path.Combine(Application.StartupPath, "Data/Mappings/cluster_soundtrack_mappings.json");
            public static readonly string ModulesMappingsPath = Path.Combine(Application.StartupPath, "Data/Mappings/modules_mappings.xml");
            public static readonly string DefaultCharactersPath = Path.Combine(Application.StartupPath, "Data/Presets/Characters/characters_default.xml");
            public static readonly string WaresMappingPath = Path.Combine(Application.StartupPath, "Data/Mappings/wares_mappings.xml");
            public static readonly string CustomGameStartPath = Path.Combine(Application.StartupPath, "Data/Mappings/custom_galaxy_gamestart.xml");
            public static readonly string VersionFilePath = Path.Combine(Application.StartupPath, "version.json");
            public static readonly string ModDirectoryPath = Path.Combine(Application.StartupPath, "GeneratedXml");
        }
    }
}
