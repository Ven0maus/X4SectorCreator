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

            // Init jobs into listbox
            ListTemplateJobs.Items.Clear();
            foreach (var job in _templateJobs.Value)
                ListTemplateJobs.Items.Add(job);
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
    }
}
