using System.Text.Json.Serialization;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Configuration
{
    public class ConfigurationObj
    {
        public List<Cluster> Clusters { get; set; }
        public List<RegionDefinition> RegionDefinitions { get; set; }
        public VanillaChanges VanillaChanges { get; set; }
        public string GalaxyName { get; set; }
        public string Version { get; set; }

        [JsonIgnore]
        public bool IsCustomGalaxy => !GalaxyName.Equals("xu_ep2_universe", StringComparison.OrdinalIgnoreCase);
    }
}
