using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class SectorForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Sector Sector { get; set; }

        public SectorForm()
        {
            InitializeComponent();
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            string name = TxtName.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                _ = MessageBox.Show("Please select a valid (non empty / non whitespace) name.");
                return;
            }

            string selectedClusterName = MainForm.Instance.ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                _ = MessageBox.Show("Please select a cluster in the main window.");
                return;
            }

            // Check if name already exists
            foreach (Cluster cluster in MainForm.Instance.AllClusters.Values)
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    // Skip the one we are modifying
                    if (Sector != null && Sector.Name.Equals(sector.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (sector.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        _ = MessageBox.Show($"A sector with the name \"{name}\" already exists in cluster \"{cluster.Name}\", please choose another name.");
                        return;
                    }
                }
            }

            KeyValuePair<(int, int), Cluster> selectedCluster = MainForm.Instance.AllClusters.First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));

            switch (BtnCreate.Text)
            {
                case "Create":
                    if (selectedCluster.Value.Sectors.Count == 3)
                    {
                        _ = MessageBox.Show($"Reached maximum allowed sectors for cluster \"{selectedCluster.Value.Name}\".");
                        return;
                    }

                    // Create new sector in selected cluster
                    selectedCluster.Value.Sectors.Add(new Sector
                    {
                        Id = selectedCluster.Value.Sectors.Count + 1,
                        Name = name,
                        Owner = "None",
                        Zones = [ ]
                    });

                    // Add sector to listbox and select it
                    _ = MainForm.Instance.SectorsListBox.Items.Add(name);
                    MainForm.Instance.SectorsListBox.SelectedItem = name;
                    break;

                case "Update":
                    string selectedSector = MainForm.Instance.SectorsListBox.SelectedItem as string;
                    if (string.IsNullOrEmpty(selectedSector))
                    {
                        ResetAndHide();
                        return;
                    }
                    Sector sector = selectedCluster.Value.Sectors.First(a => a.Name.Equals(selectedSector));
                    sector.Name = name;
                    int index = MainForm.Instance.SectorsListBox.SelectedIndex;
                    MainForm.Instance.SectorsListBox.Items[index] = name;
                    MainForm.Instance.SectorsListBox.SelectedItem = name;
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
            Sector = null;
            TxtName.Text = string.Empty;
            Hide();
        }
    }
}
