using System.Globalization;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy
{
    public partial class ProceduralGalaxyForm : Form
    {
        private readonly Random _random = new();
        private readonly Dictionary<string, string> _defaultResources = new()
            {
                { "ore", "0.5" },
                { "silicon", "0.5" },
                { "ice", "0.3" },
                { "methane", "0.5" },
                { "hydrogen", "0.5" },
                { "helium", "0.5" },
                { "scrap", "0.1" },
            };

        public ProceduralGalaxyForm()
        {
            InitializeComponent();
            InitResourceRarities();
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

        private int GetSeed()
        {
            if (string.IsNullOrWhiteSpace(TxtSeed.Text))
                TxtSeed.Text = _random.Next().ToString();
            if (!int.TryParse(TxtSeed.Text, out int seed))
                seed = Localisation.GetFnvHash(TxtSeed.Text);
            return seed;
        }

        private void BtnGenerateClusters_Click(object sender, EventArgs e)
        {
            IEnumerable<Cluster> clusters = GenerateClusters();
            SetProceduralGalaxy(clusters);
        }

        private void BtnGenerateConnections_Click(object sender, EventArgs e)
        {
            var clusters = MainForm.Instance.AllClusters.Values.ToList();
            GalaxyGenerator.CreateConnections(clusters);
            SetProceduralGalaxy(clusters);
        }

        private void BtnGenerateRegions_Click(object sender, EventArgs e)
        {
            var clusters = MainForm.Instance.AllClusters.Values.ToList();
            GalaxyGenerator.CreateRegions(clusters);
            SetProceduralGalaxy(clusters);
        }

        private void BtnGenerateCustomFactions_Click(object sender, EventArgs e)
        {
            var clusters = MainForm.Instance.AllClusters.Values.ToList();
            GalaxyGenerator.CreateCustomFactions(clusters);
            SetProceduralGalaxy(clusters);
        }

        private void BtnGenerateVanillaFactions_Click(object sender, EventArgs e)
        {
            var clusters = MainForm.Instance.AllClusters.Values.ToList();
            GalaxyGenerator.CreateVanillaFactions(clusters);
            SetProceduralGalaxy(clusters);
        }

        private void BtnGenerateAll_Click(object sender, EventArgs e)
        {
            // Re-generate seed automatically
            if (ChkAutoSeed.Checked)
                TxtSeed.Text = _random.Next().ToString();

            // Map
            var clusters = GenerateClusters();

            // Connections
            if (ChkGenerateConnections.Checked)
                GalaxyGenerator.CreateConnections(clusters);

            // Regions
            if (ChkRegions.Checked)
                GalaxyGenerator.CreateRegions(clusters);

            // Factions
            if (ChkCustomFactions.Checked)
                GalaxyGenerator.CreateCustomFactions(clusters);
            if (ChkGenerateVanillaFactions.Checked)
                GalaxyGenerator.CreateVanillaFactions(clusters);

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
                Seed = GetSeed(),
                Width = (int)NrGridWidth.Value,
                Height = (int)NrGridHeight.Value,
                ClusterChance = (int)NrClusterChance.Value,
                MultiSectorChance = (int)NrChanceMultiSectors.Value,
                MapAlgorithm = CmbClusterDistribution.SelectedItem as string
            };

            return GalaxyGenerator.CreateClusters(settings);
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

        public class ProceduralSettings
        {
            private Random _random;
            public Random Random => _random ??= new Random(Seed);

            public int Seed { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int ClusterChance { get; set; }
            public int MultiSectorChance { get; set; }
            public string MapAlgorithm { get; set; }
        }
    }
}
