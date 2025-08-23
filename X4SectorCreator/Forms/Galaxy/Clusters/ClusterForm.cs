using System.ComponentModel;
using System.Text.RegularExpressions;
using X4SectorCreator.Forms.Galaxy.Clusters;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class ClusterForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Cluster Cluster { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ClusterXml { get; set; }

        private readonly LazyEvaluated<SoundtrackSelectorForm> _soundtrackSelectorForm = new(() => new SoundtrackSelectorForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<ClusterXmlEditorForm> _clusterXmlEditorForm = new(() => new ClusterXmlEditorForm(), a => !a.IsDisposed);

        public ClusterForm()
        {
            InitializeComponent();

            foreach (KeyValuePair<string, string> mapping in MainForm.Instance.BackgroundVisualMapping.OrderBy(a => a.Key))
            {
                _ = cmbBackgroundVisual.Items.Add(mapping.Key);
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            string name = TxtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                _ = MessageBox.Show("Please select a valid (non empty / non whitespace) name.");
                return;
            }

            // Check if name already exists in cluster or sectors
            foreach (var cluster in MainForm.Instance.AllClusters.Values.OrderBy(a => a.Name))
            {
                bool invalidName = false;
                if (Cluster != null)
                {
                    // Check in case we are editing a cluster
                    invalidName = Cluster != cluster && (cluster.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        cluster.Sectors.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    // Check in case we are creating a cluster
                    invalidName = cluster.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        cluster.Sectors.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }

                if (invalidName)
                {
                    _ = MessageBox.Show($"A cluster or sector with the name \"{name}\" already exists in cluster \"{cluster.Name}\", please choose another name.");
                    return;
                }
            }

            string location = TxtLocation.Text;
            if (string.IsNullOrWhiteSpace(location))
            {
                _ = MessageBox.Show("Please select a valid location on the map by using the \"Pick\" button.");
                return;
            }

            string selectedBackgroundVisual = cmbBackgroundVisual.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedBackgroundVisual))
            {
                _ = MessageBox.Show("Please select a valid visual background for the cluster.");
                return;
            }
            string backgroundVisualMapping = MainForm.Instance.BackgroundVisualMapping[selectedBackgroundVisual];

            bool beforeAutoPos = Cluster?.CustomSectorPositioning ?? false;
            Match match = RegexHelper.TupleLocationRegex().Match(location);
            if (match.Success)
            {
                (int X, int Y) coordinate = (X: int.Parse(match.Groups[1].Value), Y: int.Parse(match.Groups[2].Value));
                if (MainForm.Instance.AllClusters.TryGetValue(coordinate, out Cluster cluster))
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
                        MainForm.Instance.AllClusters.Add(coordinate, Cluster = new Cluster
                        {
                            Id = MainForm.Instance.AllClusters.Values
                                .Where(a => !a.IsBaseGame)
                                .DefaultIfEmpty(new Cluster()).Max(a => a.Id) + 1,
                            Name = name,
                            Description = txtDescription.Text,
                            BackgroundVisualMapping = backgroundVisualMapping,
                            Soundtrack = string.IsNullOrWhiteSpace(TxtSoundtrack.Text) ? null : TxtSoundtrack.Text,
                            Position = new Point(coordinate.X, coordinate.Y),
                            CustomSectorPositioning = !ChkAutoPlacement.Checked,
                            CustomClusterXml = ClusterXml,
                            Sectors = []
                        });

                        // Create also a sector and one zone with the same name
                        var sector = new Sector
                        {
                            Id = 1,
                            Name = name,
                            Owner = "None"
                        };
                        Cluster.Sectors.Add(sector);

                        // Create initial zones based on the sector range
                        sector.InitializeOrUpdateZones();

                        // Add to listbox and select it
                        _ = MainForm.Instance.ClustersListBox.Items.Add(name);
                        MainForm.Instance.ClustersListBox.SelectedItem = name;
                        break;
                    case "Update":
                        // Update cluster
                        string oldName = Cluster.Name;
                        Point oldPosition = Cluster.Position;

                        // Re-map
                        _ = MainForm.Instance.AllClusters.Remove((oldPosition.X, oldPosition.Y));
                        Cluster.Position = new Point(coordinate.X, coordinate.Y);
                        Cluster.Name = name;
                        Cluster.Description = txtDescription.Text;
                        Cluster.BackgroundVisualMapping = backgroundVisualMapping;
                        Cluster.Soundtrack = string.IsNullOrWhiteSpace(TxtSoundtrack.Text) ? null : TxtSoundtrack.Text;
                        Cluster.CustomSectorPositioning = !ChkAutoPlacement.Checked;
                        Cluster.CustomClusterXml = ClusterXml;
                        MainForm.Instance.AllClusters.Add(coordinate, Cluster);

                        // Update listbox
                        int index = MainForm.Instance.ClustersListBox.SelectedIndex;
                        MainForm.Instance.ClustersListBox.Items.Remove(oldName);
                        MainForm.Instance.ClustersListBox.Items.Insert(index, name);
                        MainForm.Instance.ClustersListBox.SelectedItem = name;
                        break;
                }

                // If auto positioning was enabled, re position current sectors automatically
                if (beforeAutoPos != Cluster.CustomSectorPositioning && !Cluster.CustomSectorPositioning)
                {
                    Cluster.AutoPositionSectors();
                }

                if (SectorMapForm.IsMapOptionChecked(SectorMapForm.MapOption.Keep_Window_Open))
                {
                    MainForm.Instance.SectorMap.Value.Reset(false);
                }

                ResetAndHide();
            }
        }

        public static string FindBackgroundVisualMappingByCode(string mappingCode)
        {
            return MainForm.Instance.BackgroundVisualMapping
                .Where(a => a.Value.Equals(mappingCode, StringComparison.OrdinalIgnoreCase))
                .First()
                .Key;
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
            Close();
        }

        private void TxtSoundtrack_MouseClick(object sender, MouseEventArgs e)
        {
            _soundtrackSelectorForm.Value.ClusterForm = this;
            _soundtrackSelectorForm.Value.Show();
        }

        private void TxtLocation_MouseClick(object sender, MouseEventArgs e)
        {
            MainForm.Instance.SectorMap.Value.DlcListBox.Enabled = !GalaxySettingsForm.IsCustomGalaxy;
            MainForm.Instance.SectorMap.Value.GateSectorSelection = false;
            MainForm.Instance.SectorMap.Value.ClusterSectorSelection = true;
            MainForm.Instance.SectorMap.Value.BtnSelectLocation.Enabled = false;
            MainForm.Instance.SectorMap.Value.ControlPanel.Size = new Size(176, 347);
            MainForm.Instance.SectorMap.Value.BtnSelectLocation.Show();
            MainForm.Instance.SectorMap.Value.Reset();
            MainForm.Instance.SectorMap.Value.Show();
        }

        private void BtnEditClusterXml_Click(object sender, EventArgs e)
        {
            if (Cluster != null && Cluster.IsBaseGame)
            {
                _ = MessageBox.Show("Cluster XML editor is not available for vanilla clusters.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ClusterXml))
            {
                _clusterXmlEditorForm.Value.TxtClusterXml.Text = GetTemplateXml(Cluster);
            }
            else
            {
                _clusterXmlEditorForm.Value.TxtClusterXml.Text = ClusterXml;
            }

            _clusterXmlEditorForm.Value.ClusterForm = this;
            _clusterXmlEditorForm.Value.Show();
        }

        public static string GetTemplateXml(Cluster cluster)
        {
            return Cluster.TemplateClusterXml.Replace("{CLUSTERCODE}", GetClusterCode(cluster));
        }

        private static string GetClusterCode(Cluster cluster)
        {
            int id;
            if (cluster == null)
            {
                id = MainForm.Instance.AllClusters.Values
                                .Where(a => !a.IsBaseGame)
                                .DefaultIfEmpty(new Cluster()).Max(a => a.Id) + 1;
            }
            else
            {
                id = cluster.Id;
            }
            return $"PREFIX_CL_c{id:D3}";
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
