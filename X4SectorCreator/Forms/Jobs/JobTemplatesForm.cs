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

        private readonly LazyEvaluated<TemplateGroupsForm> _templateGroupsView = new(() => new TemplateGroupsForm(), a => !a.IsDisposed);
        private readonly List<Job> _currentSelection = [];

        public JobTemplatesForm()
        {
            InitializeComponent();

            // Setup filter options
            TxtSearch.EnableTextSearch(_currentSelection, a => a.ToString(), ApplyCurrentFilter);
            Disposed += JobTemplatesForm_Disposed;
        }

        private void JobTemplatesForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        private void BtnSelectExampleJob_Click(object sender, EventArgs e)
        {
            if (ListTemplateJobs.SelectedItem is not Job selectedJob)
            {
                _ = MessageBox.Show("Please select a template first.");
                return;
            }

            const string lblJobName = "New Job Name:";
            Dictionary<string, string> data = MultiInputDialog.Show("New Job name (leave empty to copy original)",
                (lblJobName, null, null)
            );
            if (data == null || data.Count == 0)
                return;

            string jobName = data[lblJobName];
            if (!string.IsNullOrWhiteSpace(jobName))
            {
                selectedJob = Job.DeserializeJob(selectedJob.SerializeJob());
                selectedJob.Id = jobName;
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
            if (jobs == null) return;

            Job[] data = [.. jobs.OrderBy(a => a.ToString())];

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
            _templateGroupsView.Value.UpdateMethod = UpdateTemplateGroups;
            _templateGroupsView.Value.TemplateGroupsFor = TemplateGroupsForm.GroupsFor.Jobs;
            _templateGroupsView.Value.Show();
        }

        private void UpdateTemplateGroups()
        {
            var prev = CmbTemplatesGroup.SelectedItem;
            CmbTemplatesGroup.Items.Clear();

            // Get all available groups
            var baseDirectory = Constants.DataPaths.TemplateJobsDirectoryPath;
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
            Clipboard.SetText(TxtExampleJob.Text);
        }

        private void CmbTemplatesGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load all available templates of the selected group
            var groupName = CmbTemplatesGroup.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                ListTemplateJobs.Items.Clear();
                return;
            }

            ListTemplateJobs.Items.Clear();
            _currentSelection.Clear();

            var baseDirectory = Constants.DataPaths.TemplateJobsDirectoryPath;
            var fileName = Path.Combine(baseDirectory, groupName, "jobs.xml");
            if (!File.Exists(fileName))
                return;

            var jobs = Jobs.DeserializeJobs(File.ReadAllText(fileName));
            foreach (var job in jobs.JobList)
            {
                ListTemplateJobs.Items.Add(job);
                _currentSelection.Add(job);
            }
            TxtSearch.GetTextSearchComponent().ForceCalculate();
        }

        private void JobTemplatesForm_Load(object sender, EventArgs e)
        {
            // Init templates
            UpdateTemplateGroups();
            CmbTemplatesGroup.SelectedItem = "Vanilla";
        }
    }
}
