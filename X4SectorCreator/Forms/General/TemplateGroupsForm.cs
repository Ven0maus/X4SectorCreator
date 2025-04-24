using System.ComponentModel;

namespace X4SectorCreator.Forms.General
{
    public partial class TemplateGroupsForm : Form
    {
        public enum GroupsFor
        {
            Factories,
            Jobs
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GroupsFor TemplateGroupsFor { get; set; }

        public TemplateGroupsForm()
        {
            InitializeComponent();
        }

        private void BtnCreateNewGroup_Click(object sender, EventArgs e)
        {

        }

        private void BtnDeleteGroup_Click(object sender, EventArgs e)
        {

        }

        private void BtnAddTemplate_Click(object sender, EventArgs e)
        {

        }

        private void BtnDeleteTemplate_Click(object sender, EventArgs e)
        {

        }
    }
}
