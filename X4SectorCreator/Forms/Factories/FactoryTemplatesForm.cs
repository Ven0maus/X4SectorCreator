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

        private static readonly Lazy<List<Factory>> _templateFactories = new(() => CollectTemplateFactories().ToList());
        private readonly LazyEvaluated<TemplateGroupsForm> _templateGroupsView = new(() => new TemplateGroupsForm(), a => !a.IsDisposed);

        public FactoryTemplatesForm()
        {
            InitializeComponent();

            // Setup filter options
            TxtSearch.EnableTextSearch(_templateFactories.Value.ToList(), a => a.ToString(), ApplyCurrentFilter);
            Disposed += FactoryTemplatesForm_Disposed;

            ApplyCurrentFilter();
        }

        private void FactoryTemplatesForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        public static IEnumerable<Factory> CollectTemplateFactories()
        {
            string directoryPath = Constants.DataPaths.TemplateFactoriesDirectoryPath;
            if (!Directory.Exists(directoryPath))
            {
                yield break;
            }

            // Collect all god.xml files in the sub directories and returns them
            foreach (string subDirectory in Directory.GetDirectories(directoryPath))
            {
                string templateName = Path.GetFileName(subDirectory);
                string godFilePath = Path.Combine(subDirectory, "god.xml");

                if (File.Exists(godFilePath))
                {
                    string xml = File.ReadAllText(godFilePath);
                    Objects.Factories factories = Objects.Factories.DeserializeFactories(xml);
                    foreach (Factory factory in factories.FactoryList)
                    {
                        factory.TemplateDirectory = templateName;
                        yield return factory;
                    }
                }
            }
        }

        private void BtnSelectExampleFactory_Click(object sender, EventArgs e)
        {
            if (ListTemplateFactories.SelectedItem is not Factory selectedFactory)
            {
                _ = MessageBox.Show("Please select a template first.");
                return;
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

        private void ApplyCurrentFilter(List<Factory> factories = null)
        {
            Factory[] data = (factories ?? _templateFactories.Value)
                .OrderBy(a => a.ToString())
                .ToArray();

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
            _templateGroupsView.Value.TemplateGroupsFor = TemplateGroupsForm.GroupsFor.Factories;
            _templateGroupsView.Value.Show();
        }
    }
}
