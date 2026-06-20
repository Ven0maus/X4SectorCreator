using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
#if DEBUG
using System.Text.Json.Nodes;
#endif
using System.Text.RegularExpressions;
using System.Xml.Linq;
using X4SectorCreator.Configuration;
using X4SectorCreator.Forms;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Helpers;
using X4SectorCreator.MdGeneration;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;
using Region = X4SectorCreator.Objects.Region;

namespace X4SectorCreator
{
    public partial class MainForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static MainForm Instance { get; private set; }

        /* FORMS */
        public readonly LazyEvaluated<RegionForm> RegionForm = new(() => new RegionForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<SectorMapForm> SectorMap = new(() => new SectorMapForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<ClusterForm> ClusterForm = new(() => new ClusterForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<GateForm> GateForm = new(() => new GateForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<JobsForm> JobsForm = new(() => new JobsForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<FactoriesForm> FactoriesForm = new(() => new FactoriesForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<FactionsForm> FactionsForm = new(() => new FactionsForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<GalaxySettingsForm> GalaxySettingsForm = new(() => new GalaxySettingsForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<FactionRelationsForm> FactionRelationsDataForm = new(() => new FactionRelationsForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<SectorForm> SectorForm = new(() => new SectorForm(), a => !a.IsDisposed);

        private readonly LazyEvaluated<VersionUpdateForm> _versionUpdateForm = new(() => new VersionUpdateForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<StationForm> _stationForm = new(() => new StationForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<ObjectOverviewForm> _objectOverviewForm = new(() => new ObjectOverviewForm(), a => !a.IsDisposed);
        /* END OF FORMS */

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<(int, int), Cluster> AllClusters { get; private set; }

        public readonly Dictionary<string, string> BackgroundVisualMapping;
        public readonly Dictionary<string, string> DlcMappings;
        public readonly Dictionary<string, Color> FactionColorMapping;

        private ClusterOption _selectedClusterOption = ClusterOption.Custom;
        private string _currentModTargetVersion;

        private string _currentConfiguration;
        private readonly StartupOptions _startupOptions;
        private readonly ContextMenuStrip _renameContextMenu = new();
        private readonly ToolStripMenuItem _renameContextMenuItem = new("Edit Name...");
        private readonly ToolStripMenuItem _editSectorDataContextMenuItem = new("Edit Sector Data...");
        private readonly ToolStripMenuItem _editSectorRawDataContextMenuItem = new("Edit Sector Data (Raw)...");
        private readonly ToolStripMenuItem _editSectorTranslationContextMenuItem = new("Edit Translation Data...");
        private ListBox _renameContextSource;

        public MainForm(StartupOptions startupOptions = null)
        {
            LogFileHelper.AppendToSessionLog("MainForm", "Constructor start.");
            InitializeComponent();

            _startupOptions = startupOptions ?? new StartupOptions();

            if (Instance != null)
            {
                throw new Exception("No more than one instance of \"MainForm\" can be active.");
            }

            Instance = this;

            InitializeRenameContextMenu();

#if DEBUG
            // Used to move sectors around and save it to the mapping file (only available in debug mode)
            BtnSaveSectorMapping.Visible = true;
            BtnSaveSectorMapping.Enabled = true;
#endif

            TxtSearch.EnableTextSearch(() => AllClusters.Values.ToList(), a => a.Name, ApplyFilter);
            Disposed += MainForm_Disposed;
            ClusterCollection clusterCollection = InitAllVanillaClusters();
            LogFileHelper.AppendToSessionLog("MainForm", $"Loaded vanilla clusters: {clusterCollection?.Clusters?.Count ?? 0}.");

            // Set background visual mapping
            BackgroundVisualMapping = AllClusters
                .Where(a => a.Value.IsBaseGame)
                .Where(a => !string.IsNullOrWhiteSpace(a.Value.BackgroundVisualMapping))
                .ToDictionary(a => a.Value.Name, a => a.Value.BackgroundVisualMapping, StringComparer.OrdinalIgnoreCase);

            // Set dlc mappings
            string json = File.ReadAllText(Constants.DataPaths.DlcMappingFilePath);
            DlcMappings = JsonSerializer.Deserialize<List<DlcMapping>>(json, ConfigSerializer.JsonSerializerOptions)
                .ToDictionary(a => a.Dlc, a => a.Prefix);

            // Set faction color mapping
            FactionColorMapping = clusterCollection.FactionColors.ToDictionary(a => a.Key, a => a.Value.HexToColor(), StringComparer.OrdinalIgnoreCase);

            // Set the default value to be custom always
            UpdateClusterOptions();

            _currentConfiguration = ExportJsonConfig();
        }

        private void InitializeRenameContextMenu()
        {
            _renameContextMenu.Items.Add(_renameContextMenuItem);
            _renameContextMenu.Items.Add(new ToolStripSeparator());
            _renameContextMenu.Items.Add(_editSectorDataContextMenuItem);
            _renameContextMenu.Items.Add(_editSectorRawDataContextMenuItem);
            _renameContextMenu.Items.Add(_editSectorTranslationContextMenuItem);
            _renameContextMenuItem.Click += RenameContextMenuItem_Click;
            _editSectorDataContextMenuItem.Click += EditSectorDataContextMenuItem_Click;
            _editSectorRawDataContextMenuItem.Click += EditSectorRawDataContextMenuItem_Click;
            _editSectorTranslationContextMenuItem.Click += EditSectorTranslationContextMenuItem_Click;

            ClustersListBox.ContextMenuStrip = _renameContextMenu;
            SectorsListBox.ContextMenuStrip = _renameContextMenu;
            ClustersListBox.MouseDown += ListBox_MouseDownForRename;
            SectorsListBox.MouseDown += ListBox_MouseDownForRename;
        }

        public void SetProceduralGalaxy(IEnumerable<Cluster> allClusters)
        {
            Reset(false, resetGalaxyType: false, resetStatics: false);
            AllClusters = allClusters.ToDictionary(a => (a.Position.X, a.Position.Y), a => a);
        }

        private void MainForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        private void ApplyFilter(List<Cluster> clusters)
        {
            // Reset
            CmbClusterOption_SelectedIndexChanged(this, null);

            var clusterSet = clusters.ToHashSet();

            // Apply text filter search
            foreach (Cluster item in ClustersListBox.Items.Cast<Cluster>().ToList())
            {
                if (!clusterSet.Contains(item))
                    ClustersListBox.Items.Remove(item);
                else if (!ClustersListBox.Items.Contains(item))
                    ClustersListBox.Items.Add(item);
            }

            if (ClustersListBox.SelectedItem is Cluster selectedCluster && ClustersListBox.Items.Contains(selectedCluster))
                ClustersListBox.SelectedItem = selectedCluster;
            else if (ClustersListBox.Items.Count > 0)
                ClustersListBox.SelectedIndex = 0;
            else
                ClustersListBox.SelectedIndex = -1;
        }

        private void BtnSaveSectorMapping_Click(object sender, EventArgs e)
        {
#if DEBUG
            // Load the JSON as a mutable DOM
            var jsonText = File.ReadAllText(Constants.DataPaths.SectorMappingFilePath);
            var root = JsonNode.Parse(jsonText)!;

            // Navigate to clusters
            var clusters = root["Clusters"]?.AsArray();
            if (clusters == null) return;

            foreach (var clusterNode in clusters)
            {
                var baseGameMapping = clusterNode?["BaseGameMapping"]?.ToString();
                if (string.IsNullOrEmpty(baseGameMapping)) continue;

                // Find corresponding runtime cluster
                var match = AllClusters.Values.FirstOrDefault(a => a.IsBaseGame && a.BaseGameMapping == baseGameMapping);
                if (match == null) continue;

                var point = clusterNode["Position"].Deserialize<Point>();
                if (point.X != match.Position.X || point.Y != match.Position.Y)
                {
                    // Update only the Position field
                    var position = new JsonObject
                    {
                        ["X"] = match.Position.X,
                        ["Y"] = match.Position.Y
                    };

                    clusterNode["Position"] = position;

                    Debug.WriteLine($"Cluster \"{match.Name}\" position updated.");
                }
            }

            // Use custom one with custom encoder
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter(), new Configuration.Converters.ColorJsonConverter() },
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            // Save only modified JSON (with original structure preserved)
            File.WriteAllText(Constants.DataPaths.SectorMappingFilePath, root.ToJsonString(jsonSerializerOptions));
#endif
        }

        private void BtnGuide_Click(object sender, EventArgs e)
        {
            const string tutorialUrl = "https://www.youtube.com/watch?v=CywvNiwQGTs";

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = tutorialUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Unable to open the tutorial link.\n\n{tutorialUrl}\n\n{ex.Message}", "Tutorial / Guide");
            }
        }

        #region Initialization
        public ClusterCollection InitAllVanillaClusters(bool replaceAllClusters = true)
        {
            string json = File.ReadAllText(Constants.DataPaths.SectorMappingFilePath);
            ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(json, ConfigSerializer.JsonSerializerOptions);

            Dictionary<(int X, int Y), Cluster> clusterLookup = clusterCollection.Clusters.ToDictionary(a => (a.Position.X, a.Position.Y));
            // Create lookups
            AllClusters = replaceAllClusters ? clusterLookup : AllClusters;

            // Init all collections
            foreach (KeyValuePair<(int, int), Cluster> cluster in clusterLookup)
            {
                // Init base game mapping
                if (cluster.Value.IsBaseGame && cluster.Value.BackgroundVisualMapping == null)
                {
                    cluster.Value.BackgroundVisualMapping = cluster.Value.BaseGameMapping;
                }

                if (cluster.Value.Sectors.Count > 1 && cluster.Value.Sectors.All(a => a.Placement == default))
                {
                    throw new Exception($"Invalid sector offset configuration for cluster \"{cluster.Value.Name} | {cluster.Value.BaseGameMapping}\".");
                }

                // By default all vanilla multi clusters should have custom positioning enabled
                if (cluster.Value.IsBaseGame && cluster.Value.Sectors.Count > 1)
                {
                    cluster.Value.CustomSectorPositioning = true;
                }

                foreach (Sector sector in cluster.Value.Sectors)
                {
                    // Init regular sectors
                    if (cluster.Value.IsBaseGame && string.IsNullOrWhiteSpace(sector.BaseGameMapping))
                    {
                        sector.BaseGameMapping = "sector001";
                    }

                    sector.Regions ??= [];
                    sector.Zones ??= [];
                    sector.ResourceAreas ??= [];
                    foreach (Zone zone in sector.Zones)
                    {
                        zone.Gates ??= [];
                    }

                    // Auto-determine offset for each sector
                    Forms.SectorForm.DetermineSectorOffset(cluster.Value, sector);
                }
            }

            InitializeVanillaRegionsAndStations(clusterLookup);
            InitializeVanillaResourceAreas(clusterLookup);

            // Create also the required connections for vanilla
            VanillaGateConnectionParser.CreateVanillaGateConnections(clusterLookup);

            return clusterCollection;
        }

        private static void InitializeVanillaRegionsAndStations(Dictionary<(int x, int y), Cluster> allClusters)
        {
            string regionsJson = File.ReadAllText(Constants.DataPaths.VanillaRegionsMappingFilePath);
            ClusterCollection regionCollection = JsonSerializer.Deserialize<ClusterCollection>(regionsJson, ConfigSerializer.JsonSerializerOptions);
            foreach (var refCluster in regionCollection.Clusters)
            {
                // Find matching cluster
                var cluster = allClusters.Values
                    .FirstOrDefault(a => a.IsBaseGame &&
                        a.BaseGameMapping.Equals(refCluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
                if (cluster != null)
                {
                    foreach (var refSector in refCluster.Sectors)
                    {
                        // Find matching sector
                        var sector = cluster.Sectors.FirstOrDefault(a => a.IsBaseGame &&
                            a.BaseGameMapping.Equals(refSector.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
                        if (sector != null)
                        {
                            sector.SectorRealOffset = refSector.SectorRealOffset;
                            foreach (var region in refSector.Regions)
                            {
                                region.IsBaseGame = true;
                                sector.Regions.Add(region);
                            }
                        }
                    }
                }
            }

            string stationsJson = File.ReadAllText(Constants.DataPaths.VanillaStationsMappingFilePath);
            ClusterCollection stationsCollection = JsonSerializer.Deserialize<ClusterCollection>(stationsJson, ConfigSerializer.JsonSerializerOptions);
            foreach (var refCluster in stationsCollection.Clusters)
            {
                // Find matching cluster
                var cluster = allClusters.Values
                    .FirstOrDefault(a => a.IsBaseGame &&
                        a.BaseGameMapping.Equals(refCluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
                if (cluster != null)
                {
                    foreach (var refSector in refCluster.Sectors)
                    {
                        // Find matching sector
                        var sector = cluster.Sectors.FirstOrDefault(a => a.IsBaseGame &&
                            a.BaseGameMapping.Equals(refSector.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
                        if (sector != null)
                        {
                            foreach (var zone in refSector.Zones)
                            {
                                sector.Zones ??= [];
                                sector.Zones.Add(zone);
                            }
                        }
                    }
                }
            }
        }

        private static void InitializeVanillaResourceAreas(Dictionary<(int x, int y), Cluster> allClusters)
        {
            var doc = XDocument.Load(Constants.DataPaths.VanillaMapDefaultsFilePath);
            var datasets = doc.Element("defaults").Elements("dataset");

            var clustersByMapping = allClusters.Values
                .SelectMany(a => a.Sectors, (a, b) => (Cluster: a, Sector: b))
                .ToDictionary(a => $"{a.Cluster.BaseGameMapping}_{a.Sector.BaseGameMapping.CapitalizeFirstLetter()}");

            foreach (var dataset in datasets)
            {
                var macro = dataset.Attribute("macro")?.Value;
                if (string.IsNullOrEmpty(macro)) continue;

                macro = macro.Replace("_macro", string.Empty);

                if (!clustersByMapping.TryGetValue(macro, out var clusterSectorMap))
                    continue;

                var properties = dataset.Element("properties");
                if (properties != null)
                {
                    var resourceAreasElement = properties.Element("resourceareas");
                    if (resourceAreasElement != null)
                    {
                        var resourceAreas = resourceAreasElement.Elements("resourcearea");
                        foreach (var resourceArea in resourceAreas)
                        {
                            var @ref = resourceArea.Attribute("ref")?.Value;
                            var amount = resourceArea.Attribute("amount")?.Value;
                            if (!string.IsNullOrEmpty(@ref) && !string.IsNullOrEmpty(amount))
                            {
                                var parts = @ref.Split('_');
                                if (parts.Length != 5) continue;

                                var size = parts[1];
                                var ware = parts[2];
                                var yield = parts[3];
                                var speed = parts[4];

                                var resource = new Resource
                                {
                                    Size = size,
                                    Ware = ware,
                                    Yield = yield,
                                    Speed = speed,
                                    Amount = int.TryParse(amount, out var amountValue) ? amountValue : 0,
                                    IsBaseGame = true
                                };

                                clusterSectorMap.Sector.ResourceAreas.Add(resource);
                            }
                        }
                    }
                }
            }
        }

        private void CmbClusterOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbClusterOption.SelectedItem is not ClusterOption selectedValue)
            {
                return;
            }

            _selectedClusterOption = selectedValue;

            ClustersListBox.Items.Clear();
            SectorsListBox.Items.Clear();
            GatesListBox.Items.Clear();
            RegionsListBox.Items.Clear();
            ListStations.Items.Clear();
            LblDetails.Text = string.Empty;
            if (e != null) // if not called from apply filter method
                TxtSearch.Text = string.Empty;

            switch (_selectedClusterOption)
            {
                case ClusterOption.Custom:
                    foreach (Cluster cluster in AllClusters.Values.Where(a => !a.IsBaseGame).OrderBy(a => a.Name))
                    {
                        _ = ClustersListBox.Items.Add(cluster);
                    }

                    break;
                case ClusterOption.Vanilla:
                    foreach (Cluster cluster in AllClusters.Values.Where(a => a.IsBaseGame).OrderBy(a => a.Name))
                    {
                        _ = ClustersListBox.Items.Add(cluster);
                    }

                    break;
                case ClusterOption.Both:
                    foreach (Cluster cluster in AllClusters.Values.OrderBy(a => a.Name))
                    {
                        _ = ClustersListBox.Items.Add(cluster);
                    }

                    break;
                default:
                    throw new NotImplementedException(selectedValue.ToString());
            }
            ClustersListBox.SelectedIndex = ClustersListBox.Items.Count == 0 ? -1 : 0;
        }

        public void UpdateClusterOptions()
        {
            if (Forms.GalaxySettingsForm.IsCustomGalaxy)
            {
                cmbClusterOption.Items.Clear();
                _ = cmbClusterOption.Items.Add(ClusterOption.Custom);
                cmbClusterOption.SelectedItem = ClusterOption.Custom;
            }
            else
            {
                cmbClusterOption.Items.Clear();
                _ = cmbClusterOption.Items.Add(ClusterOption.Custom);
                _ = cmbClusterOption.Items.Add(ClusterOption.Vanilla);
                _ = cmbClusterOption.Items.Add(ClusterOption.Both);
                cmbClusterOption.SelectedItem = ClusterOption.Custom;
            }
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            VersionChecker versionChecker = new();

            // Set form title
            Text += $" [APP v{versionChecker.CurrentVersion} | X4 v{versionChecker.TargetGameVersion}]";
            _currentModTargetVersion = versionChecker.ModTargetGameVersion;

            // Check for update
            (bool NewVersionAvailable, VersionInfo VersionInfo) result = await versionChecker.CheckForUpdatesAsync();
            if (result.NewVersionAvailable)
            {
                // If the app version remains the same, but the X4 version is different
                // That means only the mapping was updated, we can automatically update this.
                if (result.VersionInfo.AppVersion.Equals(versionChecker.CurrentVersion))
                {
                    string newSectorMappingJson = await VersionChecker.GetUpdatedSectorMappingAsync();
                    string oldSectorMappingJson = File.ReadAllText(Constants.DataPaths.SectorMappingFilePath);
                    if (newSectorMappingJson != null && !oldSectorMappingJson.Equals(newSectorMappingJson))
                    {
                        try
                        {
                            // Update mapping file
                            File.WriteAllText(Constants.DataPaths.SectorMappingFilePath, newSectorMappingJson);
                            ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(newSectorMappingJson, ConfigSerializer.JsonSerializerOptions);

                            // Replace clusters
                            Dictionary<(int X, int Y), Cluster> newClusters = clusterCollection.Clusters.ToDictionary(a => (a.Position.X, a.Position.Y));
                            if (newClusters.Count > 0)
                            {
                                AllClusters.Clear();
                                foreach (KeyValuePair<(int X, int Y), Cluster> cluster in newClusters)
                                {
                                    AllClusters[cluster.Key] = cluster.Value;
                                }
                            }

                            // Update X4 version file
                            versionChecker.UpdateX4Version(result.VersionInfo);

                            // Update title text with new version
                            Text += $" [APP v{versionChecker.CurrentVersion} | X4 v{versionChecker.TargetGameVersion}]";
                            _currentModTargetVersion = versionChecker.ModTargetGameVersion;

                            _ = MessageBox.Show($"Your cluster mapping has been automatically updated with the latest X4 version ({result.VersionInfo.X4Version}).");
                        }
                        catch (Exception)
                        {
                            // Don't do anything
                            _ = MessageBox.Show($"A new cluster mapping is available for X4 version ({result.VersionInfo.X4Version}) but was unable to download it, please update manually.");
                        }
                    }
                }
                else
                {
                    // Show update form when a new app version is available
                    _versionUpdateForm.Value.txtCurrentVersion.Text = $"v{versionChecker.CurrentVersion}";
                    _versionUpdateForm.Value.txtCurrentX4Version.Text = $"v{versionChecker.TargetGameVersion}";
                    _versionUpdateForm.Value.txtUpdateVersion.Text = $"v{result.VersionInfo.AppVersion}";
                    _versionUpdateForm.Value.txtUpdateX4Version.Text = $"v{result.VersionInfo.X4Version}";
                    _versionUpdateForm.Value.Show();
                }
            }

            // Screen scaling warning to prevent confusion for some users.
            using Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            if (g.DpiX != 96)
            {
                int usersDpiSetting = (int)(g.DpiX / 96f * 100);
                _ = MessageBox.Show($"Dear user, you are using a screen scaling setting of {usersDpiSetting}%\n" +
                    "The tool is created specifically for 100% screen scaling option.\n" +
                    "Some UI controls may not be aligned properly, this is very noticable on the sector map.\n" +
                    "Please change your screen scale setting to 100% to be able to properly use this tool.", "Incompatible DPI warning", MessageBoxButtons.OK);
            }

            LogFileHelper.AppendToSessionLog("MainForm", "Invoking ProcessStartupOptions.");
            ProcessStartupOptions();
            LogFileHelper.AppendToSessionLog("MainForm", "Constructor complete.");
        }
        #endregion

        #region Galaxy Settings
        /// <summary>
        /// Used to toggle between base game galaxy and custom galaxy.
        /// </summary>
        public void ToggleGalaxyMode(Dictionary<(int, int), Cluster> mergedClusters)
        {
            AllClusters = Forms.GalaxySettingsForm.IsCustomGalaxy
                ? AllClusters
                    .Where(a => !a.Value.IsBaseGame)
                    .ToDictionary(a => a.Key, a => a.Value)
                : mergedClusters;

            UpdateClusterOptions();
        }

        private void BtnGalaxySettings_Click(object sender, EventArgs e)
        {
            GalaxySettingsForm.Value.Initialize();
            GalaxySettingsForm.Value.Show();
        }
        #endregion

        #region Mod Generation
        private void BtnGenerateDiffs_Click(object sender, EventArgs e)
        {
            List<string> unnamedItems = [];
            foreach (Cluster cluster in AllClusters.Values)
            {
                if (string.IsNullOrWhiteSpace(cluster.Name))
                {
                    unnamedItems.Add($"Cluster: {cluster.ImportedMacroName ?? cluster.BaseGameMapping ?? "<unknown>"}");
                }

                foreach (Sector sector in cluster.Sectors)
                {
                    if (string.IsNullOrWhiteSpace(sector.Name))
                    {
                        unnamedItems.Add($"Sector: {sector.ImportedMacroName ?? sector.BaseGameMapping ?? "<unknown>"} (cluster {cluster.ImportedMacroName ?? cluster.BaseGameMapping ?? cluster.Name ?? "<unknown>"})");
                    }
                }
            }

            if (unnamedItems.Count > 0)
            {
                _ = MessageBox.Show(
                    "Mod XML export is blocked until all missing cluster and sector names are defined:\n\n- " + string.Join("\n- ", unnamedItems),
                    "Missing names block export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validate if all clusters have atleast one sector
            Cluster[] invalidClusters = AllClusters.Values
                .Where(a => a.Sectors == null || a.Sectors.Count == 0)
                .ToArray();
            if (invalidClusters.Length != 0)
            {
                _ = MessageBox.Show($"Following clusters have no sectors, please fix these first:\n- " +
                    string.Join("\n- ", invalidClusters.Select(a => a.Name)));
                return;
            }

            // Validate if all clusters have valid sector placements
            invalidClusters = AllClusters.Values
                .Where(a => !Forms.SectorForm.IsClusterPlacementValid(a))
                .ToArray();
            if (invalidClusters.Length != 0)
            {
                _ = MessageBox.Show($"Following clusters have sectors that have overlapped or invalid placements, please fix these first:\n- " +
                    string.Join("\n- ", invalidClusters.Select(a => a.Name)));
                return;
            }

            const string lblModName = "Please enter the name you'd like to use for your mod folder:";
            const string lblModPrefix = "Please enter the prefix you'd like to use for your mod:";
            const string lblPageId = "Page Id you'd like to use for t-files (leave empty to auto generate):";
            Dictionary<string, string> modInfo = MultiInputDialog.Show("Mod information",
                (lblModName, null, null),
                (lblModPrefix, null, null),
                (lblPageId, null, null)
            );
            if (modInfo == null || modInfo.Count == 0)
            {
                return;
            }

            string modName = modInfo[lblModName];
            string modPrefix = modInfo[lblModPrefix];
            string pageIdStr = modInfo[lblPageId];

            // Sanitize prefix
            modPrefix = SanitizeText(modPrefix)?.ToLower();

            if (string.IsNullOrWhiteSpace(modName))
            {
                _ = MessageBox.Show($"Please enter a valid non empty non whitespace mod folder name.");
                return;
            }
            if (string.IsNullOrWhiteSpace(modPrefix))
            {
                _ = MessageBox.Show($"Please enter a valid non empty non whitespace mod prefix.");
                return;
            }

            // Initialize localisation
            if (string.IsNullOrWhiteSpace(pageIdStr) || !int.TryParse(pageIdStr, out var pageId))
            {
                Localisation.Initialize(modName, modPrefix);
            }
            else
            {
                Localisation.Initialize(pageId);
            }

            // Generate each xml
            string mainFolder = Constants.DataPaths.ModDirectoryPath;
            string modFolder = Path.Combine(mainFolder, modName);

            try
            {
                // Clear up any previous xml
                if (Directory.Exists(mainFolder))
                {
                    Directory.Delete(mainFolder, true);
                }

                List<Cluster> clusters = [.. AllClusters.Values];

                // Collects all changes done to base game content
                ClusterCollection nonModifiedBaseGameData = InitAllVanillaClusters(false);
                VanillaChanges vanillaChanges = CollectVanillaChanges(nonModifiedBaseGameData);

                // Generate all xml files
                var actions = new List<Action>
                {
                    () => MacrosGeneration.Generate(modFolder, modName, modPrefix, clusters),
                    () => MapDefaultsGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges),
                    () => GalaxyGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges, nonModifiedBaseGameData),
                    () => ClusterGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges),
                    () => SectorGeneration.Generate(modFolder, modPrefix, clusters, nonModifiedBaseGameData, vanillaChanges),
                    () => ZoneGeneration.Generate(modFolder, modPrefix, clusters, nonModifiedBaseGameData, vanillaChanges),
                    () => ContentGeneration.Generate(modFolder, modName, _currentModTargetVersion.Replace(".", string.Empty), clusters, vanillaChanges),
                    () => RegionDefinitionGeneration.Generate(modFolder, modPrefix, clusters),
                    () => GameStartsGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges),
                    () => DlcDisableGeneration.Generate(modFolder),
                    () => GodGeneration.Generate(modFolder, modPrefix, clusters),
                    () => JobsGeneration.Generate(modFolder, modPrefix),
                    () => BasketsGeneration.Generate(modFolder, modPrefix),
                    () => FactionsGeneration.Generate(modFolder),
                    () => ColorsGeneration.Generate(modFolder),
                    () => IconsGeneration.Generate(modFolder, modName),
                    () => ShipsGeneration.Generate(modFolder),
                    () => ShipGroupsGeneration.Generate(modFolder),
                    () => StationsGeneration.Generate(modFolder, modPrefix),
                    () => StationGroupsGeneration.Generate(modFolder, modPrefix),
                    () => ConstructionplansGeneration.Generate(modFolder, modPrefix),
                    () => ModulesGeneration.Generate(modFolder),
                    () => LoadoutrulesGeneration.Generate(modFolder),
                    () => PaintmodsGeneration.Generate(modFolder),
                    () => ThemesGeneration.Generate(modFolder),
                    () => CharactersGeneration.Generate(modFolder),
                    () => WaresGeneration.Generate(modFolder),
                    () => FactionLogicGeneration.Generate(modPrefix, modFolder),
                    () => FactionSetupGeneration.Generate(modFolder),
                    () => KillMassTrafficGeneration.Generate(modFolder),
                    () => SignalLeaksGeneration.Generate(modFolder),
                    () => FactionLogicEconomyGeneration.Generate(modFolder),
                    () => FactionLogicStationsGeneration.Generate(modFolder),
                    () => WarSubscriptionsGeneration.Generate(modFolder),
                    () => DrainStationsGeneration.Generate(modFolder),
                    () => FinaliseStationsGeneration.Generate(modFolder),
                    () => GmcDynamicGeneration.Generate(modFolder),
                    () => FactionGoalInvadeSpaceGeneration.Generate(modFolder),
                    () => ComponentsGeneration.Generate(modFolder, modPrefix, modName),
                    () => PlayerReputationGeneration.Generate(modFolder),
                    () => SetupGeneration.Generate(modFolder),
                    () => PlayerHqGeneration.Generate(modFolder, modPrefix),

                    // Localisation after all the generation
                    () => Localisation.LocaliseAllFiles(modFolder)
                };

                GenerateModProgressBar.Minimum = 0;
                GenerateModProgressBar.Maximum = actions.Count;
                GenerateModProgressBar.Value = 0;
                GenerateModProgressBar.Step = 1;

                foreach (var action in actions)
                {
                    action.Invoke();
                    GenerateModProgressBar.PerformStep();
                }
            }
            catch (Exception ex)
            {
                // Clear up corrupted xml
                if (Directory.Exists(mainFolder))
                    Directory.Delete(mainFolder, true);
#if DEBUG
                throw;
#else
                _ = MessageBox.Show("Something went wrong during xml generation: \"" + ex.Message + "\".\nPlease create a bug report. (Be sure to provide the export xml or exact reproduction steps)",
                    "Error in XML Generation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
#endif
            }
            finally
            {
                Localisation.ClearCache(true);
            }

            // Show succes message
            _ = MessageBox.Show("XML Files were succesfully generated in the xml folder.");
            GenerateModProgressBar.Value = 0;
        }

        public static string SanitizeText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            // Trim leading and trailing whitespace
            string sanitizedText = input.Trim();

            // Replace spaces with empty
            sanitizedText = sanitizedText.Replace(" ", "");

            // Replace other unsafe characters with empty
            sanitizedText = SanitizeUnsafe().Replace(sanitizedText, "");

            // Optionally, remove any non-alphanumeric characters (including underscores)
            sanitizedText = SanitizeNonAlphaNumeric().Replace(sanitizedText, "_");

            // Ensure the string isn't empty
            return string.IsNullOrWhiteSpace(sanitizedText) ? null : sanitizedText;
        }

        private Cluster GetSelectedCluster()
        {
            if (ClustersListBox.SelectedItem is Cluster cluster)
                return cluster;

            if (ClustersListBox.SelectedItem is string selectedClusterName)
                return AllClusters.Values.FirstOrDefault(a => string.Equals(a.Name, selectedClusterName, StringComparison.OrdinalIgnoreCase));

            return null;
        }

        private Sector GetSelectedSector(Cluster cluster = null)
        {
            if (SectorsListBox.SelectedItem is Sector sector)
                return sector;

            cluster ??= GetSelectedCluster();
            if (cluster == null)
                return null;

            if (SectorsListBox.SelectedItem is string selectedSectorName)
                return cluster.Sectors.FirstOrDefault(a => string.Equals(a.Name, selectedSectorName, StringComparison.OrdinalIgnoreCase));

            return null;
        }

        private void ListBox_MouseDownForRename(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || sender is not ListBox listBox)
                return;

            int index = listBox.IndexFromPoint(e.Location);
            if (index < 0)
            {
                _renameContextSource = null;
                return;
            }

            listBox.SelectedIndex = index;
            _renameContextSource = listBox;
            bool isSectorList = ReferenceEquals(listBox, SectorsListBox);
            _editSectorDataContextMenuItem.Visible = isSectorList;
            _editSectorRawDataContextMenuItem.Visible = isSectorList;
            _editSectorTranslationContextMenuItem.Visible = isSectorList;
        }

        private void RenameContextMenuItem_Click(object sender, EventArgs e)
        {
            if (_renameContextSource == ClustersListBox && GetSelectedCluster() is Cluster cluster)
            {
                if (PromptForRequiredName("Edit Cluster Name", cluster.Name, out string updatedName))
                {
                    cluster.Name = updatedName;
                    RefreshListBoxDisplay(ClustersListBox);
                    UpdateDetailsText();
                }
            }
            else if (_renameContextSource == SectorsListBox && GetSelectedSector() is Sector sector)
            {
                if (PromptForRequiredName("Edit Sector Name", sector.Name, out string updatedName))
                {
                    sector.Name = updatedName;
                    RefreshListBoxDisplay(SectorsListBox);
                    UpdateDetailsText();
                }
            }
        }

        private void EditSectorDataContextMenuItem_Click(object sender, EventArgs e)
        {
            if (_renameContextSource != SectorsListBox || GetSelectedSector() is not Sector sector)
                return;

            if (PromptForSectorMacroData(sector, out string updatedName, out string updatedDescription, out int updatedRadiusKm, out int updatedSunlight, out int updatedEconomy, out int updatedSecurity, out bool allowRandomAnomalies, out bool disableFactionLogic))
            {
                sector.Name = updatedName;
                sector.Description = updatedDescription;
                sector.DiameterRadius = updatedRadiusKm * 2 * 1000;
                sector.Sunlight = (float)Math.Round(updatedSunlight / 100f, 2);
                sector.Economy = (float)Math.Round(updatedEconomy / 100f, 2);
                sector.Security = (float)Math.Round(updatedSecurity / 100f, 2);
                sector.AllowRandomAnomalies = allowRandomAnomalies;
                sector.DisableFactionLogic = disableFactionLogic;

                RefreshListBoxDisplay(SectorsListBox);
                UpdateDetailsText();
            }
        }

        private void EditSectorTranslationContextMenuItem_Click(object sender, EventArgs e)
        {
            if (_renameContextSource != SectorsListBox || GetSelectedSector() is not Sector sector)
                return;

            if (PromptForTranslationData("Edit Sector Translation Data", sector.Name, sector.Description, out string updatedName, out string updatedDescription))
            {
                sector.Name = updatedName;
                sector.Description = updatedDescription;
                RefreshListBoxDisplay(SectorsListBox);
                UpdateDetailsText();
            }
        }

        private void EditSectorRawDataContextMenuItem_Click(object sender, EventArgs e)
        {
            if (_renameContextSource != SectorsListBox || GetSelectedSector() is not Sector sector)
                return;

            if (PromptForRawSectorData(sector, out Sector updatedSector))
            {
                ApplyRawSectorData(sector, updatedSector);
                RefreshListBoxDisplay(SectorsListBox);
                UpdateDetailsText();
            }
        }

        private static void RefreshListBoxDisplay(ListBox listBox)
        {
            int selectedIndex = listBox.SelectedIndex;
            listBox.BeginUpdate();
            listBox.EndUpdate();
            listBox.Refresh();
            if (selectedIndex >= 0 && selectedIndex < listBox.Items.Count)
                listBox.SelectedIndex = selectedIndex;
        }

        private static bool PromptForRequiredName(string caption, string initialValue, out string updatedName)
        {
            updatedName = null;

            using Form dialog = new()
            {
                Text = caption,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(420, 110)
            };

            TextBox textBox = new()
            {
                Left = 12,
                Top = 12,
                Width = 396,
                Text = initialValue ?? string.Empty
            };

            Button okButton = new()
            {
                Text = "OK",
                Left = 252,
                Top = 52,
                Width = 75,
                DialogResult = DialogResult.OK
            };

            Button cancelButton = new()
            {
                Text = "Cancel",
                Left = 333,
                Top = 52,
                Width = 75,
                DialogResult = DialogResult.Cancel
            };

            okButton.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    _ = MessageBox.Show(dialog, "Name cannot be empty.", caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dialog.DialogResult = DialogResult.None;
                }
            };

            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;
            dialog.Controls.Add(textBox);
            dialog.Controls.Add(okButton);
            dialog.Controls.Add(cancelButton);

            if (dialog.ShowDialog(Instance) != DialogResult.OK)
                return false;

            updatedName = textBox.Text.Trim();
            return true;
        }

        private static bool PromptForTranslationData(string caption, string initialName, string initialDescription, out string updatedName, out string updatedDescription)
        {
            updatedName = null;
            updatedDescription = null;

            using Form dialog = new()
            {
                Text = caption,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(460, 250)
            };

            Label nameLabel = new() { Left = 12, Top = 15, Width = 90, Text = "Name:" };
            TextBox nameTextBox = new()
            {
                Left = 110,
                Top = 12,
                Width = 338,
                Text = initialName ?? string.Empty
            };

            Label descriptionLabel = new() { Left = 12, Top = 50, Width = 90, Text = "Description:" };
            TextBox descriptionTextBox = new()
            {
                Left = 110,
                Top = 47,
                Width = 338,
                Height = 145,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Text = initialDescription ?? string.Empty
            };

            Button okButton = new()
            {
                Text = "OK",
                Left = 292,
                Top = 205,
                Width = 75,
                DialogResult = DialogResult.OK
            };

            Button cancelButton = new()
            {
                Text = "Cancel",
                Left = 373,
                Top = 205,
                Width = 75,
                DialogResult = DialogResult.Cancel
            };

            okButton.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(nameTextBox.Text))
                {
                    _ = MessageBox.Show(dialog, "Name cannot be empty.", caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dialog.DialogResult = DialogResult.None;
                }
            };

            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;
            dialog.Controls.Add(nameLabel);
            dialog.Controls.Add(nameTextBox);
            dialog.Controls.Add(descriptionLabel);
            dialog.Controls.Add(descriptionTextBox);
            dialog.Controls.Add(okButton);
            dialog.Controls.Add(cancelButton);

            if (dialog.ShowDialog(Instance) != DialogResult.OK)
                return false;

            updatedName = nameTextBox.Text.Trim();
            updatedDescription = string.IsNullOrWhiteSpace(descriptionTextBox.Text) ? null : descriptionTextBox.Text.Trim();
            return true;
        }

        private bool PromptForSectorMacroData(
            Sector sector,
            out string updatedName,
            out string updatedDescription,
            out int updatedRadiusKm,
            out int updatedSunlight,
            out int updatedEconomy,
            out int updatedSecurity,
            out bool allowRandomAnomalies,
            out bool disableFactionLogic)
        {
            updatedName = null;
            updatedDescription = null;
            updatedRadiusKm = 0;
            updatedSunlight = 0;
            updatedEconomy = 0;
            updatedSecurity = 0;
            allowRandomAnomalies = false;
            disableFactionLogic = false;

            using Form dialog = new()
            {
                Text = "Edit Sector Data",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(470, 350)
            };

            int leftLabel = 12;
            int leftInput = 130;
            int widthInput = 320;

            Label nameLabel = new() { Left = leftLabel, Top = 15, Width = 110, Text = "Name:" };
            TextBox nameTextBox = new() { Left = leftInput, Top = 12, Width = widthInput, Text = sector.Name ?? string.Empty };

            Label descriptionLabel = new() { Left = leftLabel, Top = 47, Width = 110, Text = "Description:" };
            TextBox descriptionTextBox = new()
            {
                Left = leftInput,
                Top = 44,
                Width = widthInput,
                Height = 130,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Text = sector.Description ?? string.Empty
            };

            Label radiusLabel = new() { Left = leftLabel, Top = 185, Width = 110, Text = "Radius (km):" };
            TextBox radiusTextBox = new() { Left = leftInput, Top = 182, Width = 90, Text = ((int)(sector.DiameterRadius / 1000f / 2f)).ToString() };

            Label sunlightLabel = new() { Left = leftLabel, Top = 215, Width = 110, Text = "Sunlight (%):" };
            TextBox sunlightTextBox = new() { Left = leftInput, Top = 212, Width = 90, Text = ((int)(sector.Sunlight * 100)).ToString() };

            Label economyLabel = new() { Left = leftLabel, Top = 245, Width = 110, Text = "Economy (%):" };
            TextBox economyTextBox = new() { Left = leftInput, Top = 242, Width = 90, Text = ((int)(sector.Economy * 100)).ToString() };

            Label securityLabel = new() { Left = leftLabel, Top = 275, Width = 110, Text = "Security (%):" };
            TextBox securityTextBox = new() { Left = leftInput, Top = 272, Width = 90, Text = ((int)(sector.Security * 100)).ToString() };

            CheckBox allowAnomaliesCheckBox = new()
            {
                Left = 275,
                Top = 183,
                Width = 175,
                Text = "Allow random anomalies",
                Checked = sector.AllowRandomAnomalies
            };

            CheckBox disableFactionLogicCheckBox = new()
            {
                Left = 275,
                Top = 213,
                Width = 175,
                Text = "Disable faction logic",
                Checked = sector.DisableFactionLogic
            };

            Button okButton = new()
            {
                Text = "OK",
                Left = 292,
                Top = 310,
                Width = 75,
                DialogResult = DialogResult.OK
            };

            Button cancelButton = new()
            {
                Text = "Cancel",
                Left = 375,
                Top = 310,
                Width = 75,
                DialogResult = DialogResult.Cancel
            };

            okButton.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(nameTextBox.Text))
                {
                    _ = MessageBox.Show(dialog, "Name cannot be empty.", "Edit Sector Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dialog.DialogResult = DialogResult.None;
                    return;
                }

                bool invalidNumbers =
                    !int.TryParse(radiusTextBox.Text, out int radiusKm) || radiusKm <= 0 || radiusKm > 999 ||
                    !int.TryParse(sunlightTextBox.Text, out int sunlight) ||
                    !int.TryParse(economyTextBox.Text, out int economy) ||
                    !int.TryParse(securityTextBox.Text, out int security);

                if (invalidNumbers)
                {
                    _ = MessageBox.Show(dialog, "Please provide valid numerical values for radius, sunlight, economy, and security.", "Edit Sector Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dialog.DialogResult = DialogResult.None;
                    return;
                }

                string candidateName = nameTextBox.Text.Trim();
                bool duplicate = AllClusters.Values
                    .SelectMany(a => a.Sectors)
                    .Any(a => !ReferenceEquals(a, sector) && string.Equals(a.Name, candidateName, StringComparison.OrdinalIgnoreCase));

                if (duplicate)
                {
                    _ = MessageBox.Show(dialog, $"A sector with the name \"{candidateName}\" already exists.", "Edit Sector Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dialog.DialogResult = DialogResult.None;
                }
            };

            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;
            dialog.Controls.Add(nameLabel);
            dialog.Controls.Add(nameTextBox);
            dialog.Controls.Add(descriptionLabel);
            dialog.Controls.Add(descriptionTextBox);
            dialog.Controls.Add(radiusLabel);
            dialog.Controls.Add(radiusTextBox);
            dialog.Controls.Add(sunlightLabel);
            dialog.Controls.Add(sunlightTextBox);
            dialog.Controls.Add(economyLabel);
            dialog.Controls.Add(economyTextBox);
            dialog.Controls.Add(securityLabel);
            dialog.Controls.Add(securityTextBox);
            dialog.Controls.Add(allowAnomaliesCheckBox);
            dialog.Controls.Add(disableFactionLogicCheckBox);
            dialog.Controls.Add(okButton);
            dialog.Controls.Add(cancelButton);

            if (dialog.ShowDialog(Instance) != DialogResult.OK)
                return false;

            updatedName = nameTextBox.Text.Trim();
            updatedDescription = string.IsNullOrWhiteSpace(descriptionTextBox.Text) ? null : descriptionTextBox.Text.Trim();
            updatedRadiusKm = int.Parse(radiusTextBox.Text);
            updatedSunlight = int.Parse(sunlightTextBox.Text);
            updatedEconomy = int.Parse(economyTextBox.Text);
            updatedSecurity = int.Parse(securityTextBox.Text);
            allowRandomAnomalies = allowAnomaliesCheckBox.Checked;
            disableFactionLogic = disableFactionLogicCheckBox.Checked;
            return true;
        }

        private static bool PromptForRawSectorData(Sector sector, out Sector updatedSector)
        {
            updatedSector = null;
            Sector parsedSector = null;

            string json = JsonSerializer.Serialize(sector, ConfigSerializer.JsonSerializerOptions);

            using Form dialog = new()
            {
                Text = "Edit Sector Data (Raw)",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(700, 520)
            };

            TextBox textBox = new()
            {
                Left = 12,
                Top = 12,
                Width = 676,
                Height = 455,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                AcceptsReturn = true,
                AcceptsTab = true,
                WordWrap = false,
                Font = new Font("Consolas", 10F),
                Text = json
            };

            Button okButton = new()
            {
                Text = "OK",
                Left = 532,
                Top = 480,
                Width = 75,
                DialogResult = DialogResult.OK
            };

            Button cancelButton = new()
            {
                Text = "Cancel",
                Left = 613,
                Top = 480,
                Width = 75,
                DialogResult = DialogResult.Cancel
            };

            okButton.Click += (_, _) =>
            {
                try
                {
                    Sector parsed = JsonSerializer.Deserialize<Sector>(textBox.Text, ConfigSerializer.JsonSerializerOptions);
                    if (parsed == null)
                    {
                        _ = MessageBox.Show(dialog, "Parsed sector data was null.", "Edit Sector Data (Raw)", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dialog.DialogResult = DialogResult.None;
                        return;
                    }

                    parsedSector = parsed;
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(dialog, $"Unable to parse raw sector data:\n{ex.Message}", "Edit Sector Data (Raw)", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dialog.DialogResult = DialogResult.None;
                }
            };

            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;
            dialog.Controls.Add(textBox);
            dialog.Controls.Add(okButton);
            dialog.Controls.Add(cancelButton);

            if (dialog.ShowDialog(Instance) != DialogResult.OK || parsedSector == null)
                return false;

            updatedSector = parsedSector;
            return true;
        }

        private static void ApplyRawSectorData(Sector target, Sector source)
        {
            target.Id = source.Id;
            target.Name = source.Name;
            target.Description = source.Description;
            target.BaseGameMapping = source.BaseGameMapping;
            target.DisableFactionLogic = source.DisableFactionLogic;
            target.Owner = source.Owner;
            target.Sunlight = source.Sunlight;
            target.Economy = source.Economy;
            target.Security = source.Security;
            target.DiameterRadius = source.DiameterRadius;
            target.AllowRandomAnomalies = source.AllowRandomAnomalies;
            target.Tags = source.Tags;
            target.Zones = source.Zones ?? [];
            target.Regions = source.Regions ?? [];
            target.ResourceAreas = source.ResourceAreas ?? [];
            target.Placement = source.Placement;
            target.CustomOffset = source.CustomOffset;
        }
        #endregion

        #region Configuration
        public void Reset(bool fromImport, bool resetGalaxyType = true, bool resetStatics = true)
        {
            // Reset
            if (!fromImport)
            {
                if (resetGalaxyType)
                {
                    Forms.GalaxySettingsForm.GalaxyName = "xu_ep2_universe";
                    Forms.GalaxySettingsForm.IsCustomGalaxy = false;
                }

                if (resetStatics)
                {
                    RegionDefinitionForm.RegionDefinitions.Clear();
                    Forms.FactoriesForm.AllFactories.Clear();
                    Forms.JobsForm.AllJobs.Clear();
                    Forms.FactionsForm.AllCustomFactions.Clear();
                    FactionRelationsForm.Reset();
                }
                Forms.JobsForm.AllBaskets.Clear();
            }

            // Re-initialize all clusters properly
            _ = InitAllVanillaClusters();

            // Set the default value to be custom
            UpdateClusterOptions();
        }
        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will completely reset all the unsaved changes, this cannot be undone. Are you sure?", "Are you sure?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            Reset(false);
            _currentConfiguration = ExportJsonConfig();
        }

        private void BtnExportConfig_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            saveFileDialog.Title = "Save configuration export file";
            saveFileDialog.DefaultExt = "json";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    _currentConfiguration = ExportJsonConfig();
                    File.WriteAllText(filePath, _currentConfiguration);
                    _ = MessageBox.Show($"Configuration exported succesfully.", "Success");
                }
                catch (Exception)
                {
#if DEBUG
                    throw;
#else
                    _ = MessageBox.Show("Invalid JSON content in file, please try another file.",
                        "Invalid JSON Content", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                }
            }
        }

        private string ExportJsonConfig()
        {
            List<Cluster> allModifiedClusters = AllClusters.Values
                                    .Where(a => !a.IsBaseGame)
                                    .ToList();

            ClusterCollection nonModifiedBaseGameData = InitAllVanillaClusters(false);
            HashSet<string> gateConnections = nonModifiedBaseGameData
                .Clusters
                .SelectMany(a => a.Sectors)
                .SelectMany(a => a.Zones)
                .SelectMany(a => a.Gates)
                .Select(a => a.ConnectionName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Also add clusters that are basegame but have new connections compared to vanilla
            Cluster[] baseGameClusters = AllClusters.Values
                .Where(a => a.IsBaseGame)
                .Select(a => (Cluster)a.Clone())
                .ToArray();
            foreach (Cluster cluster in baseGameClusters)
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    // If sector doesn't exist in vanilla, we need to export it
                    if (!sector.IsBaseGame)
                    {
                        allModifiedClusters.Add(cluster);
                        break;
                    }

                    // Don't export base-game regions
                    sector.Regions.RemoveAll(a => a.IsBaseGame);
                    if (sector.Regions.Count != 0)
                    {
                        // Remove base-game clusters from export
                        allModifiedClusters.Add(cluster);
                        break;
                    }

                    bool breakout = false;
                    foreach (Zone zone in sector.Zones)
                    {
                        if (zone.Stations.Count > 0)
                        {
                            allModifiedClusters.Add(cluster);
                            breakout = true;
                            break;
                        }

                        // Don't export base-game gates
                        zone.Gates.RemoveAll(a => a.IsBaseGame);
                        foreach (Gate gate in zone.Gates)
                        {
                            // Check if gate exists in vanilla
                            if (!gate.IsBaseGame)
                            {
                                allModifiedClusters.Add(cluster);
                                breakout = true;
                                break;
                            }
                        }

                        if (breakout)
                        {
                            break;
                        }
                    }

                    if (breakout)
                    {
                        break;
                    }
                }
            }

            // Support also vanilla changes
            VanillaChanges vanillaChanges = CollectVanillaChanges(nonModifiedBaseGameData);

            string jsonContent = ConfigSerializer.Serialize(allModifiedClusters, vanillaChanges);
            return jsonContent;
        }

        private void BtnImportConfig_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            openFileDialog.Title = "Select configuration export file";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                // Import new configuration
                string jsonContent = File.ReadAllText(filePath);
                (List<Cluster> clusters, VanillaChanges vanillaChanges) configuration = ConfigSerializer.Deserialize(jsonContent);
                if (configuration.clusters != null)
                {
                    ApplyImportedConfiguration(configuration.clusters, configuration.vanillaChanges, "Configuration imported succesfully.");
                }
            }
        }

        private void BtnImportMod_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select an X4 extension folder to import";

            var defaultExtensionsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam/steamapps/common/X4 Foundations/extensions");
            if (Directory.Exists(defaultExtensionsPath))
            {
                folderBrowserDialog.SelectedPath = defaultExtensionsPath;
            }

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            _ = TryImportModFromPath(folderBrowserDialog.SelectedPath, showSuccessMessage: true);
        }

        private void BtnImportModMerge_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select a folder containing one or more X4 extension folders to merge-import";

            string defaultExtensionsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam/steamapps/common/X4 Foundations/extensions");
            if (Directory.Exists(defaultExtensionsPath))
            {
                folderBrowserDialog.SelectedPath = defaultExtensionsPath;
            }

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            _ = TryImportModFromPath(folderBrowserDialog.SelectedPath, showSuccessMessage: true);
        }

        private bool TryImportModFromPath(string modPath, bool showSuccessMessage)
        {
            try
            {
                WriteScannedFilesLogIfRequested(modPath);
                ClusterCollection vanillaClusters = InitAllVanillaClusters(false);
                ModImportResult importedMod = ImportModWithOptionalBase(modPath, vanillaClusters);
                ReversePathFixService.ReversePathFixSummary reversePathFixSummary = null;
                if (_startupOptions.FixReversePathsOnImport)
                {
                    reversePathFixSummary = ReversePathFixService.Apply(importedMod.Clusters);
                }

                WriteImportedSnapshotsIfRequested(modPath, importedMod, reversePathFixSummary);

                ApplyImportedConfiguration(
                    importedMod.Clusters,
                    null,
                    reversePathFixSummary == null
                        ? $"Imported mod \"{importedMod.ModName}\" succesfully."
                        : $"Imported mod \"{importedMod.ModName}\" succesfully and fixed {reversePathFixSummary.PathsNormalized} normalized gate path(s) and {reversePathFixSummary.ReverseGatesCreated} missing reverse gate(s).",
                    showSuccessMessage,
                    importedMod.Warnings);
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#else
                ShowCopyableException(ex, "Unable to import mod");
                return false;
#endif
            }
        }

        private bool TryImportAndFixSectorIslandsFromPath(string modPath, bool showSuccessMessage)
        {
            try
            {
                LogFileHelper.AppendToSessionLog("Import/Fix islands", $"Begin import/fix for path: {modPath}");
                WriteScannedFilesLogIfRequested(modPath);
                ClusterCollection vanillaClusters = InitAllVanillaClusters(false);
                ModImportResult importedMod = ImportModWithOptionalBase(modPath, vanillaClusters);
                LogFileHelper.AppendToSessionLog("Import/Fix islands", $"Import complete for mod: {importedMod.ModName}. Clusters: {importedMod.Clusters?.Count ?? 0}. Warnings: {importedMod.Warnings?.Count ?? 0}.");

                LogFileHelper.AppendToSessionLog("Import/Fix islands", "Running SectorIslandFixService.Apply.");
                var fixSummary = SectorIslandFixService.Apply(importedMod.Clusters);
                int resolvedIslandsCount = SectorIslandReportBuilder.GetResolvedIslandsCount(fixSummary.IslandsDetected, fixSummary.RemainingIslands);
                LogFileHelper.AppendToSessionLog("Import/Fix islands", $"SectorIslandFixService complete. Detected: {fixSummary.IslandsDetected}. Fixed: {resolvedIslandsCount}. Remaining: {fixSummary.RemainingIslands.Count}. Connections created: {fixSummary.IslandsFixed}.");
                ReversePathFixService.ReversePathFixSummary reversePathFixSummary = null;
                if (_startupOptions.FixReversePathsOnImport)
                {
                    LogFileHelper.AppendToSessionLog("Import/Fix islands", "Running ReversePathFixService.Apply.");
                    reversePathFixSummary = ReversePathFixService.Apply(importedMod.Clusters);
                    LogFileHelper.AppendToSessionLog("Import/Fix islands", $"ReversePathFixService complete. Paths normalized: {reversePathFixSummary.PathsNormalized}. Reverse gates created: {reversePathFixSummary.ReverseGatesCreated}.");
                }

                WriteImportedSnapshotsIfRequested(modPath, importedMod, reversePathFixSummary);

                List<string> islandWarnings = fixSummary.RemainingIslands
                    .Select(a => $"Sector island auto-fix could not resolve sector '{a.SectorName}' in cluster '{a.ClusterName}'.")
                    .ToList();

                string islandLogPath = null;
                if (fixSummary.RemainingIslands.Count > 0)
                {
                    string islandReport = SectorIslandReportBuilder.BuildUnresolvedIslandReport(
                        importedMod.ModName,
                        fixSummary.IslandsDetected,
                        resolvedIslandsCount,
                        fixSummary.RemainingIslands);
                    islandLogPath = TryWriteImportErrorLog("unresolved-sector-islands", islandReport);
                    if (!string.IsNullOrWhiteSpace(islandLogPath))
                    {
                        LogFileHelper.AppendToSessionLog("Import/Fix islands", $"Wrote unresolved island report: {islandLogPath}");
                        islandWarnings.Insert(0, $"Unresolved sector islands were written to: {islandLogPath}");
                    }
                    else
                    {
                        LogFileHelper.AppendToSessionLog("Import/Fix islands", "Unresolved island report generation succeeded but no log path was returned.");
                    }
                }

                List<string> warnings = importedMod.Warnings == null
                    ? []
                    : [.. importedMod.Warnings];
                warnings.InsertRange(0, islandWarnings);

                LogFileHelper.AppendToSessionLog("Import/Fix islands", $"Applying imported configuration with {warnings.Count} warning(s).");

                ApplyImportedConfiguration(
                    importedMod.Clusters,
                    null,
                    reversePathFixSummary == null
                        ? $"Imported mod \"{importedMod.ModName}\" and fixed {resolvedIslandsCount} of {fixSummary.IslandsDetected} isolated custom sector(s). {fixSummary.RemainingIslands.Count} unresolved sector island(s) remain."
                        : $"Imported mod \"{importedMod.ModName}\", fixed {resolvedIslandsCount} of {fixSummary.IslandsDetected} isolated custom sector(s), {fixSummary.RemainingIslands.Count} unresolved sector island(s) remain, and fixed {reversePathFixSummary.PathsNormalized} normalized gate path(s) and {reversePathFixSummary.ReverseGatesCreated} missing reverse gate(s).",
                    showSuccessMessage,
                    warnings);

                LogFileHelper.AppendToSessionLog("Import/Fix islands", "Import/fix flow completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                LogFileHelper.AppendToSessionLog("Import/Fix islands failed", ex.ToString());
#if DEBUG
                throw;
#else
                ShowCopyableException(ex, "Unable to import and fix sector islands");
                return false;
#endif
            }
        }

        private void WriteImportedSnapshotsIfRequested(
            string modPath,
            ModImportResult importedMod,
            ReversePathFixService.ReversePathFixSummary reversePathFixSummary)
        {
            WriteImportedJsonIfRequested(modPath, importedMod, reversePathFixSummary);
            WriteImportedXmlIfRequested(modPath, importedMod, reversePathFixSummary);
        }

        private void WriteImportedJsonIfRequested(
            string modPath,
            ModImportResult importedMod,
            ReversePathFixService.ReversePathFixSummary reversePathFixSummary)
        {
            if (string.IsNullOrWhiteSpace(_startupOptions.ExportImportJsonPath))
                return;

            string exportPath = Path.GetFullPath(_startupOptions.ExportImportJsonPath);
            string exportDirectory = Path.GetDirectoryName(exportPath);
            if (!string.IsNullOrWhiteSpace(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }

            var payload = new
            {
                ImportedAtUtc = DateTime.UtcNow,
                SourcePath = modPath,
                importedMod.ModName,
                importedMod.Warnings,
                ReversePathFix = reversePathFixSummary,
                importedMod.Clusters,
            };

            string json = JsonSerializer.Serialize(payload, ConfigSerializer.JsonSerializerOptions);
            File.WriteAllText(exportPath, json);
            LogFileHelper.AppendToSessionLog("Import JSON export", $"Wrote imported model snapshot to: {exportPath}");
        }

        private void WriteImportedXmlIfRequested(
            string modPath,
            ModImportResult importedMod,
            ReversePathFixService.ReversePathFixSummary reversePathFixSummary)
        {
            if (string.IsNullOrWhiteSpace(_startupOptions.ExportImportXmlPath))
                return;

            string exportPath = Path.GetFullPath(_startupOptions.ExportImportXmlPath);
            string exportDirectory = Path.GetDirectoryName(exportPath);
            if (!string.IsNullOrWhiteSpace(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }

            XDocument document = BuildImportedXmlDocument(modPath, importedMod, reversePathFixSummary);
            document.Save(exportPath);
            LogFileHelper.AppendToSessionLog("Import XML export", $"Wrote imported model snapshot to: {exportPath}");
        }

        private static XDocument BuildImportedXmlDocument(
            string modPath,
            ModImportResult importedMod,
            ReversePathFixService.ReversePathFixSummary reversePathFixSummary)
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("import-snapshot",
                    new XAttribute("mod-name", importedMod.ModName ?? string.Empty),
                    new XAttribute("source-path", modPath ?? string.Empty),
                    new XAttribute("imported-at-utc", DateTime.UtcNow.ToString("O")),
                    reversePathFixSummary == null
                        ? null
                        : new XElement("reverse-path-fix",
                            new XAttribute("paths-normalized", reversePathFixSummary.PathsNormalized),
                            new XAttribute("reverse-gates-created", reversePathFixSummary.ReverseGatesCreated)),
                    new XElement("warnings",
                        (importedMod.Warnings ?? []).Select(warning =>
                            new XElement("warning", warning ?? string.Empty))),
                    new XElement("clusters",
                        (importedMod.Clusters ?? []).Select(BuildImportedClusterElement))));
        }

        private static XElement BuildImportedClusterElement(Cluster cluster)
        {
            return new XElement("cluster",
                new XAttribute("id", cluster.Id),
                new XAttribute("name", cluster.Name ?? string.Empty),
                new XAttribute("base-game-mapping", cluster.BaseGameMapping ?? string.Empty),
                new XAttribute("imported-macro-name", cluster.ImportedMacroName ?? string.Empty),
                new XAttribute("imported-connection-name", cluster.ImportedConnectionName ?? string.Empty),
                new XAttribute("background-visual-mapping", cluster.BackgroundVisualMapping ?? string.Empty),
                new XAttribute("soundtrack", cluster.Soundtrack ?? string.Empty),
                new XAttribute("dlc", cluster.Dlc ?? string.Empty),
                new XAttribute("position-x", cluster.Position.X),
                new XAttribute("position-y", cluster.Position.Y),
                new XAttribute("custom-sector-positioning", cluster.CustomSectorPositioning),
                new XElement("description", cluster.Description ?? string.Empty),
                new XElement("sectors", (cluster.Sectors ?? []).Select(BuildImportedSectorElement)));
        }

        private static XElement BuildImportedSectorElement(Sector sector)
        {
            XElement customOffset = sector.CustomOffset.HasValue
                ? new XElement("custom-offset",
                    new XAttribute("x", sector.CustomOffset.Value.X),
                    new XAttribute("y", sector.CustomOffset.Value.Y))
                : null;

            return new XElement("sector",
                new XAttribute("id", sector.Id),
                new XAttribute("name", sector.Name ?? string.Empty),
                new XAttribute("base-game-mapping", sector.BaseGameMapping ?? string.Empty),
                new XAttribute("imported-macro-name", sector.ImportedMacroName ?? string.Empty),
                new XAttribute("owner", sector.Owner ?? string.Empty),
                new XAttribute("disable-faction-logic", sector.DisableFactionLogic),
                new XAttribute("sunlight", sector.Sunlight),
                new XAttribute("economy", sector.Economy),
                new XAttribute("security", sector.Security),
                new XAttribute("allow-random-anomalies", sector.AllowRandomAnomalies),
                new XAttribute("placement", sector.Placement),
                new XElement("description", sector.Description ?? string.Empty),
                customOffset,
                new XElement("zones", (sector.Zones ?? []).Select(BuildImportedZoneElement)));
        }

        private static XElement BuildImportedZoneElement(Zone zone)
        {
            return new XElement("zone",
                new XAttribute("id", zone.Id),
                new XAttribute("name", zone.Name ?? string.Empty),
                new XAttribute("imported-macro-name", zone.ImportedMacroName ?? string.Empty),
                new XAttribute("position-x", zone.Position.X),
                new XAttribute("position-y", zone.Position.Y),
                new XElement("gates", (zone.Gates ?? []).Select(BuildImportedGateElement)));
        }

        private static XElement BuildImportedGateElement(Gate gate)
        {
            return new XElement("gate",
                new XAttribute("id", gate.Id),
                new XAttribute("connection-name", gate.ConnectionName ?? string.Empty),
                new XAttribute("parent-sector-name", gate.ParentSectorName ?? string.Empty),
                new XAttribute("destination-sector-name", gate.DestinationSectorName ?? string.Empty),
                new XAttribute("source", gate.Source ?? string.Empty),
                new XAttribute("destination", gate.Destination ?? string.Empty),
                new XAttribute("source-path", gate.SourcePath ?? string.Empty),
                new XAttribute("destination-path", gate.DestinationPath ?? string.Empty),
                new XAttribute("type", gate.Type),
                new XAttribute("yaw", gate.Yaw),
                new XAttribute("pitch", gate.Pitch),
                new XAttribute("roll", gate.Roll),
                new XAttribute("position-x", gate.Position.X),
                new XAttribute("position-y", gate.Position.Y),
                new XAttribute("is-highway-gate", gate.IsHighwayGate));
        }

        private void ApplyImportedConfiguration(List<Cluster> clusters, VanillaChanges vanillaChanges, string successMessage, bool showSuccessMessage = true, List<string> warnings = null)
        {
            // Reset configuration
            Reset(true);

            if (Forms.GalaxySettingsForm.IsCustomGalaxy)
            {
                ToggleGalaxyMode(null);
            }

            // Apply vanilla changes to AllClusters
            if (vanillaChanges != null)
            {
                SupportVanillaChangesInConfigImport((clusters, vanillaChanges));
            }

            Lazy<Cluster[]> vanillaClustersLazy = new(() => InitAllVanillaClusters(false).Clusters.Where(a => a.IsBaseGame).ToArray());

            foreach (Cluster cluster in clusters)
            {
                Import_Support_NewVersions(cluster, vanillaClustersLazy);
                ReplaceClusterByImport(cluster);

                if (cluster.Name == null)
                {
                    warnings ??= [];
                    warnings.Add($"Imported cluster name was null for macro '{cluster.ImportedMacroName ?? "<unknown>"}'. Display fallback {ModImportService.MissingTranslationDisplayName} will be used until a real name is assigned.");
                }

                foreach (Sector sector in cluster.Sectors)
                {
                    if (sector.Name == null)
                    {
                        warnings ??= [];
                        warnings.Add($"Imported sector name was null for sector macro '{sector.ImportedMacroName ?? "<unknown>"}' in cluster '{cluster.ImportedMacroName ?? cluster.Name ?? "<unknown>"}'. Display fallback {ModImportService.MissingTranslationDisplayName} will be used until a real name is assigned.");
                    }
                }

                if (!cluster.IsBaseGame)
                {
                    _ = ClustersListBox.Items.Add(cluster);
                }
            }

            _clusterDlcLookup = null;
            ClustersListBox.SelectedItem = clusters.FirstOrDefault(a => !a.IsBaseGame);
            _currentConfiguration = ExportJsonConfig();
            if (showSuccessMessage)
                _ = MessageBox.Show(successMessage, "Success");

            if (warnings != null && warnings.Count > 0)
            {
                string fullWarningMessage = "Some import issues were detected:\n\n- " + string.Join("\n- ", warnings);
                string warningMessage = "Some import issues were detected:\n\n- " + string.Join("\n- ", warnings.Take(20));
                if (warnings.Count > 20)
                {
                    warningMessage += $"\n\n...and {warnings.Count - 20} more.";
                }

                string warningLogPath = TryWriteImportErrorLog("import-warnings", fullWarningMessage);
                if (!string.IsNullOrWhiteSpace(warningLogPath))
                {
                    warningMessage += $"\n\nA warning log was written to:\n{warningLogPath}";
                    fullWarningMessage += $"\n\nWarning log: {warningLogPath}";
                }

                ShowCopyableMessage(warningMessage, fullWarningMessage, "Import warnings", MessageBoxIcon.Warning);
            }
        }

        private static void ShowCopyableMessage(string displayMessage, string fullMessage, string caption, MessageBoxIcon icon)
        {
            bool copied = TryCopyMessageToClipboard(fullMessage);
            string message = copied
                ? displayMessage + Environment.NewLine + Environment.NewLine + "The full message was copied to your clipboard."
                : displayMessage;

            _ = MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
        }

        private static void ShowCopyableException(Exception ex, string caption)
        {
            string targetSite = ex.TargetSite == null
                ? "<unknown>"
                : $"{ex.TargetSite.DeclaringType?.FullName}.{ex.TargetSite.Name}";

            string displayMessage =
                $"{ex.GetType().Name}: {ex.Message}{Environment.NewLine}{Environment.NewLine}" +
                $"Target: {targetSite}";

            string fullMessage =
                $"{ex.GetType().FullName}: {ex.Message}{Environment.NewLine}{Environment.NewLine}" +
                $"Target: {targetSite}{Environment.NewLine}{Environment.NewLine}" +
                ex;

            string logPath = TryWriteImportErrorLog(caption, fullMessage);
            if (!string.IsNullOrWhiteSpace(logPath))
            {
                displayMessage += Environment.NewLine + Environment.NewLine + $"A log file was written to:{Environment.NewLine}{logPath}";
                fullMessage += Environment.NewLine + Environment.NewLine + $"Log file: {logPath}";
            }

            ShowCopyableMessage(displayMessage, fullMessage, caption, MessageBoxIcon.Error);
        }

        private static string TryWriteImportErrorLog(string caption, string fullMessage)
        {
            if (string.IsNullOrWhiteSpace(fullMessage))
                return null;

            try
            {
                return LogFileHelper.TryWriteDiagnosticLog(caption, caption, fullMessage);
            }
            catch
            {
                return null;
            }
        }

        private static bool TryCopyMessageToClipboard(string message)
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

        private void ProcessStartupOptions()
        {
            LogFileHelper.AppendToSessionLog("Startup options", $"OpenGalaxyView={_startupOptions.OpenGalaxyView}, ImportModPath={_startupOptions.ImportModPath ?? "<none>"}, ImportModMergePath={_startupOptions.ImportModMergePath ?? "<none>"}, FixSectorIslandsPath={_startupOptions.FixSectorIslandsPath ?? "<none>"}, FixReversePaths={_startupOptions.FixReversePathsOnImport}, ExitAfterImport={_startupOptions.ExitAfterImport}.");
            bool shouldOpenGalaxyView = _startupOptions.OpenGalaxyView;
            bool importedFromStartup = false;

            if (!string.IsNullOrWhiteSpace(_startupOptions.FixSectorIslandsPath))
            {
                LogFileHelper.AppendToSessionLog("Startup options", "Dispatching to TryImportAndFixSectorIslandsFromPath.");
                _ = TryImportAndFixSectorIslandsFromPath(_startupOptions.FixSectorIslandsPath, showSuccessMessage: true);
                shouldOpenGalaxyView = shouldOpenGalaxyView || !_startupOptions.ExitAfterImport;
                importedFromStartup = true;
            }
            else if (!string.IsNullOrWhiteSpace(_startupOptions.ImportModMergePath))
            {
                LogFileHelper.AppendToSessionLog("Startup options", "Dispatching to TryImportModFromPath with merged import path.");
                _ = TryImportModFromPath(_startupOptions.ImportModMergePath, showSuccessMessage: false);
                importedFromStartup = true;
            }
            else if (!string.IsNullOrWhiteSpace(_startupOptions.ImportModPath))
            {
                LogFileHelper.AppendToSessionLog("Startup options", "Dispatching to TryImportModFromPath with import path.");
                _ = TryImportModFromPath(_startupOptions.ImportModPath, showSuccessMessage: false);
                importedFromStartup = true;
            }

            if (shouldOpenGalaxyView)
            {
                LogFileHelper.AppendToSessionLog("Startup options", "Opening galaxy view.");
                OpenSectorMap();
            }

            if (importedFromStartup && _startupOptions.ExitAfterImport && !shouldOpenGalaxyView)
            {
                LogFileHelper.AppendToSessionLog("Startup options", "Scheduling close after import.");
                BeginInvoke(new Action(Close));
            }

            LogFileHelper.AppendToSessionLog("Startup options", "ProcessStartupOptions complete.");
        }

        private ModImportResult ImportModWithOptionalBase(string modPath, ClusterCollection vanillaClusters)
        {
            if (!string.IsNullOrWhiteSpace(_startupOptions.ImportModPath) &&
                !string.Equals(Path.GetFullPath(_startupOptions.ImportModPath), Path.GetFullPath(modPath), StringComparison.OrdinalIgnoreCase))
            {
                return ModImportService.ImportWithMerge(_startupOptions.ImportModPath, modPath, vanillaClusters, _startupOptions.ClusterHexGap);
            }

            return ModImportService.IsImportableModDirectory(modPath)
                ? ModImportService.Import(modPath, vanillaClusters, _startupOptions.ClusterHexGap)
                : ModImportService.ImportMerged(modPath, vanillaClusters, _startupOptions.ClusterHexGap);
        }

        private void WriteScannedFilesLogIfRequested(string modPath)
        {
            if (!_startupOptions.LogScannedFilesOnImport)
                return;

            ImportFileScanReport report = ImportAuditService.BuildInclusionReport(modPath, _startupOptions.ImportModPath);
            string reportText = ImportFileScanReportBuilder.Build(modPath, report);
            string reportPath = TryWriteImportErrorLog("scanned-and-skipped-files", reportText);
            if (!string.IsNullOrWhiteSpace(reportPath))
            {
                LogFileHelper.AppendToSessionLog("Import scan report", $"Wrote scanned/skipped file report: {reportPath}");
            }
        }

        private void SupportVanillaChangesInConfigImport((List<Cluster> clusters, VanillaChanges vanillaChanges) configuration)
        {
            // Cluster removal
            foreach (Cluster cluster in configuration.vanillaChanges.RemovedClusters)
            {
                _ = AllClusters.Remove((cluster.Position.X, cluster.Position.Y));
            }

            // Sector removal
            foreach (RemovedSector pair in configuration.vanillaChanges.RemovedSectors)
            {
                // If cluster doesn't exist its already removed, skip
                if (!AllClusters.TryGetValue((pair.VanillaCluster.Position.X, pair.VanillaCluster.Position.Y), out Cluster cluster))
                {
                    continue;
                }

                Sector sector = cluster.Sectors.FirstOrDefault(a => string.Equals(a.Name, pair.Sector.Name, StringComparison.OrdinalIgnoreCase));
                if (sector != null)
                {
                    _ = cluster.Sectors.Remove(sector);
                }
            }

            foreach (RemovedConnection pair in configuration.vanillaChanges.RemovedConnections)
            {
                // If cluster doesn't exist its already removed, skip
                if (!AllClusters.TryGetValue((pair.VanillaCluster.Position.X, pair.VanillaCluster.Position.Y), out Cluster cluster))
                {
                    continue;
                }

                // If sector doesn't exist its already removed, skip
                Sector sector = cluster.Sectors.FirstOrDefault(a => string.Equals(a.Name, pair.Sector.Name, StringComparison.OrdinalIgnoreCase));
                if (sector == null)
                {
                    continue;
                }

                Zone zone = sector.Zones.FirstOrDefault(a =>
                {
                    return (!string.IsNullOrWhiteSpace(a.Name) && !string.IsNullOrWhiteSpace(pair.Zone.Name) &&
                        string.Equals(a.Name, pair.Zone.Name, StringComparison.OrdinalIgnoreCase)) ||
                        ((a.Id != 0 || pair.Zone.Id != 0) && a.Id == pair.Zone.Id);
                });
                if (zone == null)
                {
                    continue;
                }

                Gate gate = zone.Gates.FirstOrDefault(a => a.SourcePath == pair.Gate.SourcePath && a.DestinationPath == pair.Gate.DestinationPath);
                if (gate != null)
                {
                    _ = zone.Gates.Remove(gate);
                }
            }

            // Cluster modification
            Dictionary<(int, int), Cluster> moveMap = new(); // Stores where each cluster should move
            HashSet<(int, int)> toRemove = new(); // Stores old positions to remove
            foreach (ModifiedCluster modification in configuration.vanillaChanges.ModifiedClusters)
            {
                Cluster Old = modification.Old;
                Cluster New = modification.New;
                // If cluster doesn't exist its already removed, skip
                if (!AllClusters.TryGetValue((Old.Position.X, Old.Position.Y), out Cluster cluster))
                {
                    continue;
                }

                // Update cluster properties
                cluster.Name = New.Name;
                cluster.Description = New.Description;
                cluster.BackgroundVisualMapping = New.BackgroundVisualMapping;
                cluster.Position = New.Position;
                cluster.CustomSectorPositioning = New.CustomSectorPositioning;
                cluster.CustomClusterXml = New.CustomClusterXml;
                cluster.Soundtrack = New.Soundtrack;

                // Re-adjust position in all clusters
                if (Old.Position != New.Position)
                {
                    moveMap[(New.Position.X, New.Position.Y)] = cluster;
                    _ = toRemove.Add((Old.Position.X, Old.Position.Y));
                }
            }

            // Remove old positions
            foreach ((int, int) oldPos in toRemove)
            {
                _ = AllClusters.Remove(oldPos);
            }

            // Insert clusters into new positions safely
            foreach (((int, int) newPos, Cluster cluster) in moveMap)
            {
                if (AllClusters.ContainsKey(newPos))
                {
                    throw new Exception("Something went wrong, cluster already exists on moved position: " + newPos);
                }

                AllClusters[newPos] = cluster;
            }

            // Sector modification
            foreach (ModifiedSector modification in configuration.vanillaChanges.ModifiedSectors)
            {
                Cluster VanillaCluster = modification.VanillaCluster;
                Sector Old = modification.Old;
                Sector New = modification.New;
                // If cluster doesn't exist its already removed, skip
                if (!AllClusters.TryGetValue((VanillaCluster.Position.X, VanillaCluster.Position.Y), out Cluster cluster))
                {
                    continue;
                }

                // Find matching sector
                Sector sector = cluster.Sectors.FirstOrDefault(a => string.Equals(a.Name, Old.Name, StringComparison.OrdinalIgnoreCase));
                if (sector == null)
                {
                    continue;
                }

                // Update sector properties
                sector.Name = New.Name;
                sector.Description = New.Description;
                sector.DisableFactionLogic = New.DisableFactionLogic;
                sector.Sunlight = New.Sunlight;
                sector.Economy = New.Economy;
                sector.Security = New.Security;
                sector.Tags = New.Tags;
                sector.AllowRandomAnomalies = New.AllowRandomAnomalies;
                sector.Placement = New.Placement;
                sector.ResourceAreas = New.ResourceAreas.ToList();
            }
        }

        private void ReplaceClusterByImport(Cluster cluster)
        {
            Cluster currentCluster = null;
            if (cluster.IsBaseGame)
            {
                currentCluster = AllClusters.Values.FirstOrDefault(a => a.BaseGameMapping.Equals(cluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
                if (currentCluster != null && currentCluster.Position != cluster.Position)
                {
                    _ = AllClusters.Remove((currentCluster.Position.X, currentCluster.Position.Y));
                    AllClusters[(cluster.Position.X, cluster.Position.Y)] = currentCluster;
                }
            }

            if (currentCluster == null && !AllClusters.TryGetValue((cluster.Position.X, cluster.Position.Y), out currentCluster))
            {
                // Custom cluster
                AllClusters[(cluster.Position.X, cluster.Position.Y)] = cluster;
                return;
            }

            // Replace each part individually as to not override the basegame data
            currentCluster.Position = cluster.Position;
            currentCluster.Description = cluster.Description;
            currentCluster.Name = cluster.Name;
            currentCluster.BackgroundVisualMapping = cluster.BackgroundVisualMapping;
            currentCluster.CustomSectorPositioning = cluster.CustomSectorPositioning;
            currentCluster.Soundtrack = cluster.Soundtrack;
            currentCluster.CustomClusterXml = cluster.CustomClusterXml;

            foreach (Sector newSector in cluster.Sectors)
            {
                // Check if it exist then adjust it else add it
                Sector currentSector = currentCluster.Sectors.FirstOrDefault(a =>
                    !string.IsNullOrWhiteSpace(a.Name) &&
                    !string.IsNullOrWhiteSpace(newSector.Name) &&
                    string.Equals(a.Name, newSector.Name, StringComparison.OrdinalIgnoreCase));
                if (currentSector == null)
                {
                    currentCluster.Sectors.Add(newSector);
                    continue;
                }

                // Replace each part individually as to not override the basegame data
                currentSector.Name = newSector.Name;
                currentSector.Economy = newSector.Economy;
                currentSector.Sunlight = newSector.Sunlight;
                currentSector.Security = newSector.Security;
                currentSector.Tags = newSector.Tags;
                currentSector.DiameterRadius = newSector.DiameterRadius;
                currentSector.AllowRandomAnomalies = newSector.AllowRandomAnomalies;
                currentSector.DisableFactionLogic = newSector.DisableFactionLogic;
                currentSector.Placement = newSector.Placement;
                currentSector.CustomOffset = newSector.CustomOffset;

                foreach (Zone newZone in newSector.Zones)
                {
                    // Check if it exist then adjust it else add it
                    Zone currentZone = currentSector.Zones.FirstOrDefault(a =>
                    {
                        return (!string.IsNullOrWhiteSpace(a.Name) && !string.IsNullOrWhiteSpace(newZone.Name) && a.Name.Equals(newZone.Name, StringComparison.OrdinalIgnoreCase))
|| ((a.Id != 0 || newZone.Id != 0) && a.Id == newZone.Id);
                    });

                    if (currentZone == null)
                    {
                        currentSector.Zones.Add(newZone);
                        continue;
                    }

                    currentZone.Name = newZone.Name;
                    currentZone.Position = newZone.Position;

                    foreach (Gate newGate in newZone.Gates)
                    {
                        // Check if it exist then adjust it else add it
                        Gate currentGate = currentZone.Gates.FirstOrDefault(a => a.SourcePath == newGate.SourcePath && a.DestinationPath == newGate.DestinationPath);
                        if (currentGate == null)
                        {
                            currentZone.Gates.Add(newGate);
                            continue;
                        }
                    }

                    foreach (Station newStation in newZone.Stations)
                    {
                        Station currentStation = currentZone.Stations.FirstOrDefault(a => a.Id == newStation.Id);
                        if (currentStation == null)
                        {
                            currentZone.Stations.Add(newStation);
                            continue;
                        }

                        currentStation.Name = newStation.Name;
                        currentStation.Position = newStation.Position;
                        currentStation.Faction = newStation.Faction;
                        currentStation.Owner = newStation.Owner;
                        currentStation.Race = newStation.Race;
                        currentStation.Id = newStation.Id;
                        currentStation.Type = newStation.Type;
                    }
                }

                foreach (var newRegion in newSector.Regions)
                {
                    var currentRegion = currentSector.Regions.FirstOrDefault(a => a.Id == newRegion.Id);
                    if (currentRegion == null)
                    {
                        currentSector.Regions.Add(newRegion);
                        continue;
                    }

                    currentRegion.Name = newRegion.Name;
                    currentRegion.Position = newRegion.Position;
                    currentRegion.BoundaryRadius = newRegion.BoundaryRadius;
                    currentRegion.Definition = newRegion.Definition;
                    currentRegion.BoundaryLinear = newRegion.BoundaryLinear;
                }
            }
        }

        private Dictionary<(int, int), Cluster> _clusterDlcLookup;
        private void Import_Support_NewVersions(Cluster cluster, Lazy<Cluster[]> vanillaClustersLazy)
        {
            // Fix background visual mapping
            if (string.IsNullOrWhiteSpace(cluster.BackgroundVisualMapping) ||
                !BackgroundVisualMapping.Values.Any(a => a.Equals(cluster.BackgroundVisualMapping, StringComparison.OrdinalIgnoreCase)))
            {
                if (!cluster.IsBaseGame)
                {
                    cluster.BackgroundVisualMapping = BackgroundVisualMapping.Values.First();
                }
                else
                {
                    Cluster matchingCluster = vanillaClustersLazy.Value.FirstOrDefault(a => a.BaseGameMapping.Equals(cluster.BaseGameMapping));
                    cluster.BackgroundVisualMapping = matchingCluster != null ? matchingCluster.BackgroundVisualMapping : BackgroundVisualMapping.Values.First();
                }
            }

            // Re-check DLCs
            if (cluster.Dlc == null)
            {
                if (_clusterDlcLookup == null)
                {
                    // Create new lookup table
                    string json = File.ReadAllText(Constants.DataPaths.SectorMappingFilePath);
                    ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(json, ConfigSerializer.JsonSerializerOptions);
                    _clusterDlcLookup = clusterCollection.Clusters.ToDictionary(a => (a.Position.X, a.Position.Y));
                }

                if (_clusterDlcLookup.TryGetValue((cluster.Position.X, cluster.Position.Y), out Cluster lookupCluster))
                {
                    cluster.Dlc = lookupCluster.Dlc;
                }
            }

            // Support for dynamic placement, if all are the same we need to init some changes dynamically
            if (cluster.Sectors.Count > 1 && cluster.Sectors.All(a => a.Placement == default))
            {
                List<SectorPlacement> placements = Enum.GetValues<SectorPlacement>().OrderBy(a => a).ToList();
                foreach (Sector sector in cluster.Sectors)
                {
                    bool placementSet = false;
                    if (sector.IsBaseGame)
                    {
                        // Determine if sector is vanilla, then copy over the original values
                        Cluster[] vanillaClusters = vanillaClustersLazy.Value;
                        Cluster matchingCluster = vanillaClusters.FirstOrDefault(a => a.BaseGameMapping.Equals(cluster.BaseGameMapping));
                        if (matchingCluster != null)
                        {
                            Sector matchingSector = matchingCluster.Sectors.FirstOrDefault(a => a.BaseGameMapping.Equals(sector.BaseGameMapping));
                            if (matchingSector != null)
                            {
                                sector.Placement = matchingSector.Placement;
                                _ = placements.Remove(sector.Placement);
                                placementSet = true;
                            }
                        }
                    }

                    if (!placementSet)
                    {
                        sector.Placement = placements[^1];
                        _ = placements.Remove(sector.Placement);
                    }

                    Forms.SectorForm.DetermineSectorOffset(cluster, sector);
                }
            }
            else if (cluster.Sectors.Count > 1)
            {
                // Determine offset dynamically based on placements
                foreach (Sector sector in cluster.Sectors)
                {
                    Forms.SectorForm.DetermineSectorOffset(cluster, sector);
                }
            }

            // Support new generated zones in new sectors that don't have them yet
            foreach (var sector in cluster.Sectors)
            {
                if (sector.IsBaseGame || sector.Zones.Any(a => a.IsGeneratedZone)) continue;
                sector.InitializeOrUpdateZones();
            }
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            string directoryPath = Constants.DataPaths.ModDirectoryPath;
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            _ = Process.Start("explorer.exe", directoryPath);
        }

        private VanillaChanges CollectVanillaChanges(ClusterCollection nonModifiedBaseGameData)
        {
            if (Forms.GalaxySettingsForm.IsCustomGalaxy)
            {
                return new VanillaChanges();
            }

            Dictionary<string, Cluster> vanillaClusters = AllClusters.Values
                .Where(a => a.IsBaseGame)
                .Select(a => (Cluster)a.Clone())
                .ToDictionary(a => a.BaseGameMapping);

            Dictionary<string, Cluster> nonModifiedVanillaClusters = nonModifiedBaseGameData
                .Clusters
                .Select(a => (Cluster)a.Clone())
                .ToDictionary(a => a.BaseGameMapping);

            // Clear up regions because they are not exported or modifyable in anyway
            foreach (var cluster in vanillaClusters.Values)
            {
                foreach (var sector in cluster.Sectors)
                {
                    sector.Regions.RemoveAll(a => a.IsBaseGame);
                }
            }
            foreach (var cluster in nonModifiedVanillaClusters.Values)
            {
                foreach (var sector in cluster.Sectors)
                {
                    sector.Regions.RemoveAll(a => a.IsBaseGame);
                }
            }

            VanillaChanges vanillaChanges = new();

            foreach (KeyValuePair<string, Cluster> nonModifiedKvp in nonModifiedVanillaClusters)
            {
                Cluster nonModifiedCluster = nonModifiedKvp.Value;

                // First check if the cluster still exists
                if (!vanillaClusters.TryGetValue(nonModifiedKvp.Key, out Cluster modifiedCluster))
                {
                    // Add to removed clusters + sectors
                    vanillaChanges.RemovedClusters.Add(nonModifiedCluster);
                    foreach (Sector nonModifiedSector in nonModifiedCluster.Sectors)
                    {
                        vanillaChanges.RemovedSectors.Add(new RemovedSector { VanillaCluster = nonModifiedCluster, Sector = nonModifiedSector });
                    }

                    continue;
                }

                if (nonModifiedCluster.Name != modifiedCluster.Name ||
                    nonModifiedCluster.Description != modifiedCluster.Description ||
                    nonModifiedCluster.BackgroundVisualMapping != modifiedCluster.BackgroundVisualMapping ||
                    nonModifiedCluster.Position != modifiedCluster.Position ||
                    nonModifiedCluster.CustomSectorPositioning != modifiedCluster.CustomSectorPositioning ||
                    nonModifiedCluster.Soundtrack != modifiedCluster.Soundtrack)
                {
                    // Add to modified clusters
                    vanillaChanges.ModifiedClusters.Add(new ModifiedCluster { Old = nonModifiedCluster, New = (Cluster)modifiedCluster.Clone() });
                }

                foreach (Sector nonModifiedSector in nonModifiedCluster.Sectors)
                {
                    Sector modifiedSector = modifiedCluster.Sectors.FirstOrDefault(a => a.BaseGameMapping == nonModifiedSector.BaseGameMapping);
                    if (modifiedSector == null)
                    {
                        // The vanilla sector was removed
                        vanillaChanges.RemovedSectors.Add(new RemovedSector { VanillaCluster = nonModifiedCluster, Sector = nonModifiedSector });
                        foreach (Zone zone in nonModifiedSector.Zones)
                        {
                            foreach (Gate gate in zone.Gates)
                            {
                                vanillaChanges.RemovedConnections.Add(new RemovedConnection
                                {
                                    VanillaCluster = nonModifiedCluster,
                                    Sector = nonModifiedSector,
                                    Zone = zone,
                                    Gate = gate
                                });
                            }
                        }

                        continue;
                    }

                    if (nonModifiedSector.Name != modifiedSector.Name ||
                        nonModifiedSector.Description != modifiedSector.Description ||
                        nonModifiedSector.DisableFactionLogic != modifiedSector.DisableFactionLogic ||
                        nonModifiedSector.Sunlight != modifiedSector.Sunlight ||
                        nonModifiedSector.Economy != modifiedSector.Economy ||
                        nonModifiedSector.Security != modifiedSector.Security ||
                        nonModifiedSector.Tags != modifiedSector.Tags ||
                        nonModifiedSector.AllowRandomAnomalies != modifiedSector.AllowRandomAnomalies ||
                        nonModifiedSector.Placement != modifiedSector.Placement ||
                        nonModifiedSector.CustomOffset != modifiedSector.CustomOffset ||
                        IsResourceAreasModified(nonModifiedSector.ResourceAreas, modifiedSector.ResourceAreas))
                    {
                        // Add to modified clusters
                        vanillaChanges.ModifiedSectors.Add(new ModifiedSector { VanillaCluster = nonModifiedCluster, Old = nonModifiedSector, New = (Sector)modifiedSector.Clone() });
                    }

                    // Connections
                    foreach (Zone nonModifiedZone in nonModifiedSector.Zones)
                    {
                        foreach (Gate nonModifiedGate in nonModifiedZone.Gates)
                        {
                            // Find matching zone & connection
                            Zone matchingZone = modifiedSector.Zones.FirstOrDefault(a => string.Equals(a.Name, nonModifiedZone.Name, StringComparison.OrdinalIgnoreCase));
                            Gate matchingGate = matchingZone?.Gates.FirstOrDefault(a => a.SourcePath == nonModifiedGate.SourcePath && a.DestinationPath == nonModifiedGate.DestinationPath);
                            if (matchingZone == null || matchingGate == null)
                            {
                                vanillaChanges.RemovedConnections.Add(new RemovedConnection
                                {
                                    VanillaCluster = nonModifiedCluster,
                                    Sector = nonModifiedSector,
                                    Zone = nonModifiedZone,
                                    Gate = nonModifiedGate
                                });
                                continue;
                            }
                        }
                    }
                }
            }

            return vanillaChanges;
        }

        private static bool IsResourceAreasModified(List<Resource> old, List<Resource> @new)
        {
            static string Key(Resource r)
                => $"{r.Ware}|{r.Yield}|{r.Size}|{r.Speed}|{r.Amount}";

            var oldGroups = old
                .GroupBy(Key)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var resource in @new)
            {
                var key = Key(resource);

                if (!oldGroups.TryGetValue(key, out var count))
                    return true;

                if (count == 1)
                    oldGroups.Remove(key);
                else
                    oldGroups[key] = count - 1;
            }

            return oldGroups.Count > 0;
        }
        #endregion

        #region Clusters
        private void BtnNewCluster_Click(object sender, EventArgs e)
        {
            ClusterForm.Value.Cluster = null;
            ClusterForm.Value.BtnCreate.Text = "Create";
            ClusterForm.Value.TxtName.Text = string.Empty;
            ClusterForm.Value.txtDescription.Text = string.Empty;
            ClusterForm.Value.cmbBackgroundVisual.SelectedItem = ClusterForm.Value.cmbBackgroundVisual.Items[0];
            ClusterForm.Value.TxtLocation.Text = string.Empty;

            // Make sure these buttons are enabled for creation
            ClusterForm.Value.BtnSector1.Enabled = true;
            ClusterForm.Value.BtnSector2.Enabled = true;
            ClusterForm.Value.BtnSector3.Enabled = true;
            ClusterForm.Value.BtnSector4.Enabled = true;

            ClusterForm.Value.Show();
        }

        private void BtnRemoveCluster_Click(object sender, EventArgs e)
        {
            Cluster selectedCluster = GetSelectedCluster();
            if (selectedCluster == null)
            {
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => ReferenceEquals(a.Value, selectedCluster));

            foreach (Sector sector in cluster.Value.Sectors)
            {
                foreach (Zone zone in sector.Zones)
                {
                    // Remove gate connections
                    foreach (Gate selectedGate in zone.Gates)
                    {
                        if (ImportedGateSectorResolver.TryFindCounterpartReference(AllClusters.Values, selectedGate, out ImportedGateSectorResolver.GateReference sourceReference))
                        {
                            _ = sourceReference.Zone.Gates.Remove(sourceReference.Gate);
                        }
                    }
                }
            }

            _ = AllClusters.Remove(cluster.Key);

            int index = ClustersListBox.Items.IndexOf(ClustersListBox.SelectedItem);
            ClustersListBox.Items.Remove(ClustersListBox.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            ClustersListBox.SelectedItem = index >= 0 && ClustersListBox.Items.Count > 0 ? ClustersListBox.Items[index] : null;

            if (ClustersListBox.SelectedItem == null)
            {
                SectorsListBox.Items.Clear();
            }

            GatesListBox.Items.Clear();
            RegionsListBox.Items.Clear();

            if (SectorMapForm.IsMapOptionChecked(SectorMapForm.MapOption.Keep_Window_Open) ||
                (SectorMap.IsInitialized && SectorMap.Value.Visible))
            {
                SectorMap.Value.Reset(false);
            }
        }

        private void ClustersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset current sectors to empty
            SectorsListBox.Items.Clear();
            SectorsListBox.SelectedItem = null;

            GatesListBox.Items.Clear();
            GatesListBox.SelectedItem = null;

            RegionsListBox.Items.Clear();
            RegionsListBox.SelectedItem = null;

            Cluster selectedCluster = GetSelectedCluster();
            if (selectedCluster == null)
            {
                LblDetails.Text = string.Empty;
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => ReferenceEquals(a.Value, selectedCluster));

            // Show new sectors and zones
            Sector selectedSector = null;
            foreach (Sector sector in cluster.Value.Sectors.OrderBy(a => a.Name))
            {
                _ = SectorsListBox.Items.Add(sector);
                if (selectedSector == null)
                {
                    SectorsListBox.SelectedItem = sector;
                    selectedSector = sector;
                }
            }

            // Set details
            SetDetailsText(cluster.Value, selectedSector);
        }

        private void ClustersListBox_DoubleClick(object sender, EventArgs e)
        {
            Cluster selectedCluster = GetSelectedCluster();
            if (selectedCluster == null)
            {
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => ReferenceEquals(a.Value, selectedCluster));

            ClusterForm.Value.Cluster = cluster.Value;
            ClusterForm.Value.ClusterXml = cluster.Value.CustomClusterXml;
            ClusterForm.Value.BtnCreate.Text = "Update";
            ClusterForm.Value.TxtName.Text = cluster.Value.Name;
            ClusterForm.Value.txtDescription.Text = cluster.Value.Description;
            ClusterForm.Value.cmbBackgroundVisual.SelectedItem = Forms.ClusterForm.FindBackgroundVisualMappingByCode(cluster.Value.BackgroundVisualMapping ?? cluster.Value.BaseGameMapping);
            ClusterForm.Value.TxtLocation.Text = cluster.Key.ToString();
            ClusterForm.Value.ChkAutoPlacement.Checked = !cluster.Value.CustomSectorPositioning;

            // Disable these buttons, they cannot be modified anymore
            ClusterForm.Value.BtnSector1.Enabled = false;
            ClusterForm.Value.BtnSector2.Enabled = false;
            ClusterForm.Value.BtnSector3.Enabled = false;
            ClusterForm.Value.BtnSector4.Enabled = false;

            // Select the correct button based on sectors in cluster
            var amountOfSectors = cluster.Value.Sectors.Count;
            if (amountOfSectors == 1)
                ClusterForm.Value.BtnSector1.Checked = true;
            else if (amountOfSectors == 2)
                ClusterForm.Value.BtnSector2.Checked = true;
            else if (amountOfSectors == 3)
                ClusterForm.Value.BtnSector3.Checked = true;
            else if (amountOfSectors == 4)
                ClusterForm.Value.BtnSector4.Checked = true;

            if (!string.IsNullOrWhiteSpace(cluster.Value.Soundtrack))
                ClusterForm.Value.TxtSoundtrack.Text = cluster.Value.Soundtrack;
            ClusterForm.Value.Show();
        }
        #endregion

        #region Sectors
        private void BtnNewSector_Click(object sender, EventArgs e)
        {
            Cluster selectedCluster = GetSelectedCluster();
            if (selectedCluster == null)
            {
                _ = MessageBox.Show("Please select a cluster first.");
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => ReferenceEquals(a.Value, selectedCluster));
            if (cluster.Value.Sectors.Count >= 4)
            {
                _ = MessageBox.Show("You've already reached the maximum allowed sectors in this sector.");
                return;
            }

            SectorForm.Value.Sector = null;
            SectorForm.Value.BtnCreate.Text = "Create";
            SectorForm.Value.Init();
            SectorForm.Value.Show();
        }

        private void BtnRemoveSector_Click(object sender, EventArgs e)
        {
            Sector selectedSector = GetSelectedSector();
            if (selectedSector == null)
            {
                return;
            }

            // Remove sector from cluster
            Cluster selectedCluster = GetSelectedCluster();
            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => ReferenceEquals(a.Value, selectedCluster));
            Sector sector = cluster.Value.Sectors.First(a => ReferenceEquals(a, selectedSector));

            foreach (Zone zone in sector.Zones)
            {
                // Remove gate connections
                foreach (Gate selectedGate in zone.Gates)
                {
                    if (ImportedGateSectorResolver.TryFindCounterpartReference(AllClusters.Values, selectedGate, out ImportedGateSectorResolver.GateReference sourceReference))
                    {
                        _ = sourceReference.Zone.Gates.Remove(sourceReference.Gate);
                    }
                }
            }

            _ = cluster.Value.Sectors.Remove(sector);

            RegionsListBox.Items.Clear();
            GatesListBox.Items.Clear();

            int index = SectorsListBox.Items.IndexOf(SectorsListBox.SelectedItem);
            SectorsListBox.Items.Remove(SectorsListBox.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            SectorsListBox.SelectedItem = index >= 0 && SectorsListBox.Items.Count > 0 ? SectorsListBox.Items[index] : null;

            sector = SectorsListBox.SelectedItem as Sector;

            // Set details
            SetDetailsText(cluster.Value, sector);

            if (!cluster.Value.CustomSectorPositioning)
                cluster.Value.AutoPositionSectors();

            if (SectorMapForm.IsMapOptionChecked(SectorMapForm.MapOption.Keep_Window_Open) ||
                (SectorMap.IsInitialized && SectorMap.Value.Visible))
            {
                SectorMap.Value.Reset(false);
            }
        }

        private void SectorsListBox_DoubleClick(object sender, EventArgs e)
        {
            Sector selectedSector = GetSelectedSector();
            if (selectedSector == null)
            {
                return;
            }

            Cluster selectedCluster = GetSelectedCluster();
            if (selectedCluster == null)
            {
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => ReferenceEquals(a.Value, selectedCluster));
            Sector sector = cluster.Value.Sectors.First(a => ReferenceEquals(a, selectedSector));

            SectorForm.Value.Sector = sector;
            SectorForm.Value.BtnCreate.Text = "Update";
            SectorForm.Value.Show();
        }

        private void SectorsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RegionsListBox.Items.Clear();
            RegionsListBox.SelectedItem = null;
            GatesListBox.Items.Clear();
            GatesListBox.SelectedItem = null;
            ListStations.Items.Clear();
            ListStations.SelectedItem = null;

            Cluster selectedCluster = GetSelectedCluster();
            if (selectedCluster == null)
            {
                return;
            }

            Sector selectedSector = GetSelectedSector(selectedCluster);
            if (selectedSector == null)
            {
                return;
            }

            // Show all gates that point to the selected sector
            Gate[] gates = AllClusters
                .SelectMany(a => a.Value.Sectors)
                .SelectMany(a => a.Zones ?? [])
                .SelectMany(a => a.Gates ?? [])
                .Where(a => ImportedGateSectorResolver.GateTargetsSector(AllClusters.Values, a, selectedSector))
                .ToArray();

            foreach (Gate gate in gates.OrderBy(a => a.ParentSectorName))
            {
                _ = GatesListBox.Items.Add(gate);
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => ReferenceEquals(a.Value, selectedCluster));
            Sector sector = cluster.Value.Sectors.First(a => ReferenceEquals(a, selectedSector));

            // Show all non base-game regions
            foreach (Region region in sector.Regions.Where(a => !a.IsBaseGame).OrderBy(a => a.Name))
            {
                _ = RegionsListBox.Items.Add(region);
            }

            // Show all stations
            foreach (Station station in sector.Zones
                .Where(a => !a.IsBaseGame)
                .SelectMany(a => a.Stations)
                .OrderBy(a => a.Name))
            {
                _ = ListStations.Items.Add(station);
            }

            // Set details
            SetDetailsText(cluster.Value, sector);
        }

        private void BtnShowSectorMap_Click(object sender, EventArgs e)
        {
            OpenSectorMap();
        }

        private void OpenSectorMap()
        {
            SectorMap.Value.DlcListBox.Enabled = !Forms.GalaxySettingsForm.IsCustomGalaxy;
            SectorMap.Value.GateSectorSelection = false;
            SectorMap.Value.BtnSelectLocation.Enabled = false;
            SectorMap.Value.ControlPanel.Size = new Size(176, 311);
            SectorMap.Value.BtnSelectLocation.Hide();
            SectorMap.Value.Reset();
            SectorMap.Value.Show();
        }

        private static void SetOwnershipInDetails(Sector sector, StringBuilder sb)
        {
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
                        _ = sb.AppendLine($"Ownership: {factions.First()}");
                    }
                    else
                    {
                        _ = factions.Count > 1 ? sb.AppendLine($"Ownership: (cannot be determined)") : sb.AppendLine($"Ownership: ownerless");
                    }
                }
                else
                {
                    _ = factions.Count == 0 || (factions.Count == 1 && factions.First().Equals(sector.Owner, StringComparison.OrdinalIgnoreCase))
                        ? sb.AppendLine($"Ownership: {sector.Owner}")
                        : sb.AppendLine($"Ownership: (cannot be determined)");
                }
            }
            else
            {
                if (factions.Count == 1)
                {
                    _ = sb.AppendLine($"Ownership: {factions.First()}");
                }
                else
                {
                    _ = factions.Count > 1 ? sb.AppendLine($"Ownership: (cannot be determined)") : sb.AppendLine($"Ownership: ownerless");
                }
            }
        }

        public void UpdateDetailsText()
        {
            Cluster selectedCluster = GetSelectedCluster();
            Sector selectedSector = GetSelectedSector(selectedCluster);

            if (selectedCluster == null)
            {
                LblDetails.Text = string.Empty;
                return;
            }

            SetDetailsText(selectedCluster, selectedSector);
        }

        public void SetDetailsText(Cluster cluster, Sector sector)
        {
            StringBuilder sb = new();
            IReadOnlyList<string> headers = SectorDetailsTextHelper.BuildHeaders(cluster?.ToString(), sector?.ToString());
            foreach (string header in headers)
            {
                _ = sb.AppendLine($"[{header}]");
            }

            if (sector != null)
            {
                _ = sb.AppendLine($"Sunlight: {(int)(sector.Sunlight * 100f)}%");
                _ = sb.AppendLine($"Economy: {(int)(sector.Economy * 100f)}%");
                _ = sb.AppendLine($"Security: {(int)(sector.Security * 100f)}%");

                // Show ownership
                SetOwnershipInDetails(sector, sb);

                // Random anomalies
                if (!sector.AllowRandomAnomalies)
                {
                    _ = sb.AppendLine("No random anomalies");
                }

                if (sector.DisableFactionLogic)
                {
                    _ = sb.AppendLine($"FactionLogic Disabled");
                }

                if (sector.ResourceAreas.Count > 0)
                {
                    // Show minerals in sector
                    HashSet<string> resources = sector.ResourceAreas
                        .Select(a => a.Ware)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    _ = sb.AppendLine($"Resources: {string.Join(", ", resources)}");
                }
            }
            LblDetails.Text = sb.ToString();
        }
        #endregion

        #region Connections
        private void BtnNewGate_Click(object sender, EventArgs e)
        {
            Cluster selectedCluster = GetSelectedCluster();
            Sector selectedSector = GetSelectedSector(selectedCluster);
            if (selectedCluster == null || selectedSector == null)
            {
                _ = MessageBox.Show("Please select a sector first.");
                return;
            }

            GateForm.Value.BtnCreateConnection.Text = "Create Connection";
            GateForm.Value.SourceCluster = selectedCluster;
            GateForm.Value.SourceSector = selectedSector;
            GateForm.Value.Show();
        }

        private void BtnRemoveGate_Click(object sender, EventArgs e)
        {
            if (GatesListBox.SelectedItem is not Gate selectedGate)
            {
                _ = MessageBox.Show("Please select a gate first.", "Gate selection required");
                return;
            }

            Sector selectedSector = GetSelectedSector();
            if (selectedSector == null)
                return;

            // Delete target connection
            if (!ImportedGateSectorResolver.TryFindGateReferenceBySourcePath(AllClusters.Values, selectedGate.SourcePath, out ImportedGateSectorResolver.GateReference targetReference))
            {
                _ = MessageBox.Show("Unable to resolve the selected gate by source path.", "Gate resolution failed");
                return;
            }

            Sector targetSector = targetReference.Sector;
            Zone targetZone = targetReference.Zone;
            _ = targetZone.Gates.Remove(targetReference.Gate);

            // Check to remove zone if empty
            if (targetZone.Gates.Count == 0)
            {
                _ = targetSector.Zones.Remove(targetZone);
            }

            // Delete source connection
            if (!ImportedGateSectorResolver.TryFindCounterpartReference(AllClusters.Values, selectedGate, out ImportedGateSectorResolver.GateReference sourceReference))
            {
                _ = MessageBox.Show("Unable to resolve the linked reverse gate by destination path.", "Gate resolution failed");
                return;
            }

            Sector sourceSector = sourceReference.Sector;
            Zone sourceZone = sourceReference.Zone;
            _ = sourceZone.Gates.Remove(sourceReference.Gate);

            // Check to remove zone if empty
            if (sourceZone.Gates.Count == 0)
            {
                _ = sourceSector.Zones.Remove(sourceZone);
            }

            int index = GatesListBox.Items.IndexOf(GatesListBox.SelectedItem);
            GatesListBox.Items.Remove(GatesListBox.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            GatesListBox.SelectedItem = index >= 0 && GatesListBox.Items.Count > 0 ? GatesListBox.Items[index] : null;

            if (SectorMapForm.IsMapOptionChecked(SectorMapForm.MapOption.Keep_Window_Open) ||
                (SectorMap.IsInitialized && SectorMap.Value.Visible))
            {
                SectorMap.Value.Reset(false);
            }
        }

        private void GatesListBox_DoubleClick(object sender, EventArgs e)
        {
            // Collect target gate data
            if (GatesListBox.SelectedItem is not Gate targetGate) return;
            if (targetGate.IsBaseGame)
            {
                _ = MessageBox.Show("Editing vanilla gates is not supported, they can only be deleted.");
                return;
            }

            if (!ImportedGateSectorResolver.TryFindGateReferenceBySourcePath(AllClusters.Values, targetGate.SourcePath, out ImportedGateSectorResolver.GateReference targetReference))
            {
                _ = MessageBox.Show("Unable to resolve the selected gate by source path.", "Gate resolution failed");
                return;
            }

            Cluster targetCluster = targetReference.Cluster;
            Sector targetSector = targetReference.Sector;
            Zone targetZone = targetReference.Zone;
            targetGate = targetReference.Gate;

            // Collect the source gate data
            if (!ImportedGateSectorResolver.TryFindCounterpartReference(AllClusters.Values, targetGate, out ImportedGateSectorResolver.GateReference sourceReference))
            {
                _ = MessageBox.Show("Unable to resolve the linked reverse gate by destination path.", "Gate resolution failed");
                return;
            }

            Cluster sourceCluster = sourceReference.Cluster;
            Sector sourceSector = sourceReference.Sector;
            Zone sourceZone = sourceReference.Zone;
            Gate sourceGate = sourceReference.Gate;

            // Set gates to be updated
            GateForm.Value.UpdateInfoObject = new GateForm.UpdateInfo
            {
                SourceGate = sourceGate,
                SourceZone = sourceZone,
                SourceSector = sourceSector,
                SourceCluster = sourceCluster,

                TargetGate = targetGate,
                TargetZone = targetZone,
                TargetSector = targetSector,
                TargetCluster = targetCluster
            };
            GateForm.Value.BtnCreateConnection.Text = "Update Connection";
            GateForm.Value.PrepareForUpdate();
            GateForm.Value.Show();
        }
        #endregion

        #region Regions
        private void BtnNewRegion_Click(object sender, EventArgs e)
        {
            Cluster cluster = GetSelectedCluster();
            Sector sector = GetSelectedSector(cluster);
            if (sector == null)
            {
                _ = MessageBox.Show("Please select a valid sector first.");
                return;
            }

            RegionForm.Value.Sector = sector;
            RegionForm.Value.Show();
        }

        private void BtnRemoveRegion_Click(object sender, EventArgs e)
        {
            if (RegionsListBox.SelectedItem is not Region selectedRegion)
            {
                return;
            }

            Cluster cluster = GetSelectedCluster();
            Sector sector = GetSelectedSector(cluster);
            if (cluster == null || sector == null)
                return;

            // Remove region from sector
            _ = sector.Regions.Remove(selectedRegion);

            int index = RegionsListBox.Items.IndexOf(RegionsListBox.SelectedItem);
            RegionsListBox.Items.Remove(RegionsListBox.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            RegionsListBox.SelectedItem = index >= 0 && RegionsListBox.Items.Count > 0 ? RegionsListBox.Items[index] : null;

            if (SectorMapForm.IsMapOptionChecked(SectorMapForm.MapOption.Keep_Window_Open) ||
                (SectorMap.IsInitialized && SectorMap.Value.Visible))
            {
                SectorMap.Value.Reset(false);
            }
        }

        private void RegionsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (RegionsListBox.SelectedItem is not Region selectedRegion)
            {
                return;
            }

            Cluster cluster = GetSelectedCluster();
            Sector sector = GetSelectedSector(cluster);
            if (cluster == null || sector == null)
                return;

            RegionForm.Value.Sector = sector;
            RegionForm.Value.CustomRegion = selectedRegion;
            RegionForm.Value.Show();
        }
        #endregion

        #region Stations
        private void BtnNewStation_Click(object sender, EventArgs e)
        {
            Cluster cluster = GetSelectedCluster();
            Sector sector = GetSelectedSector(cluster);
            if (sector == null)
            {
                _ = MessageBox.Show("Please select a valid sector first.");
                return;
            }

            _stationForm.Value.Cluster = cluster;
            _stationForm.Value.Sector = sector;
            _stationForm.Value.Station = null;
            _stationForm.Value.Show();
        }

        private void BtnRemoveStation_Click(object sender, EventArgs e)
        {
            if (ListStations.SelectedItem is not Station selectedStation)
            {
                return;
            }

            Cluster cluster = GetSelectedCluster();
            Sector sector = GetSelectedSector(cluster);
            if (sector == null)
            {
                return;
            }

            // Remove station from zone
            Zone zone = sector.Zones.First(a => a.Stations.Contains(selectedStation));
            _ = zone.Stations.Remove(selectedStation);

            // Also remove the left-over zone of the station
            sector.Zones.Remove(zone);

            int index = ListStations.Items.IndexOf(ListStations.SelectedItem);
            ListStations.Items.Remove(ListStations.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            ListStations.SelectedItem = index >= 0 && ListStations.Items.Count > 0 ? ListStations.Items[index] : null;

            // Set details
            SetDetailsText(cluster, sector);

            if (SectorMapForm.IsMapOptionChecked(SectorMapForm.MapOption.Keep_Window_Open) ||
                (SectorMap.IsInitialized && SectorMap.Value.Visible))
            {
                SectorMap.Value.Reset(false);
            }
        }

        private void ListStations_DoubleClick(object sender, EventArgs e)
        {
            if (ListStations.SelectedItem is not Station selectedStation)
            {
                return;
            }

            Cluster cluster = GetSelectedCluster();
            Sector sector = GetSelectedSector(cluster);
            if (cluster == null || sector == null)
                return;

            _stationForm.Value.Cluster = cluster;
            _stationForm.Value.Sector = sector;
            _stationForm.Value.Station = selectedStation;
            _stationForm.Value.Show();
        }
        #endregion

        #region Jobs
        private void BtnJobs_Click(object sender, EventArgs e)
        {
            JobsForm.Value.Initialize();
            JobsForm.Value.Show();
        }

        [GeneratedRegex(@"[<>:""/\\|?*]")]
        private static partial Regex SanitizeUnsafe();
        [GeneratedRegex(@"[^a-zA-Z0-9_]")]
        private static partial Regex SanitizeNonAlphaNumeric();
        #endregion

        #region Factories
        private void BtnFactories_Click(object sender, EventArgs e)
        {
            FactoriesForm.Value.Initialize();
            FactoriesForm.Value.Show();
        }
        #endregion

        #region ObjectsOverview
        private void BtnObjectsOverview_Click(object sender, EventArgs e)
        {
            _objectOverviewForm.Value.Show();
        }
        #endregion

        #region Custom Factions
        private void BtnCustomFactions_Click(object sender, EventArgs e)
        {
            FactionsForm.Value.Show();
        }
        #endregion
    }
}
