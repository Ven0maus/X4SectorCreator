using System.ComponentModel;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobTemplatesForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobForm JobForm { get; set; }

        private static readonly Lazy<List<Job>> _templateJobs = new(() => CollectTemplateJobs().ToList());
        private readonly LazyEvaluated<TemplateGroupsForm> _templateGroupsView = new(() => new TemplateGroupsForm(), a => !a.IsDisposed);

        public JobTemplatesForm()
        {
            InitializeComponent();

            // Setup filter options
            TxtSearch.EnableTextSearch(_templateJobs.Value.ToList(), a => a.ToString(), ApplyCurrentFilter);
            Disposed += JobTemplatesForm_Disposed;

            ApplyCurrentFilter();
        }

        private void JobTemplatesForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        public static IEnumerable<Job> CollectTemplateJobs()
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

        private void ApplyCurrentFilter(List<Job> jobs = null)
        {
            Job[] data = (jobs ?? _templateJobs.Value)
                .OrderBy(a => a.ToString())
                .ToArray();

            ListTemplateJobs.Items.Clear();
            foreach (Job job in data)
            {
                _ = ListTemplateJobs.Items.Add(job);
            }
        }

        private void ListTemplateJobs_DoubleClick(object sender, EventArgs e)
        {
            // Select
            BtnSelectExampleJob.PerformClick();
        }

        private void BtnViewTemplateGroups_Click(object sender, EventArgs e)
        {
            _templateGroupsView.Value.TemplateGroupsFor = TemplateGroupsForm.GroupsFor.Jobs;
            _templateGroupsView.Value.Show();
        }
    }
}
