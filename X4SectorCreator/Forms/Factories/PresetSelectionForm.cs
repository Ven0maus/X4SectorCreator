using System.ComponentModel;
using System.Data;
using X4SectorCreator.CustomComponents;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;

namespace X4SectorCreator.Forms.Factories
{
    public partial class PresetSelectionForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactoriesForm FactoriesForm { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobsForm JobsForm { get; set; }

        private readonly MultiSelectCombo _mscFactions;

        public PresetSelectionForm()
        {
            InitializeComponent();

            var races = StationForm.Races
                .OrderBy(a => a)
                .ToArray();

            foreach (var race in races)
                CmbRace.Items.Add(race);
            CmbRace.SelectedItem = races.First();

            var factions = FactionsForm.GetAllFactions(true, true);
            foreach (var faction in factions)
            {
                CmbFactions.Items.Add(faction);
                CmbOwner.Items.Add(faction);
            }

            _mscFactions = new MultiSelectCombo(CmbFactions);
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            var race = CmbRace.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(race))
            {
                _ = MessageBox.Show("Please select a valid race.");
                return;
            }

            var owner = CmbOwner.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(owner) || _mscFactions.SelectedItems.Count == 0)
            {
                _ = MessageBox.Show("Please fill in the faction fields.");
                return;
            }

            var coverageStr = TxtSectorCoverage.Text;
            if (string.IsNullOrWhiteSpace(coverageStr) || !int.TryParse(coverageStr, out var coverage))
            {
                _ = MessageBox.Show("Please fill in a valid integer sector coverage value.");
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
                var templateFactories = FactoryTemplatesForm.CollectTemplateFactories().ToArray();

                foreach (var factory in templateFactories)
                {
                    var raceKey = race[..3];
                    if (factory.Id.StartsWith(raceKey, StringComparison.OrdinalIgnoreCase))
                    {
                        EditFactoryData(factory, owner, raceKey);
                        FactoriesForm.AllFactories[factory.Id] = factory;
                    }
                }

                FactoriesForm.ApplyCurrentFilter();
            }
            
            if (JobsForm != null)
            {
                var templateJobs = JobTemplatesForm.CollectTemplateJobs().ToArray();

                foreach (var job in templateJobs)
                {
                    // Jobs have the full race, not just the 3 initials
                    if (job.Id.StartsWith(race, StringComparison.OrdinalIgnoreCase))
                    {
                        EditJobData(job, owner, race);
                        JobsForm.AllJobs[job.Id] = job;
                    }
                }

                JobsForm.ApplyCurrentFilter();
            }

            Close();
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
                factory.Id = string.Concat(factory.Id.AsSpan(0, index), ownerId, factory.Id.AsSpan(index + raceKey.Length));
            }

            // Set owner faction
            factory.Owner = GodGeneration.CorrectFactionName(CmbOwner.SelectedItem as string);
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
                job.Id = string.Concat(job.Id.AsSpan(0, index), ownerId, job.Id.AsSpan(index + raceKey.Length));
            }
        }
    }
}
