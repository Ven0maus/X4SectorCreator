using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration
{
    internal abstract class Procedural(ProceduralSettings settings)
    {
        protected ProceduralSettings Settings { get; private set; } = settings;
        protected Random Random { get; private set; } = new Random(settings.Seed);
        protected Point[] Coordinates { get; private set; } = GenerateHexCoordinates(settings.Width, settings.Height);

        public abstract IEnumerable<Cluster> Generate();

        private static int _count = 0;
        protected Cluster CreateClusterAndSectors(Point coordinate)
        {
            Cluster cluster = new() { Id = _count++, Position = coordinate, Sectors = [] };
            cluster.Name = cluster.Id.ToString();

            // 2. Generate sectors in this cluster (0–3)
            int numSectors = Random.Next(100) < Settings.MultiSectorChance ? Random.Next(1, 4) : 1;
            for (int i = 0; i < numSectors; i++)
            {
                var sector = new Sector
                {
                    Id = cluster.Sectors.Count,
                    Name = cluster.Name + "_" + cluster.Sectors.Count
                };
                cluster.Sectors.Add(sector);
            }

            cluster.AutoPositionSectors();

            return cluster;
        }

        private static Point[] GenerateHexCoordinates(int width, int height)
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
