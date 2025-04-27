using X4SectorCreator.Forms.Factories;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobsForm : Form
    {
        public static readonly Dictionary<string, Job> AllJobs = new(StringComparer.OrdinalIgnoreCase);
        public static readonly Dictionary<string, Basket> AllBaskets = new(StringComparer.OrdinalIgnoreCase);

        private readonly LazyEvaluated<JobForm> _jobForm = new(() => new JobForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<JobTemplatesForm> _jobTemplatesForm = new(() => new JobTemplatesForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<BasketsForm> _basketsForm = new(() => new BasketsForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<QuickQuotaEditorForm> _quickQuotaEditorForm = new(() => new QuickQuotaEditorForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<PresetSelectionForm> _presetSelectionForm = new(() => new PresetSelectionForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<MultiTemplateSelectorForm> _multiTemplateSelector = new(() => new MultiTemplateSelectorForm(), a => !a.IsDisposed);

        private bool _applyFilter = true;

        public JobsForm()
        {
            InitializeComponent();

            TxtSearch.EnableTextSearch(() => AllJobs.Values.ToList(), a => a.Id, ApplyCurrentFilter);
            Disposed += JobsForm_Disposed;
        }

        private void JobsForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
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
            ComboBox[] comboboxes = new[] { cmbBasket, cmbFaction, cmbOrder, cmbCluster, cmbSector };
            foreach (ComboBox cmb in comboboxes)
            {
                cmb.SelectedItem = "Any";
            }

            _applyFilter = true;

            // Apply the filter
            ApplyCurrentFilter();
        }

        public void UpdateAvailableFilterOptions()
        {
            _applyFilter = false;
            object originalFaction = cmbFaction.SelectedItem ?? "Any";
            object originalBasket = cmbBasket.SelectedItem ?? "Any";
            object originalOrder = cmbOrder.SelectedItem ?? "Any";
            object originalCluster = cmbCluster.SelectedItem ?? "Any";
            object originalSector = cmbSector.SelectedItem ?? "Any";

            // Factions
            cmbFaction.Items.Clear();
            foreach (string value in AllJobs.Select(a => a.Value.Ship?.Owner?.Exact).Where(a => a != null).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(a => a))
            {
                _ = cmbFaction.Items.Add(value);
            }

            cmbFaction.Items.Insert(0, "Any");

            // Baskets
            cmbBasket.Items.Clear();
            foreach (string basket in AllJobs.Select(a => a.Value.Basket?.Basket).Where(a => a != null).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(a => a))
            {
                _ = cmbBasket.Items.Add(basket);
            }

            cmbBasket.Items.Insert(0, "Any");

            // Orders
            cmbOrder.Items.Clear();
            foreach (string value in AllJobs.Select(a => a.Value.Orders?.Order?.Order).Where(a => a != null).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(a => a))
            {
                _ = cmbOrder.Items.Add(value);
            }

            cmbOrder.Items.Insert(0, "Any");

            // Clusters
            cmbCluster.Items.Clear();
            foreach (Cluster cluster in AllJobs.Values.Select(GetClusterFromJob).Where(a => a != null).Distinct().OrderBy(a => a.Name))
            {
                _ = cmbCluster.Items.Add(cluster);
            }

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
                    foreach (Sector sector in cluster.Sectors.OrderBy(a => a.Name))
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

                        // Check if a job exists for this sector, then add the sector
                        if (AllJobs.Any(a => a.Value.Location?.Macro != null && a.Value.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase)))
                        {
                            _ = cmbSector.Items.Add(sector);
                        }
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

        private void BtnExitJobWindow_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnResetFilter_Click(object sender, EventArgs e)
        {
            _applyFilter = false;
            ComboBox[] comboboxes = new[] { cmbBasket, cmbFaction, cmbOrder, cmbCluster, cmbSector };
            foreach (ComboBox cmb in comboboxes)
            {
                cmb.SelectedItem = "Any";
            }

            _applyFilter = true;

            // Apply filter only once
            UpdateAvailableFilterOptions();
            ApplyCurrentFilter();
        }

        public void ApplyCurrentFilter(List<Job> jobs = null)
        {
            if (!_applyFilter)
            {
                return;
            }

            List<Job> suitableJobs = jobs ?? [.. AllJobs.Values];

            // Remove jobs based on rules
            HandleFilterOption(cmbBasket, suitableJobs);
            HandleFilterOption(cmbFaction, suitableJobs);
            HandleFilterOption(cmbOrder, suitableJobs);
            HandleFilterOption(cmbCluster, suitableJobs);
            HandleFilterOption(cmbSector, suitableJobs);

            // Add all suitable jobs to the listbox
            ListJobs.Items.Clear();
            foreach (Job job in suitableJobs)
            {
                _ = ListJobs.Items.Add(job);
            }
        }

        private void HandleFilterOption(ComboBox comboBox, List<Job> jobs)
        {
            // General "Any" check
            string value = comboBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(value) && value.Equals("Any", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (comboBox == cmbBasket)
            {
                string basket = cmbBasket.SelectedItem as string;
                if (basket != null)
                {
                    _ = jobs.RemoveAll(a => a.Basket?.Basket == null || !a.Basket.Basket.Equals(basket, StringComparison.OrdinalIgnoreCase));
                }
            }
            else if (comboBox == cmbFaction)
            {
                // Focus on jobs where the ship is owned by the selected faction
                string faction = cmbFaction.SelectedItem as string;
                if (faction != null)
                {
                    _ = jobs.RemoveAll(a => a.Ship?.Owner == null || !a.Ship.Owner.Exact.Equals(faction, StringComparison.OrdinalIgnoreCase));
                }
            }
            else if (comboBox == cmbOrder)
            {
                string order = cmbOrder.SelectedItem as string;
                if (order != null)
                {
                    _ = jobs.RemoveAll(a => a.Orders?.Order?.Order == null || !a.Orders.Order.Order.Equals(order, StringComparison.OrdinalIgnoreCase));
                }
            }
            else if (comboBox == cmbCluster)
            {
                Cluster cluster = cmbCluster.SelectedItem as Cluster;
                if (cluster != null)
                {
                    string clusterCode = $"PREFIX_CL_c{cluster.Id:D3}";
                    if (cluster.IsBaseGame)
                    {
                        clusterCode = $"{cluster.BaseGameMapping}";
                    }

                    _ = jobs.RemoveAll(a => a.Location?.Macro == null || !a.Location.Macro.StartsWith(clusterCode, StringComparison.OrdinalIgnoreCase));
                }
            }
            else if (comboBox == cmbSector)
            {
                Sector sector = cmbSector.SelectedItem as Sector;
                Cluster cluster = cmbCluster.SelectedItem as Cluster;

                if (cluster != null && sector != null)
                {
                    string sectorCode = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}";
                    if (cluster.IsBaseGame && sector.IsBaseGame)
                    {
                        sectorCode = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}";
                    }
                    else if (cluster.IsBaseGame)
                    {
                        sectorCode = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}";
                    }

                    _ = jobs.RemoveAll(a => a.Location?.Macro == null || !a.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase));
                }
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
            {
                UpdateAvailableFilterOptions();
            }

            ApplyCurrentFilter();
        }

        private void CmbSector_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
        }

        private void BtnCreateCustom_Click(object sender, EventArgs e)
        {
            // Job creation/edit is ongoing at the moment
            if (_jobForm.IsInitialized && _jobForm.Value.Visible)
            {
                return;
            }

            // Template selection is ongoing at the moment
            if (_jobTemplatesForm.IsInitialized && _jobTemplatesForm.Value.Visible)
            {
                return;
            }

            _jobForm.Value.Show();
        }

        private void BtnBaskets_Click(object sender, EventArgs e)
        {
            _basketsForm.Value.Show();
        }

        private void BtnCreateFromTemplate_Click(object sender, EventArgs e)
        {
            // Job creation/edit is ongoing at the moment
            if (_jobForm.IsInitialized && _jobForm.Value.Visible)
            {
                return;
            }

            // Template selection is ongoing at the moment
            if (_jobTemplatesForm.IsInitialized && _jobTemplatesForm.Value.Visible)
            {
                return;
            }

            _jobTemplatesForm.Value.JobForm = _jobForm.Value;
            _jobTemplatesForm.Value.Show();
        }

        private void BtnRemoveJob_Click(object sender, EventArgs e)
        {
            // Job creation/edit is ongoing at the moment
            if (_jobForm.IsInitialized && _jobForm.Value.Visible)
            {
                return;
            }

            if (ListJobs.SelectedItem is Job job)
            {
                int index = ListJobs.Items.IndexOf(ListJobs.SelectedItem);
                ListJobs.Items.Remove(ListJobs.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ListJobs.SelectedItem = index >= 0 && ListJobs.Items.Count > 0 ? ListJobs.Items[index] : null;

                // Remove also from jobs collection itself
                _ = AllJobs.Remove(job.Id);

                UpdateAvailableFilterOptions();
            }
        }

        private void ListJobs_DoubleClick(object sender, EventArgs e)
        {
            if (ListJobs.SelectedItem is not Job job)
            {
                return;
            }

            // Template selection is ongoing at the moment
            if (_jobTemplatesForm.IsInitialized && _jobTemplatesForm.Value.Visible)
            {
                return;
            }

            // Job creation/edit is ongoing at the moment
            if (_jobForm.IsInitialized && _jobForm.Value.Visible)
            {
                return;
            }

            _jobForm.Value.IsEditing = true;
            _jobForm.Value.Job = job;
            _jobForm.Value.Show();
        }

        private void BtnQuickQuotaEditor_Click(object sender, EventArgs e)
        {
            _quickQuotaEditorForm.Value.JobsForm = this;
            _quickQuotaEditorForm.Value.Initialize();
            _quickQuotaEditorForm.Value.Show();
        }

        private void BtnClearAllJobs_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all custom jobs? This is a destructive operation!",
                "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                AllJobs.Clear();
                ApplyCurrentFilter();
            }
        }

        private void BtnCreateJobsFromPreset_Click(object sender, EventArgs e)
        {
            _presetSelectionForm.Value.JobsForm = this;
            _presetSelectionForm.Value.FactoriesForm = null;
            _presetSelectionForm.Value.Show();
        }

        private void BtnAddSelection_Click(object sender, EventArgs e)
        {
            _multiTemplateSelector.Value.JobsForm = this;
            _multiTemplateSelector.Value.FactoriesForm = null;
            _multiTemplateSelector.Value.Show();
        }
    }
}
