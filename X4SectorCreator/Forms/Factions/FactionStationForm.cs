using System.ComponentModel;
using X4SectorCreator.CustomComponents;

namespace X4SectorCreator.Forms.Factions
{
    public partial class FactionStationForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionForm FactionForm { get; set; }

        private readonly MultiSelectCombo _mscHqTypes;
        private readonly Dictionary<string, TextBox> _desiredBoxes;

        public FactionStationForm()
        {
            InitializeComponent();

            _desiredBoxes = new(StringComparer.OrdinalIgnoreCase)
            {
                { "shipyard", TxtDesiredShipyards },
                { "wharf", TxtDesiredWharfs },
                { "equipmentdock", TxtDesiredEquipmentDocks },
                { "tradestation", TxtDesiredTradeStations },
            };

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

            FactionForm.DesiredShipyards = AsInteger(TxtDesiredShipyards.Text).ToString();
            FactionForm.DesiredWharfs = AsInteger(TxtDesiredWharfs.Text).ToString();
            FactionForm.DesiredEquipmentDocks = AsInteger(TxtDesiredEquipmentDocks.Text).ToString();
            FactionForm.DesiredTradeStations = AsInteger(TxtDesiredTradeStations.Text).ToString();

            Close();
        }

        private static int AsInteger(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out var nr))
                return 0;
            return nr;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static string GetHQTypeBasedOnStationType(string stationType)
        {
            return stationType.ToLower() switch
            {
                "wharf" or "shipyard" => "shipbuilding",
                "defence" => "defencestation",
                "piratedock" or "freeport" => "piratebase",
                _ => stationType,
            };
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (AvailableStationTypesListBox.SelectedItem is string station &&
                !string.IsNullOrWhiteSpace(station) &&
                !SelectedStationTypesListBox.Items.Contains(station))
            {
                SelectedStationTypesListBox.Items.Add(station);

                // Add selected option
                var hqType = GetHQTypeBasedOnStationType(station);
                if (!CmbHQTypes.Items.Contains(hqType))
                    CmbHQTypes.Items.Add(hqType);

                var selectedValues = _mscHqTypes.SelectedItems.ToList();
                _mscHqTypes.ResetSelection();
                _mscHqTypes.ReInit();
                foreach (var selectedValue in selectedValues)
                    _mscHqTypes.Select(selectedValue);

                // Set value to 1 atleast if its 0
                if (_desiredBoxes.TryGetValue(station, out var box))
                {
                    if (string.IsNullOrWhiteSpace(box.Text) || box.Text == "0")
                        box.Text = "1";
                }
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (SelectedStationTypesListBox.SelectedItem is string station &&
                !string.IsNullOrWhiteSpace(station))
            {
                SelectedStationTypesListBox.Items.Remove(station);

                // Remove selected option
                var hqType = GetHQTypeBasedOnStationType(station);
                CmbHQTypes.Items.Remove(hqType);
                var selectedValues = _mscHqTypes.SelectedItems.Cast<string>()
                    .Where(a => !a.Equals(hqType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                _mscHqTypes.ResetSelection();
                _mscHqTypes.ReInit();
                foreach (var selectedValue in selectedValues)
                    _mscHqTypes.Select(selectedValue);

                // Set value back to 0
                if (_desiredBoxes.TryGetValue(station, out var box))
                {
                    if (!string.IsNullOrWhiteSpace(box.Text) && box.Text != "0")
                        box.Text = "0";
                }
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
            CmbHQTypes.Items.Add("any");

            if (FactionForm.StationTypes != null)
            {
                foreach (var stationType in FactionForm.StationTypes)
                {
                    SelectedStationTypesListBox.Items.Add(stationType);

                    // Add selected option
                    var hqType = GetHQTypeBasedOnStationType(stationType);
                    if (!CmbHQTypes.Items.Contains(hqType))
                        CmbHQTypes.Items.Add(hqType);
                }
                _mscHqTypes.ResetSelection();
                _mscHqTypes.ReInit();
            }

            if (FactionForm.PreferredHqTypes != null)
            {
                foreach (var preferredHqType in FactionForm.PreferredHqTypes)
                    _mscHqTypes.Select(preferredHqType);
            }

            TxtDesiredTradeStations.Text = FactionForm.DesiredTradeStations ?? "0";
            TxtDesiredShipyards.Text = FactionForm.DesiredShipyards ?? "0";
            TxtDesiredEquipmentDocks.Text = FactionForm.DesiredEquipmentDocks ?? "0";
            TxtDesiredWharfs.Text = FactionForm.DesiredWharfs ?? "0";
        }
    }
}
