using System.ComponentModel;
using System.Diagnostics;
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
        private ZoneForm _zoneForm;
        private GateForm _gateForm;
        private VersionUpdateForm _versionUpdateForm;

        public readonly Dictionary<(int, int), Cluster> CustomClusters = [];

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static MainForm Instance { get; private set; }

        public GuideForm GuideForm
        {
            get
            {
                if (_guideForm != null && !_guideForm.IsDisposed)
                    return _guideForm;
                return _guideForm = new GuideForm();
            }
        }

        public SectorMapForm SectorMapForm
        {
            get
            {
                if (_sectorMapForm != null && !_sectorMapForm.IsDisposed)
                    return _sectorMapForm;
                return _sectorMapForm = new SectorMapForm();
            }
        }

        public ClusterForm ClusterForm
        {
            get
            {
                if (_clusterForm != null && !_clusterForm.IsDisposed)
                    return _clusterForm;
                return _clusterForm = new ClusterForm();
            }
        }

        public SectorForm SectorForm
        {
            get
            {
                if (_sectorForm != null && !_sectorForm.IsDisposed)
                    return _sectorForm;
                return _sectorForm = new SectorForm();
            }
        }

        public ZoneForm ZoneForm
        {
            get
            {
                if (_zoneForm != null && !_zoneForm.IsDisposed)
                    return _zoneForm;
                return _zoneForm = new ZoneForm();
            }
        }

        public GateForm GateForm
        {
            get
            {
                if (_gateForm != null && !_gateForm.IsDisposed)
                    return _gateForm;
                return _gateForm = new GateForm();
            }
        }

        public VersionUpdateForm VersionUpdateForm
        {
            get
            {
                if (_versionUpdateForm != null && !_versionUpdateForm.IsDisposed)
                    return _versionUpdateForm;
                return _versionUpdateForm = new VersionUpdateForm();
            }
        }

        public MainForm()
        {
            InitializeComponent();

            if (Instance != null)
                throw new Exception("No more than one instance of \"MainForm\" can be active.");
            Instance = this;
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
            var modInfo = MultiInputDialog.Show("Mod information",
                lblModName,
                lblModPrefix);
            if (modInfo == null || modInfo.Count == 0) return;

            var modName = modInfo[lblModName];
            var modPrefix = modInfo[lblModPrefix];

            if (string.IsNullOrWhiteSpace(modInfo[lblModName]))
            {
                MessageBox.Show($"Please enter a valid non empty non whitespace mod prefix.");
                return;
            }
            if (string.IsNullOrWhiteSpace(modInfo[lblModPrefix]))
            {
                MessageBox.Show($"Please enter a valid non empty non whitespace mod folder name.");
                return;
            }

            var clusters = CustomClusters.Values.ToList();

            // Generate each xml file
            var folder = Path.Combine(Application.StartupPath, "GeneratedXml");
            try
            {
                // Clear up any previous xml
                if (Directory.Exists(folder))
                    Directory.Delete(folder, true);

                // Generate all xml files
                MacrosGeneration.Generate(folder, modName, modPrefix, clusters);
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
                MessageBox.Show("Something went wrong during xml generation, please create an issue on github with the stacktrace: " + ex.ToString(),
                    "Error in XML Generation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show succes message
            MessageBox.Show("XML Files were succesfully generated in the xml folder.");
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            var versionChecker = new VersionChecker();

            // Set form title
            Text += $" [APP v{versionChecker.CurrentVersion} | X4 v{versionChecker.TargetGameVersion}]";

            // Check for update
            var result = await versionChecker.CheckForUpdatesAsync();
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
            LblDetails.Text = string.Empty;
            CustomClusters.Clear();
            ClustersListBox.Items.Clear();
            SectorsListBox.Items.Clear();
            ZonesListBox.Items.Clear();
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
                    var jsonContent = ConfigSerializer.Serialize([.. CustomClusters.Values]);
                    File.WriteAllText(filePath, jsonContent);
                    MessageBox.Show($"Configuration exported succesfully.", "Success");
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid JSON content in file, please try another file.",
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
                    var jsonContent = File.ReadAllText(filePath);
                    var clusters = ConfigSerializer.Deserialize(jsonContent);
                    if (clusters != null)
                    {
                        // Reset configuration
                        BtnReset.PerformClick();

                        // Import new configuration
                        foreach (var cluster in clusters)
                        {
                            // Set custom clusters
                            CustomClusters.Add((cluster.Position.X, cluster.Position.Y), cluster);

                            // Setup listboxes
                            ClustersListBox.Items.Add(cluster.Name);
                        }

                        // Select first one so sector and zones populate automatically
                        ClustersListBox.SelectedItem = clusters.FirstOrDefault()?.Name ?? null;

                        MessageBox.Show($"Configuration imported succesfully.", "Success");
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid JSON content in file, please try another file.",
                        "Invalid JSON Content", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            var folder = EnsureDirectoryExists(Path.Combine(Application.StartupPath, "GeneratedXml"));
            Process.Start("explorer.exe", folder);
        }

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
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
            var selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName)) return;

            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));

            foreach (var sector in cluster.Value.Sectors)
            {
                foreach (var zone in sector.Zones)
                {
                    // Remove gate connections
                    foreach (var selectedGate in zone.Gates)
                    {
                        var sourceSector = CustomClusters.Values
                            .SelectMany(a => a.Sectors)
                            .First(a => a.Name.Equals(selectedGate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                        var sourceZone = sourceSector.Zones
                            .First(a => a.Gates
                                .Any(a => a.DestinationSectorName
                                    .Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
                        var sourceGate = sourceZone.Gates.First(a => a.DestinationSectorName.Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
                        sourceZone.Gates.Remove(sourceGate);
                    }
                }
            }

            CustomClusters.Remove(cluster.Key);
            ClustersListBox.Items.Remove(ClustersListBox.SelectedItem);
            ClustersListBox.SelectedItem = null;
            ZonesListBox.Items.Clear();
            SectorsListBox.Items.Clear();
            GatesListBox.Items.Clear();
        }

        private void ClustersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset current sectors to empty
            SectorsListBox.Items.Clear();
            SectorsListBox.SelectedItem = null;

            ZonesListBox.Items.Clear();
            ZonesListBox.SelectedItem = null;

            GatesListBox.Items.Clear();
            GatesListBox.SelectedItem = null;

            var selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName)) return;

            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));

            // Show new sectors and zones
            bool selected = false;
            foreach (var item in cluster.Value.Sectors)
            {
                SectorsListBox.Items.Add(item.Name);
                if (!selected)
                {
                    SectorsListBox.SelectedItem = item.Name;
                    selected = true;
                }
            }
        }

        private void ClustersListBox_DoubleClick(object sender, EventArgs e)
        {
            var selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName)) return;

            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));

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
            var selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                MessageBox.Show("Please select a cluster first.");
                return;
            }

            SectorForm.Sector = null;
            SectorForm.BtnCreate.Text = "Create";
            SectorForm.TxtName.Text = string.Empty;
            SectorForm.Show();
        }

        private void BtnRemoveSector_Click(object sender, EventArgs e)
        {
            var selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSectorName)) return;

            // Remove sector from cluster
            var selectedClusterName = ClustersListBox.SelectedItem as string;
            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            var sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            foreach (var zone in sector.Zones)
            {
                // Remove gate connections
                foreach (var selectedGate in zone.Gates)
                {
                    var sourceSector = CustomClusters.Values
                        .SelectMany(a => a.Sectors)
                        .First(a => a.Name.Equals(selectedGate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                    var sourceZone = sourceSector.Zones
                        .First(a => a.Gates
                            .Any(a => a.DestinationSectorName
                                .Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
                    var sourceGate = sourceZone.Gates.First(a => a.DestinationSectorName.Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
                    sourceZone.Gates.Remove(sourceGate);
                }
            }

            cluster.Value.Sectors.Remove(sector);

            ZonesListBox.Items.Clear();
            GatesListBox.Items.Clear();
            SectorsListBox.Items.Remove(SectorsListBox.SelectedItem);
            SectorsListBox.SelectedItem = null;
        }

        private void SectorsListBox_DoubleClick(object sender, EventArgs e)
        {
            var selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSectorName)) return;

            var selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName)) return;

            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            var sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            SectorForm.Sector = sector;
            SectorForm.BtnCreate.Text = "Update";
            SectorForm.TxtName.Text = selectedSectorName;
            SectorForm.Show();
        }

        private void SectorsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ZonesListBox.Items.Clear();
            ZonesListBox.SelectedItem = null;
            GatesListBox.Items.Clear();
            GatesListBox.SelectedItem = null;

            var selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName)) return;

            var selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSectorName)) return;

            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            var sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            bool selected = false;
            foreach (var zone in sector.Zones)
            {
                ZonesListBox.Items.Add(zone.Name);
                if (!selected)
                {
                    ZonesListBox.SelectedItem = zone.Name;
                    selected = true;
                }
            }
        }
        #endregion

        #region Zones
        private void BtnNewZone_Click(object sender, EventArgs e)
        {
            var selectedClusterName = ClustersListBox.SelectedItem as string;
            var selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName) ||
                string.IsNullOrWhiteSpace(selectedSectorName))
            {
                MessageBox.Show("Please select a sector first.");
                return;
            }

            ZoneForm.Zone = null;
            ZoneForm.BtnCreate.Text = "Create";
            ZoneForm.TxtName.Text = string.Empty;
            ZoneForm.Show();
        }

        private void BtnRemoveZone_Click(object sender, EventArgs e)
        {
            var selectedZoneName = ZonesListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedZoneName)) return;

            // Remove zone from sector
            var selectedClusterName = ClustersListBox.SelectedItem as string;
            var selectedSectorName = SectorsListBox.SelectedItem as string;
            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            var sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));
            var zone = sector.Zones.First(a => a.Name.Equals(selectedZoneName, StringComparison.OrdinalIgnoreCase));

            // Remove gate connections
            foreach (var selectedGate in zone.Gates)
            {
                var sourceSector = CustomClusters.Values
                    .SelectMany(a => a.Sectors)
                    .First(a => a.Name.Equals(selectedGate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                var sourceZone = sourceSector.Zones
                    .First(a => a.Gates
                        .Any(a => a.DestinationSectorName
                            .Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
                var sourceGate = sourceZone.Gates.First(a => a.DestinationSectorName.Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
                sourceZone.Gates.Remove(sourceGate);
            }

            sector.Zones.Remove(zone);

            ZonesListBox.Items.Remove(selectedZoneName);
            ZonesListBox.SelectedItem = null;
            GatesListBox.Items.Clear();
        }

        private void ZonesListBox_DoubleClick(object sender, EventArgs e)
        {
            var selectedZoneName = ZonesListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedZoneName)) return;

            var selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSectorName)) return;

            var selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName)) return;

            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            var sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));
            var zone = sector.Zones.First(a => a.Name.Equals(selectedZoneName, StringComparison.OrdinalIgnoreCase));

            ZoneForm.Zone = zone;
            ZoneForm.BtnCreate.Text = "Update";
            ZoneForm.TxtName.Text = string.Empty;
            ZoneForm.Show();
        }

        private void ZonesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Adjust gates
            GatesListBox.Items.Clear();
            GatesListBox.SelectedItem = null;

            var selectedZoneName = ZonesListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedZoneName)) return;

            var selectedSectorName = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSectorName)) return;

            var selectedClusterName = ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName)) return;

            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            var sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));
            var zone = sector.Zones.First(a => a.Name.Equals(selectedZoneName, StringComparison.OrdinalIgnoreCase));

            // Add destination gates
            foreach (var gate in zone.Gates)
            {
                var destSector = CustomClusters.Values
                .SelectMany(a => a.Sectors)
                .First(a => a.Name.Equals(gate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                var destZone = destSector.Zones
                    .First(a => a.Gates
                        .Any(a => a.DestinationSectorName
                            .Equals(gate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
                var destGate = destZone.Gates.First(a => a.DestinationSectorName.Equals(gate.ParentSectorName, StringComparison.OrdinalIgnoreCase));

                GatesListBox.Items.Add(destGate);
            }
        }
        #endregion

        #region Connections
        private void BtnNewGate_Click(object sender, EventArgs e)
        {
            var selectedZoneName = ZonesListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedZoneName))
            {
                MessageBox.Show("Please select a zone first.", "Zone selection required");
                return;
            }

            var selectedClusterName = ClustersListBox.SelectedItem as string;
            var selectedSectorName = SectorsListBox.SelectedItem as string;
            var cluster = CustomClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));
            var sector = cluster.Value.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));
            var zone = sector.Zones.First(a => a.Name.Equals(selectedZoneName, StringComparison.OrdinalIgnoreCase));

            GateForm.Reset();
            GateForm.SourceCluster = cluster.Value;
            GateForm.SourceSector = sector;
            GateForm.SourceZone = zone;
            GateForm.Show();
        }

        private void BtnRemoveGate_Click(object sender, EventArgs e)
        {
            if (GatesListBox.SelectedItem is not Gate selectedGate)
            {
                MessageBox.Show("Please select a gate first.", "Gate selection required");
                return;
            }

            var selectedSectorName = SectorsListBox.SelectedItem as string;

            // Delete target connection
            var targetSector = CustomClusters.Values
                .SelectMany(a => a.Sectors)
                .First(a => a.Name.Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
            var targetZone = targetSector.Zones
                .First(a => a.Gates
                    .Any(a => a.DestinationSectorName
                        .Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase)));
            targetZone.Gates.Remove(selectedGate);

            // Delete source connection
            var sourceSector = CustomClusters.Values
                .SelectMany(a => a.Sectors)
                .First(a => a.Name.Equals(selectedGate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
            var sourceZone = sourceSector.Zones
                .First(a => a.Gates
                    .Any(a => a.DestinationSectorName
                        .Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase)));
            var sourceGate = sourceZone.Gates.First(a => a.DestinationSectorName.Equals(selectedGate.ParentSectorName, StringComparison.OrdinalIgnoreCase));
            sourceZone.Gates.Remove(sourceGate);

            // Remove from listbox
            GatesListBox.Items.Remove(selectedGate);
            GatesListBox.SelectedItem = null;
        }
        #endregion
    }
}
