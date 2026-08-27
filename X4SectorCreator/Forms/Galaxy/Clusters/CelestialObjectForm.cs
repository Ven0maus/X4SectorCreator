using System.ComponentModel;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;

namespace X4SectorCreator.Forms.Galaxy.Clusters
{
    public partial class CelestialObjectForm : Form
    {
        private ClusterSystem.Moon _celestialObject;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ClusterSystem.Moon CelestialObject
        {
            get => _celestialObject;
            set
            {
                _celestialObject = value;

                CmbAtmopart.SelectedIndex = -1;
                CmbPart.SelectedIndex = -1;
                CmbAtmosphere.SelectedIndex = -1;
                CmbClass.SelectedIndex = -1;
                CmbGeology.SelectedIndex = -1;
                CmbPopulation.SelectedIndex = -1;
                CmbSettlements.SelectedIndex = -1;
                TxtMaxPopulation.Text = "0";

                if (_celestialObject == null)
                {
                    BtnCreate.Text = "Create";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(_celestialObject.Atmopart))
                        CmbAtmopart.SelectedItem = _celestialObject.Atmopart;
                    if (!string.IsNullOrWhiteSpace(_celestialObject.Part))
                        CmbPart.SelectedItem = _celestialObject.Part;
                    if (!string.IsNullOrWhiteSpace(_celestialObject.Atmosphere))
                        CmbAtmosphere.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.AtmosphereTypeMappings), _celestialObject.Atmosphere);

