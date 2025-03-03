using System.ComponentModel;
using System.Globalization;
using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class SplineTubeEditorForm : Form
    {
        private SplineTube _splineTube = new SplineTube();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SplineTube SplineTube
        {
            get => _splineTube;
            set
            {
                _splineTube = value;
                InitSplineTube();
            }
        }

        private int _selectedPointIndex = -1; // Track selected point
        private bool _isDragging = false; // Track dragging state
        private bool _adjustingTangent = false;

        public SplineTubeEditorForm()
        {
            InitializeComponent();

            SplineTubeRenderer.Paint += SplineTubeRenderer_Paint;
            SplineTubeRenderer.MouseDown += SplineTubeRenderer_MouseDown;
            SplineTubeRenderer.MouseMove += SplineTubeRenderer_MouseMove;
            SplineTubeRenderer.MouseUp += SplineTubeRenderer_MouseUp;
        }

        private void SplineTubeRenderer_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
            _selectedPointIndex = -1;
            _adjustingTangent = false;
        }

        private void SplineTubeRenderer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _selectedPointIndex != -1)
            {
                SplinePosition selectedPos = SplineTube.Positions[_selectedPointIndex];

                if (_adjustingTangent)
                {
                    // Adjust the tangent (direction vector)
                    float dx = e.X - selectedPos.X;
                    float dy = e.Y - selectedPos.Y;

                    float length = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (length > 0)
                    {
                        selectedPos.TX = dx / length;
                        selectedPos.TY = dy / length;
                        selectedPos.OutLength = length;
                    }
                }
                else
                {
                    // Move the control point normally
                    selectedPos.X = e.X;
                    selectedPos.Y = e.Y;
                }

                ListBoxSplinePositions.Items[_selectedPointIndex] = selectedPos;
                SplineTubeRenderer.Invalidate(); // Redraw the curve
            }
        }

        private void SplineTubeRenderer_MouseDown(object sender, MouseEventArgs e)
        {
            // First, check if the user clicked on a control point
            for (int i = 0; i < SplineTube.Positions.Count; i++)
            {
                SplinePosition p = SplineTube.Positions[i];

                // Check if click is near a control point
                if (Math.Abs(e.X - p.X) <= 5 && Math.Abs(e.Y - p.Y) <= 5)
                {
                    _selectedPointIndex = i;
                    _isDragging = true;
                    return;
                }
            }

            // If no point was selected, check if the user clicked near the spline
            for (int i = 0; i < SplineTube.Positions.Count - 1; i++)
            {
                SplinePosition p0 = SplineTube.Positions[i];
                SplinePosition p1 = SplineTube.Positions[i + 1];

                // Check if click is near an interpolated point on the spline
                for (float t = 0; t <= 1; t += 0.05f)
                {
                    PointF interpolated = HermiteInterpolation(p0, p1, t);

                    if (Math.Abs(e.X - interpolated.X) <= 5 && Math.Abs(e.Y - interpolated.Y) <= 5)
                    {
                        _selectedPointIndex = i;
                        _isDragging = true;
                        _adjustingTangent = true;  // Enable tangent adjustment mode
                        return;
                    }
                }
            }
        }


        private void InitSplineTube()
        {

        }

        private void AddSplinePoint(float x, float y)
        {
            var pos = new SplinePosition
            {
                X = x,
                Y = y,
                TX = 0,  // Default tangent in X direction
                TY = 0,
                TZ = 0,
                InLength = 50,
                OutLength = 50
            };

            _splineTube.Positions.Add(pos);
            ListBoxSplinePositions.Items.Add(pos);
            SplineTubeRenderer.Invalidate();
        }

        private void SplineTubeRenderer_Paint(object sender, PaintEventArgs e)
        {
            DrawSpline(e.Graphics);
        }

        private void BtnAddPoint_Click(object sender, EventArgs e)
        {
            AddSplinePoint(SplineTubeRenderer.Width / 2f, SplineTubeRenderer.Height / 2f);
        }

        private void BtnRemovePoint_Click(object sender, EventArgs e)
        {
            var selectedPosition = ListBoxSplinePositions.SelectedItem as SplinePosition;
            if (selectedPosition != null)
            {
                int index = ListBoxSplinePositions.Items.IndexOf(selectedPosition);
                SplineTube.Positions.Remove(selectedPosition);
                ListBoxSplinePositions.Items.Remove(selectedPosition);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ListBoxSplinePositions.SelectedItem = index >= 0 && ListBoxSplinePositions.Items.Count > 0 ? ListBoxSplinePositions.Items[index] : null;
                SplineTubeRenderer.Invalidate();
            }
        }

        private void DrawSpline(Graphics g)
        {
            g.Clear(Color.White);
            if (SplineTube.Positions.Count < 2) return;

            using Pen splinePen = new Pen(Color.Blue, 2);
            using Pen tangentPen = new Pen(Color.Green, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            Brush pointBrush = Brushes.Red;
            Brush handleBrush = Brushes.Green;

            // Draw smooth Hermite spline
            for (int i = 0; i < SplineTube.Positions.Count - 1; i++)
            {
                SplinePosition p0 = SplineTube.Positions[i];
                SplinePosition p1 = SplineTube.Positions[i + 1];

                PointF lastPoint = new PointF(p0.X, p0.Y);
                for (float t = 0; t <= 1; t += 0.05f)
                {
                    PointF interpolated = HermiteInterpolation(p0, p1, t);
                    g.DrawLine(splinePen, lastPoint, interpolated);
                    lastPoint = interpolated;
                }
            }

            // Draw control points and tangent handles
            foreach (var pos in SplineTube.Positions)
            {
                g.FillEllipse(pointBrush, pos.X - 5, pos.Y - 5, 10, 10);

                // Tangent visualization
                PointF tangentEnd = new PointF(pos.X + pos.TX * pos.OutLength, pos.Y + pos.TY * pos.OutLength);
                g.DrawLine(tangentPen, new PointF(pos.X, pos.Y), tangentEnd);
                g.FillEllipse(handleBrush, tangentEnd.X - 3, tangentEnd.Y - 3, 6, 6);
            }
        }


        private static PointF HermiteInterpolation(SplinePosition p0, SplinePosition p1, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            // Hermite basis functions
            float h00 = 2 * t3 - 3 * t2 + 1;
            float h10 = t3 - 2 * t2 + t;
            float h01 = -2 * t3 + 3 * t2;
            float h11 = t3 - t2;

            // Tangents scaled by inlength/outlength
            float tangent0X = p0.TX * p0.OutLength;
            float tangent0Y = p0.TY * p0.OutLength;

            float tangent1X = p1.TX * p1.InLength;
            float tangent1Y = p1.TY * p1.InLength;

            // Compute interpolated position
            float x = h00 * p0.X + h10 * tangent0X + h01 * p1.X + h11 * tangent1X;
            float y = h00 * p0.Y + h10 * tangent0Y + h01 * p1.Y + h11 * tangent1Y;

            return new PointF(x, y);
        }


        private void BtnImportSplineTube_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Select a Spline XML File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SplineTube.Positions.Clear();
                ListBoxSplinePositions.Items.Clear();

                string filePath = openFileDialog.FileName;

                XDocument xmlDoc = XDocument.Load(filePath);
                var positions = xmlDoc.Descendants("splineposition");

                // Extract X and Z values for front view (XZ Projection)
                var xValues = positions.Select(p => float.Parse(p.Attribute("x").Value));
                var zValues = positions.Select(p => float.Parse(p.Attribute("z").Value));

                float minX = xValues.Min(), maxX = xValues.Max();
                float minZ = zValues.Min(), maxZ = zValues.Max();

                float rangeX = maxX - minX;
                float rangeZ = maxZ - minZ;

                // Determine scale factor to fit within 400x400
                float maxRange = Math.Max(rangeX, rangeZ);
                float scale = 400.0f / maxRange;

                foreach (var pos in positions)
                {
                    float x = float.Parse(pos.Attribute("x").Value);
                    float z = float.Parse(pos.Attribute("z").Value);
                    float tx = float.Parse(pos.Attribute("tx").Value);
                    float ty = float.Parse(pos.Attribute("ty").Value);
                    float tz = float.Parse(pos.Attribute("tz").Value);
                    float inLength = float.Parse(pos.Attribute("inlength").Value);
                    float outLength = float.Parse(pos.Attribute("outlength").Value);

                    // Normalize and scale
                    float scaledX = (x - minX) * scale;
                    float scaledZ = (z - minZ) * scale; // Z becomes Y in 2D

                    // Store in SplinePosition class
                    var splinePos = new SplinePosition
                    {
                        X = scaledX,
                        Y = scaledZ,
                        TX = tx,
                        TY = ty,
                        TZ = tz,
                        InLength = inLength,
                        OutLength = outLength
                    };
                    SplineTube.Positions.Add(splinePos);
                }

                ScaleSplinePositionsToFitBox(SplineTube.Positions, SplineTubeRenderer.Width, SplineTubeRenderer.Height);
                foreach (var pos in SplineTube.Positions)
                    ListBoxSplinePositions.Items.Add(pos);
                SplineTubeRenderer.Invalidate();
            }
        }

        private void BtnExportSplineTube_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Save Spline XML File"
            };

            string filePath = saveFileDialog.FileName;

            XElement root = new XElement("boundary",
                new XAttribute("class", "splinetube"),
                new XElement("size", new XAttribute("r", "5000")) // Preserve size if needed
            );

            foreach (var pos in SplineTube.Positions)
            {
                XElement splinePos = new XElement("splineposition",
                    new XAttribute("x", pos.X.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("y", "0.0"), // Assuming original Y was 0 for 2D
                    new XAttribute("z", pos.Y.ToString(CultureInfo.InvariantCulture)), // Convert back to Z
                    new XAttribute("tx", pos.TX.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("ty", "0.0"), // Assuming original TY was not needed in 2D
                    new XAttribute("tz", pos.TY.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("inlength", pos.InLength.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("outlength", pos.OutLength.ToString(CultureInfo.InvariantCulture))
                );

                root.Add(splinePos);
            }

            XDocument xmlDoc = new XDocument(root);
            xmlDoc.Save(filePath);
        }

        static void ScaleSplinePositionsToFitBox(List<SplinePosition> positions, float boxWidth, float boxHeight)
        {
            if (positions == null || positions.Count == 0)
                return;

            // Find bounding box
            float minX = positions.Min(p => p.X);
            float maxX = positions.Max(p => p.X);
            float minY = positions.Min(p => p.Y);
            float maxY = positions.Max(p => p.Y);

            float rangeX = maxX - minX;
            float rangeY = maxY - minY;

            // Avoid division by zero
            float scaleX = (rangeX > 0) ? (boxWidth / rangeX) : 1;
            float scaleY = (rangeY > 0) ? (boxHeight / rangeY) : 1;
            float scale = Math.Min(scaleX, scaleY);  // Uniform scaling

            // Compute centering offset
            float offsetX = (boxWidth - (rangeX * scale)) / 2f;
            float offsetY = (boxHeight - (rangeY * scale)) / 2f;

            for (int i = 0; i < positions.Count; i++)
            {
                // Scale and translate position
                positions[i].X = ((positions[i].X - minX) * scale) + offsetX;
                positions[i].Y = boxHeight - (((positions[i].Y - minY) * scale) + offsetY); // Flip Y-axis if needed

                // Apply the same scale factor to the in/out tangents
                positions[i].InLength *= scale;
                positions[i].OutLength *= scale;

                // Optionally, clamp to prevent extreme values (adjust if needed)
                float maxTangentLength = Math.Max(boxWidth, boxHeight) * 0.1f; // 20% of the largest box dimension
                positions[i].InLength = Math.Min(positions[i].InLength, maxTangentLength);
                positions[i].OutLength = Math.Min(positions[i].OutLength, maxTangentLength);
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {

        }
    }
}
