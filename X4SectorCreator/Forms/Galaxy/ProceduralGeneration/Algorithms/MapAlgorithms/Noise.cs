using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Helpers;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.MapAlgorithms
{
    internal class Noise(ProceduralSettings settings) : Procedural(settings)
    {
        public override IEnumerable<Cluster> Generate()
        {
            var noiseMap = OpenSimplex2.GenerateNoiseMap(Settings.Width, Settings.Height, Settings.Seed,
                Settings.NoiseScale, Settings.NoiseOctaves, Settings.NoisePersistance, Settings.NoiseLacunarity, Settings.NoiseOffset);

            int count = 0;
            foreach (var coordinate in Coordinates)
            {
                var gridCoordinate = coordinate.HexToSquareGridCoordinate();

                // Off-set negative coordinates to fit within the grid
                gridCoordinate = new Point(Settings.Width / 2 + gridCoordinate.X, Settings.Height / 2 + gridCoordinate.Y);

                var noise = Math.Clamp(noiseMap[gridCoordinate.Y * Settings.Width + gridCoordinate.X], 0f, 1f);
                if (noise < Settings.NoiseThreshold)
                {
                    yield return CreateClusterAndSectors(coordinate, ++count);
                }
            }
        }

        public static void GenerateVisual(PictureBox noiseVisual, ProceduralSettings settings)
        {
            var noiseMap = OpenSimplex2.GenerateNoiseMap(settings.Width, settings.Height, settings.Seed,
                settings.NoiseScale, settings.NoiseOctaves, settings.NoisePersistance, settings.NoiseLacunarity, settings.NoiseOffset);

            noiseVisual.Image = GenerateNoiseBitmap(noiseMap, settings.Width, settings.Height);
        }

        private static Bitmap GenerateNoiseBitmap(float[] noiseMap, int width, int height)
        {
            Bitmap bmp = new(width, height, PixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb
            );

            int stride = data.Stride;
            nint ptr = data.Scan0;
            int bytes = Math.Abs(stride) * height;
            byte[] rgbValues = new byte[bytes];

            for (int y = 0; y < height; y++)
            {
                // Invert the Y-coordinate to simulate bottom-left origin
                int invertedY = height - 1 - y;

                for (int x = 0; x < width; x++)
                {
                    float value = Math.Clamp(noiseMap[y * width + x], 0f, 1f);
                    byte gray = (byte)(value * 255);

                    // Calculate the index based on the inverted Y-coordinate
                    int index = invertedY * stride + x * 3;
                    rgbValues[index] = gray;        // Blue
                    rgbValues[index + 1] = gray;    // Green
                    rgbValues[index + 2] = gray;    // Red
                }
            }

            Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(data);

            return bmp;
        }
    }
}
