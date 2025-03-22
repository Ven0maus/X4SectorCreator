using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobTemplatesForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobForm JobForm { get; set; }

        private static readonly Lazy<Job[]> _templateJobs = new(() => InitTemplateJobs().ToArray());
        private const string _templateJobsPath = "Data/TemplateJobs";

        public JobTemplatesForm()
        {
            InitializeComponent();

            // Collect all filter options
            var filterOptions = _templateJobs.Value
                .GroupBy(a => a.TemplateDirectory)
                .Select(a => a.Key)
                .OrderBy(a => a)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Setup filter options
            CmbFilterOption.Items.Clear();
            foreach (var option in filterOptions)
                CmbFilterOption.Items.Add(option);
            
            // Show by default vanilla option if present
            if (filterOptions.Contains("Vanilla"))
                CmbFilterOption.SelectedItem = "Vanilla";
        }

        private static IEnumerable<Job> InitTemplateJobs()
        {
            var directoryPath = Path.Combine(Application.StartupPath, _templateJobsPath);
            if (!Directory.Exists(directoryPath))
                yield break;

            // Collect all jobs.xml files in the sub directories and returns them
            foreach (var subDirectory in Directory.GetDirectories(directoryPath))
            {
                string templateName = Path.GetFileName(subDirectory); // Extracts "BaseGameJobs", "DLCJobs"
                string jobFilePath = Path.Combine(subDirectory, "jobs.xml");

                if (File.Exists(jobFilePath))
                {
                    var xml = File.ReadAllText(jobFilePath);
                    Jobs jobs = Jobs.DeserializeJobs(xml);
                    foreach (var job in jobs.JobList)
                    {
                        job.TemplateDirectory = templateName;
                        yield return job;
                    }
                }
            }
        }

        private void BtnSelectExampleJob_Click(object sender, EventArgs e)
        {
            var selectedJob = ListTemplateJobs.SelectedItem as Job;
            if (selectedJob == null)
            {
                _ = MessageBox.Show("Please select a template first.");
                return;
            }

            JobForm.Job = selectedJob;
            JobForm.Show();
            TxtExampleJob.Clear();
            ListTemplateJobs.ClearSelected();
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            TxtExampleJob.Clear();
            ListTemplateJobs.ClearSelected();
            Close();
        }

        private void ListTemplateJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedJob = ListTemplateJobs.SelectedItem as Job;
            if (selectedJob == null)
            {
                TxtExampleJob.Clear();
                return;
            }

            TxtExampleJob.Text = selectedJob.SerializeJob();
        }

        private void CmbFilterOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedFilterOption = CmbFilterOption.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedFilterOption))
            {
                ListTemplateJobs.Items.Clear();
                ListTemplateJobs.ClearSelected();
                return;
            }

            var jobs = _templateJobs.Value
                .Where(a => a.TemplateDirectory.Equals(selectedFilterOption))
                .OrderBy(a => a.ToString())
                .ToArray();

            ListTemplateJobs.Items.Clear();
            foreach (var job in jobs)
                ListTemplateJobs.Items.Add(job);
        }
    }
}
