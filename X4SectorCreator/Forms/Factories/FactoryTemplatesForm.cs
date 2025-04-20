using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactoryTemplatesForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactoryForm FactoryForm { get; set; }

        private static readonly Lazy<Factory[]> _templateFactories = new(() => CollectTemplateFactories().ToArray());

        public FactoryTemplatesForm()
        {
            InitializeComponent();

            // Collect all filter options
            HashSet<string> filterOptions = _templateFactories.Value
                .GroupBy(a => a.TemplateDirectory)
                .Select(a => a.Key)
                .OrderBy(a => a)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Setup filter options
            CmbFilterOption.Items.Clear();
            foreach (string option in filterOptions)
            {
                _ = CmbFilterOption.Items.Add(option);
            }

            // Show by default vanilla option if present
            if (filterOptions.Contains("Vanilla"))
            {
                CmbFilterOption.SelectedItem = "Vanilla";
            }
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

        private void CmbFilterOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFilterOption = CmbFilterOption.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedFilterOption))
            {
                ListTemplateFactories.Items.Clear();
                ListTemplateFactories.ClearSelected();
                return;
            }

            Factory[] factories = _templateFactories.Value
                .Where(a => a.TemplateDirectory.Equals(selectedFilterOption))
                .OrderBy(a => a.ToString())
                .ToArray();

            ListTemplateFactories.Items.Clear();
            foreach (Factory factory in factories)
            {
                _ = ListTemplateFactories.Items.Add(factory);
            }
        }

        private void ListTemplateJobs_DoubleClick(object sender, EventArgs e)
        {
            string selectedFilterOption = CmbFilterOption.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedFilterOption))
            {
                return;
            }

            // Select
            BtnSelectExampleFactory.PerformClick();
        }
    }
}
