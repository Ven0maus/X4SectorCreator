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

        private bool _mapGenerated = false;

        public ProceduralGalaxyForm()
        {
            InitializeComponent();
            InitializePageAlgorithmOptions();
            InitResourceRarities();
            NoiseProperty_ValueChanged(this, null);
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
            var currentMapSeed = GetSeed(TxtMapSeed);

            RandomizeSeeds();

            var regeneratedMap = (!_mapGenerated || currentMapSeed != GetSeed(TxtMapSeed));
            // Generate map if not generated before or seed changed
            List<Cluster> clusters = regeneratedMap ? 
                GenerateClusters() : [.. MainForm.Instance.AllClusters.Values];

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
            else
            {
                if (regeneratedMap)
                    RegionDefinitionForm.RegionDefinitions.Clear();
            }

            // Factions
            if (ChkFactions.Checked)
            {
                var settings = new ProceduralSettings
                {
                    Seed = GetSeed(TxtFactionSeed),
                    GenerateCustomFactions = ChkCustomFactions.Checked,
                    GenerateVanillaFactions = ChkVanillaFactions.Checked,
                    MinTotalFactions = (int)NrFactionMin.Value,
                    MaxTotalFactions = (int)NrFactionMax.Value,
                    MinSectorOwnership = (int)NrFacControlMin.Value,
                    MaxSectorOwnership = (int)NrFacControlMax.Value,
                };

                GalaxyGenerator.CreateFactions(clusters, settings);
            }

            SetProceduralGalaxy(clusters);
        }

        private static void SetProceduralGalaxy(IEnumerable<Cluster> clusters)
        {
            MainForm.Instance.SetProceduralGalaxy(clusters);
            MainForm.Instance.SectorMapForm.Value.Reset();
            MainForm.Instance.UpdateClusterOptions();
        }

        private List<Cluster> GenerateClusters()
        {
            var settings = new ProceduralSettings
            {
                Seed = GetSeed(TxtMapSeed),
                Width = (int)NrGridWidth.Value,
                Height = (int)NrGridHeight.Value,
                MultiSectorChance = (int)NrChanceMultiSectors.Value,
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
            _mapGenerated = true;
            return clusters;
        }

        private void BtnOpenSectorMap_Click(object sender, EventArgs e)
        {
            var sectorMapForm = MainForm.Instance.SectorMapForm;
            sectorMapForm.Value.DlcListBox.Enabled = !Forms.GalaxySettingsForm.IsCustomGalaxy;
            sectorMapForm.Value.chkShowX4Sectors.Enabled = !Forms.GalaxySettingsForm.IsCustomGalaxy;
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
