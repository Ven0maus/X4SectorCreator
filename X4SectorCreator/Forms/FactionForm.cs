﻿using System.ComponentModel;
using System.Drawing.Imaging;
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

        private readonly Dictionary<string, string> _licenseNameMapping = new()
        {
            { "capitalequipment", "Capital Equipment License" },
            { "capitalship", "Capital Ship License" },
            { "ceremonyally", "Ally" },
            { "ceremonyfriend", "Friend" },
            { "generaluseequipment", "General Use Equipment Licence" },
            { "generaluseship", "General Use Ship Licence" },
            { "militaryequipment", "Military Equipment License" },
            { "militaryship", "Military Ship License" },
            { "police", "Police License" },
            { "shipsalecontract", "Ship Sale Contract" },
            { "station_equip_lxl", "Capital Ship Building Module Licence" },
            { "station_equip_sm", "Ancillary Ship Building Module Licence" },
            { "station_gen_advanced", "Advanced Module Licence" },
            { "station_gen_basic", "Basic Module License" },
            { "station_gen_intermediate", "Intermediate Module Licence" },
            { "tradesubscription", "Trade Offer Subscription" }
        };

        private readonly Dictionary<string, HashSet<string>> _tagPresets = new()
        {
            { "Main faction", new HashSet<string> { "claimspace", "economic", "police", "privateloadout", "privateship", "protective", "publicloadout", "publicship", "standard", "watchdoguser" } },
            { "Pirate faction", new HashSet<string> { "economic", "pirate", "plunder", "privateloadout", "privateship", "protective", "watchdoguser" } },
        };

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color? FactionColor { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string IconData { get; set; }

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
                    FactionColor = _faction.Color;
                    IconData = _faction.IconData;
                    IconBox.Image = Base64ToImage(IconData);
                    ApplyFactionXmlToFieldsContent();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionsForm FactionsForm { get; set; }

        public FactionForm()
        {
            InitializeComponent();

            var factions = FactionsForm.GetAllFactions(true);
            foreach (var faction in factions)
                CmbPoliceFaction.Items.Add(faction);
        }

        public void SetFactionXml(Faction faction)
        {
            _factionXml = faction.Serialize();
        }

        private void BtnPickColor_Click(object sender, EventArgs e)
        {
            using ColorDialog colorDialog = new();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                BtnPickColor.BackColor = FactionColor ??= colorDialog.Color;
                BtnPickColor.ForeColor = colorDialog.Color.GetBrightness() < 0.5 ? Color.White : Color.Black;
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            // First apply content of fields to xml
            if (!ApplyFieldsContentToFactionXml())
                return;

            // Validate color
            if (FactionColor == null)
            {
                _ = MessageBox.Show("Faction Color must be set.");
                return;
            }

            // Validate icon
            if (string.IsNullOrWhiteSpace(IconData))
            {
                _ = MessageBox.Show("Faction Icon must be set.");
                return;
            }

            var faction = Faction.Deserialize(_factionXml);
            faction.Color = FactionColor.Value;
            faction.IconData = IconData;

            switch (BtnCreate.Text)
            {
                case "Create":
                    // Verify if faction id already exists
                    if (FactionsForm.GetAllFactions(true, false).Contains(faction.Id))
                    {
                        _ = MessageBox.Show("A faction with this name already exists.");
                        return;
                    }
                    FactionsForm.AllCustomFactions.Add(faction.Id, faction);
                    break;
                case "Update":
                    if (faction.Id != Faction.Id)
                    {
                        // Verify if faction id already exists
                        if (FactionsForm.GetAllFactions(true, false).Contains(faction.Id))
                        {
                            _ = MessageBox.Show("You modified the faction name, but a faction with this name already exists.");
                            return;
                        }
                    }

                    FactionsForm.AllCustomFactions.Remove(Faction.Id);
                    FactionsForm.AllCustomFactions.Add(faction.Id, faction);
                    Faction = faction;
                    break;
            }

            // Update values
            FactionsForm.InitFactionValues();
            Close();
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

            // Adjust license names if they're still placeholder
            var faction = Faction.Deserialize(_factionXml);
            if (faction.Licences?.Licence != null)
            {
                bool updated = false;
                if (faction.Licences.Licence.All(a => !a.Name.Equals("Placeholder", StringComparison.OrdinalIgnoreCase)) &&
                    faction.Licences.Licence.Any(a => !a.Name.StartsWith(faction.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    if (MessageBox.Show("Update license names?", "Update license names?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        UpdateLicenseNames(faction, false);
                        updated = true;
                    }
                }

                if (!updated)
                {
                    UpdateLicenseNames(faction, true);
                }
            }

            // Show form with the XML
            _factionXmlForm.Value.FactionForm = this;
            _factionXmlForm.Value.TxtFactionXml.Text = _factionXml;
            _factionXmlForm.Value.Show();
        }

        private void UpdateLicenseNames(Faction faction, bool onlyPlaceholders)
        {
            foreach (var license in faction.Licences.Licence)
            {
                if (_licenseNameMapping.TryGetValue(license.Type, out var name))
                {
                    if (!onlyPlaceholders)
                    {
                        license.Name = $"{faction.Name} {name}";
                        continue;
                    }

                    if (license.Name.Equals("Placeholder", StringComparison.OrdinalIgnoreCase))
                    {
                        license.Name = $"{faction.Name} {name}";
                    }
                }
            }
            _factionXml = faction.Serialize();
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

            // Set color (not part of serialized data)
            BtnPickColor.BackColor = FactionColor.Value;
            BtnPickColor.ForeColor = BtnPickColor.BackColor.GetBrightness() < 0.5 ? Color.White : Color.Black;
        }

        private bool ApplyFieldsContentToFactionXml()
        {
            // Validate if all fields have correct information to be converted
            var name = Sanitize(TxtFactionName.Text, true, false);
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
            shortName = shortName.ToUpper();

            var prefix = Sanitize(TxtPrefix.Text, convertLowercase: false);
            if (string.IsNullOrWhiteSpace(prefix))
            {
                _ = MessageBox.Show("Please first provide a valid faction prefix.");
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
            faction.PoliceFaction = policeFaction;
            faction.Description = TxtDescription.Text;
            faction.Tags = string.Join(" ", TagsListBox.Items.Cast<string>().Select(a => a.ToLower()));

            // Re-serialize into xml string
            _factionXml = faction.Serialize();

            return true;
        }

        private void BtnFactionRelations_Click(object sender, EventArgs e)
        {
            _factionRelationsForm.Value.FactionForm = this;
            _factionRelationsForm.Value.Faction = Faction.Deserialize(_factionXml);
            _factionRelationsForm.Value.Show();
        }

        private void BtnAddTag_Click(object sender, EventArgs e)
        {
            const string lblTag = "Tag:";
            Dictionary<string, string> data = MultiInputDialog.Show("Create New Tag",
                (lblTag, null, null)
            );
            if (data == null || data.Count == 0)
                return;

            string tag = data[lblTag];
            if (string.IsNullOrWhiteSpace(tag)) 
                return;

            if (TagsListBox.Items.Cast<string>().Any(a => a.Equals(tag, StringComparison.InvariantCultureIgnoreCase)))
                return;

            TagsListBox.Items.Add(tag);
        }

        private void BtnDeleteTag_Click(object sender, EventArgs e)
        {
            if (TagsListBox.SelectedItem is string tag && !string.IsNullOrWhiteSpace(tag))
            {
                int index = TagsListBox.Items.IndexOf(TagsListBox.SelectedItem);
                TagsListBox.Items.Remove(TagsListBox.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                TagsListBox.SelectedItem = index >= 0 && TagsListBox.Items.Count > 0 ?
                    TagsListBox.Items[index] : null;
            }
        }

        private void BtnUseTagsPreset_Click(object sender, EventArgs e)
        {
            const string lblPreset = "Preset:";
            Dictionary<string, string> data = MultiInputDialog.Show("Select Tags Preset",
                (lblPreset, _tagPresets.Keys.ToArray(), _tagPresets.Keys.First())
            );
            if (data == null || data.Count == 0)
                return;

            string preset = data[lblPreset];
            if (string.IsNullOrWhiteSpace(preset))
                return;

            TagsListBox.Items.Clear();
            foreach (var tag in _tagPresets[preset])
                TagsListBox.Items.Add(tag);
        }

        private void BtnSetIcon_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "PNG files (*.png)|*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                Image image = Image.FromFile(filePath);

                if (image.Width != 256 && image.Height != 256)
                {
                    _ = MessageBox.Show("Image size must be 256x256 please try upload another image.");
                    return;
                }

                // Optional: Display the image
                IconBox.Image = image;
                IconBox.SizeMode = PictureBoxSizeMode.StretchImage;

                // Convert to Base64
                string base64String = ImageToBase64(image, ImageFormat.Png);

                // Store in your object
                IconData = base64String;
            }
        }

        private static string ImageToBase64(Image image, ImageFormat format)
        {
            using MemoryStream ms = new();
            image.Save(ms, format);
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        private static Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using MemoryStream ms = new(imageBytes);
            return Image.FromStream(ms);
        }

        public static string Sanitize(string text, bool allowWhitespace = false, bool convertLowercase = true)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Remove all non-alphanumeric characters (including whitespace)
            string cleaned = (!allowWhitespace ? SanitizeRegex() : SanitizeRegexAllowWhitespace()).Replace(text, "").Trim();

            // Convert to lowercase
            return convertLowercase ? cleaned.ToLower() : cleaned;
        }

        [GeneratedRegex(@"[^a-zA-Z0-9]")]
        private static partial Regex SanitizeRegex();

        [GeneratedRegex(@"[^a-zA-Z0-9\s]")]
        private static partial Regex SanitizeRegexAllowWhitespace();
    }
}
