using System.ComponentModel;
using System.Drawing.Drawing2D;
using X4SectorCreator.Objects;

namespace X4SectorCreator
{
    public partial class SectorMapForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool GateSectorSelection { get; set; } = false;

        private readonly Dictionary<(int, int), Hexagon> _hexagons = [];
        private Dictionary<(int, int), Cluster> _baseGameClusters;
        private Cluster[] _customClusters;

        private const int _hexSize = 100;

        // How many extra rows and cols will be "open" around the base game sectors + custom sectors for the user to select
        private const int _minExpansionRoom = 20;
        private int _cols, _rows;
        private bool _dragging = false;
        private Point _lastMousePos, _mouseDownPos;
        private int? _selectedChildHexIndex, _previousSelectedChildHexIndex;
        private (int, int)? _selectedHex, _previousSelectedHex;

        private const float _defaultZoom = 1f;
        private static PointF _offset;
        private static float _zoom = _defaultZoom; // 1.0 means 100% scale

        public static IReadOnlyDictionary<string, string> DlcMapping => _dlcMapping;
        private static readonly Dictionary<string, string> _dlcMapping = new()
        {
            { "Split Vendetta", "ego_dlc_split" },
            { "Tides Of Avarice", "ego_dlc_pirate" },
            { "Cradle Of Humanity", "ego_dlc_terran" },
            { "Kingdom End", "ego_dlc_boron" },
            { "Timelines", "ego_dlc_timelines" },
            { "Hyperion Pack", "ego_dlc_mini_01" }
        };
        private static readonly Dictionary<int, bool> _dlcsSelected = [];
        private static readonly List<string> _dlcIndexOrder = [];

        private static bool _sectorMapFirstTimeOpen = true;

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

