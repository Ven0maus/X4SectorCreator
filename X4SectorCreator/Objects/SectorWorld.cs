namespace X4SectorCreator.Objects
{
    public class SectorWorld : ICloneable
    {
        public string Part { get; set; }
        public decimal Factor { get; set; }

        public object Clone()
        {
            return new SectorWorld
            {
                Part = Part,
                Factor = Factor
            };
        }
    }
}
