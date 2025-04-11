using System.Text.Json;
using X4SectorCreator.Configuration;

namespace X4SectorCreator.Objects
{
    public sealed class SoundtrackCollection
    {
        private static SoundtrackCollection _instance;
        public static SoundtrackCollection Instance => _instance ??= Deserialize(File.ReadAllText(Constants.DataPaths.ClusterSoundtrackMappings));

        public Dictionary<string, List<string>> Soundtracks { get; set; }

        public static SoundtrackCollection Deserialize(string json)
        {
            return JsonSerializer.Deserialize<SoundtrackCollection>(json, ConfigSerializer.JsonSerializerOptions);
        }
    }
}
