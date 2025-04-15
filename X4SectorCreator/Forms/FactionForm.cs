using System.ComponentModel;
using System.Text.RegularExpressions;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Color = System.Drawing.Color;

namespace X4SectorCreator.Forms
{
    public partial class FactionForm : Form
    {
        private readonly LazyEvaluated<FactionXmlForm> _factionXmlForm = new(() => new FactionXmlForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<FactionRelationsForm> _factionRelationsForm = new(() => new FactionRelationsForm(), a => !a.IsDisposed);
        private Color? _factionColor = null;
        private string _factionXml = @"<faction id=""placeholder"" name=""placeholder"" description=""placeholder"" shortname=""placeholder"" prefixname=""placeholder"" primaryrace=""argon"" behaviourset=""default"" tags="""">
    <color ref=""placeholder"" />
    <icon active=""placeholder"" inactive=""placeholder"" />
    <licences>
      <licence type=""capitalequipment"" name=""placeholder"" icon=""bse_star"" minrelation=""0.1"" precursor=""ceremonyally"" />
      <licence type=""capitalship"" name=""placeholder"" icon=""bse_star"" minrelation=""0.1"" precursor=""ceremonyally"" />
      <licence type=""ceremonyally"" name=""placeholder"" minrelation=""0.1"" />
      <licence type=""ceremonyfriend"" name=""placeholder"" minrelation=""0.01"" />
      <licence type=""generaluseequipment"" name=""placeholder"" minrelation=""-0.01"" tags=""basic"" />
      <licence type=""generaluseship"" name=""placeholder"" minrelation=""-0.01"" tags=""basic"" />
      <licence type=""militaryequipment"" name=""placeholder"" icon=""bse_star"" minrelation=""0.01"" precursor=""ceremonyfriend"" />
      <licence type=""militaryship"" name=""placeholder"" icon=""bse_star"" minrelation=""0.01"" precursor=""ceremonyfriend"" />
      <licence type=""police"" name=""placeholder"" description=""placeholder"" minrelation=""0.01"" precursor=""ceremonyfriend"" price=""156000"" maxlegalscan=""2"" />
      <licence type=""shipsalecontract"" name=""placeholder"" description=""placeholder"" minrelation=""0.1"" precursor=""ceremonyally"" price=""1"" tags=""hidden"" />
      <licence type=""station_equip_lxl"" name=""placeholder"" minrelation=""0.1"" precursor=""ceremonyally"" />
      <licence type=""station_equip_sm"" name=""placeholder"" minrelation=""0.1"" precursor=""ceremonyally"" />
      <licence type=""station_gen_advanced"" name=""placeholder"" minrelation=""0.1"" precursor=""ceremonyally"" />
      <licence type=""station_gen_basic"" name=""placeholder"" minrelation=""-0.01"" tags=""basic"" />
      <licence type=""station_gen_intermediate"" name=""placeholder"" minrelation=""0.01"" precursor=""ceremonyfriend"" />
      <licence type=""tradesubscription"" name=""placeholder"" description=""placeholder"" minrelation=""0.1"" precursor=""ceremonyally"" price=""10000000"" />
    </licences>
    <relations>
	  <relation faction=""xenon"" relation=""-1"" />
	  <relation faction=""khaak"" relation=""-1"" />
      <relation faction=""buccaneers"" relation=""-0.032"" />
      <relation faction=""criminal"" relation=""-0.5"" />
      <relation faction=""scaleplate"" relation=""-0.32"" />
      <relation faction=""smuggler"" relation=""-0.06"" />
	  <relation faction=""fallensplit"" relation=""-0.32""/>
	  <relation faction=""loanshark"" relation=""-0.032""/>
	  <relation faction=""yaki"" relation=""-0.032""/>
    </relations>
  </faction>";

        private Faction _faction;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Faction Faction
        {
            get => _faction;
            set
            {
                _faction = value;
                if (_faction != null)
                {
                    _factionXml = _faction.Serialize();
                    ApplyFactionXmlToFieldsContent();
                }
            }
        }

        public FactionForm()
        {
            InitializeComponent();

            var factions = MainForm.Instance.FactionColorMapping.Keys
                .Where(a => !a.Equals("None", StringComparison.OrdinalIgnoreCase))
                .Concat(FactionsForm.AllCustomFactions.Keys)
                .Select(a => a.ToLower())
                .OrderBy(a => a)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var faction in factions)
                CmbPoliceFaction.Items.Add(faction);
        }

        private void BtnPickColor_Click(object sender, EventArgs e)
        {
            using ColorDialog colorDialog = new();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                BtnPickColor.BackColor = _factionColor ??= colorDialog.Color;
                BtnPickColor.ForeColor = colorDialog.Color.GetBrightness() < 0.5 ? Color.White : Color.Black;
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            // First apply content of fields to xml
            if (!ApplyFieldsContentToFactionXml())
                return;

            switch (BtnCreate.Text)
            {
                case "Create":
                    var faction = Faction.Deserialize(_factionXml);
                    FactionsForm.AllCustomFactions.Add(faction.Id, faction);
                    break;
                case "Update":
                    break;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnEditXml_Click(object sender, EventArgs e)
        {
            // Apply all field data to the XML
            if (!ApplyFieldsContentToFactionXml())
                return;

            // Show form with the XML
            _factionXmlForm.Value.FactionForm = this;
            _factionXmlForm.Value.TxtFactionXml.Text = _factionXml;
            _factionXmlForm.Value.Show();
        }

        public void ApplyFactionXmlToFieldsContent()
        {
            var faction = Faction.Deserialize(_factionXml);

            // Main fields
            TxtFactionName.Text = faction.Name.Trim();
            TxtShortName.Text = faction.Shortname;
            TxtPrefix.Text = faction.Prefixname;
            CmbRace.SelectedItem = faction.Primaryrace;
            CmbPoliceFaction.SelectedItem = faction.PoliceFaction ?? "None";
            TxtDescription.Text = faction.Description;

            // Add tags
            TagsListBox.Items.Clear();
            foreach (var tag in (faction.Tags ?? "").Split(' '))
                TagsListBox.Items.Add(tag);

            // Set color
            _factionColor = faction.Color;
            BtnPickColor.BackColor = _factionColor.Value;
            BtnPickColor.ForeColor = BtnPickColor.BackColor.GetBrightness() < 0.5 ? Color.White : Color.Black;
        }

        private bool ApplyFieldsContentToFactionXml()
        {
            // Validate if all fields have correct information to be converted
            var name = Sanitize(TxtFactionName.Text, true);
            if (string.IsNullOrWhiteSpace(name))
            {
                _ = MessageBox.Show("Please first provide a valid faction name.");
                return false;
            }

            var shortName = Sanitize(TxtShortName.Text);
            if (string.IsNullOrWhiteSpace(shortName) || shortName.Length != 3)
            {
                _ = MessageBox.Show("Please first provide a valid faction shortname. (must be 3 characters long)");
                return false;
            }

            var prefix = Sanitize(TxtPrefix.Text);
            if (string.IsNullOrWhiteSpace(prefix))
            {
                _ = MessageBox.Show("Please first provide a valid faction prefix.");
                return false;
            }

            if (_factionColor == null)
            {
                _ = MessageBox.Show("Please first provide a valid faction color.");
                return false;
            }

            var policeFaction = CmbPoliceFaction.SelectedItem as string;
            if (policeFaction.Equals("None", StringComparison.OrdinalIgnoreCase))
                policeFaction = null;

            var faction = Faction.Deserialize(_factionXml);

            // Update Fields
            faction.Id = name.Trim().ToLower().Replace(" ", "_");
            faction.Name = name;
            faction.Shortname = shortName;
            faction.Prefixname = prefix;
            faction.Primaryrace = (CmbRace.SelectedItem as string).ToLower();
            faction.Color = _factionColor.Value;
            faction.PoliceFaction = policeFaction;
            faction.Description = TxtDescription.Text;
            faction.Tags = string.Join(" ", TagsListBox.Items.Cast<string>().Select(a => a.ToLower()));

            // Re-serialize into xml string
            _factionXml = faction.Serialize();

            return true;
        }

        private void BtnFactionRelations_Click(object sender, EventArgs e)
        {
            _factionRelationsForm.Value.Show();
        }

        public static string Sanitize(string text, bool allowWhitespace = false)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Remove all non-alphanumeric characters (including whitespace)
            string cleaned = (!allowWhitespace ? SanitizeRegex() : SanitizeRegexAllowWhitespace()).Replace(text, "").Trim();

            // Convert to lowercase
            return cleaned.ToLower();
        }

        [GeneratedRegex(@"[^a-zA-Z0-9]")]
        private static partial Regex SanitizeRegex();

        [GeneratedRegex(@"[^a-zA-Z0-9\s]")]
        private static partial Regex SanitizeRegexAllowWhitespace();
    }
}
