using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
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
        private const float _hexPadding = 10f;
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
        private static float _zoom = 0.45f;
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
        private static readonly Dictionary<string, bool> _mapOptionsSelected = [];

        private static bool _sectorMapFirstTimeOpen = true;
        private int _originalLegendPanelHeight, _originalControlPanelHeight;
        private bool _isHdExportRendering;

        private Image _factionLogicImageLarge;
        private Image _factionLogicImageSmall;
        private readonly Dictionary<Color, Dictionary<string, Image>> _cachedStationIconsLarge = [];
        private readonly Dictionary<Color, Dictionary<string, Image>> _cachedStationIconsSmall = [];
        private readonly Dictionary<Color, Image> cachedRegionImagesLarge = [];
        private readonly Dictionary<Color, Image> cachedRegionImagesSmall = [];

        private readonly Dictionary<string, List<object>> _legend = new(StringComparer.OrdinalIgnoreCase)
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
                    ("RawScrap", Color.Red),
                    ("RawKhaakScrap", Color.DarkRed)
                }
            },
            {
                "Stations", new List<object>
                {
                    "Factory",
                    "Defence",
                    "Wharf",
                    "Shipyard",
                    "Equipmentdock",
                    "Tradestation",
                    "Piratebase",
                    "Piratedock",
                    "Freeport"
                }
            },
            {
                "Others", new List<object>
                {
                    "Faction Logic Disabled"
                }
            },
            {
                "Factions", new List<object>
                {
                    ("Argon", "#001eff"),
                    ("Antigone", "#0073ff"),
                    ("Teladi", "#ddff00"),
                    ("Paranid", "#9500ff"),
                    ("HolyOrder", "#ff82cf"),
                    ("Terran", "#bdebff"),
                    ("Segaris", "#286d7a"),
                    ("Zyarth", "#b87811"),
                    ("FreeSplit", "#8c5906"),
                    ("Hatikvah", "#00f2ff"),
                    ("Xenon", "#ff0000"),
                    ("Riptide", "#031f57"),
                    ("Boron", "#00aeff"),
                    ("Vigor", "#a19958"),
                    ("Quettanauts", "#baa079"),
                    ("Yaki", "#ff00ea"),
                    ("Scaleplate", "#524e34"),
                    ("FallenSplit", "#6e300c"),
                    ("Ministry", "#546339"),
                    ("Alliance", "#3d1f4d"),
                    ("Buccaneers", "#361f0d"),
                    ("Player", "#00ff15"),
                    ("Khaak", "#d16fba")
                }
            }
        };

        private static readonly Dictionary<string, double> _yieldDensities = new(StringComparer.OrdinalIgnoreCase)
        {
            ["verylow"] = 0.06,
            ["low"] = 0.6,
            ["medium"] = 6,
            ["high"] = 60,
            ["veryhigh"] = 3600
        };

        private static readonly Dictionary<string, Image> _imageMap = new(StringComparer.OrdinalIgnoreCase);

        private static bool _optionWasMinimzed = false, _legendWasMinimized = false;

        private readonly List<Sector> _availableSearchSectors = [];
        private readonly HashSet<Sector> _visibleSectorsFromSearch = [];
        private const int _invalidGatePreviewLimit = 15;

        public enum MapOption
        {
            Keep_Window_Open,
            Show_Vanilla_Sectors,
            Show_Custom_Sectors,
            Show_Vanilla_Gates,
            Show_Custom_Gates,
            Show_Coordinates,
            Show_Vanilla_Regions,
            Show_Custom_Regions,
            Visualize_Regions,
            Show_Vanilla_Stations,
            Show_Custom_Stations
        }

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

            // Init default map options
            int mapOptionIndex = 0;
            foreach (var mapOption in MapOptionsListBox.Items.OfType<string>().ToArray())
            {
                if (!_mapOptionsSelected.TryGetValue(mapOption, out var selected))
                {
                    // If not yet initialized, it will be by default selected except "show coordinates"
                    _mapOptionsSelected[mapOption] = selected =
                        !mapOption.Equals("Show Coordinates", StringComparison.OrdinalIgnoreCase) &&
                        !mapOption.Equals("Visualize Regions", StringComparison.OrdinalIgnoreCase) &&
                        !mapOption.Equals("Keep Window Open", StringComparison.OrdinalIgnoreCase);
                }

                MapOptionsListBox.SetItemChecked(mapOptionIndex, selected);
                mapOptionIndex++;
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

        public static bool IsMapOptionChecked(MapOption mapOption)
        {
            var index = (int)mapOption;
            _mapOptionsSelected.TryGetValue(mapOption.ToString().Replace("_", " "), out var value);
            return value;
        }

        private void SectorMapForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        private void SearchRender(List<Sector> data)
        {
            _visibleSectorsFromSearch.Clear();
            if (!string.IsNullOrEmpty(TxtSearch.Text))
            {
                foreach (var item in data)
                    _visibleSectorsFromSearch.Add(item);
            }
            Invalidate();
        }

        private static readonly Dictionary<Image, Dictionary<Color, Image>> _tintCache = [];
        private static Image GetTintFromCache(Image image, Color color)
        {
            if (!_tintCache.TryGetValue(image, out var cache))
                _tintCache[image] = cache = [];
            if (!cache.TryGetValue(color, out var value))
                cache[color] = value = image.CopyAsTint(color);
            return value;
        }

        private void SetupLegendTree()
        {
            LegendPanel.Top = ClientSize.Height - LegendPanel.Height - 3;
            LegendTree.Nodes.Clear();
            LegendTree.DrawNode += LegendTree_DrawNode;
            LegendTree.ImageList = new ImageList
            {
                ImageSize = new Size(16, 16)
            };

            var regionImage = GetIconFromStore("region_resource");
            if (FactionsForm.AllCustomFactions.Count > 0)
            {
                _legend["Custom Factions"] = new List<object>(FactionsForm.AllCustomFactions.Values);
                foreach (Faction faction in _legend["Custom Factions"].Cast<Faction>())
                    LegendTree.ImageList.Images.Add(faction.Id, GetTintFromCache(regionImage, faction.Color));
            }
            else
            {
                _legend.Remove("Custom Factions");
            }

            foreach (var station in _legend["stations"])
            {
                LegendTree.ImageList.Images.Add(station.ToString(), GetIconFromStore(station.ToString().ToLower()));
            }

            // Don't show vanilla, if no sector contains vanilla factions
            if (MainForm.Instance.AllClusters.Values.SelectMany(a => a.Sectors).All(a => string.IsNullOrWhiteSpace(a.Owner)))
                _legend.Remove("factions");

            // Init region images
            foreach (var resource in _legend["resources"])
            {
                var (name, color) = ((string name, Color color))resource;
                LegendTree.ImageList.Images.Add(name, GetTintFromCache(regionImage, color));
            }
            if (_legend.TryGetValue("factions", out var factions))
            {
                foreach (var faction in factions)
                {
                    var (name, colorHex) = ((string, string))faction;
                    LegendTree.ImageList.Images.Add(name, GetTintFromCache(regionImage, colorHex.HexToColor()));
                }
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
                    if (legendEntry.Key == "Others")
                    {
                        var entryStr = entry as string;
                        childNode = new TreeNode(entryStr);
                        if (entryStr.Equals("Faction Logic Disabled", StringComparison.OrdinalIgnoreCase))
                            childNode.ImageKey = entryStr;
                    }
                    else if (legendEntry.Key == "Factions")
                    {
                        var (name, _) = ((string, string))entry;
                        childNode = new TreeNode(name)
                        {
                            ImageKey = name
                        };
                    }
                    else if (legendEntry.Key == "Custom Factions")
                    {
                        var faction = (Faction)entry;
                        childNode = new TreeNode(faction.Name)
                        {
                            ImageKey = faction.Id
                        };
                    }
                    else if (legendEntry.Key == "Stations")
                    {
                        var entryStr = entry as string;
                        childNode = new TreeNode(entryStr)
                        {
                            ImageKey = entryStr
                        };
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
            if (iconName.Equals("piratedock") || iconName.Equals("freeport"))
                iconName = "piratebase";

            if (!_imageMap.TryGetValue(iconName, out var image))
            {
                var path = Path.Combine(Application.StartupPath, $"Data/Icons/{iconName}.png");
                if (File.Exists(path))
                {
                    _imageMap[iconName] = image = Image.FromFile(path);
                }
                else
                {
                    throw new Exception($"Cannot find icon \"{iconName}.png\".");
                }
            }
            return image;
        }

        private void SectorMapForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && _movingCluster != null)
            {
                _movingCluster = null;
                ResetInteractionCursor();
                Invalidate();
            }

            if (e.KeyCode == Keys.Escape && _movingSector != null)
            {
                _movingSector.CustomOffset = _movingSectorOriginalOffset;
                SectorForm.DetermineSectorOffset(_movingSectorCluster, _movingSector);
                _movingSector = null;
                _movingSectorCluster = null;
                _movingSectorChildIndex = null;
                _movingSectorOriginalOffset = null;
                ResetInteractionCursor();
                Invalidate();
            }

            if (e.KeyCode == Keys.Escape && _movingHighway != null)
            {
                _movingHighway.Gate.Position = _movingHighway.OriginalGatePosition;
                _movingHighway = null;
                ResetInteractionCursor();
                Invalidate();
            }
        }

        private Cluster _movingCluster = null;
        private bool _draggingClusterMove = false;
        private Cluster _movingSectorCluster = null;
        private Sector _movingSector = null;
        private int? _movingSectorChildIndex = null;
        private bool _draggingSectorMove = false;
        private Point? _movingSectorOriginalOffset = null;
        private DateTime _lastSectorDragPreviewUpdate = DateTime.MinValue;
        private HighwayEndpointDragState _movingHighway = null;

        private void ResetInteractionCursor()
        {
            Cursor = Cursors.Default;
        }

        private void SetDragCursor()
        {
            Cursor = Cursors.SizeAll;
        }

        private void SectorMapForm_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void CompleteSectorMove(PointF adjustedMousePos)
        {
            if (_movingSector == null || _movingSectorCluster == null)
                return;

            _movingSector = null;
            _movingSectorCluster = null;
            _movingSectorChildIndex = null;
            _movingSectorOriginalOffset = null;
            ResetInteractionCursor();

            RebuildMapGeometry(snapChildSectors: false, avoidChildCollisions: false, snapTravelNodes: true);
        }

        private void BtnAutoCorrectLayout_Click(object sender, EventArgs e)
        {
            ApplyAutoCorrection();
        }

        private void ApplyAutoCorrection()
        {
            _movingCluster = null;
            _draggingClusterMove = false;
            _movingSector = null;
            _movingSectorCluster = null;
            _movingSectorChildIndex = null;
            _movingSectorOriginalOffset = null;
            _draggingSectorMove = false;
            _movingHighway = null;
            ResetInteractionCursor();

            RebuildMapGeometry(snapChildSectors: false, avoidChildCollisions: false, snapTravelNodes: false);
        }

        private Cluster GetClusterAtMousePos(PointF mousePos, out (int x, int y)? pos)
        {
            pos = null;
            foreach (var hex in _hexagons.Values)
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

        private bool TryGetSectorAtMousePos(PointF mousePos, out Cluster cluster, out Sector sector, out int sectorIndex)
        {
            foreach (var hex in _hexagons.Values)
            {
                if (!MainForm.Instance.AllClusters.TryGetValue(hex.Position, out cluster))
                    continue;

                if (cluster.Sectors.Count <= 1 || hex.Children.Count == 0)
                    continue;

                for (int index = 0; index < hex.Children.Count && index < cluster.Sectors.Count; index++)
                {
                    if (IsPointInPolygon(hex.Children[index].Points, mousePos))
                    {
                        sector = cluster.Sectors[index];
                        sectorIndex = index;
                        return true;
                    }
                }
            }

            cluster = null;
            sector = null;
            sectorIndex = -1;
            return false;
        }

        private void UpdateSectorDrag(PointF mousePos)
        {
            if (_movingSector == null || _movingSectorCluster == null)
                return;

            PointF snappedCenter = GetNearestChildSectorSnapCenter(_movingSectorCluster, _movingSectorChildIndex.Value, mousePos);
            Point offset = ConvertPointInClusterHexToCustomOffset(_movingSectorCluster, snappedCenter);

            _movingSector.CustomOffset = offset;
            SectorForm.DetermineSectorOffset(_movingSectorCluster, _movingSector);
        }

        private PointF GetNearestChildSectorSnapCenter(Cluster cluster, int movingSectorIndex, PointF mousePos)
        {
            bool includeOuterAnchors = cluster.Sectors.Count == 4;
            PointF[] candidates = SectorMapInteractionRules.UseCanonicalSectorDragLayout(cluster.Sectors.Count)
                ? GetAutocorrectChildSectorSnapCenters(cluster, includeOuterAnchors).ToArray()
                : GetChildSectorSnapCenters(cluster, includeOuterAnchors: true).ToArray();
            List<PointF> occupiedCenters = cluster.Sectors
                .Where((_, index) => index != movingSectorIndex)
                .Select(a => GetCurrentSectorCenter(cluster, a))
                .ToList();

            int selectedIndex = -1;
            float selectedDistance = float.MaxValue;
            for (int i = 0; i < candidates.Length; i++)
            {
                if (occupiedCenters.Any(a => Distance(a, candidates[i]) < 0.01f))
                    continue;

                float distance = Distance(mousePos, candidates[i]);
                if (distance < selectedDistance)
                {
                    selectedDistance = distance;
                    selectedIndex = i;
                }
            }

            if (selectedIndex == -1)
                return GetCurrentSectorCenter(cluster, _movingSector);

            return candidates[selectedIndex];
        }

        private IEnumerable<PointF> GetChildSectorSnapCenters(Cluster cluster, bool includeOuterAnchors)
        {
            PointF[] parentHex = cluster.Hexagon.Points;
            PointF parentCenter = GetHexCenter(parentHex);

            if (cluster.Sectors.Count <= 3)
            {
                for (int index = 0; index < parentHex.Length; index++)
                {
                    PointF vertex = parentHex[index];
                    PointF nextVertex = parentHex[(index + 1) % parentHex.Length];
                    PointF sideMidpoint = new(
                        (vertex.X + nextVertex.X) / 2f,
                        (vertex.Y + nextVertex.Y) / 2f);

                    yield return LerpPoint(parentCenter, vertex, 0.5f);
                    yield return LerpPoint(parentCenter, sideMidpoint, 0.5f);
                }

                yield break;
            }

            if (cluster.Sectors.Count == 4 && includeOuterAnchors)
            {
                const float vertexRatio = 3f / 5f;
                const float sideRatio = 3f / 5f;

                for (int index = 0; index < parentHex.Length; index++)
                {
                    PointF vertex = parentHex[index];
                    PointF nextVertex = parentHex[(index + 1) % parentHex.Length];
                    PointF sideMidpoint = new(
                        (vertex.X + nextVertex.X) / 2f,
                        (vertex.Y + nextVertex.Y) / 2f);

                    yield return LerpPoint(parentCenter, vertex, vertexRatio);
                    yield return LerpPoint(parentCenter, sideMidpoint, sideRatio);
                }

                yield break;
            }

            float[] ratios = includeOuterAnchors
                ? [0.5f, 3f / 5f, 2f / 3f]
                : [0.5f];

            foreach (float ratio in ratios)
            {
                for (int index = 0; index < parentHex.Length; index++)
                {
                    PointF vertex = parentHex[index];
                    PointF nextVertex = parentHex[(index + 1) % parentHex.Length];
                    PointF sideMidpoint = new(
                        (vertex.X + nextVertex.X) / 2f,
                        (vertex.Y + nextVertex.Y) / 2f);

                    yield return LerpPoint(parentCenter, vertex, ratio);
                    yield return LerpPoint(parentCenter, sideMidpoint, ratio);
                }
            }
        }

        private IEnumerable<PointF> GetAutocorrectChildSectorSnapCenters(Cluster cluster, bool includeOuterAnchors)
        {
            PointF[] parentHex = cluster.Hexagon.Points;
            PointF parentCenter = GetHexCenter(parentHex);

            if (cluster.Sectors.Count == 2)
            {
                PointF topLeft = parentHex.OrderBy(a => a.Y).ThenBy(a => a.X).First();
                PointF topRight = parentHex.OrderBy(a => a.Y).ThenByDescending(a => a.X).First();
                PointF bottomLeft = parentHex.OrderByDescending(a => a.Y).ThenBy(a => a.X).First();
                PointF bottomRight = parentHex.OrderByDescending(a => a.Y).ThenByDescending(a => a.X).First();

                PointF up = new((topLeft.X + topRight.X) / 2f, (topLeft.Y + topRight.Y) / 2f);
                PointF down = new((bottomLeft.X + bottomRight.X) / 2f, (bottomLeft.Y + bottomRight.Y) / 2f);

                yield return LerpPoint(parentCenter, up, 0.5f);
                yield return LerpPoint(parentCenter, down, 0.5f);
                yield break;
            }

            if (cluster.Sectors.Count == 3)
            {
                PointF bottomLeft = parentHex.OrderByDescending(a => a.Y).ThenBy(a => a.X).First();
                PointF right = parentHex.OrderByDescending(a => a.X).First();
                PointF topLeft = parentHex.OrderBy(a => a.Y).ThenBy(a => a.X).First();

                yield return LerpPoint(parentCenter, topLeft, 0.5f);
                yield return LerpPoint(parentCenter, right, 0.5f);
                yield return LerpPoint(parentCenter, bottomLeft, 0.5f);
                yield break;
            }

            if (cluster.Sectors.Count == 4 && includeOuterAnchors)
            {
                PointF left = parentHex.OrderBy(a => a.X).First();
                PointF right = parentHex.OrderByDescending(a => a.X).First();
                PointF topLeft = parentHex.OrderBy(a => a.Y).ThenBy(a => a.X).First();
                PointF topRight = parentHex.OrderBy(a => a.Y).ThenByDescending(a => a.X).First();
                PointF bottomLeft = parentHex.OrderByDescending(a => a.Y).ThenBy(a => a.X).First();
                PointF bottomRight = parentHex.OrderByDescending(a => a.Y).ThenByDescending(a => a.X).First();

                PointF up = new((topLeft.X + topRight.X) / 2f, (topLeft.Y + topRight.Y) / 2f);
                PointF down = new((bottomLeft.X + bottomRight.X) / 2f, (bottomLeft.Y + bottomRight.Y) / 2f);

                yield return LerpPoint(parentCenter, up, 3f / 5f);
                yield return LerpPoint(parentCenter, down, 3f / 5f);
                yield return LerpPoint(parentCenter, right, 3f / 5f);
                yield return LerpPoint(parentCenter, left, 3f / 5f);
                yield break;
            }

            foreach (PointF point in GetChildSectorSnapCenters(cluster, includeOuterAnchors))
                yield return point;
        }

        private PointF GetCurrentSectorCenter(Cluster cluster, Sector sector)
        {
            PointF[] parentHex = cluster.Hexagon.Points;
            PointF parentCenter = GetHexCenter(parentHex);
            float parentRadius = GetHexRadius(parentHex);

            if (sector.CustomOffset.HasValue)
            {
                (float x, float y) = ConvertCustomOffsetToChildCenter(sector.CustomOffset.Value, parentRadius);
                return new PointF(parentCenter.X + x, parentCenter.Y + y);
            }

            int index = cluster.Sectors.IndexOf(sector);
            return GetHexCenter(cluster.Hexagon.Children[index].Points);
        }

        private static PointF ClampPointInsideHex(PointF[] polygon, PointF point)
        {
            if (IsPointInPolygon(polygon, point))
                return point;

            PointF center = GetHexCenter(polygon);
            PointF low = center;
            PointF high = point;

            for (int i = 0; i < 20; i++)
            {
                PointF mid = new((low.X + high.X) / 2f, (low.Y + high.Y) / 2f);
                if (IsPointInPolygon(polygon, mid))
                    low = mid;
                else
                    high = mid;
            }

            return low;
        }

        private PointF ClampChildHexCenterInsideParent(Cluster cluster, int childIndex, PointF desiredCenter)
        {
            PointF[] parentPolygon = cluster.Hexagon.Points;
            PointF[] childPolygon = cluster.Hexagon.Children[childIndex].Points;
            PointF childCenter = GetHexCenter(childPolygon);
            const float insetMargin = 0f;

            PointF[] relativePoints = childPolygon
                .Select(a => new PointF(a.X - childCenter.X, a.Y - childCenter.Y))
                .ToArray();

            if (IsTranslatedChildInsideParent(parentPolygon, relativePoints, desiredCenter, insetMargin))
                return desiredCenter;

            PointF low = childCenter;
            PointF high = desiredCenter;

            for (int i = 0; i < 24; i++)
            {
                PointF mid = new((low.X + high.X) / 2f, (low.Y + high.Y) / 2f);
                if (IsTranslatedChildInsideParent(parentPolygon, relativePoints, mid, insetMargin))
                    low = mid;
                else
                    high = mid;
            }

            return low;
        }

        private static bool IsTranslatedChildInsideParent(PointF[] parentPolygon, PointF[] childRelativePoints, PointF center, float insetMargin)
        {
            foreach (PointF point in childRelativePoints)
            {
                PointF translated = new(center.X + point.X, center.Y + point.Y);
                if (!IsPointInPolygon(parentPolygon, translated) ||
                    DistanceToPolygonEdges(parentPolygon, translated) < insetMargin)
                    return false;
            }

            return true;
        }

        private static float DistanceToPolygonEdges(PointF[] polygon, PointF point)
        {
            float bestDistance = float.MaxValue;

            for (int i = 0; i < polygon.Length; i++)
            {
                PointF a = polygon[i];
                PointF b = polygon[(i + 1) % polygon.Length];
                bestDistance = Math.Min(bestDistance, DistanceToSegment(point, a, b));
            }

            return bestDistance;
        }

        private int? GetNearestSectorIndexInCluster(PointF mousePos, Cluster cluster, int? excludeIndex = null)
        {
            if (cluster?.Hexagon?.Children == null || cluster.Hexagon.Children.Count == 0)
                return null;

            if (!IsPointInPolygon(cluster.Hexagon.Points, mousePos))
                return null;

            int? bestIndex = null;
            float bestDistance = float.MaxValue;

            for (int index = 0; index < cluster.Hexagon.Children.Count && index < cluster.Sectors.Count; index++)
            {
                if (excludeIndex != null && index == excludeIndex.Value)
                    continue;

                PointF center = GetHexCenter(cluster.Hexagon.Children[index].Points);
                float distance = Distance(mousePos, center);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = index;
                }
            }

            return bestIndex;
        }

        public void Reset(bool resetLegendTree = true)
        {
            _movingCluster = null;
            _draggingClusterMove = false;
            _movingSector = null;
            _movingSectorCluster = null;
            _movingSectorChildIndex = null;
            _draggingSectorMove = false;
            _movingHighway = null;
            _baseGameClusters = MainForm.Instance.AllClusters
                .Where(a => a.Value.IsBaseGame)
                .ToDictionary(a => a.Key, a => a.Value);

            _customClusters = [.. MainForm.Instance.AllClusters.Values.Where(a => !a.IsBaseGame)];

            // Setup all data
            _availableSearchSectors.Clear();
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

            if (resetLegendTree)
                SetupLegendTree();

            RebuildMapGeometry(snapChildSectors: true, avoidChildCollisions: true, snapTravelNodes: true);
        }

        private void RebuildMapGeometry(bool snapChildSectors, bool avoidChildCollisions, bool snapTravelNodes)
        {
            GenerateHexagons();
            AssignClusterHexagons();

            if (snapChildSectors)
            {
                SnapChildSectorsToParentBounds(avoidChildCollisions);
                GenerateHexagons();
                AssignClusterHexagons();
            }

            if (snapTravelNodes)
                SnapHighwayNodesToParentBounds();

            Invalidate();
        }

        private void AssignClusterHexagons()
        {
            foreach (var cluster in MainForm.Instance.AllClusters.Values)
            {
                if (_hexagons.TryGetValue((cluster.Position.X, cluster.Position.Y), out var hex))
                {
                    cluster.Hexagon = hex;
                }
            }
        }

        private void SnapHighwayNodesToParentBounds()
        {
            foreach (GateConnection connection in GetVisibleGateConnections())
            {
                if (!connection.Source.Gate.IsAcceleratorNode && !connection.Source.Gate.IsInterSectorGate &&
                    !connection.Target.Gate.IsAcceleratorNode && !connection.Target.Gate.IsInterSectorGate)
                {
                    continue;
                }

                PointF[] sourceSnapPoints = GetTravelNodeSnapPoints(connection.Source.Cluster, connection.Source.Sector).ToArray();
                PointF[] targetSnapPoints = GetTravelNodeSnapPoints(connection.Target.Cluster, connection.Target.Sector).ToArray();

                Point sourceRealGatePos = new(connection.Source.Zone.Position.X + connection.Source.Gate.Position.X, connection.Source.Zone.Position.Y + connection.Source.Gate.Position.Y);
                Point targetRealGatePos = new(connection.Target.Zone.Position.X + connection.Target.Gate.Position.X, connection.Target.Zone.Position.Y + connection.Target.Gate.Position.Y);

                PointF sourceSectorHexCenter = GetSectorHexCenter(connection.Source.Cluster, connection.Source.Sector);
                float sourceSectorHexRadius = GetSectorHexRadius(connection.Source.Cluster);
                PointF targetSectorHexCenter = GetSectorHexCenter(connection.Target.Cluster, connection.Target.Sector);
                float targetSectorHexRadius = GetSectorHexRadius(connection.Target.Cluster);

                PointF sourceCurrentScreen = ConvertFromWorldCoordinate(sourceRealGatePos, connection.Source.Sector.DiameterRadius, sourceSectorHexRadius);
                sourceCurrentScreen.X += sourceSectorHexCenter.X;
                sourceCurrentScreen.Y += sourceSectorHexCenter.Y;

                PointF targetCurrentScreen = ConvertFromWorldCoordinate(targetRealGatePos, connection.Target.Sector.DiameterRadius, targetSectorHexRadius);
                targetCurrentScreen.X += targetSectorHexCenter.X;
                targetCurrentScreen.Y += targetSectorHexCenter.Y;

                PointF selectedSource = sourceCurrentScreen;
                PointF selectedTarget = targetCurrentScreen;
                float bestScore = float.MaxValue;

                foreach (PointF sourceCandidate in sourceSnapPoints)
                {
                    foreach (PointF targetCandidate in targetSnapPoints)
                    {
                        float score = Distance(sourceCandidate, targetCandidate);
                        if (score < bestScore)
                        {
                            bestScore = score;
                            selectedSource = sourceCandidate;
                            selectedTarget = targetCandidate;
                        }
                    }
                }

                Point sourceWorldPoint = ConvertToWorldCoordinate(
                    new PointF(selectedSource.X - sourceSectorHexCenter.X, selectedSource.Y - sourceSectorHexCenter.Y),
                    connection.Source.Sector.DiameterRadius,
                    sourceSectorHexRadius);
                connection.Source.Gate.Position = new Point(sourceWorldPoint.X - connection.Source.Zone.Position.X, sourceWorldPoint.Y - connection.Source.Zone.Position.Y);

                Point targetWorldPoint = ConvertToWorldCoordinate(
                    new PointF(selectedTarget.X - targetSectorHexCenter.X, selectedTarget.Y - targetSectorHexCenter.Y),
                    connection.Target.Sector.DiameterRadius,
                    targetSectorHexRadius);
                connection.Target.Gate.Position = new Point(targetWorldPoint.X - connection.Target.Zone.Position.X, targetWorldPoint.Y - connection.Target.Zone.Position.Y);
            }
        }

        private static int GetSnapIndex(PointF point, PointF[] snapPoints)
        {
            int bestIndex = 0;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < snapPoints.Length; i++)
            {
                float distance = Distance(point, snapPoints[i]);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private IEnumerable<PointF> GetTravelNodeSnapPoints(Cluster cluster, Sector sector)
        {
            PointF[] polygon = cluster.Sectors.Count == 1
                ? cluster.Hexagon.Points
                : cluster.Hexagon.Children[cluster.Sectors.IndexOf(sector)].Points;

            PointF center = GetHexCenter(polygon);
            for (int i = 0; i < polygon.Length; i++)
            {
                PointF vertex = polygon[i];
                PointF midpoint = new(
                    (polygon[i].X + polygon[(i + 1) % polygon.Length].X) / 2f,
                    (polygon[i].Y + polygon[(i + 1) % polygon.Length].Y) / 2f);

                yield return LerpPoint(center, vertex, 0.75f);
                yield return LerpPoint(center, midpoint, 0.75f);
            }
        }

        private static PointF LerpPoint(PointF from, PointF to, float amount)
        {
            return new PointF(
                from.X + ((to.X - from.X) * amount),
                from.Y + ((to.Y - from.Y) * amount));
        }

        private void SnapChildSectorsToParentBounds(bool avoidCollisions)
        {
            foreach (var cluster in MainForm.Instance.AllClusters.Values)
            {
                if (cluster.Sectors.Count <= 1 || cluster.Hexagon == null)
                    continue;

                bool includeOuterAnchors = cluster.Sectors.Count == 4;
                const int preferredSnapStep = 1;
                PointF[] snapPoints = GetAutocorrectChildSectorSnapCenters(cluster, includeOuterAnchors).ToArray();
                PointF[] fallbackSnapPoints = includeOuterAnchors
                    ? GetAutocorrectChildSectorSnapCenters(cluster, includeOuterAnchors: false).ToArray()
                    : snapPoints;
                HashSet<int> usedIndices = avoidCollisions ? [] : null;
                PointF[] parentPolygon = cluster.Hexagon.Points;
                PointF parentCenter = GetHexCenter(parentPolygon);
                PointF[][] childRelativePolygons = cluster.Hexagon.Children
                    .Take(cluster.Sectors.Count)
                    .Select(a =>
                    {
                        PointF center = GetHexCenter(a.Points);
                        return a.Points.Select(b => new PointF(b.X - center.X, b.Y - center.Y)).ToArray();
                    })
                    .ToArray();
                List<PointF[]> placedPolygons = [];
                List<PointF> placedCenters = [];
                List<int> sectorOrder = Enumerable.Range(0, cluster.Sectors.Count)
                    .OrderBy(a => Distance(GetCurrentSectorCenter(cluster, cluster.Sectors[a]), parentCenter))
                    .ThenBy(a => a)
                    .ToList();

                for (int orderIndex = 0; orderIndex < sectorOrder.Count; orderIndex++)
                {
                    int sectorIndex = sectorOrder[orderIndex];
                    Sector sector = cluster.Sectors[sectorIndex];
                    PointF currentCenter = GetCurrentSectorCenter(cluster, sector);
                    int startIndex = orderIndex % snapPoints.Length;
                    PointF selectedCenter = snapPoints[startIndex];
                    PointF[] selectedPolygon = TranslatePolygon(childRelativePolygons[sectorIndex], selectedCenter);
                    if (!TryFindChildSectorPlacement(
                        snapPoints,
                        startIndex,
                        childRelativePolygons[sectorIndex],
                        parentPolygon,
                        placedCenters,
                        placedPolygons,
                        usedIndices,
                        avoidCollisions,
                        preferredSnapStep,
                        out selectedCenter,
                        out selectedPolygon,
                        out int selectedIndex))
                    {
                        int fallbackStartIndex = orderIndex % fallbackSnapPoints.Length;
                        if (TryFindChildSectorPlacement(
                            fallbackSnapPoints,
                            fallbackStartIndex,
                            childRelativePolygons[sectorIndex],
                            parentPolygon,
                            placedCenters,
                            placedPolygons,
                            usedIndices: null,
                            avoidCollisions,
                            preferredSnapStep,
                            out PointF fallbackCenter,
                            out PointF[] fallbackPolygon,
                            out _))
                        {
                            selectedCenter = fallbackCenter;
                            selectedPolygon = fallbackPolygon;
                        }
                    }

                    int selectedPrimaryIndex = Array.FindIndex(snapPoints, a => Distance(a, selectedCenter) < 0.01f);
                    if (selectedPrimaryIndex >= 0)
                        usedIndices?.Add(selectedPrimaryIndex);

                    placedCenters.Add(selectedCenter);
                    placedPolygons.Add(selectedPolygon);
                    sector.CustomOffset = ConvertPointInClusterHexToCustomOffset(cluster, selectedCenter);
                    SectorForm.DetermineSectorOffset(cluster, sector);
                }
            }
        }

        private static bool TryFindChildSectorPlacement(
            PointF[] snapPoints,
            int startIndex,
            PointF[] childRelativePolygon,
            PointF[] parentPolygon,
            List<PointF> placedCenters,
            List<PointF[]> placedPolygons,
            HashSet<int> usedIndices,
            bool avoidCollisions,
            int preferredSnapStep,
            out PointF selectedCenter,
            out PointF[] selectedPolygon,
            out int selectedIndex)
        {
            selectedIndex = startIndex;
            selectedCenter = snapPoints[startIndex];
            selectedPolygon = TranslatePolygon(childRelativePolygon, selectedCenter);

            foreach (int candidateIndex in GetAutocorrectCandidateOrder(snapPoints.Length, startIndex, preferredSnapStep))
            {
                PointF candidateCenter = snapPoints[candidateIndex];
                PointF[] candidatePolygon = TranslatePolygon(childRelativePolygon, candidateCenter);

                bool indexOccupied = avoidCollisions && usedIndices != null && usedIndices.Contains(candidateIndex);
                bool centerOccupied = avoidCollisions && placedCenters.Any(a => Distance(a, candidateCenter) < 0.01f);
                bool outsideParent = !IsTranslatedChildInsideParent(parentPolygon, childRelativePolygon, candidateCenter, 0f);
                bool overlapsExisting = avoidCollisions && placedPolygons.Any(a => DoPolygonsOverlap(a, candidatePolygon));

                if (!indexOccupied && !centerOccupied && !outsideParent && !overlapsExisting)
                {
                    selectedIndex = candidateIndex;
                    selectedCenter = candidateCenter;
                    selectedPolygon = candidatePolygon;
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<int> GetAutocorrectCandidateOrder(int snapCount, int startIndex, int preferredStep)
        {
            HashSet<int> yielded = [];

            int normalizedStep = preferredStep <= 1 ? 1 : preferredStep;
            if (normalizedStep > 1)
            {
                for (int offset = 0; offset < snapCount; offset += normalizedStep)
                {
                    int index = (startIndex + offset) % snapCount;
                    if (yielded.Add(index))
                        yield return index;
                }
            }

            for (int offset = 0; offset < snapCount; offset++)
            {
                int index = (startIndex + offset) % snapCount;
                if (yielded.Add(index))
                    yield return index;
            }
        }

        private static PointF[] TranslatePolygon(PointF[] relativePolygon, PointF center)
        {
            return relativePolygon.Select(a => new PointF(center.X + a.X, center.Y + a.Y)).ToArray();
        }

        private static bool DoPolygonsOverlap(PointF[] polygonA, PointF[] polygonB)
        {
            for (int i = 0; i < polygonA.Length; i++)
            {
                PointF a1 = polygonA[i];
                PointF a2 = polygonA[(i + 1) % polygonA.Length];

                for (int j = 0; j < polygonB.Length; j++)
                {
                    PointF b1 = polygonB[j];
                    PointF b2 = polygonB[(j + 1) % polygonB.Length];
                    if (DoSegmentsIntersect(a1, a2, b1, b2))
                        return true;
                }
            }

            return IsPointInPolygon(polygonA, polygonB[0]) || IsPointInPolygon(polygonB, polygonA[0]);
        }

        private static bool DoSegmentsIntersect(PointF a1, PointF a2, PointF b1, PointF b2)
        {
            float o1 = CrossProduct(a1, a2, b1);
            float o2 = CrossProduct(a1, a2, b2);
            float o3 = CrossProduct(b1, b2, a1);
            float o4 = CrossProduct(b1, b2, a2);

            if ((o1 > 0f && o2 < 0f || o1 < 0f && o2 > 0f) &&
                (o3 > 0f && o4 < 0f || o3 < 0f && o4 > 0f))
                return true;

            return (Math.Abs(o1) < 0.001f && IsPointOnSegment(a1, a2, b1)) ||
                   (Math.Abs(o2) < 0.001f && IsPointOnSegment(a1, a2, b2)) ||
                   (Math.Abs(o3) < 0.001f && IsPointOnSegment(b1, b2, a1)) ||
                   (Math.Abs(o4) < 0.001f && IsPointOnSegment(b1, b2, a2));
        }

        private static float CrossProduct(PointF a, PointF b, PointF c)
        {
            return ((b.X - a.X) * (c.Y - a.Y)) - ((b.Y - a.Y) * (c.X - a.X));
        }

        private static bool IsPointOnSegment(PointF start, PointF end, PointF point)
        {
            return point.X >= Math.Min(start.X, end.X) - 0.001f &&
                   point.X <= Math.Max(start.X, end.X) + 0.001f &&
                   point.Y >= Math.Min(start.Y, end.Y) - 0.001f &&
                   point.Y <= Math.Max(start.Y, end.Y) + 0.001f;
        }

        private void HandleMouseWheel(object sender, MouseEventArgs e)
        {
            const float zoomFactor = 1.2f; // 20% zoom per wheel step
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
            if (e.Button == MouseButtons.Right)
            {
                if (_visibleSectorsFromSearch.Count > 0)
                {
                    _ = MessageBox.Show("Cannot move clusters, sectors, or highways while a search filter is set.");
                    return;
                }

                PointF adjustedMousePos = new(
                    (e.Location.X - _offset.X) / _zoom,
                    (e.Location.Y - _offset.Y) / _zoom
                );

                if (TryGetHighwayAtMousePos(e.Location, out HighwayEndpointDragState highwayNode))
                {
                    _movingHighway = highwayNode;
                    _movingHighway.StartMouseScreen = adjustedMousePos;
                    SetDragCursor();
                    Invalidate();
                    return;
                }

                if (TryGetSectorAtMousePos(adjustedMousePos, out Cluster sectorCluster, out Sector sector, out int sectorIndex))
                {
                    _movingSector = sector;
                    _movingSectorCluster = sectorCluster;
                    _movingSectorChildIndex = sectorIndex;
                    _movingSectorOriginalOffset = sector.CustomOffset;
                    _draggingSectorMove = true;
                    _lastSectorDragPreviewUpdate = DateTime.MinValue;
                    SetDragCursor();
                    Invalidate();
                    return;
                }

                Cluster cluster = GetClusterAtMousePos(adjustedMousePos, out _);
                if (cluster != null)
                {
                    _movingCluster = cluster;
                    _draggingClusterMove = true;
                    SetDragCursor();
                    Invalidate();
                }

                return;
            }

            _dragging = true;
            _mouseDownPos = e.Location; // Store initial position
            _lastMousePos = e.Location;
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (_movingHighway != null)
            {
                PointF adjustedMousePos = new(
                    (e.Location.X - _offset.X) / _zoom,
                    (e.Location.Y - _offset.Y) / _zoom
                );
                UpdateHighwayDrag(adjustedMousePos);
                Invalidate();
                return;
            }

            if (_draggingSectorMove)
            {
                PointF adjustedMousePos = new(
                    (e.Location.X - _offset.X) / _zoom,
                    (e.Location.Y - _offset.Y) / _zoom
                );

                UpdateSectorDrag(adjustedMousePos);

                if ((DateTime.UtcNow - _lastSectorDragPreviewUpdate).TotalMilliseconds >= 100)
                {
                    UpdateClusterHexagon(_movingSectorCluster);
                    _lastSectorDragPreviewUpdate = DateTime.UtcNow;
                }

                Invalidate();
                return;
            }

            if (_draggingClusterMove)
            {
                Invalidate();
                return;
            }

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
            if (_movingHighway != null)
            {
                _movingHighway = null;
                ResetInteractionCursor();
                Invalidate();
                return;
            }

            if (_draggingSectorMove)
            {
                _draggingSectorMove = false;

                PointF adjustedMousePos = new(
                    (e.Location.X - _offset.X) / _zoom,
                    (e.Location.Y - _offset.Y) / _zoom
                );
                CompleteSectorMove(adjustedMousePos);
                return;
            }

            if (_draggingClusterMove)
            {
                _draggingClusterMove = false;

                if (_movingCluster != null)
                {
                    PointF adjustedMousePos = new(
                        (e.Location.X - _offset.X) / _zoom,
                        (e.Location.Y - _offset.Y) / _zoom
                    );

                    Cluster clusterAtPos = GetClusterAtMousePos(adjustedMousePos, out (int x, int y)? coordinate);
                    if (clusterAtPos == _movingCluster)
                    {
                        _movingCluster = null;
                        ResetInteractionCursor();
                        Invalidate();
                        return;
                    }

                    if (clusterAtPos != null)
                    {
                        _ = MessageBox.Show("Cannot place cluster at the target location because another cluster already exists here.");
                        _movingCluster = null;
                        ResetInteractionCursor();
                        Invalidate();
                        return;
                    }

                    if (coordinate != null)
                    {
                        _ = MainForm.Instance.AllClusters.Remove((_movingCluster.Position.X, _movingCluster.Position.Y));
                        _movingCluster.Position = new Point(coordinate.Value.x, coordinate.Value.y);
                        MainForm.Instance.AllClusters[coordinate.Value] = _movingCluster;
                        _movingCluster = null;
                        if (SectorMapInteractionRules.PreserveChildSectorLayoutAfterClusterMove())
                        {
                            ResetInteractionCursor();
                            RebuildMapGeometry(snapChildSectors: false, avoidChildCollisions: false, snapTravelNodes: false);
                        }
                        else
                        {
                            ResetInteractionCursor();
                            Reset(false);
                        }
                        return;
                    }

                    _movingCluster = null;
                    ResetInteractionCursor();
                    Invalidate();
                    return;
                }
            }

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
                    var translatedCoordinate = new Point(q, r).SquareGridToHexCoordinate();

                    _ = MainForm.Instance.AllClusters.TryGetValue((translatedCoordinate.X, translatedCoordinate.Y), out Cluster cluster);

                    // Determine hex information
                    Hexagon hex = GenerateHexagonWithChildren(hexHeight, r, q, 0, 0, cluster?.Sectors, (translatedCoordinate.X, translatedCoordinate.Y), _defaultZoom);
                    _hexagons[(translatedCoordinate.X, translatedCoordinate.Y)] = hex;
                }
            }
        }

        private void UpdateClusterHexagon(Cluster cluster)
        {
            if (cluster == null)
                return;

            Point squareCoordinate = new Point(cluster.Position.X, cluster.Position.Y).HexToSquareGridCoordinate();
            float hexHeight = (float)(Math.Sqrt(3) * _hexSize);

            Hexagon hex = GenerateHexagonWithChildren(
                hexHeight,
                squareCoordinate.Y,
                squareCoordinate.X,
                0,
                0,
                cluster.Sectors,
                (cluster.Position.X, cluster.Position.Y),
                _defaultZoom);

            _hexagons[(cluster.Position.X, cluster.Position.Y)] = hex;
            cluster.Hexagon = hex;
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
            {SectorPlacement.MiddleLeft, (float width, float childHeight) => (width * 0, 0) },
            {SectorPlacement.MiddleTop, (float width, float childHeight) => (width * 0.25f, -(childHeight * 0.5f)) },
            {SectorPlacement.MiddleBottom, (float width, float childHeight) => (width * 0.25f, childHeight * 0.5f) }
        };

        private static Hexagon GenerateHexagonWithChildren(float height, int row, int col, float centerX, float centerY, List<Sector> sectors, (int x, int y) translatedCoordinate, float zoom = 1.0f)
        {
            // Step 1: Scale base height with zoom
            float zoomedHeight = height * zoom;
            float zoomedWidth = (float)(4 * (zoomedHeight / 2 / Math.Sqrt(3)));

            // Step 2: Apply padding by shrinking the actual drawn hex size
            float hexDrawHeight = zoomedHeight - _hexPadding;
            float hexDrawWidth = zoomedWidth - _hexPadding;

            // Step 3: Positioning the hex center using spacing based on full zoomed size
            float xOffset = col * (zoomedWidth * 0.75f);
            float yOffset = -row * zoomedHeight;
            if (col % 2 != 0)
            {
                yOffset -= zoomedHeight / 2;
            }

            xOffset += centerX;
            yOffset -= centerY;

            // Step 4: Build the actual hex points using the *shrunk* draw width/height
            PointF[] parentHex =
            [
                new PointF(xOffset, yOffset),
                new PointF(xOffset + (hexDrawWidth * 0.25f), yOffset - (hexDrawHeight / 2)),
                new PointF(xOffset + (hexDrawWidth * 0.75f), yOffset - (hexDrawHeight / 2)),
                new PointF(xOffset + hexDrawWidth, yOffset),
                new PointF(xOffset + (hexDrawWidth * 0.75f), yOffset + (hexDrawHeight / 2)),
                new PointF(xOffset + (hexDrawWidth * 0.25f), yOffset + (hexDrawHeight / 2)),
            ];

            PointF parentHexCenter = GetHexCenter(parentHex);
            float parentHexRadius = GetHexRadius(parentHex);

            // Child hexes are 50% of parent size
            float childHeight = hexDrawHeight / 2;
            float childWidth = hexDrawWidth / 2;
            List<PointF[]> childHexes = [];

            // Child hex positions (equally spaced inside parent)
            int children = sectors?.Count ?? 0;

            // 4 sector shenanigans
            if (children == 4)
            {
                childWidth /= 1.25f;
                childHeight /= 1.25f;
            }

            List<PointF> childHexPositions = [];
            if (children > 1)
            {
                // Child hex centers for top-left, bottom-right
                for (int i = 0; i < children; i++)
                {
                    float x, y;
                    if (sectors[i].CustomOffset.HasValue)
                    {
                        (x, y) = ConvertCustomOffsetToChildCenter(sectors[i].CustomOffset.Value, parentHexRadius);
                        childHexPositions.Add(new PointF(parentHexCenter.X + x, parentHexCenter.Y + y));
                        continue;
                    }
                    else
                    {
                        (x, y) = _childPlacementMappings[sectors[i].Placement](hexDrawWidth, childHeight);

                        // More 4 sector shenanigans
                        if (children == 4)
                        {
                            var placement = sectors[i].Placement;
                            if (placement == SectorPlacement.MiddleRight)
                            {
                                x = hexDrawWidth * 0.6f;
                            }
                            else if (placement == SectorPlacement.MiddleTop ||
                                placement == SectorPlacement.MiddleBottom)
                            {
                                x = hexDrawWidth * 0.3f;
                                if (placement == SectorPlacement.MiddleTop)
                                {
                                    y = -(childHeight * 0.75f);
                                }
                                else if (placement == SectorPlacement.MiddleBottom)
                                {
                                    y = childHeight * 0.75f;
                                }
                            }
                        }

                        // The legacy placement mappings were authored against a left-edge child origin.
                        // Child hexes are now rendered around their true center, so shift by half a child width.
                        x += childWidth * 0.5f;
                    }

                    childHexPositions.Add(new PointF(xOffset + x, yOffset + y));
                }
            }

            foreach (PointF childCenter in childHexPositions)
            {
                childHexes.Add(
                [
                    new PointF(childCenter.X - (childWidth * 0.5f), childCenter.Y),
                    new PointF(childCenter.X - (childWidth * 0.25f), childCenter.Y - (childHeight / 2)),
                    new PointF(childCenter.X + (childWidth * 0.25f), childCenter.Y - (childHeight / 2)),
                    new PointF(childCenter.X + (childWidth * 0.5f), childCenter.Y),
                    new PointF(childCenter.X + (childWidth * 0.25f), childCenter.Y + (childHeight / 2)),
                    new PointF(childCenter.X - (childWidth * 0.25f), childCenter.Y + (childHeight / 2)),
                ]);
            }

            return new Hexagon(translatedCoordinate, parentHex, childHexes.Select(a => new Hexagon(translatedCoordinate, a, null)).ToList());
        }

        private static (float x, float y) ConvertCustomOffsetToChildCenter(Point customOffset, float parentHexRadius)
        {
            PointF screenOffset = ConvertFromWorldCoordinate(customOffset, 2000000, parentHexRadius);
            return (screenOffset.X, screenOffset.Y);
        }

        private static Point ConvertPointInClusterHexToCustomOffset(Cluster cluster, PointF point)
        {
            PointF clusterCenter = GetHexCenter(cluster.Hexagon.Points);
            float clusterHexRadius = GetHexRadius(cluster.Hexagon.Points);
            return ConvertToWorldCoordinate(new PointF(point.X - clusterCenter.X, point.Y - clusterCenter.Y), 2000000, clusterHexRadius);
        }

        private void DrawHexGrid(object sender, PaintEventArgs e)
        {
            try
            {
                RenderMap(e.Graphics, allowReset: true, includeTipLabel: true);
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

        private void RenderMap(Graphics graphics, bool allowReset, bool includeTipLabel)
        {
            graphics.Clear(Color.Black);
            graphics.TranslateTransform(_offset.X, _offset.Y);
            graphics.ScaleTransform(_zoom, _zoom);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            PaintEventArgs paintArgs = new(graphics, Rectangle.Round(graphics.VisibleClipBounds));

            RenderAllHexes(paintArgs, out bool invalid);
            if (invalid)
            {
                if (allowReset)
                {
                    Reset();
                    return;
                }

                throw new InvalidOperationException("Unable to render the map because the sector layout is invalid.");
            }

            if (_isHdExportRendering)
            {
                RenderExportHexGridOverlay(paintArgs);
            }

            RenderHexSelection(paintArgs);
            RenderRegionCircles(paintArgs);
            RenderGateConnections(paintArgs);
            RenderHexIcons(paintArgs);
            RenderAllHexNames(paintArgs);

            if (includeTipLabel)
            {
                RenderTipLabel(paintArgs);
            }
        }

        private readonly Dictionary<Color, SolidBrush> _brushColorCache = [];
        private SolidBrush GetBrushColor(Color color)
        {
            if (_brushColorCache.TryGetValue(color, out var brush))
                return brush;
            _brushColorCache[color] = brush = new SolidBrush(color);
            return brush;
        }

        private void RenderRegionCircles(PaintEventArgs e)
        {
            if (!IsMapOptionChecked(MapOption.Visualize_Regions))
                return;

            var showVanilla = IsMapOptionChecked(MapOption.Show_Vanilla_Regions);
            var showCustom = IsMapOptionChecked(MapOption.Show_Custom_Regions);
            if (!showVanilla && !showCustom) return;

            // Define region circle size based on boundary radius
            // Place region at region coordinates based on sector position within cluster position
            var clusters = _baseGameClusters.Values
                .Concat(_customClusters);

            // Collect resource colors from the legend
            var resourceColors = _legend["resources"]
                .Select(a => ((string name, Color color))a)
                .ToDictionary(a => a.name, a => a.color, StringComparer.OrdinalIgnoreCase);

            // Calculate hex size and radius based on zoom and sector size
            float hexHeight = (float)(Math.Sqrt(3) * _hexSize) * _defaultZoom; // Height for flat-top hexes, applying zoom
            float hexRadius = (float)(hexHeight / Math.Sqrt(3)); // Recalculate radius based on zoom

            // Setup color mapping based on resources of region definitions
            var colorMappings = clusters
                .SelectMany(c => c.Sectors)
                .SelectMany(
                    sector => sector.Regions,
                    (sector, region) => new
                    {
                        Definition = region.Definition,
                        Sector = sector
                    })
                .GroupBy(x => x.Definition)
                .ToDictionary(
                    g => g.Key,
                    g => g.SelectMany(x => x.Sector.ResourceAreas)
                          .GroupBy(x => x.Ware)
                          .Select(x => resourceColors[x.Key])
                          .ToArray());

            foreach (var cluster in clusters)
            {
                // The region position is based on the sector position (center is not 0, 0, 0 but the position of the sector)
                // Which is in the high numbers, and we don't have these coordinates in our mapping, so we need to find a way
                // To convert these region positions to the correct position where 0,0,0 is the center of the sector

                //var clusterPos = new PointF(cluster.Position.X * 15000 * 1000, cluster.Position.Y * 8660 * 1000);
                int sectorIndex = 0;
                foreach (var sector in cluster.Sectors)
                {
                    // Collect the child hexagon points
                    Hexagon childHexagon = cluster.Sectors.Count == 1 ? cluster.Hexagon : cluster.Hexagon.Children[sectorIndex];
                    PointF hexCenter = GetHexCenter(childHexagon.Points);
                    float correctHexRadius = cluster.Sectors.Count == 1 ? hexRadius : cluster.Sectors.Count == 4 ? hexRadius / 2f / 1.25f : hexRadius / 2f;

                    // Ordered by desc, so biggest regions are drawn first and smaller ones on top
                    // This improves visibility
                    foreach (var obj in sector.Regions
                        .Select(a =>
                        {
                            if (string.IsNullOrWhiteSpace(a.BoundaryRadius) || !int.TryParse(a.BoundaryRadius, out var radius))
                            {
#if DEBUG
                                throw new Exception("No boundary radius defined: " + sector.Name + " | " + a.Name + " | " + a.BoundaryRadius);
#else
                                return null;
#endif
                            }
                            return new { Boundary = radius, Region = a };
                        })
                        .Where(a => a != null)
                        .OrderByDescending(a => a.Boundary))
                    {
                        var region = obj.Region;
                        var radius = obj.Boundary;
                        if (!showVanilla && region.IsBaseGame) continue;
                        if (!showCustom && !region.IsBaseGame) continue;

                        // Offset the region with the base position
                        var basePos = region.IsBaseGame ? new PointF(region.Position.X - sector.SectorRealOffset.X, region.Position.Y - sector.SectorRealOffset.Y) : region.Position;
                        if (radius == 0 || radius >= 1000000)
                        {
                            // Center it, this is a sector wide region
                            basePos = new(0, 0);
                        }

                        float screenRadius = radius >= 1000000 || radius == 0 ? correctHexRadius * 1.6f : Math.Max(20, Math.Min(ConvertFromWorldRadius(radius, hexRadius, sector.DiameterRadius), correctHexRadius * 1.6f));
                        PointF regionScreenPosition = ConvertFromWorldCoordinate(basePos, sector.DiameterRadius, correctHexRadius);

                        regionScreenPosition.X += hexCenter.X;
                        regionScreenPosition.Y += hexCenter.Y;

                        // Determine region color
                        var regionColors = colorMappings[region.Definition];

                        // Visualize outside of hex bounds
                        if (!IsPointInPolygon(childHexagon.Points, regionScreenPosition))
                        {
                            DrawRegionCircle(e.Graphics, screenRadius, regionScreenPosition, regionColors);
                            e.Graphics.DrawLine(Pens.Fuchsia, hexCenter.X, hexCenter.Y, regionScreenPosition.X, regionScreenPosition.Y);
                            continue;
                        }

                        using GraphicsPath hexPath = new();
                        hexPath.AddPolygon(childHexagon.Points);
                        using var hexClipRegion = new System.Drawing.Region(hexPath);

                        // Save current clipping region
                        using var oldClip = e.Graphics.Clip.Clone();

                        // Set clip to hex shape
                        e.Graphics.SetClip(hexClipRegion, CombineMode.Intersect);

                        // Draw the pie circle
                        DrawRegionCircle(e.Graphics, screenRadius, regionScreenPosition, regionColors);

                        // Restore the original clipping region
                        e.Graphics.SetClip(oldClip, CombineMode.Replace);
                    }

                    sectorIndex++;
                }
            }
        }

        private readonly Pen _edgePen = new(Color.FromArgb(150, Color.White), 1f);
        private void DrawRegionCircle(Graphics g, float radius, PointF center, Color[] colors)
        {
            if (colors == null || colors.Length == 0)
                return;

            RectangleF rect = new(center.X - radius / 2f, center.Y - radius / 2f, radius, radius);

            float startAngle = 0f;
            float sweepAngle = 360f / colors.Length;

            // Fill pie segments
            foreach (var color in colors)
            {
                var brush = GetBrushColor(color);
                g.FillPie(brush, rect, startAngle, sweepAngle);
                startAngle += sweepAngle;
            }

            // Draw lighter outline (edge)
            g.DrawEllipse(_edgePen, rect);
        }

        private static int ConvertFromWorldRadius(int worldRadius, float hexRadius, float diameterRadius)
        {
            return (int)Math.Round(worldRadius * 2f * hexRadius / diameterRadius);
        }

        private void RenderTipLabel(PaintEventArgs e)
        {
            GraphicsState state = e.Graphics.Save();
            e.Graphics.ResetTransform();

            string labelText = GetInteractionStatusText();
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

        private string GetInteractionStatusText()
        {
            if (_movingHighway != null)
                return "Dragging gate node: release to keep the new gate position, ESC to cancel.";

            if (_draggingSectorMove && _movingSector != null)
                return $"Dragging sector \"{_movingSector.Name}\": snapping to safe in-game multi-sector positions, ESC to cancel.";

            if (_draggingClusterMove && _movingCluster != null)
                return $"Dragging cluster \"{_movingCluster.Name}\": release over an empty hex to move it, ESC to cancel.";

            return "Tip: Right drag moves clusters, sectors, and gate nodes.";
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
            foreach (var group in iconDatas
                .Where(a => IsDlcClusterEnabled(a.Cluster))
                .GroupBy(a => a.Sector))
            {
                if (_visibleSectorsFromSearch.Count > 0 && !_visibleSectorsFromSearch.Contains(group.Key))
                    continue;

                if ((!IsMapOptionChecked(MapOption.Show_Vanilla_Sectors) && group.Key.IsBaseGame) ||
                    (!IsMapOptionChecked(MapOption.Show_Custom_Sectors) && !group.Key.IsBaseGame))
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
                    float correctHexHeight = cluster.Sectors.Count == 1 ? hexHeight : cluster.Sectors.Count == 4 ? hexHeight / 2f / 1.25f : hexHeight / 2f;

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
                    e.Graphics.DrawImageUnscaled(iconToUse, new Point((int)pos.X, (int)pos.Y));
                    processed++;

                    if (!string.IsNullOrWhiteSpace(icon.Yield))
                    {
                        using Font fBold = new(Font.FontFamily, (cluster.Sectors.Count == 1 ? 12 : 10), FontStyle.Bold);
                        var text = icon.Yield;
                        e.Graphics.DrawString(text, fBold, Brushes.Black,
                            pos.X - 1f, pos.Y - 1f);
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
            var showVanilla = IsMapOptionChecked(MapOption.Show_Vanilla_Regions);
            var showCustom = IsMapOptionChecked(MapOption.Show_Custom_Regions);
            if (!showVanilla && !showCustom)
                yield break;

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
                    var resources = sector.ResourceAreas;
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
            if (!IsMapOptionChecked(MapOption.Show_Vanilla_Stations) &&
                !IsMapOptionChecked(MapOption.Show_Custom_Stations)) return;

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
                // Check if the dlc is selected
                if (!IsDlcClusterEnabled(cluster))
                {
                    continue;
                }

                int sectorIndex = 0;
                foreach (Sector sector in cluster.Sectors)
                {
                    if (_visibleSectorsFromSearch.Count > 0 && !_visibleSectorsFromSearch.Contains(sector))
                        continue;

                    if ((!IsMapOptionChecked(MapOption.Show_Vanilla_Sectors) && sector.IsBaseGame) ||
                        (!IsMapOptionChecked(MapOption.Show_Custom_Sectors) && !sector.IsBaseGame))
                        continue;

                    // Collect the child hexagon points
                    Hexagon childHexagon = cluster.Sectors.Count == 1 ? cluster.Hexagon : cluster.Hexagon.Children[sectorIndex];
                    PointF hexCenter = GetHexCenter(childHexagon.Points);
                    float correctHexRadius = cluster.Sectors.Count == 1 ? hexRadius : cluster.Sectors.Count == 4 ? hexRadius / 2f / 1.25f : hexRadius / 2f;

                    foreach (Zone zone in sector.Zones)
                    {
                        if (!IsMapOptionChecked(MapOption.Show_Vanilla_Stations) && zone.IsBaseGame) continue;
                        if (!IsMapOptionChecked(MapOption.Show_Custom_Stations) && !zone.IsBaseGame) continue;

                        foreach (Station station in zone.Stations)
                        {
                            var stationIcon = GetIconFromStore(station.Type.ToLower());
                            if (stationIcon == null) continue;

                            // Convert the zone position from world to screen space
                            PointF stationScreenPosition = ConvertFromWorldCoordinate(station.Position, sector.DiameterRadius, correctHexRadius);

                            stationScreenPosition.X += hexCenter.X;
                            stationScreenPosition.Y += hexCenter.Y;

                            Color color = FactionsForm.GetColorForFaction(station.Owner, checkClaimSpace: false);

                            // Define the size for the resized icon (width and height)
                            int width = cluster.Sectors.Count == 1 ? sizeLarge.X : sizeSmall.X;
                            int height = cluster.Sectors.Count == 1 ? sizeLarge.Y : sizeSmall.Y;
                            width /= 2;
                            height /= 2;

                            if (cluster.Sectors.Count == 1)
                            {
                                // Reduce icon size of 1 sector clusters
                                width = (int)(width * 0.8f);
                                height = (int)(height * 0.8f);
                            }
                            else if (cluster.Sectors.Count == 4)
                            {
                                // Reduce icon size of 4 sector clusters even more
                                width = (int)(width * 0.75f);
                                height = (int)(height * 0.75f);
                            }

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
            if (_movingSectorCluster != null && _movingSectorChildIndex != null)
            {
                using SolidBrush movingBrush = new(Color.Gold);
                Hexagon movingHex = _hexagons[(_movingSectorCluster.Position.X, _movingSectorCluster.Position.Y)].Children[_movingSectorChildIndex.Value];
                e.Graphics.FillPolygon(movingBrush, movingHex.Points);
            }

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

        private void RenderAllHexes(PaintEventArgs e, out bool invalid)
        {
            invalid = false;
            // First step render non existant hexagons
            Color nonExistantHexColor = "#121212".HexToColor();
            using SolidBrush mainBrush = new(Color.Black);
            using Pen mainPen = new(nonExistantHexColor, 4);
            foreach (KeyValuePair<(int, int), Hexagon> hex in _hexagons)
            {
                RenderNonSectorGrid(e, mainBrush, mainPen, hex);
            }

            // Next step render the game clusters on top
            foreach (Cluster cluster in _baseGameClusters.Values)
            {
                if (cluster.Sectors.All(a => a.IsBaseGame) && !IsMapOptionChecked(MapOption.Show_Vanilla_Sectors))
                {
                    continue;
                }

                // Check if the dlc is selected
                if (!IsDlcClusterEnabled(cluster))
                {
                    continue;
                }

                RenderClusters(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon), out invalid);
                if (invalid) return;
            }

            if (IsMapOptionChecked(MapOption.Show_Custom_Sectors))
            {
                // Next step render the custom clusters
                foreach (Cluster cluster in _customClusters)
                {
                    // Always overwrite the hexagon as it can change between sessions
                    cluster.Hexagon = _hexagons[(cluster.Position.X, cluster.Position.Y)];
                    RenderClusters(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon), out invalid);
                    if (invalid) return;
                }
            }
        }

        private void RenderAllHexNames(PaintEventArgs e)
        {
            if (IsMapOptionChecked(MapOption.Show_Custom_Sectors))
            {
                // Next step render names
                foreach (Cluster cluster in _customClusters)
                {
                    RenderHexNames(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon));
                }
            }

            // Next step render names
            foreach (Cluster cluster in _baseGameClusters.Values)
            {
                if (cluster.Sectors.All(a => a.IsBaseGame) && !IsMapOptionChecked(MapOption.Show_Vanilla_Sectors))
                    continue;

                // Check if the dlc is selected
                if (!IsDlcClusterEnabled(cluster))
                {
                    continue;
                }

                RenderHexNames(e, new KeyValuePair<(int, int), Hexagon>((cluster.Position.X, cluster.Position.Y), cluster.Hexagon));
            }
        }

        private void RenderNonSectorGrid(PaintEventArgs e, SolidBrush mainBrush, Pen mainPen, KeyValuePair<(int, int), Hexagon> hex)
        {
            // Render each non-existant hex first
            if (!MainForm.Instance.AllClusters.TryGetValue(hex.Key, out Cluster cluster) ||
                !IsDlcClusterEnabled(cluster) ||
                (!IsMapOptionChecked(MapOption.Show_Vanilla_Sectors) && cluster.IsBaseGame) ||
                (!IsMapOptionChecked(MapOption.Show_Custom_Sectors) && !cluster.IsBaseGame) ||
                _visibleSectorsFromSearch.Count > 0 && cluster.Sectors.Any(a => !_visibleSectorsFromSearch.Contains(a)))
            {
                // Fill hex
                e.Graphics.FillPolygon(mainBrush, hex.Value.Points);
                // Draw edges
                e.Graphics.DrawPolygon(mainPen, hex.Value.Points);

                SizeF textSize;
                if (IsMapOptionChecked(MapOption.Show_Coordinates))
                {
                    PointF hexCenter = GetHexCenter(hex.Value.Points);
                    SizeF hexSize = GetHexSize(hex.Value.Points);

                    using Font fBold = new(Font.FontFamily, Font.Size * (_hexSize / 100), FontStyle.Bold);
                    (int x, int y) = hex.Key;
                    string coordText = $"({x}, {y})";
                    textSize = e.Graphics.MeasureString(coordText, fBold);
                    e.Graphics.DrawString(coordText, fBold, Brushes.White,
                        hexCenter.X - (hexSize.Width * 0.25f),            // Align to the left
                        hexCenter.Y + (hexSize.Height / 2) - textSize.Height - (_hexPadding / 2f)); // Align to the bottom
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
            GateConnection[] connections = GetVisibleGateConnections();

            foreach (GateConnection connection in connections)
            {
                PaintConnection(connection, e);
            }
        }

        private GateConnection[] GetVisibleGateConnections()
        {
            if (!IsMapOptionChecked(MapOption.Show_Vanilla_Gates) &&
                !IsMapOptionChecked(MapOption.Show_Custom_Gates))
            {
                return [];
            }

            List<GateData> gatesData = [];

            if (IsMapOptionChecked(MapOption.Show_Custom_Sectors) && IsMapOptionChecked(MapOption.Show_Custom_Gates))
            {
                foreach (Cluster cluster in _customClusters)
                {
                    gatesData.AddRange(CollectGateDataFromCluster(cluster));
                }
            }

            foreach (KeyValuePair<(int, int), Cluster> cluster in _baseGameClusters)
            {
                if (cluster.Value.Sectors.All(a => a.IsBaseGame) && !IsMapOptionChecked(MapOption.Show_Vanilla_Sectors))
                    continue;

                if (!IsDlcClusterEnabled(cluster.Value))
                    continue;

                gatesData.AddRange(CollectGateDataFromCluster(cluster.Value));
            }

            return [.. CollectConnectionsFromGateData(gatesData).FilterDuplicateHighwayConnections()];
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
            if (connection.Source.Gate.IsAcceleratorNode || connection.Target.Gate.IsAcceleratorNode)
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

            linePen.DashStyle = (connection.Source.Gate.IsAcceleratorNode || connection.Target.Gate.IsAcceleratorNode)
                ? DashStyle.Dash
                : DashStyle.Dot;

            // Draw connection line between source and target
            e.Graphics.DrawLine(linePen, connection.Source.ScreenX, connection.Source.ScreenY, connection.Target.ScreenX, connection.Target.ScreenY);
        }

        private bool TryGetHighwayAtMousePos(Point mousePos, out HighwayEndpointDragState state)
        {
            float nodeHitRadius = SectorMapInteractionRules.GetGateNodeHitRadius(_zoom, _gateSizeRadius);
            PointF mouse = new(mousePos.X, mousePos.Y);
            float bestNodeScore = float.MaxValue;
            HighwayEndpointDragState bestNodeState = null;
            bool foundNode = false;

            foreach (var item in GetVisibleGateConnections())
            {
                PointF source = new((item.Source.ScreenX * _zoom) + _offset.X, (item.Source.ScreenY * _zoom) + _offset.Y);
                PointF target = new((item.Target.ScreenX * _zoom) + _offset.X, (item.Target.ScreenY * _zoom) + _offset.Y);

                float sourceDistance = Distance(mouse, source);
                float targetDistance = Distance(mouse, target);

                if (sourceDistance <= nodeHitRadius && sourceDistance < bestNodeScore)
                {
                    bestNodeScore = sourceDistance;
                    bestNodeState = CreateHighwayEndpointState(item, isSource: true);
                    foundNode = true;
                }

                if (targetDistance <= nodeHitRadius && targetDistance < bestNodeScore)
                {
                    bestNodeScore = targetDistance;
                    bestNodeState = CreateHighwayEndpointState(item, isSource: false);
                    foundNode = true;
                }
            }

            state = bestNodeState;
            return foundNode;
        }

        private void UpdateHighwayDrag(PointF mousePos)
        {
            if (_movingHighway == null)
                return;

            PointF delta = new(mousePos.X - _movingHighway.StartMouseScreen.X, mousePos.Y - _movingHighway.StartMouseScreen.Y);

            PointF desired = new(_movingHighway.OriginalScreen.X + delta.X, _movingHighway.OriginalScreen.Y + delta.Y);
            _movingHighway.Gate.Position = ConvertDraggedHighwayPosition(_movingHighway.Cluster, _movingHighway.Sector, _movingHighway.Zone, desired);
        }

        private Point ConvertDraggedHighwayPosition(Cluster cluster, Sector sector, Zone zone, PointF desiredScreenPosition)
        {
            PointF sectorHexCenter = GetSectorHexCenter(cluster, sector);
            float sectorHexRadius = GetSectorHexRadius(cluster);
            PointF clamped = ClampPointInsideSectorHex(desiredScreenPosition, cluster, sector);

            Point worldPoint = ConvertToWorldCoordinate(new PointF(clamped.X - sectorHexCenter.X, clamped.Y - sectorHexCenter.Y), sector.DiameterRadius, sectorHexRadius);
            return new Point(worldPoint.X - zone.Position.X, worldPoint.Y - zone.Position.Y);
        }

        private HighwayEndpointDragState CreateHighwayEndpointState(GateConnection connection, bool isSource)
        {
            GateData endpoint = isSource ? connection.Source : connection.Target;

            return new HighwayEndpointDragState
            {
                Connection = connection,
                Cluster = endpoint.Cluster,
                Sector = endpoint.Sector,
                Zone = endpoint.Zone,
                Gate = endpoint.Gate,
                OriginalGatePosition = endpoint.Gate.Position,
                OriginalScreen = new PointF(endpoint.ScreenX, endpoint.ScreenY)
            };
        }

        private PointF GetSectorHexCenter(Cluster cluster, Sector sector)
        {
            if (cluster.Sectors.Count == 1)
                return GetHexCenter(cluster.Hexagon.Points);

            int index = cluster.Sectors.IndexOf(sector);
            return GetHexCenter(cluster.Hexagon.Children[index].Points);
        }

        private float GetSectorHexRadius(Cluster cluster)
        {
            float hexHeight = (float)(Math.Sqrt(3) * _hexSize) * _defaultZoom;
            float hexRadius = (float)(hexHeight / Math.Sqrt(3));
            return cluster.Sectors.Count == 1 ? hexRadius : hexRadius / 2f;
        }

        private PointF ClampPointInsideSectorHex(PointF point, Cluster cluster, Sector sector)
        {
            PointF[] polygon = cluster.Sectors.Count == 1
                ? cluster.Hexagon.Points
                : cluster.Hexagon.Children[cluster.Sectors.IndexOf(sector)].Points;

            if (IsPointInPolygon(polygon, point))
                return point;

            PointF center = GetHexCenter(polygon);
            PointF low = center;
            PointF high = point;

            for (int i = 0; i < 20; i++)
            {
                PointF mid = new((low.X + high.X) / 2f, (low.Y + high.Y) / 2f);
                if (IsPointInPolygon(polygon, mid))
                    low = mid;
                else
                    high = mid;
            }

            return low;
        }

        private static Point ConvertToWorldCoordinate(PointF localScreenPos, int sectorDiameterRadius, float hexRadius)
        {
            float normalizedX = localScreenPos.X / hexRadius;
            float normalizedY = -localScreenPos.Y / hexRadius;

            float worldX = normalizedX * sectorDiameterRadius / 2f;
            float worldY = normalizedY * sectorDiameterRadius / 2f;

            return new Point((int)Math.Round(worldX), (int)Math.Round(worldY));
        }

        private static float DistanceToSegment(PointF point, PointF a, PointF b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            if (dx == 0 && dy == 0)
                return Distance(point, a);

            float t = ((point.X - a.X) * dx + (point.Y - a.Y) * dy) / ((dx * dx) + (dy * dy));
            t = Math.Clamp(t, 0f, 1f);
            PointF projection = new(a.X + (t * dx), a.Y + (t * dy));
            return Distance(point, projection);
        }

        private static float Distance(PointF a, PointF b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (float)Math.Sqrt((dx * dx) + (dy * dy));
        }

        private IEnumerable<GateConnection> CollectConnectionsFromGateData(List<GateData> gatesData)
        {
            Dictionary<string, GateData> sourcePathLookup = GateConnectionResolver
                .BuildSourcePathLookup(gatesData, a => a.Gate.SourcePath);

            // Make sure we don't double process target gates we already processed
            // We still have an issue with highway type gates showing as a double because they have different paths
            HashSet<Gate> processedTargets = [];

            // Any invalid connections will be recorded
            List<GateData> invalidConnections = [];

            // Set to keep track of processed connections
            foreach (GateData sourceGateData in gatesData)
            {
                if (!IsMapOptionChecked(MapOption.Show_Custom_Sectors) && !sourceGateData.Sector.IsBaseGame)
                    continue;

                // Find the connection with the matching path
                if (processedTargets.Contains(sourceGateData.Gate))
                {
                    continue;
                }

                if (!GateConnectionResolver.TryResolveTarget(sourcePathLookup, sourceGateData.Gate.DestinationPath, out GateData targetGateData))
                {
                    invalidConnections.Add(sourceGateData);
                    continue;
                }

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

                if (!IsMapOptionChecked(MapOption.Show_Custom_Sectors) && !targetGateData.Sector.IsBaseGame)
                    continue;

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
                string message = BuildInvalidGateMessage(invalidConnections);
                bool copiedToClipboard = TryCopyInvalidGateMessageToClipboard(message);
                string logPath = TryWriteInvalidGateMessageLog(message);

                if (!string.IsNullOrWhiteSpace(logPath))
                {
                    message += Environment.NewLine + Environment.NewLine + $"A log file was written to:{Environment.NewLine}{logPath}";
                }

                _ = MessageBox.Show(
                    copiedToClipboard
                        ? message + Environment.NewLine + Environment.NewLine + "The full error details were copied to your clipboard."
                        : message,
                    $"Invalid gate connections ({invalidConnections.Count})",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private static string TryWriteInvalidGateMessageLog(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return null;

            try
            {
                string logsDirectory = Path.Combine(Application.StartupPath, "logs");
                _ = Directory.CreateDirectory(logsDirectory);

                string fileName = $"invalid-gate-connections-{DateTime.Now:yyyyMMdd-HHmmss}.log";
                string fullPath = Path.Combine(logsDirectory, fileName);
                File.WriteAllText(fullPath, message);
                return fullPath;
            }
            catch
            {
                return null;
            }
        }

        private static bool TryCopyInvalidGateMessageToClipboard(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            try
            {
                Clipboard.SetText(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string BuildInvalidGateMessage(IEnumerable<GateData> invalidConnections)
        {
            GateData[] invalidConnectionsArray = invalidConnections
                .GroupBy(a => a.Gate.SourcePath, StringComparer.OrdinalIgnoreCase)
                .Select(a => a.First())
                .OrderBy(a => a.Gate.ParentSectorName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(a => a.Gate.DestinationSectorName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(a => a.Gate.ConnectionName, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var builder = new StringBuilder();
            _ = builder.AppendLine("Some gate connections could not be resolved when opening Galaxy View.");
            _ = builder.AppendLine();
            _ = builder.AppendLine("Each item shows the gate, its source, its intended destination, and the reverse path that could not be found.");
            _ = builder.AppendLine();

            foreach (GateData gateData in invalidConnectionsArray.Take(_invalidGatePreviewLimit))
            {
                Gate gate = gateData.Gate;
                string sourceZone = ExtractZoneToken(gate.Source);
                string destinationZone = ExtractZoneToken(gate.Destination);
                string gateName = !string.IsNullOrWhiteSpace(gate.ConnectionName)
                    ? gate.ConnectionName
                    : gate.SourcePath?.Split('/').LastOrDefault() ?? "<unknown gate>";

                _ = builder.AppendLine($"- Gate: {gateName}");
                _ = builder.AppendLine($"  Source: {gate.ParentSectorName} ({sourceZone})");
                _ = builder.AppendLine($"  Destination: {gate.DestinationSectorName} ({destinationZone})");
                _ = builder.AppendLine($"  Reverse path not found: {gate.DestinationPath}");
                _ = builder.AppendLine();
            }

            if (invalidConnectionsArray.Length > _invalidGatePreviewLimit)
            {
                int remaining = invalidConnectionsArray.Length - _invalidGatePreviewLimit;
                _ = builder.AppendLine($"...and {remaining} more invalid gate connection(s).");
                _ = builder.AppendLine();
            }

            _ = builder.AppendLine("Tip: search the gate connection name in your generated XML or imported config to find the exact source gate object.");
            return builder.ToString().TrimEnd();
        }

        private static string ExtractZoneToken(string gateEndpoint)
        {
            if (string.IsNullOrWhiteSpace(gateEndpoint))
                return "unknown zone";

            string zoneToken = gateEndpoint
                .Split('_', StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault(a => a.StartsWith("z", StringComparison.OrdinalIgnoreCase));

            return string.IsNullOrWhiteSpace(zoneToken) ? gateEndpoint : zoneToken;
        }

        private IEnumerable<GateData> CollectGateDataFromCluster(Cluster cluster)
        {
            // Calculate hex size and radius based on zoom and sector size
            float hexHeight = (float)(Math.Sqrt(3) * _hexSize) * _defaultZoom; // Height for flat-top hexes, applying zoom
            float hexRadius = (float)(hexHeight / Math.Sqrt(3)); // Recalculate radius based on zoom

            var collectVanillaGates = IsMapOptionChecked(MapOption.Show_Vanilla_Gates);
            var collectCustomGates = IsMapOptionChecked(MapOption.Show_Custom_Gates);

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
                        if (gate.IsBaseGame && !collectVanillaGates) continue;
                        if (!gate.IsBaseGame && !collectCustomGates) continue;

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

                HashSet<string> factions = sector.Zones
                    .Where(a => !a.IsBaseGame)
                    .SelectMany(a => a.Stations)
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

            HashSet<string> factions = sector.Zones
                .Where(a => !a.IsBaseGame)
                .SelectMany(a => a.Stations)
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

        private void RenderClusters(PaintEventArgs e, KeyValuePair<(int, int), Hexagon> hex, out bool invalid)
        {
            invalid = false;
            if (!MainForm.Instance.AllClusters.TryGetValue(hex.Key, out var cluster))
            {
                invalid = true;
                return;
            }

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
            if (!IsMapOptionChecked(MapOption.Show_Custom_Sectors) && !IsMapOptionChecked(MapOption.Show_Vanilla_Sectors))
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
                    if (cluster.Sectors.Count <= index)
                    {
                        invalid = true;
                        return;
                    }

                    Sector sector = cluster.Sectors[index];
                    bool renderChild = true;
                    if ((!IsMapOptionChecked(MapOption.Show_Custom_Sectors) && !sector.IsBaseGame) || (!IsMapOptionChecked(MapOption.Show_Vanilla_Sectors) && sector.IsBaseGame))
                    {
                        renderChild = false;
                    }

                    Color ownerColor = GetSectorOwnershipColor(sector);
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
            if (IsMapOptionChecked(MapOption.Show_Coordinates))
            {
                using Font fBold = new(Font.FontFamily, Font.Size * (_hexSize / 100), FontStyle.Bold);
                (int x, int y) = hex.Key;
                string coordText = $"({x}, {y})";
                textSize = e.Graphics.MeasureString(coordText, fBold);
                e.Graphics.DrawString(coordText, fBold, Brushes.White,
                    hexCenter.X - (hexSize.Width * 0.25f),                 // Align to the left
                    hexCenter.Y + (hexSize.Height / 2) - textSize.Height - (_hexPadding / 2f)); // Align to the bottom
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
            float mainFontSize = Font.Size * (_hexSize / 100);
            float childFontSize = mainFontSize / 2f;

            // Draw child names
            int index = 0; // reset for name rendering
            foreach (Hexagon child in hex.Value.Children)
            {
                // Render child sector name
                Sector sector = cluster.Sectors[index];
                if ((!IsMapOptionChecked(MapOption.Show_Custom_Sectors) && !sector.IsBaseGame) || (!IsMapOptionChecked(MapOption.Show_Vanilla_Sectors) && sector.IsBaseGame))
                {
                    index++;
                    continue;
                }
                if (_visibleSectorsFromSearch.Count > 0 && !_visibleSectorsFromSearch.Contains(sector))
                {
                    index++;
                    continue;
                }

                PointF childHexCenter = GetHexCenter(child.Points);
                SizeF childHexSize = GetHexSize(child.Points);
                RectangleF childBounds = new(
                    childHexCenter.X - (childHexSize.Width * 0.37f),
                    childHexCenter.Y - (childHexSize.Height * 0.20f),
                    childHexSize.Width * 0.74f,
                    childHexSize.Height * 0.40f);
                DrawCenteredFittedLabel(e.Graphics, sector.Name, childBounds, childFontSize, 1.5f, FontStyle.Bold, Brushes.White);
                index++;
            }

            // Render main hex sector name if no children
            if (hex.Value.Children.Count == 0 && cluster != null)
            {
                var sector = cluster.Sectors.First();
                if (_visibleSectorsFromSearch.Count > 0 && !_visibleSectorsFromSearch.Contains(sector))
                {
                    return;
                }

                RectangleF mainBounds = new(
                    hexCenter.X - (hexSize.Width * 0.39f),
                    hexCenter.Y - (hexSize.Height * 0.25f),
                    hexSize.Width * 0.78f,
                    hexSize.Height * 0.50f);
                DrawCenteredFittedLabel(e.Graphics, cluster.Sectors[index].Name, mainBounds, mainFontSize, 2.5f, FontStyle.Bold, Brushes.White);
            }
        }

        private static void DrawCenteredFittedLabel(Graphics graphics, string text, RectangleF bounds, float maxFontSize, float minFontSize, FontStyle style, Brush brush)
        {
            using StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };

            for (float fontSize = maxFontSize; fontSize >= minFontSize; fontSize -= 0.5f)
            {
                using Font font = new("Segoe UI", fontSize, style);
                SizeF measured = graphics.MeasureString(text, font, new SizeF(bounds.Width, bounds.Height), format);
                if (measured.Width <= bounds.Width && measured.Height <= bounds.Height)
                {
                    graphics.DrawString(text, font, brush, bounds, format);
                    return;
                }
            }

            using Font fallbackFont = new("Segoe UI", minFontSize, style);
            graphics.DrawString(text, fallbackFont, brush, bounds, format);
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

        private static float GetHexRadius(PointF[] hex)
        {
            PointF center = GetHexCenter(hex);
            float maxDistance = 0;
            foreach (PointF point in hex)
            {
                maxDistance = Math.Max(maxDistance, Distance(center, point));
            }

            return maxDistance;
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

        private void BtnSaveHdImage_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "PNG image (*.png)|*.png";
            saveFileDialog.DefaultExt = "png";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Title = "Save high-definition sector map image";
            saveFileDialog.FileName = $"sector-map-{DateTime.Now:yyyyMMdd-HHmmss}.png";

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                const int exportSize = 20000;
                int imageWidth = exportSize;
                int imageHeight = exportSize;

                using Bitmap bitmap = new(imageWidth, imageHeight);
                bitmap.SetResolution(300, 300);
                using Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                float previousZoom = _zoom;
                PointF previousOffset = _offset;

                try
                {
                    var bounds = GetMapBounds();
                    const float padding = 400f;
                    float availableWidth = imageWidth - (padding * 2);
                    float availableHeight = imageHeight - (padding * 2);
                    float scale = Math.Min(availableWidth / bounds.Width, availableHeight / bounds.Height);

                    _isHdExportRendering = true;
                    _zoom = scale;
                    _offset = new PointF(
                        padding + ((availableWidth - (bounds.Width * scale)) / 2f) - (bounds.Left * scale),
                        padding + ((availableHeight - (bounds.Height * scale)) / 2f) - (bounds.Top * scale));

                    RenderMap(graphics, allowReset: false, includeTipLabel: false);
                    graphics.ResetTransform();
                    DrawHdScaleKey(graphics, imageWidth, imageHeight);
                }
                finally
                {
                    _isHdExportRendering = false;
                    _zoom = previousZoom;
                    _offset = previousOffset;
                }

                bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);

                _ = MessageBox.Show($"High-definition sector map saved succesfully.\nResolution: {imageWidth}x{imageHeight}", "Sector map saved");
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#else
                _ = MessageBox.Show("Unable to save the sector map image: " + ex.Message, "Export failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
            }
        }

        private void RenderExportHexGridOverlay(PaintEventArgs e)
        {
            using Pen gridPen = new(Color.FromArgb(140, 80, 80, 80), 2f / Math.Max(_zoom, 0.0001f));

            foreach (var hex in _hexagons.Values)
            {
                e.Graphics.DrawPolygon(gridPen, hex.Points);

                foreach (var child in hex.Children)
                {
                    e.Graphics.DrawPolygon(gridPen, child.Points);
                }
            }
        }

        private void DrawHdScaleKey(Graphics graphics, int imageWidth, int imageHeight)
        {
            const int boxWidth = 900;
            const int boxHeight = 250;
            const int margin = 70;
            Rectangle box = new(margin, imageHeight - boxHeight - margin, boxWidth, boxHeight);

            using SolidBrush backgroundBrush = new(Color.FromArgb(210, 0, 0, 0));
            using Pen borderPen = new(Color.White, 3f);
            using Font titleFont = new("Segoe UI", 22, FontStyle.Bold);
            using Font bodyFont = new("Segoe UI", 16, FontStyle.Regular);
            const int leftPadding = 24;
            const int topPadding = 18;
            const int lineSpacing = 14;

            graphics.FillRectangle(backgroundBrush, box);
            graphics.DrawRectangle(borderPen, box);

            var originCluster = MainForm.Instance.AllClusters.Values
                .FirstOrDefault(a => a.Name.Equals("Getsu Fune", StringComparison.OrdinalIgnoreCase));
            var originText = originCluster != null
                ? $"Grid origin: Getsu Fune ({originCluster.Position.X}, {originCluster.Position.Y})"
                : "Grid origin: Getsu Fune";

            float y = box.Top + topPadding;
            graphics.DrawString("Hex Grid Scale", titleFont, Brushes.White, box.Left + leftPadding, y);
            y += titleFont.GetHeight(graphics) + lineSpacing;
            graphics.DrawString(originText, bodyFont, Brushes.White, box.Left + leftPadding, y);
            y += bodyFont.GetHeight(graphics) + lineSpacing;
            graphics.DrawString("1 horizontal hex step = 15000000 m", bodyFont, Brushes.White, box.Left + leftPadding, y);
            y += bodyFont.GetHeight(graphics) + lineSpacing;
            graphics.DrawString("1 vertical half-step = 8660000 m", bodyFont, Brushes.White, box.Left + leftPadding, y);
        }

        private RectangleF GetMapBounds()
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var hex in _hexagons.Values)
            {
                foreach (var point in hex.Points)
                {
                    minX = Math.Min(minX, point.X);
                    minY = Math.Min(minY, point.Y);
                    maxX = Math.Max(maxX, point.X);
                    maxY = Math.Max(maxY, point.Y);
                }

                foreach (var child in hex.Children)
                {
                    foreach (var point in child.Points)
                    {
                        minX = Math.Min(minX, point.X);
                        minY = Math.Min(minY, point.Y);
                        maxX = Math.Max(maxX, point.X);
                        maxY = Math.Max(maxY, point.Y);
                    }
                }
            }

            if (minX == float.MaxValue || minY == float.MaxValue)
            {
                return new RectangleF(0, 0, 1, 1);
            }

            return RectangleF.FromLTRB(minX, minY, maxX, maxY);
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

            // Close or keep open behaviour
            if (!IsMapOptionChecked(MapOption.Keep_Window_Open))
            {
                Close();
            }
            else
            {
                // Keep map open: exit selection mode and update UI
                GateSectorSelection = false;
                ClusterSectorSelection = false;
                BtnSelectLocation.Enabled = false;
                BtnSelectLocation.Hide();
                Invalidate();
            }
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

        internal struct GateConnection
        {
            public GateData Source { get; set; }
            public GateData Target { get; set; }
        }

        private class HighwayEndpointDragState
        {
            public GateConnection Connection { get; set; }
            public Cluster Cluster { get; set; }
            public Sector Sector { get; set; }
            public Zone Zone { get; set; }
            public Gate Gate { get; set; }
            public PointF StartMouseScreen { get; set; }
            public PointF OriginalScreen { get; set; }
            public Point OriginalGatePosition { get; set; }
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

        private void MapOptionsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _mapOptionsSelected[MapOptionsListBox.Items[e.Index] as string] = e.NewValue == CheckState.Checked;
            Invalidate();
        }
    }
}
