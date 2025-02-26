using System.Text.Json;
using X4SectorCreator.Objects;
using System.ComponentModel;

namespace X4SectorCreator
{
    public partial class SectorMapForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool GateSectorSelection { get; set; } = false;

        private readonly Dictionary<(int, int), Hexagon> _hexagons = [];
        private readonly Dictionary<(int, int), Cluster> _clusterMapping;
        private readonly Dictionary<string, Color> _colorMapping;
        private readonly int _hexSize = 100;

        // How many extra rows and cols will be "open" around the base game sectors + custom sectors for the user to select
        private const int _minExpansionRoom = 20; 
        private int _cols, _rows;
        private PointF _offset;
        private bool _dragging = false;
        private Point _lastMousePos, _mouseDownPos;
        private int? _selectedChildHexIndex, _previousSelectedChildHexIndex;
        private (int, int)? _selectedHex, _previousSelectedHex;
        private float _zoom = 1.2f; // 1.0 means 100% scale

        public SectorMapForm()
        {
            InitializeComponent();

            // Setup events
            DoubleBuffered = true;
            MouseDown += HandleMouseDown;
            MouseUp += HandleMouseUp;
            MouseMove += HandleMouseMove;
            Paint += DrawHexGrid;
            Resize += HandleResize;
            MouseWheel += HandleMouseWheel;

            // Initializes the full X4 map from JSON data
            const string filePath = "Mappings/sector_mappings.json";

            var json = File.ReadAllText(filePath);
            var clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(json);

            // Create lookups
            _clusterMapping = clusterCollection.Clusters.ToDictionary(a => (a.Position.X, a.Position.Y));
            _colorMapping = clusterCollection.FactionColors.ToDictionary(a => a.Key, a => HexToColor(a.Value), StringComparer.OrdinalIgnoreCase);
        }

        public void Reset()
        {
            _zoom = 1.2f;
            _offset = new PointF(0, 0);

            var clusters = _clusterMapping.Values
                .Concat(MainForm.Instance.CustomClusters.Values)
                .DefaultIfEmpty()
                .ToArray();

            // Determine size of hex grid based on cluster mapping + custom sector
            _cols = (Math.Max(Math.Abs(clusters.Max(a => a.Position.X)), Math.Abs(clusters.Min(a => a.Position.X))) + _minExpansionRoom) * 2 + 1;
            _rows = ((int)((Math.Max(Math.Abs(clusters.Max(b => b.Position.Y)), Math.Abs(clusters.Min(b => b.Position.Y))) + _minExpansionRoom / 2) * 1.5f)) + 1;

            GenerateHexagons();
            Invalidate();
        }

        private void HandleMouseWheel(object sender, MouseEventArgs e)
        {
            const float zoomFactor = 1.1f; // 10% zoom per wheel step
            float oldZoom = _zoom;

            if (e.Delta > 0)
                _zoom *= zoomFactor; // Zoom in
            else
                _zoom /= zoomFactor; // Zoom out

            _zoom = Math.Clamp(_zoom, 0.25f, 2.0f); // Limit zoom between 50% and 200%

            // Convert mouse position to world coordinates before zoom
            float worldXBefore = (e.X - _offset.X) / oldZoom;
            float worldYBefore = (e.Y - _offset.Y) / oldZoom;

            // Convert mouse position to world coordinates after zoom
            float worldXAfter = (e.X - _offset.X) / _zoom;
            float worldYAfter = (e.Y - _offset.Y) / _zoom;

            // Adjust offset to keep the zoom centered at the cursor
            _offset.X += (worldXAfter - worldXBefore) * _zoom;
            _offset.Y += (worldYAfter - worldYBefore) * _zoom;

            Invalidate(); // Redraw
        }

        private void HandleResize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                // Recalculate the offset to keep (0,0) in the center
                if (_hexagons.TryGetValue((0, 0), out Hexagon zeroHex))
                {
                    var center = GetHexCenter(zeroHex.Points);
                    _offset = new PointF(ClientSize.Width / 2 - center.X, ClientSize.Height / 2 - center.Y);
                }

