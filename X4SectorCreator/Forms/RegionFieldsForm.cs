namespace X4SectorCreator.Forms
{
    public partial class RegionFieldsForm : Form
    {
        public RegionFieldsForm()
        {
            InitializeComponent();
        }

        public class FieldObj
        {
            public override string ToString()
            {
                // TODO
                return base.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // TODO: Validation

            var regionForm = MainForm.Instance.RegionForm;

            // Add new field object
            var newField = new FieldObj
            {
                // TODO
            };

            regionForm.ListBoxFields.Items.Add(newField);
            regionForm.ListBoxFields.SelectedItem = newField;

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