            // Init dlcs
            for (int i = 0; i < _dlcMapping.Count; i++)
            {
                if (!_dlcsSelected.TryGetValue(i, out bool value))
                {
                    // If not yet initialized, it will be by default selected
                    _dlcsSelected[i] = value = true;
                }

                // Init listbox values and pre-check the cached selected dlcs
                _dlcIndexOrder.Add(_dlcMapping.Keys.ElementAt(i));
                _ = DlcListBox.Items.Add(_dlcIndexOrder[i]);
                DlcListBox.SetItemChecked(i, value);
            }
        }

        public void Reset()
        {
            _baseGameClusters = MainForm.Instance.AllClusters
                .Where(a => a.Value.IsBaseGame)
                .ToDictionary(a => a.Key, a => a.Value);

            _customClusters = MainForm.Instance.AllClusters.Values
                .Where(a => !a.IsBaseGame)
                .ToArray();

            Dictionary<(int, int), Cluster>.ValueCollection allClusters = MainForm.Instance.AllClusters.Values;

            // Determine size of hex grid based on cluster mapping + custom sector
            if (allClusters.Count == 0) // Check if the list is empty
            {
                _cols = (_minExpansionRoom * 2) + 1;
                _rows = ((int)(_minExpansionRoom / 2 * 1.5f)) + 1;
            }
            else
            {
                _cols = ((Math.Max(Math.Abs(allClusters.Max(a => a.Position.X)), Math.Abs(allClusters.Min(a => a.Position.X))) + _minExpansionRoom) * 2) + 1;
                _rows = ((int)((Math.Max(Math.Abs(allClusters.Max(b => b.Position.Y)), Math.Abs(allClusters.Min(b => b.Position.Y))) + (_minExpansionRoom / 2)) * 1.5f)) + 1;
            }

            GenerateHexagons();
            Invalidate();
        }

        private void HandleMouseWheel(object sender, MouseEventArgs e)
        {
            const float zoomFactor = 1.1f; // 10% zoom per wheel step
            float oldZoom = _zoom;

            if (e.Delta > 0)
            {
                _zoom *= zoomFactor; // Zoom in
            }
            else
            {
                _zoom /= zoomFactor; // Zoom out
            }

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
                if (_sectorMapFirstTimeOpen)
                {
                    // Recalculate the offset to keep (0,0) in the center
                    if (_hexagons.TryGetValue((0, 0), out Hexagon zeroHex))
                    {
                        PointF center = GetHexCenter(zeroHex.Points);
                        _offset = new PointF((ClientSize.Width / 2) - center.X, (ClientSize.Height / 2) - center.Y);
                    }
                    _sectorMapFirstTimeOpen = false;
                    Invalidate(); // Force redraw
                }
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

                foreach (KeyValuePair<(int, int), Hexagon> hex in _hexagons)
                {
                    if (GateSectorSelection)
                    {
                        // Allow selecting child hex too
                        if (hex.Value.Children != null)
                        {
                            int index = 0;
                            foreach (Hexagon child in hex.Value.Children)
                            {
                                if (IsPointInPolygon(child.Points, adjustedMousePos))
                                {
                                    if (hex.Key == _selectedHex && _selectedChildHexIndex == index)
                                    {
                                        DeselectHex();
                                    }
                                    else
                                    {
                                        SelectHex(hex.Key, index);
                                    }

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
                        {
                            DeselectHex();
                        }
                        else
                        {
                            SelectHex(hex.Key);
                        }

                        return;
                    }
                }

                DeselectHex();
            }
        }

        private void SelectHex((int, int) pos, int? childIndex = null)
        {
            if (!BtnSelectLocation.Visible)
            {
                return;
            }

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
            return (q, -((r * 2) + (q & 1)));
        }

        private void GenerateHexagons()
        {
            _hexagons.Clear();

            float hexHeight = (float)(Math.Sqrt(3) * _hexSize); // Height for flat-top hexes
            int halfRow = _rows / 2;
            int halfCol = _cols / 2;

            for (int r = -halfRow; r <= halfRow; r++)
            {
                for (int q = -halfCol; q <= halfCol; q++)
                {
                    (int x, int y) translatedCoordinate = TranslateCoordinate(q, r);

                    // Define how many sectors should be in this cluster
                    int children = 1;
                    if (MainForm.Instance.AllClusters.TryGetValue(translatedCoordinate, out Cluster cluster))
                    {
                        children = cluster.Sectors.Count;
                    }

                    // Determine hex information
                    Hexagon hex = GenerateHexagonWithChildren(hexHeight, r, q, 0, 0, _defaultZoom, children);
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
            if (col % 2 != 0)
            {
                yOffset += height / 2;
            }

            xOffset += centerX;
            yOffset += centerY;

            // Generate the parent hexagon
            PointF[] parentHex =
            [
                new PointF(xOffset, yOffset),
                new PointF(xOffset + (width * 0.25f), yOffset - (height / 2)),
                new PointF(xOffset + (width * 0.75f), yOffset - (height / 2)),
                new PointF(xOffset + width, yOffset),
                new PointF(xOffset + (width * 0.75f), yOffset + (height / 2)),
                new PointF(xOffset + (width * 0.25f), yOffset + (height / 2)),
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
                    new PointF(xOffset + (width * 0.125f), yOffset - (childHeight * 0.5f)), // Top-left
                    new PointF(xOffset + (width * 0.375f), yOffset + (childHeight * 0.5f)), // Bottom-right
                ];
            }
            else if (children == 3)
            {
                // Child hex centers for top-left, bottom-left, and middle-right
                childCenters =
                [
                    new PointF(xOffset + (width * 0.125f), yOffset - (childHeight * 0.5f)), // Top-left
                    new PointF(xOffset + (width * 0.125f), yOffset + (childHeight * 0.5f)), // Bottom-left
                    new PointF(xOffset  + (width * 0.5f), yOffset) // Middle-right
                ];
            }
            else
            {
                // No children, if its other than 2 or 3
                childCenters = [];
            }

            foreach (PointF childCenter in childCenters)
            {
                childHexes.Add(
                [
                    new PointF(childCenter.X, childCenter.Y),
                    new PointF(childCenter.X + (childWidth * 0.25f), childCenter.Y - (childHeight / 2)),
                    new PointF(childCenter.X + (childWidth * 0.75f), childCenter.Y - (childHeight / 2)),
                    new PointF(childCenter.X + childWidth, childCenter.Y),
                    new PointF(childCenter.X + (childWidth * 0.75f), childCenter.Y + (childHeight / 2)),
                    new PointF(childCenter.X + (childWidth * 0.25f), childCenter.Y + (childHeight / 2)),
                ]);
            }

            return new Hexagon(parentHex, childHexes.Select(a => new Hexagon(a, null)).ToList());
        }

        private void DrawHexGrid(object sender, PaintEventArgs e)
        {
            // Init graphics properly with offset and scaling
            e.Graphics.Clear(Color.Black);
            e.Graphics.TranslateTransform(_offset.X, _offset.Y);
            e.Graphics.ScaleTransform(_zoom, _zoom);

            // Renders all the hexagons in the screen
            RenderAllHexes(e);

            // Highlight selected hex
            RenderHexSelection(e);

            // Render gate connections above hexes + selected hex
            RenderGateConnections(e);

            // Hex names draw on top of everything
            RenderAllHexNames(e);
        }

        private void RenderHexSelection(PaintEventArgs e)
        {
            if (_selectedHex != null)
            {
                using SolidBrush brush = new(Color.Cyan);
                Hexagon hexc = _hexagons[_selectedHex.Value];
                if (_selectedChildHexIndex != null)
                {
                    hexc = hexc.Children[_selectedChildHexIndex.Value];
                }
                e.Graphics.FillPolygon(brush, hexc.Points);
            }
        }

        private void RenderAllHexes(PaintEventArgs e)
        {
            // First step render non existant hexagons
            Color nonExistantHexColor = HexToColor("#121212");
            foreach (KeyValuePair<(int, int), Hexagon> hex in _hexagons)
            {
                RenderNonSectorGrid(e, nonExistantHexColor, hex);
            }

            if (chkShowX4Sectors.Checked)
            {
                // Next step render the game clusters on top
                foreach (Cluster cluster in _baseGameClusters.Values)
                {
                    // Check if the dlc is selected
                    if (!IsSelectedDlcCluster(cluster))
                    {
                        continue;
                    }

                    RenderClusters(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon));
                }
            }

            if (chkShowCustomSectors.Checked)
            {
                // Next step render the custom clusters
                foreach (Cluster cluster in _customClusters)
                {
                    // Always overwrite the hexagon as it can change between sessions
                    cluster.Hexagon = _hexagons[(cluster.Position.X, cluster.Position.Y)];
                    RenderClusters(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon));
                }
            }
        }

        private void RenderAllHexNames(PaintEventArgs e)
        {
            if (chkShowCustomSectors.Checked)
            {
                // Next step render names
                foreach (Cluster cluster in _customClusters)
                {
                    RenderHexNames(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon));
                }
            }

            if (chkShowX4Sectors.Checked)
            {
                // Next step render names
                foreach (Cluster cluster in _baseGameClusters.Values)
                {
                    // Check if the dlc is selected
                    if (!IsSelectedDlcCluster(cluster))
                    {
                        continue;
                    }

                    RenderHexNames(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon));
                }
            }
        }

        private void RenderNonSectorGrid(PaintEventArgs e, Color nonExistantHexColor, KeyValuePair<(int, int), Hexagon> hex)
        {
            // Render each non-existant hex first
            if (!MainForm.Instance.AllClusters.TryGetValue(hex.Key, out Cluster cluster) ||
                !IsSelectedDlcCluster(cluster) ||
                (!chkShowX4Sectors.Checked && cluster.IsBaseGame) ||
                (!chkShowCustomSectors.Checked && !cluster.IsBaseGame))
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
                    PointF hexCenter = GetHexCenter(hex.Value.Points);
                    SizeF hexSize = GetHexSize(hex.Value.Points);

                    using Font fBold = new(Font, FontStyle.Bold);
                    (int x, int y) = hex.Key;
                    string coordText = $"({x}, {y})";
                    textSize = e.Graphics.MeasureString(coordText, fBold);
                    e.Graphics.DrawString(coordText, fBold, Brushes.White,
                        hexCenter.X - (hexSize.Width * 0.25f),            // Align to the left
                        hexCenter.Y + (hexSize.Height / 2) - textSize.Height); // Align to the bottom
                }
            }
            else
            {
                // Set for later
                cluster.Hexagon = hex.Value;
            }
        }

        private static bool IsSelectedDlcCluster(Cluster cluster)
        {
            // If no dlc, its selected by default
            if (string.IsNullOrWhiteSpace(cluster.Dlc))
            {
                return true;
            }

            // Check if the dlc is selected
            string key = _dlcMapping.First(a => a.Value.Equals(cluster.Dlc, StringComparison.OrdinalIgnoreCase)).Key;
            int dlcIndex = _dlcIndexOrder.IndexOf(key);
            return _dlcsSelected[dlcIndex];
        }

        private static PointF ConvertFromWorldCoordinate(PointF worldPos, float sectorDiameterRadius, float hexRadius)
        {
            // Reverse world scaling
            float normalizedX = worldPos.X * 2f / sectorDiameterRadius;
            float normalizedY = worldPos.Y * 2f / sectorDiameterRadius;

            // Reverse normalization and centering
            float screenX = normalizedX * hexRadius;
            float screenY = -normalizedY * hexRadius; // Correct Y-axis negation

            return new PointF(screenX, screenY);
        }

        private void RenderGateConnections(PaintEventArgs e)
        {
            if (!chkShowConnections.Checked) return;

            var gatesData = new List<GateData>();

            // Render custom cluster gates
            if (chkShowCustomSectors.Checked)
            {
                foreach (var cluster in _customClusters)
                    gatesData.AddRange(CollectGateDataFromCluster(cluster));
            }

            // Render base game cluster gates
            if (chkShowX4Sectors.Checked)
            {
                foreach (var cluster in _baseGameClusters)
                    gatesData.AddRange(CollectGateDataFromCluster(cluster.Value));
            }

            // Collect the source / target for each gate data in one connection
            var connections = CollectConnectionsFromGateData(gatesData);
            foreach (var connection in connections)
                PaintConnection(connection, e);
        }

        private static void PaintConnection(GateConnection connection, PaintEventArgs e)
        {
            int gateSizeRadius = 6;
            float diameter = gateSizeRadius * 2;

            // Define source
            float sourceX = connection.Source.ScreenX - gateSizeRadius;
            float sourceY = connection.Source.ScreenY - gateSizeRadius;

            // Define target
            float targetX = connection.Target.ScreenX - gateSizeRadius;
            float targetY = connection.Target.ScreenY - gateSizeRadius;

            using Pen circlePen = new(Color.LightGray, 2);
            using SolidBrush circleBrush = new(HexToColor("#575757"));
            // Draw source and target gates
            e.Graphics.FillEllipse(circleBrush, sourceX, sourceY, diameter, diameter);
            e.Graphics.DrawEllipse(circlePen, sourceX, sourceY, diameter, diameter);

            e.Graphics.FillEllipse(circleBrush, targetX, targetY, diameter, diameter);
            e.Graphics.DrawEllipse(circlePen, targetX, targetY, diameter, diameter);


            using Pen linePen = new(Color.LightGray, 3);
            linePen.DashStyle = DashStyle.Dot;
            // Draw connection line between source and target
            e.Graphics.DrawLine(linePen, connection.Source.ScreenX, connection.Source.ScreenY, connection.Target.ScreenX, connection.Target.ScreenY);
        }

        private static IEnumerable<GateConnection> CollectConnectionsFromGateData(List<GateData> gatesData)
        {
            // Set to keep track of processed connections
            HashSet<(string, string)> processedConnections = [];

            foreach (var gateData in gatesData)
            {
                // Find the corresponding source sector for the current gate
                var sourceSector = gatesData
                    .Select(a => (a.Sector, GateData: a))
                    .First(a => a.Sector.Name.Equals(gateData.Gate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));

                // Find the source zone and source gate within that zone
                Zone sourceZone = sourceSector.Sector.Zones
                    .First(a => a.Gates
                        .Any(a => a.DestinationSectorName
                            .Equals(gateData.Gate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));

                Gate sourceGate = sourceZone.Gates
                    .First(a => a.DestinationSectorName.Equals(gateData.Gate.ParentSectorName, StringComparison.OrdinalIgnoreCase));

                // Create a tuple representing the source-target connection (could be source to target or target to source)
                var connectionKey = (sourceGate.DestinationSectorName, gateData.Gate.DestinationSectorName);

                // Check if the reverse connection has already been processed
                if (!processedConnections.Contains(connectionKey) && !processedConnections.Contains((gateData.Gate.DestinationSectorName, sourceGate.DestinationSectorName)))
                {
                    // Mark both directions as processed to prevent duplicates
                    processedConnections.Add(connectionKey);

                    // Yield the connection
                    yield return new GateConnection
                    {
                        Source = sourceSector.GateData, // set the stored gatesData
                        Target = gateData
                    };
                }
            }
        }

        private static IEnumerable<GateData> CollectGateDataFromCluster(Cluster cluster)
        {
            // Calculate hex size and radius based on zoom and sector size
            float hexHeight = (float)(Math.Sqrt(3) * _hexSize) * _zoom; // Height for flat-top hexes, applying zoom
            float hexRadius = (float)(hexHeight / Math.Sqrt(3)); // Recalculate radius based on zoom

            int sectorIndex = 0;
            foreach (var sector in cluster.Sectors)
            {
                // Collect the child hexagon points
                var childHexagon = cluster.Sectors.Count == 1 ? cluster.Hexagon : cluster.Hexagon.Children[sectorIndex];
                PointF hexCenter = GetHexCenter(childHexagon.Points);

                var correctHexRadius = cluster.Sectors.Count == 1 ? hexRadius : hexRadius / 2;

                foreach (var zone in sector.Zones)
                {
                    foreach (var gate in zone.Gates)
                    {
                        // Convert the zone position from world to screen space
                        PointF gateScreenPosition = ConvertFromWorldCoordinate(zone.Position, sector.DiameterRadius, correctHexRadius);

                        gateScreenPosition.X += hexCenter.X;
                        gateScreenPosition.Y += hexCenter.Y;

                        yield return new GateData
                        {
                            Cluster = cluster,
                            Sector = sector,
                            Zone = zone,
                            Gate = gate,
                            ScreenX = gateScreenPosition.X,
                            ScreenY = gateScreenPosition.Y
                        };
                    }
                }
                sectorIndex++;
            }
        }

        private void RenderClusters(PaintEventArgs e, KeyValuePair<(int, int), Hexagon> hex)
        {
            Cluster cluster = MainForm.Instance.AllClusters[hex.Key];
            if (cluster.Sectors.Count == 0)
            {
                return;
            }

            // If all sectors are the "same" owner, then the main hex becomes that color
            // If not the main hex is considered "unclaimed" until all sectors within are owned by the same faction
            Color color = MainForm.Instance.FactionColorMapping["None"];
            Sector firstSector = cluster.Sectors.FirstOrDefault();
            if (firstSector != null)
            {
                string owner = firstSector.Owner;
                if (cluster.Sectors.All(a => a.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase)))
                {
                    color = !MainForm.Instance.FactionColorMapping.TryGetValue(owner, out Color value) ? MainForm.Instance.FactionColorMapping["None"] : value;
                }
            }

            // Main hex outline
            int index = 0;
            using (Pen mainPen = new(color, 2))
            {
                if (cluster.Sectors.Count > 1)
                {
                    color = Color.Black;
                }

                // Fill with darker color
                using SolidBrush mainBrush = new(LerpColor(color, Color.Black, 0.85f));
                e.Graphics.FillPolygon(mainBrush, hex.Value.Points);

                // Draw child hex outlines
                foreach (Hexagon child in hex.Value.Children)
                {
                    // First child hex: up
                    // Second child hex: down
                    // Third child hex: right
                    Sector sector = cluster.Sectors[index];
                    Color ownerColor = !MainForm.Instance.FactionColorMapping.TryGetValue(sector.Owner, out Color value) ?
                        MainForm.Instance.FactionColorMapping["None"] : value;
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
            PointF hexCenter = GetHexCenter(hex.Value.Points);
            SizeF hexSize = GetHexSize(hex.Value.Points);

            SizeF textSize;
            if (chkShowCoordinates.Checked)
            {
                using Font fBold = new(Font, FontStyle.Bold);
                (int x, int y) = hex.Key;
                string coordText = $"({x}, {y})";
                textSize = e.Graphics.MeasureString(coordText, fBold);
                e.Graphics.DrawString(coordText, fBold, Brushes.White,
                    hexCenter.X - (hexSize.Width * 0.25f),                 // Align to the left
                    hexCenter.Y + (hexSize.Height / 2) - textSize.Height); // Align to the bottom
            }
        }

        private void RenderHexNames(PaintEventArgs e, KeyValuePair<(int, int), Hexagon> hex)
        {
            Cluster cluster = MainForm.Instance.AllClusters[hex.Key];
            if (cluster.Sectors.Count == 0)
            {
                return;
            }

            using Font fBoldAndUnderlined = new(Font, FontStyle.Bold | FontStyle.Underline);

            PointF hexCenter = GetHexCenter(hex.Value.Points);
            SizeF hexSize = GetHexSize(hex.Value.Points);

            // Draw child names
            int index = 0; // reset for name rendering
            foreach (Hexagon child in hex.Value.Children)
            {
                // Render child sector name
                Sector sector = cluster.Sectors[index];
                PointF childHexCenter = GetHexCenter(child.Points);
                SizeF childHexSize = GetHexSize(child.Points);
                SizeF childTextSize = e.Graphics.MeasureString(sector.Name, fBoldAndUnderlined);
                e.Graphics.DrawString(sector.Name, fBoldAndUnderlined, Brushes.White,
                    childHexCenter.X - (childTextSize.Width / 2),  // Center horizontally
                    childHexCenter.Y - (childHexSize.Height * 0.3f) - childTextSize.Height); // Place above the hex
                index++;
            }

            // Render main hex sector name if no children
            if (hex.Value.Children.Count == 0 && cluster != null)
            {
                SizeF textSize = e.Graphics.MeasureString(cluster.Sectors[index].Name, fBoldAndUnderlined);
                e.Graphics.DrawString(cluster.Sectors[index].Name, fBoldAndUnderlined, Brushes.White,
                    hexCenter.X - (textSize.Width / 2),  // Center horizontally
                    hexCenter.Y - (hexSize.Height * 0.37f) - textSize.Height); // Place above the hex
            }
        }

        private static PointF GetHexCenter(PointF[] hex)
        {
            float centerX = 0, centerY = 0;
            foreach (PointF point in hex)
            {
                centerX += point.X;
                centerY += point.Y;
            }
            return new PointF(centerX / hex.Length, centerY / hex.Length);
        }

        private static SizeF GetHexSize(PointF[] hex)
        {
            if (hex == null || hex.Length < 6)
            {
                throw new ArgumentException("Hexagon must have at least 6 points.");
            }

            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (PointF point in hex)
            {
                if (point.X < minX)
                {
                    minX = point.X;
                }

                if (point.X > maxX)
                {
                    maxX = point.X;
                }

                if (point.Y < minY)
                {
                    minY = point.Y;
                }

                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
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
                    (point.X < ((polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y)) + polygon[i].X))
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

            int r = (int)(color1.R + ((color2.R - color1.R) * t));
            int g = (int)(color1.G + ((color2.G - color1.G) * t));
            int b = (int)(color1.B + ((color2.B - color1.B) * t));
            int a = (int)(color1.A + ((color2.A - color1.A) * t));

            return Color.FromArgb(a, r, g, b);
        }

        private void BtnSelectLocation_Click(object sender, EventArgs e)
        {
            (int, int) position = _selectedHex.Value;

            if (GateSectorSelection)
            {
                if (!MainForm.Instance.AllClusters.TryGetValue(position, out Cluster cluster))
                {
                    _ = MessageBox.Show("Invalid cluster selected.");
                    return;
                }

                // Verify if cluster has atleast one sector and one zone
                if (cluster.Sectors.Count == 0)
                {
                    _ = MessageBox.Show("The selected cluster must have atleast one sector.");
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
                        _ = MessageBox.Show("Invalid cluster selected, must be an existing cluster with atleast one sector and one zone.");
                        return;
                    }
                }

                MainForm.Instance.GateForm.txtTargetSector.Text = selectedSector.Name;
                MainForm.Instance.GateForm.txtTargetSectorLocation.Text = position.ToString() + $" [{_selectedChildHexIndex?.ToString() ?? "0"}]";
                MainForm.Instance.GateForm.TargetSectorSelection = null; // Recalibrates automatically
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

        private void ChkShowConnections_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void DlcListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _dlcsSelected[e.Index] = e.NewValue == CheckState.Checked;
            Invalidate();
        }

        struct GateConnection
        {
            public GateData Source { get; set; }
            public GateData Target { get; set; }
        }

        struct GateData
        {
            public float ScreenX { get; set; }
            public float ScreenY { get; set; }
            public Gate Gate { get; set; }
            public Zone Zone { get; set; }
            public Sector Sector { get; set; }
            public Cluster Cluster { get; set; }
        }
    }
}
