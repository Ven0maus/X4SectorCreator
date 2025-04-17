using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactionShipsForm : Form
    {
        private Faction _faction;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Faction Faction
        {
            get => _faction;
            set
            {
                _faction = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionForm FactionForm { get; set; }

        public FactionShipsForm()
        {
            InitializeComponent();
        }
    }
}
