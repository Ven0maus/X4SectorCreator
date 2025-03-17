using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobsForm : Form
    {
        private static readonly List<Job> _jobs = [];
        public static IReadOnlyList<Job> Jobs => _jobs;

        private static readonly List<Basket> _baskets = [];
        public static IReadOnlyList<Basket> Baskets => _baskets;

        private readonly string[] _factions;

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

            // Load job items into the listbox
            foreach (var job in Jobs.OrderBy(a => a.Name))
                ListJobs.Items.Add(job);

            // Clear existing values for the filter options
            cmbBasket.Items.Clear();
            cmbCommandeerable.Items.Clear();
            cmbFaction.Items.Clear();
            cmbOrder.Items.Clear();
            cmbCluster.Items.Clear();
            cmbSector.Items.Clear();

            // Set new default values for filter options

            // Baskets
            foreach (var basket in Baskets.OrderBy(a => a.Name))
                cmbBasket.Items.Add(basket.Name);
            cmbBasket.Items.Insert(0, "Any"); // Custom any filter

            // Commandeerable
            cmbCommandeerable.Items.AddRange("Any", "False", "True");

            // Factions
            foreach (var faction in _factions)
                cmbFaction.Items.Add(faction);
            cmbFaction.Items.Insert(0, "Any");

            // Order
            // TODO: Extract all different types of possible orders from jobs and show them here

            // Cluster
            foreach (var cluster in MainForm.Instance.AllClusters.OrderBy(a => a.Value.Name))
                cmbCluster.Items.Add(cluster.Value);
            cmbCluster.Items.Insert(0, "Any");

            // Sector
            foreach (var sector in MainForm.Instance.AllClusters.Values.SelectMany(a => a.Sectors).OrderBy(a => a.Name))
                cmbSector.Items.Add(sector);
            cmbSector.Items.Insert(0, "Any");

            // By default set for each option "Any"
            var comboboxes = new[] { cmbBasket, cmbCommandeerable, cmbFaction, cmbOrder, cmbCluster, cmbSector };
            foreach (var cmb in comboboxes)
                cmb.SelectedItem = "Any";
        }

        /// <summary>
        /// Used to initialize jobs from a config file.
        /// </summary>
        /// <param name="jobs"></param>
        public static void InitJobsFromConfig(List<Job> jobs)
        {
            ClearAllJobs();
            _jobs.AddRange(jobs);
        }

        /// <summary>
        /// Used to initialize baskets from a config file.
        /// </summary>
        /// <param name="jobs"></param>
        public static void InitBasketsFromConfig(List<Basket> baskets)
        {
            ClearAllBaskets();
            _baskets.AddRange(baskets);
        }

        /// <summary>
        /// Used to clear all stored jobs.
        /// </summary>
        public static void ClearAllJobs()
        {
            _jobs.Clear();
        }

        /// <summary>
        /// Used to clear all stored baskets.
        /// </summary>
        public static void ClearAllBaskets()
        {
            _baskets.Clear();
        }

        private void BtnExitJobWindow_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
