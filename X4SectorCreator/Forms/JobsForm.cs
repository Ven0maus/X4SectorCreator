using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobsForm : Form
    {
        public static readonly Dictionary<string, Job> AllJobs = new(StringComparer.OrdinalIgnoreCase);
        public static readonly Dictionary<string, Basket> AllBaskets = new(StringComparer.OrdinalIgnoreCase);

        private JobForm _jobForm;
        public JobForm JobForm => _jobForm != null && !_jobForm.IsDisposed ? _jobForm : (_jobForm = new JobForm());

        private JobTemplatesForm _jobTemplatesForm;
        public JobTemplatesForm JobTemplatesForm => _jobTemplatesForm != null && !_jobTemplatesForm.IsDisposed ? _jobTemplatesForm : (_jobTemplatesForm = new JobTemplatesForm());

        private BasketsForm _basketsForm;
        public BasketsForm BasketsForm => _basketsForm != null && !_basketsForm.IsDisposed ? _basketsForm : (_basketsForm = new BasketsForm());

        private bool _applyFilter = true;

        public JobsForm()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            // Clear existing values for the filter options
            cmbBasket.Items.Clear();
            cmbFaction.Items.Clear();
            cmbOrder.Items.Clear();
            cmbCluster.Items.Clear();
            cmbSector.Items.Clear();

            // Set new default values for filter options
            UpdateAvailableFilterOptions();

            // By default set for each option "Any"
            _applyFilter = false;
            var comboboxes = new[] { cmbBasket, cmbFaction, cmbOrder, cmbCluster, cmbSector };
            foreach (var cmb in comboboxes)
                cmb.SelectedItem = "Any";
            _applyFilter = true;

            // Apply the filter
            ApplyCurrentFilter();
        }

        public void UpdateAvailableFilterOptions()
        {
            _applyFilter = false;
            var originalFaction = cmbFaction.SelectedItem ?? "Any";
            var originalBasket = cmbBasket.SelectedItem ?? "Any";
            var originalOrder = cmbOrder.SelectedItem ?? "Any";
            var originalCluster = cmbCluster.SelectedItem ?? "Any";
            var originalSector = cmbSector.SelectedItem ?? "Any";

            // Factions
            cmbFaction.Items.Clear();
            foreach (var value in AllJobs.Select(a => a.Value.Ship?.Owner?.Exact).Where(a => a != null).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(a => a))
                cmbFaction.Items.Add(value);
            cmbFaction.Items.Insert(0, "Any");

            // Baskets
            cmbBasket.Items.Clear();
            foreach (var basket in AllJobs.Select(a => a.Value.Basket?.Basket).Where(a => a != null).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(a => a))
                cmbBasket.Items.Add(basket);
            cmbBasket.Items.Insert(0, "Any");

            // Orders
            cmbOrder.Items.Clear();
            foreach (var value in AllJobs.Select(a => a.Value.Orders?.Order?.Order).Where(a => a != null).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(a => a))
                cmbOrder.Items.Add(value);
            cmbOrder.Items.Insert(0, "Any");

            // Clusters
            cmbCluster.Items.Clear();
            foreach (var cluster in AllJobs.Values.Select(GetClusterFromJob).Where(a => a != null).Distinct().OrderBy(a => a.Name))
                cmbCluster.Items.Add(cluster);
            cmbCluster.Items.Insert(0, "Any");

            // Reset original selected values if still available
            cmbFaction.SelectedItem = cmbFaction.Items.Contains(originalFaction) ? originalFaction : "Any";
            cmbBasket.SelectedItem = cmbBasket.Items.Contains(originalBasket) ? originalBasket : "Any";
            cmbOrder.SelectedItem = cmbOrder.Items.Contains(originalOrder) ? originalOrder : "Any";
            cmbCluster.SelectedItem = cmbCluster.Items.Contains(originalCluster) ? originalCluster : "Any";

            // Sectors is exceptional, only populated when a cluster is selected
            cmbSector.Items.Clear();
            if (cmbCluster.SelectedItem != null)
            {
                if (cmbCluster.SelectedItem is Cluster cluster)
                {
                    foreach (var sector in cluster.Sectors.OrderBy(a => a.Name))
                    {
                        string sectorCode = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
                        if (cluster.IsBaseGame && sector.IsBaseGame)
                            sectorCode = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                        else if (cluster.IsBaseGame)
                            sectorCode = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}_macro";

                        // Check if a job exists for this sector, then add the sector
                        if (AllJobs.Any(a => a.Value.Location?.Macro != null && a.Value.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase)))
                            cmbSector.Items.Add(sector);
                    }
                }
            }
            cmbSector.Enabled = cmbSector.Items.Count > 0;
            cmbSector.Items.Insert(0, "Any");

            // Reset original sector selection
            cmbSector.SelectedItem = cmbSector.Items.Contains(originalSector) ? originalSector : "Any";
            _applyFilter = true;
        }

        private static Cluster GetClusterFromJob(Job job)
        {
            if (string.IsNullOrWhiteSpace(job.Location?.Macro)) return null;

            string jobLocation = job.Location.Macro;
            var allClusters = MainForm.Instance.AllClusters;

            foreach (var cluster in allClusters)
            {
                string clusterCode = $"PREFIX_CL_c{cluster.Value.Id:D3}_macro";
                if (cluster.Value.IsBaseGame)
                    clusterCode = $"{cluster.Value.BaseGameMapping}_macro";

                if (jobLocation.Equals(clusterCode, StringComparison.OrdinalIgnoreCase))
                    return cluster.Value;
                
                foreach (var sector in cluster.Value.Sectors)
                {
                    string sectorCode = $"PREFIX_SE_c{cluster.Value.Id:D3}_s{sector.Id:D3}_macro";
                    if (cluster.Value.IsBaseGame && sector.IsBaseGame)
                        sectorCode = $"{cluster.Value.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                    else if (cluster.Value.IsBaseGame)
                        sectorCode = $"PREFIX_SE_c{cluster.Value.BaseGameMapping}_s{sector.Id}_macro";

                    if (jobLocation.Equals(sectorCode, StringComparison.OrdinalIgnoreCase))
                        return cluster.Value;
                }
            }

            return null;
        }

        private void BtnExitJobWindow_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnResetFilter_Click(object sender, EventArgs e)
        {
            _applyFilter = false;
            var comboboxes = new[] { cmbBasket, cmbFaction, cmbOrder, cmbCluster, cmbSector };
            foreach (var cmb in comboboxes)
                cmb.SelectedItem = "Any";
            _applyFilter = true;

            // Apply filter only once
            UpdateAvailableFilterOptions();
            ApplyCurrentFilter();
        }

        public void ApplyCurrentFilter()
        {
            if (!_applyFilter) return;

            var suitableJobs = AllJobs.Values.ToList();

            // Remove jobs based on rules
            HandleFilterOption(cmbBasket, suitableJobs);
            HandleFilterOption(cmbFaction, suitableJobs);
            HandleFilterOption(cmbOrder, suitableJobs);
            HandleFilterOption(cmbCluster, suitableJobs);
            HandleFilterOption(cmbSector, suitableJobs);

            // Add all suitable jobs to the listbox
            ListJobs.Items.Clear();
            foreach (var job in suitableJobs)
                ListJobs.Items.Add(job);
        }

        private void HandleFilterOption(ComboBox comboBox, List<Job> jobs)
        {
            // General "Any" check
            var value = comboBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(value) && value.Equals("Any", StringComparison.OrdinalIgnoreCase))
                return;

            if (comboBox == cmbBasket)
            {
                var basket = cmbBasket.SelectedItem as string;
                jobs.RemoveAll(a => a.Basket?.Basket == null || !a.Basket.Basket.Equals(basket, StringComparison.OrdinalIgnoreCase));
            }
            else if (comboBox == cmbFaction)
            {
                // Focus on jobs where the ship is owned by the selected faction
                var faction = cmbFaction.SelectedItem as string;
                jobs.RemoveAll(a => a.Ship?.Owner == null || !a.Ship.Owner.Exact.Equals(faction, StringComparison.OrdinalIgnoreCase));
            }
            else if (comboBox == cmbOrder)
            {
                var order = cmbOrder.SelectedItem as string;
                jobs.RemoveAll(a => a.Orders?.Order?.Order == null || !a.Orders.Order.Order.Equals(order, StringComparison.OrdinalIgnoreCase));
            }
            else if (comboBox == cmbCluster)
            {
                var cluster = cmbCluster.SelectedItem as Cluster;
                string clusterCode = $"PREFIX_CL_c{cluster.Id:D3}";
                if (cluster.IsBaseGame)
                    clusterCode = $"{cluster.BaseGameMapping}";
                jobs.RemoveAll(a => a.Location?.Macro == null || !a.Location.Macro.StartsWith(clusterCode, StringComparison.OrdinalIgnoreCase));
            }
            else if (comboBox == cmbSector)
            {
                var sector = cmbSector.SelectedItem as Sector;
                var cluster = cmbCluster.SelectedItem as Cluster;

                string sectorCode = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}";
                if (cluster.IsBaseGame && sector.IsBaseGame)
                    sectorCode = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}";
                else if (cluster.IsBaseGame)
                    sectorCode = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}";

                jobs.RemoveAll(a => a.Location?.Macro == null || !a.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                throw new NotImplementedException($"Combobox \"{comboBox.Name}\" implementation not available.");
            }
        }

        private void CmbFaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
        }

        private void CmbOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
        }

        private void CmbBasket_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
        }

        private void CmbCluster_SelectedIndexChanged(object sender, EventArgs e)
        {
            // To adjust sector options
            if (_applyFilter)
                UpdateAvailableFilterOptions();
            ApplyCurrentFilter();
        }

        private void CmbSector_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
        }

        private void BtnCreateCustom_Click(object sender, EventArgs e)
        {
            // Job creation/edit is ongoing at the moment
            if (JobForm != null && !JobForm.IsDisposed && JobForm.Visible) return;

            // Template selection is ongoing at the moment
            if (JobTemplatesForm != null && !JobTemplatesForm.IsDisposed && JobTemplatesForm.Visible) return;

            JobForm.Show();
        }

        private void BtnBaskets_Click(object sender, EventArgs e)
        {
            BasketsForm.Show();
        }

        private void BtnCreateFromTemplate_Click(object sender, EventArgs e)
        {
            // Job creation/edit is ongoing at the moment
            if (JobForm != null && !JobForm.IsDisposed && JobForm.Visible) return;

            // Template selection is ongoing at the moment
            if (JobTemplatesForm != null && !JobTemplatesForm.IsDisposed && JobTemplatesForm.Visible) return;

            JobTemplatesForm.JobForm = JobForm;
            JobTemplatesForm.Show();
        }

        private void BtnRemoveJob_Click(object sender, EventArgs e)
        {
            // Job creation/edit is ongoing at the moment
            if (JobForm != null && !JobForm.IsDisposed && JobForm.Visible) return;

            if (ListJobs.SelectedItem is Job job)
            {
                int index = ListJobs.Items.IndexOf(ListJobs.SelectedItem);
                ListJobs.Items.Remove(ListJobs.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ListJobs.SelectedItem = index >= 0 && ListJobs.Items.Count > 0 ? ListJobs.Items[index] : null;

                // Remove also from jobs collection itself
                AllJobs.Remove(job.Id);

                UpdateAvailableFilterOptions();
            }
        }

        private void ListJobs_DoubleClick(object sender, EventArgs e)
        {
            if (ListJobs.SelectedItem is not Job job) return;

            // Template selection is ongoing at the moment
            if (JobTemplatesForm != null && !JobTemplatesForm.IsDisposed && JobTemplatesForm.Visible) return;

            // Job creation/edit is ongoing at the moment
            if (JobForm != null && !JobForm.IsDisposed && JobForm.Visible) return;

            JobForm.IsEditing = true;
            JobForm.Job = job;
            JobForm.Show();
        }
    }
}
