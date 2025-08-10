using X4SectorCreator.CustomComponents;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Region = X4SectorCreator.Objects.Region;

namespace X4SectorCreator.Forms
{
    public partial class ObjectOverviewForm : Form
    {
        private readonly List<DataObject> _dataObjects = [];
        private readonly MultiSelectCombo _mscFilterType;
        private readonly HashSet<string> _defaultShownInFilter = new(StringComparer.OrdinalIgnoreCase)
        {
            "Sector"
        };

        public ObjectOverviewForm()
        {
            InitializeComponent();
            InitObjects();
            ApplyFilter();

            TxtSearch.EnableTextSearch(_dataObjects, a => a.Name + a.Code, ApplyFilter);
            Disposed += ObjectOverviewForm_Disposed;

            // Setup multicombobox component & select all available types by default
            _mscFilterType = new MultiSelectCombo(CmbFilterType);
            _mscFilterType.OnItemChecked += CmbFilterType_OnItemChecked;
        }

        private void InitObjects()
        {
            foreach (var cluster in MainForm.Instance.AllClusters.Values)
            {
                // Add cluster
                _dataObjects.Add(new DataObject(cluster));

                foreach (var sector in cluster.Sectors)
                {
                    // Add sector
                    _dataObjects.Add(new DataObject(cluster, sector));

                    foreach (var region in sector.Regions)
                    {
                        if (region.IsBaseGame) continue;

                        // Add region
                        _dataObjects.Add(new DataObject(cluster, sector, region));
                    }

                    foreach (var zone in sector.Zones)
                    {
                        // Add only non-basegame zones
                        if (!zone.IsBaseGame)
                            _dataObjects.Add(new DataObject(cluster, sector, zone));

                        foreach (var gate in zone.Gates)
                        {
                            // Add connection (also basegame)
                            _dataObjects.Add(new DataObject(gate));
                        }

                        if (!zone.IsBaseGame)
                        {
                            foreach (var station in zone.Stations)
                            {
                                _dataObjects.Add(new DataObject(cluster, sector, station));
                            }
                        }
                    }
                }
            }

            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                _dataObjects.Add(new DataObject(faction));
            }

