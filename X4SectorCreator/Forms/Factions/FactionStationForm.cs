using System.ComponentModel;
using X4SectorCreator.CustomComponents;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Factions
{
    public partial class FactionStationForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionForm FactionForm { get; set; }

        private readonly MultiSelectCombo _mscHqTypes;

        public FactionStationForm()
        {
            InitializeComponent();

            _mscHqTypes = new MultiSelectCombo(CmbHQTypes);
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            FactionForm.StationTypes = SelectedStationTypesListBox.Items.Count == 0 ? null :
                SelectedStationTypesListBox.Items.Cast<string>().ToList();

            FactionForm.PreferredHqTypes = _mscHqTypes.SelectedItems.Count == 0 ? null :
                _mscHqTypes.SelectedItems.Cast<string>().ToList();

            if (FactionForm.StationTypes == null)
            {
                _ = MessageBox.Show("Please select atleast one station type.");
                return;
            }

            if (FactionForm.PreferredHqTypes == null)
            {
                _ = MessageBox.Show("Please select atleast one preferred HQ type.");
                return;
            }

            Close();
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

                // Add selected option
                CmbHQTypes.Items.Add(station);
                var selectedValues = _mscHqTypes.SelectedItems.ToList();
                _mscHqTypes.ResetSelection();
                _mscHqTypes.ReInit();
                foreach (var selectedValue in selectedValues)
                    _mscHqTypes.Select(selectedValue);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (SelectedStationTypesListBox.SelectedItem is string station &&
                !string.IsNullOrWhiteSpace(station))
            {
                SelectedStationTypesListBox.Items.Remove(station);

                // Remove selected option
                CmbHQTypes.Items.Remove(station);
                var selectedValues = _mscHqTypes.SelectedItems.ToList();
                _mscHqTypes.ResetSelection();
                _mscHqTypes.ReInit();
                foreach (var selectedValue in selectedValues)
                    _mscHqTypes.Select(selectedValue);
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

        private void FactionStationForm_Load(object sender, EventArgs e)
        {
            if (FactionForm.StationTypes != null)
            {
                foreach (var stationType in FactionForm.StationTypes)
                {
                    SelectedStationTypesListBox.Items.Add(stationType);

                    // Add selected option
                    CmbHQTypes.Items.Add(stationType);
                }
                _mscHqTypes.ResetSelection();
                _mscHqTypes.ReInit();
            }

            if (FactionForm.PreferredHqTypes != null)
            {
                foreach (var preferredHqType in FactionForm.PreferredHqTypes)
                    _mscHqTypes.Select(preferredHqType);
            }
        }
    }
}
