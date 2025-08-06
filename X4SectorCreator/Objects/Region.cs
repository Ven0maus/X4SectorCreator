using System.Text.Json.Serialization;
using X4SectorCreator.Helpers;

namespace X4SectorCreator.Objects
{
    public class Region : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RegionDefinition Definition { get; set; }
        public string BoundaryRadius { get; set; }
        public string BoundaryLinear { get; set; }
        public Point Position { get; set; }

        [JsonIgnore]
        public bool IsBaseGame { get; set; } = false;

        public object Clone()
        {
            return new Region
            {
                BoundaryLinear = BoundaryLinear,
                BoundaryRadius = BoundaryRadius,
                Definition = Definition,
                Id = Id,
                Name = Name,
                Position = Position
            };
        }

        public string GetIdentifier(string modPrefix)
        {
            return $"{modPrefix}_re_{Definition.Name}_hash_{Localisation.GetFnvHash(Definition.Guid)}".ToLower();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
