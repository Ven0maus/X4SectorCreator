using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using X4SectorCreator.Configuration;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;
using Region = X4SectorCreator.Objects.Region;

namespace X4SectorCreator
{
    public partial class MainForm : Form
    {
        private GalaxySettingsForm _galaxySettingsForm;
        private SectorMapForm _sectorMapForm;
        private ClusterForm _clusterForm;
        private SectorForm _sectorForm;
        private GateForm _gateForm;
        private RegionForm _regionForm;
        private VersionUpdateForm _versionUpdateForm;

        private string _currentX4Version;

        private readonly string _sectorMappingFilePath = Path.Combine(Application.StartupPath, "Mappings/sector_mappings.json");
        private readonly string _dlcMappingFilePath = Path.Combine(Application.StartupPath, "Mappings/dlc_mappings.json");

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<(int, int), Cluster> AllClusters { get; private set; }

        public readonly Dictionary<string, Color> FactionColorMapping;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static MainForm Instance { get; private set; }

        public GalaxySettingsForm GalaxySettingsForm => _galaxySettingsForm != null && !_galaxySettingsForm.IsDisposed ? _galaxySettingsForm : (_galaxySettingsForm = new GalaxySettingsForm());
        public RegionForm RegionForm => _regionForm != null && !_regionForm.IsDisposed ? _regionForm : (_regionForm = new RegionForm());
        public SectorMapForm SectorMapForm => _sectorMapForm != null && !_sectorMapForm.IsDisposed ? _sectorMapForm : (_sectorMapForm = new SectorMapForm());
        public ClusterForm ClusterForm => _clusterForm != null && !_clusterForm.IsDisposed ? _clusterForm : (_clusterForm = new ClusterForm());
        public SectorForm SectorForm => _sectorForm != null && !_sectorForm.IsDisposed ? _sectorForm : (_sectorForm = new SectorForm());
        public GateForm GateForm => _gateForm != null && !_gateForm.IsDisposed ? _gateForm : (_gateForm = new GateForm());
        public VersionUpdateForm VersionUpdateForm => _versionUpdateForm != null && !_versionUpdateForm.IsDisposed
            ? _versionUpdateForm
            : (_versionUpdateForm = new VersionUpdateForm());

        public readonly Dictionary<string, string> BackgroundVisualMapping;
        public readonly Dictionary<string, string> DlcMappings;

        private ClusterOption _selectedClusterOption = ClusterOption.Custom;

        public MainForm()
        {
            InitializeComponent();

            if (Instance != null)
            {
                throw new Exception("No more than one instance of \"MainForm\" can be active.");
            }

            Instance = this;

            ClusterCollection clusterCollection = InitAllClusters();

            // Set background visual mapping
            BackgroundVisualMapping = AllClusters
                .Where(a => a.Value.IsBaseGame)
                .Where(a => !string.IsNullOrWhiteSpace(a.Value.BaseGameMapping))
                .ToDictionary(a => a.Value.Name, a => a.Value.BaseGameMapping);

            // Set dlc mappings
            string json = File.ReadAllText(_dlcMappingFilePath);
            DlcMappings = JsonSerializer.Deserialize<List<DlcMapping>>(json, ConfigSerializer.SerializerOptions)
                .ToDictionary(a => a.Dlc, a => a.Prefix);

            // Set faction color mapping
            FactionColorMapping = clusterCollection.FactionColors.ToDictionary(a => a.Key, a => HexToColor(a.Value), StringComparer.OrdinalIgnoreCase);

            // Set the default value to be custom always
            UpdateClusterOptions();
        }

        public ClusterCollection InitAllClusters(bool replaceAllClusters = true)
        {
            string json = File.ReadAllText(_sectorMappingFilePath);
            ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(json, ConfigSerializer.SerializerOptions);

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
                    cluster.Value.CustomSectorPositioning = true;

                foreach (Sector sector in cluster.Value.Sectors)
                {
                    // Init regular sectors
                    if (cluster.Value.IsBaseGame && string.IsNullOrWhiteSpace(sector.BaseGameMapping))
                    {
                        sector.BaseGameMapping = "sector001";
                    }

                    sector.Regions ??= [];
                    sector.Zones ??= [];
                    foreach (Zone zone in sector.Zones)
                    {
                        zone.Gates ??= [];
                    }

                    // Auto-determine offset for each sector
                    SectorForm.DetermineSectorOffset(cluster.Value, sector);
                }
            }

