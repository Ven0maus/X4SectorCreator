using System.Globalization;

namespace X4SectorCreator.Objects
{
    public class SplineTube
    {
        public float Radius { get; set; }
        public List<SplinePosition> Positions { get; set; } = [];
    }

    public class SplinePosition
    {
        public float X { get; set; }  // 2D X (maps to 3D X)
        public float Y { get; set; }  // 2D Y (maps to 3D Z)

        // Interpolation Data
        public float TX { get; set; } = 0;  // Tangent X
        public float TY { get; set; } = 1;  // Tangent Y (normal default)
        public float TZ { get; set; } = 0;  // Tangent Z
        public float InLength { get; set; } = 0;
        public float OutLength { get; set; } = 100;  // Default segment length

        // Convert 2D to 3D format
        public string To3DFormat()
        {
            return $"<splineposition x=\"{X.ToString(CultureInfo.InvariantCulture)}\" " +
                   $"y=\"0.0\" z=\"{Y.ToString(CultureInfo.InvariantCulture)}\" " +
                   $"tx=\"{TX.ToString(CultureInfo.InvariantCulture)}\" " +
                   $"ty=\"{TY.ToString(CultureInfo.InvariantCulture)}\" " +
                   $"tz=\"{TZ.ToString(CultureInfo.InvariantCulture)}\" " +
                   $"inlength=\"{InLength.ToString(CultureInfo.InvariantCulture)}\" " +
                   $"outlength=\"{OutLength.ToString(CultureInfo.InvariantCulture)}\" />";
        }

        public override string ToString()
        {
            return $"({X}, {Y}) - TX:{TX}, TY:{TY}, In:{InLength}, Out:{OutLength}";
        }
    }
}
