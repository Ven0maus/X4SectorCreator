using X4SectorCreator.Helpers;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGenerators
{
    internal abstract class Procedural(ProceduralGalaxyForm.ProceduralSettings settings)
    {
        protected ProceduralGalaxyForm.ProceduralSettings Settings { get; private set; } = settings;
        protected Random Random { get; private set; } = new Random(settings.Seed);

        protected static Point[] GenerateGrid(int width, int height)
        {
            HashSet<Point> uniquePoints = [];
            for (int dx = -(width / 2); dx < width / 2; dx++)
            {
                for (int dy = -(height / 2); dy < height / 2; dy++)
                {
                    Point hexPoint = new Point(dx, dy).SquareGridToHexCoordinate();
                    uniquePoints.Add(hexPoint);
                }
            }
            return [.. uniquePoints];
        }
    }
}
