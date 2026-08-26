using System.ComponentModel;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;

namespace X4SectorCreator.Forms.Galaxy.Clusters
{
    public partial class PlanetForm : Form
    {
        private ClusterSystem.Planet _planet;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ClusterSystem.Planet Planet
        {
            get => _planet;
            set
            {
                _planet = value;

                CmbAtmopart.SelectedIndex = -1;
                CmbPart.SelectedIndex = -1;
                CmbAtmosphere.SelectedIndex = -1;
                CmbClass.SelectedIndex = -1;
                CmbGeology.SelectedIndex = -1;
                CmbPopulation.SelectedIndex = -1;
                CmbSettlements.SelectedIndex = -1;
                NrMaxPopulation.Value = 0;

                if (_planet == null)
                {
                    BtnCreate.Text = "Create";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(_planet.Atmopart))
                        CmbAtmopart.SelectedItem = _planet.Atmopart;
                    if (!string.IsNullOrWhiteSpace(_planet.Part))
                        CmbPart.SelectedItem = _planet.Part;
                    if (!string.IsNullOrWhiteSpace(_planet.Atmosphere))
                        CmbAtmosphere.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.AtmosphereTypeMappings), _planet.Atmosphere);
                    if (!string.IsNullOrWhiteSpace(_planet.Class))
                        CmbClass.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.PlanetTypeMappings), _planet.Class);
                    if (!string.IsNullOrWhiteSpace(_planet.Geology))
                        CmbGeology.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.GeologyTypeMappings), _planet.Geology);
                    if (!string.IsNullOrWhiteSpace(_planet.Settlements))
                        CmbSettlements.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.SettlementTypeMappings), _planet.Settlements);
                    if (!string.IsNullOrWhiteSpace(_planet.Population))
                        CmbPopulation.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.PopulationMappings), _planet.Population);

                    NrMaxPopulation.Value = !string.IsNullOrWhiteSpace(_planet.MaxPopulation) ? int.Parse(_planet.MaxPopulation) : 0;
                    BtnCreate.Text = "Update";
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ClusterSystem ClusterSystem { get; set; }

        public PlanetForm()
        {
            InitializeComponent();

            foreach (var item in MapDefaultsGeneration.AtmosphereTypeMappings.Keys)
                CmbAtmosphere.Items.Add(item);
            foreach (var item in MapDefaultsGeneration.PlanetTypeMappings.Keys)
                CmbClass.Items.Add(item);
            foreach (var item in MapDefaultsGeneration.GeologyTypeMappings.Keys)
                CmbGeology.Items.Add(item);
            foreach (var item in MapDefaultsGeneration.SettlementTypeMappings.Keys)
                CmbSettlements.Items.Add(item);
            foreach (var item in MapDefaultsGeneration.PopulationMappings.Keys)
                CmbPopulation.Items.Add(item);

            // TODO: Get parts & atmoparts from planets and moons
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            switch (BtnCreate.Text)
            {
                case "Create":
                    break;
                case "Update":
                    break;
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
