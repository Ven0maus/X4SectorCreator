using System.Text.Json.Serialization;

namespace X4SectorCreator.Objects
{
    public class Sector
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BaseGameMapping { get; set; }
        public bool DisableFactionLogic { get; set; } = false;
        public string Owner { get; set; }
        public float Sunlight { get; set; } = 1.0f;
        public float Economy { get; set; } = 1.0f;
        public float Security { get; set; } = 1.0f;
        public int DiameterRadius { get; set; } = 500000;
        public bool AllowRandomAnomalies { get; set; } = true;
        public string Tags { get; set; }
        public List<Zone> Zones { get; set; }
        public List<Region> Regions { get; set; }
        public SectorPlacement Placement { get; set; }

        [JsonIgnore]
        public Point Offset { get; set; }

        [JsonIgnore]
        public bool IsBaseGame => !string.IsNullOrWhiteSpace(BaseGameMapping);
    }

    public enum SectorPlacement
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        MiddleLeft,
        MiddleRight
    }
}
