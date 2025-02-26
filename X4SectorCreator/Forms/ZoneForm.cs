using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class ZoneForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Zone Zone { get; set; }

        public ZoneForm()
        {
            InitializeComponent();
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            var name = TxtName.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please select a valid (non empty / non whitespace) name.");
                return;
            }

            var selectedClusterName = MainForm.Instance.ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                MessageBox.Show("Please select a cluster in the main window.");
                return;
            }

            var selectedSectorName = MainForm.Instance.SectorsListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSectorName))
            {
                MessageBox.Show("Please select a sector in the main window.");
                return;
            }

            var cluster = MainForm.Instance.CustomClusters.Values.First(a => a.Name.Equals(selectedClusterName));
            var sector = cluster.Sectors.First(a => a.Name.Equals(selectedSectorName, StringComparison.OrdinalIgnoreCase));

            // Check if name already exists within the selected sector
            foreach (var zone in sector.Zones)
            {
                // Skip the one we are modifying
                if (Zone != null && Zone.Name.Equals(zone.Name, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (zone.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"A zone with the name \"{name}\" already exists in sector \"{sector.Name}\", please choose another name.");
                    return;
                }
            }

            switch (BtnCreate.Text)
            {
                case "Create":
                    // No limit on zones (as far as I know)
                    // Create new zone in selected sector
                    sector.Zones.Add(new Zone
                    {
                        Id = sector.Zones.Count + 1,
                        Name = name,
                        Gates = []
                    });

                    // Add zone to listbox and select it
                    MainForm.Instance.ZonesListBox.Items.Add(name);
                    MainForm.Instance.ZonesListBox.SelectedItem = name;
                    break;

                case "Update":
                    Zone.Name = name;
                    var index = MainForm.Instance.ZonesListBox.SelectedIndex;
                    MainForm.Instance.ZonesListBox.Items[index] = name;
                    MainForm.Instance.ZonesListBox.SelectedItem = name;
                    break;
            }

            ResetAndHide();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ResetAndHide();
        }

        private void ResetAndHide()
        {
            Zone = null;
            TxtName.Text = string.Empty;
            Hide();
        }
    }
}
