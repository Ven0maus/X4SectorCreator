using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using X4SectorCreator.Configuration;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;

namespace X4SectorCreator
{
    public partial class MainForm : Form
    {
        private GuideForm _guideForm;
        private SectorMapForm _sectorMapForm;
        private ClusterForm _clusterForm;
        private SectorForm _sectorForm;
        private GateForm _gateForm;
        private VersionUpdateForm _versionUpdateForm;

        private string _currentX4Version;

        private readonly string _sectorMappingFilePath = Path.Combine(Application.StartupPath, "Mappings/sector_mappings.json");

        public readonly Dictionary<(int, int), Cluster> AllClusters;
        public readonly Dictionary<string, Color> FactionColorMapping;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static MainForm Instance { get; private set; }

        public GuideForm GuideForm => _guideForm != null && !_guideForm.IsDisposed ? _guideForm : (_guideForm = new GuideForm());

        public SectorMapForm SectorMapForm => _sectorMapForm != null && !_sectorMapForm.IsDisposed ? _sectorMapForm : (_sectorMapForm = new SectorMapForm());

        public ClusterForm ClusterForm => _clusterForm != null && !_clusterForm.IsDisposed ? _clusterForm : (_clusterForm = new ClusterForm());

        public SectorForm SectorForm => _sectorForm != null && !_sectorForm.IsDisposed ? _sectorForm : (_sectorForm = new SectorForm());

        public GateForm GateForm => _gateForm != null && !_gateForm.IsDisposed ? _gateForm : (_gateForm = new GateForm());

        public VersionUpdateForm VersionUpdateForm => _versionUpdateForm != null && !_versionUpdateForm.IsDisposed
                    ? _versionUpdateForm
                    : (_versionUpdateForm = new VersionUpdateForm());

        public MainForm()
        {
            InitializeComponent();

            if (Instance != null)
            {
                throw new Exception("No more than one instance of \"MainForm\" can be active.");
            }

            Instance = this;

            string json = File.ReadAllText(_sectorMappingFilePath);
            ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(json);

            // Create lookups
            AllClusters = clusterCollection.Clusters.ToDictionary(a => (a.Position.X, a.Position.Y));
            // Init all collections
            foreach (var cluster in AllClusters)
            {
                foreach (var sector in cluster.Value.Sectors)
                {
                    // Init regular sectors
                    if (cluster.Value.IsBaseGame && string.IsNullOrWhiteSpace(sector.BaseGameMapping))
                        sector.BaseGameMapping = "sector001";

                    sector.Zones ??= [];
                    foreach (var zone in sector.Zones)
                    {
                        zone.Gates ??= [];
                    }
                }
            }
            FactionColorMapping = clusterCollection.FactionColors.ToDictionary(a => a.Key, a => HexToColor(a.Value), StringComparer.OrdinalIgnoreCase);
        }

        private void BtnSectorCreationGuide_Click(object sender, EventArgs e)
        {
            GuideForm.Show();
        }

        private void BtnShowSectorMap_Click(object sender, EventArgs e)
        {
            SectorMapForm.GateSectorSelection = false;
            SectorMapForm.BtnSelectLocation.Enabled = false;
            SectorMapForm.ControlPanel.Size = new Size(176, 215);
            SectorMapForm.BtnSelectLocation.Hide();
            SectorMapForm.Reset();
            SectorMapForm.Show();
        }

