using System.ComponentModel;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class ShipGroupsForm : Form
    {
        private FactionShipsForm _factionShipsForm;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionShipsForm FactionShipsForm
        {
            get => _factionShipsForm;
            set
            {
                _factionShipsForm = value;
            }
        }

        private ShipGroup _shipGroup;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShipGroup ShipGroup
        {
            get => _shipGroup;
            set
            {
                _shipGroup = value;
                BtnCreateGroup.Text = "Update";
                InitShipGroup(_shipGroup);
            }
        }

        private readonly LazyEvaluated<ShipMacrosForm> _shipMacrosForm = new(() => new ShipMacrosForm(), a => !a.IsDisposed);

        public ShipGroupsForm()
        {
            InitializeComponent();
        }

        private void InitShipGroup(ShipGroup shipGroup)
        {
            TxtGroupName.Text = shipGroup.Name;
            ShipMacroListBox.Items.Clear();
            foreach (var select in ShipGroup.SelectObj)
            {
                ShipMacroListBox.Items.Add(select);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnCreateGroup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtGroupName.Text))
            {
                _ = MessageBox.Show("Please enter a valid group name first.");
                return;
            }

            if (ShipMacroListBox.Items.Count == 0)
            {
                _ = MessageBox.Show("Group must have atleast one ship macro selected.");
                return;
            }

            switch (BtnCreateGroup.Text)
            {
                case "Create":
                    // Validate if group with this name doesn't already exist
                    if (FactionShipsForm.ShipGroupsListBox.Items
                        .Cast<ShipGroup>()
                        .Select(a => a.Name)
                        .Contains(TxtGroupName.Text, StringComparer.OrdinalIgnoreCase))
                    {
                        _ = MessageBox.Show("A shipgroup with this name already exists.");
                        return;
                    }

                    var group = new ShipGroup
                    {
                        Name = TxtGroupName.Text,
                        SelectObj = []
                    };
                    foreach (var select in ShipMacroListBox.Items.Cast<ShipGroup.Select>())
                        group.SelectObj.Add(select);

                    FactionShipsForm.ShipGroupsListBox.Items.Add(group);
                    break;
                case "Update":
                    if (ShipGroup.Name != TxtGroupName.Text)
                    {
                        // Name was changed, check if it already exists
                        if (FactionShipsForm.ShipGroupsListBox.Items
                            .Cast<ShipGroup>()
                            .Select(a => a.Name)
                            .Contains(TxtGroupName.Text, StringComparer.OrdinalIgnoreCase))
                        {
                            _ = MessageBox.Show("You modified the shipgroup name, but a shipgroup with this name already exists.");
                            return;
                        }
                    }

                    ShipGroup.Name = TxtGroupName.Text;
                    ShipGroup.SelectObj.Clear();
                    foreach (var select in ShipMacroListBox.Items.Cast<ShipGroup.Select>())
                        ShipGroup.SelectObj.Add(select);
                    break;
            }

            Close();
        }

        private void BtnAddCustomMacro_Click(object sender, EventArgs e)
        {
            const string lblMacro = "Macro:";
            const string lblWeight = "Weight:";
            Dictionary<string, string> data = MultiInputDialog.Show("Create ship macro",
                (lblMacro, null, null),
                (lblWeight, null, null)
            );
            if (data == null || data.Count == 0)
                return;

            string macro = data[lblMacro];
            string weight = data[lblWeight];
            if (string.IsNullOrWhiteSpace(macro) || string.IsNullOrWhiteSpace(weight))
            {
                _ = MessageBox.Show("Set a valid macro and weight first.");
                return;
            }

            var items = ShipMacroListBox.Items
                .Cast<ShipGroup.Select>()
                .Select(a => a.Macro)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (items.Contains(macro))
            {
                _ = MessageBox.Show("A macro with this name already exists.");
                return;
            }

            ShipMacroListBox.Items.Add(new ShipGroup.Select
            {
                Macro = macro,
                Weight = weight
            });
        }

        private void ButtonSelectVanillaMacros_Click(object sender, EventArgs e)
        {
            _shipMacrosForm.Value.ShipGroupsForm = this;
            _shipMacrosForm.Value.Show();
        }

        private void BtnDeleteMacro_Click(object sender, EventArgs e)
        {
            if (ShipMacroListBox.SelectedItem is ShipGroup.Select)
            {
                int index = ShipMacroListBox.Items.IndexOf(ShipMacroListBox.SelectedItem);
                ShipMacroListBox.Items.Remove(ShipMacroListBox.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ShipMacroListBox.SelectedItem = index >= 0 && ShipMacroListBox.Items.Count > 0 ?
                    ShipMacroListBox.Items[index] : null;
            }
        }
    }
}
