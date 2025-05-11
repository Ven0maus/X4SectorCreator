using System.Data;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.Regions
{
    public partial class RegionDefinitionsForm : Form
    {
        public readonly LazyEvaluated<RegionDefinitionForm> RegionDefinitionForm = new(() => new RegionDefinitionForm(), a => !a.IsDisposed);

        public RegionDefinitionsForm()
        {
            InitializeComponent();

            foreach (RegionDefinition definition in Forms.RegionDefinitionForm.RegionDefinitions.OrderBy(a => a.Name))
            {
                _ = ListBoxRegionDefinitions.Items.Add(definition);
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnNewDefinition_Click(object sender, EventArgs e)
        {
            RegionDefinitionForm.Value.InitDefaultFalloff();
            RegionDefinitionForm.Value.Show();
        }

        private void BtnRemoveDefinition_Click(object sender, EventArgs e)
        {
            if (ListBoxRegionDefinitions.SelectedItem is not RegionDefinition selectedRegionDefinition)
            {
                return;
            }

            var currentRegion = MainForm.Instance.RegionForm.Value.CustomRegion;
            // Check if regions exist that use this given region definition
            var regionsThatUseThisDefinition = MainForm.Instance.AllClusters.Values
                .SelectMany(cluster => cluster.Sectors, (cluster, sector) => new { cluster, sector })
                .SelectMany(cs => cs.sector.Regions, (cs, region) => new { cs.cluster, cs.sector, region })
                .Where(x => x.region.Definition.Equals(selectedRegionDefinition))
                .Where(x => currentRegion == null || !x.region.Equals(currentRegion))
                .ToArray();

            if (regionsThatUseThisDefinition.Length > 0)
            {
                string message = "Following regions use this region definition, they must first be changed or deleted before this definition can be deleted:\n" +
                    string.Join("\n", regionsThatUseThisDefinition
                    .OrderBy(a => a.cluster.Name)
                    .ThenBy(a => a.sector.Name)
                    .Select(x => $"- {x.region.Name} (Cluster: {x.cluster.Name}, Sector: {x.sector.Name})"));

                _ = MessageBox.Show(message);
                return;
            }

            int index = ListBoxRegionDefinitions.Items.IndexOf(selectedRegionDefinition);
            ListBoxRegionDefinitions.Items.Remove(selectedRegionDefinition);
            _ = Forms.RegionDefinitionForm.RegionDefinitions.Remove(selectedRegionDefinition); // remove from static cache

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            ListBoxRegionDefinitions.SelectedItem = index >= 0 && ListBoxRegionDefinitions.Items.Count > 0 ? ListBoxRegionDefinitions.Items[index] : null;

            // Remove from cmb items
            MainForm.Instance.RegionForm.Value.CmbRegionDefinition.Items.Remove(selectedRegionDefinition);

            // Reset selection if it was selected
            if (MainForm.Instance.RegionForm.Value.CmbRegionDefinition.SelectedItem == selectedRegionDefinition)
                MainForm.Instance.RegionForm.Value.CmbRegionDefinition.SelectedItem = null;
        }

        private void ListBoxRegionDefinitions_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxRegionDefinitions.SelectedItem is not RegionDefinition selectedRegionDefinition)
            {
                return;
            }

            RegionDefinitionForm.Value.RegionDefinition = selectedRegionDefinition;
            RegionDefinitionForm.Value.Show();
        }
    }
}
