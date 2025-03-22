using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobForm : Form
    {
        private Job _job;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Job Job
        {
            get => _job;
            set
            {
                _job = value;
                if (_job != null)
                {
                    TxtJobXml.Text = _job.SerializeJob();
                }
            }
        }

        private bool _isEditing;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                BtnCreate.Text = _isEditing ? "Update" : "Create";
            }
        }

        public JobForm()
        {
            InitializeComponent();
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var job = Job.DeserializeJob(TxtJobXml.Text) ?? 
                    throw new Exception("No valid job exists within xml structure.");

                // Validation on same id
                if (!IsEditing)
                {
                    if (JobsForm.AllJobs.ContainsKey(job.Id))
                        throw new Exception($"A job with the id \"{job.Id}\" already exists, please use another id.");
                }

                // Set or update
                JobsForm.AllJobs[job.Id] = job;
                if (MainForm.Instance.JobsForm.Visible)
                    MainForm.Instance.JobsForm.ApplyCurrentFilter();
                Close();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Invalid XML: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
