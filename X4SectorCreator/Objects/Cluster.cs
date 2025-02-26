using System.Text.Json.Serialization;

namespace X4SectorCreator.Objects
{
    public class Cluster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Sector> Sectors { get; set; }
        public Point Position { get; set; }

        [JsonIgnore]
        public Hexagon Hexagon { get; set; }
    }
}
