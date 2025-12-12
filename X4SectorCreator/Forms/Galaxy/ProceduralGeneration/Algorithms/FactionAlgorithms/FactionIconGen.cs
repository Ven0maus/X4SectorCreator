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
                _ => GenerateHumanIcon(seed)
            };
        }

        private static Bitmap GenerateHumanIcon(int seed)
        {
            var rand = new Random(seed);
            var bmp = new Bitmap(256, 256);
            using var g = Graphics.FromImage(bmp);

            int style = rand.Next(6);
            switch (style)
            {
                case 0: DrawTricolorBanner(g, rand); break;
                case 1: DrawEmblemOverField(g, rand); break;
                case 2: DrawDiagonalDivision(g, rand); break;
                case 3: DrawCrestAndStarline(g, rand); break;
                case 4: DrawGridAndPixel(g, rand); break;
                case 5: DrawRisingPath(g, rand); break;
            }

            return bmp;
        }

        static void DrawTricolorBanner(Graphics g, Random rand)
        {
            for (int i = 0; i < 3; i++)
            {
                var stripeColor = Color.FromArgb(255, rand.Next(256), rand.Next(256), rand.Next(256));
                g.FillRectangle(new SolidBrush(stripeColor), 0, i * 85, 256, 85);
            }

            var centerColor = Color.FromArgb(255, rand.Next(256), rand.Next(256), rand.Next(256));
            g.FillEllipse(new SolidBrush(centerColor), 78, 78, 100, 100);
        }

        static void DrawEmblemOverField(Graphics g, Random rand)
        {
            var bgColor = Color.FromArgb(255, rand.Next(100, 200), rand.Next(100, 200), rand.Next(100, 200));
            g.Clear(bgColor);

            var symbolColor = Color.FromArgb(255, 255 - bgColor.R, 255 - bgColor.G, 255 - bgColor.B);
            g.FillPolygon(new SolidBrush(symbolColor),
            [
                new Point(128, 60),
                new Point(170, 128),
                new Point(128, 196),
                new Point(86, 128)
            ]);
        }

        static void DrawDiagonalDivision(Graphics g, Random rand)
        {
            var color1 = Color.FromArgb(255, rand.Next(256), rand.Next(256), rand.Next(256));
            var color2 = Color.FromArgb(255, rand.Next(256), rand.Next(256), rand.Next(256));

            g.FillPolygon(new SolidBrush(color1), [new Point(0, 0), new Point(256, 0), new Point(0, 256)]);
            g.FillPolygon(new SolidBrush(color2), [new Point(256, 256), new Point(256, 0), new Point(0, 256)]);

            g.FillEllipse(new SolidBrush(Color.White), 108, 108, 40, 40);
        }

        static void DrawCrestAndStarline(Graphics g, Random rand)
        {
            // Randomize background color (dark blues, purples, or black)
            var backgroundColor = Color.FromArgb(rand.Next(0, 100), rand.Next(0, 50), rand.Next(100, 200)); // Random dark blue/purple
            g.Clear(backgroundColor);

            // Randomized emblem color (instead of always gold)
            var emblemColor = Color.FromArgb(rand.Next(100, 256), rand.Next(100, 256), rand.Next(100, 256));
            g.FillEllipse(new SolidBrush(emblemColor), 90 + rand.Next(-10, 11), 90 + rand.Next(-10, 11), 76 + rand.Next(-10, 11), 76 + rand.Next(-10, 11));

            // Draw randomized stars
            int starCount = rand.Next(4, 8);  // Random number of stars between 4 and 8
            for (int i = 0; i < starCount; i++)
            {
                int x = rand.Next(10, 200);  // Random X position within a range
                int y = rand.Next(10, 50);   // Random Y position (closer to the top)
                int size = rand.Next(5, 15); // Random size of the stars

                g.FillEllipse(new SolidBrush(Color.FromArgb(rand.Next(180, 256), rand.Next(180, 256), rand.Next(180, 256))), x, y, size, size);
            }

            // Optionally, add a random starline (connecting stars) if you like
            if (rand.NextDouble() > 0.5)  // 50% chance to draw a connecting line between stars
            {
                var starPoints = new List<Point>();
                for (int i = 0; i < starCount; i++)
                {
                    int x = rand.Next(10, 200);
                    int y = rand.Next(10, 50);
                    starPoints.Add(new Point(x, y));
                }
                g.DrawLines(new Pen(Color.White, 1), starPoints.ToArray());
            }
        }

        static void DrawGridAndPixel(Graphics g, Random rand)
        {
            g.Clear(Color.White);

            for (int x = 0; x < 256; x += 32)
            {
                for (int y = 0; y < 256; y += 32)
                {
                    if (rand.NextDouble() < 0.3)
                    {
                        var col = Color.FromArgb(255, rand.Next(255), rand.Next(255), rand.Next(255));
                        g.FillRectangle(new SolidBrush(col), x, y, 32, 32);
                    }
                }
            }

            g.DrawRectangle(new Pen(Color.Black, 2), 64, 64, 128, 128);
        }

        static void DrawRisingPath(Graphics g, Random rand)
        {
            g.Clear(Color.FromArgb(240, 240, 240));

            var baseColor = Color.FromArgb(255, rand.Next(100, 255), rand.Next(100, 255), rand.Next(255));

            for (int i = 0; i < 5; i++)
            {
                int y = 200 - i * 30;
                g.DrawLine(new Pen(baseColor, 4), 50 + i * 10, y, 200 - i * 10, y - 20);
            }

            g.FillEllipse(new SolidBrush(baseColor), 108, 108, 40, 40);
        }

        private static Bitmap GenerateAlienIcon(int seed)
        {
            var rand = new Random(seed);
            var bmp = new Bitmap(256, 256);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Black);

            int designType = rand.Next(6);

            switch (designType)
            {
                case 0: DrawTentacleCore(g, rand); break;
                case 1: DrawOrbitalGlyph(g, rand); break;
                case 2: DrawCrystalMatrix(g, rand); break;
                case 3: DrawSwarmCloud(g, rand); break;
                case 4: DrawOrganicNest(g, rand); break;
                case 5: DrawSignalMap(g, rand); break;
            }

            return bmp;
        }

        private static void DrawTentacleCore(Graphics g, Random rand)
        {
            var centerX = 128;
            var centerY = 128;

            // Variable number of tentacles
            int tentacleCount = 6 + rand.Next(5); // 6 to 10
            double baseAngle = rand.NextDouble() * 2 * Math.PI;

            for (int i = 0; i < tentacleCount; i++)
            {
                int points = 5 + rand.Next(4, 9); // 9 to 13
                var path = new List<Point>();
                double angle = baseAngle + i * (2 * Math.PI / tentacleCount) + rand.NextDouble() * 0.4;

                double angleOffset = 0.0;
                double wiggle = 0.1 + rand.NextDouble() * 0.3;

                for (int j = 0; j < points; j++)
                {
                    angleOffset += wiggle * (rand.Next(2) == 0 ? -1 : 1);
                    var r = 20 + j * 15 + rand.Next(10);
                    var x = (int)(centerX + Math.Cos(angle + angleOffset) * r);
                    var y = (int)(centerY + Math.Sin(angle + angleOffset) * r);
                    path.Add(new Point(x, y));
                }

                var tentacleColor = Color.FromArgb(255, rand.Next(50, 120), rand.Next(180, 255), rand.Next(50, 120));
                var pen = new Pen(tentacleColor, 1 + rand.Next(3));
                g.DrawLines(pen, path.ToArray());
            }

            // Alien eye / core with random offset and size
            var eyeColor = Color.FromArgb(255, rand.Next(100, 255), rand.Next(0, 100), rand.Next(100, 255));
            int eyeX = 100 + rand.Next(-15, 15);
            int eyeY = 100 + rand.Next(-15, 15);
            int eyeSize = 40 + rand.Next(20);

            if (rand.Next(2) == 0)
            {
                g.FillEllipse(new SolidBrush(eyeColor), eyeX, eyeY, eyeSize, eyeSize);
            }
            else
            {
                Point[] triangle =
                [
                    new Point(eyeX, eyeY),
                    new Point(eyeX + eyeSize, eyeY + rand.Next(eyeSize)),
                    new Point(eyeX + rand.Next(eyeSize), eyeY + eyeSize)
                ];
                g.FillPolygon(new SolidBrush(eyeColor), triangle);
            }

            // Add sparkles or spots
            for (int i = 0; i < 30 + rand.Next(30); i++)
            {
                int x = rand.Next(256);
                int y = rand.Next(256);
                int size = rand.Next(1, 4);
                var sparkleColor = Color.FromArgb(255, rand.Next(150, 255), rand.Next(100, 255), rand.Next(150, 255));
                g.FillEllipse(new SolidBrush(sparkleColor), x, y, size, size);
            }
        }

        static void DrawOrbitalGlyph(Graphics g, Random rand)
        {
            var centerX = 128;
            var centerY = 128;
            int orbitCount = 3 + rand.Next(4);

            for (int i = 0; i < orbitCount; i++)
            {
                int radius = 30 + i * 20 + rand.Next(10);
                var color = Color.FromArgb(255, rand.Next(100, 255), rand.Next(100, 255), rand.Next(200, 255));
                g.DrawEllipse(new Pen(color, 1 + rand.Next(2)), centerX - radius, centerY - radius, radius * 2, radius * 2);

                // Add small "satellites"
                int satellites = 3 + rand.Next(5);
                for (int j = 0; j < satellites; j++)
                {
                    double angle = rand.NextDouble() * 2 * Math.PI;
                    int x = (int)(centerX + Math.Cos(angle) * radius);
                    int y = (int)(centerY + Math.Sin(angle) * radius);
                    g.FillEllipse(new SolidBrush(color), x - 2, y - 2, 4, 4);
                }
            }
        }

        static void DrawCrystalMatrix(Graphics g, Random rand)
        {
            for (int i = 0; i < 20 + rand.Next(20); i++)
            {
                Point p1 = new(rand.Next(256), rand.Next(256));
                Point p2 = new(p1.X + rand.Next(-40, 40), p1.Y + rand.Next(-40, 40));
                Point p3 = new(p1.X + rand.Next(-40, 40), p1.Y + rand.Next(-40, 40));
                var brush = new SolidBrush(Color.FromArgb(80 + rand.Next(150), rand.Next(150, 255), rand.Next(150, 255), rand.Next(150, 255)));
                g.FillPolygon(brush, [p1, p2, p3]);
            }
        }

        static void DrawOrganicNest(Graphics g, Random rand)
        {
            for (int i = 0; i < 8 + rand.Next(6); i++)
            {
                var path = new List<PointF>();
                float x = rand.Next(256);
                float y = rand.Next(256);
                for (int j = 0; j < 5; j++)
                {
                    x += rand.Next(-30, 30);
                    y += rand.Next(-30, 30);
                    path.Add(new PointF(x, y));
                }

                var brush = new SolidBrush(Color.FromArgb(100 + rand.Next(100), rand.Next(255), rand.Next(150), rand.Next(150)));
                using var graphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                graphicsPath.AddCurve(path.ToArray());
                g.FillPath(brush, graphicsPath);
            }
        }

        static void DrawSignalMap(Graphics g, Random rand)
        {
            int lines = 10 + rand.Next(10);
            for (int i = 0; i < lines; i++)
            {
                int y = 20 + i * 20 + rand.Next(-5, 5);
                int pulses = 4 + rand.Next(6);
                for (int j = 0; j < pulses; j++)
                {
                    int startX = rand.Next(0, 200);
                    int width = rand.Next(20, 40);
                    var color = Color.FromArgb(255, rand.Next(100, 255), rand.Next(100), rand.Next(255));
                    g.DrawLine(new Pen(color, 2), startX, y, startX + width, y);
                }
            }
        }

        static void DrawSwarmCloud(Graphics g, Random rand)
        {
            var centerX = 128;
            var centerY = 128;
            for (int i = 0; i < 300 + rand.Next(200); i++)
            {
                double angle = rand.NextDouble() * 2 * Math.PI;
                double radius = 10 + rand.NextDouble() * 120;
                double spiral = radius + Math.Sin(radius / 10.0) * 10;
                int x = (int)(centerX + Math.Cos(angle) * spiral);
                int y = (int)(centerY + Math.Sin(angle) * spiral);

                int size = rand.Next(1, 3);
                var color = Color.FromArgb(255, rand.Next(200, 255), rand.Next(200), rand.Next(255));
                g.FillEllipse(new SolidBrush(color), x, y, size, size);
            }
        }
    }
}
