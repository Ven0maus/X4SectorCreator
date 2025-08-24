using System.Globalization;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.MapAlgorithms;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy
{
    public partial class ProceduralGalaxyForm : Form
    {
        private readonly Random _random = new();
        private readonly Dictionary<string, string> _defaultResources = new()
        {
            { "ore", "0.6" },
            { "silicon", "0.65" },
            { "ice", "0.3" },
            { "nividium", "0.01" },
            { "methane", "0.52" },
            { "hydrogen", "0.5" },
            { "helium", "0.47" },
            { "rawscrap", "0.05" },
        };

        private readonly Dictionary<string, Type> _mapAlgorithms = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, TabPage> _originalTabPageNames = new(StringComparer.OrdinalIgnoreCase);

        private readonly System.Windows.Forms.Timer _progressResetTimer;

        public ProceduralGalaxyForm()
        {
            InitializeComponent();

            _progressResetTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _progressResetTimer.Tick += ProgressResetTimerTick;

            InitializePageAlgorithmOptions();
            InitResourceRarities();
            NoiseProperty_ValueChanged(this, null);
        }

        private void ProgressResetTimerTick(object sender, EventArgs e)
        {
            ProcGenProcess.Value = 0;
            _progressResetTimer.Stop();
        }

        private void InitializePageAlgorithmOptions()
        {
            // Init algorithms
            RegisterMapAlgorithm<PureRandom>(TabRandom);
            RegisterMapAlgorithm<Noise>(TabNoise);

            var tabPages = MapAlgorithmOptions.TabPages.Cast<TabPage>();

            int count = 0;
            foreach (var tabPage in tabPages)
            {
                _originalTabPageNames[tabPage.Text] = tabPage;

                // Rename
                tabPage.Text = "Settings";

                // Remove all except first page
                if (count != 0)
                    MapAlgorithmOptions.TabPages.Remove(tabPage);
                count++;
            }
        }

        private void RegisterMapAlgorithm<T>(TabPage tabPage) where T : Procedural
        {
            _mapAlgorithms.Add(tabPage.Text, typeof(PureRandom));
        }

        private void InitResourceRarities()
        {
            foreach (var resource in _defaultResources)
            {
                RegionResourceRarity.Rows.Add(resource.Key, resource.Value);
            }

            RegionResourceRarity.CellValidating += RegionResourceRarity_CellValidating;
            RegionResourceRarity.CellValidated += RegionResourceRarity_CellValidated;
        }

        private void RegionResourceRarity_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var row = RegionResourceRarity.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];
            var value = cell.Value as string;

            if (string.IsNullOrWhiteSpace(value))
            {
                cell.Value = _defaultResources[row.Cells[0].Value as string];
            }
        }

        private void RegionResourceRarity_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var value = e.FormattedValue as string;
            if (e.ColumnIndex != 1 || string.IsNullOrWhiteSpace(value)) return;

            if (!float.TryParse(value, CultureInfo.InvariantCulture, out var nr) || nr < 0 || nr > 1)
            {
                _ = MessageBox.Show("Please specify a valid value between 0 and 1");
                e.Cancel = true;
                return;
            }
        }

        private int GetSeed(TextBox textbox)
        {
            if (string.IsNullOrWhiteSpace(textbox.Text))
                textbox.Text = _random.Next().ToString();
            if (!int.TryParse(textbox.Text, out int seed))
                seed = Localisation.GetFnvHash(textbox.Text);
            return seed;
        }

        private void RandomizeSeeds()
        {
            if (ChkMapRandomizeSeed.Checked)
            {
                TxtMapSeed.Text = _random.Next().ToString();
                NoiseProperty_ValueChanged(this, null);
            }
            if (ChkFactionsRandomizeSeed.Checked)
                TxtFactionSeed.Text = _random.Next().ToString();
            if (ChkRegionRandomizeSeed.Checked)
                TxtRegionSeed.Text = _random.Next().ToString();
            if (ChkConnectionsRandomizeSeed.Checked)
                TxtConnectionSeed.Text = _random.Next().ToString();
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            // Cleanup before start
            FactoriesForm.AllFactories.Clear();
            JobsForm.AllJobs.Clear();
            FactionsForm.AllCustomFactions.Clear();
            RegionDefinitionForm.RegionDefinitions.Clear();

            ProcGenProcess.Minimum = 0;
            ProcGenProcess.Maximum = 7;
            ProcGenProcess.Value = 0;
            ProcGenProcess.Step = 1;

            RandomizeSeeds();
            ProcGenProcess.PerformStep();

            // Generate map if not generated before or seed changed
            List<Cluster> clusters = GenerateClusters();
            ProcGenProcess.PerformStep();

            // Connections
            if (ChkGenerateConnections.Checked)
            {
                var settings = new ProceduralSettings
                {
                    Seed = GetSeed(TxtConnectionSeed),
                    MinGatesPerSector = (int)NrMinGates.Value,
                    MaxGatesPerSector = (int)NrMaxGates.Value,
                    GateMultiChancePerSector = (int)NrMultiConnectionChance.Value
                };

                GalaxyGenerator.CreateConnections(clusters, settings);
            }
            ProcGenProcess.PerformStep();

            // Regions
            if (ChkRegions.Checked)
            {
                var settings = new ProceduralSettings
                {
                    Seed = GetSeed(TxtRegionSeed),
                    Resources = _defaultResources
                };

                GalaxyGenerator.CreateRegions(clusters, settings);
            }
            ProcGenProcess.PerformStep();

            // Factions
            if (ChkFactions.Checked)
            {
                var settings = new ProceduralSettings
                {
                    Seed = GetSeed(TxtFactionSeed),
                    MinMainFactions = (int)NrMinMainFactions.Value,
                    MaxMainFactions = (int)NrMaxMainFactions.Value,
                    MinPirateFactions = (int)NrMinPirateFactions.Value,
                    MaxPirateFactions = (int)NrMaxPirateFactions.Value,
                    MinSectorOwnership = (int)NrFacControlMin.Value,
                    MaxSectorOwnership = (int)NrFacControlMax.Value,
                };

                GalaxyGenerator.CreateFactions(clusters, settings);
            }
            ProcGenProcess.PerformStep();

            // Pick new player HQ sector
            SetPlayerHQSector(clusters);
            ProcGenProcess.PerformStep();

            // Assign galaxy
            SetProceduralGalaxy(clusters);
            ProcGenProcess.PerformStep();
            _progressResetTimer.Start();
        }

        public static void SetPlayerHQSector(List<Cluster> clusters)
        {
            // Attempt to find a cluster with only one unowned sector
            // Prefer a sector with only one gate connection (cornered)
            var claimSpaceFactions = FactionsForm.AllCustomFactions.Values
                .Where(a => a.Tags.Contains("claimspace", StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Id)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Filter clusters that only contain unowned sectors
            var validClusters = clusters
                .Where(c => c.Sectors.All(s => IsOwnerless(s, claimSpaceFactions)))
                .ToList();

            // Try to prefer cluster with exactly one sector and one zone gate
            var preferredCluster = validClusters
                .FirstOrDefault(c =>
                    c.Sectors.Count == 1 &&
                    GetZoneGateCount(c.Sectors.First()) == 1
                );

            // Next attempt only one sector
            preferredCluster ??= validClusters.FirstOrDefault(c => c.Sectors.Count == 1);

            // Fallback to first valid cluster if no preferred one is found
            var selectedCluster = preferredCluster ?? validClusters.FirstOrDefault() ?? clusters.First();
            var selectedSector = selectedCluster.Sectors.First();

            // Disable faction logic if sector is ownerless
            if (IsOwnerless(selectedSector, claimSpaceFactions))
                selectedSector.DisableFactionLogic = true;

            // Set as headquarters sector
            GalaxySettingsForm.HeadQuartersSector = GetSectorMacro(selectedCluster, selectedSector);
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

        private static int GetZoneGateCount(Sector sector)
        {
            return sector.Zones.SelectMany(a => a.Gates).Count();
        }

        private static bool IsOwnerless(Sector sector, HashSet<string> claimSpaceFactions)
        {
            foreach (var zone in sector.Zones)
            {
                foreach (var station in zone.Stations)
                {
                    if (claimSpaceFactions.Contains(station.Owner))
                        return false;
                }
            }
            return true;
        }

        private static void SetProceduralGalaxy(List<Cluster> clusters)
        {
            MainForm.Instance.SetProceduralGalaxy(clusters);

            // Visual update the sector map if opened
            if (MainForm.Instance.SectorMapForm.IsInitialized)
                MainForm.Instance.SectorMapForm.Value.Reset();

            // Update cluster options
            MainForm.Instance.UpdateClusterOptions();

            // Update faction values if opened
            if (MainForm.Instance.FactionsForm.IsInitialized)
                MainForm.Instance.FactionsForm.Value.InitFactionValues();

            // Update player HQ selection in combobox if opened
            if (MainForm.Instance.GalaxySettingsForm.IsInitialized)
                MainForm.Instance.GalaxySettingsForm.Value.InitPlayerHqSectorSelection();
        }

        private List<Cluster> GenerateClusters()
        {
            var settings = new ProceduralSettings
            {
                Seed = GetSeed(TxtMapSeed),
                Width = (int)NrGridWidth.Value,
                Height = (int)NrGridHeight.Value,
                MultiSectorChance = (int)NrChanceMultiSectors.Value,
                MaxSectorRadius = (int)NrMaxSectorRadius.Value,
                MapAlgorithm = CmbClusterDistribution.SelectedItem as string,

                /* Algorithm data */
                // Pure Random
                ClusterChance = (int)NrClusterChance.Value,

                // Noise
                NoiseOctaves = (int)NrNoiseOctaves.Value,
                NoisePersistance = (float)NrNoisePersistance.Value,
                NoiseLacunarity = (float)NrNoiseLacunarity.Value,
                NoiseScale = (float)NrNoiseScale.Value,
                NoiseOffset = new Point((int)NrNoiseOffsetX.Value, (int)NrNoiseOffsetY.Value),
                NoiseThreshold = (float)NrNoiseThreshold.Value,
            };

            var selectedAlgorithm = CmbClusterDistribution.SelectedItem as string;
            var procedural = (Procedural)Activator.CreateInstance(_mapAlgorithms[selectedAlgorithm], settings);
            var clusters = procedural.Generate().ToList();
            return clusters;
        }

        private void BtnOpenSectorMap_Click(object sender, EventArgs e)
        {
            var sectorMapForm = MainForm.Instance.SectorMapForm;
            sectorMapForm.Value.DlcListBox.Enabled = !Forms.GalaxySettingsForm.IsCustomGalaxy;
            sectorMapForm.Value.GateSectorSelection = false;
            sectorMapForm.Value.BtnSelectLocation.Enabled = false;
            sectorMapForm.Value.ControlPanel.Size = new Size(176, 311);
            sectorMapForm.Value.BtnSelectLocation.Hide();
            sectorMapForm.Value.Reset();
            sectorMapForm.Value.Show();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CmbClusterDistribution_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage _currentMapAlgorithmOptionsPage = _originalTabPageNames[CmbClusterDistribution.SelectedItem as string];

            MapAlgorithmOptions.TabPages.Clear();
            MapAlgorithmOptions.TabPages.Add(_currentMapAlgorithmOptionsPage);
        }

        private void NoiseProperty_ValueChanged(object sender, EventArgs e)
        {
            var settings = new ProceduralSettings
            {
                Seed = GetSeed(TxtMapSeed),
                Width = (int)NrGridWidth.Value,
                Height = (int)NrGridHeight.Value,

                // Noise
                NoiseOctaves = (int)NrNoiseOctaves.Value,
                NoisePersistance = (float)NrNoisePersistance.Value,
                NoiseLacunarity = (float)NrNoiseLacunarity.Value,
                NoiseScale = (float)NrNoiseScale.Value,
                NoiseOffset = new Point((int)NrNoiseOffsetX.Value, (int)NrNoiseOffsetY.Value),
                NoiseThreshold = (float)NrNoiseThreshold.Value,
            };

            Noise.GenerateVisual(NoiseVisual, settings);
            NoiseVisual.Invalidate();
        }
    }
}