            // Add all possible types to combobox filter type
            var types = _dataObjects.Select(a => a.Type).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var type in types.OrderBy(a => a))
                CmbFilterType.Items.Add(type);
        }

        private void ApplyFilter(List<DataObject> dataObjects = null)
        {
            var objects = dataObjects ?? [.. _dataObjects];

            // Apply type filter
            if (_mscFilterType != null)
            {
                var selectedTypes = _mscFilterType.SelectedItems.Cast<string>().ToHashSet(StringComparer.OrdinalIgnoreCase);
                objects.RemoveAll(a => !selectedTypes.Contains(a.Type));
            }

            if (ChkExcludeVanillaObjects.Checked)
                objects.RemoveAll(a => a.IsBaseGame);

            ObjectView.Rows.Clear();
            foreach (var obj in objects.OrderBy(a => a.Name))
                ObjectView.Rows.Add(obj.Type, obj.Name, obj.Code);
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ObjectOverviewForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
            _dataObjects.Clear(); // Garbage collection
        }

        private void CmbFilterType_OnItemChecked(object sender, ItemCheckEventArgs e)
        {
            TxtSearch.GetTextSearchComponent()?.ForceCalculate();
        }

        class DataObject
        {
            public bool IsBaseGame { get; }
            public string Type { get; }
            public string Name { get; }
            public string Code { get; }

            public DataObject(Cluster cluster)
            {
                Type = "Cluster";
                Name = cluster.Name;
                Code = cluster.IsBaseGame ? $"{cluster.BaseGameMapping}" : $"PREFIX_CL_c{cluster.Id:D3}";
                IsBaseGame = cluster.IsBaseGame;
            }

            public DataObject(Cluster cluster, Sector sector)
            {
                Type = "Sector";
                Name = sector.Name;
                Code = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}";
                if (cluster.IsBaseGame && sector.IsBaseGame)
                {
                    Code = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}";
                }
                else if (cluster.IsBaseGame)
                {
                    Code = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}";
                }
                IsBaseGame = sector.IsBaseGame;
            }

            public DataObject(Gate gate)
            {
                Type = "Connection";

                // Find source connection
                var sourceSector = MainForm.Instance.AllClusters.Values
                        .SelectMany(a => a.Sectors)
                        .First(a => a.Name.Equals(gate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                Zone sourceZone = sourceSector.Zones
                    .First(a => a.Gates
                        .Any(a => a.SourcePath
                            .Equals(gate.DestinationPath, StringComparison.OrdinalIgnoreCase)));
                Gate sourceGate = sourceZone.Gates.First(a => a.SourcePath.Equals(gate.DestinationPath, StringComparison.OrdinalIgnoreCase));

                Name = $"{gate.ParentSectorName} -> {sourceGate.ParentSectorName}";
                Code = gate.DestinationPath;
                IsBaseGame = gate.IsBaseGame;
            }

            public DataObject(Cluster cluster, Sector sector, Zone zone)
            {
                Type = "Zone";
                Name = $"{sector.Name} Zone {zone.Id:D3}";
                Code = $"PREFIX_ZO_c{cluster.Id:D3}_s{sector.Id:D3}_z{zone.Id:D3}_macro";
                if (cluster.IsBaseGame && sector.IsBaseGame)
                    Code = $"PREFIX_ZO_{cluster.BaseGameMapping}_{sector.BaseGameMapping}_z{zone.Id:D3}_macro";
                else if (cluster.IsBaseGame)
                    Code = $"PREFIX_ZO_{cluster.BaseGameMapping}_s{sector.Id:D3}_z{zone.Id:D3}_macro";
                IsBaseGame = zone.IsBaseGame;
            }

            public DataObject(Cluster cluster, Sector sector, Station station)
            {
                Type = "Station";
                Name = $"{sector.Name} {station.Name}";
                string clusterPrefix = $"c{cluster.Id:D3}";
                if (cluster.IsBaseGame)
                    clusterPrefix = cluster.BaseGameMapping.CapitalizeFirstLetter();
                string sectorPrefix = $"s{sector.Id:D3}";
                if (sector.IsBaseGame)
                    sectorPrefix = sector.BaseGameMapping.CapitalizeFirstLetter();
                Code = $"PREFIX_ST_{clusterPrefix}_{sectorPrefix}_st{station.Id:D3}";
                IsBaseGame = false;
            }

            public DataObject(Cluster cluster, Sector sector, Region region)
            {
                Type = "Region";
                Name = $"{sector.Name} {region.Name}";
                Code = $"re_c{cluster.Id:D3}_s{sector.Id:D3}_r{region.Id:D3}";
                if (cluster.IsBaseGame && sector.IsBaseGame)
                    Code = $"re_{cluster.BaseGameMapping}_{sector.BaseGameMapping}_r{region.Id:D3}";
                else if (cluster.IsBaseGame)
                    Code = $"re_{cluster.BaseGameMapping}_s{sector.Id:D3}_r{region.Id:D3}";
                Code = $"PREFIX_{Code.ToLower()}";
                IsBaseGame = false;
            }

            public DataObject(Faction faction)
            {
                Type = "Faction";
                Name = faction.Name;
                Code = faction.Id;
                IsBaseGame = false;
            }
        }

        private void ObjectOverviewForm_Load(object sender, EventArgs e)
        {
            _mscFilterType.Select(CmbFilterType.Items.Cast<string>().Where(_defaultShownInFilter.Contains).ToArray());
        }

        private void ChkExcludeVanillaObjects_CheckedChanged(object sender, EventArgs e)
        {
            TxtSearch.GetTextSearchComponent()?.ForceCalculate();
        }
    }
}
