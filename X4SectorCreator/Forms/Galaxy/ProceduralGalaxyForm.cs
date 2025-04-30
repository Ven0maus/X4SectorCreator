using System.Globalization;

namespace X4SectorCreator.Forms.Galaxy
{
    public partial class ProceduralGalaxyForm : Form
    {
        private readonly Dictionary<string, string> _defaultResources = new()
            {
                { "ore", "0.38" },
                { "silicon", "0.43" },
                { "ice", "0.28" },
                { "methane", "0.42" },
                { "hydrogen", "0.43" },
                { "helium", "0.41" },
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
    }
}