                Invalidate(); // Force redraw
            }
        }

        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _mouseDownPos = e.Location; // Store initial position
            _lastMousePos = e.Location;
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging && e.Button == MouseButtons.Left)
            {
                _offset.X += e.X - _lastMousePos.X;
                _offset.Y += e.Y - _lastMousePos.Y;
                _lastMousePos = e.Location;
                Invalidate();
            }
        }

        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;

            // Calculate total movement distance
            int dx = Math.Abs(e.Location.X - _mouseDownPos.X);
            int dy = Math.Abs(e.Location.Y - _mouseDownPos.Y);
            int movementThreshold = 5;

            if (dx <= movementThreshold && dy <= movementThreshold)
            {
                // Click detected (not a drag), check for hex selection
                PointF adjustedMousePos = new(
                    (e.Location.X - _offset.X) / _zoom,
                    (e.Location.Y - _offset.Y) / _zoom
                );

                foreach (var hex in _hexagons)
                {
                    if (GateSectorSelection)
                    {
                        // Allow selecting child hex too
                        if (hex.Value.Children != null)
                        {
                            int index = 0;
                            foreach (var child in hex.Value.Children)
                            {
                                if (IsPointInPolygon(child.Points, adjustedMousePos))
                                {
                                    if (hex.Key == _selectedHex && _selectedChildHexIndex == index)
                                        DeselectHex();
                                    else
                                        SelectHex(hex.Key, index);
                                    return;
                                }
                                index++;
                            }
                        }
                    }

                    // Check main hex
                    if (IsPointInPolygon(hex.Value.Points, adjustedMousePos))
                    {
                        if (hex.Key == _selectedHex)
                            DeselectHex();
                        else
                            SelectHex(hex.Key);
                        return;
                    }
                }

                DeselectHex();
            }
        }

        private void SelectHex((int, int) pos, int? childIndex = null)
        {
            if (!BtnSelectLocation.Visible) return;
            if (_previousSelectedHex != _selectedHex || _previousSelectedChildHexIndex != _selectedChildHexIndex || _selectedHex == null)
            {
                _previousSelectedHex = _selectedHex;
                _previousSelectedChildHexIndex = childIndex;
                _selectedChildHexIndex = childIndex;
                _selectedHex = pos;
                BtnSelectLocation.Enabled = true;
                Invalidate();
            }
        }

        private void DeselectHex()
        {
            if (_selectedHex != null)
            {
                _selectedHex = null;
                _selectedChildHexIndex = null;
                BtnSelectLocation.Enabled = false;
                Invalidate();
            }
        }

        private static (int x, int y) TranslateCoordinate(int q, int r)
        {
            return (q, -(r * 2 + (q & 1)));
        }

        private void GenerateHexagons()
        {
            _hexagons.Clear();

            float hexHeight = (float)(Math.Sqrt(3) * _hexSize); // Height for flat-top hexes
            var halfRow = _rows / 2;
            var halfCol = _cols / 2;

            for (int r = -halfRow; r <= halfRow; r++)
            {
                for (int q = -halfCol; q <= halfCol; q++)
                {
                    var translatedCoordinate = TranslateCoordinate(q, r);

                    // Define how many sectors should be in this cluster
                    int children = 1;
                    if (_clusterMapping.TryGetValue(translatedCoordinate, out var cluster) ||
                        MainForm.Instance.CustomClusters.TryGetValue(translatedCoordinate, out cluster))
                        children = cluster.Sectors.Count;

                    // Determine hex information
                    var hex = GenerateHexagonWithChildren(hexHeight, r, q, ClientSize.Width / 2, ClientSize.Height / 2, _zoom, children);
                    _hexagons[translatedCoordinate] = hex;
                }
            }
        }

        private static Hexagon GenerateHexagonWithChildren(float height, int row, int col, float centerX, float centerY, float zoom = 1.0f, int children = 1)
        {
            // Scale parent hex
            height *= zoom;
            float width = (float)(4 * (height / 2 / Math.Sqrt(3))); // Compute width based on height

            // Positioning parent hex
            float xOffset = col * (width * 0.75f);
            float yOffset = row * height;
            if (col % 2 != 0) yOffset += height / 2;

            xOffset += centerX;
            yOffset += centerY;

            // Generate the parent hexagon
            PointF[] parentHex =
            [
                new PointF(xOffset, yOffset),
                new PointF(xOffset + width * 0.25f, yOffset - height / 2),
                new PointF(xOffset + width * 0.75f, yOffset - height / 2),
                new PointF(xOffset + width, yOffset),
                new PointF(xOffset + width * 0.75f, yOffset + height / 2),
                new PointF(xOffset + width * 0.25f, yOffset + height / 2),
            ];

            // Child hexes are 50% of parent size
            float childHeight = height / 2;
            float childWidth = width / 2;
            List<PointF[]> childHexes = [];

            // Child hex positions (equally spaced inside parent)

            PointF[] childCenters;
            if (children == 2)
            {
                // Child hex centers for top-left, bottom-right
                childCenters =
                [
                    new PointF(xOffset + width * 0.125f, yOffset - childHeight * 0.5f), // Top-left
                    new PointF(xOffset + width * 0.375f, yOffset + childHeight * 0.5f), // Bottom-right
                ];
            }
            else if (children == 3)
            {
                // Child hex centers for top-right, bottom-right, and middle-left
                childCenters =
                [
                    new PointF(xOffset + width * 0.375f, yOffset - childHeight * 0.5f), // Top-left
                    new PointF(xOffset + width * 0.375f, yOffset + childHeight * 0.5f), // Bottom-right
                    new PointF(xOffset, yOffset) // Middle-left
                ];
            }
            else
            {
                // No children, if its other than 2 or 3
                childCenters = [];
            }

            foreach (var childCenter in childCenters)
            {
                childHexes.Add(
                [
                    new PointF(childCenter.X, childCenter.Y),
                    new PointF(childCenter.X + childWidth * 0.25f, childCenter.Y - childHeight / 2),
                    new PointF(childCenter.X + childWidth * 0.75f, childCenter.Y - childHeight / 2),
                    new PointF(childCenter.X + childWidth, childCenter.Y),
                    new PointF(childCenter.X + childWidth * 0.75f, childCenter.Y + childHeight / 2),
                    new PointF(childCenter.X + childWidth * 0.25f, childCenter.Y + childHeight / 2),
                ]);
            }

            return new Hexagon(parentHex, childHexes.Select(a => new Hexagon(a, null)).ToList());
        }

        private void DrawHexGrid(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            e.Graphics.TranslateTransform(_offset.X, _offset.Y);
            e.Graphics.ScaleTransform(_zoom, _zoom);

            // First step render non existant hexagons
            var nonExistantHexColor = HexToColor("#121212");
            foreach (var hex in _hexagons)
                RenderNonSectorGrid(e, nonExistantHexColor, hex);

            if (chkShowX4Sectors.Checked)
            {
                // Next step render the game clusters on top
                foreach (var cluster in _clusterMapping.Values)
                    RenderClusters(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon), _clusterMapping);

                if (chkShowCustomSectors.Checked)
                {
                    // Next step render the custom clusters
                    foreach (var cluster in MainForm.Instance.CustomClusters.Values)
                    {
                        // Always overwrite the hexagon as it can change between sessions
                        cluster.Hexagon = _hexagons[(cluster.Position.X, cluster.Position.Y)];
                        RenderClusters(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon), MainForm.Instance.CustomClusters);
                    }

                    // Next step render names
                    foreach (var cluster in MainForm.Instance.CustomClusters.Values)
                        RenderHexNames(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon), MainForm.Instance.CustomClusters);
                }

                // Next step render names
                foreach (var cluster in _clusterMapping.Values)
                    RenderHexNames(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon), _clusterMapping);
            }

            // Highlight selected hex
            if (_selectedHex != null)
            {
                using SolidBrush brush = new(Color.Cyan);
                var hexc = _hexagons[_selectedHex.Value];
                if (_selectedChildHexIndex != null)
                {
                    hexc = hexc.Children[_selectedChildHexIndex.Value];
                }
                e.Graphics.FillPolygon(brush, hexc.Points);
            }
        }

        private void RenderNonSectorGrid(PaintEventArgs e, Color nonExistantHexColor, KeyValuePair<(int, int), Hexagon> hex)
        {
            // Render each non-existant hex first
            if (!_clusterMapping.TryGetValue(hex.Key, out var cluster) || !chkShowX4Sectors.Checked)
            {
                using SolidBrush mainBrush = new(Color.Black);
                using Pen mainPen = new(nonExistantHexColor, 2);
                // Fill hex
                e.Graphics.FillPolygon(mainBrush, hex.Value.Points);
                // Draw edges
                e.Graphics.DrawPolygon(mainPen, hex.Value.Points);

                SizeF textSize;
                if (chkShowCoordinates.Checked)
                {
                    var hexCenter = GetHexCenter(hex.Value.Points);
                    var hexSize = GetHexSize(hex.Value.Points);

                    using var fBold = new Font(Font, FontStyle.Bold);
                    (int x, int y) = hex.Key;
                    string coordText = $"({x}, {y})";
                    textSize = e.Graphics.MeasureString(coordText, fBold);
                    e.Graphics.DrawString(coordText, fBold, Brushes.White,
                        hexCenter.X - hexSize.Width * 0.25f,            // Align to the left
                        hexCenter.Y + hexSize.Height / 2 - textSize.Height); // Align to the bottom
                }
            }
            else
            {
                // Set for later
                cluster.Hexagon = hex.Value;
            }
        }

        private void RenderClusters(PaintEventArgs e, KeyValuePair<(int, int), Hexagon> hex, Dictionary<(int, int), Cluster> lookupTable)
        {
            var cluster = lookupTable[hex.Key];
            if (cluster.Sectors.Count == 0) return;

            // If all sectors are the "same" owner, then the main hex becomes that color
            // If not the main hex is considered "unclaimed" until all sectors within are owned by the same faction
            Color color = _colorMapping["None"];
            var firstSector = cluster.Sectors.FirstOrDefault();
            if (firstSector != null)
            {
                string owner = firstSector.Owner;
                if (cluster.Sectors.All(a => a.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase)))
                {
                    color = !_colorMapping.TryGetValue(owner, out Color value) ? _colorMapping["None"] : value;
                }
            }

            // Main hex outline
            int index = 0;
            using (Pen mainPen = new(color, 2))
            {
                if (cluster.Sectors.Count > 1)
                    color = Color.Black;

                // Fill with darker color
                using SolidBrush mainBrush = new(LerpColor(color, Color.Black, 0.85f));
                e.Graphics.FillPolygon(mainBrush, hex.Value.Points);

                // Draw child hex outlines
                foreach (var child in hex.Value.Children)
                {
                    var sector = cluster.Sectors[index];
                    var ownerColor = !_colorMapping.TryGetValue(sector.Owner, out Color value) ? _colorMapping["None"] : value;
                    using Pen pen = new(ownerColor, 2);
                    using SolidBrush brush = new(LerpColor(ownerColor, Color.Black, 0.85f));
                    e.Graphics.FillPolygon(brush, child.Points);
                    e.Graphics.DrawPolygon(pen, child.Points);
                    index++;
                }

                // Draw edges
                e.Graphics.DrawPolygon(mainPen, hex.Value.Points);
            }

            // Render the coordinates
            var hexCenter = GetHexCenter(hex.Value.Points);
            var hexSize = GetHexSize(hex.Value.Points);

            SizeF textSize;
            if (chkShowCoordinates.Checked)
            {
                using var fBold = new Font(Font, FontStyle.Bold);
                (int x, int y) = hex.Key;
                string coordText = $"({x}, {y})";
                textSize = e.Graphics.MeasureString(coordText, fBold);
                e.Graphics.DrawString(coordText, fBold, Brushes.White,
                    hexCenter.X - hexSize.Width * 0.25f,                 // Align to the left
                    hexCenter.Y + hexSize.Height / 2 - textSize.Height); // Align to the bottom
            }
        }

        private void RenderHexNames(PaintEventArgs e, KeyValuePair<(int, int), Hexagon> hex, Dictionary<(int, int), Cluster> lookupTable)
        {
            var cluster = lookupTable[hex.Key];
            if (cluster.Sectors.Count == 0) return;

            using var fBoldAndUnderlined = new Font(Font, FontStyle.Bold | FontStyle.Underline);

            var hexCenter = GetHexCenter(hex.Value.Points);
            var hexSize = GetHexSize(hex.Value.Points);

            // Draw child names
            int index = 0; // reset for name rendering
            foreach (var child in hex.Value.Children)
            {
                // Render child sector name
                var sector = cluster.Sectors[index];
                var childHexCenter = GetHexCenter(child.Points);
                var childHexSize = GetHexSize(child.Points);
                var childTextSize = e.Graphics.MeasureString(sector.Name, fBoldAndUnderlined);
                e.Graphics.DrawString(sector.Name, fBoldAndUnderlined, Brushes.White,
                    childHexCenter.X - childTextSize.Width / 2,  // Center horizontally
                    childHexCenter.Y - childHexSize.Height * 0.3f - childTextSize.Height); // Place above the hex
                index++;
            }

            // Render main hex sector name if no children
            if (hex.Value.Children.Count == 0 && cluster != null)
            {
                var textSize = e.Graphics.MeasureString(cluster.Sectors[index].Name, fBoldAndUnderlined);
                e.Graphics.DrawString(cluster.Sectors[index].Name, fBoldAndUnderlined, Brushes.White,
                    hexCenter.X - textSize.Width / 2,  // Center horizontally
                    hexCenter.Y - hexSize.Height * 0.37f - textSize.Height); // Place above the hex
            }
        }

        private static PointF GetHexCenter(PointF[] hex)
        {
            float centerX = 0, centerY = 0;
            foreach (var point in hex)
            {
                centerX += point.X;
                centerY += point.Y;
            }
            return new PointF(centerX / hex.Length, centerY / hex.Length);
        }

        private static SizeF GetHexSize(PointF[] hex)
        {
            if (hex == null || hex.Length < 6)
                throw new ArgumentException("Hexagon must have at least 6 points.");

            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (var point in hex)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
            }

            float width = maxX - minX;
            float height = maxY - minY;

            return new SizeF(width, height);
        }

        private static bool IsPointInPolygon(PointF[] polygon, PointF point)
        {
            int i, j = polygon.Length - 1;
            bool inside = false;

            for (i = 0; i < polygon.Length; i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                    (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    inside = !inside;
                }
                j = i;
            }
            return inside;
        }

        private static Color HexToColor(string hexstring)
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

        public static Color LerpColor(Color color1, Color color2, float t)
        {
            // Clamp t between 0 and 1
            t = Math.Max(0, Math.Min(1, t));

            int r = (int)(color1.R + (color2.R - color1.R) * t);
            int g = (int)(color1.G + (color2.G - color1.G) * t);
            int b = (int)(color1.B + (color2.B - color1.B) * t);
            int a = (int)(color1.A + (color2.A - color1.A) * t);

            return Color.FromArgb(a, r, g, b);
        }

        private void BtnSelectLocation_Click(object sender, EventArgs e)
        {
            var position = _selectedHex.Value;

            if (GateSectorSelection)
            {
                if (!MainForm.Instance.CustomClusters.TryGetValue(position, out var cluster))
                {
                    // Either it's not a custom cluster or not a valid cluster that exists
                    // TODO: Check base game clusters and find the right cluster
                    // (for developer) Also make sure all the base game clusters are instantiated with atleast one zone in each sector.

                    // Verify if cluster has atleast one sector and one zone
                    if (cluster == null || cluster.Sectors.Count == 0 || cluster.Sectors.All(a => a.Zones.Count == 0))
                        MessageBox.Show("Invalid cluster selected, must be an existing cluster with atleast one sector and one zone.");
                    return;
                }

                // Find selected sector in cluster
                Sector selectedSector;
                if (_selectedChildHexIndex != null)
                {
                    selectedSector = cluster.Sectors[_selectedChildHexIndex.Value];
                }
                else
                {
                    selectedSector = cluster.Sectors.FirstOrDefault();
                    if (selectedSector == null)
                    {
                        MessageBox.Show("Invalid cluster selected, must be an existing cluster with atleast one sector and one zone.");
                        return;
                    }
                }

                MainForm.Instance.GateForm.txtTargetSector.Text = selectedSector.Name;
                MainForm.Instance.GateForm.txtTargetSectorLocation.Text = position.ToString() + $" [{_selectedChildHexIndex?.ToString() ?? "0"}]";
            }
            else
            {
                MainForm.Instance.ClusterForm.TxtLocation.Text = position.ToString();
            }

            DeselectHex();
            Close();
        }

        private void ChkShowCoordinates_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ChkShowX4Sectors_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ChkShowCustomSectors_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
