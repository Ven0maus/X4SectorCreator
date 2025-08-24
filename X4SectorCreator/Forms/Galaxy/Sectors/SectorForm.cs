using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class SectorForm : Form
    {
        private Sector _sector;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Sector Sector
        {
            get => _sector;
            set
            {
                _sector = value;
                if (_sector != null)
                {
                    TxtName.Text = _sector.Name;
                    txtDescription.Text = _sector.Description;
                    txtSunlight.Text = ((int)(_sector.Sunlight * 100)).ToString();
                    txtEconomy.Text = ((int)(_sector.Economy * 100)).ToString();
                    txtSecurity.Text = ((int)(_sector.Security * 100)).ToString();
                    txtSectorRadius.Text = ((int)(_sector.DiameterRadius / 1000f / 2f)).ToString();
                    chkAllowRandomAnomalies.Checked = _sector.AllowRandomAnomalies;
                    chkDisableFactionLogic.Checked = _sector.DisableFactionLogic;

                    Init();
                }
                else
                {
                    TxtName.Text = string.Empty;
                    txtDescription.Text = string.Empty;
                    txtSunlight.Text = "100";
                    txtEconomy.Text = "100";
                    txtSecurity.Text = "100";
                    txtSectorRadius.Text = "250";
                    chkAllowRandomAnomalies.Checked = true;
                    chkDisableFactionLogic.Checked = false;
                }
            }
        }

        private readonly Color _controlColor;
        private readonly Cluster _selectedCluster;

        public static readonly List<SectorPlacement[]> ValidSectorCombinations =
        [
            [SectorPlacement.TopLeft, SectorPlacement.BottomLeft, SectorPlacement.MiddleRight],
            [SectorPlacement.TopRight, SectorPlacement.BottomRight, SectorPlacement.MiddleLeft],
            [SectorPlacement.TopLeft, SectorPlacement.BottomRight],
            [SectorPlacement.TopRight, SectorPlacement.BottomLeft],
            [SectorPlacement.MiddleLeft, SectorPlacement.MiddleTop, SectorPlacement.MiddleRight, SectorPlacement.MiddleBottom, ]
        ];

        public SectorForm()
        {
            InitializeComponent();
            _controlColor = lblRadiusUnderText.ForeColor;

            // Get the cluster that was selected in the mainform
            string selectedClusterName = MainForm.Instance.ClustersListBox.SelectedItem as string;
            _selectedCluster = MainForm.Instance.AllClusters
                .First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase))
                .Value;
        }

        public void Init()
        {
            if (_selectedCluster.CustomSectorPositioning)
            {
                SetupPlacementValues();
                cmbPlacement.Enabled = true;
            }
            else
            {
                cmbPlacement.Items.Clear();
                cmbPlacement.SelectedIndex = -1;
                cmbPlacement.Enabled = false;
            }
        }

        /// <summary>
        /// Automatically determines and sets the sector offset based on the SectorPlacement property of the sector.
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="sector"></param>
        public static void DetermineSectorOffset(Cluster cluster, Sector sector)
        {
            // If the sector is the only one in the cluster, use center (0, 0)
            if (cluster.Sectors.Count <= 1)
            {
                // Either no sectors exist, or we're updating an existing sector
                if (cluster.Sectors.Count == 0 || cluster.Sectors.First() == sector)
                {
                    sector.Offset = new(0, 0);
                    return;
                }
            }

            const int amount = 1000000; // 1000km should be enough
            sector.Offset = sector.Placement switch
            {
                SectorPlacement.TopLeft => new(-amount, amount),
                SectorPlacement.BottomLeft => new(-amount, -amount),
                SectorPlacement.TopRight => new(amount, amount),
                SectorPlacement.BottomRight => new(amount, -amount),
                SectorPlacement.MiddleLeft => new(-amount, 0),
                SectorPlacement.MiddleRight => new(amount, 0),
                SectorPlacement.MiddleTop => new(0, amount),
                SectorPlacement.MiddleBottom => new(0, -amount),
                _ => new(0, 0) // Default case
            };
        }

        private void SetupPlacementValues()
        {
            SectorPlacement[] placementValues = Enum.GetValues<SectorPlacement>().OrderBy(a => a).ToArray();

            // Select selected placement value dynamically based on other sectors in the cluster
            cmbPlacement.Items.Clear();
            foreach (SectorPlacement placementValue in placementValues)
            {
                _ = cmbPlacement.Items.Add(placementValue);
            }

            cmbPlacement.SelectedItem = Sector != null ? Sector.Placement : cmbPlacement.Items[0];
        }

        public static bool IsClusterPlacementValid(Cluster cluster)
        {
            if (cluster.Sectors.Count == 1)
            {
                return true;
            }

            HashSet<SectorPlacement> sectorPlacements = cluster.Sectors
                .Select(a => a.Placement)
                .ToHashSet();

            foreach (SectorPlacement[] combination in ValidSectorCombinations)
            {
                bool valid = true;
                foreach (SectorPlacement placement in sectorPlacements)
                {
                    if (!combination.Contains(placement))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    return true;
                }
            }

            return false;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            string name = TxtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                _ = MessageBox.Show("Please select a valid (non empty / non whitespace) name.");
                return;
            }

            // Check if name already exists
            foreach (Cluster cluster in MainForm.Instance.AllClusters.Values)
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    bool invalidName = false;
                    if (Sector != null)
                    {
                        // Check in case we are editing a sector
                        invalidName = Sector != sector && sector.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        // Check in case we are creating a sector
                        invalidName = sector.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
                    }

                    if (invalidName)
                    {
                        _ = MessageBox.Show($"A sector with the name \"{name}\" already exists in cluster \"{cluster.Name}\", please choose another name.");
                        return;
                    }
                }
            }

            // Validate sector radius
            if (!int.TryParse(txtSectorRadius.Text, out int sectorRadius) || sectorRadius <= 0 || sectorRadius > 999)
            {
                _ = MessageBox.Show($"Please use a valid numerical value for the sector radius, and specify a value between 1 and 999.");
                return;
            }

            // Validate sunlight, economy, security
            if (!int.TryParse(txtSunlight.Text, out int sunlight) ||
                !int.TryParse(txtEconomy.Text, out int economy) ||
                !int.TryParse(txtSecurity.Text, out int security))
            {
                _ = MessageBox.Show($"Please use valid numerical values for sunlight, economy and security.");
                return;
            }

            SectorPlacement sectorPlacement = !_selectedCluster.CustomSectorPositioning ? default : (SectorPlacement)cmbPlacement.SelectedItem;
            Sector sectorValue = Sector;
            SectorPlacement? beforePlacement = sectorValue?.Placement;
            switch (BtnCreate.Text)
            {
                case "Create":
                    if (_selectedCluster.Sectors.Count == 4)
                    {
                        _ = MessageBox.Show($"Reached maximum allowed sectors for cluster \"{_selectedCluster.Name}\".");
                        return;
                    }

                    sectorValue = new Sector
                    {
                        Id = _selectedCluster.Sectors.DefaultIfEmpty(new Sector()).Max(a => a.Id) + 1,
                        Name = name,
                        Owner = "None",
                        DiameterRadius = sectorRadius * 2 * 1000, // Convert to diameter + km
                        Description = txtDescription.Text,
                        Sunlight = (float)Math.Round(sunlight / 100f, 2),
                        Economy = (float)Math.Round(economy / 100f, 2),
                        Security = (float)Math.Round(security / 100f, 2),
                        AllowRandomAnomalies = chkAllowRandomAnomalies.Checked,
                        DisableFactionLogic = chkDisableFactionLogic.Checked,
                        Placement = sectorPlacement
                    };

                    // Create new sector in selected cluster
                    _selectedCluster.Sectors.Add(sectorValue);

                    // Create zones based on the sector range
                    sectorValue.InitializeOrUpdateZones();

                    // Add to sector listbox
                    _ = MainForm.Instance.SectorsListBox.Items.Add(name);
                    MainForm.Instance.SectorsListBox.SelectedItem = name;
                    break;

                case "Update":
                    // Adjust first the gates
                    Gate[] gates = sectorValue.Zones.SelectMany(a => a.Gates).ToArray();
                    foreach (Gate gate in gates)
                    {
                        Sector sourceSector = MainForm.Instance.AllClusters.Values
                        .SelectMany(a => a.Sectors)
                        .First(a => a.Name.Equals(gate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                        Zone sourceZone = sourceSector.Zones
                            .First(a => a.Gates
                                .Any(a => a.SourcePath
                                    .Equals(gate.DestinationPath, StringComparison.OrdinalIgnoreCase)));
                        Gate sourceGate = sourceZone.Gates.First(a => a.SourcePath.Equals(gate.DestinationPath, StringComparison.OrdinalIgnoreCase));

                        sourceGate.DestinationSectorName = name;
                        gate.ParentSectorName = name;
                    }

                    // Update fields
                    sectorValue.Name = name;
                    sectorValue.Description = txtDescription.Text;
                    sectorValue.Sunlight = (float)Math.Round(sunlight / 100f, 2);
                    sectorValue.Economy = (float)Math.Round(economy / 100f, 2);
                    sectorValue.Security = (float)Math.Round(security / 100f, 2);
                    sectorValue.AllowRandomAnomalies = chkAllowRandomAnomalies.Checked;
                    sectorValue.DisableFactionLogic = chkDisableFactionLogic.Checked;
                    sectorValue.DiameterRadius = sectorRadius * 2 * 1000; // Convert to diameter + km
                    sectorValue.Placement = sectorPlacement;

                    int index = MainForm.Instance.SectorsListBox.SelectedIndex;
                    MainForm.Instance.SectorsListBox.Items[index] = name;
                    MainForm.Instance.SectorsListBox.SelectedItem = name;
                    break;
            }

            // Reposition sectors in the cluster
            if (!_selectedCluster.CustomSectorPositioning)
            {
                if (beforePlacement == null || (beforePlacement != Sector.Placement))
                {
                    _selectedCluster.AutoPositionSectors();
                }
            }

            // Determines the position inside the cluster based on the selected placement
            DetermineSectorOffset(_selectedCluster, sectorValue);
            ResetAndHide();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ResetAndHide();
        }

        private void ResetAndHide()
        {
            Sector = null;
            Close();
        }

        private void TxtSectorRadius_TextChanged(object sender, EventArgs e)
        {
            string radiusText = txtSectorRadius.Text;
            if (string.IsNullOrWhiteSpace(radiusText) || !int.TryParse(radiusText, out int radius))
            {
                lblRadiusUnderText.Text = $"(Please enter a valid numerical value for radius.)";
                lblRadiusUnderText.ForeColor = Color.Red;
            }
            else if (radius is <= 0 or > 999)
            {
                lblRadiusUnderText.Text = $"(Maximum radius must be between 1 and 999)";
                lblRadiusUnderText.ForeColor = Color.Red;
            }
            else
            {
                lblRadiusUnderText.Text = $"From the center, {radius}km in every direction. {radius * 2}km diameter.";
                lblRadiusUnderText.ForeColor = _controlColor;
            }
        }
    }
}
