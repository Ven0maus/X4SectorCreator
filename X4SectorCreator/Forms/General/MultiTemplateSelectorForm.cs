using System.ComponentModel;
using System.Data;
using X4SectorCreator.CustomComponents;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;

namespace X4SectorCreator.Forms.General
{
    public partial class MultiTemplateSelectorForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactoriesForm FactoriesForm { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobsForm JobsForm { get; set; }

        private readonly MultiSelectCombo _mscFactions;
        private readonly List<object> _currentSelection = [];
        private readonly LazyEvaluated<TemplateGroupsForm> _templateGroupsView = new(() => new TemplateGroupsForm(), a => !a.IsDisposed);

        public MultiTemplateSelectorForm()
        {
            InitializeComponent();

            var factions = FactionsForm.GetAllFactions(true, true);
            foreach (var faction in factions)
            {
                CmbFactions.Items.Add(faction);
                CmbOwner.Items.Add(faction);
            }

            _mscFactions = new MultiSelectCombo(CmbFactions);

            // Setup filter options
            TxtSearch.EnableTextSearch(_currentSelection, a => a.ToString(), ApplyCurrentFilter);
            Disposed += MultiTemplateSelectorForm_Disposed;
        }

        private void MultiTemplateSelectorForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        private void ApplyCurrentFilter(List<object> items = null)
        {
            if (items == null) return;

            object[] data = [.. items.OrderBy(a => a.ToString())];

            ListTemplates.Items.Clear();
            foreach (object obj in data)
            {
                _ = ListTemplates.Items.Add(obj);
            }
        }

