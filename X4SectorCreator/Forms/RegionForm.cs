namespace X4SectorCreator.Forms
{
    public partial class RegionForm : Form
    {
        private RegionPropertiesForm _regionPropertiesForm;
        public RegionPropertiesForm RegionPropertiesForm => _regionPropertiesForm != null && !_regionPropertiesForm.IsDisposed
            ? _regionPropertiesForm
            : (_regionPropertiesForm = new RegionPropertiesForm());

        public RegionForm()
        {
            InitializeComponent();
        }

        private void BtnCreateRegion_Click(object sender, EventArgs e)
        {

        }

        private void BtnUpdateRegionProperties_Click(object sender, EventArgs e)
        {
            RegionPropertiesForm.Show();
        }
    }
}
