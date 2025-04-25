using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.General
{
    public partial class TemplateContentForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TemplateGroupsForm TemplateGroupsForm { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CurrentTemplateGroup { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Job Job { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Factory Factory { get; set; }

        public TemplateContentForm()
        {
            InitializeComponent();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtXml.Text))
            {
                _ = MessageBox.Show("XML Cannot be whitespace or empty.");
                return;
            }

            // Check if valid
            object @new = null;
            object original = null;
            if (TemplateGroupsForm.TemplateGroupsFor == TemplateGroupsForm.GroupsFor.Jobs)
            {
                try
                {
                    original = Job;
                    @new = Job.DeserializeJob(TxtXml.Text);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Invalid XML: " + ex.Message);
                    return;
                }
            }
            else if (TemplateGroupsForm.TemplateGroupsFor == TemplateGroupsForm.GroupsFor.Factories)
            {
                try
                {
                    original = Factory;
                    @new = Factory.DeserializeFactory(TxtXml.Text);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("Invalid XML: " + ex.Message);
                    return;
                }
            }
            else
            {
                throw new NotSupportedException($"Not supported template groups for \"{TemplateGroupsForm.TemplateGroupsFor}\".");
            }

            TemplateGroupsForm.AddOrUpdateTemplate(CurrentTemplateGroup, original, @new);
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void LoadContent(object sender, EventArgs e)
        {
            if (Job != null)
            {
                TxtXml.Text = Job.SerializeJob();
            }
            else if (Factory != null)
            {
                TxtXml.Text = Factory.SerializeFactory();
            }
            else
            {
                TxtXml.Text = string.Empty;
            }
        }
    }
}
