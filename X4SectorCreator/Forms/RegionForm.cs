using X4SectorCreator.Objects;
using System.ComponentModel;

namespace X4SectorCreator.Forms
{
    public partial class RegionForm : Form
    {
        private RegionPropertiesForm _regionPropertiesForm;
        public RegionPropertiesForm RegionPropertiesForm => _regionPropertiesForm != null && !_regionPropertiesForm.IsDisposed
            ? _regionPropertiesForm
            : (_regionPropertiesForm = new RegionPropertiesForm());

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Cluster Cluster { get; set; }

        private Sector _sector;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Sector Sector
        {
            get => _sector;
            set 
            {
                _sector = value;
                if (_sector != null)
                    InitSectorValues();
            }
        }

        public RegionForm()
        {
            InitializeComponent();
        }

        private void InitSectorValues()
        {

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
