﻿using System.ComponentModel;
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
                    txtCustomTags.Text = _sector.Tags;
                    txtSectorRadius.Text = ((int)(_sector.DiameterRadius / 1000f / 2f)).ToString();
                    chkAllowRandomAnomalies.Checked = _sector.AllowRandomAnomalies;
                    chkDisableFactionLogic.Checked = _sector.DisableFactionLogic;
                }
                else
                {
                    TxtName.Text = string.Empty;
                    txtDescription.Text = string.Empty;
                    txtSunlight.Text = "100";
                    txtEconomy.Text = "100";
                    txtSecurity.Text = "100";
                    txtSectorRadius.Text = "250";
                    txtCustomTags.Text = string.Empty;
                    chkAllowRandomAnomalies.Checked = true;
                    chkDisableFactionLogic.Checked = false;
                }
            }
        }

        private readonly Color _controlColor;

        public SectorForm()
        {
            InitializeComponent();
            _controlColor = lblRadiusUnderText.ForeColor;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            string name = TxtName.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                _ = MessageBox.Show("Please select a valid (non empty / non whitespace) name.");
                return;
            }

            string selectedClusterName = MainForm.Instance.ClustersListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedClusterName))
            {
                _ = MessageBox.Show("Please select a cluster in the main window.");
                return;
            }

            // Check if name already exists
            foreach (Cluster cluster in MainForm.Instance.AllClusters.Values)
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    // Skip the one we are modifying
                    if (Sector != null && Sector.Name.Equals(sector.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (sector.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        _ = MessageBox.Show($"A sector with the name \"{name}\" already exists in cluster \"{cluster.Name}\", please choose another name.");
                        return;
                    }
                }
            }

            // Validate sector radius
            if (!int.TryParse(txtSectorRadius.Text, out var sectorRadius) || sectorRadius <= 0 || sectorRadius > 999)
            {
                _ = MessageBox.Show($"Please use a valid numerical value for the sector radius, and specify a value between 1 and 999.");
                return;
            }

            // Validate sunlight, economy, security
            if (!int.TryParse(txtSunlight.Text, out var sunlight) ||
                !int.TryParse(txtEconomy.Text, out var economy) ||
                !int.TryParse(txtSecurity.Text, out var security))
            {
                _ = MessageBox.Show($"Please use valid numerical values for sunlight, economy and security.");
                return;
            }

            KeyValuePair<(int, int), Cluster> selectedCluster = MainForm.Instance.AllClusters
                .First(a => a.Value.Name.Equals(selectedClusterName, StringComparison.OrdinalIgnoreCase));

            switch (BtnCreate.Text)
            {
                case "Create":
                    int offSetX, offsetY;
                    if (selectedCluster.Value.Sectors.Count == 0)
                    {
                        offSetX = 0;
                        offsetY = 0;
                    }
                    if (selectedCluster.Value.Sectors.Count == 1)
                    {
                        // Down
                        offSetX = 0;
                        offsetY = -1000000;
                    }
                    else if (selectedCluster.Value.Sectors.Count == 2)
                    {
                        // Right
                        offSetX = 1000000;
                        offsetY = -500000;
                    }
                    else
                    {
                        _ = MessageBox.Show($"Reached maximum allowed sectors for cluster \"{selectedCluster.Value.Name}\".");
                        return;
                    }

                    // Create new sector in selected cluster
                    selectedCluster.Value.Sectors.Add(new Sector
                    {
                        Id = selectedCluster.Value.Sectors.DefaultIfEmpty(new Sector()).Max(a => a.Id) + 1,
                        Name = name,
                        Owner = "None",
                        DiameterRadius = sectorRadius * 2 * 1000, // Convert to diameter + km
                        Description = txtDescription.Text,
                        Sunlight = (float)Math.Round(sunlight / 100f, 2),
                        Economy = (float)Math.Round(economy / 100f, 2),
                        Security = (float)Math.Round(security / 100f, 2),
                        Tags = txtCustomTags.Text,
                        AllowRandomAnomalies = chkAllowRandomAnomalies.Checked,
                        DisableFactionLogic = chkDisableFactionLogic.Checked,
                        Offset = new Point(offSetX, offsetY),
                        Zones = [],
                        Regions = []
                    });

                    // Add to sector listbox
                    MainForm.Instance.SectorsListBox.Items.Add(name);
                    MainForm.Instance.SectorsListBox.SelectedItem = name;
                    break;

                case "Update":
                    string selectedSector = MainForm.Instance.SectorsListBox.SelectedItem as string;
                    if (string.IsNullOrEmpty(selectedSector))
                    {
                        ResetAndHide();
                        return;
                    }
                    Sector sector = selectedCluster.Value.Sectors.First(a => a.Name.Equals(selectedSector));

                    // Update fields
                    sector.Name = name;
                    sector.Description = txtDescription.Text;
                    sector.Sunlight = (float)Math.Round(sunlight / 100f, 2);
                    sector.Economy = (float)Math.Round(economy / 100f, 2);
                    sector.Security = (float)Math.Round(security / 100f, 2);
                    sector.Tags = txtCustomTags.Text;
                    sector.AllowRandomAnomalies = chkAllowRandomAnomalies.Checked;
                    sector.DisableFactionLogic = chkDisableFactionLogic.Checked;
                    sector.DiameterRadius = sectorRadius * 2 * 1000; // Convert to diameter + km

                    int index = MainForm.Instance.SectorsListBox.SelectedIndex;
                    MainForm.Instance.SectorsListBox.Items[index] = name;
                    MainForm.Instance.SectorsListBox.SelectedItem = name;
                    break;
            }


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
            var radiusText = txtSectorRadius.Text;
            if (string.IsNullOrWhiteSpace(radiusText) || !int.TryParse(radiusText, out var radius))
            {
                lblRadiusUnderText.Text = $"(Please enter a valid numerical value for radius.)";
                lblRadiusUnderText.ForeColor = Color.Red;
            }
            else if (radius <= 0 || radius > 999)
            {
                lblRadiusUnderText.Text = $"(Maximum radius must be between 1 and 999)";
                lblRadiusUnderText .ForeColor = Color.Red;
            }
            else
            {
                lblRadiusUnderText.Text = $"From the center, {radiusText}km in every direction. {radiusText}km diameter.";
                lblRadiusUnderText.ForeColor = _controlColor;
            }
        }
    }
}
