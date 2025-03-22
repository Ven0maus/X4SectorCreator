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
                    FillAllFieldsFromJob(_job);
                }
            }
        }

        public JobForm()
        {
            InitializeComponent();
        }

        private void FillAllFieldsFromJob(Job job)
        {

        }
    }
}
