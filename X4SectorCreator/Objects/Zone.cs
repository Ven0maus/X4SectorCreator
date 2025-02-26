namespace X4SectorCreator.Objects
{
    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point Position { get; set; }
        public int Radius { get; set; } = 400; // Default 400km
        public List<Gate> Gates { get; set; }
    }
}
