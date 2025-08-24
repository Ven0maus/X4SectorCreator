using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using X4SectorCreator.Configuration;
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

                RelationsPanel.VerticalScroll.Value = 0;
                RelationsPanel.PerformLayout(); // ensures the panel refreshes

                if (_faction == null) return;

                if (!CmbSelectedFaction.Items.Contains(_faction.Id))
                    CmbSelectedFaction.Items.Add(_faction.Id);

                CmbSelectedFaction.SelectedItem = _faction.Id;
                CmbSelectedFaction.Enabled = false;
            }
        }

        private static Dictionary<string, bool> _factionsLocked = [];
        private static Dictionary<string, Dictionary<string, int>> _factionRelations = [];

        private readonly Dictionary<string, Dictionary<string, int>> _unsavedChanges = [];
        private readonly static Dictionary<string, bool> _unsavedFactionsLocked = [];

        public FactionRelationsForm()
        {
            InitializeComponent();
        }

        static FactionRelationsForm()
        {
            // Init the initial relations
            var defaultRelations = GetDefaultRelations();

            foreach (var kvp in defaultRelations)
            {
                var dict = new Dictionary<string, int>();
                _factionRelations[kvp.Key] = dict;
                _factionsLocked[kvp.Key] = kvp.Value.Locked != null && kvp.Value.Locked == "1";

                foreach (var otherFaction in defaultRelations)
                {
                    if (otherFaction.Key == kvp.Key) continue;

                    var value = otherFaction.Value.Relation.FirstOrDefault(a => a.Faction == kvp.Key);
                    if (value == null)
                    {
                        dict[otherFaction.Key] = 0;
                        continue;
                    }

                    dict[otherFaction.Key] = ConvertRelationToUIValue(value.RelationValue);
                }
            }
        }

        /// <summary>
        /// Returns only the modified faction relations for XML Generation compared to the default relations.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, Dictionary<string, double>> GetModifiedFactionRelations()
        {
            var relations = new Dictionary<string, Dictionary<string, double>>();
            // Same as relations but contains the original values
            var originalRelations = GetDefaultRelations()
                .ToDictionary(a => a.Key, a =>
                    new Dictionary<string, double>(a.Value.Relation
                        .Select(b => new KeyValuePair<string, double>(b.Faction,
                        ConvertUIToRelationValue(ConvertRelationToUIValue(b.RelationValue))))));

            foreach (var relation in _factionRelations)
            {
                var dict = new Dictionary<string, double>();
                foreach (var innerRelation in relation.Value)
                {
                    dict[innerRelation.Key] = ConvertUIToRelationValue(innerRelation.Value);
                }
                relations[relation.Key] = dict;
            }

            // Keep only modified relations
            var modified = new Dictionary<string, Dictionary<string, double>>();
            foreach (var faction in relations)
            {
                if (!originalRelations.TryGetValue(faction.Key, out var originalFactionRelations))
                    continue;

                Dictionary<string, double> diffs = null;
                foreach (var inner in faction.Value)
                {
                    if ((!originalFactionRelations.TryGetValue(inner.Key, out var origValue)
                        || !inner.Value.Equals(origValue)) && inner.Value != 0) // different or missing
                    {
                        diffs ??= [];
                        diffs[inner.Key] = inner.Value;
                    }
                }

                if (diffs != null && diffs.Count > 0)
                {
                    modified[faction.Key] = diffs;
                }
            }

            return modified;
        }

        /// <summary>
        /// Returns all locked states.
        /// </summary>
        /// <param name="factionA"></param>
        /// <returns></returns>
        public static Dictionary<string, bool> GetModifiedRelationLockStates()
        {
            var original = GetDefaultRelations().ToDictionary(a => a.Key, a => a.Value.Locked != null && a.Value.Locked == "1");
            var modified = new Dictionary<string, bool>();
            foreach (var value in _factionsLocked)
            {
                if (!original.TryGetValue(value.Key, out var originalValue) && value.Value)
                    modified[value.Key] = value.Value;
                else if (originalValue != value.Value)
                    modified[value.Key] = value.Value;
            }
            return modified;
        }

        /// <summary>
        /// Used to init faction relations from a save file.
        /// </summary>
        /// <param name="factionRelations"></param>
        /// <param name="_factionLockedRelations"></param>
        public static void LoadFactionRelations(Dictionary<string, Dictionary<string, int>> factionRelations, Dictionary<string, bool> _factionLockedRelations)
        {
            _factionRelations = factionRelations;
            _factionsLocked = _factionLockedRelations;
        }

        /// <summary>
        /// Call when a new custom faction is created.
        /// </summary>
        /// <param name="faction"></param>
        public static void InsertFaction(Faction faction)
        {
            var relations = new Dictionary<string, int>();
            _factionRelations[faction.Id] = relations;
            _factionsLocked[faction.Id] = false;

            // Init default relations all to 0 include all custom factions except itself
            var defaultRelations = GetDefaultRelations()
                .Keys
                .Concat(FactionsForm.AllCustomFactions.Keys)
                .Where(a => a != faction.Id)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var defaultRelation in defaultRelations)
            {
                relations[defaultRelation] = 0;
            }

            // Overwrite existing default values from custom faction
            if (faction.Relations?.Relation != null)
            {
                foreach (var rel in faction.Relations.Relation)
                {
                    var factionName = GodGeneration.CorrectFactionName(rel.Faction);
                    if (relations.ContainsKey(factionName))
                    {
                        relations[factionName] = ConvertRelationToUIValue(rel.RelationValue);
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

        /// <summary>
        /// Call when a custom faction is deleted.
        /// </summary>
        /// <param name="faction"></param>
        public static void DeleteFaction(Faction faction)
        {
            foreach (var kvp in _factionRelations)
            {
                var relations = kvp.Value;
                relations.Remove(faction.Id);
            }
            _factionRelations.Remove(faction.Id);
            _factionsLocked.Remove(faction.Id);

            // Apply also to custom faction relation objects
            foreach (var customFaction in FactionsForm.AllCustomFactions.Values)
                HandleCustomFactionFormsRelations(customFaction, _factionRelations[customFaction.Id], _factionsLocked[customFaction.Id]);
        }

        private static int ConvertRelationToUIValue(string value)
        {
            if (!double.TryParse(value, CultureInfo.InvariantCulture, out var result) || result == 0) return 0;
            bool isNegative = result < 0;
            var final = (int)Math.Round((10 * Math.Log10(Math.Abs(result) * 1000)), 0);
            return isNegative ? -final : final;
        }

        private static double ConvertUIToRelationValue(int value)
        {
            if (value == 0) return 0;
            bool isNegative = value < 0;
            double relation = Math.Pow(10, Math.Abs(value) / 10.0) / 1000.0;
            double delta = relation * (Math.Pow(10, 1.0 / 10.0) - 1.0); // relation step to next UI
            int decimalPlaces = (int)Math.Ceiling(-Math.Log10(delta));
            var roundedValue = Math.Round(relation, decimalPlaces);
            return isNegative ? -roundedValue : roundedValue;
        }

        private static Dictionary<string, Faction.RelationsObj> _vanillaRelationsCached;
        public static Dictionary<string, Faction.RelationsObj> GetDefaultRelations()
        {
            if (_vanillaRelationsCached != null) return _vanillaRelationsCached;
            string relationsJson = File.ReadAllText(Constants.DataPaths.VanillaRelationsMappingFilePath);
            return _vanillaRelationsCached ??= JsonSerializer.Deserialize<Dictionary<string, Faction.RelationsObj>>(relationsJson, ConfigSerializer.JsonSerializerOptions);
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

            // Apply unsaved locked state changes
            foreach (var kvp in _unsavedFactionsLocked)
                _factionsLocked[kvp.Key] = kvp.Value;

            // Apply also to custom faction relation objects
            foreach (var customFaction in FactionsForm.AllCustomFactions.Values)
                HandleCustomFactionFormsRelations(customFaction, _factionRelations[customFaction.Id], _factionsLocked[customFaction.Id]);

            // Clear after changes are processed
            _unsavedChanges.Clear();
            _unsavedFactionsLocked.Clear();

            Close();
        }

        private static void HandleCustomFactionFormsRelations(Faction target, Dictionary<string, int> relations, bool locked)
        {
            target.Relations ??= new Faction.RelationsObj();
            if (target.Relations.Relation == null)
                target.Relations.Relation = [];

            // Remove relations that were removed
            target.Relations.Relation.RemoveAll(a => !relations.ContainsKey(a.Faction));
            target.Relations.Locked = locked ? "1" : null;

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
                        target.Relations.Relation.Add(new Faction.Relation
                        {
                            Faction = factionName,
                            RelationValue = ConvertUIToRelationValue(relation.Value).ToString(CultureInfo.InvariantCulture)
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
                    faction.RelationValue = ConvertUIToRelationValue(relation.Value).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        class CustomFlowLayoutPanel : FlowLayoutPanel
        {
            public void ForwardMousewheel(MouseEventArgs e)
            {
                OnMouseWheel(e);
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

            if (_unsavedFactionsLocked.TryGetValue(selectedFaction, out var value))
                ChkLockRelations.Checked = value;
            else 
                ChkLockRelations.Checked = _factionsLocked[selectedFaction];

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
                track.MouseWheel += (s, e) =>
                {
                    ((HandledMouseEventArgs)e).Handled = true;
                    RelationsPanel.ForwardMousewheel(e); // forward event to FlowLayoutPanel
                };
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
            else
                CmbSelectedFaction.SelectedIndex = 0;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChkLockRelations_CheckedChanged(object sender, EventArgs e)
        {
            var selectedFaction = (string)CmbSelectedFaction.SelectedItem;
            if (_unsavedFactionsLocked.ContainsKey(selectedFaction))
            {
                // The value exists already, check if its the same as the default value, then remove it
                if (_factionsLocked[selectedFaction] == ChkLockRelations.Checked)
                    _unsavedFactionsLocked.Remove(selectedFaction);
                else
                    _unsavedFactionsLocked[selectedFaction] = ChkLockRelations.Checked;
            }
            else if (_factionsLocked[selectedFaction] != ChkLockRelations.Checked)
            {
                _unsavedFactionsLocked[selectedFaction] = ChkLockRelations.Checked;
            }
        }

        internal static void Clear()
        {
            _factionRelations.Clear();
            _factionsLocked.Clear();
            _unsavedFactionsLocked.Clear();
            _unsavedFactionsLocked.Clear();
        }
    }
}