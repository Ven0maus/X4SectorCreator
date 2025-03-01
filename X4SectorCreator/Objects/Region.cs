namespace X4SectorCreator.Objects
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BoundaryType { get; set; }
        public string Density { get; set; }
        public string Rotation { get; set; }
        public string NoiseScale { get; set; }
        public string Seed { get; set; }
        public string MinNoiseValue { get; set; }
        public string MaxNoiseValue { get; set; }
        public string BoundaryRadius { get; set; }
        public string BoundaryLinear { get; set; }
        public Point Position { get; set; }

        public List<FieldObj> Fields { get; set; } = [];
        public List<Resource> Resources { get; set; } = [];
        public List<StepObj> Falloff { get; set; } = [];

        public override string ToString()
        {
            return Name;
        }
    }
}
