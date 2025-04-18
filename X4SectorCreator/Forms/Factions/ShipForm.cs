using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Factions
{
    public partial class ShipForm : Form
    {
        private FactionShipsForm _factionShipsForm;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionShipsForm FactionShipsForm
        {
            get => _factionShipsForm;
            set
            {
                _factionShipsForm = value;
            }
        }

        private Ship _ship;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Ship Ship
        {
            get => _ship;
            set
            {
                _ship = value;
                InitShip(value);
            }
        }

        public ShipForm()
        {
            InitializeComponent();
        }

        private void InitShip(Ship ship)
        {

        }
    }
}
