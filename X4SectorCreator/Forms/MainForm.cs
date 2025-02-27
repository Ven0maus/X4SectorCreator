using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

            // Initializes the full X4 map from JSON data
            const string filePath = "Mappings/sector_mappings.json";

            string json = File.ReadAllText(filePath);
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
            SectorMapForm.ControlPanel.Size = new Size(204, 106);
            SectorMapForm.BtnSelectLocation.Hide();
            SectorMapForm.Reset();
            SectorMapForm.Show();
        }

        private void BtnGenerateDiffs_Click(object sender, EventArgs e)
        {
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

            // Generate each xml file
            string folder = Path.Combine(Application.StartupPath, "GeneratedXml");
            try
            {
                // Clear up any previous xml
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }

                // Generate all xml files
                MacrosGeneration.Generate(folder, modName, modPrefix);
                MapDefaultsGeneration.Generate(folder, modPrefix, clusters);
                GalaxyGeneration.Generate(folder, modPrefix, clusters);
                ClusterGeneration.Generate(folder, modPrefix, clusters);
                SectorGeneration.Generate(folder, modPrefix, clusters);
                ZoneGeneration.Generate(folder, modPrefix, clusters);
            }
            catch (Exception ex)
            {
                // Clear up corrupted xml
                Directory.Delete(folder, true);
                _ = MessageBox.Show("Something went wrong during xml generation, please create an issue on github with the stacktrace: " + ex.ToString(),
                    "Error in XML Generation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

            // Show succes message
            _ = MessageBox.Show("XML Files were succesfully generated in the xml folder.");
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            VersionChecker versionChecker = new();

            // Set form title
            Text += $" [APP v{versionChecker.CurrentVersion} | X4 v{versionChecker.TargetGameVersion}]";

            // Check for update
            (bool NewVersionAvailable, VersionInfo VersionInfo) result = await versionChecker.CheckForUpdatesAsync();
            if (result.NewVersionAvailable)
            {
                VersionUpdateForm.txtCurrentVersion.Text = $"v{versionChecker.CurrentVersion}";
                VersionUpdateForm.txtCurrentX4Version.Text = $"v{versionChecker.TargetGameVersion}";
                VersionUpdateForm.txtUpdateVersion.Text = $"v{result.VersionInfo.AppVersion}";
                VersionUpdateForm.txtUpdateX4Version.Text = $"v{result.VersionInfo.X4Version}";
                VersionUpdateForm.Show();
            }
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

                            // Setup listboxes
                            if (!cluster.IsBaseGame)
                                _ = ClustersListBox.Items.Add(cluster.Name);
                        }

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

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            string folder = EnsureDirectoryExists(Path.Combine(Application.StartupPath, "GeneratedXml"));
            _ = Process.Start("explorer.exe", folder);
        }

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }
        #endregion

        #region Clusters
        private void BtnNewCluster_Click(object sender, EventArgs e)
        {
            ClusterForm.Cluster = null;
            ClusterForm.BtnCreate.Text = "Create";
            ClusterForm.TxtName.Text = string.Empty;
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
            bool selected = false;
            foreach (Sector item in cluster.Value.Sectors)
            {
                _ = SectorsListBox.Items.Add(item.Name);
                if (!selected)
                {
                    SectorsListBox.SelectedItem = item.Name;
                    selected = true;
                }
            }
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
            SectorForm.TxtName.Text = string.Empty;
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

            GatesListBox.Items.Clear();
            SectorsListBox.Items.Remove(SectorsListBox.SelectedItem);
            SectorsListBox.SelectedItem = null;
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
            SectorForm.TxtName.Text = selectedSectorName;
            SectorForm.Show();
        }

        private void SectorsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GatesListBox.Items.Clear();
            GatesListBox.SelectedItem = null;

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

            // Remove from listbox
            GatesListBox.Items.Remove(selectedGate);
            GatesListBox.SelectedItem = null;
        }
        #endregion

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
    }
}
