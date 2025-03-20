using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobsForm : Form
    {
        private static readonly List<Job> _allJobs = [];
        public static IReadOnlyList<Job> AllJobs => _allJobs;

        private static readonly List<Basket> _allBaskets = [];
        public static IReadOnlyList<Basket> AllBaskets => _allBaskets;

        private JobForm _jobForm;
        public JobForm JobForm => _jobForm != null && !_jobForm.IsDisposed ? _jobForm : (_jobForm = new JobForm());

        private JobTemplatesForm _jobTemplatesForm;
        public JobTemplatesForm JobTemplatesForm => _jobTemplatesForm != null && !_jobTemplatesForm.IsDisposed ? _jobTemplatesForm : (_jobTemplatesForm = new JobTemplatesForm());

        private BasketsForm _basketsForm;
        public BasketsForm BasketsForm => _basketsForm != null && !_basketsForm.IsDisposed ? _basketsForm : (_basketsForm = new BasketsForm());

        private readonly string[] _factions;
        private bool _applyFilter = true;

        public JobsForm()
        {
            InitializeComponent();

            _factions = [.. MainForm.Instance.FactionColorMapping
                .Select(a => a.Key)
                .Where(a => !a.Equals("None", StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a)];
        }

        public void Initialize()
        {
            ListJobs.Items.Clear();

            // Load all job items into the listbox
            foreach (var job in AllJobs.OrderBy(a => a.Name))
                ListJobs.Items.Add(job);

            // Clear existing values for the filter options
            cmbBasket.Items.Clear();
            cmbFaction.Items.Clear();
            cmbOrder.Items.Clear();
            cmbCluster.Items.Clear();
            cmbSector.Items.Clear();

            // Set new default values for filter options

            // Baskets
            UpdateBasketsFilter();

            // Factions
            foreach (var faction in _factions)
                cmbFaction.Items.Add(faction);
            cmbFaction.Items.Insert(0, "Any");

            // Order
            // TODO: Extract all different types of possible orders from jobs and show them here
            cmbOrder.Items.Insert(0, "Any");

            // Cluster
            foreach (var cluster in MainForm.Instance.AllClusters.OrderBy(a => a.Value.Name))
                cmbCluster.Items.Add(cluster.Value);
            cmbCluster.Items.Insert(0, "Any");

            // Sector
            foreach (var sector in MainForm.Instance.AllClusters.Values.SelectMany(a => a.Sectors).OrderBy(a => a.Name))
                cmbSector.Items.Add(sector);
            cmbSector.Items.Insert(0, "Any");

            // By default set for each option "Any"
            _applyFilter = false;
            var comboboxes = new[] { cmbBasket, cmbFaction, cmbOrder, cmbCluster, cmbSector };
            foreach (var cmb in comboboxes)
                cmb.SelectedItem = "Any";
            _applyFilter = true;
        }

        /// <summary>
        /// This method should be called when a basket is created / removed through the basket interface.
        /// <br>This will update the basket filters option to include the adjusted baskets.</br>
        /// </summary>
        private void UpdateBasketsFilter()
        {
            foreach (var basket in AllBaskets.OrderBy(a => a.Name))
                cmbBasket.Items.Add(basket.Name);
            cmbBasket.Items.Insert(0, "Any"); // Custom any filter
        }

        /// <summary>
        /// Used to initialize jobs from a config file.
        /// </summary>
        /// <param name="jobs"></param>
        public static void InitJobsFromConfig(List<Job> jobs)
        {
            ClearAllJobs();
            _allJobs.AddRange(jobs);
        }

        /// <summary>
        /// Used to initialize baskets from a config file.
        /// </summary>
        /// <param name="jobs"></param>
        public static void InitBasketsFromConfig(List<Basket> baskets)
        {
            ClearAllBaskets();
            _allBaskets.AddRange(baskets);
        }

        /// <summary>
        /// Used to clear all stored jobs.
        /// </summary>
        public static void ClearAllJobs()
        {
            _allJobs.Clear();
        }

        /// <summary>
        /// Used to clear all stored baskets.
        /// </summary>
        public static void ClearAllBaskets()
        {
            _allBaskets.Clear();
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
            ApplyCurrentFilter();
        }

        private void ApplyCurrentFilter()
        {
            if (!_applyFilter) return;

            var suitableJobs = AllJobs.ToList();

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
                jobs.RemoveAll(a => a.Basket == null || !a.Basket.Equals(basket, StringComparison.OrdinalIgnoreCase));
            }
            else if (comboBox == cmbFaction)
            {
                // Focus on jobs where the ship is owned by the selected faction
                var faction = cmbFaction.SelectedItem as string;
                jobs.RemoveAll(a => a.Ship?.Owner == null || !a.Ship.Owner.Equals(faction, StringComparison.OrdinalIgnoreCase));
            }
            else if (comboBox == cmbOrder)
            {
                var order = cmbOrder.SelectedItem as string;
                jobs.RemoveAll(a => a.Orders == null || a.Orders.Count == 0 || !a.Orders.Any(a => a.Name.Equals(order, StringComparison.OrdinalIgnoreCase)));
            }
            else if (comboBox == cmbCluster)
            {
                var cluster = (cmbCluster.SelectedItem as Cluster).Name;
                jobs.RemoveAll(a => a.Location.Name == null || !a.Location.Name.Equals(cluster, StringComparison.OrdinalIgnoreCase));
            }
            else if (comboBox == cmbSector)
            {
                var sector = (cmbSector.SelectedItem as Sector).Name;
                jobs.RemoveAll(a => a.Location.Name == null || !a.Location.Name.Equals(sector, StringComparison.OrdinalIgnoreCase));
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
            ApplyCurrentFilter();
        }

        private void CmbSector_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
        }

        private void BtnCreateCustom_Click(object sender, EventArgs e)
        {
            JobForm.Show();
        }

        private void BtnBaskets_Click(object sender, EventArgs e)
        {
            BasketsForm.Show();
        }

        private void BtnCreateFromTemplate_Click(object sender, EventArgs e)
        {
            JobTemplatesForm.Show();
        }

        private void BtnRemoveJob_Click(object sender, EventArgs e)
        {
            var job = ListJobs.SelectedItem as Job;
            if (job != null)
            {
                int index = ListJobs.Items.IndexOf(ListJobs.SelectedItem);
                ListJobs.Items.Remove(ListJobs.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ListJobs.SelectedItem = index >= 0 && ListJobs.Items.Count > 0 ? ListJobs.Items[index] : null;

                // Remove also from jobs collection itself
                _allJobs.Remove(job);
            }    
        }

        private void ListJobs_DoubleClick(object sender, EventArgs e)
        {
            var job = ListJobs.SelectedItem as Job;
            if (job == null) return;

            JobForm.Job = job;
            JobForm.Show();
        }
    }
}
