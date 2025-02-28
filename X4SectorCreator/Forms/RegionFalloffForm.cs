using System.Globalization;

namespace X4SectorCreator.Forms
{
    public partial class RegionFalloffForm : Form
    {
        public RegionFalloffForm()
        {
            InitializeComponent();
        }

        public class StepObj
        {
            public float Position { get; set; }
            public float Value { get; set; }

            public override string ToString()
            {
                return $"[pos=\"{Position}\" val=\"{Value}\"]";
            }
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
                Position = posValue,
                Value = valueValue
            };

            switch (sT.Name)
            {
                case "tabLateral":
                    regionForm.ListBoxLateral.Items.Add(stepObj);
                    break;
                case "tabRadial":
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
