using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text.Json;
using X4SectorCreator.Configuration;
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

        private readonly HashSet<string> _invalidFactions = new(StringComparer.OrdinalIgnoreCase)
        {
            "ministry", "buccaneers", "player", "quettanauts"
        };

        public PresetSelectionForm()
        {
            InitializeComponent();

            var vanillaFactions = FactionsForm.GetAllFactions(false)
                .Where(a => !_invalidFactions.Contains(a))
                .OrderBy(a => a)
                .ToArray();

            foreach (var race in vanillaFactions)
                CmbFaction.Items.Add(race);

            var factions = FactionsForm.GetAllFactions(true, true);
            foreach (var faction in factions)
            {
                CmbFactions.Items.Add(faction);
                CmbOwner.Items.Add(faction);
            }

            _mscFactions = new MultiSelectCombo(CmbFactions);
        }

        public static void ExecuteForProcGen(Faction faction, int coverage)
        {
            using var psf = new PresetSelectionForm();
            using var factoriesForm = new FactoriesForm();
            using var jobsForm = new JobsForm();

            // Set all fields
            psf.SetAllFieldsToFaction(faction, coverage);

            // Generate factories
            psf.FactoriesForm = factoriesForm;
            psf.BtnConfirm_Click(null, null);

            // Generate jobs
            psf.FactoriesForm = null;
            psf.JobsForm = jobsForm;
            psf.BtnConfirm_Click(null, null);
        }

        private bool _isProcGen = false;
        public void SetAllFieldsToFaction(Faction faction, int coverage)
        {
            string selectedPresetFaction = faction.Primaryrace.Equals("split", StringComparison.OrdinalIgnoreCase) ?
                "zyarth" : faction.Primaryrace;

            // Take scaleplate for pirate factions
            if (faction.Tags.Contains("plunder", StringComparison.OrdinalIgnoreCase))
                selectedPresetFaction = "scaleplate";

            CmbFaction.SelectedItem = selectedPresetFaction;
            CmbOwner.SelectedItem = faction.Id;
            _mscFactions.Select(faction.Id);
            TxtSectorCoverage.Text = coverage.ToString();
            _isProcGen = true;
        }

        public void BtnConfirm_Click(object sender, EventArgs e)
        {
            var faction = CmbFaction.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(faction))
            {
                _ = MessageBox.Show("Please select a valid preset faction.");
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

            if (!_isProcGen)
            {
                var type = FactoriesForm != null ? "factories" : "jobs";
                if (MessageBox.Show($"This will overwrite existing {type} that have the same ID, are you sure you want to do this?",
                    "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            string json = File.ReadAllText(Constants.DataPaths.SectorMappingFilePath);
            ClusterCollection clusterCollection = JsonSerializer.Deserialize<ClusterCollection>(json, ConfigSerializer.JsonSerializerOptions);

            // Make a sector ownership mapping
            var ownershipCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var cluster in clusterCollection.Clusters)
            {
                foreach (var sector in cluster.Sectors) 
                {
                    if (sector.Owner == null) continue;
                    ownershipCounts.TryGetValue(sector.Owner, out var value);
                    value++;
                    ownershipCounts[sector.Owner] = value;
                }
            }

            bool adjustQuotas = ownershipCounts.TryGetValue(faction, out var ownership);

            if (FactoriesForm != null)
            {
                // All factories where class is galaxy
                var templateFactories = GetTemplateFactories()
                    .Where(a => !a.Id.Contains("dlc_split", StringComparison.OrdinalIgnoreCase) && !a.Id.EndsWith("_new", StringComparison.OrdinalIgnoreCase))
                    .Where(a => a.Location != null && !string.IsNullOrWhiteSpace(a.Location.Class) &&
                        a.Location.Class.Equals("Galaxy", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                // Fix faction
                faction = GodGeneration.CorrectFactionName(faction);

                foreach (var factory in templateFactories)
                {
                    var raceKey = GetRaceKey(faction);
                    if (factory.Id.StartsWith(raceKey, StringComparison.OrdinalIgnoreCase) || 
                        (faction.Equals("scaleplate", StringComparison.OrdinalIgnoreCase) && factory.Id.StartsWith("pir_", StringComparison.OrdinalIgnoreCase)))
                    {
                        if (ContainsTag(factory.Location.Tags, "anarchy")) continue;

                        factory.Location.Excludedtags = null;
                        factory.Location.Tags = null;

                        // Location is too restrictive on factory presets, remove it!
                        if (factory.Location?.Economy != null)
                            factory.Location.Economy = null;
                        if (factory.Location?.Region != null)
                            factory.Location.Region = null;

                        EditFactoryData(factory, owner, raceKey);
                        if (adjustQuotas)
                            ApplyQuotaAdjustment(coverage, ownership, factoryQuota: factory.Quotas?.Quota);
                        FactoriesForm.AllFactories[factory.Id] = factory;
                    }
                }

                FactoriesForm.ApplyCurrentFilter();
            }
            
            if (JobsForm != null)
            {
                // All jobs where class is galaxy
                var templateJobs = GetTemplateJobs()
                    .Where(a => a.Location != null && !string.IsNullOrWhiteSpace(a.Location.Class) &&
                        a.Location.Class.Equals("Galaxy", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                foreach (var job in templateJobs)
                {
                    // Jobs have the full race, not just the 3 initials
                    if (job.Id.StartsWith(faction, StringComparison.OrdinalIgnoreCase))
                    {
                        if (ContainsTag(job.Location.Tags, "anarchy")) continue;

                        job.Location.Excludedtags = null;
                        job.Location.Tags = null;

                        EditJobData(job, owner, faction);
                        if (adjustQuotas)
                            ApplyQuotaAdjustment(coverage, ownership, jobQuota: job.Quota);
                        JobsForm.AllJobs[job.Id] = job;
                    }
                }

                JobsForm.ApplyCurrentFilter();
            }

            Close();
        }

        private static List<Job> GetTemplateJobs()
        {
            var baseDirectory = Constants.DataPaths.TemplateJobsDirectoryPath;
            var fileName = Path.Combine(baseDirectory, "Vanilla", "jobs.xml");
            if (!File.Exists(fileName))
                return [];

            var jobs = Jobs.DeserializeJobs(File.ReadAllText(fileName));
            return jobs.JobList;
        }

        private static List<Factory> GetTemplateFactories()
        {
            var baseDirectory = Constants.DataPaths.TemplateFactoriesDirectoryPath;
            var fileName = Path.Combine(baseDirectory, "Vanilla", "god.xml");
            if (!File.Exists(fileName))
                return [];

            var factories = Objects.Factories.DeserializeFactories(File.ReadAllText(fileName));
            return factories.FactoryList;
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

        private static void ApplyQuotaAdjustment(int sectorCoverage, int defaultOwnership, 
            Factory.Quota factoryQuota = null, Job.QuotaObject jobQuota = null)
        {
            if (factoryQuota != null)
            {
                if (factoryQuota.Galaxy != null &&
                    int.TryParse(factoryQuota.Galaxy, CultureInfo.InvariantCulture, out var galaxyQuota))
                {
                    factoryQuota.Galaxy = Math.Max(1, Math.Ceiling((double)galaxyQuota / defaultOwnership * sectorCoverage)).ToString();
                }

                /*
                if (factoryQuota.Cluster != null && 
                    int.TryParse(factoryQuota.Cluster, CultureInfo.InvariantCulture, out var clusterQuota))
                {
                    factoryQuota.Cluster = Math.Max(1, Math.Ceiling((double)clusterQuota / defaultOwnership * sectorCoverage)).ToString();
                }
                if (factoryQuota.Sector != null &&
                    int.TryParse(factoryQuota.Sector, CultureInfo.InvariantCulture, out var sectorQuota))
                {
                    factoryQuota.Sector = Math.Max(1, Math.Ceiling((double)sectorQuota / defaultOwnership * sectorCoverage)).ToString();
                }
                if (factoryQuota.Zone != null &&
                    int.TryParse(factoryQuota.Zone, CultureInfo.InvariantCulture, out var zoneQuota))
                {
                    factoryQuota.Zone = Math.Max(1, Math.Ceiling((double)zoneQuota / defaultOwnership * sectorCoverage)).ToString();
                }
                */
            }

            if (jobQuota != null)
            {
                if (jobQuota.Galaxy != null &&
                    int.TryParse(jobQuota.Galaxy, CultureInfo.InvariantCulture, out var galaxyQuota))
                {
                    jobQuota.Galaxy = Math.Max(1, Math.Ceiling((double)galaxyQuota / defaultOwnership * sectorCoverage)).ToString();
                }

                /*
                if (jobQuota.Cluster != null &&
                    int.TryParse(jobQuota.Cluster, CultureInfo.InvariantCulture, out var clusterQuota))
                {
                    jobQuota.Cluster = Math.Max(1, Math.Ceiling((double)clusterQuota / defaultOwnership * sectorCoverage)).ToString();
                }
                if (jobQuota.Sector != null &&
                    int.TryParse(jobQuota.Sector, CultureInfo.InvariantCulture, out var sectorQuota))
                {
                    jobQuota.Sector = Math.Max(1, Math.Ceiling((double)sectorQuota / defaultOwnership * sectorCoverage)).ToString();
                }
                if (jobQuota.Zone != null &&
                    int.TryParse(jobQuota.Zone, CultureInfo.InvariantCulture, out var zoneQuota))
                {
                    jobQuota.Zone = Math.Max(1, Math.Ceiling((double)zoneQuota / defaultOwnership * sectorCoverage)).ToString();
                }
                */
            }
        }

        private static bool ContainsTag(string field, string value)
        {
            return !string.IsNullOrWhiteSpace(field) && field.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        private void EditFactoryData(Factory factory, string ownerId, string raceKey)
        {
            // Replace the first instance of the raceKey
            if (factory.Id.StartsWith("pir_", StringComparison.OrdinalIgnoreCase))
            {
                factory.Id = string.Concat(ownerId, factory.Id.AsSpan("pir".Length)).Replace(" ", "_");
            }
            else
            {
                int index = factory.Id.IndexOf(raceKey, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    factory.Id = string.Concat(factory.Id.AsSpan(0, index), ownerId, factory.Id.AsSpan(index + raceKey.Length)).Replace(" ", "_");
                }
            }
            
            // Set owner faction
            factory.Owner = GodGeneration.CorrectFactionName(CmbOwner.SelectedItem as string);
            if (factory.Module?.Select?.Faction != null)
                factory.Module.Select.Faction = factory.Owner;
            factory.Location ??= new Factory.LocationObj();

            // Only replace faction location if its not ownerless (pirate stations and jobs spawn in unowned sectors)
            if (factory.Location.Faction != null && !factory.Location.Faction.Contains("ownerless", StringComparison.OrdinalIgnoreCase))
            {
                factory.Location.Faction = "[" + string.Join(",", _mscFactions.SelectedItems.Cast<string>().Select(GodGeneration.CorrectFactionName)) + "]";
            }

            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                factory.Location.Macro = $"{GalaxySettingsForm.GalaxyName}_macro";
            }
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

                // Only replace faction location if its not ownerless (pirate stations and jobs spawn in unowned sectors)
                if (job.Location.Faction != null && !job.Location.Faction.Contains("ownerless", StringComparison.OrdinalIgnoreCase))
                {
                    job.Location.Faction = "[" + string.Join(",", _mscFactions.SelectedItems.Cast<string>().Select(GodGeneration.CorrectFactionName)) + "]";
                }
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

            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                job.Location.Macro = $"{GalaxySettingsForm.GalaxyName}_macro";
            }
        }
    }
}
