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

        public override string ToString()
        {
            return Name;
        }
    }
}
