using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class JobTemplatesForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobForm JobForm { get; set; }

        public JobTemplatesForm()
        {
            InitializeComponent();
        }

        private void BtnSelectExampleJob_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtExampleJob.Text))
            {
                _ = MessageBox.Show("Please select a template first.");
                return;
            }

            JobForm.Job = ConvertToJob(TxtExampleJob.Text);
            JobForm.Show();
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Job ConvertToJob(string exampleJobXml)
        {
            // TODO
            return null;
        }
    }
}
