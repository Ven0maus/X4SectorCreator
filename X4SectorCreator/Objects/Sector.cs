using System.Text.Json.Serialization;

namespace X4SectorCreator.Objects
{
    public class Sector
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BaseGameMapping { get; set; }
        public string Owner { get; set; }
        public float Sunlight { get; set; } = 1.0f;
        public float Economy { get; set; } = 1.0f;
        public float Security { get; set; } = 1.0f;
        public bool AllowRandomAnomalies { get; set; } = true;
        public string Tags { get; set; }
        public List<Zone> Zones { get; set; }
        public Point Offset { get; set; }

        [JsonIgnore]
        public bool IsBaseGame => !string.IsNullOrWhiteSpace(BaseGameMapping);
    }
}
