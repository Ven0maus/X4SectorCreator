﻿using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using X4SectorCreator.Configuration;
using X4SectorCreator.CustomComponents;

namespace X4SectorCreator.Helpers
{
    internal static class Extensions
    {
        private static readonly Dictionary<TextBox, TextSearchComponent> _textSearchComponents = [];
        public static void EnableTextSearch<T>(this TextBox textBox, List<T> items, Func<T, string> filterCriteriaSelector, Action<List<T>> onFiltered, int debounceDelayMilliseconds = 500)
        {
            if (_textSearchComponents.ContainsKey(textBox)) return;
            _textSearchComponents[textBox] = new TextSearchComponent<T>(textBox, items, filterCriteriaSelector, onFiltered, debounceDelayMilliseconds);
        }

        public static void EnableTextSearch<T>(this TextBox textBox, Func<List<T>> itemGetter, Func<T, string> filterCriteriaSelector, Action<List<T>> onFiltered, int debounceDelayMilliseconds = 500)
        {
            if (_textSearchComponents.ContainsKey(textBox)) return;
            _textSearchComponents[textBox] = new TextSearchComponent<T>(textBox, itemGetter, filterCriteriaSelector, onFiltered, debounceDelayMilliseconds);
        }

        public static void DisableTextSearch(this TextBox textBox)
        {
            if (!_textSearchComponents.TryGetValue(textBox, out var component)) return;
            component.Dispose();
            _textSearchComponents.Remove(textBox);
        }

        public static TextSearchComponent GetTextSearchComponent(this TextBox textBox)
        {
            if (_textSearchComponents.TryGetValue(textBox, out var component))
                return component;
            return null;
        }

        public static Image Resize(this Image source, int width, int height, InterpolationMode mode, Color? tintColor = null)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = mode;
                graphics.SmoothingMode = SmoothingMode.None; // For sharper result at small sizes
                graphics.PixelOffsetMode = PixelOffsetMode.Half; // Sharper placement of pixels

                using var attributes = new ImageAttributes();

                if (tintColor.HasValue)
                {
                    attributes.SetColorMatrix(tintColor.Value.ToColorMatrix());
                }

                graphics.DrawImage(
                    source,
                    destRect,
                    0,
                    0,
                    source.Width,
                    source.Height,
                    GraphicsUnit.Pixel,
                    attributes);
            }

            return destImage;
        }

        public static ColorMatrix ToColorMatrix(this Color color)
        {
            // Normalize the RGB components of the color (values between 0 and 1)
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            float a = color.A / 255f;

            // Create a ColorMatrix and apply the color's tint
            ColorMatrix matrix = new(
            [
                [r, 0, 0, 0, 0],  // Red component
                [0, g, 0, 0, 0],  // Green component
                [0, 0, b, 0, 0],  // Blue component
                [0, 0, 0, a, 0],  // Alpha component
                [0, 0, 0, 0, 1]   // No change to the final pixel intensity
            ]);

            return matrix;
        }

        public static Image CopyAsTint(this Image image, Color tintColor)
        {
            Bitmap tintedImage = new(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(tintedImage))
            {
                ColorMatrix colorMatrix = tintColor.ToColorMatrix();

                using ImageAttributes attributes = new();
                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(
                    image,
                    new Rectangle(0, 0, image.Width, image.Height),
                    0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel,
                    attributes
                );
            }

            return tintedImage;
        }

        public static Color HexToColor(this string hexstring)
        {
            // Remove '#' if present
            if (hexstring.StartsWith('#'))
            {
                hexstring = hexstring[1..];
            }

            // Convert hex to RGB
            if (hexstring.Length == 6)
            {
                int r = Convert.ToInt32(hexstring[..2], 16);
                int g = Convert.ToInt32(hexstring.Substring(2, 2), 16);
                int b = Convert.ToInt32(hexstring.Substring(4, 2), 16);
                return Color.FromArgb(r, g, b);
            }
            else if (hexstring.Length == 8) // If it includes alpha (ARGB)
            {
                int a = Convert.ToInt32(hexstring[..2], 16);
                int r = Convert.ToInt32(hexstring.Substring(2, 2), 16);
                int g = Convert.ToInt32(hexstring.Substring(4, 2), 16);
                int b = Convert.ToInt32(hexstring.Substring(6, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
            else
            {
                throw new ArgumentException($"Parsing error: \"{hexstring}\" is an invalid hex color format.");
            }
        }

        /// <summary>
        /// Removes duplicate highway connections from connections enumerable.
        /// </summary>
        /// <param name="connections"></param>
        /// <returns></returns>
        public static IEnumerable<SectorMapForm.GateConnection> FilterDuplicateHighwayConnections(this IEnumerable<SectorMapForm.GateConnection> connections)
        {
            HashSet<SectorMapForm.GateConnection> allConnections = connections
                .ToHashSet();
            List<SectorMapForm.GateConnection> highways = connections
                .Where(a => a.Source.Gate.IsHighwayGate || a.Target.Gate.IsHighwayGate)
                .ToList();

            HashSet<(string, string)> processedHighways = [];
            foreach (SectorMapForm.GateConnection highway in highways)
            {
                if (processedHighways.Contains((highway.Source.Gate.ParentSectorName, highway.Target.Gate.ParentSectorName)) ||
                    processedHighways.Contains((highway.Target.Gate.ParentSectorName, highway.Source.Gate.ParentSectorName)))
                {
                    _ = allConnections.Remove(highway);
                }
                _ = processedHighways.Add((highway.Source.Gate.ParentSectorName, highway.Target.Gate.ParentSectorName));
            }

            return allConnections;
        }

        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.Left + (rect.Width / 2), rect.Top + (rect.Height / 2));
        }

        public static string CapitalizeFirstLetter(this string input)
        {
            return string.IsNullOrEmpty(input) ? input : char.ToUpper(input[0]) + input[1..];
        }

        public static bool HasStringChanged(string old, string @new)
        {
            string oldValueTrimmed = old?.Trim();
            string newValueTrimmed = @new?.Trim();

            // Treat null and empty as the same (no change)
            return (!string.IsNullOrEmpty(oldValueTrimmed) || !string.IsNullOrEmpty(newValueTrimmed)) && oldValueTrimmed != newValueTrimmed;
        }

        public static CustomVector ToEulerAngles(this Quaternion q)
        {
            Vector3 angles = new();

            // roll / x
            double sinr_cosp = 2 * ((q.W * q.X) + (q.Y * q.Z));
            double cosr_cosp = 1 - (2 * ((q.X * q.X) + (q.Y * q.Y)));
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch / y
            double sinp = 2 * ((q.W * q.Y) - (q.Z * q.X));
            angles.Y = Math.Abs(sinp) >= 1 ? (float)Math.CopySign(Math.PI / 2, sinp) : (float)Math.Asin(sinp);

            // yaw / z
            double siny_cosp = 2 * ((q.W * q.Z) + (q.X * q.Y));
            double cosy_cosp = 1 - (2 * ((q.Y * q.Y) + (q.Z * q.Z)));
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return new CustomVector((int)angles.X, (int)angles.Y, (int)angles.Z);
        }
    }
}