            // Create also the required connections for vanilla
            VanillaGateConnectionParser.CreateVanillaGateConnections(clusterLookup);

            return clusterCollection;
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
            LblDetails.Text = string.Empty;

            switch (_selectedClusterOption)
            {
                case ClusterOption.Custom:
                    foreach (Cluster cluster in AllClusters.Values.Where(a => !a.IsBaseGame).OrderBy(a => a.Name))
                    {
                        _ = ClustersListBox.Items.Add(cluster.Name);
                    }

                    break;
                case ClusterOption.Vanilla:
                    foreach (Cluster cluster in AllClusters.Values.Where(a => a.IsBaseGame).OrderBy(a => a.Name))
                    {
                        _ = ClustersListBox.Items.Add(cluster.Name);
                    }

                    break;
                case ClusterOption.Both:
                    foreach (Cluster cluster in AllClusters.Values.OrderBy(a => a.Name))
                    {
                        _ = ClustersListBox.Items.Add(cluster.Name);
                    }

                    break;
                default:
                    throw new NotImplementedException(selectedValue.ToString());
            }
            ClustersListBox.SelectedIndex = ClustersListBox.Items.Count == 0 ? -1 : 0;
        }

        private void UpdateClusterOptions()
        {
            if (GalaxySettingsForm.IsCustomGalaxy)
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

        /// <summary>
        /// Used to toggle between base game galaxy and custom galaxy.
        /// </summary>
        public void ToggleGalaxyMode(Dictionary<(int, int), Cluster> mergedClusters)
        {
            AllClusters = GalaxySettingsForm.IsCustomGalaxy
                ? AllClusters
                    .Where(a => !a.Value.IsBaseGame)
                    .ToDictionary(a => a.Key, a => a.Value)
                : mergedClusters;

            UpdateClusterOptions();
        }

        private void BtnGalaxySettings_Click(object sender, EventArgs e)
        {
            GalaxySettingsForm.Initialize();
            GalaxySettingsForm.Show();
        }

        private void BtnShowSectorMap_Click(object sender, EventArgs e)
        {
            SectorMapForm.DlcListBox.Enabled = !GalaxySettingsForm.IsCustomGalaxy;
            SectorMapForm.chkShowX4Sectors.Enabled = !GalaxySettingsForm.IsCustomGalaxy;
            SectorMapForm.GateSectorSelection = false;
            SectorMapForm.BtnSelectLocation.Enabled = false;
            SectorMapForm.ControlPanel.Size = new Size(176, 241);
            SectorMapForm.BtnSelectLocation.Hide();
            SectorMapForm.Reset();
            SectorMapForm.Show();
        }

        private void BtnGenerateDiffs_Click(object sender, EventArgs e)
        {
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
                .Where(a => !SectorForm.IsClusterPlacementValid(a))
                .ToArray();
            if (invalidClusters.Length != 0)
            {
                _ = MessageBox.Show($"Following clusters have sectors that have overlapped placements, please fix these first:\n- " +
                    string.Join("\n- ", invalidClusters.Select(a => a.Name)));
                return;
            }

            const string lblModName = "Please enter the name you'd like to use for your mod folder:";
            const string lblModPrefix = "Please enter the prefix you'd like to use for your mod:";
            Dictionary<string, string> modInfo = MultiInputDialog.Show("Mod information",
                lblModName,
                lblModPrefix);
            if (modInfo == null || modInfo.Count == 0)
            {
                return;
            }

            string modName = modInfo[lblModName];
            string modPrefix = modInfo[lblModPrefix];

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

            // lowercase modPrefix just incase
            modPrefix = modPrefix.ToLower();

            List<Cluster> clusters = [.. AllClusters.Values];

            // Collects all changes done to base game content
            ClusterCollection nonModifiedBaseGameData = InitAllClusters(false);
            VanillaChanges vanillaChanges = CollectVanillaChanges(nonModifiedBaseGameData);

            // Generate each xml
            string mainFolder = Path.Combine(Application.StartupPath, "GeneratedXml");
            string modFolder = Path.Combine(Application.StartupPath, "GeneratedXml", modName);
            try
            {
                // Clear up any previous xml
                if (Directory.Exists(mainFolder))
                {
                    Directory.Delete(mainFolder, true);
                }

                // Generate all xml files
                MacrosGeneration.Generate(modFolder, modName, modPrefix, clusters);
                MapDefaultsGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges);
                GalaxyGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges, nonModifiedBaseGameData);
                ClusterGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges);
                SectorGeneration.Generate(modFolder, modPrefix, clusters, nonModifiedBaseGameData, vanillaChanges);
                ZoneGeneration.Generate(modFolder, modPrefix, clusters, nonModifiedBaseGameData, vanillaChanges);
                ContentGeneration.Generate(modFolder, modName, _currentX4Version.Replace(".", string.Empty) + "0", clusters, vanillaChanges);
                RegionDefinitionGeneration.Generate(modFolder, modPrefix, clusters);
                GameStartsGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges);
                DlcDisableGeneration.Generate(modFolder);
                GodGeneration.Generate(modFolder);
                JobsGeneration.Generate(modFolder);
            }
            catch (Exception ex)
            {
                // Clear up corrupted xml
                Directory.Delete(mainFolder, true);
                _ = MessageBox.Show("Something went wrong during xml generation: " + ex.Message,
                    "Error in XML Generation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show succes message
            _ = MessageBox.Show("XML Files were succesfully generated in the xml folder.");
        }

        private VanillaChanges CollectVanillaChanges(ClusterCollection nonModifiedBaseGameData)
        {
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                return new VanillaChanges();
            }

            Dictionary<string, Cluster> vanillaClusters = AllClusters.Values
                .Where(a => a.IsBaseGame)
                .ToDictionary(a => a.BaseGameMapping);

            Dictionary<string, Cluster> nonModifiedVanillaClusters = nonModifiedBaseGameData
                .Clusters
                .ToDictionary(a => a.BaseGameMapping);

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
                    nonModifiedCluster.CustomSectorPositioning != modifiedCluster.CustomSectorPositioning)
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
                        nonModifiedSector.AllowRandomAnomalies != modifiedSector.AllowRandomAnomalies)
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
                            Zone matchingZone = modifiedSector.Zones.FirstOrDefault(a => a.Name != null && a.Name.Equals(nonModifiedZone.Name, StringComparison.OrdinalIgnoreCase));
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

        private async void MainForm_Load(object sender, EventArgs e)
        {
            VersionChecker versionChecker = new();

            // Set form title
            Text += $" [APP v{versionChecker.CurrentVersion} | X4 v{versionChecker.TargetGameVersion}]";
            _currentX4Version = versionChecker.TargetGameVersion;

            // Check for update
            (bool NewVersionAvailable, VersionInfo VersionInfo) result = await versionChecker.CheckForUpdatesAsync();
            if (result.NewVersionAvailable)
            {
                // If the app version remains the same, but the X4 version is different
                // That means only the mapping was updated, we can automatically update this.
                if (result.VersionInfo.AppVersion.Equals(versionChecker.CurrentVersion))
                {
                    string newSectorMappingJson = await VersionChecker.GetUpdatedSectorMappingAsync();
                    string oldSectorMappingJson = File.ReadAllText(_sectorMappingFilePath);
                    if (newSectorMappingJson != null && !oldSectorMappingJson.Equals(newSectorMappingJson))
                    {
                        try
                        {
                            // Update mapping file
                            File.WriteAllText(_sectorMappingFilePath, newSectorMappingJson);
                            ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(newSectorMappingJson, ConfigSerializer.SerializerOptions);

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
                            _currentX4Version = versionChecker.TargetGameVersion;

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
                    VersionUpdateForm.txtCurrentVersion.Text = $"v{versionChecker.CurrentVersion}";
                    VersionUpdateForm.txtCurrentX4Version.Text = $"v{versionChecker.TargetGameVersion}";
                    VersionUpdateForm.txtUpdateVersion.Text = $"v{result.VersionInfo.AppVersion}";
                    VersionUpdateForm.txtUpdateX4Version.Text = $"v{result.VersionInfo.X4Version}";
                    VersionUpdateForm.Show();
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

        private void SetDetailsText(Cluster cluster, Sector sector)
        {
            StringBuilder sb = new();
            _ = sb.Append($"[{cluster.Name}]");

            if (sector != null)
            {
                if (!sector.Name.Equals(cluster.Name, StringComparison.OrdinalIgnoreCase))
                {
                    _ = sb.AppendLine($"[{sector.Name}]");
                }

                _ = sb.AppendLine($"Sunlight: {(int)(sector.Sunlight * 100f)}%");
                _ = sb.AppendLine($"Economy: {(int)(sector.Economy * 100f)}%");
                _ = sb.AppendLine($"Security: {(int)(sector.Security * 100f)}%");
                if (!sector.AllowRandomAnomalies)
                {
                    _ = sb.AppendLine("No random anomalies");
                }

                if (sector.DisableFactionLogic)
                {
                    _ = sb.AppendLine($"FactionLogic Disabled");
                }

                if (sector.Regions.Count > 0)
                {
                    // Show minerals in sector
                    HashSet<string> resources = sector.Regions
                        .SelectMany(a => a.Definition.Resources)
                        .Select(a => a.Ware)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    _ = sb.AppendLine($"Resources: {string.Join(", ", resources)}");
                }
            }
            LblDetails.Text = sb.ToString();
        }

        #region Configuration
        public void Reset(bool fromImport)
        {
            // Reset
            if (!fromImport)
            {
                GalaxySettingsForm.GalaxyName = "xu_ep2_universe";
                GalaxySettingsForm.IsCustomGalaxy = false;
            }

            // Re-initialize all clusters properly
            _ = InitAllClusters();

            // Set the default value to be custom
            UpdateClusterOptions();

            if (!fromImport)
            {
                RegionDefinitionForm.RegionDefinitions.Clear();
            }
        }
        private void BtnReset_Click(object sender, EventArgs e)
        {
            Reset(false);
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
                    List<Cluster> allModifiedClusters = AllClusters.Values
                        .Where(a => !a.IsBaseGame)
                        .ToList();

                    ClusterCollection nonModifiedBaseGameData = InitAllClusters(false);
                    HashSet<string> gateConnections = nonModifiedBaseGameData
                        .Clusters
                        .SelectMany(a => a.Sectors)
                        .SelectMany(a => a.Zones)
                        .SelectMany(a => a.Gates)
                        .Select(a => a.ConnectionName)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    // Also add clusters that are basegame but have new connections compared to vanilla
                    Cluster[] baseGameClusters = AllClusters.Values.Where(a => a.IsBaseGame).ToArray();
                    foreach (Cluster cluster in baseGameClusters)
                    {
                        foreach (Sector sector in cluster.Sectors)
                        {
                            // If sector doesn't exist in vanilla, we need to export it
                            if (!sector.IsBaseGame)
                            {
                                allModifiedClusters.Add(cluster);
                                continue;
                            }

                            foreach (Zone zone in sector.Zones)
                            {
                                foreach (Gate gate in zone.Gates)
                                {
                                    // Check if gate exists in vanilla
                                    if (!gate.IsBaseGame)
                                    {
                                        allModifiedClusters.Add(cluster);
                                    }
                                }
                            }
                        }
                    }

                    // Support also vanilla changes
                    VanillaChanges vanillaChanges = CollectVanillaChanges(nonModifiedBaseGameData);

                    string jsonContent = ConfigSerializer.Serialize(allModifiedClusters, vanillaChanges);
                    File.WriteAllText(filePath, jsonContent);
                    _ = MessageBox.Show($"Configuration exported succesfully.", "Success");
                }
                catch (Exception)
                {
                    _ = MessageBox.Show("Invalid JSON content in file, please try another file.",
                        "Invalid JSON Content", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
                    List<Cluster> clusters = configuration.clusters;
                    // Reset configuration
                    Reset(true);

                    if (GalaxySettingsForm.IsCustomGalaxy)
                    {
                        ToggleGalaxyMode(null);
                    }

                    // Apply vanilla changes to AllClusters
                    if (configuration.vanillaChanges != null)
                    {
                        SupportVanillaChangesInConfigImport(configuration);
                    }

                    var vanillaClustersLazy = new Lazy<Cluster[]>(() => InitAllClusters(false).Clusters.Where(a => a.IsBaseGame).ToArray());

                    // Import new configuration
                    foreach (Cluster cluster in clusters)
                    {
                        // Adjust the cluster
                        ReplaceClusterByImport(cluster);

                        // Apply support additions for new versions
                        Import_Support_NewVersions(cluster, vanillaClustersLazy);

                        // Setup listboxes
                        if (!cluster.IsBaseGame)
                        {
                            _ = ClustersListBox.Items.Add(cluster.Name);
                        }
                    }

                    // No longer needed
                    _clusterDlcLookup = null;

                    // Select first one so sector and zones populate automatically
                    ClustersListBox.SelectedItem = clusters.FirstOrDefault(a => !a.IsBaseGame)?.Name ?? null;

                    _ = MessageBox.Show($"Configuration imported succesfully.", "Success");
                }
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

                Sector sector = cluster.Sectors.FirstOrDefault(a => a.Name.Equals(pair.Sector.Name, StringComparison.OrdinalIgnoreCase));
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
                Sector sector = cluster.Sectors.FirstOrDefault(a => a.Name.Equals(pair.Sector.Name, StringComparison.OrdinalIgnoreCase));
                if (sector == null)
                {
                    continue;
                }

                Zone zone = sector.Zones.FirstOrDefault(a =>
                {
                    return (!string.IsNullOrWhiteSpace(a.Name) && !string.IsNullOrWhiteSpace(pair.Zone.Name) && 
                        a.Name.Equals(pair.Zone.Name, StringComparison.OrdinalIgnoreCase)) || 
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
                Sector sector = cluster.Sectors.FirstOrDefault(a => a.Name.Equals(Old.Name, StringComparison.OrdinalIgnoreCase));
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
            }
        }

        private void ReplaceClusterByImport(Cluster cluster)
        {
            if (!AllClusters.TryGetValue((cluster.Position.X, cluster.Position.Y), out Cluster currentCluster))
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

            foreach (Sector newSector in cluster.Sectors)
            {
                // Check if it exist then adjust it else add it
                Sector currentSector = currentCluster.Sectors.FirstOrDefault(a => a.Name.Equals(newSector.Name, StringComparison.OrdinalIgnoreCase));
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
                currentSector.Offset = newSector.Offset;

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
                }
            }
        }

        private Dictionary<(int, int), Cluster> _clusterDlcLookup;
        private void Import_Support_NewVersions(Cluster cluster, Lazy<Cluster[]> vanillaClustersLazy)
        {
            if (string.IsNullOrWhiteSpace(cluster.BackgroundVisualMapping))
            {
                cluster.BackgroundVisualMapping = "cluster_01";
            }

            // Re-check DLCs
            if (cluster.Dlc == null)
            {
                if (_clusterDlcLookup == null)
                {
                    // Create new lookup table
                    string json = File.ReadAllText(_sectorMappingFilePath);
                    ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(json, ConfigSerializer.SerializerOptions);
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
                var placements = Enum.GetValues<SectorPlacement>().OrderBy(a => a).ToList();
                foreach (var sector in cluster.Sectors)
                {
                    bool placementSet = false;
                    if (sector.IsBaseGame)
                    {
                        // Determine if sector is vanilla, then copy over the original values
                        var vanillaClusters = vanillaClustersLazy.Value;
                        var matchingCluster = vanillaClusters.FirstOrDefault(a => a.BaseGameMapping.Equals(cluster.BaseGameMapping));
                        if (matchingCluster != null)
                        {
                            var matchingSector = matchingCluster.Sectors.FirstOrDefault(a => a.BaseGameMapping.Equals(sector.BaseGameMapping));
                            if (matchingSector != null)
                            {
                                sector.Placement = matchingSector.Placement;
                                placements.Remove(sector.Placement);
                                placementSet = true;
                            }
                        }
                    }

                    if (!placementSet)
                    {
                        sector.Placement = placements[^1];
                        placements.Remove(sector.Placement);
                    }

                    SectorForm.DetermineSectorOffset(cluster, sector);
                }
            }
            else if (cluster.Sectors.Count > 1)
            {
                // Determine offset dynamically based on placements
                foreach (var sector in cluster.Sectors)
                {
                    SectorForm.DetermineSectorOffset(cluster, sector);
                }
            }
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            string directoryPath = Path.Combine(Application.StartupPath, "GeneratedXml");
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            _ = Process.Start("explorer.exe", directoryPath);
        }
        #endregion

        #region Clusters
        private void BtnNewCluster_Click(object sender, EventArgs e)
        {
            ClusterForm.Cluster = null;
            ClusterForm.BtnCreate.Text = "Create";
            ClusterForm.TxtName.Text = string.Empty;
            ClusterForm.txtDescription.Text = string.Empty;
            ClusterForm.cmbBackgroundVisual.SelectedItem = ClusterForm.cmbBackgroundVisual.Items[0];
            ClusterForm.TxtLocation.Text = string.Empty;
            ClusterForm.Show();
        }

        private void BtnRemoveCluster_Click(object sender, EventArgs e)
        {
            string selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));

            foreach (Sector sector in cluster.Value.Sectors)
            {
                foreach (Zone zone in sector.Zones)
                {
                    // Remove gate connections
                    foreach (Gate selectedGate in zone.Gates)
                    {
                        Sector sourceSector = AllClusters.Values
                            .SelectMany(a => a.Sectors)
                            .First(a => a.Name.Equals(selectedGate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                        Zone sourceZone = sourceSector.Zones
                            .FirstOrDefault(a => a.Gates
                                .Any(a => a.SourcePath
                                    .Equals(selectedGate.DestinationPath, StringComparison.OrdinalIgnoreCase)));
                        Gate sourceGate = sourceZone.Gates.FirstOrDefault(a => a.SourcePath.Equals(selectedGate.DestinationPath, StringComparison.OrdinalIgnoreCase));
                        _ = sourceZone.Gates.Remove(sourceGate);
                    }
                }
            }

            _ = AllClusters.Remove(cluster.Key);

            // Re-align ids
            int count = 0;
            foreach (Cluster clust in AllClusters.Values.Where(a => !a.IsBaseGame).OrderBy(a => a.Id))
            {
                clust.Id = ++count;
            }

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

            string selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                LblDetails.Text = string.Empty;
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));

            // Show new sectors and zones
            Sector selectedSector = null;
            foreach (Sector sector in cluster.Value.Sectors.OrderBy(a => a.Name))
            {
                _ = SectorsListBox.Items.Add(sector.Name);
                if (selectedSector == null)
                {
                    SectorsListBox.SelectedItem = sector.Name;
                    selectedSector = sector;
                }
            }

            // Set details
            SetDetailsText(cluster.Value, selectedSector);
        }

        private void ClustersListBox_DoubleClick(object sender, EventArgs e)
        {
            string selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));

            ClusterForm.Cluster = cluster.Value;
            ClusterForm.BtnCreate.Text = "Update";
            ClusterForm.TxtName.Text = selectedClusterName;
            ClusterForm.txtDescription.Text = cluster.Value.Description;
            ClusterForm.cmbBackgroundVisual.SelectedItem = ClusterForm.FindBackgroundVisualMappingByCode(cluster.Value.BackgroundVisualMapping ?? cluster.Value.BaseGameMapping);
            ClusterForm.TxtLocation.Text = cluster.Key.ToString();
            ClusterForm.ChkAutoPlacement.Checked = !cluster.Value.CustomSectorPositioning;
            ClusterForm.Show();
        }
        #endregion

        #region Sectors
        private void BtnNewSector_Click(object sender, EventArgs e)
        {
            string selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                _ = MessageBox.Show("Please select a cluster first.");
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            if (cluster.Value.Sectors.Count >= 3)
            {
                _ = MessageBox.Show("You've already reached the maximum allowed sectors in this sector.");
                return;
            }

            SectorForm.Sector = null;
            SectorForm.BtnCreate.Text = "Create";
            SectorForm.Show();
        }

        private void BtnRemoveSector_Click(object sender, EventArgs e)
        {
            string selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSectorName))
            {
                return;
            }

            // Remove sector from cluster
            string selectedClusterName = ClustersListBox.SelectedItem as string;
            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            foreach (Zone zone in sector.Zones)
            {
                // Remove gate connections
                foreach (Gate selectedGate in zone.Gates)
                {
                    Sector sourceSector = AllClusters.Values
                        .SelectMany(a => a.Sectors)
                        .First(a => a.Name.Equals(selectedGate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                    Zone sourceZone = sourceSector.Zones
                        .First(a => a.Gates
                            .Any(a => a.SourcePath
                                .Equals(selectedGate.DestinationPath, StringComparison.OrdinalIgnoreCase)));
                    Gate sourceGate = sourceZone.Gates.First(a => a.SourcePath.Equals(selectedGate.DestinationPath, StringComparison.OrdinalIgnoreCase));
                    _ = sourceZone.Gates.Remove(sourceGate);
                }
            }

            _ = cluster.Value.Sectors.Remove(sector);

            // Re-align ids before re-calculate
            int count = 0;
            foreach (Sector sect in cluster.Value.Sectors.OrderBy(a => a.Id))
            {
                sect.Id = ++count;
            }

            RegionsListBox.Items.Clear();
            GatesListBox.Items.Clear();

            int index = SectorsListBox.Items.IndexOf(SectorsListBox.SelectedItem);
            SectorsListBox.Items.Remove(SectorsListBox.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            SectorsListBox.SelectedItem = index >= 0 && SectorsListBox.Items.Count > 0 ? SectorsListBox.Items[index] : null;

            sector = SectorsListBox.SelectedItem != null
                ? cluster.Value.Sectors.First(a => a.Name.Equals(SectorsListBox.SelectedItem as string, StringComparison.OrdinalIgnoreCase))
                : null;

            // Set details
            SetDetailsText(cluster.Value, sector);
        }

        private void SectorsListBox_DoubleClick(object sender, EventArgs e)
        {
            string selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSectorName))
            {
                return;
            }

            string selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            SectorForm.Sector = sector;
            SectorForm.BtnCreate.Text = "Update";
            SectorForm.Show();
        }

        private void SectorsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RegionsListBox.Items.Clear();
            RegionsListBox.SelectedItem = null;
            GatesListBox.Items.Clear();
            GatesListBox.SelectedItem = null;

            string selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                return;
            }

            string selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSectorName))
            {
                return;
            }

            // Show all gates that point to the selected sector
            Gate[] gates = AllClusters
                .SelectMany(a => a.Value.Sectors)
                .SelectMany(a => a.Zones ?? [])
                .SelectMany(a => a.Gates ?? [])
                .Where(a => a.DestinationSectorName.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (Gate gate in gates.OrderBy(a => a.ParentSectorName))
            {
                _ = GatesListBox.Items.Add(gate);
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            // Show all regions
            foreach (Region region in sector.Regions.OrderBy(a => a.Name))
            {
                _ = RegionsListBox.Items.Add(region);
            }

            // Set details
            SetDetailsText(cluster.Value, sector);
        }
        #endregion

        #region Connections
        private void BtnNewGate_Click(object sender, EventArgs e)
        {
            string selectedClusterName = ClustersListBox.SelectedItem as string;
            string selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedClusterName) ||
                string.IsNullOrWhiteSpace(selectedSectorName))
            {
                _ = MessageBox.Show("Please select a sector first.");
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            GateForm.BtnCreateConnection.Text = "Create Connection";
            GateForm.SourceCluster = cluster.Value;
            GateForm.SourceSector = sector;
            GateForm.Show();
        }

        private void BtnRemoveGate_Click(object sender, EventArgs e)
        {
            if (GatesListBox.SelectedItem is not Gate selectedGate)
            {
                _ = MessageBox.Show("Please select a gate first.", "Gate selection required");
                return;
            }

            string selectedSectorName = SectorsListBox.SelectedItem as string;

            // Delete target connection
            Sector targetSector = AllClusters.Values
                .SelectMany(a => a.Sectors)
                .First(a => a.Name.Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
            Zone targetZone = targetSector.Zones
                .Where(a => a.Gates
                    .Any(a => a.DestinationSectorName
                        .Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase)))
                .First(a => a.Gates.Contains(selectedGate));
            _ = targetZone.Gates.Remove(selectedGate);

            // Check to remove zone if empty
            if (targetZone.Gates.Count == 0)
            {
                _ = targetSector.Zones.Remove(targetZone);
            }

            // Re-order zone ids if needed
            int count = 0;
            foreach (Zone tZone in targetSector.Zones.OrderBy(a => a.Id))
            {
                tZone.Id = ++count;
            }

            // Delete source connection
            Sector sourceSector = AllClusters.Values
                .SelectMany(a => a.Sectors)
                .First(a => a.Name.Equals(selectedGate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
            Zone sourceZone = sourceSector.Zones
                .First(a => a.Gates
                    .Any(a => a.SourcePath
                        .Equals(selectedGate.DestinationPath, StringComparison.OrdinalIgnoreCase)));
            Gate sourceGate = sourceZone.Gates.First(a => a.SourcePath.Equals(selectedGate.DestinationPath, StringComparison.OrdinalIgnoreCase));
            _ = sourceZone.Gates.Remove(sourceGate);

            // Check to remove zone if empty
            if (sourceZone.Gates.Count == 0)
            {
                _ = sourceSector.Zones.Remove(sourceZone);
            }

            // Re-order zone ids if needed
            count = 0;
            foreach (Zone sZone in sourceSector.Zones.OrderBy(a => a.Id))
            {
                sZone.Id = ++count;
            }

            int index = GatesListBox.Items.IndexOf(GatesListBox.SelectedItem);
            GatesListBox.Items.Remove(GatesListBox.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            GatesListBox.SelectedItem = index >= 0 && GatesListBox.Items.Count > 0 ? GatesListBox.Items[index] : null;
        }

        private void GatesListBox_DoubleClick(object sender, EventArgs e)
        {
            // Collect target gate data
            Gate targetGate = GatesListBox.SelectedItem as Gate;
            if (targetGate.IsBaseGame)
            {
                _ = MessageBox.Show("Editing vanilla gates is not supported, they can only be deleted.");
                return;
            }

            var targetQueryResult = AllClusters.Values
                .SelectMany(cluster => cluster.Sectors, (cluster, sector) => new { cluster, sector })
                .First(pair => pair.sector.Name.Equals(targetGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));

            Cluster targetCluster = targetQueryResult.cluster;
            Sector targetSector = targetQueryResult.sector;
            Zone targetZone = targetSector.Zones.First(a => a.Gates.Contains(targetGate));

            // Collect the source gate data
            var sourceQueryResult = AllClusters.Values
                .SelectMany(cluster => cluster.Sectors, (cluster, sector) => new { cluster, sector })
                .First(pair => pair.sector.Name.Equals(targetGate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));

            // Delete source connection
            Cluster sourceCluster = sourceQueryResult.cluster;
            Sector sourceSector = sourceQueryResult.sector;
            Zone sourceZone = sourceSector.Zones
                .First(a => a.Gates
                    .Any(a => a.SourcePath
                        .Equals(targetGate.DestinationPath, StringComparison.OrdinalIgnoreCase)));
            Gate sourceGate = sourceZone.Gates.First(a => a.SourcePath.Equals(targetGate.DestinationPath, StringComparison.OrdinalIgnoreCase));

            // Set gates to be updated
            GateForm.UpdateInfoObject = new GateForm.UpdateInfo
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
            GateForm.BtnCreateConnection.Text = "Update Connection";
            GateForm.PrepareForUpdate();
            GateForm.Show();
        }
        #endregion

        #region Regions
        private void BtnNewRegion_Click(object sender, EventArgs e)
        {
            string selectedSector = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSector))
            {
                _ = MessageBox.Show("Please select a valid sector first.");
                return;
            }

            string selectedCluster = ClustersListBox.SelectedItem as string;
            Cluster cluster = AllClusters.Values.First(a => a.Name.Equals(selectedCluster, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Sectors.First(a => a.Name.Equals(selectedSector, StringComparison.OrdinalIgnoreCase));

            RegionForm.Sector = sector;
            RegionForm.Show();
        }

        private void BtnRemoveRegion_Click(object sender, EventArgs e)
        {
            if (RegionsListBox.SelectedItem is not Region selectedRegion)
            {
                return;
            }

            string selectedCluster = ClustersListBox.SelectedItem as string;
            string selectedSector = SectorsListBox.SelectedItem as string;
            Cluster cluster = AllClusters.Values.First(a => a.Name.Equals(selectedCluster, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Sectors.First(a => a.Name.Equals(selectedSector, StringComparison.OrdinalIgnoreCase));

            // Remove region from sector
            _ = sector.Regions.Remove(selectedRegion);

            int index = RegionsListBox.Items.IndexOf(RegionsListBox.SelectedItem);
            RegionsListBox.Items.Remove(RegionsListBox.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            RegionsListBox.SelectedItem = index >= 0 && RegionsListBox.Items.Count > 0 ? RegionsListBox.Items[index] : null;
        }

        private void RegionsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (RegionsListBox.SelectedItem is not Region selectedRegion)
            {
                return;
            }

            string selectedCluster = ClustersListBox.SelectedItem as string;
            string selectedSector = SectorsListBox.SelectedItem as string;
            Cluster cluster = AllClusters.Values.First(a => a.Name.Equals(selectedCluster, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Sectors.First(a => a.Name.Equals(selectedSector, StringComparison.OrdinalIgnoreCase));

            RegionForm.Sector = sector;
            RegionForm.CustomRegion = selectedRegion;
            RegionForm.Show();
        }
        #endregion
    }
}
