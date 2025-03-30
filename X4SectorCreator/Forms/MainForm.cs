using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using X4SectorCreator.Configuration;
using X4SectorCreator.Forms;
using X4SectorCreator.Helpers;
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
        public readonly LazyEvaluated<GalaxySettingsForm> GalaxySettingsForm = new(() => new GalaxySettingsForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<RegionForm> RegionForm = new(() => new RegionForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<SectorMapForm> SectorMapForm = new(() => new SectorMapForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<ClusterForm> ClusterForm = new(() => new ClusterForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<SectorForm> SectorForm = new(() => new SectorForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<GateForm> GateForm = new(() => new GateForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<VersionUpdateForm> VersionUpdateForm = new(() => new VersionUpdateForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<StationForm> StationForm = new(() => new StationForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<JobsForm> JobsForm = new(() => new JobsForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<FactoriesForm> FactoriesForm = new(() => new FactoriesForm(), a => !a.IsDisposed);
        /* END OF FORMS */

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<(int, int), Cluster> AllClusters { get; private set; }

        public readonly Dictionary<string, string> BackgroundVisualMapping;
        public readonly Dictionary<string, string> DlcMappings;
        public readonly Dictionary<string, Color> FactionColorMapping;

        private ClusterOption _selectedClusterOption = ClusterOption.Custom;
        private string _currentX4Version, _currentModTargetVersion;

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
                .Where(a => !string.IsNullOrWhiteSpace(a.Value.BackgroundVisualMapping))
                .ToDictionary(a => a.Value.Name, a => a.Value.BackgroundVisualMapping, StringComparer.OrdinalIgnoreCase);

            // Set dlc mappings
            string json = File.ReadAllText(Constants.DataPaths.DlcMappingFilePath);
            DlcMappings = JsonSerializer.Deserialize<List<DlcMapping>>(json, ConfigSerializer.SerializerOptions)
                .ToDictionary(a => a.Dlc, a => a.Prefix);

            // Set faction color mapping
            FactionColorMapping = clusterCollection.FactionColors.ToDictionary(a => a.Key, a => a.Value.HexToColor(), StringComparer.OrdinalIgnoreCase);

            // Set the default value to be custom always
            UpdateClusterOptions();
        }

        #region Initialization
        public ClusterCollection InitAllClusters(bool replaceAllClusters = true)
        {
            string json = File.ReadAllText(Constants.DataPaths.SectorMappingFilePath);
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
                    foreach (Zone zone in sector.Zones)
                    {
                        zone.Gates ??= [];
                    }

                    // Auto-determine offset for each sector
                    Forms.SectorForm.DetermineSectorOffset(cluster.Value, sector);
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
            ListStations.Items.Clear();
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
            _currentX4Version = versionChecker.TargetGameVersion;
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
                    VersionUpdateForm.Value.txtCurrentVersion.Text = $"v{versionChecker.CurrentVersion}";
                    VersionUpdateForm.Value.txtCurrentX4Version.Text = $"v{versionChecker.TargetGameVersion}";
                    VersionUpdateForm.Value.txtUpdateVersion.Text = $"v{result.VersionInfo.AppVersion}";
                    VersionUpdateForm.Value.txtUpdateX4Version.Text = $"v{result.VersionInfo.X4Version}";
                    VersionUpdateForm.Value.Show();
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
                _ = MessageBox.Show($"Following clusters have sectors that have overlapped placements, please fix these first:\n- " +
                    string.Join("\n- ", invalidClusters.Select(a => a.Name)));
                return;
            }

            const string lblModName = "Please enter the name you'd like to use for your mod folder:";
            const string lblModPrefix = "Please enter the prefix you'd like to use for your mod:";
            Dictionary<string, string> modInfo = MultiInputDialog.Show("Mod information",
                (lblModName, null, null),
                (lblModPrefix, null, null)
            );
            if (modInfo == null || modInfo.Count == 0)
            {
                return;
            }

            string modName = modInfo[lblModName];
            string modPrefix = modInfo[lblModPrefix];

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

            List<Cluster> clusters = [.. AllClusters.Values];

            // Collects all changes done to base game content
            ClusterCollection nonModifiedBaseGameData = InitAllClusters(false);
            VanillaChanges vanillaChanges = CollectVanillaChanges(nonModifiedBaseGameData);

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

                // Generate all xml files
                MacrosGeneration.Generate(modFolder, modName, modPrefix, clusters);
                MapDefaultsGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges);
                GalaxyGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges, nonModifiedBaseGameData);
                ClusterGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges);
                SectorGeneration.Generate(modFolder, modPrefix, clusters, nonModifiedBaseGameData, vanillaChanges);
                ZoneGeneration.Generate(modFolder, modPrefix, clusters, nonModifiedBaseGameData, vanillaChanges);
                ContentGeneration.Generate(modFolder, modName, _currentModTargetVersion.Replace(".", string.Empty), clusters, vanillaChanges);
                RegionDefinitionGeneration.Generate(modFolder, modPrefix, clusters);
                GameStartsGeneration.Generate(modFolder, modPrefix, clusters, vanillaChanges);
                DlcDisableGeneration.Generate(modFolder);
                GodGeneration.Generate(modFolder, modPrefix, clusters);
                JobsGeneration.Generate(modFolder, modPrefix);
                BasketsGeneration.Generate(modFolder, modPrefix);
            }
            catch (Exception ex)
            {
                // Clear up corrupted xml
                Directory.Delete(mainFolder, true);
                #if DEBUG
                throw;
                #else
                _ = MessageBox.Show("Something went wrong during xml generation: \"" + ex.Message + "\".\nPlease create a bug report. (Be sure to provide the export xml or exact reproduction steps)",
                    "Error in XML Generation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
                #endif
            }

            // Show succes message
            _ = MessageBox.Show("XML Files were succesfully generated in the xml folder.");
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
#endregion

        #region Configuration
        public void Reset(bool fromImport)
        {
            // Reset
            if (!fromImport)
            {
                Forms.GalaxySettingsForm.GalaxyName = "xu_ep2_universe";
                Forms.GalaxySettingsForm.IsCustomGalaxy = false;

                RegionDefinitionForm.RegionDefinitions.Clear();
                Forms.FactoriesForm.AllFactories.Clear();
                Forms.JobsForm.AllJobs.Clear();
                Forms.JobsForm.AllBaskets.Clear();
            }

            // Re-initialize all clusters properly
            _ = InitAllClusters();

            // Set the default value to be custom
            UpdateClusterOptions();
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
                                break;
                            }

                            if (sector.Regions.Count > 0)
                            {
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
                    File.WriteAllText(filePath, jsonContent);
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

                    if (Forms.GalaxySettingsForm.IsCustomGalaxy)
                    {
                        ToggleGalaxyMode(null);
                    }

                    // Apply vanilla changes to AllClusters
                    if (configuration.vanillaChanges != null)
                    {
                        SupportVanillaChangesInConfigImport(configuration);
                    }

                    Lazy<Cluster[]> vanillaClustersLazy = new(() => InitAllClusters(false).Clusters.Where(a => a.IsBaseGame).ToArray());

                    // Import new configuration
                    foreach (Cluster cluster in clusters)
                    {
                        // Apply support additions for new versions
                        Import_Support_NewVersions(cluster, vanillaClustersLazy);

                        // Adjust the cluster
                        ReplaceClusterByImport(cluster);

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

                if (cluster.Name.Equals("Mitsuno's Sacrifice", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine();
                }

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
                sector.Placement = New.Placement;
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
                currentSector.Placement = newSector.Placement;

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
            ClusterForm.Value.Show();
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

            ClusterForm.Value.Cluster = cluster.Value;
            ClusterForm.Value.BtnCreate.Text = "Update";
            ClusterForm.Value.TxtName.Text = selectedClusterName;
            ClusterForm.Value.txtDescription.Text = cluster.Value.Description;
            ClusterForm.Value.cmbBackgroundVisual.SelectedItem = Forms.ClusterForm.FindBackgroundVisualMappingByCode(cluster.Value.BackgroundVisualMapping ?? cluster.Value.BaseGameMapping);
            ClusterForm.Value.TxtLocation.Text = cluster.Key.ToString();
            ClusterForm.Value.ChkAutoPlacement.Checked = !cluster.Value.CustomSectorPositioning;
            ClusterForm.Value.Show();
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

            SectorForm.Value.Sector = null;
            SectorForm.Value.BtnCreate.Text = "Create";
            SectorForm.Value.Init();
            SectorForm.Value.Show();
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

            // Show all stations
            foreach (Station station in sector.Zones.SelectMany(a => a.Stations).OrderBy(a => a.Name))
            {
                _ = ListStations.Items.Add(station);
            }

            // Set details
            SetDetailsText(cluster.Value, sector);
        }

        private void BtnShowSectorMap_Click(object sender, EventArgs e)
        {
            SectorMapForm.Value.DlcListBox.Enabled = !Forms.GalaxySettingsForm.IsCustomGalaxy;
            SectorMapForm.Value.chkShowX4Sectors.Enabled = !Forms.GalaxySettingsForm.IsCustomGalaxy;
            SectorMapForm.Value.GateSectorSelection = false;
            SectorMapForm.Value.BtnSelectLocation.Enabled = false;
            SectorMapForm.Value.ControlPanel.Size = new Size(176, 241);
            SectorMapForm.Value.BtnSelectLocation.Hide();
            SectorMapForm.Value.Reset();
            SectorMapForm.Value.Show();
        }

        private static void SetOwnershipInDetails(Sector sector, StringBuilder sb)
        {
            HashSet<string> factions = sector.Zones.SelectMany(a => a.Stations)
                .Select(a => a.Faction)
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

        public void SetDetailsText(Cluster cluster, Sector sector)
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

            GateForm.Value.BtnCreateConnection.Text = "Create Connection";
            GateForm.Value.SourceCluster = cluster.Value;
            GateForm.Value.SourceSector = sector;
            GateForm.Value.Show();
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
            string selectedSector = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSector))
            {
                _ = MessageBox.Show("Please select a valid sector first.");
                return;
            }

            string selectedCluster = ClustersListBox.SelectedItem as string;
            Cluster cluster = AllClusters.Values.First(a => a.Name.Equals(selectedCluster, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Sectors.First(a => a.Name.Equals(selectedSector, StringComparison.OrdinalIgnoreCase));

            RegionForm.Value.Sector = sector;
            RegionForm.Value.Show();
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

            RegionForm.Value.Sector = sector;
            RegionForm.Value.CustomRegion = selectedRegion;
            RegionForm.Value.Show();
        }
        #endregion

        #region Stations
        private void BtnNewStation_Click(object sender, EventArgs e)
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

            StationForm.Value.Cluster = cluster;
            StationForm.Value.Sector = sector;
            StationForm.Value.Station = null;
            StationForm.Value.Show();
        }

        private void BtnRemoveStation_Click(object sender, EventArgs e)
        {
            if (ListStations.SelectedItem is not Station selectedStation)
            {
                return;
            }

            string selectedSector = SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSector))
            {
                return;
            }

            string selectedCluster = ClustersListBox.SelectedItem as string;
            Cluster cluster = AllClusters.Values.First(a => a.Name.Equals(selectedCluster, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Sectors.First(a => a.Name.Equals(selectedSector, StringComparison.OrdinalIgnoreCase));

            // Remove station from zone
            Zone zone = sector.Zones.First(a => a.Stations.Contains(selectedStation));
            _ = zone.Stations.Remove(selectedStation);

            int index = ListStations.Items.IndexOf(ListStations.SelectedItem);
            ListStations.Items.Remove(ListStations.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            ListStations.SelectedItem = index >= 0 && ListStations.Items.Count > 0 ? ListStations.Items[index] : null;

            // Set details
            SetDetailsText(cluster, sector);
        }

        private void ListStations_DoubleClick(object sender, EventArgs e)
        {
            if (ListStations.SelectedItem is not Station selectedStation)
            {
                return;
            }

            string selectedSector = SectorsListBox.SelectedItem as string;
            string selectedCluster = ClustersListBox.SelectedItem as string;
            Cluster cluster = AllClusters.Values.First(a => a.Name.Equals(selectedCluster, StringComparison.OrdinalIgnoreCase));
            Sector sector = cluster.Sectors.First(a => a.Name.Equals(selectedSector, StringComparison.OrdinalIgnoreCase));

            StationForm.Value.Cluster = cluster;
            StationForm.Value.Sector = sector;
            StationForm.Value.Station = selectedStation;
            StationForm.Value.Show();
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
    }
}
