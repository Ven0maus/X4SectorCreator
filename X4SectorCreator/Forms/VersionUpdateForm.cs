using System.Diagnostics;

namespace X4SectorCreator.Forms
{
    public partial class VersionUpdateForm : Form
    {
        public VersionUpdateForm()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnNavigate_Click(object sender, EventArgs e)
        {
            // Navigate to releases page
            _ = Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Ven0maus/X4SectorCreator/releases",
                UseShellExecute = true
            });

            Application.Exit();
        }
    }
}
