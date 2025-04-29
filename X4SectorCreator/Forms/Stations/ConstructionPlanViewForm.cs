using System.ComponentModel;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Stations
{
    public partial class ConstructionPlanViewForm : Form
    {
        public static readonly Dictionary<string, Constructionplan> AllCustomConstructionPlans = new(StringComparer.OrdinalIgnoreCase);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StationForm StationForm { get; set; }

        public ConstructionPlanViewForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This keeps all stations with custom construction plans in sync when a change happens.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="new"></param>
        private static void UpdateConfiguredStationsOnPlanModified(Constructionplan previous, Constructionplan @new)
        {
            var stationGroups = MainForm.Instance.AllClusters.Values
                .SelectMany(a => a.Sectors)
                .SelectMany(a => a.Zones)
                .SelectMany(a => a.Stations)
                .Where(a => !string.IsNullOrWhiteSpace(a.CustomConstructionPlan))
                .GroupBy(a => a.CustomConstructionPlan, StringComparer.OrdinalIgnoreCase);

            foreach (var group in stationGroups)
            {
                if (!group.Key.Equals(previous.Id, StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (var station in group)
                {
                    // Update construction plan with new ID or set to null if removed
                    station.CustomConstructionPlan = @new?.Id;
                }
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            openFileDialog.Title = "Select Construction Plan";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                var xml = File.ReadAllText(filePath);

                try
                {
                    var constructionPlan = Constructionplans.Deserialize(xml)?.Plan?.FirstOrDefault();
                    if (constructionPlan == null)
                    {
                        _ = MessageBox.Show("The file contains no valid construction plans.");
                        return;
                    }

                    const string lblId = "Choose Unique ID:";
                    const string lblName = "Choose Station Name:";
                    Dictionary<string, string> data = MultiInputDialog.Show("Construction Plan Properties",
                        (lblId, null, constructionPlan.Id),
                        (lblName, null, constructionPlan.Name)
                    );
                    if (data == null || data.Count == 0)
                        return;

                    string id = FactionForm.Sanitize(data[lblId] ?? string.Empty, true).Replace(" ", "_");
                    string name = data[lblName];

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        if ((data[lblId] ?? string.Empty).Length > 0)
                            _ = MessageBox.Show("ID has invalid characters.");
                        else
                            _ = MessageBox.Show("ID cannot be empty or whitespace only.");
                        return;
                    }

                    if (AllCustomConstructionPlans.ContainsKey(id))
                    {
                        _ = MessageBox.Show($"A construction plan with the ID \"{id}\" already exists.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        _ = MessageBox.Show("Name cannot be empty or whitespace only.");
                        return;
                    }

                    constructionPlan.Id = id;
                    constructionPlan.Name = name;

                    AllCustomConstructionPlans[id] = constructionPlan;
                    ConstructionPlansListBox.Items.Add(constructionPlan);

                    if (StationForm != null && !StationForm.IsDisposed)
                        StationForm.UpdateAvailableConstructionPlans();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Invalid construction plan xml: " + ex.Message);
                    return;
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (ConstructionPlansListBox.SelectedItem is Constructionplan constructionPlan)
            {
                AllCustomConstructionPlans.Remove(constructionPlan.Id);

                int index = ConstructionPlansListBox.Items.IndexOf(ConstructionPlansListBox.SelectedItem);
                ConstructionPlansListBox.Items.Remove(ConstructionPlansListBox.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ConstructionPlansListBox.SelectedItem = index >= 0 && ConstructionPlansListBox.Items.Count > 0 ? ConstructionPlansListBox.Items[index] : null;

                if (StationForm != null && !StationForm.IsDisposed)
                    StationForm.UpdateAvailableConstructionPlans();

                UpdateConfiguredStationsOnPlanModified(constructionPlan, null);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (ConstructionPlansListBox.SelectedItem is Constructionplan selectedConstructionPlan)
            {
                if (!FieldValidation()) return;

                var old = selectedConstructionPlan.Clone();
                selectedConstructionPlan.Id = TxtId.Text;
                selectedConstructionPlan.Name = TxtName.Text;

                // Re-map dictionary key if ID was changed
                if (!old.Id.Equals(selectedConstructionPlan.Id))
                {
                    AllCustomConstructionPlans.Remove(old.Id);
                    AllCustomConstructionPlans[selectedConstructionPlan.Id] = selectedConstructionPlan;

                    // Re-adjust view
                    var indexOf = ConstructionPlansListBox.Items.IndexOf(selectedConstructionPlan);
                    ConstructionPlansListBox.Items.Remove(selectedConstructionPlan);
                    ConstructionPlansListBox.Items.Insert(indexOf, selectedConstructionPlan);
                    ConstructionPlansListBox.SelectedItem = selectedConstructionPlan;
                }

                if (StationForm != null && !StationForm.IsDisposed)
                    StationForm.UpdateAvailableConstructionPlans();

                UpdateConfiguredStationsOnPlanModified(old, selectedConstructionPlan);
            }
        }

        private bool FieldValidation()
        {
            // First sanitize ID field
            TxtId.Text = FactionForm.Sanitize(TxtId.Text ?? string.Empty, true).Replace(" ", "_");

            if (string.IsNullOrWhiteSpace(TxtId.Text))
            {
                _ = MessageBox.Show("ID cannot be empty or whitespace only.");
                return false;
            }

            if (AllCustomConstructionPlans.ContainsKey(TxtId.Text))
            {
                _ = MessageBox.Show($"A construction plan with the ID \"{TxtId.Text}\" already exists.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                _ = MessageBox.Show("Name cannot be empty or whitespace only.");
                return false;
            }
            return true;
        }

        private void ConstructionPlansListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show all modules in the selected construction plan
            if (ConstructionPlansListBox.SelectedItem is Constructionplan selectedConstructionPlan)
            {
                // Display all constructionplan modules and their count
                var moduleGroups = selectedConstructionPlan.EntryObj
                    .OrderBy(a => int.TryParse(a.Index, out var index) ? index : 0)
                    .Select(a => a.Macro)
                    .GroupBy(a => a, StringComparer.OrdinalIgnoreCase);
                foreach (var group in moduleGroups)
                    ModulesListBox.Items.Add($"[{group.Count()}]: {group.Key}");

                // Set values of fields properly
                TxtId.Text = selectedConstructionPlan.Id;
                TxtName.Text = selectedConstructionPlan.Name;

                TxtId.Enabled = true;
                TxtName.Enabled = true;
                BtnUpdate.Enabled = true;
                BtnReImport.Enabled = true;
            }
            else if (ModulesListBox.Items.Count > 0)
            {
                TxtId.Clear();
                TxtName.Clear();
                TxtId.Enabled = false;
                TxtName.Enabled = false;
                BtnUpdate.Enabled = false;
                BtnReImport.Enabled = false;
                ModulesListBox.Items.Clear();
            }
        }

        private void ConstructionPlanViewForm_Load(object sender, EventArgs e)
        {
            TxtId.Enabled = false;
            TxtName.Enabled = false;
            BtnUpdate.Enabled = false;
            BtnReImport.Enabled = false;

            foreach (var constructionplan in AllCustomConstructionPlans)
            {
                ConstructionPlansListBox.Items.Add(constructionplan.Value);
            }

            ConstructionPlansListBox.SelectedItem = ConstructionPlansListBox.Items
                .OfType<Constructionplan>()
                .FirstOrDefault();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnReImport_Click(object sender, EventArgs e)
        {
            if (ConstructionPlansListBox.SelectedItem is not Constructionplan selectedPlan) return;

            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            openFileDialog.Title = "Select Construction Plan";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                var xml = File.ReadAllText(filePath);

                try
                {
                    var constructionPlan = Constructionplans.Deserialize(xml)?.Plan?.FirstOrDefault();
                    if (constructionPlan == null)
                    {
                        _ = MessageBox.Show("The file contains no valid construction plans.");
                        return;
                    }

                    // Match ID and Name
                    constructionPlan.Id = selectedPlan.Id;
                    constructionPlan.Name = selectedPlan.Name;

                    // Update entries
                    AllCustomConstructionPlans[constructionPlan.Id] = constructionPlan;
                    ConstructionPlansListBox.Items.Remove(selectedPlan);
                    ConstructionPlansListBox.Items.Add(constructionPlan);
                    ConstructionPlansListBox.SelectedItem = constructionPlan;

                    if (StationForm != null && !StationForm.IsDisposed)
                        StationForm.UpdateAvailableConstructionPlans();

                    _ = MessageBox.Show("Construction plan was re-imported succesfully.");
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Invalid construction plan xml: " + ex.Message);
                    return;
                }
            }
        }
    }
}
