using System.Text.RegularExpressions;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactionForm : Form
    {
        public FactionForm()
        {
            InitializeComponent();
        }

        private void BtnPickColor_Click(object sender, EventArgs e)
        {
            using ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                BtnPickColor.BackColor = colorDialog.Color;
                BtnPickColor.ForeColor = colorDialog.Color.GetBrightness() < 0.5 ? Color.White : Color.Black;
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            var factionId = SanitizeId(TxtFactionName.Text);
            if (string.IsNullOrEmpty(factionId))
            {
                _ = MessageBox.Show("Please provide a valid faction name.");
                return;
            }

            switch (BtnCreate.Text)
            {
                case "Create":
                    var faction = new Faction
                    {
                        Id = factionId,
                        Name = TxtFactionName.Text,
                    };
                    FactionsForm.AllCustomFactions.Add(factionId, faction);
                    break;
                case "Update":
                    break;
            }
        }

        private static string SanitizeId(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Remove all non-alphanumeric characters (including whitespace)
            string cleaned = SanitizeRegex().Replace(text, "");

            // Convert to lowercase
            return cleaned.ToLower();
        }

        [GeneratedRegex(@"[^a-zA-Z0-9]")]
        private static partial Regex SanitizeRegex();
    }
}
