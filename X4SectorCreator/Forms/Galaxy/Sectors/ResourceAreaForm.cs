using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class ResourceAreaForm : Form
    {
        private Resource _resource;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Resource Resource
        {
            get => _resource;
            set
            {
                _resource = value;
                if (_resource != null)
                {
                    cmbWare.SelectedItem = _resource.Ware;
                    cmbYield.SelectedItem = _resource.Yield;
                    cmbSize.SelectedItem = _resource.Size;
                    cmbSpeed.SelectedItem = _resource.Speed;
                    nrAmount.Value = _resource.Amount;
                    BtnAdd.Text = "Update";
                }
            }
        }

        public ResourceAreaForm()
        {
            InitializeComponent();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cmbWare.SelectedItem == null)
            {
                _ = MessageBox.Show("Please select a valid ware.");
                return;
            }

            if (cmbYield.SelectedItem == null)
            {
                _ = MessageBox.Show("Please select a valid yield.");
                return;
            }

            if (cmbSize.SelectedItem == null)
            {
                _ = MessageBox.Show("Please select a valid size.");
                return;
            }

            if (cmbSpeed.SelectedItem == null)
            {
                _ = MessageBox.Show("Please select a valid speed.");
                return;
            }

            if (nrAmount.Value == 0)
            {
                _ = MessageBox.Show("Amount must be higher than 0.");
                return;
            }

            switch (BtnAdd.Text)
            {
                case "Add":
                    Resource resource = new()
                    {
                        Ware = cmbWare.Text,
                        Yield = cmbYield.Text,
                        Size = cmbSize.Text,
                        Speed = cmbSpeed.Text,
                        Amount = (int)nrAmount.Value
                    };
                    _ = MainForm.Instance.SectorForm.Value.RAListBox.Items.Add(resource);
                    break;
                case "Update":
                    int index = MainForm.Instance.SectorForm.Value.RAListBox.SelectedIndex;
                    MainForm.Instance.SectorForm.Value.RAListBox.Items.Remove(Resource);
                    Resource.Ware = cmbWare.Text;
                    Resource.Yield = cmbYield.Text;
                    Resource.Size = cmbSize.Text;
                    Resource.Speed = cmbSpeed.Text;
                    Resource.Amount = (int)nrAmount.Value;
                    MainForm.Instance.SectorForm.Value.RAListBox.Items.Insert(index, Resource);
                    MainForm.Instance.SectorForm.Value.RAListBox.SelectedItem = Resource;
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
