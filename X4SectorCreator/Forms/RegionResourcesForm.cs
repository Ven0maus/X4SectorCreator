using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class RegionResourcesForm : Form
    {
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

            // Add resource to listbox
            var resource = new Resource
            {
                Ware = cmbWare.Text,
                Yield = cmbYield.Text,
            };
            MainForm.Instance.RegionForm.ListBoxResources.Items.Add(resource);

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
