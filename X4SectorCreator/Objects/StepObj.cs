namespace X4SectorCreator.Objects
{
    public class StepObj
    {
        public string Type { get; set; }
        public string Position { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"[pos=\"{Position}\" val=\"{Value}\"]";
        }
    }
}
