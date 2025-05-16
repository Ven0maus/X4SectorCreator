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

        private static HashSet<string> _vanillaBackgroundMappings;

        protected Cluster CreateClusterAndSectors(Point coordinate, int count)
        {
            _vanillaBackgroundMappings ??= MainForm.Instance.InitAllVanillaClusters(false).Clusters
                .Where(a => string.IsNullOrWhiteSpace(a.Dlc))
                .Select(a => a.BackgroundVisualMapping)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            Cluster cluster = new()
            {
                Id = count,
                Position = coordinate,
                BackgroundVisualMapping = _vanillaBackgroundMappings.RandomOrDefault(Random),
                Soundtrack = SoundtrackCollection.Instance.Soundtracks["vanilla"].RandomOrDefault(Random),
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
                    DiameterRadius = Random.Next(100, Settings.MaxSectorRadius) * 2 * 1000, // in km
                    Name = numSectors == 1 ? cluster.Name :
                        cluster.Name + " " + (cluster.Sectors.Count + 1).ToRomanString(),
                    Sunlight = DetermineSunlightSample()
                };
                sector.InitializeOrUpdateZones();
                cluster.Sectors.Add(sector);
            }

            // Fix sector positioning + offset
            cluster.AutoPositionSectors(true, Random);
            foreach (var sector in cluster.Sectors)
                SectorForm.DetermineSectorOffset(cluster, sector);

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

        private static readonly double TwoPi = 2.0 * Math.PI;
        private float DetermineSunlightSample()
        {
            const int min = 0;
            const int max = 300;
            const int mean = 100;
            const int stdDev = 20;

            double u1 = Random.NextDouble();
            double u2 = Random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(TwoPi * u2);
            int result = (int)(mean + stdDev * randStdNormal + 0.5);
            result = result < min ? min : result > max ? max : result;
            return (float)result / 100;
        }
    }
}
