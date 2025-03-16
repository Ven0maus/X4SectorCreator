namespace X4SectorCreator.Objects
{
    public class Station
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Faction { get; set; }
        public Point Position { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
