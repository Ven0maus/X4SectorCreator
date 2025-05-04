using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.NameAlgorithms;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    internal static class FactionIconGen
    {
        public static Bitmap GenerateFactionIcon(int seed, FactionNameGen.FactionNameStyle style)
        {
            return style switch
            {
                FactionNameGen.FactionNameStyle.Human => GenerateHumanIcon(seed),
                FactionNameGen.FactionNameStyle.Alien => GenerateAlienIcon(seed),
                FactionNameGen.FactionNameStyle.Robot => GenerateRobotIcon(seed),
                _ => GenerateHumanIcon(seed)
            };
        }

        private static Bitmap GenerateHumanIcon(int seed)
        {
            var rand = new Random(seed);
            var bmp = new Bitmap(256, 256);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            // Simple flag stripes
            for (int i = 0; i < 3; i++)
            {
                var stripeColor = Color.FromArgb(255, rand.Next(256), rand.Next(256), rand.Next(256));
                g.FillRectangle(new SolidBrush(stripeColor), 0, i * 85, 256, 85);
            }

            // Central emblem
            var centerColor = Color.FromArgb(255, rand.Next(256), rand.Next(256), rand.Next(256));
            g.FillEllipse(new SolidBrush(centerColor), 78, 78, 100, 100);

            return bmp;
        }

        private static Bitmap GenerateAlienIcon(int seed)
        {
            var rand = new Random(seed);
            var bmp = new Bitmap(256, 256);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Black);

            var centerX = 128;
            var centerY = 128;

            // Draw fractal-style asymmetric tentacles
            for (int i = 0; i < 8; i++)
            {
                int points = 6 + rand.Next(6);
                var path = new List<Point>();
                double angle = i * Math.PI / 4 + rand.NextDouble() * 0.5;
                for (int j = 0; j < points; j++)
                {
                    var r = 20 + j * 15 + rand.Next(10);
                    var x = (int)(centerX + Math.Cos(angle + j * 0.2) * r);
                    var y = (int)(centerY + Math.Sin(angle + j * 0.2) * r);
                    path.Add(new Point(x, y));
                }
                g.DrawLines(new Pen(Color.LimeGreen, 2), path.ToArray());
            }

            // Alien eye / core
            var eyeColor = Color.FromArgb(255, rand.Next(100, 255), rand.Next(0, 100), rand.Next(100, 255));
            g.FillEllipse(new SolidBrush(eyeColor), 100, 100, 56, 56);

            return bmp;
        }

        private static Bitmap GenerateRobotIcon(int seed)
        {
            var rand = new Random(seed);
            var bmp = new Bitmap(256, 256);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.FromArgb(15, 15, 15)); // dark tech background

            // Grid lines
            var pen = new Pen(Color.DarkCyan, 1);
            for (int i = 0; i <= 256; i += 32)
            {
                g.DrawLine(pen, i, 0, i, 256);
                g.DrawLine(pen, 0, i, 256, i);
            }

            // Core circuitry
            for (int i = 0; i < 10; i++)
            {
                var x = rand.Next(0, 256);
                var y = rand.Next(0, 256);
                var w = rand.Next(10, 60);
                var h = rand.Next(10, 60);
                g.FillRectangle(new SolidBrush(Color.FromArgb(200, 50, 200, 255)), x, y, w, h);
            }

            // Central processor / emblem
            g.FillRectangle(Brushes.LightBlue, 108, 108, 40, 40);
            g.DrawRectangle(Pens.Cyan, 108, 108, 40, 40);

            return bmp;
        }
    }
}
