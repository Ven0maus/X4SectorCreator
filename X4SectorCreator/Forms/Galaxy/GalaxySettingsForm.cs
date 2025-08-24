using System.ComponentModel;
using System.Globalization;
using System.Text;
using X4SectorCreator.Forms.Galaxy;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class GalaxySettingsForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static bool DisableAllStorylines { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static bool IsCustomGalaxy { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string GalaxyName { get; set; } = "xu_ep2_universe";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string HeadQuartersSector { get; set; }

        private static Dictionary<(int, int), Cluster> _baseGameClusters;

        private readonly LazyEvaluated<ProceduralGalaxyForm> _proceduralGalaxyForm = new(() => new ProceduralGalaxyForm(), a => !a.IsDisposed);

        private bool _originalDisableStoryLins;
        private bool _originalCustomGalaxy;
        private string _originalGalaxyName;
        private Sector _originalHeadQuartersSector;

        public GalaxySettingsForm()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            chkCustomGalaxy.Checked = IsCustomGalaxy;
            txtGalaxyName.Text = GalaxyName;

            // Init sector values
            BtnGenerateProceduralGalaxy.Enabled = IsCustomGalaxy;
            CmbPlayerHq.DropDown += CmbPlayerHq_DropDown;

            EnableSaveButtons(false);
            UpdateOriginals();
            InitPlayerHqSectorSelection();
        }

        private void CmbPlayerHq_DropDown(object sender, EventArgs e)
        {
            InitPlayerHqSectorSelection();
        }

        private void SetPlayerHqAsSelectedItem()
        {
            if (!string.IsNullOrWhiteSpace(HeadQuartersSector))
            {
                foreach (var cluster in MainForm.Instance.AllClusters.Values)
                {
                    foreach (var sector in cluster.Sectors)
                    {
                        var macro = GetSectorMacro(cluster, sector);
                        if (HeadQuartersSector == macro)
                        {
                            if (CmbPlayerHq.Items.Contains(sector))
                                CmbPlayerHq.SelectedItem = sector;
                            return;
                        }
                    }
                }
            }
        }

        private static string GetSectorMacro(Cluster cluster, Sector sector)
        {
            var sectorMacro = $"PREFIX_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";
            if (cluster.IsBaseGame && sector.IsBaseGame)
            {
                sectorMacro = $"{cluster.BaseGameMapping}_{sector.BaseGameMapping}_macro";
            }
            else if (cluster.IsBaseGame)
            {
                sectorMacro = $"PREFIX_SE_c{cluster.BaseGameMapping}_s{sector.Id}_macro";
            }
            return sectorMacro;
        }

        public void InitPlayerHqSectorSelection(bool? overwrite = null)
        {
            CmbPlayerHq.Enabled = overwrite ?? IsCustomGalaxy;
            CmbPlayerHq.Items.Clear();

            if (!CmbPlayerHq.Enabled)
            {
                CmbPlayerHq.SelectedIndex = -1;
                return;
            }

            // Setup items
            var sectors = MainForm.Instance.AllClusters.Values
                .SelectMany(a => a.Sectors)
                .Where(a => !a.IsBaseGame);
            foreach (var sector in sectors)
                CmbPlayerHq.Items.Add(sector);

            // Setup selection
            SetPlayerHqAsSelectedItem();
        }

        private void UpdateOriginals()
        {
            _originalDisableStoryLins = chkDisableAllStorylines.Checked;
            _originalCustomGalaxy = chkCustomGalaxy.Checked;
            _originalGalaxyName = txtGalaxyName.Text;
            _originalHeadQuartersSector = CmbPlayerHq.SelectedItem as Sector;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _ = Save();
        }

        private void BtnSaveAndClose_Click(object sender, EventArgs e)
        {
            if (Save())
                Close();
        }

        private bool Save()
        {
            if (chkCustomGalaxy.Checked && (string.IsNullOrWhiteSpace(txtGalaxyName.Text) || txtGalaxyName.Text.Equals("xu_ep2_universe", StringComparison.OrdinalIgnoreCase)))
            {
                _ = MessageBox.Show("Please select a valid galaxy name, it cannot be empty or equal to \"xu_ep2_universe\".", "Invalid custom galaxy name");
                return false;
            }

            // Return if no change
            if (IsCustomGalaxy == chkCustomGalaxy.Checked)
            {
                GalaxyName = txtGalaxyName.Text.ToLower();
                SetHQ();
                UpdateOriginals();
                EnableSaveButtons(false);
                return true;
            }

            Dictionary<(int, int), Cluster> mergedClusters = null;
            if (!chkCustomGalaxy.Checked)
            {
                _baseGameClusters ??= MainForm.Instance.InitAllVanillaClusters(false)
                        .Clusters.ToDictionary(a => (a.Position.X, a.Position.Y));

                // Perform validation to avoid key collisions
                mergedClusters = new Dictionary<(int, int), Cluster>(_baseGameClusters);
                List<Cluster> invalidClusters = [];
                foreach (KeyValuePair<(int, int), Cluster> cluster in MainForm.Instance.AllClusters)
                {
                    if (!mergedClusters.ContainsKey(cluster.Key) &&
                        !mergedClusters.Values.Any(c => c.Name.Equals(cluster.Value.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        mergedClusters.Add(cluster.Key, cluster.Value);
                    }
                    else
                    {
                        invalidClusters.Add(cluster.Value);
                    }
                }

                if (invalidClusters.Count > 0)
                {
                    _ = MessageBox.Show("Unable to revert to normal galaxy because some clusters are overlapping with the base game clusters.\n" +
                        "To revert back to a normal galaxy please adjust the position or naming of all the following clusters so there is no overlap:\n- " +
                        string.Join("\n- ", invalidClusters.Select(a => a.Name)));
                    return false;
                }
            }

            // Validate if there are any gate connections with basegame clusters existing if we're going to custom galaxy
            if (chkCustomGalaxy.Checked && ContainsGateConnectionsToBaseGameClusters(out List<(Cluster, Sector, Gate)> clusters))
            {
                DialogResult resultDialog = MessageBox.Show("There exist custom sectors that have a gate connection to a base game cluster, these will be removed when converting to a custom galaxy.\n" +
                    "Are you sure you want to continue?", "Warning: loss of sector connections!", MessageBoxButtons.YesNo);
                if (resultDialog == DialogResult.No)
                {
                    return false;
                }

                // Delete base game gate connections for these invalid clusters
                RemoveBaseGameGateConnections(clusters);
            }

            if (!chkCustomGalaxy.Checked)
            {
                GalaxyName = "xu_ep2_universe";
            }
            else
            {
                GalaxyName = txtGalaxyName.Text.ToLower();

                // Store base game clusters lazily
                _baseGameClusters ??= MainForm.Instance.AllClusters
                        .Where(a => a.Value.IsBaseGame)
                        .ToDictionary(a => a.Key, a => a.Value);
            }

            // Apply change
            IsCustomGalaxy = chkCustomGalaxy.Checked;
            DisableAllStorylines = chkDisableAllStorylines.Checked;
            BtnGenerateProceduralGalaxy.Enabled = IsCustomGalaxy;
            SetHQ();

            if (CmbPlayerHq.Enabled != IsCustomGalaxy)
            {
                InitPlayerHqSectorSelection();
            }

            // Toggle galaxy mode
            MainForm.Instance.ToggleGalaxyMode(mergedClusters);

            UpdateOriginals();
            EnableSaveButtons(false);

            return true;
        }

        private void SetHQ()
        {
            if (CmbPlayerHq.SelectedItem is not Sector hqSector)
            {
                HeadQuartersSector = null;
            }
            else
            {
                var hqCluster = MainForm.Instance.AllClusters.Values.First(a => a.Sectors.Contains(hqSector));
                HeadQuartersSector = GetSectorMacro(hqCluster, hqSector);
            }
        }

        private static bool ContainsGateConnectionsToBaseGameClusters(out List<(Cluster, Sector, Gate)> invalidClusters)
        {
            invalidClusters = [];
            List<KeyValuePair<(int, int), Cluster>> customClusters = MainForm.Instance.AllClusters
                .Where(a => !a.Value.IsBaseGame)
                .ToList();
            foreach (KeyValuePair<(int, int), Cluster> cluster in customClusters)
            {
                foreach (Sector sector in cluster.Value.Sectors)
                {
                    foreach (Zone zone in sector.Zones)
                    {
                        foreach (Gate gate in zone.Gates)
                        {
                            Cluster destCluster = FindDestinationCluster(gate);
                            if (destCluster.IsBaseGame)
                            {
                                invalidClusters.Add((cluster.Value, sector, gate));
                            }
                        }
                    }
                }
            }
            return invalidClusters.Count > 0;
        }

        private static void RemoveBaseGameGateConnections(List<(Cluster Cluster, Sector Sector, Gate Gate)> pairs)
        {
            foreach ((Cluster Cluster, Sector Sector, Gate Gate) pair in pairs)
            {
                // Delete target connection
                Zone targetZone = pair.Sector.Zones
                    .First(a => a.Gates
                        .Any(a => a.ParentSectorName
                            .Equals(pair.Sector.Name, StringComparison.OrdinalIgnoreCase)));
                _ = targetZone.Gates.Remove(pair.Gate);

                // Check to remove zone if empty
                if (targetZone.Gates.Count == 0)
                {
                    _ = pair.Sector.Zones.Remove(targetZone);
                }

                // Delete source connection
                Sector sourceSector = MainForm.Instance.AllClusters.Values
                    .SelectMany(a => a.Sectors)
                    .First(a => a.Name.Equals(pair.Gate.DestinationSectorName, StringComparison.OrdinalIgnoreCase));
                Zone sourceZone = sourceSector.Zones
                    .First(a => a.Gates
                        .Any(a => a.SourcePath
                            .Equals(pair.Gate.DestinationPath, StringComparison.OrdinalIgnoreCase)));
                Gate sourceGate = sourceZone.Gates.First(a => a.SourcePath.Equals(pair.Gate.DestinationPath, StringComparison.OrdinalIgnoreCase));
                _ = sourceZone.Gates.Remove(sourceGate);

                // Check to remove zone if empty
                if (sourceZone.Gates.Count == 0)
                {
                    _ = sourceSector.Zones.Remove(sourceZone);
                }
            }
        }

        private static Cluster FindDestinationCluster(Gate gate)
        {
            return MainForm.Instance.AllClusters.Values
                    .FirstOrDefault(cluster => cluster.Sectors.Any(sector =>
                        sector.Name.Equals(gate.DestinationSectorName, StringComparison.OrdinalIgnoreCase)));
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChkCustomGalaxy_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkCustomGalaxy.Checked)
            {
                // Reset to default galaxy
                txtGalaxyName.Text = "xu_ep2_universe";
                txtGalaxyName.Enabled = false;
                chkDisableAllStorylines.Enabled = true;
                chkDisableAllStorylines.Checked = false;
            }
            else
            {
                chkDisableAllStorylines.Enabled = false;
                chkDisableAllStorylines.Checked = true;
                txtGalaxyName.Enabled = true;
                txtGalaxyName.Text = (_originalGalaxyName == "xu_ep2_universe" || _originalGalaxyName == null) ?
                    string.Empty : _originalGalaxyName;
            }

            InitPlayerHqSectorSelection(chkCustomGalaxy.Checked);
            EnableSaveButtons(_originalCustomGalaxy != chkCustomGalaxy.Checked);
        }

        private readonly char[] _invalidChars = Path.GetInvalidFileNameChars();
        private void TxtGalaxyName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow backspace
            if (e.KeyChar == (char)Keys.Back)
            {
                return;
            }

            // Block whitespace and invalid characters
            if (char.IsWhiteSpace(e.KeyChar) || _invalidChars.Contains(e.KeyChar))
            {
                e.Handled = true; // Block input
            }

            // Convert the character to its non-accented form
            string normalized = RemoveDiacritics(e.KeyChar.ToString());

            // If the sanitized version is different, block input
            if (normalized != e.KeyChar.ToString())
            {
                e.Handled = true; // Prevent typing the original character
                txtGalaxyName.AppendText(normalized); // Type the sanitized version instead
            }
        }

        private static string RemoveDiacritics(string text)
        {
            return string.Concat(text.Normalize(NormalizationForm.FormD)
                                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
                                    .Normalize(NormalizationForm.FormC);
        }

        private void BtnGenerateProceduralGalaxy_Click(object sender, EventArgs e)
        {
            if (!IsCustomGalaxy)
            {
                _ = MessageBox.Show("Please change to \"Custom Galaxy\" mode first, by checking the checkbox and pressing Save.");
                return;
            }

            _proceduralGalaxyForm.Value.Show();
        }

        private void chkDisableAllStorylines_CheckedChanged(object sender, EventArgs e)
        {
            EnableSaveButtons(_originalDisableStoryLins != chkDisableAllStorylines.Checked);
        }

        private void EnableSaveButtons(bool enable)
        {
            BtnSave.Enabled = enable;
            BtnSaveAndClose.Enabled = enable;
        }

        private void txtGalaxyName_TextChanged(object sender, EventArgs e)
        {
            EnableSaveButtons(txtGalaxyName.Text != _originalGalaxyName);
        }

        private void CmbPlayerHq_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableSaveButtons(_originalHeadQuartersSector != CmbPlayerHq.SelectedItem);
        }
    }
}
