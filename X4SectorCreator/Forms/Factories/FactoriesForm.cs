using X4SectorCreator.Forms.Factories;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactoriesForm : Form
    {
        public static readonly Dictionary<string, Factory> AllFactories = new(StringComparer.OrdinalIgnoreCase);

        private readonly LazyEvaluated<FactoryForm> _factoryForm = new(() => new FactoryForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<FactoryTemplatesForm> _factoryTemplatesForm = new(() => new FactoryTemplatesForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<QuickQuotaEditorForm> _quickQuotaEditorForm = new(() => new QuickQuotaEditorForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<PresetSelectionForm> _presetSelectionForm = new(() => new PresetSelectionForm(), a => !a.IsDisposed);
        private readonly LazyEvaluated<MultiTemplateSelectorForm> _multiTemplateSelector = new(() => new MultiTemplateSelectorForm(), a => !a.IsDisposed);

        private bool _applyFilter = true;

        public FactoriesForm()
        {
            InitializeComponent();

            TxtSearch.EnableTextSearch(() => AllFactories.Values.ToList(), a => a.Id, ApplyCurrentFilter);
            Disposed += FactoriesForm_Disposed;
        }

        private void FactoriesForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        public void Initialize()
        {
            // Clear existing values for the filter options
            cmbFaction.Items.Clear();
            cmbCluster.Items.Clear();
            cmbSector.Items.Clear();
            CmbWare.Items.Clear();

            // Set new default values for filter options
            UpdateAvailableFilterOptions();

            // By default set for each option "Any"
            _applyFilter = false;
            ComboBox[] comboboxes = new[] { cmbFaction, cmbCluster, cmbSector, CmbWare };
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
            object originalCluster = cmbCluster.SelectedItem ?? "Any";
            object originalSector = cmbSector.SelectedItem ?? "Any";
            object originalWare = CmbWare.SelectedItem ?? "Any";

            // Factions
            var allFactions = new HashSet<string>(AllFactories
                .Select(obj => obj.Value.Owner)
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
            foreach (Cluster cluster in AllFactories.Values.Select(GetClusterFromFactory).Where(a => a != null).Distinct().OrderBy(a => a.Name))
            {
                _ = cmbCluster.Items.Add(cluster);
            }
            cmbCluster.Items.Insert(0, "Any");

            // Wares
            var allWares = AllFactories
                .Select(obj => obj.Value.Ware)
                .Where(a => a != null)
                .Select(f => f.Trim()) // clean up whitespace
                .Where(f => !string.IsNullOrWhiteSpace(f));
            CmbWare.Items.Clear();
            foreach (var ware in allWares)
            {
                _ = CmbWare.Items.Add(ware);
            }
            CmbWare.Items.Insert(0, "Any");

            // Reset original selected values if still available
            cmbFaction.SelectedItem = cmbFaction.Items.Contains(originalFaction) ? originalFaction : "Any";
            cmbCluster.SelectedItem = cmbCluster.Items.Contains(originalCluster) ? originalCluster : "Any";
            CmbWare.SelectedItem = CmbWare.Items.Contains(originalWare) ? originalWare : "Any";

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

                        // Check if a factory exists for this sector, then add the sector
                        if (AllFactories.Any(a => a.Value.Location?.Macro != null && a.Value.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase)))
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

        private void BtnExitFactoryWindow_Click(object sender, EventArgs e)
        {
            Close();
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

        public void ApplyCurrentFilter(List<Factory> factories = null)
        {
            if (!_applyFilter)
            {
                return;
            }

            List<Factory> suitableFactories = factories ?? [.. AllFactories.Values];

            // Remove factories based on rules
            HandleFilterOption(cmbFaction, suitableFactories);
            HandleFilterOption(cmbCluster, suitableFactories);
            HandleFilterOption(cmbSector, suitableFactories);
            HandleFilterOption(CmbWare, suitableFactories);

            // Add all suitable factories to the listbox
            ListFactories.Items.Clear();
            foreach (Factory factory in suitableFactories)
            {
                _ = ListFactories.Items.Add(factory);
            }
        }

        private void HandleFilterOption(ComboBox comboBox, List<Factory> factories)
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
                if (!string.IsNullOrWhiteSpace(owner))
                {
                    _ = factories.RemoveAll(a => string.IsNullOrWhiteSpace(a.Owner) || !a.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase));
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

                    _ = factories.RemoveAll(a => a.Location?.Macro == null || !a.Location.Macro.StartsWith(clusterCode, StringComparison.OrdinalIgnoreCase));
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

                    _ = factories.RemoveAll(a => a.Location?.Macro == null || !a.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase));
                }
            }
            else if (comboBox == CmbWare)
            {
                string ware = CmbWare.SelectedItem as string;
                if (!string.IsNullOrWhiteSpace(ware))
                {
                    _ = factories.RemoveAll(a => string.IsNullOrWhiteSpace(a.Ware) || !a.Ware.Equals(ware, StringComparison.OrdinalIgnoreCase));
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

        private void CmbWare_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentFilter();
        }

        private void BtnCreateCustom_Click(object sender, EventArgs e)
        {
            // Factory creation/edit is ongoing at the moment
            if (_factoryForm.IsInitialized && _factoryForm.Value.Visible)
            {
                return;
            }

            // Template selection is ongoing at the moment
            if (_factoryTemplatesForm.IsInitialized && _factoryTemplatesForm.Value.Visible)
            {
                return;
            }

            _factoryForm.Value.Show();
        }

        private void BtnCreateFromTemplate_Click(object sender, EventArgs e)
        {
            // Factory creation/edit is ongoing at the moment
            if (_factoryForm.IsInitialized && _factoryForm.Value.Visible)
            {
                return;
            }

            // Template selection is ongoing at the moment
            if (_factoryTemplatesForm.IsInitialized && _factoryTemplatesForm.Value.Visible)
            {
                return;
            }

            _factoryTemplatesForm.Value.FactoryForm = _factoryForm.Value;
            _factoryTemplatesForm.Value.Show();
        }

        private void BtnRemoveFactory_Click(object sender, EventArgs e)
        {
            // Factory creation/edit is ongoing at the moment
            if (_factoryForm.IsInitialized && _factoryForm.Value.Visible)
            {
                return;
            }

            if (ListFactories.SelectedItem is Factory factory)
            {
                int index = ListFactories.Items.IndexOf(ListFactories.SelectedItem);
                ListFactories.Items.Remove(ListFactories.SelectedItem);

                // Ensure index is within valid range
                index--;
                index = Math.Max(0, index);
                ListFactories.SelectedItem = index >= 0 && ListFactories.Items.Count > 0 ? ListFactories.Items[index] : null;

                // Remove also from factories collection itself
                _ = AllFactories.Remove(factory.Id);

                UpdateAvailableFilterOptions();
            }
        }

        private void ListFactories_DoubleClick(object sender, EventArgs e)
        {
            if (ListFactories.SelectedItem is not Factory factory)
            {
                return;
            }

            // Template selection is ongoing at the moment
            if (_factoryTemplatesForm.IsInitialized && _factoryTemplatesForm.Value.Visible)
            {
                return;
            }

            // Factory creation/edit is ongoing at the moment
            if (_factoryForm.IsInitialized && _factoryForm.Value.Visible)
            {
                return;
            }

            _factoryForm.Value.IsEditing = true;
            _factoryForm.Value.Factory = factory;
            _factoryForm.Value.Show();
        }

        private void BtnQuickQuotaEditor_Click(object sender, EventArgs e)
        {
            _quickQuotaEditorForm.Value.FactoriesForm = this;
            _quickQuotaEditorForm.Value.Initialize();
            _quickQuotaEditorForm.Value.Show();
        }

        private void BtnClearAllFactories_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all custom factories? This is a destructive operation!",
                "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                AllFactories.Clear();
                ApplyCurrentFilter();
            }
        }

        private void BtnCreateFactoriesFromPreset_Click(object sender, EventArgs e)
        {
            _presetSelectionForm.Value.FactoriesForm = this;
            _presetSelectionForm.Value.JobsForm = null;
            _presetSelectionForm.Value.Show();
        }

        private void BtnAddSelection_Click(object sender, EventArgs e)
        {
            _multiTemplateSelector.Value.JobsForm = null;
            _multiTemplateSelector.Value.FactoriesForm = this;
            _multiTemplateSelector.Value.Show();
        }
    }
}
