using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.NameAlgorithms;
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

        private readonly ScifiNameGen _nameGenerator = new(settings);
        private readonly ScifiNameGen.NameStyle[] _nameStyles = Enum.GetValues<ScifiNameGen.NameStyle>();
        private static int _count = 0;

        protected Cluster CreateClusterAndSectors(Point coordinate)
        {
            Cluster cluster = new()
            {
                Id = ++_count,
                Position = coordinate,
                Sectors = [],
                Name = _nameGenerator.Generate(_nameStyles[Random.Next(_nameStyles.Length)], Random.Next(100) < 25)
            };

            // 2. Generate sectors in this cluster (1–3)
            int numSectors = Random.Next(100) < Settings.MultiSectorChance ? Random.Next(1, 4) : 1;
            for (int i = 0; i < numSectors; i++)
            {
                var sector = new Sector
                {
                    Id = cluster.Sectors.Count + 1,
                    DiameterRadius = Random.Next(200, 500) * 2 * 1000, // in km
                    Name = numSectors == 1 ? cluster.Name :
                        cluster.Name + " " + (cluster.Sectors.Count + 1).ToRomanString()
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
