using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.Sectors
{
    public partial class SectorWorldForm : Form
    {
        private SectorWorld _sectorWorld;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SectorWorld SectorWorld
        {
            get => _sectorWorld;
            set
            {
                _sectorWorld = value;
                BtnCreate.Text = "Update";
                CmbPartSelection.SelectedItem = _sectorWorld.Part;
                NrFactor.Value = _sectorWorld.Factor;
            }
        }

        private Cluster _cluster;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Cluster Cluster
        {
            get => _cluster;
            set
            {
                _cluster = value;
                CmbPartSelection.Items.Clear();

                // Populate from cluster
                foreach (var worldPart in _cluster.ClusterSystem.Parts)
                    CmbPartSelection.Items.Add(worldPart);

                // Select the first item by default
                if (CmbPartSelection.Items.Count > 0)
                    CmbPartSelection.SelectedIndex = 0;
            }
        }

        public SectorWorldForm()
        {
            InitializeComponent();
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (CmbPartSelection.SelectedIndex == -1)
            {
                MessageBox.Show("Selection Part is a mandatory field.", "Please select a part.", MessageBoxButtons.OK);
                return;
            }

            switch (BtnCreate.Text)
            {
                case "Create":
                    if (MainForm.Instance.SectorForm.IsInitialized && MainForm.Instance.SectorForm.Value.Visible)
                    {
                        var sectorWorld = new SectorWorld
                        {
                            Part = CmbPartSelection.SelectedItem.ToString(),
                            Factor = NrFactor.Value
                        };
                        MainForm.Instance.SectorForm.Value.ListBoxWorlds.Items.Add(sectorWorld);
                    }
                    break;
                case "Update":
                    if (MainForm.Instance.SectorForm.IsInitialized &&
                        MainForm.Instance.SectorForm.Value.Visible &&
                        SectorWorld != null)
                    {
                        SectorWorld.Part = CmbPartSelection.SelectedItem.ToString();
                        SectorWorld.Factor = NrFactor.Value;
                        MainForm.Instance.SectorForm.Value.ListBoxWorlds.Invalidate();
                    }
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
