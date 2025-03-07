using System.Text.Json.Serialization;
using X4SectorCreator.Forms;

namespace X4SectorCreator.Objects
{
    public class Cluster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BackgroundVisualMapping { get; set; }
        public string BaseGameMapping { get; set; }
        public string Dlc { get; set; }
        public List<Sector> Sectors { get; set; }
        public Point Position { get; set; }
        public bool CustomSectorPositioning { get; set; } = false;

        [JsonIgnore]
        public Hexagon Hexagon { get; set; }

        [JsonIgnore]
        public bool IsBaseGame => !string.IsNullOrWhiteSpace(BaseGameMapping);

        public void AutoPositionSectors()
        {
            var sectorCount = Sectors.Count;
            if (sectorCount <= 1) return; // Always centered, placement has no effect

            var combination = SectorForm.ValidSectorCombinations.First(a => a.Length == sectorCount);
            for (int i=0; i < sectorCount; i++)
                Sectors[i].Placement = combination[i];
        }
    }

    public enum ClusterOption
    {
        Custom,
        Vanilla,
        Both
    }
}
