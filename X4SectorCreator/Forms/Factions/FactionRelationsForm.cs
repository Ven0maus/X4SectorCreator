using System.ComponentModel;
using System.Globalization;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;

namespace X4SectorCreator.Forms
{
    public partial class FactionRelationsForm : Form
    {
        private Faction _faction;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Faction Faction
        {
            get => _faction;
            set
            {
                _faction = value;
                CmbSelectedFaction.Enabled = true;
                FactionRelationsForm_Load(null, null);

                if (_faction == null) return;

                if (!CmbSelectedFaction.Items.Contains(_faction.Id))
                    CmbSelectedFaction.Items.Add(_faction.Id);

                CmbSelectedFaction.SelectedItem = _faction.Name;
                CmbSelectedFaction.Enabled = false;
            }
        }

        private static Dictionary<string, Dictionary<string, int>> _factionRelations = [];
        private readonly Dictionary<string, Dictionary<string, int>> _unsavedChanges = [];

        public FactionRelationsForm()
        {
            InitializeComponent();
        }

        static FactionRelationsForm()
        {
            // Init the initial relations
            var factions = GetFactions();
            foreach (var faction in factions)
            {
                var dict = new Dictionary<string, int>();
                _factionRelations[faction] = dict;
                foreach (var otherFaction in factions)
                {
                    if (otherFaction == faction) continue;
                    dict[otherFaction] = 0;
                }
            }
        }

        /// <summary>
        /// Returns all faction relations for XML generation.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, Dictionary<string, int>> GetFactionRelations()
        {
            return _factionRelations;
        }

        /// <summary>
        /// Used to init faction relations from a save file.
        /// </summary>
        /// <param name="factionRelations"></param>
        public static void LoadFactionRelations(Dictionary<string, Dictionary<string, int>> factionRelations)
        {
            _factionRelations = factionRelations;
        }

        public static void InsertFaction(Faction faction)
        {
            Dictionary<string, int> relations = GetFactions()
                .Where(a => a != faction.Id)
                .ToDictionary(a => a, a => 0);
            _factionRelations[faction.Id] = relations;

            // Overwrite existing values
            if (faction.Relations?.Relation != null)
            {
                foreach (var rel in faction.Relations.Relation)
                {
                    var factionName = GodGeneration.CorrectFactionNameReversed(rel.Faction);
                    if (relations.ContainsKey(factionName)) 
                    {
                        var relValue = ConvertRelationToUIValue(rel.RelationValue, out bool isNegative);
                        relations[factionName] = isNegative ? -relValue : relValue;
                    }
                }
            }

            // Insert in other factions
            foreach (var kvp in _factionRelations)
            {
                if (kvp.Key == faction.Id) continue;

                kvp.Value[faction.Id] = _factionRelations[faction.Id][kvp.Key];
            }
        }

        public static void DeleteFaction(Faction faction)
        {
            foreach (var kvp in _factionRelations)
            {
                var relations = kvp.Value;
                relations.Remove(faction.Id);
            }
            _factionRelations.Remove(faction.Id);

            // Apply also to custom faction relation objects
            foreach (var customFaction in FactionsForm.AllCustomFactions.Values)
            {
                HandleCustomFactionFormsRelations(customFaction, _factionRelations[customFaction.Id]);
            }
        }

        private static int ConvertRelationToUIValue(string value, out bool isNegative)
        {
            isNegative = false;
            if (!double.TryParse(value, CultureInfo.InvariantCulture, out var result)) return 0;
            isNegative = result < 0;
            return (int)Math.Round((10 * Math.Log10(Math.Abs(result) * 1000)), 0);
        }

        private static double ConvertUIToRelationValue(int value, out bool isNegative)
        {
            isNegative = value < 0;
            double relation = Math.Pow(10, Math.Abs(value) / 10.0) / 1000.0;
            double delta = relation * (Math.Pow(10, 1.0 / 10.0) - 1.0); // relation step to next UI
            int decimalPlaces = (int)Math.Ceiling(-Math.Log10(delta));
            return Math.Round(relation, decimalPlaces);
        }

        private static HashSet<string> GetFactions()
        {
            return [.. FactionsForm.GetAllFactions(true)
                .Append("criminal")
                .Append("smuggler")
                .Select(GodGeneration.CorrectFactionNameReversed)];
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // Apply unsaved changes
            foreach (var kvp in _unsavedChanges)
            {
                var relations = _factionRelations[kvp.Key];
                foreach (var change in kvp.Value)
                {
                    relations[change.Key] = change.Value;
                }
            }

            // Apply also to custom faction relation objects
            foreach (var customFaction in FactionsForm.AllCustomFactions.Values)
            {
                HandleCustomFactionFormsRelations(customFaction, _factionRelations[customFaction.Id]);
            }

            // Clear after changes are processed
            _unsavedChanges.Clear();

            Close();
        }

