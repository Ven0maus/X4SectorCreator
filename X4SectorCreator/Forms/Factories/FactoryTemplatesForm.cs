using System.ComponentModel;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactoryTemplatesForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactoryForm FactoryForm { get; set; }

        private readonly LazyEvaluated<TemplateGroupsForm> _templateGroupsView = new(() => new TemplateGroupsForm(), a => !a.IsDisposed);
        private readonly List<Factory> _currentSelection = [];

        public FactoryTemplatesForm()
        {
            InitializeComponent();

            // Setup filter options
            TxtSearch.EnableTextSearch(_currentSelection, a => a.ToString(), ApplyCurrentFilter);
            Disposed += FactoryTemplatesForm_Disposed;
        }

        private void FactoryTemplatesForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        private void BtnSelectExampleFactory_Click(object sender, EventArgs e)
        {
            if (ListTemplateFactories.SelectedItem is not Factory selectedFactory)
            {
                _ = MessageBox.Show("Please select a template first.");
                return;
            }

            const string lblFactoryName = "New Factory Name:";
            Dictionary<string, string> data = MultiInputDialog.Show("New Factory name (leave empty to copy original)",
                (lblFactoryName, null, null)
            );
            if (data == null || data.Count == 0)
                return;

            string factoryName = data[lblFactoryName];
            if (!string.IsNullOrWhiteSpace(factoryName))
            {
                selectedFactory = Factory.DeserializeFactory(selectedFactory.SerializeFactory());
                selectedFactory.Id = factoryName;
            }

            FactoryForm.Factory = selectedFactory;
            FactoryForm.Show();
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ListTemplateJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTemplateFactories.SelectedItem is not Factory selectedJob)
            {
                TxtExampleFactory.Clear();
                return;
            }

            TxtExampleFactory.Text = selectedJob.SerializeFactory();
        }

        private void ApplyCurrentFilter(List<Factory> factories)
        {
            if (factories == null) return;

            Factory[] data = [.. factories.OrderBy(a => a.ToString())];

            ListTemplateFactories.Items.Clear();
            foreach (Factory factory in data)
            {
                _ = ListTemplateFactories.Items.Add(factory);
            }
        }

        private void ListTemplateJobs_DoubleClick(object sender, EventArgs e)
        {
            // Select
            BtnSelectExampleFactory.PerformClick();
        }

        private void BtnViewTemplateGroups_Click(object sender, EventArgs e)
        {
            _templateGroupsView.Value.UpdateMethod = UpdateTemplateGroups;
            _templateGroupsView.Value.TemplateGroupsFor = TemplateGroupsForm.GroupsFor.Factories;
            _templateGroupsView.Value.Show();
        }

        private void UpdateTemplateGroups()
        {
            var prev = CmbTemplatesGroup.SelectedItem;
            CmbTemplatesGroup.Items.Clear();

            // Get all available groups
            var baseDirectory = Constants.DataPaths.TemplateFactoriesDirectoryPath;
            foreach (var directory in Directory.GetDirectories(baseDirectory))
            {
                string groupName = new DirectoryInfo(Path.GetFileName(directory)).Name;
                CmbTemplatesGroup.Items.Add(groupName);
            }

            if (CmbTemplatesGroup.Items.Contains(prev))
                CmbTemplatesGroup.SelectedItem = prev;
        }

        private void BtnCopyXml_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TxtExampleFactory.Text);
        }

        private void CmbTemplatesGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load all available templates of the selected group
            var groupName = CmbTemplatesGroup.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                ListTemplateFactories.Items.Clear();
                _currentSelection.Clear();
                return;
            }

            ListTemplateFactories.Items.Clear();
            _currentSelection.Clear();

            var baseDirectory = Constants.DataPaths.TemplateFactoriesDirectoryPath;
            var fileName = Path.Combine(baseDirectory, groupName, "god.xml");
            if (!File.Exists(fileName))
                return;

            var factories = Objects.Factories.DeserializeFactories(File.ReadAllText(fileName));
            foreach (var factory in factories.FactoryList)
            {
                ListTemplateFactories.Items.Add(factory);
                _currentSelection.Add(factory);
            }
            TxtSearch.GetTextSearchComponent().ForceCalculate();
        }

        private void FactoryTemplatesForm_Load(object sender, EventArgs e)
        {
            // Init templates
            UpdateTemplateGroups();
            CmbTemplatesGroup.SelectedItem = "Vanilla";
        }
    }
}
