namespace X4SectorCreator.Objects
{
    public class Hexagon((int, int) position, PointF[] points, List<Hexagon> children)
    {
        public (int, int) Position { get; } = position;
        public PointF[] Points { get; } = points;
        public List<Hexagon> Children { get; private set; } = children ?? [];
    }
}
