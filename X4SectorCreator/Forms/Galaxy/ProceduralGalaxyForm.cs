using System.Globalization;
using X4SectorCreator.Forms.Galaxy.ProceduralGenerators;

namespace X4SectorCreator.Forms.Galaxy
{
    public partial class ProceduralGalaxyForm : Form
    {
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

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            var generator = new GalaxyGenerator(new Random().Next());
            MainForm.Instance.SetProceduralGalaxy(generator.GenerateGalaxy(16, 8));
            MainForm.Instance.SectorMapForm.Value.Reset();

            // Update also the main form's clusters listbox
            MainForm.Instance.UpdateClusterOptions();
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
    }
}
