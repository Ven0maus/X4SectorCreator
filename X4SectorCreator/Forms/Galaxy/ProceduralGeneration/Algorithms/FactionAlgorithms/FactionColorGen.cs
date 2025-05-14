namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    public class FactionColorGen(int seed)
    {
        private readonly Random _random = new(seed);
        private readonly List<Color> _usedColors = [];

        public Color GenerateDistinctColor()
        {
            const double minDistance = 0.25; // on 0–1 RGB scale

            for (int attempts = 0; attempts < 100; attempts++)
            {
                // Generate using spaced HSL: Vivid colors
                double hue;
                do
                {
                    hue = _random.NextDouble(); // 0-1
                } while (hue >= 0.25 && hue <= 0.42); // Avoid green range (player faction)

                double saturation = 0.7 + _random.NextDouble() * 0.3; // 0.7–1
                double lightness = 0.4 + _random.NextDouble() * 0.2;  // 0.4–0.6

                var color = HSLToRGB(hue, saturation, lightness);

                // Ensure uniqueness & separation
                if (_usedColors.All(c => ColorDistance(c, color) >= minDistance))
                {
                    _usedColors.Add(color);
                    return color;
                }
            }

            // Fallback after 100 attempts
            return Color.Gray;
        }

        private static double ColorDistance(Color a, Color b)
        {
            double dr = (a.R - b.R) / 255.0;
            double dg = (a.G - b.G) / 255.0;
            double db = (a.B - b.B) / 255.0;
            return Math.Sqrt(dr * dr + dg * dg + db * db);
        }

        private static Color HSLToRGB(double h, double s, double l)
        {
            double r, g, b;

            if (s == 0)
            {
                r = g = b = l; // achromatic
            }
            else
            {
                double q = l < 0.5
                    ? l * (1 + s)
                    : l + s - l * s;
                double p = 2 * l - q;
                r = HueToRGB(p, q, h + 1.0 / 3.0);
                g = HueToRGB(p, q, h);
                b = HueToRGB(p, q, h - 1.0 / 3.0);
            }

            return Color.FromArgb(
                (int)(r * 255),
                (int)(g * 255),
                (int)(b * 255));
        }

        private static double HueToRGB(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
            return p;
        }
    }
}
