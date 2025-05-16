using System.Text.Json.Serialization;
using X4SectorCreator.Forms;
using X4SectorCreator.Helpers;

namespace X4SectorCreator.Objects
{
    public class Cluster : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BackgroundVisualMapping { get; set; }
        public string BaseGameMapping { get; set; }
        public string Soundtrack { get; set; }
        public string Dlc { get; set; }
        public List<Sector> Sectors { get; set; }
        public Point Position { get; set; }
        public bool CustomSectorPositioning { get; set; } = false;
        public string CustomClusterXml { get; set; }

        public const string TemplateClusterXml = @"<?xml version=""1.0""?>
<components>
	<component name=""{CLUSTERCODE}"" class=""celestialbody"">
		<source geometry=""assets\environments\cluster\Cluster_01_data""/>
		
	</component>
</components>";

        [JsonIgnore]
        public Hexagon Hexagon { get; set; }

        [JsonIgnore]
        public bool IsBaseGame => !string.IsNullOrWhiteSpace(BaseGameMapping);

        public void AutoPositionSectors(bool randomize = false, Random random = null)
        {
            int sectorCount = Sectors.Count;
            if (sectorCount <= 1)
            {
                return; // Always centered, placement has no effect
            }

            var combinations = SectorForm.ValidSectorCombinations
                .Where(a => a.Length == sectorCount)
                .ToArray();

            SectorPlacement[] combination = randomize ? 
                combinations.Random(random) : combinations.First();

            for (int i = 0; i < sectorCount; i++)
            {
                Sectors[i].Placement = combination[i];
            }
        }

        public object Clone()
        {
            return new Cluster
            {
                Id = Id,
                Dlc = Dlc,
                BackgroundVisualMapping = BackgroundVisualMapping,
                BaseGameMapping = BaseGameMapping,
                Soundtrack = Soundtrack,
                CustomSectorPositioning = CustomSectorPositioning,
                Hexagon = Hexagon,
                Name = Name,
                Position = Position,
                Description = Description,
                CustomClusterXml = CustomClusterXml,
                Sectors = Sectors.Select(a => (Sector)a.Clone()).ToList()
            };
        }

        public override string ToString()
        {
            return Name ?? "Unknown";
        }
    }

    public enum ClusterOption
    {
        Custom,
        Vanilla,
        Both
    }
}
