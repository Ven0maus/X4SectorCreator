namespace X4SectorCreator.Forms.General
{
    public partial class TutorialVideoForm : Form
    {
        public TutorialVideoForm()
        {
            InitializeComponent();
        }

        private async void TutorialVideoForm_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.Source = new Uri("https://www.youtube.com/embed/CywvNiwQGTs");
        }
    }
}
