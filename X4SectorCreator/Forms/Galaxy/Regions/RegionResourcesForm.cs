using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class RegionResourcesForm : Form
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
                    BtnAdd.Text = "Update";
                }
            }
        }

        public RegionResourcesForm()
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

            switch (BtnAdd.Text)
            {
                case "Add":
                    Resource resource = new()
                    {
                        Ware = cmbWare.Text,
                        Yield = cmbYield.Text,
                    };
                    _ = MainForm.Instance.RegionForm.Value.RegionDefinitionsForm.Value.RegionDefinitionForm.Value.ListBoxResources.Items.Add(resource);
                    break;
                case "Update":
                    int index = MainForm.Instance.RegionForm.Value.RegionDefinitionsForm.Value.RegionDefinitionForm.Value.ListBoxResources.SelectedIndex;
                    MainForm.Instance.RegionForm.Value.RegionDefinitionsForm.Value.RegionDefinitionForm.Value.ListBoxResources.Items.Remove(Resource);
                    Resource.Ware = cmbWare.Text;
                    Resource.Yield = cmbYield.Text;
                    MainForm.Instance.RegionForm.Value.RegionDefinitionsForm.Value.RegionDefinitionForm.Value.ListBoxResources.Items.Insert(index, Resource);
                    MainForm.Instance.RegionForm.Value.RegionDefinitionsForm.Value.RegionDefinitionForm.Value.ListBoxResources.SelectedItem = Resource;
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
