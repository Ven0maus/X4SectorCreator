using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactionXmlForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionForm FactionForm { get; set; }

        public FactionXmlForm()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var faction = Faction.Deserialize(TxtFactionXml.Text);
                faction.Color = FactionForm.FactionColor.Value;
                faction.Icon = FactionForm.IconData;
                var dataEntryName = $"faction_{faction.Id}";
                faction.ColorData = new Faction.ColorDataObj { Ref = dataEntryName };
                faction.IconData = new Faction.IconObj { Active = dataEntryName, Inactive = dataEntryName };

                if (!ValidateFactionFields(faction)) return;
                FactionForm.Faction = faction;
                Close();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Invalid xml: {ex.Message}", "Invalid xml");
                return;
            }
        }

        private static bool ValidateFactionFields(Faction faction)
        {
            // Validate if all fields have correct information to be converted
            var id = (FactionForm.Sanitize(faction.Id, true) ?? "").Trim().ToLower().Replace(" ", "_");
            if (string.IsNullOrWhiteSpace(id))
            {
                _ = MessageBox.Show("Please first provide a valid faction id.");
                return false;
            }

            var name = faction.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                _ = MessageBox.Show("Please first provide a valid faction name.");
                return false;
            }

            var shortName = FactionForm.Sanitize(faction.Shortname);
            if (string.IsNullOrWhiteSpace(shortName) || shortName.Length != 3)
            {
                _ = MessageBox.Show("Please first provide a valid faction shortname. (must be 3 characters long)");
                return false;
            }

            var prefix = FactionForm.Sanitize(faction.Prefixname);
            if (string.IsNullOrWhiteSpace(prefix))
            {
                _ = MessageBox.Show("Please first provide a valid faction prefix.");
                return false;
            }

            var race = FactionForm.Sanitize(faction.Primaryrace);
            if (string.IsNullOrWhiteSpace(race))
            {
                _ = MessageBox.Show("Please first provide a valid race.");
                return false;
            }

            return true;
        }
    }
}
