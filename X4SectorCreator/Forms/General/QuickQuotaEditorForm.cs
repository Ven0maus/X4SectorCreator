using System.ComponentModel;
using System.Globalization;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class QuickQuotaEditorForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactoriesForm FactoriesForm { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobsForm JobsForm { get; set; }

        private bool _applyFilter = true;

        private readonly List<DataGridObject> _dataGridObjects = new();

        public QuickQuotaEditorForm()
        {
            InitializeComponent();

            // Enables text search component
            TxtSearch.EnableTextSearch(_dataGridObjects, a => a.Id, ApplyCurrentFilter);

            QuotaView.CellValidating += QuotaView_CellValidating;
            QuotaView.CellValidated += QuotaView_CellValidated;

            Disposed += QuickQuotaEditorForm_Disposed;
        }

        private void QuickQuotaEditorForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        public void Initialize()
        {
            // Setup data grid values
            if (JobsForm != null)
            {
                MaxGalaxy.ReadOnly = false;
                Wing.ReadOnly = false;
                foreach (var job in JobsForm.AllJobs)
                {
                    _dataGridObjects.Add(new DataGridObject(job.Value));
                }
            }
            else if (FactoriesForm != null)
            {
                MaxGalaxy.ReadOnly = true;
                Wing.ReadOnly = true;
                foreach (var factory in FactoriesForm.AllFactories)
                {
                    _dataGridObjects.Add(new DataGridObject(factory.Value));
                }
            }
            else
            {
                throw new Exception("No JobsForm or FactoriesForm set on QuickQuotaEditorForm. Invalid state, report a bug!");
            }

            // Filter initializes after data grid objects are initialized
            InitFilterComponent();
        }

        private void QuotaView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            // Update the data grid object
            var dataGridObject = _dataGridObjects.ElementAt(e.RowIndex);
            if (dataGridObject == null) return;

            var row = QuotaView.Rows[e.RowIndex];
            dataGridObject.GalaxyQuota = (row.Cells[(int)Quota.Galaxy].Value as string)?.Trim();
            dataGridObject.ClusterQuota = (row.Cells[(int)Quota.Cluster].Value as string)?.Trim();
            dataGridObject.SectorQuota = (row.Cells[(int)Quota.Sector].Value as string)?.Trim();
            dataGridObject.MaxGalaxyQuota = (row.Cells[(int)Quota.MaxGalaxy].Value as string)?.Trim();
            dataGridObject.WingQuota = (row.Cells[(int)Quota.Wing].Value as string)?.Trim();
        }

        private void QuotaView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var value = e.FormattedValue as string;
            if (string.IsNullOrWhiteSpace(value)) return;

            if (!ValidateIndex(e.RowIndex, e.ColumnIndex, value))
            {
                MessageBox.Show("You must set a valid integer value or empty value for this quota.");
                e.Cancel = true;
            }
        }

        private static bool IsValidValue(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && int.TryParse(value, CultureInfo.InvariantCulture, out _);
        }

        private bool ValidateIndex(int row, int index, string value)
        {
            // Validate if the given cell is a match with the object
            if (JobsForm != null)
            {
                var job = JobsForm.AllJobs[QuotaView.Rows[row].Cells[0].Value as string];
                switch ((Quota)index)
                {
                    case Quota.Galaxy:
                        if (job.Quota?.Galaxy != null && !string.IsNullOrWhiteSpace(job.Quota?.Galaxy))
                            return IsValidValue(value);
                        break;
                    case Quota.Cluster:
                        if (job.Quota?.Cluster != null && !string.IsNullOrWhiteSpace(job.Quota?.Cluster))
                            return IsValidValue(value);
                        break;
                    case Quota.Sector:
                        if (job.Quota?.Sector != null && !string.IsNullOrWhiteSpace(job.Quota?.Sector))
                            return IsValidValue(value);
                        break;
                    case Quota.MaxGalaxy:
                        if (job.Quota?.Maxgalaxy != null && !string.IsNullOrWhiteSpace(job.Quota?.Maxgalaxy))
                            return IsValidValue(value);
                        break;
                    case Quota.Wing:
                        if (job.Quota?.Wing != null && !string.IsNullOrWhiteSpace(job.Quota?.Wing))
                            return IsValidValue(value);
                        break;
                }
                return true;
            }
            else if (FactoriesForm != null)
            {
                var factory = FactoriesForm.AllFactories[QuotaView.Rows[row].Cells[0].Value as string];
                switch ((Quota)index)
                {
                    case Quota.Galaxy:
                        if (factory.Quotas?.Quota?.Galaxy != null && !string.IsNullOrWhiteSpace(factory.Quotas?.Quota?.Galaxy))
                            return IsValidValue(value);
                        break;
                    case Quota.Cluster:
                        if (factory.Quotas?.Quota?.Cluster != null && !string.IsNullOrWhiteSpace(factory.Quotas?.Quota?.Cluster))
                            return IsValidValue(value);
                        break;
                    case Quota.Sector:
                        if (factory.Quotas?.Quota?.Sector != null && !string.IsNullOrWhiteSpace(factory.Quotas?.Quota?.Sector))
                            return IsValidValue(value);
                        break;
                    case Quota.MaxGalaxy:
                    case Quota.Wing:
                        return false; // Not supported on factory
                }
                return true;
            }
            return true;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _dataGridObjects.Count; i++)
            {
                var dataGridObject = _dataGridObjects[i];
                if (JobsForm != null)
                {
                    var job = JobsForm.AllJobs[dataGridObject.Id];

                    if (!string.IsNullOrWhiteSpace(dataGridObject.GalaxyQuota))
                        job.Quota.Galaxy = dataGridObject.GalaxyQuota;

                    if (!string.IsNullOrWhiteSpace(dataGridObject.ClusterQuota))
                        job.Quota.Cluster = dataGridObject.ClusterQuota;

                    if (!string.IsNullOrWhiteSpace(dataGridObject.SectorQuota))
                        job.Quota.Sector = dataGridObject.SectorQuota;

                    if (!string.IsNullOrWhiteSpace(dataGridObject.MaxGalaxyQuota))
                        job.Quota.Maxgalaxy = dataGridObject.MaxGalaxyQuota;

                    if (!string.IsNullOrWhiteSpace(dataGridObject.WingQuota))
                        job.Quota.Wing = dataGridObject.WingQuota;
                }
                else if (FactoriesForm != null)
                {
                    var factory = FactoriesForm.AllFactories[dataGridObject.Id];

                    if (!string.IsNullOrWhiteSpace(dataGridObject.GalaxyQuota))
                        factory.Quotas.Quota.Galaxy = dataGridObject.GalaxyQuota;

                    if (!string.IsNullOrWhiteSpace(dataGridObject.ClusterQuota))
                        factory.Quotas.Quota.Cluster = dataGridObject.ClusterQuota;

                    if (!string.IsNullOrWhiteSpace(dataGridObject.SectorQuota))
                        factory.Quotas.Quota.Sector = dataGridObject.SectorQuota;
                }
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            var dialog = MessageBox.Show("Are you sure you want to cancel?", "Are you sure?", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
                Close();
        }

        private void InitFilterComponent()
        {
            // Clear existing values for the filter options
            cmbFaction.Items.Clear();
            cmbCluster.Items.Clear();
            cmbSector.Items.Clear();

            // Set new default values for filter options
            UpdateAvailableFilterOptions();

            // By default set for each option "Any"
            _applyFilter = false;
            ComboBox[] comboboxes = new[] { cmbFaction, cmbCluster, cmbSector };
            foreach (ComboBox cmb in comboboxes)
            {
                cmb.SelectedItem = "Any";
            }

            _applyFilter = true;

            // Apply the filter
            ApplyCurrentFilter();
        }

        private void ApplyCurrentFilter(List<DataGridObject> dataGridObjects = null)
        {
            if (!_applyFilter)
            {
                return;
            }

            List<DataGridObject> suitableObjects = dataGridObjects ?? [.. _dataGridObjects];

            // Remove factories based on rules
            HandleFilterOption(cmbFaction, suitableObjects);
            HandleFilterOption(cmbCluster, suitableObjects);
            HandleFilterOption(cmbSector, suitableObjects);

            // Add all suitable factories to the listbox
            QuotaView.Rows.Clear();
            foreach (DataGridObject dataGridObject in suitableObjects)
            {
                _ = QuotaView.Rows.Add(dataGridObject.Id, dataGridObject.GalaxyQuota, dataGridObject.ClusterQuota, dataGridObject.SectorQuota, dataGridObject.MaxGalaxyQuota, dataGridObject.WingQuota);
            }
        }

        private void HandleFilterOption(ComboBox comboBox, List<DataGridObject> dataGridObjects)
        {
            // General "Any" check
            string value = comboBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(value) && value.Equals("Any", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (comboBox == cmbFaction)
            {
                string owner = cmbFaction.SelectedItem as string;
                if (owner != null)
                {
                    _ = dataGridObjects.RemoveAll(a => string.IsNullOrWhiteSpace(a.Faction) || !a.Faction.Equals(owner, StringComparison.OrdinalIgnoreCase));
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

                    _ = dataGridObjects.RemoveAll(a => a.ClusterObj == null || !a.ClusterObj.Equals(cluster));
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

                    _ = dataGridObjects.RemoveAll(a => a.SectorObj == null || !a.SectorObj.Equals(sector));
                }
            }
            else
            {
                throw new NotImplementedException($"Combobox \"{comboBox.Name}\" implementation not available.");
            }
        }

        public void UpdateAvailableFilterOptions()
        {
            _applyFilter = false;
            object originalFaction = cmbFaction.SelectedItem ?? "Any";
            object originalCluster = cmbCluster.SelectedItem ?? "Any";
            object originalSector = cmbSector.SelectedItem ?? "Any";

            // Factions
            var allFactions = new HashSet<string>(_dataGridObjects
                .Select(obj => obj.Faction)
                .Where(a => a != null)
                .Select(f => f.Trim()) // clean up whitespace
                .Where(f => !string.IsNullOrWhiteSpace(f)) // ignore blanks
            );
            cmbFaction.Items.Clear();
            foreach (string value in allFactions.OrderBy(a => a))
            {
                _ = cmbFaction.Items.Add(value);
            }
            cmbFaction.Items.Insert(0, "Any");

            // Clusters
            cmbCluster.Items.Clear();
            foreach (Cluster cluster in _dataGridObjects
                .Select(a => a.ClusterObj)
                .Where(a => a != null)
                .Distinct()
                .OrderBy(a => a.Name))
            {
                _ = cmbCluster.Items.Add(cluster);
            }

            cmbCluster.Items.Insert(0, "Any");

            // Reset original selected values if still available
            cmbFaction.SelectedItem = cmbFaction.Items.Contains(originalFaction) ? originalFaction : "Any";
            cmbCluster.SelectedItem = cmbCluster.Items.Contains(originalCluster) ? originalCluster : "Any";

            // Sectors is exceptional, only populated when a cluster is selected
            cmbSector.Items.Clear();
            if (cmbCluster.SelectedItem != null)
            {
                if (cmbCluster.SelectedItem is Cluster cluster)
                {
                    foreach (Sector sector in cluster.Sectors.OrderBy(a => a.Name))
                    {
                        // Check if a factory exists for this sector, then add the sector
                        if (_dataGridObjects.Any(a => a.SectorObj != null && a.SectorObj == sector))
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

        private static Sector GetSectorFromJob(Job job, Cluster cluster)
        {
            if (cluster == null) return null;
            if (job.Location?.Class != null &&
                job.Location.Class.Equals("Sector", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var sector in cluster.Sectors)
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
            }
            return null;
        }

        private static Sector GetSectorFromFactory(Factory factory, Cluster cluster)
        {
            if (cluster == null) return null;
            if (factory.Location?.Class != null &&
                factory.Location.Class.Equals("Sector", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var sector in cluster.Sectors)
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

                    if (factory.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase))
                    {
                        return sector;
                    }
                }
            }
            return null;
        }

        private static Cluster GetClusterFromFactory(Factory factory)
        {
            if (string.IsNullOrWhiteSpace(factory.Location?.Macro))
            {
                return null;
            }

            string factoryLocation = factory.Location.Macro;
            Dictionary<(int, int), Cluster> allClusters = MainForm.Instance.AllClusters;

            foreach (KeyValuePair<(int, int), Cluster> cluster in allClusters)
            {
                string clusterCode = $"PREFIX_CL_c{cluster.Value.Id:D3}_macro";
                if (cluster.Value.IsBaseGame)
                {
                    clusterCode = $"{cluster.Value.BaseGameMapping}_macro";
                }

                if (factoryLocation.Equals(clusterCode, StringComparison.OrdinalIgnoreCase))
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

                    if (factoryLocation.Equals(sectorCode, StringComparison.OrdinalIgnoreCase))
                    {
                        return cluster.Value;
                    }
                }
            }

            return null;
        }

        class DataGridObject
        {
            public string Id { get; set; }
            public string Faction { get; set; }
            public Cluster ClusterObj { get; set; }
            public Sector SectorObj { get; set; }
            public string SectorName { get; set; }
            public string GalaxyQuota { get; set; }
            public string ClusterQuota { get; set; }
            public string SectorQuota { get; set; }
            public string MaxGalaxyQuota { get; set; }
            public string WingQuota { get; set; }

            public DataGridObject(Factory factory)
            {
                Id = factory.Id;
                GalaxyQuota = factory.Quotas?.Quota?.Galaxy;
                ClusterQuota = factory.Quotas?.Quota?.Cluster;
                SectorQuota = factory.Quotas?.Quota?.Sector;
                Faction = factory.Owner;
                ClusterObj = GetClusterFromFactory(factory);
                SectorObj = GetSectorFromFactory(factory, ClusterObj);
            }

            public DataGridObject(Job job)
            {
                Id = job.Id;
                GalaxyQuota = job.Quota?.Galaxy;
                ClusterQuota = job.Quota?.Cluster;
                SectorQuota = job.Quota?.Sector;
                Faction = job.Ship?.Owner?.Exact;
                MaxGalaxyQuota = job.Quota?.Maxgalaxy;
                WingQuota = job.Quota?.Wing;
                ClusterObj = GetClusterFromJob(job);
                SectorObj = GetSectorFromJob(job, ClusterObj);
            }
        }

        enum Quota
        {
            Galaxy = 1,
            Cluster = 2,
            Sector = 3,
            MaxGalaxy = 4,
            Wing = 5
        }

        private void cmbFaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
        }

        private void cmbCluster_SelectedIndexChanged(object sender, EventArgs e)
        {
            // To adjust sector options
            if (_applyFilter)
            {
                UpdateAvailableFilterOptions();
            }

            ApplyCurrentFilter();
        }

        private void cmbSector_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
        }

        private void BtnResetFilter_Click(object sender, EventArgs e)
        {
            _applyFilter = false;
            ComboBox[] comboboxes = new[] { cmbFaction, cmbCluster, cmbSector };
            foreach (ComboBox cmb in comboboxes)
            {
                cmb.SelectedItem = "Any";
            }

            _applyFilter = true;

            // Apply filter only once
            UpdateAvailableFilterOptions();
            ApplyCurrentFilter();
        }
    }
}
