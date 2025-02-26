namespace X4SectorCreator.Objects
{
    public class Hexagon(PointF[] points, List<Hexagon> children)
    {
        public PointF[] Points { get; } = points;
        public List<Hexagon> Children { get; private set; } = children ?? [];
    }
}