        private static void HandleCustomFactionFormsRelations(Faction target, Dictionary<string, int> relations)
        {
            target.Relations ??= new Faction.RelationsObj();
            if (target.Relations.Relation == null)
                target.Relations.Relation = [];

            // Remove relations that were removed
            target.Relations.Relation.RemoveAll(a => !relations.ContainsKey(a.Faction));

            // Set all relations for this faction properly
            foreach (var relation in relations)
            {
                var factionName = GodGeneration.CorrectFactionName(relation.Key);
                var faction = target.Relations.Relation
                    .FirstOrDefault(a => a.Faction.Equals(factionName, StringComparison.OrdinalIgnoreCase));
                if (faction == null)
                {
                    if (relation.Value == 0)
                    {
                        var oldRelation = target.Relations.Relation.FirstOrDefault(a => a.Faction.Equals(factionName));
                        if (oldRelation != null)
                            target.Relations.Relation.Remove(oldRelation);
                    }
                    else
                    {
                        var relValue = ConvertUIToRelationValue(relation.Value, out bool isNegative);
                        target.Relations.Relation.Add(new Faction.Relation
                        {
                            Faction = factionName,
                            RelationValue = isNegative ? 
                                (-relValue).ToString(CultureInfo.InvariantCulture) : 
                                relValue.ToString(CultureInfo.InvariantCulture)
                        });
                    }
                }
                else
                {
                    if (relation.Value == 0)
                    {
                        target.Relations.Relation.Remove(faction);
                        continue;
                    }
                    var relValue = ConvertUIToRelationValue(relation.Value, out bool isNegative);
                    faction.RelationValue = isNegative ?
                                (-relValue).ToString(CultureInfo.InvariantCulture) :
                                relValue.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private void CmbSelectedFaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedFaction = (string)CmbSelectedFaction.SelectedItem;

            RelationsPanel.SuspendLayout();  // improves redraw performance
            RelationsPanel.Controls.Clear();

            // Get relations of selected faction
            if (!_factionRelations.TryGetValue(selectedFaction, out var factionRelations))
            {
                RelationsPanel.ResumeLayout();
                // This cannot happen naturally
                _ = MessageBox.Show($"Faction \"{selectedFaction}\" no longer exists.");
                return;
            }

            // Apply unsaved changes if present
            if (_unsavedChanges.TryGetValue(selectedFaction, out var unsavedChanges))
            {
                // Make a copy as to not modify the original
                factionRelations = factionRelations.ToDictionary(a => a.Key, a => a.Value);
                foreach (var kvp in unsavedChanges)
                {
                    factionRelations[kvp.Key] = kvp.Value;
                }
            }

            foreach (var kvp in factionRelations.OrderBy(a => a.Key))
            {
                if (kvp.Key == selectedFaction) continue;

                var panel = new Panel
                {
                    Width = RelationsPanel.ClientSize.Width - 25,
                    Height = 30,
                    Margin = new Padding(2)
                };

                var lbl = new Label { Text = kvp.Key, Width = 100, Left = 5, Top = 7 };
                var track = new TrackBar
                {
                    Minimum = -30,
                    Maximum = 30,
                    TickStyle = TickStyle.None,
                    Value = factionRelations[kvp.Key],
                    Width = 250,
                    Left = 110,
                    Top = 3,
                    Height = 25
                };
                track.MouseWheel += (s, e) => ((HandledMouseEventArgs)e).Handled = true;
                track.BackColor = track.Value < 0 ? Color.LightCoral : Color.LightGreen;
                track.ValueChanged += (s, e) => 
                {
                    if (track.Value < 0)
                        track.BackColor = Color.LightCoral;   // reddish
                    else
                        track.BackColor = Color.LightGreen;   // greenish
                };
                var lblValue = new Label { Text = track.Value.ToString(), Left = 370, Top = 7, Width = 30 };

                track.Scroll += (s, ev) =>
                {
                    lblValue.Text = track.Value.ToString();

                    // Adjust on sender end
                    if (!_unsavedChanges.TryGetValue(selectedFaction, out var relations))
                        _unsavedChanges[selectedFaction] = relations = [];
                    relations[kvp.Key] = track.Value;

                    // Also adjust on the receiving end as well
                    if (!_unsavedChanges.TryGetValue(kvp.Key, out relations))
                        _unsavedChanges[kvp.Key] = relations = [];
                    relations[selectedFaction] = track.Value;
                };

                panel.Controls.Add(lbl);
                panel.Controls.Add(track);
                panel.Controls.Add(lblValue);

                RelationsPanel.Controls.Add(panel);
            }

            RelationsPanel.ResumeLayout();
        }

        private void FactionRelationsForm_Load(object sender, EventArgs e)
        {
            if (CmbSelectedFaction.Enabled == false) return;
            var prevSelected = (string)CmbSelectedFaction.SelectedItem;

            // Reset
            CmbSelectedFaction.Items.Clear();
            foreach (var faction in _factionRelations.Keys.OrderBy(a => a))
                CmbSelectedFaction.Items.Add(faction);

            if (prevSelected != null && CmbSelectedFaction.Items.Contains(prevSelected))
                CmbSelectedFaction.SelectedItem = prevSelected;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}