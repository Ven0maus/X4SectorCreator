using System.ComponentModel;
using System.Data;
using X4SectorCreator.Forms.General;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactoryForm : Form
    {
        private Factory _factory;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Factory Factory
        {
            get => _factory;
            set
            {
                _factory = value;
                if (_factory != null)
                {
                    TxtFactoryXml.Text = _factory.SerializeFactory();
                    TxtFactoryXml.SelectionStart = TxtFactoryXml.Text.Length;
                }
            }
        }

        private bool _isEditing;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                BtnCreate.Text = _isEditing ? "Update" : "Create";
            }
        }

        private readonly LazyEvaluated<FactionSelectionForm> FactionSelectionForm = new(() => new FactionSelectionForm(), a => !a.IsDisposed);

        private static readonly string[] _typeLabels = ["galaxy", "cluster", "sector"];

        public FactoryForm()
        {
            InitializeComponent();
        }

        private void BtnSelectFaction_Click(object sender, EventArgs e)
        {
            Factory factory = TryDeserializeFactory(false);
            if (factory == null)
            {
                _ = MessageBox.Show("The xml must be valid, if you want to select a faction for the factory.");
                return;
            }

            // Set factoryform & factory in factoryselectionform
            FactionSelectionForm.Value.FactoryForm = this;
            FactionSelectionForm.Value.Factory = factory;
            FactionSelectionForm.Value.Show();
        }

        private void BtnSelectFactoryLocation_Click(object sender, EventArgs e)
        {
            Factory factory = TryDeserializeFactory(false);
            if (factory == null)
            {
                _ = MessageBox.Show("The xml must be valid, if you want to select a location for the factory.");
                return;
            }

            Cluster factoryCluster = GetClusterFromFactory(factory);
            Sector factorySector = GetSectorFromFactory(factory, factoryCluster);

            const string lblType = "Location Type:";
            const string lblCluster = "Location Cluster (only if type is cluster):";
            const string lblSector = "Location Sector (only if type is sector):";
            Dictionary<string, string> modInfo = MultiInputDialog.Show("Select Location",
                (lblType, _typeLabels, factorySector != null ? "sector" : factoryCluster != null ? "cluster" : "galaxy"),
                (lblCluster, MainForm.Instance.AllClusters.Values.Select(a => a.Name).OrderBy(a => a).ToArray(), factoryCluster?.Name),
                (lblSector, MainForm.Instance.AllClusters.Values.SelectMany(a => a.Sectors).Select(a => a.Name).OrderBy(a => a).ToArray(), factorySector?.Name)
            );

            if (modInfo == null || modInfo.Count != 3)
            {
                return;
            }

            string type = modInfo[lblType]?.ToLower();
            if (string.IsNullOrWhiteSpace(type) || !_typeLabels.Contains(type))
            {
                _ = MessageBox.Show("Location Type must have a valid value, no changes applied.");
                return;
            }

            string clusterName = modInfo[lblCluster];
            string sectorName = modInfo[lblSector];

            // Create location if not exist yet
            factory.Location ??= new Factory.LocationObj();

            switch (type)
            {
                case "galaxy":
                    factory.Location.Class = "galaxy";
                    factory.Location.Macro = $"{GalaxySettingsForm.GalaxyName}_macro";
                    break;
                case "cluster":
                    Cluster clusterValue = MainForm.Instance.AllClusters.Values
                        .First(a => a.Name.Equals(clusterName, StringComparison.OrdinalIgnoreCase));

                    string clusterCode = $"PREFIX_CL_c{clusterValue.Id:D3}_macro";
                    if (clusterValue.IsBaseGame)
                    {
                        clusterCode = $"{clusterValue.BaseGameMapping}_macro";
                    }

                    factory.Location.Class = "cluster";
                    factory.Location.Macro = clusterCode;
                    break;
                case "sector":
                    var sectorValue = MainForm.Instance.AllClusters.Values
                        .SelectMany(cluster => cluster.Sectors, (cluster, sector) => new { cluster, sector })
                        .First(a => a.sector.Name.Equals(sectorName, StringComparison.OrdinalIgnoreCase));
                    Cluster cluster = sectorValue.cluster;
                    Sector sector = sectorValue.sector;

                    string sectorCode = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
                    if (cluster.IsBaseGame && sector.IsBaseGame)
                    {
                        sectorCode = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                    }
                    else if (cluster.IsBaseGame)
                    {
                        sectorCode = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}_macro";
                    }

                    factory.Location.Class = "sector";
                    factory.Location.Macro = sectorCode;
                    break;
            }

            TxtFactoryXml.Text = factory.SerializeFactory();
            TxtFactoryXml.SelectionStart = TxtFactoryXml.Text.Length;
        }

        private static Cluster GetClusterFromFactory(Factory factory)
        {
            if (string.IsNullOrWhiteSpace(factory.Location?.Macro))
            {
                return null;
            }

            string factoryLocation = factory.Location.Macro;
            Dictionary<(int, int), Cluster> allClusters = MainForm.Instance.AllClusters;

            foreach (KeyValuePair<(int, int), Cluster> cluster in allClusters)
            {
                string clusterCode = $"PREFIX_CL_c{cluster.Value.Id:D3}_macro";
                if (cluster.Value.IsBaseGame)
                {
                    clusterCode = $"{cluster.Value.BaseGameMapping}_macro";
                }

                if (factoryLocation.Equals(clusterCode, StringComparison.OrdinalIgnoreCase))
                {
                    return cluster.Value;
                }

                foreach (Sector sector in cluster.Value.Sectors)
                {
                    string sectorCode = $"PREFIX_SE_c{cluster.Value.Id:D3}_s{sector.Id:D3}_macro";
                    if (cluster.Value.IsBaseGame && sector.IsBaseGame)
                    {
                        sectorCode = $"{cluster.Value.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                    }
                    else if (cluster.Value.IsBaseGame)
                    {
                        sectorCode = $"PREFIX_SE_c{cluster.Value.BaseGameMapping}_s{sector.Id}_macro";
                    }

                    if (factoryLocation.Equals(sectorCode, StringComparison.OrdinalIgnoreCase))
                    {
                        return cluster.Value;
                    }
                }
            }

            return null;
        }

        private static Sector GetSectorFromFactory(Factory factory, Cluster cluster)
        {
            if (string.IsNullOrWhiteSpace(factory.Location?.Macro) || cluster == null)
            {
                return null;
            }

            foreach (Sector sector in cluster.Sectors)
            {
                string sectorCode = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
                if (cluster.IsBaseGame && sector.IsBaseGame)
                {
                    sectorCode = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                }
                else if (cluster.IsBaseGame)
                {
                    sectorCode = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}_macro";
                }

                if (factory.Location.Macro.Equals(sectorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return sector;
                }
            }
            return null;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                Factory factory = TryDeserializeFactory(true) ??
                    throw new Exception("No valid factory exists within xml structure.");

                if (!IsEditing)
                {
                    // If not editing, always validate factory id
                    if (FactoriesForm.AllFactories.ContainsKey(factory.Id))
                    {
                        throw new Exception($"A factory with the id \"{factory.Id}\" already exists, please use another id.");
                    }

                    FactoriesForm.AllFactories.Add(factory.Id, factory);
                }
                else
                {
                    // If editing and factory id was changed we need to validate
                    if (Factory.Id != factory.Id)
                    {
                        if (FactoriesForm.AllFactories.ContainsKey(factory.Id))
                        {
                            throw new Exception($"A factory with the id \"{factory.Id}\" already exists, please use another id.");
                        }
                    }

                    // Remove old factory
                    _ = FactoriesForm.AllFactories.Remove(Factory.Id);
                    // Replace with new factory
                    FactoriesForm.AllFactories.Add(factory.Id, factory);
                }

                if (MainForm.Instance.FactoriesForm.IsInitialized && MainForm.Instance.FactoriesForm.Value.Visible)
                {
                    MainForm.Instance.FactoriesForm.Value.UpdateAvailableFilterOptions();
                    MainForm.Instance.FactoriesForm.Value.ApplyCurrentFilter();
                }
                Close();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Invalid XML: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Factory TryDeserializeFactory(bool throwException)
        {
            try
            {
                return Factory.DeserializeFactory(TxtFactoryXml.Text);
            }
            catch (Exception)
            {
                if (!throwException)
                {
                    return null;
                }

                throw;
            }
        }
    }
}
