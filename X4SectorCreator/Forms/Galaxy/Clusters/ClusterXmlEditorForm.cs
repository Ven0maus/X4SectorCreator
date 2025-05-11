using System.Xml.Linq;
using System.ComponentModel;

namespace X4SectorCreator.Forms.Galaxy.Clusters
{
    public partial class ClusterXmlEditorForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ClusterForm ClusterForm { get; set; }

        public ClusterXmlEditorForm()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            // If text empty remain null
            if (string.IsNullOrWhiteSpace(TxtClusterXml.Text))
            {
                ClusterForm.ClusterXml = null;
                return;
            }

            // Validate if xml is valid
            try
            {
                XDocument.Parse(TxtClusterXml.Text);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Invalid XML: {ex.Message}");
                return;
            }

            // If template unchanged, remain null
            if (TxtClusterXml.Equals(ClusterForm.GetTemplateXml(ClusterForm.Cluster)))
            {
                ClusterForm.ClusterXml = null;
                return;
            }

            // Update xml
            ClusterForm.ClusterXml = TxtClusterXml.Text;
            Close();
        }
    }
}
