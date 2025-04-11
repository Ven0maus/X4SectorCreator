﻿using System.Collections.Generic;
using X4SectorCreator.CustomComponents;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class ObjectOverviewForm : Form
    {
        private readonly List<DataObject> _dataObjects = [];
        private readonly MultiSelectCombo _mscFilterType;

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
            // Add clusters
            _dataObjects.AddRange(MainForm.Instance.AllClusters.Select(a => new DataObject(a.Value)));

            // Add sectors
            _dataObjects.AddRange(MainForm.Instance.AllClusters
                .SelectMany(a => a.Value.Sectors, (cluster, sector) => new { Cluster = cluster.Value, Sector = sector })
                .Select(a => new DataObject(a.Cluster, a.Sector)));

            // Add connections
            _dataObjects.AddRange(MainForm.Instance.AllClusters
                .SelectMany(a => a.Value.Sectors.SelectMany(b => b.Zones).SelectMany(c => c.Gates))
                .Select(a => new DataObject(a)));

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
        }

        private void CmbFilterType_OnItemChecked(object sender, ItemCheckEventArgs e)
        {
            TxtSearch.GetTextSearchComponent()?.ForceCalculate();
        }

        class DataObject
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }

            public DataObject(Cluster cluster)
            {
                Type = "Cluster";
                Name = cluster.Name;
                Code = cluster.IsBaseGame ? $"{cluster.BaseGameMapping}" : $"PREFIX_CL_c{cluster.Id:D3}";
            }

            public DataObject(Cluster cluster, Sector sector)
            {
                Type = "Sector";
                Name = sector.Name;
                Code = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
                if (cluster.IsBaseGame && sector.IsBaseGame)
                {
                    Code = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_macro";
                }
                else if (cluster.IsBaseGame)
                {
                    Code = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}_macro";
                }
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
            }
        }

        private void ObjectOverviewForm_Load(object sender, EventArgs e)
        {
            _mscFilterType.Select(CmbFilterType.Items.Cast<string>().ToArray());
        }
    }
}
