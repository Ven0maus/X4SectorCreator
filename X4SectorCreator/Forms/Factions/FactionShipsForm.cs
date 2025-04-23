using System.ComponentModel;
using X4SectorCreator.Forms.Factions;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactionShipsForm : Form
    {
        private Faction _faction;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Faction Faction
        {
            get => _faction;
            set
            {
                _faction = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionForm FactionForm { get; set; }

        private static Dictionary<string, ShipGroups> _shipGroupPresets;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static Dictionary<string, ShipGroups> ShipGroupPresets
        {
            get
            {
                if (_shipGroupPresets == null)
                {
                    _shipGroupPresets = new(StringComparer.OrdinalIgnoreCase);

                    var presets = Directory.GetFiles(Path.Combine(Application.StartupPath, $"Data/Presets/ShipGroups"), "*.xml");
                    foreach (var preset in presets)
                    {
                        var shipGroups = ShipGroups.Deserialize(File.ReadAllText(preset));
                        var fileName = Path.GetFileName(preset);
                        _shipGroupPresets.Add(fileName.Split('_')[0], shipGroups);
                    }
                }
                return _shipGroupPresets;
            }
        }

        private static Dictionary<string, Ships> _shipPresets;
        public static Dictionary<string, Ships> ShipPresets
        {
            get
            {
                if (_shipPresets == null)
                {
                    _shipPresets = new(StringComparer.OrdinalIgnoreCase);

                    var presets = Directory.GetFiles(Path.Combine(Application.StartupPath, $"Data/Presets/Ships"), "*.xml");
                    foreach (var preset in presets)
                    {
                        var ships = Ships.Deserialize(File.ReadAllText(preset));
                        var fileName = Path.GetFileName(preset);
                        _shipPresets.Add(fileName.Split('_')[0], ships);
                    }
                }
                return _shipPresets;
            }
        }

        private readonly LazyEvaluated<ShipGroupsForm> _shipGroupsForm = new(() => new ShipGroupsForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<ShipForm> _shipForm = new(() => new ShipForm(), a => !a.IsDisposed);

        public FactionShipsForm()
        {
            InitializeComponent();
        }

        private void BtnUseFactionPreset_Click(object sender, EventArgs e)
        {
            if (ShipGroupsListBox.Items.Count > 0 || ShipsListBox.Items.Count > 0)
            {
                if (MessageBox.Show("Selecting a preset will reset the current selection, are you sure?",
                    "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            // Reset current selection
            ShipGroupsListBox.Items.Clear();
            ShipsListBox.Items.Clear();

            var factions = ShipGroupPresets.Select(a => a.Key).OrderBy(a => a).ToArray();

            const string lblFaction = "Faction:";
            Dictionary<string, string> data = MultiInputDialog.Show("Select faction preset",
                (lblFaction, factions, factions.First())
            );
            if (data == null || data.Count == 0)
                return;

            string faction = data[lblFaction];
            if (string.IsNullOrWhiteSpace(faction))
            {
                _ = MessageBox.Show("Select a valid faction first.");
                return;
            }

            // 1. Load ShipGroups preset
            var shipGroups = ShipGroupPresets[faction];
            var exists = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var shipGroup in shipGroups.Group)
            {
                var newGroup = shipGroup.Clone();
                newGroup.Name = $"{Faction.Id}_{string.Join("_", shipGroup.Name.Split('_').Skip(1))}";

                if (exists.Contains(newGroup.Name))
                    throw new Exception("Found shipgroup dupe: " + newGroup.Name);
                exists.Add(newGroup.Name);

                ShipGroupsListBox.Items.Add(newGroup);
            }
            exists.Clear();
            // 2. Load Ships preset
            var ships = ShipPresets[faction];
            foreach (var ship in ships.Ship)
            {
                var newShip = ship.Clone();
                newShip.Id = $"{Faction.Id}_{string.Join("_", ship.Id.Split('_').Skip(1))}";
                if (ship.Group != null)
                    newShip.Group = $"{Faction.Id}_{string.Join("_", ship.Group.Split('_').Skip(1))}";

                if (newShip.CategoryObj != null)
                    newShip.CategoryObj.Faction = Faction.Id;
                if (newShip.PilotObj != null && newShip.PilotObj.Select != null)
                    newShip.PilotObj.Select.Faction = Faction.Id;
                ShipsListBox.Items.Add(newShip);

                if (exists.Contains(newShip.Id))
                    throw new Exception("Found ship dupe: " + newShip.Id);
                exists.Add(newShip.Id);
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ShipGroupsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (ShipGroupsListBox.SelectedItem is ShipGroup shipGroup)
            {
                _shipGroupsForm.Value.FactionShipsForm = this;
                _shipGroupsForm.Value.ShipGroup = shipGroup;
                _shipGroupsForm.Value.Show();
            }
        }

        private void BtnCreateGroup_Click(object sender, EventArgs e)
        {
            _shipGroupsForm.Value.FactionShipsForm = this;
            _shipGroupsForm.Value.Show();
        }

        private void BtnDeleteGroup_Click(object sender, EventArgs e)
        {
            if (ShipGroupsListBox.SelectedItem is ShipGroup)
            {
                int index = ShipGroupsListBox.Items.IndexOf(ShipGroupsListBox.SelectedItem);
                ShipGroupsListBox.Items.Remove(ShipGroupsListBox.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ShipGroupsListBox.SelectedItem = index >= 0 && ShipGroupsListBox.Items.Count > 0 ?
                    ShipGroupsListBox.Items[index] : null;
            }
        }

        private void BtnDeleteShip_Click(object sender, EventArgs e)
        {
            if (ShipsListBox.SelectedItem is Ship)
            {
                int index = ShipsListBox.Items.IndexOf(ShipsListBox.SelectedItem);
                ShipsListBox.Items.Remove(ShipsListBox.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ShipsListBox.SelectedItem = index >= 0 && ShipsListBox.Items.Count > 0 ?
                    ShipsListBox.Items[index] : null;
            }
        }

        private void BtnCreateShip_Click(object sender, EventArgs e)
        {
            _shipForm.Value.FactionShipsForm = this;
            _shipForm.Value.Show();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            FactionForm.ShipGroups = ShipGroupsListBox.Items.Cast<ShipGroup>().ToList();
            FactionForm.Ships = ShipsListBox.Items.Cast<Ship>().ToList();
            Close();
        }

        private void FactionShipsForm_Load(object sender, EventArgs e)
        {
            bool isNameModified = FactionForm.Faction != null && Faction.Id != FactionForm.Faction.Id;

            if (FactionForm.ShipGroups != null)
            {
                foreach (var shipGroup in FactionForm.ShipGroups.Select(a => a.Clone()))
                {
                    if (isNameModified)
                        UpdateShipsAndGroups(shipGroup, FactionForm.Faction.Id, Faction.Id);
                    ShipGroupsListBox.Items.Add(shipGroup);
                }
            }
            if (FactionForm.Ships != null)
            {
                foreach (var ship in FactionForm.Ships.Select(a => a.Clone()))
                {
                    if (isNameModified)
                        UpdateShipsAndGroups(ship, FactionForm.Faction.Id, Faction.Id);
                    ShipsListBox.Items.Add(ship);
                }
            }
        }

        private static void UpdateShipsAndGroups(object obj, string old, string @new)
        {
            if (obj is Ship ship)
            {
                if (ship.Id.StartsWith($"{old}_", StringComparison.OrdinalIgnoreCase))
                {
                    ship.Id = ship.Id.Replace($"{old}_", $"{@new}_");

                    if (ship.Group != null && ship.Group.StartsWith($"{old}_", StringComparison.OrdinalIgnoreCase))
                    {
                        ship.Group = ship.Group.Replace($"{old}_", $"{@new}_");
                    }

                    // Update factions
                    if (ship.PilotObj?.Select?.Faction != null &&
                        ship.PilotObj.Select.Faction.Equals(old, StringComparison.OrdinalIgnoreCase))
                    {
                        ship.PilotObj.Select.Faction = @new;
                    }
                    if (ship.CategoryObj?.Faction != null)
                    {
                        var factions = ParseMultiField(ship.CategoryObj.Faction);
                        if (factions.Remove(old))
                        {
                            factions.Add(@new);
                            ship.CategoryObj.Faction = factions.Count == 0 ? null : "[" + string.Join(",", factions) + "]";
                        }
                    }
                }
            }
            else if (obj is ShipGroup shipgroup)
            {
                if (shipgroup.Name.StartsWith($"{old}_", StringComparison.OrdinalIgnoreCase))
                {
                    shipgroup.Name = shipgroup.Name.Replace($"{old}_", $"{@new}_");
                }
            }
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

        private void BtnClearAllGroups_Click(object sender, EventArgs e)
        {
            ShipGroupsListBox.Items.Clear();
        }

        private void BtnClearAllShips_Click(object sender, EventArgs e)
        {
            ShipsListBox.Items.Clear();
        }

        private void ShipsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (ShipsListBox.SelectedItem is Ship ship)
            {
                _shipForm.Value.FactionShipsForm = this;
                _shipForm.Value.Ship = ship;
                _shipForm.Value.Show();
            }
        }
    }
}
