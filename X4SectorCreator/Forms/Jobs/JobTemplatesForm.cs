using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobTemplatesForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobForm JobForm { get; set; }

        private static readonly Lazy<Job[]> _templateJobs = new(() => InitTemplateJobs().ToArray());

        public JobTemplatesForm()
        {
            InitializeComponent();

            // Collect all filter options
            HashSet<string> filterOptions = _templateJobs.Value
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

        private static IEnumerable<Job> InitTemplateJobs()
        {
            string directoryPath = Constants.DataPaths.TemplateJobsDirectoryPath;
            if (!Directory.Exists(directoryPath))
            {
                yield break;
            }

            // Collect all jobs.xml files in the sub directories and returns them
            foreach (string subDirectory in Directory.GetDirectories(directoryPath))
            {
                string templateName = Path.GetFileName(subDirectory);
                string jobFilePath = Path.Combine(subDirectory, "jobs.xml");

                if (File.Exists(jobFilePath))
                {
                    string xml = File.ReadAllText(jobFilePath);
                    Jobs jobs = Jobs.DeserializeJobs(xml);
                    foreach (Job job in jobs.JobList)
                    {
                        job.TemplateDirectory = templateName;
                        yield return job;
                    }
                }
            }
        }

        private void BtnSelectExampleJob_Click(object sender, EventArgs e)
        {
            if (ListTemplateJobs.SelectedItem is not Job selectedJob)
            {
                _ = MessageBox.Show("Please select a template first.");
                return;
            }

            JobForm.Job = selectedJob;
            JobForm.Show();
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ListTemplateJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTemplateJobs.SelectedItem is not Job selectedJob)
            {
                TxtExampleJob.Clear();
                return;
            }

            TxtExampleJob.Text = selectedJob.SerializeJob();
        }

        private void CmbFilterOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFilterOption = CmbFilterOption.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedFilterOption))
            {
                ListTemplateJobs.Items.Clear();
                ListTemplateJobs.ClearSelected();
                return;
            }

            Job[] jobs = _templateJobs.Value
                .Where(a => a.TemplateDirectory.Equals(selectedFilterOption))
                .OrderBy(a => a.ToString())
                .ToArray();

            ListTemplateJobs.Items.Clear();
            foreach (Job job in jobs)
            {
                _ = ListTemplateJobs.Items.Add(job);
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
            BtnSelectExampleJob.PerformClick();
        }
    }
}
