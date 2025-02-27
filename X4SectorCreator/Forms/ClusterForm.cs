using System.ComponentModel;
using System.Text.RegularExpressions;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class ClusterForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Cluster Cluster { get; set; }

        public ClusterForm()
        {
            InitializeComponent();
        }

        private void BtnPick_Click(object sender, EventArgs e)
        {
            MainForm.Instance.SectorMapForm.GateSectorSelection = false;
            MainForm.Instance.SectorMapForm.BtnSelectLocation.Enabled = false;
            MainForm.Instance.SectorMapForm.ControlPanel.Size = new Size(204, 144);
            MainForm.Instance.SectorMapForm.BtnSelectLocation.Show();
            MainForm.Instance.SectorMapForm.Reset();
            MainForm.Instance.SectorMapForm.Show();
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            string name = TxtName.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                _ = MessageBox.Show("Please select a valid (non empty / non whitespace) name.");
                return;
            }

            // Check if name already exists
            if (MainForm.Instance.CustomClusters.Values.Any(a =>
            {
                // Skip the cluster we're updating
                return (Cluster == null || !Cluster.Name.Equals(a.Name, StringComparison.OrdinalIgnoreCase))
&& a.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
            }))
            {
                _ = MessageBox.Show($"A cluster with the name \"{name}\" already exists, please choose another name.");
                return;
            }

            string location = TxtLocation.Text;
            if (string.IsNullOrWhiteSpace(location))
            {
                _ = MessageBox.Show("Please select a valid location on the map by using the \"Pick\" button.");
                return;
            }

            Match match = RegexHelper.TupleLocationRegex().Match(location);
            if (match.Success)
            {
                (int X, int Y) coordinate = (X: int.Parse(match.Groups[1].Value), Y: int.Parse(match.Groups[2].Value));
                if (MainForm.Instance.CustomClusters.TryGetValue(coordinate, out Cluster cluster))
                {
                    if (Cluster == null || !cluster.Name.Equals(Cluster.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        _ = MessageBox.Show($"The selected coordinate already contains a custom cluster \"{cluster.Name}\".");
                        return;
                    }
                }

                switch (BtnCreate.Text)
                {
                    case "Create":
                        // Add new custom cluster
                        MainForm.Instance.CustomClusters.Add(coordinate, cluster = new Cluster
                        {
                            Id = MainForm.Instance.CustomClusters.Values.Count + 1,
                            Name = name,
                            Position = new Point(coordinate.X, coordinate.Y),
                            Sectors = []
                        });

                        // Create also a sector and one zone with the same name
                        cluster.Sectors.Add(new Sector
                        {
                            Id = 1,
                            Name = name,
                            Owner = "None",
                            Zones =
                            [
                                new Zone
                                {
                                    Id = 1,
                                    Name = "Zone 1",
                                    Gates = []
                                }
                            ]
                        });

                        // Add to listbox and select it
                        _ = MainForm.Instance.ClustersListBox.Items.Add(name);
                        MainForm.Instance.ClustersListBox.SelectedItem = name;
                        break;
                    case "Update":
                        // Update cluster
                        string oldName = Cluster.Name;
                        Point oldPosition = Cluster.Position;

                        // Re-map
                        _ = MainForm.Instance.CustomClusters.Remove((oldPosition.X, oldPosition.Y));
                        Cluster.Position = new Point(coordinate.X, coordinate.Y);
                        Cluster.Name = name;
                        MainForm.Instance.CustomClusters.Add(coordinate, Cluster);

                        // Update listbox
                        MainForm.Instance.ClustersListBox.Items.Remove(oldName);
                        _ = MainForm.Instance.ClustersListBox.Items.Add(name);
                        MainForm.Instance.ClustersListBox.SelectedItem = name;
                        break;
                }

                ResetAndHide();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ResetAndHide();
        }

        private void ResetAndHide()
        {
            Cluster = null;
            TxtName.Text = string.Empty;
            TxtLocation.Text = string.Empty;
            BtnCreate.Text = "Create";
            Hide();
        }
    }

    public partial class RegexHelper
    {
        [GeneratedRegex(@"\((-?\d+),\s*(-?\d+)\)")]
        public static partial Regex TupleLocationRegex();

        [GeneratedRegex(@"\((-?\d+),\s*(-?\d+)\)\s*\[(\d+)\]")]
        public static partial Regex TupleLocationChildIndexRegex();
    }
}
