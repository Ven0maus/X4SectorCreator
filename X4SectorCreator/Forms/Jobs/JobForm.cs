using System.ComponentModel;
using System.Data;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobForm : Form
    {
        private Job _job;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Job Job
        {
            get => _job;
            set
            {
                _job = value;
                if (_job != null)
                {
                    TxtJobXml.Text = _job.SerializeJob();
                    TxtJobXml.SelectionStart = TxtJobXml.Text.Length;
                }
            }
        }

        private bool _isEditing;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                BtnCreate.Text = _isEditing ? "Update" : "Create";
            }
        }

        private readonly LazyEvaluated<FactionSelectionForm> FactionSelectionForm = new(() => new FactionSelectionForm(), a => !a.IsDisposed);
        private static readonly string[] _typeLabels = ["galaxy", "cluster", "sector"];

        public JobForm()
        {
            InitializeComponent();
        }

        private void BtnSelectFaction_Click(object sender, EventArgs e)
        {
            Job job = TryDeserializeJob(false);
            if (job == null)
            {
                _ = MessageBox.Show("The xml must be valid, if you want to select a faction for the job.");
                return;
            }

            FactionSelectionForm.Value.Job = job;
            FactionSelectionForm.Value.JobForm = this;
            FactionSelectionForm.Value.Show();
        }

        private void BtnSelectBasket_Click(object sender, EventArgs e)
        {
            Job job = TryDeserializeJob(false);
            if (job == null)
            {
                _ = MessageBox.Show("The xml must be valid, if you want to select a basket for the job.");
                return;
            }

            const string lblVanillaBasket = "Vanilla Baskets:";
            const string lblCustomBasket = "Custom Baskets:";
            Dictionary<string, string> modInfo = MultiInputDialog.Show("Select Basket (Pick Vanilla Or Custom)",
                (lblVanillaBasket, [.. BasketsForm.VanillaBaskets.Value.Select(a => a.Id).OrderBy(a => a)], null),
                (lblCustomBasket, [.. JobsForm.AllBaskets.Values.Select(a => a.Id.Replace("PREFIX_", "")).OrderBy(a => a)], null)
            );

            if (modInfo == null || modInfo.Count != 2)
            {
                return;
            }

            string basketName = modInfo[lblVanillaBasket];
            if (string.IsNullOrWhiteSpace(basketName))
            {
                basketName = modInfo[lblCustomBasket] ?? "";
                if (!string.IsNullOrWhiteSpace(basketName))
                {
                    basketName = $"PREFIX_{basketName}";
                }
            }

            if (string.IsNullOrWhiteSpace(basketName))
            {
                _ = MessageBox.Show("Please select a basket.");
                return;
            }

            job.Basket ??= new Job.BasketObj();
            job.Basket.Basket = basketName;

            TxtJobXml.Text = job.SerializeJob();
            TxtJobXml.SelectionStart = TxtJobXml.Text.Length;
        }

        private void BtnSelectJobLocation_Click(object sender, EventArgs e)
        {
            Job job = TryDeserializeJob(false);
            if (job == null)
            {
                _ = MessageBox.Show("The xml must be valid, if you want to select a location for the job.");
                return;
            }

            Cluster jobCluster = GetClusterFromJob(job);
            Sector jobSector = GetSectorFromJob(job, jobCluster);

            const string lblType = "Location Type:";
            const string lblCluster = "Location Cluster (only if type is cluster):";
            const string lblSector = "Location Sector (only if type is sector):";
            Dictionary<string, string> modInfo = MultiInputDialog.Show("Select Location",
                (lblType, _typeLabels, jobSector != null ? "sector" : jobCluster != null ? "cluster" : "galaxy"),
                (lblCluster, MainForm.Instance.AllClusters.Values.Select(a => a.Name).OrderBy(a => a).ToArray(), jobCluster?.Name),
                (lblSector, MainForm.Instance.AllClusters.Values.SelectMany(a => a.Sectors).Select(a => a.Name).OrderBy(a => a).ToArray(), jobSector?.Name)
            );

            if (modInfo == null || modInfo.Count != 3)
            {
                return;
            }

            string type = modInfo[lblType]?.ToLower();
            if (string.IsNullOrWhiteSpace(type) || !_typeLabels.Contains(type))
            {
                _ = MessageBox.Show("Location Type must have a valid value, no changes applied.");
                return;
            }

            string clusterName = modInfo[lblCluster];
            string sectorName = modInfo[lblSector];

            // Create location if not exist yet
            job.Location ??= new Job.LocationObject();

            switch (type)
            {
                case "galaxy":
                    job.Location.Class = "galaxy";
                    job.Location.Macro = $"{GalaxySettingsForm.GalaxyName}_macro";
                    break;
                case "cluster":
                    Cluster clusterValue = MainForm.Instance.AllClusters.Values
                        .First(a => a.Name.Equals(clusterName, StringComparison.OrdinalIgnoreCase));

                    string clusterCode = $"PREFIX_CL_c{clusterValue.Id:D3}_macro";
                    if (clusterValue.IsBaseGame)
                    {
                        clusterCode = $"{clusterValue.BaseGameMapping}_macro";
                    }

                    job.Location.Class = "cluster";
                    job.Location.Macro = clusterCode;
                    break;
                case "sector":
                    var sectorValue = MainForm.Instance.AllClusters.Values
                        .SelectMany(cluster => cluster.Sectors, (cluster, sector) => new { cluster, sector })
                        .First(a => a.sector.Name.Equals(sectorName, StringComparison.OrdinalIgnoreCase));
                    Cluster cluster = sectorValue.cluster;
                    Sector sector = sectorValue.sector;

                    string sectorCode = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
                    if (cluster.IsBaseGame && sector.IsBaseGame)
                    {
                        sectorCode = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                    }
                    else if (cluster.IsBaseGame)
                    {
                        sectorCode = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}_macro";
                    }

                    job.Location.Class = "sector";
                    job.Location.Macro = sectorCode;
                    break;
            }

            TxtJobXml.Text = job.SerializeJob();
            TxtJobXml.SelectionStart = TxtJobXml.Text.Length;
        }

        private static Cluster GetClusterFromJob(Job job)
        {
            if (string.IsNullOrWhiteSpace(job.Location?.Macro))
            {
                return null;
            }

            string jobLocation = job.Location.Macro;
            Dictionary<(int, int), Cluster> allClusters = MainForm.Instance.AllClusters;

            foreach (KeyValuePair<(int, int), Cluster> cluster in allClusters)
            {
                string clusterCode = $"PREFIX_CL_c{cluster.Value.Id:D3}_macro";
                if (cluster.Value.IsBaseGame)
                {
                    clusterCode = $"{cluster.Value.BaseGameMapping}_macro";
                }

                if (jobLocation.Equals(clusterCode, StringComparison.OrdinalIgnoreCase))
                {
                    return cluster.Value;
                }

                foreach (Sector sector in cluster.Value.Sectors)
                {
                    string sectorCode = $"PREFIX_SE_c{cluster.Value.Id:D3}_s{sector.Id:D3}_macro";
                    if (cluster.Value.IsBaseGame && sector.IsBaseGame)
                    {
                        sectorCode = $"{cluster.Value.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                    }
                    else if (cluster.Value.IsBaseGame)
                    {
                        sectorCode = $"PREFIX_SE_c{cluster.Value.BaseGameMapping}_s{sector.Id}_macro";
                    }

                    if (jobLocation.Equals(sectorCode, StringComparison.OrdinalIgnoreCase))
                    {
                        return cluster.Value;
                    }
                }
            }

            return null;
        }

        private static Sector GetSectorFromJob(Job job, Cluster cluster)
        {
            if (string.IsNullOrWhiteSpace(job.Location?.Macro) || cluster == null)
            {
                return null;
            }

            foreach (Sector sector in cluster.Sectors)
            {
                string sectorCode = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
                if (cluster.IsBaseGame && sector.IsBaseGame)
                {
                    sectorCode = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                }
                else if (cluster.IsBaseGame)
                {
                    sectorCode = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}_macro";
                }

                if (job.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return sector;
                }
            }
            return null;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                Job job = TryDeserializeJob(true) ??
                    throw new Exception("No valid job exists within xml structure.");

                if (!IsEditing)
                {
                    // If not editing, always validate job id
                    if (JobsForm.AllJobs.ContainsKey(job.Id))
                    {
                        throw new Exception($"A job with the id \"{job.Id}\" already exists, please use another id.");
                    }

                    JobsForm.AllJobs.Add(job.Id, job);
                }
                else
                {
                    // If editing and job id was changed we need to validate
                    if (Job.Id != job.Id)
                    {
                        if (JobsForm.AllJobs.ContainsKey(job.Id))
                        {
                            throw new Exception($"A job with the id \"{job.Id}\" already exists, please use another id.");
                        }
                    }

                    // Remove old job
                    _ = JobsForm.AllJobs.Remove(Job.Id);
                    // Replace with new job
                    JobsForm.AllJobs.Add(job.Id, job);
                }

                if (MainForm.Instance.JobsForm.Value.Visible)
                {
                    MainForm.Instance.JobsForm.Value.UpdateAvailableFilterOptions();
                    MainForm.Instance.JobsForm.Value.ApplyCurrentFilter();
                }
                Close();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Invalid XML: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Job TryDeserializeJob(bool throwException)
        {
            try
            {
                return Job.DeserializeJob(TxtJobXml.Text);
            }
            catch (Exception)
            {
                if (!throwException)
                {
                    return null;
                }

                throw;
            }
        }
    }
}
