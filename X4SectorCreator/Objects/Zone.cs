using System.Text.Json.Serialization;

namespace X4SectorCreator.Objects
{
    public class Zone : ICloneable
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

        public object Clone()
        {
            return new Zone
            {
                Id = Id,
                Name = Name,
                Position = Position,
                Gates = Gates.Select(a => (Gate)a.Clone()).ToList()
            };
        }
    }
}
