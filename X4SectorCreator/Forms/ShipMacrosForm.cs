using X4SectorCreator.Helpers;
using System.ComponentModel;
using X4SectorCreator.Objects;
using System.Text.Json;

namespace X4SectorCreator.Forms
{
    public partial class ShipMacrosForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShipGroupsForm ShipGroupsForm { get; set; }

        private List<ShipMacro> _shipMacros = [];

        public ShipMacrosForm()
        {
            InitializeComponent();
            InitShipMacros();

            var factions = FactionsForm.GetAllFactions(false, false);
            foreach (var faction in factions)
                CmbFaction.Items.Add(faction);
            CmbFaction.Items.Add("any");
            CmbFaction.SelectedItem = "any";

            TxtSearch.EnableTextSearch(_shipMacros, a => a.Name, ApplyFilter);
            Disposed += ShipMacrosForm_Disposed;

            ApplyFilter();
        }

        private void InitShipMacros()
        {
            _shipMacros = FactionShipsForm.ShipGroupPresets
                .SelectMany(a => a.Value.Group.Select(b => new ShipMacro(b.Name, a.Key)))
                .ToList();

            ApplyFilter();
        }

        private void ApplyFilter(List<ShipMacro> macros = null)
        {
            var shipMacros = macros ?? _shipMacros;

            if (CmbFaction.SelectedItem is string faction && 
                !faction.Equals("any", StringComparison.OrdinalIgnoreCase))
            {
                shipMacros = shipMacros
                    .Where(a => a.Faction.Equals(faction, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ShipMacroListBox.Items.Clear();
            foreach (var macro in shipMacros.Select(a => a.Name))
                ShipMacroListBox.Items.Add(macro);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (ShipMacroListBox.SelectedItem is string macro &&
                !string.IsNullOrWhiteSpace(macro) &&
                !SelectedShipMacroListBox.Items.Contains(macro))
            {
                SelectedShipMacroListBox.Items.Add(macro);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (SelectedShipMacroListBox.SelectedItem is string macro &&
                !string.IsNullOrWhiteSpace(macro))
            {
                SelectedShipMacroListBox.Items.Remove(macro);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ShipMacrosForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            var items = ShipGroupsForm.ShipMacroListBox.Items
                .Cast<ShipGroup.Select>()
                .Select(a => a.Macro)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newItems = SelectedShipMacroListBox.Items
                .Cast<string>()
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var item in newItems)
            {
                if (!items.Contains(item))
                {
                    ShipGroupsForm.ShipMacroListBox.Items.Add(new ShipGroup.Select { Macro = item, Weight = "100" });
                }
            }

            Close();
        }

        private void ShipMacroListBox_DoubleClick(object sender, EventArgs e)
        {
            BtnAdd.PerformClick();
        }

        private void SelectedShipMacroListBox_DoubleClick(object sender, EventArgs e)
        {
            BtnRemove.PerformClick();
        }

        private void CmbFaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            TxtSearch.GetTextSearchComponent()?.ForceCalculate();
        }

        class ShipMacro(string name, string faction)
        {
            public string Name { get; set; } = name;
            public string Faction { get; set; } = faction;
        }
    }
}
