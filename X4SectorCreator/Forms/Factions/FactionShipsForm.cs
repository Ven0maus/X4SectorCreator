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

        private readonly LazyEvaluated<ShipGroupsForm> _shipGroupsForm = new(() => new ShipGroupsForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<ShipForm> _shipForm = new(() => new ShipForm(), a => !a.IsDisposed);

        public FactionShipsForm()
        {
            InitializeComponent();
            InitPresets();
        }

        private static void InitPresets()
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
            foreach (var shipGroup in shipGroups.Group)
            {
                var newGroup = shipGroup.Clone();
                newGroup.Name = $"{Faction.Id}_{string.Join("_", shipGroup.Name.Split('_').Skip(1))}";
                ShipGroupsListBox.Items.Add(newGroup);
            }

            // 2. Load Ships preset
            var ships = _shipPresets[faction];
            foreach (var ship in ships.Ship)
            {
                var newShip = ship.Clone();
                newShip.Id = $"{Faction.Id}_{string.Join("_", ship.Id.Split('_').Skip(1))}";
                newShip.Group = $"{Faction.Id}_{string.Join("_", ship.Group.Split('_').Skip(1))}";
                ShipsListBox.Items.Add(newShip);
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

        private void ShipsListBox_SelectedIndexChanged(object sender, EventArgs e)
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
