using System.ComponentModel;
using System.Drawing.Drawing2D;
using X4SectorCreator.Forms;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator
{
    public partial class SectorMapForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool GateSectorSelection { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ClusterSectorSelection { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionForm FactionForm { get; set; }

        private readonly Dictionary<(int, int), Hexagon> _hexagons = [];
        private Dictionary<(int, int), Cluster> _baseGameClusters;
        private Cluster[] _customClusters;

        private const int _hexSize = 200;
        private const int _iconSize = 128;

        // How many extra rows and cols will be "open" around the base game sectors + custom sectors for the user to select
        private const int _minExpansionRoom = 20;
        private int _cols, _rows;
        private bool _dragging = false;
        private Point _lastMousePos, _mouseDownPos;
        private int? _selectedChildHexIndex, _previousSelectedChildHexIndex;
        private (int, int)? _selectedHex, _previousSelectedHex;

        private const float _defaultZoom = 1f; // 1.0 means 100% scale
        private static PointF _offset;
        private static float _zoom = 0.8f;
        private const float _minZoom = 0.075f, _maxZoom = 2.5f;
        private const float _gateSizeRadius = 8f;

        public static IReadOnlyDictionary<string, string> DlcMapping => _dlcMapping;
        private static readonly Dictionary<string, string> _dlcMapping = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Split Vendetta", "ego_dlc_split" },
            { "Tides Of Avarice", "ego_dlc_pirate" },
            { "Cradle Of Humanity", "ego_dlc_terran" },
            { "Kingdom End", "ego_dlc_boron" },
            { "Timelines", "ego_dlc_timelines" },
            { "Hyperion Pack", "ego_dlc_mini_01" }
        };

        private static readonly Dictionary<string, int> _selectedDlcMapping = _dlcMapping
            .Select((a, i) => (a.Value, index: i))
            .ToDictionary(a => a.Value, a => a.index, StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<int, bool> _dlcsSelected = [];

        private static bool _sectorMapFirstTimeOpen = true;
        private int _originalLegendPanelHeight, _originalControlPanelHeight;

        private Image _factionLogicImageLarge;
        private Image _factionLogicImageSmall;
        private readonly Dictionary<Color, Dictionary<string, Image>> _cachedStationIconsLarge = [];
        private readonly Dictionary<Color, Dictionary<string, Image>> _cachedStationIconsSmall = [];
        private readonly Dictionary<Color, Image> cachedRegionImagesLarge = [];
        private readonly Dictionary<Color, Image> cachedRegionImagesSmall = [];

        // TODO: Rework to use ImageList in treeview with image nodes and colors
        private static readonly Dictionary<string, List<object>> _legend = new(StringComparer.OrdinalIgnoreCase)
        {
            {
                "Resources", new List<object>
                {
                    ("Ore", Color.Orange),
                    ("Silicon", Color.SlateGray),
                    ("Ice", Color.White),
                    ("Methane", Color.DeepSkyBlue),
                    ("Helium", Color.LightCoral),
                    ("Hydrogen", Color.DarkCyan),
                    ("Nividium", Color.Fuchsia),
                    ("RawScrap", Color.Red)
                }
            },
            {
                "Others", new List<object>
                {
                    "Faction Logic Disabled"
                }
            }
        };

        private static readonly Dictionary<string, double> _yieldDensities = new(StringComparer.OrdinalIgnoreCase)
        {
            ["lowest"] = 0.026,
            ["verylow"] = 0.06,
            ["lowminus"] = 0.2,
            ["low"] = 0.6,
            ["lowplus"] = 1.8,
            ["medlow"] = 4,
            ["medium"] = 6,
            ["medplus"] = 16,
            ["medhigh"] = 32,
            ["highlow"] = 48,
            ["high"] = 60,
            ["highplus"] = 120,
            ["veryhigh"] = 3600,
            ["highest"] = 60000
        };

        private static readonly Dictionary<string, Image> _imageMap = new(StringComparer.OrdinalIgnoreCase);

        private static bool _optionWasMinimzed = false, _legendWasMinimized = false;

        protected override CreateParams CreateParams
        {
            get
            {
                // Used to start draw process on init of class
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private readonly List<Sector> _availableSearchSectors = [];
        private readonly HashSet<Sector> _visibleSectorsFromSearch = [];

        public SectorMapForm()
        {
            InitializeComponent();

            TxtSearch.EnableTextSearch(_availableSearchSectors, a => a.Name, SearchRender);
            Disposed += SectorMapForm_Disposed;

            ControlPanel.Top = 12;

            // Setup events
            DoubleBuffered = true;
            KeyPreview = true;

            // Init dlcs
            foreach (KeyValuePair<string, int> mapping in _selectedDlcMapping)
            {
                if (!_dlcsSelected.TryGetValue(mapping.Value, out bool value))
                {
                    // If not yet initialized, it will be by default selected
                    _dlcsSelected[mapping.Value] = value = true;
                }

                // Init dlc list box
                _ = DlcListBox.Items.Add(_dlcMapping.First(a => a.Value.Equals(mapping.Key)).Key);
                DlcListBox.SetItemChecked(mapping.Value, value);
            }

            // Setup legend
            SetupLegendTree();

            MouseDown += HandleMouseDown;
            MouseUp += HandleMouseUp;
            MouseMove += HandleMouseMove;
            Paint += DrawHexGrid;
            Resize += HandleResize;
            MouseWheel += HandleMouseWheel;
            MouseClick += SectorMapForm_MouseClick;
            KeyDown += SectorMapForm_KeyDown;
        }

        private void SectorMapForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        private void SearchRender(List<Sector> data)
        {
            _visibleSectorsFromSearch.Clear();
            foreach (var item in data)
                _visibleSectorsFromSearch.Add(item);

            Invalidate();
        }

        private void SetupLegendTree()
        {
            LegendPanel.Top = ClientSize.Height - LegendPanel.Height - 3;
            LegendTree.DrawNode += LegendTree_DrawNode;
            LegendTree.ImageList = new ImageList
            {
                ImageSize = new Size(16, 16)
            };

            // Init region images
            var regionImage = GetIconFromStore("region_resource");
            foreach (var resource in _legend["resources"])
            {
                var (name, color) = ((string name, Color color))resource;
                LegendTree.ImageList.Images.Add(name, regionImage.CopyAsTint(color));
            }
            LegendTree.ImageList.Images.Add("Faction Logic Disabled", GetIconFromStore("faction_logic_disabled"));

            foreach (var legendEntry in _legend)
            {
                var node = new TreeNode(legendEntry.Key)
                {
                    ImageIndex = LegendTree.ImageList.Images.Count,
                    SelectedImageIndex = LegendTree.ImageList.Images.Count
                };

                foreach (var entry in legendEntry.Value)
                {
                    TreeNode childNode;
                    if (entry is string entryStr)
                    {
                        childNode = new TreeNode(entryStr);
                        if (entryStr.Equals("Faction Logic Disabled", StringComparison.OrdinalIgnoreCase))
                            childNode.ImageKey = entryStr;
                    }
                    else
                    {
                        var (name, _) = ((string name, Color color))entry;
                        childNode = new TreeNode(name);
                        if (legendEntry.Key.Equals("resources", StringComparison.OrdinalIgnoreCase))
                            childNode.ImageKey = name;
                    }
                    node.Nodes.Add(childNode);
                }
                LegendTree.Nodes.Add(node);
            }
            LegendTree.ExpandAll();
        }

        private void LegendTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            TreeView tree = e.Node.TreeView;

            int textX = (e.Node.ImageKey == string.Empty && e.Node.SelectedImageKey == string.Empty) ?
                e.Bounds.Left - e.Node.TreeView.ImageList.ImageSize.Width : e.Bounds.Left - 2;
            int textY = (e.Bounds.Top + e.Bounds.Bottom) / 2 + 1;

            TextRenderer.DrawText(
                e.Graphics,
                e.Node.Text,
                e.Node.NodeFont ?? tree.Font,
                new Point(textX, textY),
                tree.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left
            );

            e.DrawDefault = false;
        }

        public static Image GetIconFromStore(string iconName)
        {
            if (!_imageMap.TryGetValue(iconName, out var image))
            {
                var path = Path.Combine(Application.StartupPath, $"Data/Icons/{iconName}.png");
                if (File.Exists(path))
                {
                    _imageMap[iconName] = image = Image.FromFile(path);
                }
                else
                {
                    _imageMap[iconName] = image = null;
                }
            }
            return image;
        }

        private void SectorMapForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && _movingCluster != null)
            {
                _movingCluster = null;
                Invalidate();
            }
        }

        private Cluster _movingCluster = null;
        private void SectorMapForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PointF adjustedMousePos = new(
                    (e.Location.X - _offset.X) / _zoom,
                    (e.Location.Y - _offset.Y) / _zoom
                );

                // Don't allow cluster movement when searching
                if (_visibleSectorsFromSearch.Count > 0)
                {
                    _ = MessageBox.Show("Cannot move clusters while a search filter is set.");
                    return;
                }

                if (_movingCluster == null)
                {
                    Cluster cluster = GetClusterAtMousePos(adjustedMousePos, out _);
                    if (cluster != null)
                    {
                        _movingCluster = cluster;
                        Invalidate();
                    }
                }
                else
                {
                    // Verify for valid position
                    Cluster clusterAtPos = GetClusterAtMousePos(adjustedMousePos, out (int x, int y)? coordinate);
                    if (clusterAtPos != null)
                    {
                        // Cancel because we're moving to the same position
                        if (clusterAtPos == _movingCluster)
                        {
                            _movingCluster = null;
                            Invalidate();
                            return;
                        }

                        _ = MessageBox.Show("Cannot place cluster at the target location because another cluster already exists here.");
                        return;
                    }

                    // Place cluster down at the new position if it is valid
                    _ = MainForm.Instance.AllClusters.Remove((_movingCluster.Position.X, _movingCluster.Position.Y));
                    _movingCluster.Position = new Point(coordinate.Value.x, coordinate.Value.y);
                    MainForm.Instance.AllClusters[coordinate.Value] = _movingCluster;
                    _movingCluster = null;
                    Reset();
                }
            }
        }

        private Cluster GetClusterAtMousePos(PointF mousePos, out (int x, int y)? pos)
        {
            pos = null;
            foreach (var hex in _baseGameClusters.Values
                .Concat(_customClusters)
                .Select(a => a.Hexagon)
                .Where(a => a != null))
            {
                // Determine if there is a cluster at the position we clicked
                if (IsPointInPolygon(hex.Points, mousePos))
                {
                    pos = hex.Position;
                    if (MainForm.Instance.AllClusters.TryGetValue(hex.Position, out Cluster cluster))
                    {
                        return cluster;
                    }
                    break;
                }
            }
            return null;
        }

        public void Reset()
        {
            _movingCluster = null;
            _baseGameClusters = MainForm.Instance.AllClusters
                .Where(a => a.Value.IsBaseGame)
                .ToDictionary(a => a.Key, a => a.Value);

            _customClusters = [.. MainForm.Instance.AllClusters.Values.Where(a => !a.IsBaseGame)];

            // Setup all data
            _availableSearchSectors.AddRange(_baseGameClusters.Values.Concat(_customClusters).SelectMany(a => a.Sectors));

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

            _zoom = Math.Clamp(_zoom, _minZoom, _maxZoom); // Limit zoom between 50% and 200%

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
                }
            }
            Invalidate(); // Force redraw
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
                    if (GateSectorSelection || FactionForm != null)
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

                    _ = MainForm.Instance.AllClusters.TryGetValue(translatedCoordinate, out Cluster cluster);

                    // Determine hex information
                    Hexagon hex = GenerateHexagonWithChildren(hexHeight, r, q, 0, 0, cluster?.Sectors, translatedCoordinate, _defaultZoom);
                    _hexagons[translatedCoordinate] = hex;
                }
            }
        }

        /// <summary>
        /// Determine's the calculations that need to be done based on the sector's placement value
        /// </summary>
        private static readonly Dictionary<SectorPlacement, Func<float, float, (float x, float y)>> _childPlacementMappings = new()
        {
            {SectorPlacement.TopRight, (float width, float childHeight) => (width * 0.375f, -(childHeight * 0.5f)) },
            {SectorPlacement.TopLeft, (float width, float childHeight) => (width * 0.125f, -(childHeight * 0.5f)) },
            {SectorPlacement.BottomRight, (float width, float childHeight) => (width * 0.375f, childHeight * 0.5f) },
            {SectorPlacement.BottomLeft, (float width, float childHeight) => (width * 0.125f, childHeight * 0.5f) },
            {SectorPlacement.MiddleRight, (float width, float childHeight) => (width * 0.5f, 0) },
            {SectorPlacement.MiddleLeft, (float width, float childHeight) => (width * 0, 0) }
        };

        private static Hexagon GenerateHexagonWithChildren(float height, int row, int col, float centerX, float centerY, List<Sector> sectors, (int x, int y) translatedCoordinate, float zoom = 1.0f)
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
            int children = sectors?.Count ?? 0;

            List<PointF> childHexPositions = [];
            if (children is 2 or 3)
            {
                // Child hex centers for top-left, bottom-right
                for (int i = 0; i < children; i++)
                {
                    (float x, float y) = _childPlacementMappings[sectors[i].Placement](width, childHeight);
                    childHexPositions.Add(new PointF(xOffset + x, yOffset + y));
                }
            }

            foreach (PointF childCenter in childHexPositions)
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

            return new Hexagon(translatedCoordinate, parentHex, childHexes.Select(a => new Hexagon(translatedCoordinate, a, null)).ToList());
        }

        private void DrawHexGrid(object sender, PaintEventArgs e)
        {
            try
            {
                // Init graphics properly with offset and scaling
                e.Graphics.Clear(Color.Black);
                e.Graphics.TranslateTransform(_offset.X, _offset.Y);
                e.Graphics.ScaleTransform(_zoom, _zoom);
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Renders all the hexagons in the screen
                RenderAllHexes(e);

                // Highlight selected hex
                RenderHexSelection(e);

                // Render gate connections above hexes + selected hex
                RenderGateConnections(e);

                // Render station icons
                RenderHexIcons(e);

                // Hex names draw on top of everything
                RenderAllHexNames(e);

                // Draw tip label
                RenderTipLabel(e);
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#else
                _ = MessageBox.Show("An error occured when trying to render the map view: \"" + ex.Message + "\".\nPlease create a bug report. (Be sure to provide the export xml or exact reproduction steps).");
                Close();
#endif
            }
        }

        private void RenderTipLabel(PaintEventArgs e)
        {
            GraphicsState state = e.Graphics.Save();
            e.Graphics.ResetTransform();

            string labelText = "Tip: You can right click on clusters to move them around.";
            using (Font font = new("Segoe UI", 12f, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Yellow))
            {
                // Measure text size
                SizeF textSize = e.Graphics.MeasureString(labelText, font);

                // Position label at screen top center
                float x = (ClientSize.Width - textSize.Width) / 2f;
                float y = 10;

                // Draw text with fixed size
                e.Graphics.DrawString(labelText, font, brush, x, y);
            }

            e.Graphics.Restore(state);
        }

        private void RenderHexIcons(PaintEventArgs e)
        {
            var sizeSmall = new Point(_iconSize / 2, _iconSize / 2);
            var sizeLarge = new Point(_iconSize, _iconSize);

            // These are exceptional as they are rendered directly in the position of the station
            RenderStationIcons(e, sizeSmall, sizeLarge);

            // Other icons should be rendered much smaller
            sizeSmall = new Point((int)(_iconSize / 2.5f), (int)(_iconSize / 2.5f));
            sizeLarge = new Point(_iconSize / 2, _iconSize / 2);

            // Collection of icon data
            var icons = new List<IconData>();
            icons.AddRange(CollectRegionIconData(sizeSmall, sizeLarge));
            icons.AddRange(CollectOtherIconData(sizeSmall, sizeLarge));

            // Render other icons at the bottom of the hex
            RenderSmallHexIcons(e, icons);
        }

        private void RenderSmallHexIcons(PaintEventArgs e, List<IconData> iconDatas)
        {
            // Calculate hex size and radius based on zoom and sector size
            float hexHeight = (float)(Math.Sqrt(3) * _hexSize) * _defaultZoom; // Height for flat-top hexes, applying zoom
            float hexRadius = (float)(hexHeight / Math.Sqrt(3)); // Recalculate radius based on zoom

            // Each icon is rendered in the cluster or sector bottom right corner
            foreach (var group in iconDatas.GroupBy(a => a.Sector))
            {
                if (_visibleSectorsFromSearch.Count > 0 && !_visibleSectorsFromSearch.Contains(group.Key))
                    continue;

                var processed = 0;
                var xLayerProcess = 0;
                foreach (var icon in group.DistinctBy(a => a.Type, StringComparer.OrdinalIgnoreCase))
                {
                    var cluster = icon.Cluster;
                    var sector = icon.Sector;
                    var sectorIndex = cluster.Sectors.IndexOf(sector);

                    // Define the size for the resized icon (width and height)
                    int width = cluster.Sectors.Count == 1 ? icon.ImageLarge.Width : icon.ImageSmall.Width;
                    int height = cluster.Sectors.Count == 1 ? icon.ImageLarge.Height : icon.ImageSmall.Height;

                    // Collect the child hexagon points
                    Hexagon childHexagon = cluster.Sectors.Count == 1 ? cluster.Hexagon : cluster.Hexagon.Children[sectorIndex];
                    PointF hexCenter = GetHexCenter(childHexagon.Points);
                    float correctHexHeight = cluster.Sectors.Count == 1 ? hexHeight : hexHeight / 2;

                    // Bottom left corner
                    float startX = hexCenter.X - correctHexHeight / 4 - (cluster.Sectors.Count == 1 ? 10 : 5);
                    float startY = hexCenter.Y + correctHexHeight / 2 - (height / 2);

                    // Increment by icon size + 1
                    for (int i = 0; i < xLayerProcess; i++)
                    {
                        startX += (int)((width / 2f)) - (cluster.Sectors.Count == 1 ? 10 : 5);
                    }

                    // Icons are shown per 4, on each Y layer
                    var yLayer = (int)(processed / 4f);
                    startY -= ((height / 2) - (cluster.Sectors.Count == 1 ? 10 : 5)) * yLayer;

                    // Define position
                    var pos = new PointF(startX, startY);

                    // Draw the resized icon at a specific position on the form (x, y)
                    var iconToUse = cluster.Sectors.Count == 1 ? icon.ImageLarge : icon.ImageSmall;
                    e.Graphics.DrawImage(iconToUse, pos.X, pos.Y);
                    processed++;

                    if (!string.IsNullOrWhiteSpace(icon.Yield))
                    {
                        using Font fBold = new(Font.FontFamily, (cluster.Sectors.Count == 1 ? 12 : 10), FontStyle.Bold);
                        var text = icon.Yield;
                        e.Graphics.DrawString(text, fBold, Brushes.Black,
                            pos.X - 1f, pos.Y -1f);
                    }

                    // Reset
                    xLayerProcess++;
                    if (xLayerProcess == 4)
                        xLayerProcess = 0;
                }
            }
        }

        private IEnumerable<IconData> CollectRegionIconData(Point small, Point large)
        {
            if (!ChkShowRegions.Checked) yield break;

            List<Cluster> relevantClusters = _baseGameClusters.Values
                .Concat(_customClusters)
                .Where(cluster => cluster.Sectors.Any(sector => sector.Regions.Count > 0))
                .ToList();
            if (relevantClusters.Count == 0) yield break;

            var regionIcon = GetIconFromStore("region_resource");
            if (regionIcon == null) yield break;

            var resourceColors = _legend["resources"]
                .Select(a => ((string name, Color color))a)
                .ToDictionary(a => a.name, a => a.color, StringComparer.OrdinalIgnoreCase);
            if (resourceColors.Count == 0) yield break;

            foreach (Cluster cluster in relevantClusters)
            {
                foreach (Sector sector in cluster.Sectors.Where(a => a.Regions.Count > 0))
                {
                    var resources = sector.Regions
                        .SelectMany(a => a.Definition.Resources);
                    foreach (var resource in resources)
                    {
                        if (!resourceColors.TryGetValue(resource.Ware, out var resourceColor))
                        {
                            throw new Exception("No legend color defined for resource: " + resource.Ware);
                        }

                        if (!cachedRegionImagesLarge.TryGetValue(resourceColor, out var imageTintLarge))
                        {
                            cachedRegionImagesLarge[resourceColor] = imageTintLarge = regionIcon.Resize(large.X, large.Y, InterpolationMode.HighQualityBicubic, resourceColor);
                        }
                        if (!cachedRegionImagesSmall.TryGetValue(resourceColor, out var imageTintSmall))
                        {
                            cachedRegionImagesSmall[resourceColor] = imageTintSmall = regionIcon.Resize(small.X, small.Y, InterpolationMode.HighQualityBicubic, resourceColor);
                        }

                        yield return new IconData
                        {
                            Cluster = cluster,
                            Sector = sector,
                            ImageLarge = imageTintLarge,
                            ImageSmall = imageTintSmall,
                            Type = resource.Ware,
                            Yield = GetYieldValue(resource.Yield)
                        };
                    }
                }
            }
        }

        private static string GetYieldValue(string yield)
        {
            if (!_yieldDensities.TryGetValue(yield.ToLower(), out double density))
                return "0";

            double min = _yieldDensities.Values.Min();
            double max = _yieldDensities.Values.Max();

            // Log scale
            double logMin = Math.Log10(min);
            double logMax = Math.Log10(max);
            double logValue = Math.Log10(density);

            double normalized = (logValue - logMin) / (logMax - logMin);
            int scaled = (int)Math.Round(1 + normalized * (99 - 1));

            return scaled.ToString();
        }

        private IEnumerable<IconData> CollectOtherIconData(Point sizeSmall, Point sizeLarge)
        {
            // Also hide it behind regions
            if (!ChkShowRegions.Checked) yield break;

            var factionLogicDisabledIcon = GetIconFromStore("faction_logic_disabled");
            if (factionLogicDisabledIcon == null) yield break;

            var iconLarge = _factionLogicImageLarge ??= factionLogicDisabledIcon.Resize(sizeLarge.X, sizeLarge.Y, InterpolationMode.HighQualityBicubic);
            var iconSmall = _factionLogicImageSmall ??= factionLogicDisabledIcon.Resize(sizeSmall.X, sizeSmall.Y, InterpolationMode.HighQualityBicubic);

            foreach (Cluster cluster in _baseGameClusters.Values.Concat(_customClusters))
            {
                foreach (var sector in cluster.Sectors)
                {
                    // Icon for disabled faction logic
                    if (sector.DisableFactionLogic)
                    {
                        yield return new IconData
                        {
                            Cluster = cluster,
                            Sector = sector,
                            ImageLarge = iconLarge,
                            ImageSmall = iconSmall,
                            Type = "faction_logic_disabled"
                        };
                    }
                }
            }
        }

        private void RenderStationIcons(PaintEventArgs e, Point sizeSmall, Point sizeLarge)
        {
            if (!ChkShowStations.Checked) return;

            List<Cluster> relevantClusters = _baseGameClusters.Values
                .Concat(_customClusters)
                .Where(cluster => cluster.Sectors.Any(sector => sector.Zones.Any(zone => zone.Stations.Count != 0)))
                .ToList();
            if (relevantClusters.Count == 0)
            {
                return;
            }

            // Calculate hex size and radius based on zoom and sector size
            float hexHeight = (float)(Math.Sqrt(3) * _hexSize) * _defaultZoom; // Height for flat-top hexes, applying zoom
            float hexRadius = (float)(hexHeight / Math.Sqrt(3)); // Recalculate radius based on zoom

            foreach (Cluster cluster in relevantClusters)
            {
                int sectorIndex = 0;
                foreach (Sector sector in cluster.Sectors)
                {
                    if (_visibleSectorsFromSearch.Count > 0 && !_visibleSectorsFromSearch.Contains(sector))
                        continue;

                    // Collect the child hexagon points
                    Hexagon childHexagon = cluster.Sectors.Count == 1 ? cluster.Hexagon : cluster.Hexagon.Children[sectorIndex];
                    PointF hexCenter = GetHexCenter(childHexagon.Points);
                    float correctHexRadius = cluster.Sectors.Count == 1 ? hexRadius : hexRadius / 2;

                    foreach (Zone zone in sector.Zones)
                    {
                        foreach (Station station in zone.Stations)
                        {
                            var stationIcon = GetIconFromStore(station.Type.ToLower());
                            if (stationIcon == null) continue;

                            // Convert the zone position from world to screen space
                            PointF stationScreenPosition = ConvertFromWorldCoordinate(station.Position, sector.DiameterRadius, correctHexRadius);

                            stationScreenPosition.X += hexCenter.X;
                            stationScreenPosition.Y += hexCenter.Y;

                            Color color = FactionsForm.GetColorForFaction(station.Owner);

                            // Define the size for the resized icon (width and height)
                            int width = cluster.Sectors.Count == 1 ? sizeLarge.X : sizeSmall.X;
                            int height = cluster.Sectors.Count == 1 ? sizeLarge.Y : sizeSmall.Y;
                            width /= 2;
                            height /= 2;

                            Image resizedIcon;
                            if (cluster.Sectors.Count == 1)
                            {
                                if (!_cachedStationIconsLarge.TryGetValue(color, out var iconsLarge))
                                {
                                    _cachedStationIconsLarge[color] = iconsLarge = new(StringComparer.OrdinalIgnoreCase);
                                }

                                if (!iconsLarge.TryGetValue(station.Type, out var icon))
                                {
                                    icon = stationIcon.Resize(width, height, InterpolationMode.HighQualityBicubic, color);
                                    iconsLarge[station.Type] = icon;
                                }
                                resizedIcon = icon;
                            }
                            else
                            {
                                if (!_cachedStationIconsSmall.TryGetValue(color, out var iconsSmall))
                                {
                                    _cachedStationIconsSmall[color] = iconsSmall = new(StringComparer.OrdinalIgnoreCase);
                                }

                                if (!iconsSmall.TryGetValue(station.Type, out var icon))
                                {
                                    icon = stationIcon.Resize(width, height, InterpolationMode.HighQualityBicubic, color);
                                    iconsSmall[station.Type] = icon;
                                }
                                resizedIcon = icon;
                            }

                            // Draw the resized icon at a specific position on the form (x, y)
                            e.Graphics.DrawImage(resizedIcon, (int)stationScreenPosition.X - (width / 2), (int)stationScreenPosition.Y - (height / 2));
                        }
                    }
                    sectorIndex++;
                }
            }
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
            Color nonExistantHexColor = "#121212".HexToColor();
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
                    if (!IsDlcClusterEnabled(cluster))
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
                    if (!IsDlcClusterEnabled(cluster))
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
                !IsDlcClusterEnabled(cluster) ||
                (!chkShowX4Sectors.Checked && cluster.IsBaseGame) ||
                (!chkShowCustomSectors.Checked && !cluster.IsBaseGame) ||
                _visibleSectorsFromSearch.Count > 0 && cluster.Sectors.Any(a => !_visibleSectorsFromSearch.Contains(a)))
            {
                using SolidBrush mainBrush = new(Color.Black);
                using Pen mainPen = new(nonExistantHexColor, 4);
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

        private static bool IsDlcClusterEnabled(Cluster cluster)
        {
            // If no dlc, its selected by default
            if (string.IsNullOrWhiteSpace(cluster.Dlc))
            {
                return true;
            }

            // Check if the dlc is selected
            return _dlcsSelected[_selectedDlcMapping[cluster.Dlc]];
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
            if (!chkShowConnections.Checked)
            {
                return;
            }

            List<GateData> gatesData = [];

            // Render custom cluster gates
            if (chkShowCustomSectors.Checked)
            {
                foreach (Cluster cluster in _customClusters)
                {
                    gatesData.AddRange(CollectGateDataFromCluster(cluster));
                }
            }

            // Render base game cluster gates
            if (chkShowX4Sectors.Checked)
            {
                foreach (KeyValuePair<(int, int), Cluster> cluster in _baseGameClusters)
                {
                    // Check if the dlc is selected
                    if (!IsDlcClusterEnabled(cluster.Value))
                    {
                        continue;
                    }

                    gatesData.AddRange(CollectGateDataFromCluster(cluster.Value));
                }
            }

            // Collect the source / target for each gate data in one connection
            // Filter out highway connections they are always duped but they have different paths
            // It's kinda difficult to filter them out properly, we do it for now based on sector name but its not the ideal solution.
            // Because as a side effect this can cause multiple highways with the same from/to sector to be filtered out unintentionally
            // But as far as I have seen, these type of connections don't exist in the base game.
            GateConnection[] connections = [.. CollectConnectionsFromGateData(gatesData).FilterDuplicateHighwayConnections()];

            foreach (GateConnection connection in connections)
            {
                PaintConnection(connection, e);
            }
        }

        private static void PaintConnection(GateConnection connection, PaintEventArgs e)
        {
            float diameter = _gateSizeRadius * 2;

            // Define source
            float sourceX = connection.Source.ScreenX - _gateSizeRadius;
            float sourceY = connection.Source.ScreenY - _gateSizeRadius;

            // Define target
            float targetX = connection.Target.ScreenX - _gateSizeRadius;
            float targetY = connection.Target.ScreenY - _gateSizeRadius;

            Color color = Color.LightGray;
            if (connection.Source.Gate.IsHighwayGate)
            {
                color = Color.SlateGray;
            }

            using Pen circlePen = new(color, _gateSizeRadius / 2f);
            using SolidBrush circleBrush = new("#575757".HexToColor());

            // Draw source and target gates
            e.Graphics.FillEllipse(circleBrush, sourceX, sourceY, diameter, diameter);
            e.Graphics.DrawEllipse(circlePen, sourceX, sourceY, diameter, diameter);

            e.Graphics.FillEllipse(circleBrush, targetX, targetY, diameter, diameter);
            e.Graphics.DrawEllipse(circlePen, targetX, targetY, diameter, diameter);

            using Pen linePen = new(color, _gateSizeRadius / 2f);

            linePen.DashStyle = connection.Source.Gate.IsHighwayGate ? DashStyle.Dash : DashStyle.Dot;

            // Draw connection line between source and target
            e.Graphics.DrawLine(linePen, connection.Source.ScreenX, connection.Source.ScreenY, connection.Target.ScreenX, connection.Target.ScreenY);
        }

        private IEnumerable<GateConnection> CollectConnectionsFromGateData(List<GateData> gatesData)
        {
            Dictionary<string, GateData[]> sectorGrouping = gatesData
                .GroupBy(a => a.Sector.Name)
                .ToDictionary(a => a.Key, a => a.ToArray(), StringComparer.OrdinalIgnoreCase);

            // Make sure we don't double process target gates we already processed
            // We still have an issue with highway type gates showing as a double because they have different paths
            HashSet<Gate> processedTargets = [];

            // Any invalid connections will be recorded
            List<GateData> invalidConnections = [];

            // Set to keep track of processed connections
            foreach (GateData sourceGateData in gatesData)
            {
                // Find the connection with the matching path
                if (processedTargets.Contains(sourceGateData.Gate) || !sectorGrouping.TryGetValue(sourceGateData.Gate.DestinationSectorName, out GateData[] availableGateData))
                {
                    continue;
                }

                GateData targetGateData = availableGateData
                    .FirstOrDefault(a => a.Zone.Gates.Any(b => b.SourcePath == sourceGateData.Gate.DestinationPath));
                if (targetGateData.Cluster == null) //Default
                {
                    invalidConnections.Add(sourceGateData);
                    continue;
                }

                _ = processedTargets.Add(targetGateData.Gate);

                if (!IsDlcClusterEnabled(targetGateData.Cluster))
                {
                    continue;
                }

                if (_visibleSectorsFromSearch.Count > 0 &&
                    (!_visibleSectorsFromSearch.Contains(targetGateData.Sector) ||
                    !_visibleSectorsFromSearch.Contains(sourceGateData.Sector)))
                {
                    continue;
                }

                yield return new GateConnection
                {
                    Source = sourceGateData,
                    Target = targetGateData
                };
            }

            if (invalidConnections.Count > 0)
            {
                _ = MessageBox.Show("Some of your gate connections are invalid, please double check them:\n- " +
                    string.Join("\n- ", invalidConnections.Select(a => a.Gate.ParentSectorName + " -> " + a.Gate.DestinationSectorName)));
            }
        }

        private static IEnumerable<GateData> CollectGateDataFromCluster(Cluster cluster)
        {
            // Calculate hex size and radius based on zoom and sector size
            float hexHeight = (float)(Math.Sqrt(3) * _hexSize) * _defaultZoom; // Height for flat-top hexes, applying zoom
            float hexRadius = (float)(hexHeight / Math.Sqrt(3)); // Recalculate radius based on zoom

            int sectorIndex = 0;
            foreach (Sector sector in cluster.Sectors)
            {
                // Collect the child hexagon points
                Hexagon childHexagon = cluster.Sectors.Count == 1 ? cluster.Hexagon : cluster.Hexagon.Children[sectorIndex];
                PointF hexCenter = GetHexCenter(childHexagon.Points);

                float correctHexRadius = cluster.Sectors.Count == 1 ? hexRadius : hexRadius / 2;

                foreach (Zone zone in sector.Zones)
                {
                    foreach (Gate gate in zone.Gates)
                    {
                        // Convert the zone position from world to screen space
                        Point realGatePos = new(zone.Position.X + gate.Position.X, zone.Position.Y + gate.Position.Y);
                        PointF gateScreenPosition = ConvertFromWorldCoordinate(realGatePos, sector.DiameterRadius, correctHexRadius);

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

        private static Color GetClusterOwnershipColor(Cluster cluster)
        {
            var ownerships = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var sector in cluster.Sectors)
            {
                if (sector == null) return MainForm.Instance.FactionColorMapping["None"];

                HashSet<string> factions = sector.Zones.SelectMany(a => a.Stations)
                    .Select(a => a.Owner)
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (sector.IsBaseGame)
                {
                    if (sector.Owner.Equals("None", StringComparison.OrdinalIgnoreCase))
                    {
                        if (factions.Count == 1)
                        {
                            ownerships.Add(factions.First());
                        }
                        else
                        {
                            ownerships.Add(sector.Owner);
                        }
                    }
                    else
                    {
                        if (factions.Count == 0 || (factions.Count == 1 && factions.First().Equals(sector.Owner, StringComparison.OrdinalIgnoreCase)))
                            ownerships.Add(sector.Owner);
                    }
                }
                else
                {
                    if (factions.Count == 1)
                    {
                        ownerships.Add(factions.First());
                    }
                }
            }

            if (ownerships.Count == 1)
                return FactionsForm.GetColorForFaction(ownerships.First());
            return MainForm.Instance.FactionColorMapping["None"];
        }

        private static Color GetSectorOwnershipColor(Sector sector)
        {
            if (sector == null) return MainForm.Instance.FactionColorMapping["None"];

            HashSet<string> factions = sector.Zones.SelectMany(a => a.Stations)
                .Select(a => a.Faction)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var color = MainForm.Instance.FactionColorMapping["None"];
            if (sector.IsBaseGame)
            {
                if (sector.Owner.Equals("None", StringComparison.OrdinalIgnoreCase))
                {
                    if (factions.Count == 1)
                    {
                        color = FactionsForm.GetColorForFaction(factions.First());
                    }
                }
                else
                {
                    if (factions.Count == 0 || (factions.Count == 1 && factions.First().Equals(sector.Owner, StringComparison.OrdinalIgnoreCase)))
                        color = FactionsForm.GetColorForFaction(sector.Owner);
                }
            }
            else
            {
                if (factions.Count == 1)
                {
                    color = FactionsForm.GetColorForFaction(factions.First());
                }
            }

            return color;
        }

        private void RenderClusters(PaintEventArgs e, KeyValuePair<(int, int), Hexagon> hex)
        {
            Cluster cluster = MainForm.Instance.AllClusters[hex.Key];
            if (cluster.Sectors.Count == 0)
            {
                return;
            }

            bool render = true;
            Color color = GetClusterOwnershipColor(cluster);

            if (_visibleSectorsFromSearch.Count > 0 && cluster.Sectors.Any(a => !_visibleSectorsFromSearch.Contains(a)))
            {
                render = false;
            }

            bool isMovingCluster = _movingCluster != null && _movingCluster == cluster;
            if (isMovingCluster)
            {
                color = Color.Yellow;
            }

            // Main hex outline
            int index = 0;
            using (Pen mainPen = new(color, 4))
            {
                if (cluster.Sectors.Count > 1 && !isMovingCluster)
                {
                    color = Color.Black;
                }

                // Fill with darker color
                if (render)
                {
                    using SolidBrush mainBrush = new(LerpColor(color, Color.Black, 0.85f));
                    e.Graphics.FillPolygon(mainBrush, hex.Value.Points);
                }

                // Draw child hex outlines
                foreach (Hexagon child in hex.Value.Children)
                {
                    Sector sector = cluster.Sectors[index];
                    Color ownerColor = GetSectorOwnershipColor(sector);

                    bool renderChild = true;
                    if (_visibleSectorsFromSearch.Count > 0 && !_visibleSectorsFromSearch.Contains(sector))
                    {
                        renderChild = false;
                    }

                    if (renderChild)
                    {
                        using Pen pen = new(ownerColor, 2);
                        using SolidBrush brush = new(LerpColor(ownerColor, Color.Black, 0.85f));

                        e.Graphics.FillPolygon(brush, child.Points);
                        e.Graphics.DrawPolygon(pen, child.Points);
                    }
                    index++;
                }

                if (render)
                {
                    // Draw edges
                    e.Graphics.DrawPolygon(mainPen, hex.Value.Points);
                }
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

            PointF hexCenter = GetHexCenter(hex.Value.Points);
            SizeF hexSize = GetHexSize(hex.Value.Points);

            // Don't render hex names if we're moving this cluster at the moment
            if (_movingCluster != null && _movingCluster == cluster)
            {
                SizeF textSize;
                // Scaled text font
                using Font fBold = new(Font.FontFamily, Font.Size * (_hexSize / 100), FontStyle.Bold);
                string text = $"(Right-click again to move)";
                textSize = e.Graphics.MeasureString(text, fBold);
                e.Graphics.DrawString(text, fBold, Brushes.White,
                    hexCenter.X - (textSize.Width / 2),
                    hexCenter.Y - (textSize.Height / 2));

                text = $"(Press ESC to cancel)";
                textSize = e.Graphics.MeasureString(text, fBold);
                e.Graphics.DrawString(text, fBold, Brushes.White,
                    hexCenter.X - (textSize.Width / 2),
                    hexCenter.Y + (textSize.Height / 2));

                // Don't render any other text
                return;
            }

            // Scaled text font
            using Font fBoldAndUnderlined = new(Font.FontFamily, Font.Size * (_hexSize / 100), FontStyle.Bold | FontStyle.Underline);

            // Draw child names
            int index = 0; // reset for name rendering
            foreach (Hexagon child in hex.Value.Children)
            {
                // Render child sector name
                Sector sector = cluster.Sectors[index];
                if (_visibleSectorsFromSearch.Count > 0 && !_visibleSectorsFromSearch.Contains(sector))
                {
                    index++;
                    continue;
                }

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
                if (_visibleSectorsFromSearch.Count > 0 && !_visibleSectorsFromSearch.Contains(cluster.Sectors.First()))
                {
                    return;
                }

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
                var selectedSector = GetSectorFromPosition(position, out _);
                if (selectedSector == null) return;

                MainForm.Instance.GateForm.Value.txtTargetSector.Text = selectedSector.Name;
                MainForm.Instance.GateForm.Value.txtTargetSectorLocation.Text = position.ToString() + $" [{_selectedChildHexIndex?.ToString() ?? "0"}]";
                MainForm.Instance.GateForm.Value.TargetSectorSelection = null; // Recalibrates automatically
            }
            else if (ClusterSectorSelection)
            {
                MainForm.Instance.ClusterForm.Value.TxtLocation.Text = position.ToString();
            }
            else
            {
                var selectedSector = GetSectorFromPosition(position, out var cluster);
                if (selectedSector == null) return;

                // Set the selected cluster/sector as hq space
                FactionForm.PreferredHqSpace = _selectedChildHexIndex != null ?
                    GetSectorMacro(cluster, selectedSector) : GetClusterMacro(cluster);
            }

            DeselectHex();
            Close();
        }

        private static string GetClusterMacro(Cluster cluster)
        {
            var clusterMacro = $"PREFIX_CL_c{cluster.Id:D3}_macro";
            if (cluster.IsBaseGame)
                clusterMacro = $"{cluster.BaseGameMapping}_macro";
            return clusterMacro;
        }

        private static string GetSectorMacro(Cluster cluster, Sector sector)
        {
            var sectorMacro = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
            if (cluster.IsBaseGame && sector.IsBaseGame)
            {
                sectorMacro = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_macro";
            }
            else if (cluster.IsBaseGame)
            {
                sectorMacro = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}_macro";
            }
            return sectorMacro;
        }

        private Sector GetSectorFromPosition((int, int) position, out Cluster cluster)
        {
            if (!MainForm.Instance.AllClusters.TryGetValue(position, out cluster))
            {
                _ = MessageBox.Show("Invalid cluster selected.");
                return null;
            }

            // Verify if cluster has atleast one sector and one zone
            if (cluster.Sectors.Count == 0)
            {
                _ = MessageBox.Show("The selected cluster must have atleast one sector.");
                return null;
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
                    return null;
                }
            }

            return selectedSector;
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

        private void BtnHideLegend_Click(object sender, EventArgs e)
        {
            var isHidden = BtnHideLegend.Text == "^";
            if (isHidden)
            {
                BtnHideLegend.Text = "V";
                BtnHideLegend.Font = new Font(BtnHideLegend.Font.FontFamily, 13, BtnHideLegend.Font.Style, GraphicsUnit.Pixel);
                LegendTree.Visible = true;
                LegendPanel.Height = _originalLegendPanelHeight;
                LegendPanel.Top = ClientSize.Height - LegendPanel.Height - 3;
                _legendWasMinimized = false;
            }
            else
            {
                BtnHideLegend.Text = "^";
                BtnHideLegend.Font = new Font(BtnHideLegend.Font.FontFamily, 15, BtnHideLegend.Font.Style, GraphicsUnit.Pixel);
                LegendTree.Visible = false;
                _originalLegendPanelHeight = LegendPanel.Height;
                LegendPanel.Height = 35;
                LegendPanel.Top = ClientSize.Height - LegendPanel.Height - 3;
                _legendWasMinimized = true;
            }
        }

        private void BtnHideOptions_Click(object sender, EventArgs e)
        {
            var isHidden = BtnHideOptions.Text == "V";
            if (isHidden)
            {
                BtnHideOptions.Text = "^";
                BtnHideOptions.Font = new Font(BtnHideOptions.Font.FontFamily, 14, BtnHideOptions.Font.Style, GraphicsUnit.Pixel);
                ControlPanel.Height = _originalControlPanelHeight;
                ControlPanel.Top = 12;
                _optionWasMinimzed = false;
            }
            else
            {
                BtnHideOptions.Text = "V";
                BtnHideOptions.Font = new Font(BtnHideOptions.Font.FontFamily, 11, BtnHideOptions.Font.Style, GraphicsUnit.Pixel);
                _originalControlPanelHeight = ControlPanel.Height;
                ControlPanel.Height = 24;
                ControlPanel.Top = 12;
                _optionWasMinimzed = true;
            }
        }

        private void SectorMapForm_Load(object sender, EventArgs e)
        {
            // Pre-hide boxes if stored in mem
            if (_optionWasMinimzed)
                BtnHideOptions.PerformClick();
            if (_legendWasMinimized)
                BtnHideLegend.PerformClick();
        }

        private void ChkShowStations_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ChkShowRegions_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        internal struct GateConnection
        {
            public GateData Source { get; set; }
            public GateData Target { get; set; }
        }

        internal struct GateData
        {
            public float ScreenX { get; set; }
            public float ScreenY { get; set; }
            public Gate Gate { get; set; }
            public Zone Zone { get; set; }
            public Sector Sector { get; set; }
            public Cluster Cluster { get; set; }
        }

        class IconData
        {
            public Cluster Cluster { get; set; }
            public Sector Sector { get; set; }
            public Image ImageLarge { get; set; }
            public Image ImageSmall { get; set; }
            public string Type { get; set; }
            public string Yield { get; set; }
        }

        private void LegendTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