        private void BtnGenerateDiffs_Click(object sender, EventArgs e)
        {
            // Validate if all clusters have atleast one sector
            var invalidClusters = AllClusters.Values
                .Where(a => a.Sectors == null || a.Sectors.Count == 0)
                .ToArray();
            if (invalidClusters.Length != 0)
            {
                _ = MessageBox.Show($"Following clusters have no sectors, please fix these first:\n- " +
                    string.Join("\n- ", invalidClusters.Select(a => a.Name)));
                return;
            }

            const string lblModName = "Please enter the full name of your mod's folder:";
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

            if (string.IsNullOrWhiteSpace(modInfo[lblModName]))
            {
                _ = MessageBox.Show($"Please enter a valid non empty non whitespace mod prefix.");
                return;
            }
            if (string.IsNullOrWhiteSpace(modInfo[lblModPrefix]))
            {
                _ = MessageBox.Show($"Please enter a valid non empty non whitespace mod folder name.");
                return;
            }

            List<Cluster> clusters = [.. AllClusters.Values];

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
                MacrosGeneration.Generate(modFolder, modName, modPrefix);
                MapDefaultsGeneration.Generate(modFolder, modPrefix, clusters);
                GalaxyGeneration.Generate(modFolder, modPrefix, clusters);
                ClusterGeneration.Generate(modFolder, modPrefix, clusters);
                SectorGeneration.Generate(modFolder, modPrefix, clusters);
                ZoneGeneration.Generate(modFolder, modPrefix, clusters);
                ContentGeneration.Generate(modFolder, modName, _currentX4Version.Replace(".", string.Empty) + "0", clusters);
            }
            catch (Exception ex)
            {
                // Clear up corrupted xml
                Directory.Delete(mainFolder, true);
                _ = MessageBox.Show("Something went wrong during xml generation: " + ex.Message,
                    "Error in XML Generation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Show succes message
            _ = MessageBox.Show("XML Files were succesfully generated in the xml folder.");
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
                    var newSectorMappingJson = await versionChecker.GetUpdatedSectorMappingAsync();
                    var oldSectorMappingJson = File.ReadAllText(_sectorMappingFilePath);
                    if (!oldSectorMappingJson.Equals(newSectorMappingJson))
                    {
                        try
                        {
                            // Update mapping file
                            File.WriteAllText(_sectorMappingFilePath, newSectorMappingJson);
                            ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(newSectorMappingJson);

                            // Replace clusters
                            var newClusters = clusterCollection.Clusters.ToDictionary(a => (a.Position.X, a.Position.Y));
                            if (newClusters.Count > 0)
                            {
                                AllClusters.Clear();
                                foreach (var cluster in newClusters)
                                    AllClusters[cluster.Key] = cluster.Value;
                            }

                            // Update X4 version file
                            versionChecker.UpdateX4Version(result.VersionInfo);

                            // Update title text with new version
                            Text += $" [APP v{versionChecker.CurrentVersion} | X4 v{versionChecker.TargetGameVersion}]";
                            _currentX4Version = versionChecker.TargetGameVersion;

                            MessageBox.Show($"Your cluster mapping has been automatically updated with the latest X4 version ({result.VersionInfo.X4Version}).");
                        }
                        catch (Exception)
                        {
                            // Don't do anything
                            MessageBox.Show($"A new cluster mapping is available for X4 version ({result.VersionInfo.X4Version}) but was unable to download it, please update manually.");
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
            var sb = new StringBuilder();
            sb.AppendLine($"[Cluster]: {cluster.Name}");
            sb.AppendLine("Location: " + (cluster.Position.X, cluster.Position.Y));
            sb.AppendLine();
            if (sector != null)
            {
                sb.AppendLine($"[Sector]: {sector.Name}");
                sb.AppendLine($"Sunlight: {(int)(sector.Sunlight * 100f)}");
                sb.AppendLine($"Economy: {(int)(sector.Economy * 100f)}");
                sb.AppendLine($"Security: {(int)(sector.Security * 100f)}");
                sb.AppendLine($"Tags: {sector.Tags}");
                sb.AppendLine($"Allow Anomalies: {sector.AllowRandomAnomalies}");
                sb.AppendLine($"FactionLogic Disabled: {sector.DisableFactionLogic}");
            }
            LblDetails.Text = sb.ToString();
        }

        #region Configuration
        private void BtnReset_Click(object sender, EventArgs e)
        {
            // Remove custom clusters
            var toBeRemoved = AllClusters.Where(a => !a.Value.IsBaseGame).ToArray();
            foreach (var cluster in toBeRemoved)
                AllClusters.Remove(cluster.Key);

            LblDetails.Text = string.Empty;
            ClustersListBox.Items.Clear();
            SectorsListBox.Items.Clear();
            GatesListBox.Items.Clear();
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
                    var allModifiedClusters = AllClusters.Values
                        .Where(a => !a.IsBaseGame)
                        .Concat(AllClusters.Values
                            .Where(a => a.IsBaseGame && a.Sectors
                                .SelectMany(a => a.Zones)
                                .Any()))
                        .ToList();

                    string jsonContent = ConfigSerializer.Serialize(allModifiedClusters);
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
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    List<Cluster> clusters = ConfigSerializer.Deserialize(jsonContent);
                    if (clusters != null)
                    {
                        // Reset configuration
                        BtnReset.PerformClick();

                        // Import new configuration
                        foreach (Cluster cluster in clusters)
                        {
                            // Contains also base game clusters that have zones for exported gate connections
                            // Set custom clusters
                            AllClusters[(cluster.Position.X, cluster.Position.Y)] = cluster;

                            // Apply support additions for new versions
                            Import_Support_NewVersions(cluster);

                            // Setup listboxes
                            if (!cluster.IsBaseGame)
                                _ = ClustersListBox.Items.Add(cluster.Name);
                        }

                        // No longer needed
                        _clusterDlcLookup = null;

                        // Select first one so sector and zones populate automatically
                        ClustersListBox.SelectedItem = clusters.FirstOrDefault(a => !a.IsBaseGame)?.Name ?? null;

                        _ = MessageBox.Show($"Configuration imported succesfully.", "Success");
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Invalid JSON content in file, please try another file: {ex.Message}",
                        "Invalid JSON Content", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Dictionary<(int, int), Cluster> _clusterDlcLookup;
        private void Import_Support_NewVersions(Cluster cluster)
        {
            if (string.IsNullOrWhiteSpace(cluster.BackgroundVisualMapping))
                cluster.BackgroundVisualMapping = "cluster_01";

            // Re-check DLCs
            if (cluster.Dlc == null)
            {
                if (_clusterDlcLookup == null)
                {
                    // Create new lookup table
                    string json = File.ReadAllText(_sectorMappingFilePath);
                    ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(json);
                    _clusterDlcLookup = clusterCollection.Clusters.ToDictionary(a => (a.Position.X, a.Position.Y));
                }

                if (_clusterDlcLookup.TryGetValue((cluster.Position.X, cluster.Position.Y), out var lookupCluster))
                    cluster.Dlc = lookupCluster.Dlc;
            }
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            var directoryPath = Path.Combine(Application.StartupPath, "GeneratedXml");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
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
                            .First(a => a.Gates
                                .Any(a => a.DestinationSectorName
                                    .Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
                        Gate sourceGate = sourceZone.Gates.First(a => a.DestinationSectorName.Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
                        _ = sourceZone.Gates.Remove(sourceGate);
                    }
                }
            }

            _ = AllClusters.Remove(cluster.Key);

            // Re-align ids
            int count = 0;
            foreach (var clust in AllClusters.Values.Where(a => !a.IsBaseGame).OrderBy(a => a.Id))
            {
                clust.Id = ++count;
            }

            ClustersListBox.Items.Remove(ClustersListBox.SelectedItem);
            ClustersListBox.SelectedItem = null;
            SectorsListBox.Items.Clear();
            GatesListBox.Items.Clear();
        }

        private void ClustersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset current sectors to empty
            SectorsListBox.Items.Clear();
            SectorsListBox.SelectedItem = null;

            GatesListBox.Items.Clear();
            GatesListBox.SelectedItem = null;

            string selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                return;
            }

            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));

            // Show new sectors and zones
            Sector selectedSector = null;
            foreach (Sector sector in cluster.Value.Sectors)
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
            ClusterForm.cmbBackgroundVisual.SelectedItem = ClusterForm.FindBackgroundVisualMappingByCode(cluster.Value.BackgroundVisualMapping);
            ClusterForm.TxtLocation.Text = cluster.Key.ToString();
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
                            .Any(a => a.DestinationSectorName
                                .Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
                    Gate sourceGate = sourceZone.Gates.First(a => a.DestinationSectorName.Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
                    _ = sourceZone.Gates.Remove(sourceGate);
                }
            }

            _ = cluster.Value.Sectors.Remove(sector);

            // Re-align ids before re-calculate
            int count = 0;
            foreach (var sect in cluster.Value.Sectors.OrderBy(a => a.Id))
            {
                sect.Id = ++count;
            }

            RecalculateSectorOffsets(cluster.Value);

            GatesListBox.Items.Clear();
            SectorsListBox.Items.Remove(SectorsListBox.SelectedItem);
            SectorsListBox.SelectedItem = null;

            // Set details
            SetDetailsText(cluster.Value, null);
        }

        private static void RecalculateSectorOffsets(Cluster cluster)
        {
            var sectors = new List<Sector>();
            foreach (var sector in cluster.Sectors.OrderBy(a => a.Id))
            {
                int offSetX, offsetY;
                if (sectors.Count == 0)
                {
                    offSetX = 0;
                    offsetY = 0;
                }
                if (sectors.Count == 1)
                {
                    // Down
                    offSetX = 0;
                    offsetY = -1000000;
                }
                else
                {
                    // Right
                    offSetX = 1000000;
                    offsetY = -500000;
                }

                sector.Offset = new Point(offSetX, offsetY);
                sectors.Add(sector);
            }
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

            foreach (Gate gate in gates)
            {
                _ = GatesListBox.Items.Add(gate);
            }

            var cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            var sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            // Set details
            SetDetailsText(cluster.Value, sector);
        }
        #endregion

        #region Connections
        private void BtnNewGate_Click(object sender, EventArgs e)
        {
            string selectedClusterName = ClustersListBox.SelectedItem as string;
            string selectedSectorName = SectorsListBox.SelectedItem as string;
            KeyValuePair<(int, int), Cluster> cluster = AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            GateForm.Reset();
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
                .First(a => a.Gates
                    .Any(a => a.DestinationSectorName
                        .Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase)));
            _ = targetZone.Gates.Remove(selectedGate);

            // Check to remove zone if empty
            if (targetZone.Gates.Count == 0)
                targetSector.Zones.Remove(targetZone);

            // Re-order zone ids if needed
            int count = 0;
            foreach (var tZone in targetSector.Zones.OrderBy(a => a.Id))
            {
                tZone.Id = ++count;
            }

            // Delete source connection
            Sector sourceSector = AllClusters.Values
                .SelectMany(a => a.Sectors)
                .First(a => a.Name.Equals(selectedGate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
            Zone sourceZone = sourceSector.Zones
                .First(a => a.Gates
                    .Any(a => a.DestinationSectorName
                        .Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
            Gate sourceGate = sourceZone.Gates.First(a => a.DestinationSectorName.Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
            _ = sourceZone.Gates.Remove(sourceGate);

            // Check to remove zone if empty
            if (sourceZone.Gates.Count == 0)
                sourceSector.Zones.Remove(sourceZone);

            // Re-order zone ids if needed
            count = 0;
            foreach (var sZone in sourceSector.Zones.OrderBy(a => a.Id))
            {
                sZone.Id = ++count;
            }

            // Remove from listbox
            GatesListBox.Items.Remove(selectedGate);
            GatesListBox.SelectedItem = null;
        }

        private void GatesListBox_DoubleClick(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
