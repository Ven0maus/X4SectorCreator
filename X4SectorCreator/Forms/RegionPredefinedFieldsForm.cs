namespace X4SectorCreator.Forms
{
    public partial class RegionPredefinedFieldsForm : Form
    {
        private bool _suppressEvents = false;

        public RegionPredefinedFieldsForm()
        {
            InitializeComponent();

            // TODO: Read mapping and convert to object
            // TODO: Init each field with its mapping
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // TODO: Add field obj to listbox
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CmbAsteroids_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbAsteroids);
        }

        private void CmbNebula_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbNebula);
        }

        private void CmbVolumetricfog_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbVolumetricfog);
        }

        private void CmbObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbObjects);
        }

        private void CmbGravidar_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbGravidar);
        }

        private void CmbPositional_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbPositional);
        }

        private void ResetAllExcept(ComboBox cmb)
        {
            if (_suppressEvents) return;
            _suppressEvents = true;
            var fields = new List<ComboBox>
            {
                cmbAsteroids,
                cmbNebula,
                cmbObjects,
                cmbVolumetricfog,
                cmbPositional,
                cmbGravidar
            };
            fields.Remove(cmb);
            foreach (var field in fields)
                field.SelectedItem = null;
            _suppressEvents = false;
        }
    }
}
