using System.ComponentModel;
using X4SectorCreator.CustomComponents;
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
                InitShip(value);
            }
        }

        private readonly MultiSelectCombo _mscCatTags, _mscCatFactions, _mscPilotTags;

        public ShipForm()
        {
            InitializeComponent();

            // Init combobox values based on preset values
            SetupComboboxValues();

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
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var catFactions = shipPresets.SelectMany(a => ParseMultiField(a.CategoryObj?.Faction))
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var pilotTags = shipPresets.SelectMany(a => ParseMultiField(a.PilotObj?.Select?.Tags))
                .Where(a => a != null).ToHashSet(StringComparer.OrdinalIgnoreCase);

            AddValues(CmbCatSize, catSizes);
            AddValues(CmbPilotFaction, pilotFactions);
            AddValues(CmbBasket, baskets);
            AddValues(CmbDrop, drops);
            AddValues(CmbPeople, peoples);
            AddValues(CmbCatTags, catTags);
            AddValues(CmbCatFactions, catFactions);
            AddValues(CmbPilotTags, pilotTags);
        }

        private static void AddValues(ComboBox cmb, IEnumerable<string> values)
        {
            foreach (var value in values.OrderBy(a => a))
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
            switch (BtnCreate.Text)
            {
                case "Create":
                    break;
                case "Update":
                    break;
            }
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
    }
}
