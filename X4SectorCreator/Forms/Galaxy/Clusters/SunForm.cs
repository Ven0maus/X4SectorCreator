using System.ComponentModel;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;

namespace X4SectorCreator.Forms.Galaxy.Clusters
{
    public partial class SunForm : Form
    {
        private ClusterSystem.Sun _sun;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ClusterSystem.Sun Sun
        {
            get => _sun;
            set
            {
                _sun = value;
                if (_sun == null)
                {
                    CmbClass.SelectedIndex = 0;
                    TxtName.Text = string.Empty;
                    BtnCreate.Text = "Create";
                }
                else
                {
                    var sunType = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.SunTypeMappings), _sun.Class);
                    CmbClass.SelectedItem = sunType;
                    TxtName.Text = _sun.Name ?? string.Empty;
                    BtnCreate.Text = "Update";
                }
            }
        }

        public SunForm()
        {
            InitializeComponent();

            foreach (var sunType in MapDefaultsGeneration.SunTypeMappings.Keys)
                CmbClass.Items.Add(sunType);
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (CmbClass.SelectedIndex == -1)
            {
                MessageBox.Show("A class must be selected.", "Please fill in the mandatory fields", MessageBoxButtons.OK);
                return;
            }

            switch (BtnCreate.Text)
            {
                case "Create":
                    var sun = new ClusterSystem.Sun()
                    {
                        Name = !string.IsNullOrWhiteSpace(TxtName.Text) ? TxtName.Text : null,
                        Class = MapDefaultsGeneration.SunTypeMappings[CmbClass.SelectedItem.ToString()]
                    };
                    MainForm.Instance.ClusterForm.Value.ListBoxSuns.Items.Add(sun);
                    break;
                case "Update":
                    Sun.Name = !string.IsNullOrWhiteSpace(TxtName.Text) ? TxtName.Text : null;
                    Sun.Class = MapDefaultsGeneration.SunTypeMappings[CmbClass.SelectedItem.ToString()];
                    break;
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
