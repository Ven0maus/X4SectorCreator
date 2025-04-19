using System.ComponentModel;

namespace X4SectorCreator.Forms.Factions
{
    public partial class FactionStationForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionForm FactionForm { get; set; }

        public FactionStationForm()
        {
            InitializeComponent();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (SelectedStationTypesListBox.Items.Count == 0)
            {
                FactionForm.StationTypes = null;
                return;
            }

            FactionForm.StationTypes = SelectedStationTypesListBox.Items.Cast<string>().ToList();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (AvailableStationTypesListBox.SelectedItem is string station &&
                !string.IsNullOrWhiteSpace(station) &&
                !SelectedStationTypesListBox.Items.Contains(station))
            {
                SelectedStationTypesListBox.Items.Add(station);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (SelectedStationTypesListBox.SelectedItem is string station &&
                !string.IsNullOrWhiteSpace(station))
            {
                SelectedStationTypesListBox.Items.Remove(station);
            }
        }

        private void AvailableStationTypesListBox_DoubleClick(object sender, EventArgs e)
        {
            BtnAdd.PerformClick();
        }

        private void SelectedStationTypesListBox_DoubleClick(object sender, EventArgs e)
        {
            BtnRemove.PerformClick();
        }
    }
}
