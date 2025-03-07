using System.Text.Json.Serialization;

namespace X4SectorCreator.Objects
{
    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point Position { get; set; }
        public List<Gate> Gates { get; set; }

        /// <summary>
        /// Determines if it is a base game zone.
        /// </summary>
        [JsonIgnore]
        public bool IsBaseGame => Name != null;
    }
}