                    if (_celestialObject is ClusterSystem.Planet p && !string.IsNullOrWhiteSpace(p.Class))
                        CmbClass.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.PlanetTypeMappings), p.Class);

                    if (!string.IsNullOrWhiteSpace(_celestialObject.Geology))
                        CmbGeology.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.GeologyTypeMappings), _celestialObject.Geology);
                    if (!string.IsNullOrWhiteSpace(_celestialObject.Settlements))
                        CmbSettlements.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.SettlementTypeMappings), _celestialObject.Settlements);
                    if (!string.IsNullOrWhiteSpace(_celestialObject.Population))
                        CmbPopulation.SelectedItem = MapDefaultsGeneration.GetReverseLookup(nameof(MapDefaultsGeneration.PopulationMappings), _celestialObject.Population);

                    TxtMaxPopulation.Text = string.IsNullOrWhiteSpace(_celestialObject.MaxPopulation) ? "0" : _celestialObject.MaxPopulation;
                    BtnCreate.Text = "Update";
                }
            }
        }

        private ClusterSystem _clusterSystem;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ClusterSystem ClusterSystem
        {
            get => _clusterSystem;
            set
            {
                _clusterSystem = value;
                CmbPart.Items.Clear();
                CmbAtmopart.Items.Clear();

                var parts = GetParts(_clusterSystem);
                if (_clusterSystem.SharedData != null)
                    parts = parts.Concat(GetAtmoParts(_clusterSystem.SharedData));

                foreach (var part in parts.Distinct())
                    CmbPart.Items.Add(part);

                var atmoParts = GetParts(_clusterSystem);
                if (_clusterSystem.SharedData != null)
                    atmoParts = atmoParts.Concat(GetAtmoParts(_clusterSystem.SharedData));

                foreach (var atmoPart in atmoParts.Distinct())
                    CmbAtmopart.Items.Add(atmoPart);
            }
        }

        public enum Type
        {
            Moon,
            Planet
        }

        private Type _celestialType;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type CelestialType
        {
            get => _celestialType;
            set
            {
                _celestialType = value;
                if (_celestialType == Type.Planet)
                    CmbClass.Enabled = true;
                else
                    CmbClass.Enabled = false;
            }
        }

        public CelestialObjectForm()
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
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (!long.TryParse(TxtMaxPopulation.Text, out _))
            {
                MessageBox.Show("Max Population must be a valid number.", "Validation", MessageBoxButtons.OK);
                return;
            }

            switch (BtnCreate.Text)
            {
                case "Create":
                    if (CelestialType == Type.Planet)
                    {
                        var obj = new ClusterSystem.Planet
                        {
                            Name = TxtName.Text,
                            Class = MapDefaultsGeneration.PlanetTypeMappings[CmbClass.SelectedItem?.ToString()],
                            Part = CmbPart.SelectedItem?.ToString(),
                            Atmopart = CmbAtmopart.SelectedItem?.ToString(),
                            Settlements = MapDefaultsGeneration.SettlementTypeMappings[CmbSettlements.SelectedItem?.ToString()],
                            Atmosphere = MapDefaultsGeneration.AtmosphereTypeMappings[CmbAtmosphere.SelectedItem?.ToString()],
                            Geology = MapDefaultsGeneration.GeologyTypeMappings[CmbGeology.SelectedItem?.ToString()],
                            Population = MapDefaultsGeneration.PopulationMappings[CmbPopulation.SelectedItem?.ToString()],
                            MaxPopulation = TxtMaxPopulation.Text == "0" ? null : TxtMaxPopulation.Text
                        };

                        MainForm.Instance.ClusterForm.Value.ListBoxPlanets.Items.Add(obj);
                    }
                    else
                    {
                        var obj = new ClusterSystem.Moon
                        {
                            Name = TxtName.Text,
                            Part = CmbPart.SelectedItem?.ToString(),
                            Atmopart = CmbAtmopart.SelectedItem?.ToString(),
                            Settlements = MapDefaultsGeneration.SettlementTypeMappings[CmbSettlements.SelectedItem?.ToString()],
                            Atmosphere = MapDefaultsGeneration.AtmosphereTypeMappings[CmbAtmosphere.SelectedItem?.ToString()],
                            Geology = MapDefaultsGeneration.GeologyTypeMappings[CmbGeology.SelectedItem?.ToString()],
                            Population = MapDefaultsGeneration.PopulationMappings[CmbPopulation.SelectedItem?.ToString()],
                            MaxPopulation = TxtMaxPopulation.Text == "0" ? null : TxtMaxPopulation.Text
                        };

                        MainForm.Instance.ClusterForm.Value.ListBoxMoons.Items.Add(obj);
                    }
                    break;
                case "Update":
                    if (CelestialObject is ClusterSystem.Planet planet)
                    {
                        planet.Class = MapDefaultsGeneration.PlanetTypeMappings[CmbClass.SelectedItem?.ToString()];
                    }
                    CelestialObject.Name = TxtName.Text;
                    CelestialObject.Part = CmbPart.SelectedItem?.ToString();
                    CelestialObject.Atmopart = CmbAtmopart.SelectedItem?.ToString();
                    CelestialObject.Settlements = MapDefaultsGeneration.SettlementTypeMappings[CmbSettlements.SelectedItem?.ToString()];
                    CelestialObject.Atmosphere = MapDefaultsGeneration.AtmosphereTypeMappings[CmbAtmosphere.SelectedItem?.ToString()];
                    CelestialObject.Geology = MapDefaultsGeneration.GeologyTypeMappings[CmbGeology.SelectedItem?.ToString()];
                    CelestialObject.Population = MapDefaultsGeneration.PopulationMappings[CmbPopulation.SelectedItem?.ToString()];
                    CelestialObject.MaxPopulation = TxtMaxPopulation.Text == "0" ? null : TxtMaxPopulation.Text;
                    break;
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static IEnumerable<string> GetParts(ClusterSystem cs)
        {
            return cs.Planets.Select(a => a.Part)
                    .Concat(cs.Planets.SelectMany(a => a.Moons).Select(a => a.Part));
        }

        private static IEnumerable<string> GetAtmoParts(ClusterSystem cs)
        {
            return cs.Planets.Select(a => a.Atmopart)
                    .Concat(cs.Planets.SelectMany(a => a.Moons).Select(a => a.Atmopart));
        }
    }
}
