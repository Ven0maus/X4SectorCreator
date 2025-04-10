using System.Text.Json.Serialization;

namespace X4SectorCreator.Objects
{
    public class Zone : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point Position { get; set; }
        public bool IsGeneratedZone { get; set; }
        public List<Gate> Gates { get; set; } = [];
        public List<Station> Stations { get; set; } = [];

        /// <summary>
        /// Determines if it is a base game zone.
        /// </summary>
        [JsonIgnore]
        public bool IsBaseGame => Name != null;

        public object Clone()
        {
            return new Zone
            {
                Id = Id,
                Name = Name,
                Position = Position,
                IsGeneratedZone = IsGeneratedZone,
                Gates = Gates.Select(a => (Gate)a.Clone()).ToList(),
                Stations = Stations.Select(a => (Station)a.Clone()).ToList()
            };
        }
    }
}
