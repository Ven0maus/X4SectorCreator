namespace X4SectorCreator.Objects
{
    public class Zone
    {
        public int Id { get; set; }
        public Point Position { get; set; }
        public List<Gate> Gates { get; set; }
    }
}
