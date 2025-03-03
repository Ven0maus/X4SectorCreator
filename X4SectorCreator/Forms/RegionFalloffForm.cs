using X4SectorCreator.Objects;
using System.ComponentModel;

namespace X4SectorCreator.Forms
{
    public partial class RegionFalloffForm : Form
    {
        private StepObj _stepObj;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StepObj StepObj
        {
            get => _stepObj;
            set
            {
                _stepObj = value;
                if (_stepObj != null)
                {
                    txtPosition.Text = _stepObj.Position;
                    txtValue.Text = _stepObj.Value;
                    BtnAdd.Text = "Update";
                }
            }
        }

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

            switch (BtnAdd.Text)
            {
                case "Add":
                    HandleAdditionStep(posValue, valueValue);
                    break;
                case "Update":
                    HandleUpdateStep(posValue, valueValue);
                    break;
            }

            Close();
        }

        private static void HandleAdditionStep(float posValue, float valueValue)
        {
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
        }

        private void HandleUpdateStep(float posValue, float valueValue)
        {
            // Determine which tab is active
            var regionForm = MainForm.Instance.RegionForm;
            var sT = regionForm.TabControlFalloff.SelectedTab;

            StepObj.Position = posValue.ToString("0.0");
            StepObj.Value = valueValue.ToString("0.0");

            int index;
            switch (sT.Name)
            {
                case "tabLateral":
                    index = regionForm.ListBoxLateral.SelectedIndex;
                    regionForm.ListBoxLateral.Items.Remove(StepObj);
                    regionForm.ListBoxLateral.Items.Insert(index, StepObj);
                    regionForm.ListBoxLateral.SelectedItem = StepObj;
                    break;
                case "tabRadial":
                    index = regionForm.ListBoxRadial.SelectedIndex;
                    regionForm.ListBoxRadial.Items.Remove(StepObj);
                    regionForm.ListBoxRadial.Items.Insert(index, StepObj);
                    regionForm.ListBoxRadial.SelectedItem = StepObj;
                    break;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
