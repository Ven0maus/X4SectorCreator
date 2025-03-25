using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactoriesForm : Form
    {
        public static readonly Dictionary<string, Factory> AllFactories = new(StringComparer.OrdinalIgnoreCase);
        public readonly LazyEvaluated<FactoryForm> FactoryForm = new(() => new FactoryForm(), a => !a.IsDisposed);
        public readonly LazyEvaluated<FactoryTemplatesForm> FactoryTemplatesForm = new(() => new FactoryTemplatesForm(), a => !a.IsDisposed);

        private bool _applyFilter = true;

        public FactoriesForm()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            // Clear existing values for the filter options
            cmbFaction.Items.Clear();
            cmbCluster.Items.Clear();
            cmbSector.Items.Clear();

            // Set new default values for filter options
            UpdateAvailableFilterOptions();

            // By default set for each option "Any"
            _applyFilter = false;
            var comboboxes = new[] { cmbFaction, cmbCluster, cmbSector };
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
            var originalCluster = cmbCluster.SelectedItem ?? "Any";
            var originalSector = cmbSector.SelectedItem ?? "Any";

            // Factions
            cmbFaction.Items.Clear();
            foreach (var value in AllFactories.Select(a => a.Value.Ship?.Owner?.Exact).Distinct(StringComparer.OrdinalIgnoreCase).Where(a => a != null).OrderBy(a => a))
                cmbFaction.Items.Add(value);
            cmbFaction.Items.Insert(0, "Any");

            // Clusters
            cmbCluster.Items.Clear();
            foreach (var cluster in AllFactories.Values.Select(GetClusterFromFactory).Where(a => a != null).Distinct().OrderBy(a => a.Name))
                cmbCluster.Items.Add(cluster);
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
                    foreach (var sector in cluster.Sectors.OrderBy(a => a.Name))
                    {
                        string sectorCode = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
                        if (cluster.IsBaseGame && sector.IsBaseGame)
                            sectorCode = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                        else if (cluster.IsBaseGame)
                            sectorCode = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}_macro";

                        // Check if a factory exists for this sector, then add the sector
                        if (AllFactories.Any(a => a.Value.Location?.Macro != null && a.Value.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase)))
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

        private static Cluster GetClusterFromFactory(Factory factory)
        {
            if (string.IsNullOrWhiteSpace(factory.Location?.Macro)) return null;

            string factoryLocation = factory.Location.Macro;
            var allClusters = MainForm.Instance.AllClusters;

            foreach (var cluster in allClusters)
            {
                string clusterCode = $"PREFIX_CL_c{cluster.Value.Id:D3}_macro";
                if (cluster.Value.IsBaseGame)
                    clusterCode = $"{cluster.Value.BaseGameMapping}_macro";

                if (factoryLocation.Equals(clusterCode, StringComparison.OrdinalIgnoreCase))
                    return cluster.Value;
                
                foreach (var sector in cluster.Value.Sectors)
                {
                    string sectorCode = $"PREFIX_SE_c{cluster.Value.Id:D3}_s{sector.Id:D3}_macro";
                    if (cluster.Value.IsBaseGame && sector.IsBaseGame)
                        sectorCode = $"{cluster.Value.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                    else if (cluster.Value.IsBaseGame)
                        sectorCode = $"PREFIX_SE_c{cluster.Value.BaseGameMapping}_s{sector.Id}_macro";

                    if (factoryLocation.Equals(sectorCode, StringComparison.OrdinalIgnoreCase))
                        return cluster.Value;
                }
            }

            return null;
        }

        private void BtnExitFactoryWindow_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnResetFilter_Click(object sender, EventArgs e)
        {
            _applyFilter = false;
            var comboboxes = new[] { cmbFaction, cmbCluster, cmbSector };
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

            var suitableFactories = AllFactories.Values.ToList();

            // Remove factories based on rules
            HandleFilterOption(cmbFaction, suitableFactories);
            HandleFilterOption(cmbCluster, suitableFactories);
            HandleFilterOption(cmbSector, suitableFactories);

            // Add all suitable factories to the listbox
            ListFactories.Items.Clear();
            foreach (var factory in suitableFactories)
                ListFactories.Items.Add(factory);
        }

        private void HandleFilterOption(ComboBox comboBox, List<Factory> factories)
        {
            // General "Any" check
            var value = comboBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(value) && value.Equals("Any", StringComparison.OrdinalIgnoreCase))
                return;

            if (comboBox == cmbFaction)
            {

            }
            else if (comboBox == cmbCluster)
            {

            }
            else if (comboBox == cmbSector)
            {

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
            // Factory creation/edit is ongoing at the moment
            if (FactoryForm.IsInitialized && FactoryForm.Value.Visible) return;

            // Template selection is ongoing at the moment
            if (FactoryTemplatesForm.IsInitialized && FactoryTemplatesForm.Value.Visible) return;

            FactoryForm.Value.Show();
        }

        private void BtnCreateFromTemplate_Click(object sender, EventArgs e)
        {
            // Factory creation/edit is ongoing at the moment
            if (FactoryForm.IsInitialized && FactoryForm.Value.Visible) return;

            // Template selection is ongoing at the moment
            if (FactoryTemplatesForm.IsInitialized && FactoryTemplatesForm.Value.Visible) return;

            FactoryTemplatesForm.Value.ProductForm = FactoryForm.Value;
            FactoryTemplatesForm.Value.Show();
        }

        private void BtnRemoveFactory_Click(object sender, EventArgs e)
        {
            // Factory creation/edit is ongoing at the moment
            if (FactoryForm.IsInitialized && FactoryForm.Value.Visible) return;

            if (ListFactories.SelectedItem is Factory factory)
            {
                int index = ListFactories.Items.IndexOf(ListFactories.SelectedItem);
                ListFactories.Items.Remove(ListFactories.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ListFactories.SelectedItem = index >= 0 && ListFactories.Items.Count > 0 ? ListFactories.Items[index] : null;

                // Remove also from factories collection itself
                AllFactories.Remove(factory.Id);

                UpdateAvailableFilterOptions();
            }
        }

        private void ListFactories_DoubleClick(object sender, EventArgs e)
        {
            if (ListFactories.SelectedItem is not Factory factory) return;

            // Template selection is ongoing at the moment
            if (FactoryTemplatesForm.IsInitialized && FactoryTemplatesForm.Value.Visible) return;

            // Factory creation/edit is ongoing at the moment
            if (FactoryForm.IsInitialized && FactoryForm.Value.Visible) return;

            FactoryForm.Value.IsEditing = true;
            FactoryForm.Value.Factory = factory;
            FactoryForm.Value.Show();
        }
    }
}