        private static string GetRaceKey(string faction)
        {
            var key = faction[..3];

            // Its not consistent for all factions
            return faction.ToLower() switch
            {
                "freesplit" => "frf",
                _ => key,
            };
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void EditFactoryData(Factory factory, string ownerId, string raceKey)
        {
            // Replace the first instance of the raceKey
            int index = factory.Id.IndexOf(raceKey, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                factory.Id = string.Concat(factory.Id.AsSpan(0, index), ownerId, factory.Id.AsSpan(index + raceKey.Length)).Replace(" ", "_");
            }

            // Set owner faction
            factory.Owner = GodGeneration.CorrectFactionName(CmbOwner.SelectedItem as string);
            if (factory.Module?.Select?.Faction != null)
                factory.Module.Select.Faction = factory.Owner;
            factory.Location ??= new Factory.LocationObj();
            factory.Location.Faction = "[" + string.Join(",", _mscFactions.SelectedItems.Cast<string>().Select(GodGeneration.CorrectFactionName)) + "]";
        }

        private void EditJobData(Job job, string ownerId, string raceKey)
        {
            var owner = GodGeneration.CorrectFactionName(CmbOwner.SelectedItem as string);

            // Set faction on various objects
            if (job.Category != null)
            {
                job.Category.Faction = owner;
            }

            if (job.Location != null)
            {
                if (job.Location?.Policefaction != null)
                    job.Location.Policefaction = owner;
                job.Location.Faction = "[" + string.Join(",", _mscFactions.SelectedItems.Cast<string>().Select(GodGeneration.CorrectFactionName)) + "]";
            }

            if (job.Ship?.Select != null)
            {
                job.Ship.Select.Faction = owner;
            }

            if (job.Ship?.Owner != null)
            {
                job.Ship.Owner.Exact = owner;
            }

            // Replace the first instance of the raceKey
            int index = job.Id.IndexOf(raceKey, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                job.Id = string.Concat(job.Id.AsSpan(0, index), ownerId, job.Id.AsSpan(index + raceKey.Length)).Replace(" ", "_");

                // Do the same with subordinates
                if (job.Subordinates?.Subordinate != null)
                {
                    foreach (var subordinate in job.Subordinates.Subordinate)
                    {
                        if (string.IsNullOrWhiteSpace(subordinate.Job)) continue;
                        index = subordinate.Job.IndexOf(raceKey, StringComparison.OrdinalIgnoreCase);
                        if (index >= 0)
                        {
                            subordinate.Job = string.Concat(subordinate.Job.AsSpan(0, index), ownerId, subordinate.Job.AsSpan(index + raceKey.Length)).Replace(" ", "_");
                        }
                    }
                }
            }
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            var owner = CmbOwner.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(owner) || _mscFactions.SelectedItems.Count == 0)
            {
                _ = MessageBox.Show("Please fill in the faction fields.");
                return;
            }

            var type = FactoriesForm != null ? "factories" : "jobs";
            if (MessageBox.Show($"This will overwrite existing {type} that have the same ID, are you sure you want to do this?",
                "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            if (FactoriesForm != null)
            {
                // All factories where class is galaxy
                var templateFactories = SelectedTemplatesListBox.Items.Cast<Factory>().ToArray();

                foreach (var factory in templateFactories)
                {
                    var parts = factory.Id.Split('_');
                    string raceKey;
                    if (parts.Length > 0)
                    {
                        raceKey = GetRaceKey(parts[0]);
                    }
                    else
                    {
                        raceKey = "{faction.Id}";
                    }

                    EditFactoryData(factory, owner, raceKey);
                    var newFactory = Factory.DeserializeFactory(factory.SerializeFactory().Replace("{faction.Id}", owner, StringComparison.OrdinalIgnoreCase));
                    FactoriesForm.AllFactories[newFactory.Id] = newFactory;
                }

                FactoriesForm.ApplyCurrentFilter();
            }

            if (JobsForm != null)
            {
                // All jobs where class is galaxy
                var templateJobs = SelectedTemplatesListBox.Items.Cast<Job>().ToArray();
                var factions = FactionsForm.GetAllFactions(false);
                foreach (var job in templateJobs)
                {
                    var parts = job.Id.Split('_');
                    string raceKey;
                    if (parts.Length > 0)
                    {
                        // Check if part matches any of the vanilla factions
                        raceKey = parts[0];
                        if (!factions.Contains(raceKey))
                            raceKey = "{faction.Id}";
                    }
                    else
                    {
                        raceKey = "{faction.Id}";
                    }

                    EditJobData(job, owner, raceKey);
                    var newJob = Job.DeserializeJob(job.SerializeJob().Replace("{faction.Id}", owner, StringComparison.OrdinalIgnoreCase));
                    JobsForm.AllJobs[newJob.Id] = newJob;
                }

                JobsForm.ApplyCurrentFilter();
            }

            Close();
        }

        private void MultiTemplateSelectorForm_Load(object sender, EventArgs e)
        {
            CmbTemplatesGroup.Items.Clear();

            // Get all available groups
            var baseDirectory = FactoriesForm != null ?
                Constants.DataPaths.TemplateFactoriesDirectoryPath :
                Constants.DataPaths.TemplateJobsDirectoryPath;

            foreach (var directory in Directory.GetDirectories(baseDirectory))
            {
                string groupName = new DirectoryInfo(Path.GetFileName(directory)).Name;
                CmbTemplatesGroup.Items.Add(groupName);
            }
            CmbTemplatesGroup.SelectedItem = "Vanilla";
        }

        private void BtnDeselect_Click(object sender, EventArgs e)
        {
            if (SelectedTemplatesListBox.SelectedItem != null)
            {
                int index = SelectedTemplatesListBox.Items.IndexOf(SelectedTemplatesListBox.SelectedItem);
                SelectedTemplatesListBox.Items.Remove(SelectedTemplatesListBox.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                SelectedTemplatesListBox.SelectedItem = index >= 0 && SelectedTemplatesListBox.Items.Count > 0 ?
                    SelectedTemplatesListBox.Items[index] : null;
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (ListTemplates.SelectedItem != null && !SelectedTemplatesListBox.Items.Contains(ListTemplates.SelectedItem))
            {
                SelectedTemplatesListBox.Items.Add(ListTemplates.SelectedItem);
            }
        }

        private void CmbTemplatesGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load all available templates of the selected group
            var groupName = CmbTemplatesGroup.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                ListTemplates.Items.Clear();
                return;
            }

            ListTemplates.Items.Clear();
            _currentSelection.Clear();

            var baseDirectory = FactoriesForm != null ?
                Constants.DataPaths.TemplateFactoriesDirectoryPath :
                Constants.DataPaths.TemplateJobsDirectoryPath;
            var fileName = Path.Combine(baseDirectory, groupName, FactoriesForm == null ? "jobs.xml" : "god.xml");
            if (!File.Exists(fileName))
                return;

            if (JobsForm != null)
            {
                var jobs = Jobs.DeserializeJobs(File.ReadAllText(fileName));
                foreach (var job in jobs.JobList)
                {
                    ListTemplates.Items.Add(job);
                    _currentSelection.Add(job);
                }
            }
            else if (FactoriesForm != null)
            {
                var factories = Objects.Factories.DeserializeFactories(File.ReadAllText(fileName));
                foreach (var factory in factories.FactoryList)
                {
                    ListTemplates.Items.Add(factory);
                    _currentSelection.Add(factory);
                }
            }
            TxtSearch.GetTextSearchComponent().ForceCalculate();
        }

        private void ListTemplates_DoubleClick(object sender, EventArgs e)
        {
            BtnSelect.PerformClick();
        }

        private void SelectedTemplatesListBox_DoubleClick(object sender, EventArgs e)
        {
            BtnDeselect.PerformClick();
        }

        private void SelectGroup_Click(object sender, EventArgs e)
        {
            foreach (var option in ListTemplates.Items)
            {
                if (!SelectedTemplatesListBox.Items.Contains(option))
                {
                    SelectedTemplatesListBox.Items.Add(option);
                }
            }
        }

        private void BtnDeselectGroup_Click(object sender, EventArgs e)
        {
            foreach (var option in ListTemplates.Items)
            {
                SelectedTemplatesListBox.Items.Remove(option);
            }
        }

        private void BtnViewTemplateGroups_Click(object sender, EventArgs e)
        {
            _templateGroupsView.Value.UpdateMethod = UpdateTemplateGroups;
            _templateGroupsView.Value.TemplateGroupsFor = FactoriesForm == null ? 
                TemplateGroupsForm.GroupsFor.Jobs : TemplateGroupsForm.GroupsFor.Factories;
            _templateGroupsView.Value.Show();
        }

        private void UpdateTemplateGroups()
        {
            MultiTemplateSelectorForm_Load(this, null);
        }
    }
}
