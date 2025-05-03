using System.Windows.Forms;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Helpers;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.MapAlgorithms
{
    internal class Noise(ProceduralGalaxyForm.ProceduralSettings settings) : Procedural(settings)
    {
        public override IEnumerable<Cluster> Generate()
        {
            var noiseMap = OpenSimplex2.GenerateNoiseMap(Settings.Width, Settings.Height, Settings.Seed,
                Settings.NoiseScale, Settings.NoiseOctaves, Settings.NoisePersistance, Settings.NoiseLacunarity, Settings.NoiseOffset);

            foreach (var coordinate in Coordinates)
            {
                var gridCoordinate = coordinate.HexToSquareGridCoordinate();

                // Off-set negative coordinates to fit within the grid
                gridCoordinate = new Point(Settings.Width /2 + gridCoordinate.X, Settings.Height /2 + gridCoordinate.Y);

                var noise = noiseMap[gridCoordinate.Y * Settings.Width + gridCoordinate.X];
                if (noise > Settings.NoiseThreshold)
                {
                    Cluster cluster = new() { Name = "test", Position = coordinate, Sectors = [] };

                    // 2. Generate sectors in this cluster (0–3)
                    int numSectors = Random.Next(100) < Settings.MultiSectorChance ? Random.Next(1, 4) : 1;
                    for (int i = 0; i < numSectors; i++)
                    {
                        var sector = new Sector
                        {
                            Id = cluster.Sectors.Count,
                            Name = "test"
                        };
                        cluster.Sectors.Add(sector);
                    }

                    cluster.AutoPositionSectors();
                    yield return cluster;
                }
            }
        }

        public static void GenerateVisual(PictureBox noiseVisual, ProceduralGalaxyForm.ProceduralSettings settings)
        {
            var noiseMap = OpenSimplex2.GenerateNoiseMap(noiseVisual.Width, noiseVisual.Height, settings.Seed,
                settings.NoiseScale, settings.NoiseOctaves, settings.NoisePersistance, settings.NoiseLacunarity, settings.NoiseOffset);

            Bitmap bmp = new Bitmap(noiseVisual.Width, noiseVisual.Height);

            for (int x = 0; x < noiseVisual.Width; x++)
            {
                for (int y = 0; y < noiseVisual.Height; y++)
                {
                    // Clamp and scale float value to 0–255
                    float value = Math.Clamp(noiseMap[y * noiseVisual.Width + x], 0f, 1f);
                    int gray = (int)(value * 255);
                    Color color = Color.FromArgb(gray, gray, gray);

                    bmp.SetPixel(x, y, color);
                }
            }

            noiseVisual.Image = bmp;
        }
    }
}
