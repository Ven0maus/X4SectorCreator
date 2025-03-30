using System.Text.Json.Serialization;

namespace X4SectorCreator.Objects
{
    internal class VersionInfo
    {
        [JsonPropertyName("app_version")]
        public string AppVersion { get; set; }

        [JsonPropertyName("x4_version")]
        public string X4Version { get; set; }

        [JsonPropertyName("x4_mod_target_version")]
        public string X4ModTargetVersion { get; set; }
    }
}
