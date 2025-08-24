using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;

namespace X4SectorCreator.Forms
{
    public partial class FactionsForm : Form
    {
        public static readonly Dictionary<string, Faction> AllCustomFactions = new(StringComparer.OrdinalIgnoreCase);
        private readonly LazyEvaluated<FactionForm> _factionForm = new(() => new FactionForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<Factions.FactionCreationHelpForm> _factionCreationHelpForm = new(() => new Factions.FactionCreationHelpForm(), a => !a.IsDisposed);
        public FactionsForm()
        {
            InitializeComponent();
            InitFactionValues();
        }

        public void InitFactionValues()
        {
            CustomFactionsListBox.Items.Clear();
            foreach (var faction in AllCustomFactions.Values.OrderBy(a => a.Name))
            {
                CustomFactionsListBox.Items.Add(faction);
            }
        }

        public static Color GetColorForFaction(string faction, bool checkClaimSpace = true)
        {
            // First check for custom faction
            var customFaction = AllCustomFactions.Values.FirstOrDefault(a => a.Id
                .Equals(faction, StringComparison.OrdinalIgnoreCase));
            if (customFaction != null)
            {
                // Only when faction claims space
                if (!checkClaimSpace || customFaction.Tags.Contains("claimspace", StringComparison.OrdinalIgnoreCase))
                    return customFaction.Color;
                return MainForm.Instance.FactionColorMapping["None"];
            }

            // Then for vanilla faction
            if (MainForm.Instance.FactionColorMapping.TryGetValue(faction, out Color value))
                return value;

            // Attempt to reverse some X4 faction names to readable names for lookup
            faction = GodGeneration.CorrectFactionNameReversed(faction);
            if (MainForm.Instance.FactionColorMapping.TryGetValue(faction, out value))
                return value;

            // If not found, then "ownerless"
            return MainForm.Instance.FactionColorMapping["None"];
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
                AllCustomFactions.Remove(selectedFaction.Id);

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

        private void BtnFactionCreationHelp_Click(object sender, EventArgs e)
        {
            _factionCreationHelpForm.Value.Show();
        }
    }
}
