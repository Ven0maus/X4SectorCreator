using System.ComponentModel;
using X4SectorCreator.CustomComponents;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class BasketForm : Form
    {
        private readonly MultiSelectCombo _mscWares;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BasketsForm BasketsForm { get; set; }

        private Basket _basket;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Basket Basket
        {
            get => _basket;
            set
            {
                _basket = value;
                if (_basket != null)
                {
                    TxtName.Text = _basket.Id.Replace("PREFIX_", "");
                    BtnCreate.Text = "Update";
                    BtnCreate.Enabled = !_basket.IsBaseGame;
                }
            }
        }

        private readonly string[] _wares =
        [
            "advancedelectronics",
            "antimattercells",
            "antimatterconverters",
            "advancedcomposites",
            "claytronics",
            "dronecomponents",
            "energycells",
            "engineparts",
            "fieldcoils",
            "foodrations",
            "graphene",
            "helium",
            "hullparts",
            "hydrogen",
            "ice",
            "majadust",
            "majasnails",
            "meat",
            "medicalsupplies",
            "methane",
            "microchips",
            "missilecomponents",
            "nividium",
            "nostropoil",
            "ore",
            "plasmaconductors",
            "quantumtubes",
            "refinedmetals",
            "scanningarrays",
            "shieldcomponents",
            "silicon",
            "siliconwafers",
            "smartchips",
            "sojabeans",
            "sojahusk",
            "spacefuel",
            "spaceweed",
            "spices",
            "sunriseflowers",
            "superfluidcoolant",
            "swampplant",
            "teladianium",
            "turretcomponents",
            "water",
            "weaponcomponents",
            "wheat",
            "computronicsubstrate",
            "metallicmicrolattice",
            "siliconcarbide",
            "proteinpaste",
            "terranmre",
            "stimulants",
            "cheltmeat",
            "scruffinfruits",
            "bofu",
            "plankton",
            "bogas"
        ];

        public BasketForm()
        {
            InitializeComponent();

            // Init wares
            foreach (string ware in _wares.OrderBy(a => a))
            {
                _ = CmbWares.Items.Add(ware);
            }

            _mscWares = new MultiSelectCombo(CmbWares);
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (Basket != null && Basket.IsBaseGame)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                _ = MessageBox.Show("Please fill in a valid basket name.");
                return;
            }

            if (_mscWares.SelectedItems.Count == 0)
            {
                _ = MessageBox.Show("You must select atleast one ware.");
                return;
            }

            if (JobsForm.AllBaskets.ContainsKey(TxtName.Text) && Basket == null)
            {
                _ = MessageBox.Show("A basket with this name already exists, please use another name.");
                return;
            }

            Basket selected = Basket;
            if (BtnCreate.Text == "Update")
            {
                Basket.Id = $"PREFIX_{TxtName.Text.ToLower()}";
                Basket.Wares.Wares.Clear();
                foreach (string ware in _mscWares.SelectedItems.Cast<string>())
                {
                    Basket.Wares.Wares.Add(new Basket.WareObjects.WareObj { Ware = ware });
                }
            }
            else
            {
                Basket basket = new()
                {
                    Id = $"PREFIX_{TxtName.Text.ToLower()}",
                    IsBaseGame = false,
                    Wares = new Basket.WareObjects()
                };

                basket.Wares.Wares = [];
                foreach (string ware in _mscWares.SelectedItems.Cast<string>())
                {
                    basket.Wares.Wares.Add(new Basket.WareObjects.WareObj { Ware = ware });
                }

                selected = basket;

                JobsForm.AllBaskets.Add(basket.Id, basket);
            }

            BasketsForm.UpdateBaskets(selected);
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BasketForm_Load(object sender, EventArgs e)
        {
            if (Basket != null)
            {
                _mscWares.ResetSelection();
                _mscWares.Select(_basket.Wares.Wares.Select(a => a.Ware).ToArray());
            }
        }
    }
}
