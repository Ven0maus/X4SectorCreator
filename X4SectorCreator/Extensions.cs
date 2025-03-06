using System.Numerics;
using X4SectorCreator.Configuration;

namespace X4SectorCreator
{
    internal static class Extensions
    {
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
            if (string.IsNullOrEmpty(oldValueTrimmed) && string.IsNullOrEmpty(newValueTrimmed))
            {
                return false;
            }

            return oldValueTrimmed != newValueTrimmed;
        }

        public static CustomVector ToEulerAngles(this Quaternion q)
        {
            Vector3 angles = new();

            // Roll (X-axis rotation)
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.X = (float)(Math.Atan2(sinr_cosp, cosr_cosp) * (180 / Math.PI)); // Convert to degrees

            // Pitch (Y-axis rotation)
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
                angles.Y = (float)Math.CopySign(90, sinp); // Clamp to ±90°
            else
                angles.Y = (float)(Math.Asin(sinp) * (180 / Math.PI)); // Convert to degrees

            // Yaw (Z-axis rotation)
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Z = (float)(Math.Atan2(siny_cosp, cosy_cosp) * (180 / Math.PI)); // Convert to degrees

            // Ensure all angles are in range [0, 360]
            angles.X = (angles.X + 360) % 360;
            angles.Y = (angles.Y + 360) % 360;
            angles.Z = (angles.Z + 360) % 360;

            return new CustomVector((int)Math.Round(angles.X), (int)Math.Round(angles.Y), (int)Math.Round(angles.Z));
        }
    }
}
