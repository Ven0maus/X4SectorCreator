using System.ComponentModel;
using X4SectorCreator.CustomComponents;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Factions
{
    public partial class ShipForm : Form
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

        private Ship _ship;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Ship Ship
        {
            get => _ship;
            set
            {
                _ship = value;
                BtnCreate.Text = "Update";
            }
        }

        private readonly MultiSelectCombo _mscCatTags, _mscCatFactions, _mscPilotTags;

        public ShipForm()
        {
            InitializeComponent();

            _mscCatTags = new MultiSelectCombo(CmbCatTags);
            _mscCatFactions = new MultiSelectCombo(CmbCatFactions);
            _mscPilotTags = new MultiSelectCombo(CmbPilotTags);
        }

        private void SetupComboboxValues()
        {
            var shipPresets = FactionShipsForm.ShipPresets
                .Values.SelectMany(a => a.Ship)
                .ToArray();

            var catSizes = shipPresets.Select(a => a.CategoryObj?.Size)
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var pilotFactions = shipPresets.Select(a => a.PilotObj?.Select?.Faction)
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var baskets = shipPresets.Select(a => a.BasketObj?.BasketValue)
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var drops = shipPresets.Select(a => a.DropObj?.Ref)
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var peoples = shipPresets.Select(a => a.PeopleObj?.Ref)
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var catTags = shipPresets.SelectMany(a => ParseMultiField(a.CategoryObj?.Tags))
                .Where(a => a != null).Append("plunder").ToHashSet(StringComparer.OrdinalIgnoreCase);
            var catFactions = shipPresets.SelectMany(a => ParseMultiField(a.CategoryObj?.Faction))
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var pilotTags = shipPresets.SelectMany(a => ParseMultiField(a.PilotObj?.Select?.Tags))
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Add custom factions and current faction to the faction comboboxes
            var customFactions = FactionsForm.AllCustomFactions.Select(a => a.Key)
                .Append(FactionShipsForm.Faction.Id)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var faction in customFactions)
            {
                catFactions.Add(faction);
                pilotFactions.Add(faction);
            }

            AddValues(CmbCatSize, catSizes);
            AddValues(CmbBasket, baskets);
            AddValues(CmbDrop, drops);
            AddValues(CmbPeople, peoples);
            AddValues(CmbCatTags, catTags);
            AddValues(CmbPilotTags, pilotTags);
            AddValues(CmbCatFactions, catFactions);
            AddValues(CmbPilotFaction, pilotFactions);

            _mscCatFactions.ReInit();
            _mscCatTags.ReInit();
            _mscPilotTags.ReInit();
        }

        private static void AddValues(ComboBox cmb, IEnumerable<string> values)
        {
            foreach (var value in values.Append("None").OrderBy(a => a))
                cmb.Items.Add(value);
        }

        private void InitShip(Ship ship)
        {
            TxtId.Text = ship.Id;
            TxtGroup.Text = ship.Group;
            CmbCatSize.SelectedItem = ship.CategoryObj?.Size ?? "None";
            CmbPilotFaction.SelectedItem = ship.PilotObj?.Select?.Faction ?? "None";
            CmbBasket.SelectedItem = ship.BasketObj?.BasketValue ?? "None";
            CmbDrop.SelectedItem = ship.DropObj?.Ref ?? "None";
            CmbPeople.SelectedItem = ship.PeopleObj?.Ref ?? "None";

            // Select multi's
            foreach (var value in ParseMultiField(ship.CategoryObj?.Tags).Where(a => a != null))
                _mscCatTags.Select(value);
            foreach (var value in ParseMultiField(ship.CategoryObj?.Faction).Where(a => a != null))
                _mscCatFactions.Select(value);
            foreach (var value in ParseMultiField(ship.PilotObj?.Select?.Tags).Where(a => a != null))
                _mscPilotTags.Select(value);
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtId.Text))
            {
                _ = MessageBox.Show("Please enter a valid ship id first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtGroup.Text))
            {
                _ = MessageBox.Show("Please enter a valid ship group first.");
                return;
            }

            switch (BtnCreate.Text)
            {
                case "Create":
                    // Validate if ship with this id doesn't already exist
                    if (FactionShipsForm.ShipsListBox.Items
                        .Cast<Ship>()
                        .Select(a => a.Id)
                        .Contains(TxtId.Text, StringComparer.OrdinalIgnoreCase))
                    {
                        _ = MessageBox.Show("A ship with this id already exists.");
                        return;
                    }

                    var ship = new Ship
                    {
                        Id = TxtId.Text,
                        Group = TxtGroup.Text
                    };
                    SetValues(ship);
                    FactionShipsForm.ShipsListBox.Items.Add(ship);
                    break;
                case "Update":
                    if (Ship.Id != TxtId.Text)
                    {
                        // Id was changed, check if it already exists
                        if (FactionShipsForm.ShipsListBox.Items
                            .Cast<Ship>()
                            .Select(a => a.Id)
                            .Contains(TxtId.Text, StringComparer.OrdinalIgnoreCase))
                        {
                            _ = MessageBox.Show("You modified the ship id, but a ship with this id already exists.");
                            return;
                        }
                    }

                    Ship.Id = TxtId.Text;
                    Ship.Group = TxtGroup.Text;
                    SetValues(Ship);
                    break;
            }

            Close();
        }

        private void SetValues(Ship ship)
        {
            // Set other properties
            if (SetValue(CmbBasket, out var value))
                ship.BasketObj = new Ship.Basket { BasketValue = value };
            if (SetValue(CmbDrop, out value))
                ship.DropObj = new Ship.Drop { Ref = value };
            if (SetValue(CmbPeople, out value))
                ship.PeopleObj = new Ship.People { Ref = value };
            if (SetValue(CmbCatSize, out value))
            {
                ship.CategoryObj ??= new Ship.Category();
                ship.CategoryObj.Size = value;
            }
            if (SetValue(CmbPilotFaction, out value))
            {
                ship.PilotObj ??= new Ship.Pilot();
                ship.PilotObj.Select ??= new Ship.Select();
                ship.PilotObj.Select.Faction = value;
            }
            if (_mscCatTags.SelectedItems.Count > 0)
            {
                var values = _mscCatTags.SelectedItems.Cast<string>().ToArray();
                ship.CategoryObj ??= new Ship.Category();
                ship.CategoryObj.Tags = values.Length > 1 ? $"[{string.Join(", ", values)}]" : values.First();
            }
            if (_mscCatFactions.SelectedItems.Count > 0)
            {
                var values = _mscCatFactions.SelectedItems.Cast<string>().ToArray();
                ship.CategoryObj ??= new Ship.Category();
                ship.CategoryObj.Faction = values.Length > 1 ? $"[{string.Join(", ", values)}]" : values.First();
            }
            if (_mscPilotTags.SelectedItems.Count > 0)
            {
                var values = _mscPilotTags.SelectedItems.Cast<string>().ToArray();
                ship.PilotObj ??= new Ship.Pilot();
                ship.PilotObj.Select ??= new Ship.Select();
                ship.PilotObj.Select.Tags = values.Length > 1 ? $"[{string.Join(", ", values)}]" : values.First();
            }
        }

        private static bool SetValue(ComboBox cmb, out string value)
        {
            value = cmb.SelectedItem as string ?? "None";
            if (!string.IsNullOrWhiteSpace(value) && !value.Equals("None", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static HashSet<string> ParseMultiField(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return [];

            // Remove brackets if present
            value = value.Trim();
            if (value.StartsWith('[') && value.EndsWith(']'))
            {
                value = value[1..^1];
            }

            // Split and add to HashSet
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in value.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                result.Add(r.Trim());
            }

            return result;
        }

        private void ShipForm_Load(object sender, EventArgs e)
        {
            // Init combobox values based on preset values
            SetupComboboxValues();

            if (Ship != null)
                InitShip(Ship);
        }

        private void BtnSelectCustomGroup_Click(object sender, EventArgs e)
        {
            var customGroups = FactionShipsForm.ShipGroupsListBox.Items
                .Cast<ShipGroup>()
                .Select(a => a.Name)
                .ToArray();
            if (customGroups.Length == 0)
            {
                _ = MessageBox.Show("You have no custom groups created.");
                return;
            }

            const string lblGroup = "Custom group:";
            Dictionary<string, string> data = MultiInputDialog.Show("Select custom group",
                (lblGroup, customGroups, null)
            );
            if (data == null || data.Count == 0)
                return;

            string group = data[lblGroup];
            if (string.IsNullOrWhiteSpace(group))
                return;

            TxtGroup.Text = group;
        }
    }
}
