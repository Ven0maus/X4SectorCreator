using X4SectorCreator.Helpers;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGenerators
{
    internal abstract class Procedural
    {
        protected int Seed { get; private set; }
        protected Random Random { get; private set; }

        public Procedural(int seed)
        {
            Random = new Random(seed);
            Seed = seed;
        }

        protected static Point[] GenerateGrid(int width, int height)
        {
            Point[] grid = new Point[width * height];
            for (int dx = -(width / 2); dx < width / 2; dx++)
            {
                for (int dy = -(height / 2); dy < height / 2; dy++)
                {
                    int index = (dy + height / 2) * width + dx + (width / 2);
                    grid[index] = new Point(dx, dy).SquareGridToHexCoordinate();
                }
            }
            return grid;
        }

        protected int WeightedRandom(int[] values, double[] weights)
        {
            double totalWeight = weights.Sum();
            double choice = Random.NextDouble() * totalWeight;

            for (int i = 0; i < weights.Length; i++)
            {
                if (choice < weights[i])
                    return values[i];
                choice -= weights[i];
            }
            return values[0];
        }
    }
}
