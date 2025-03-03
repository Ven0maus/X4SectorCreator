namespace X4SectorCreator.Objects
{
    public class RegionDefinition : IEquatable<RegionDefinition>
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string BoundaryType { get; set; }
        public string Density { get; set; }
        public string Rotation { get; set; }
        public string NoiseScale { get; set; }
        public string Seed { get; set; }
        public string MinNoiseValue { get; set; }
        public string MaxNoiseValue { get; set; }

        public List<FieldObj> Fields { get; set; } = [];
        public List<Resource> Resources { get; set; } = [];
        public List<StepObj> Falloff { get; set; } = [];

        public bool Equals(RegionDefinition other)
        {
            return other != null && Guid != null && other.Guid != null && Guid.Equals(other.Guid);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RegionDefinition);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
