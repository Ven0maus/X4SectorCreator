using X4SectorCreator.Helpers;
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
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
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
    }
}
