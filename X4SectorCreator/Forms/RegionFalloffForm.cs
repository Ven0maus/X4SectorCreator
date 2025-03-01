using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class RegionFalloffForm : Form
    {
        public RegionFalloffForm()
        {
            InitializeComponent();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPosition.Text) || !float.TryParse(txtPosition.Text, out var posValue))
            {
                _ = MessageBox.Show("Please enter a valid numerical \"position\".");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtValue.Text) || !float.TryParse(txtValue.Text, out var valueValue))
            {
                _ = MessageBox.Show("Please enter a valid numerical \"value\".");
                return;
            }

            // Determine which tab is active
            var regionForm = MainForm.Instance.RegionForm;
            var sT = regionForm.TabControlFalloff.SelectedTab;

            var stepObj = new StepObj
            {
                Position = posValue.ToString("0.0"),
                Value = valueValue.ToString("0.0")
            };

            switch (sT.Name)
            {
                case "tabLateral":
                    stepObj.Type = "Lateral";
                    regionForm.ListBoxLateral.Items.Add(stepObj);
                    break;
                case "tabRadial":
                    stepObj.Type = "Radial";
                    regionForm.ListBoxRadial.Items.Add(stepObj);
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
