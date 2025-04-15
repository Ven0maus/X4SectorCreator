﻿using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactionsForm : Form
    {
        public static readonly Dictionary<string, Faction> AllCustomFactions = [];
        private readonly LazyEvaluated<FactionForm> _factionForm = new(() => new FactionForm(), a => !a.IsDisposed);

        public FactionsForm()
        {
            InitializeComponent();
            InitFactionValues();
        }

        public void InitFactionValues()
        {
            CustomFactionsListBox.Items.Clear();
            foreach (var faction in AllCustomFactions.Values.OrderBy(a => a))
            {
                CustomFactionsListBox.Items.Add(faction);
            }
        }

        public static HashSet<string> GetAllFactions(bool includeCustom, bool includeOwnerless = false)
        {
            var factions = MainForm.Instance.FactionColorMapping.Keys
                .Where(a => !a.Equals("None", StringComparison.OrdinalIgnoreCase));

            if (includeCustom)
                factions = factions.Concat(AllCustomFactions.Keys);
            if (includeOwnerless)
                factions = factions.Append("Ownerless");

            return factions
                .Select(a => a.ToLower())
                .OrderBy(a => a)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            _factionForm.Value.FactionsForm = this;
            _factionForm.Value.BtnCreate.Text = "Create";
            _factionForm.Value.Show();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (CustomFactionsListBox.SelectedItem is Faction selectedFaction)
            {
                AllCustomFactions.Remove(selectedFaction.Name);

                int index = CustomFactionsListBox.Items.IndexOf(CustomFactionsListBox.SelectedItem);
                CustomFactionsListBox.Items.Remove(CustomFactionsListBox.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                CustomFactionsListBox.SelectedItem = index >= 0 && CustomFactionsListBox.Items.Count > 0 ?
                    CustomFactionsListBox.Items[index] : null;
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CustomFactionsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (CustomFactionsListBox.SelectedItem is Faction faction)
            {
                _factionForm.Value.FactionsForm = this;
                _factionForm.Value.Faction = faction;
                _factionForm.Value.BtnCreate.Text = "Update";
                _factionForm.Value.Show();
            }
        }
    }
}
