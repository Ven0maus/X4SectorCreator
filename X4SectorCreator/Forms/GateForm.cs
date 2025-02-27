using System.ComponentModel;
using System.Drawing.Drawing2D;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class GateForm : Form
    {
        private readonly int _hexRadius;
        private readonly PointF[] _hexagonPoints;

        private readonly int _worldRadius = 400000;
        private Point _sourceDotPosition, _targetDotPosition;
        private bool _dragging = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Cluster SourceCluster { get; set; }

        private Sector _sourceSector;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Sector SourceSector
        {
            get => _sourceSector;
            set
            {
                _sourceSector = value;
                if (_sourceSector == null)
                {
                    txtSourceSector.ResetText();
                    txtSourceSectorLocation.ResetText();
                }
                else
                {
                    txtSourceSector.Text = _sourceSector.Name;
                    int sectorIndex = SourceCluster.Sectors.IndexOf(_sourceSector);
                    txtSourceSectorLocation.Text = (SourceCluster.Position.X, SourceCluster.Position.Y).ToString() + $" [{sectorIndex}]";
                }
            }
        }

        public GateForm()
        {
            InitializeComponent();

            // Create and define hexagon
            _hexagonPoints = new PointF[6];
            _hexRadius = (int)Math.Min(SourceSectorHexagon.Width / 2, SourceSectorHexagon.Height / (float)Math.Sqrt(3));

            // Init hexagon
            InitializeHexagon();

            // Set inital position
            UpdateGatePosition(SourceSectorHexagon, txtSourceGatePosition, _sourceDotPosition);
            UpdateGatePosition(TargetSectorHexagon, txtTargetGatePosition, _targetDotPosition);

            // Attach events
            SourceSectorHexagon.Paint += SourceSectorHexagon_Paint;
            SourceSectorHexagon.MouseDown += SourceSectorHexagon_MouseDown;
            SourceSectorHexagon.MouseMove += SourceSectorHexagon_MouseMove;
            SourceSectorHexagon.MouseUp += SourceSectorHexagon_MouseUp;

            TargetSectorHexagon.Paint += TargetSectorHexagon_Paint;
            TargetSectorHexagon.MouseDown += TargetSectorHexagon_MouseDown;
            TargetSectorHexagon.MouseMove += TargetSectorHexagon_MouseMove;
            TargetSectorHexagon.MouseUp += TargetSectorHexagon_MouseUp;
        }

        public void Reset()
        {
            // Source gate
            txtSourceGatePitch.Text = "0";
            txtSourceGateRoll.Text = "0";
            txtSourceGateYaw.Text = "0";

            // Target gate
            txtTargetGatePitch.Text = "0";
            txtTargetGateRoll.Text = "0";
            txtTargetGateYaw.Text = "0";

            // Sectors
            txtTargetSector.ResetText();
            txtTargetSectorLocation.ResetText();
            txtSourceSector.ResetText();
            txtSourceSectorLocation.ResetText();

            // Reset dot position
            _sourceDotPosition = SourceSectorHexagon.ClientRectangle.Center();
            _targetDotPosition = TargetSectorHexagon.ClientRectangle.Center();

            // Reset gate positions
            UpdateGatePosition(SourceSectorHexagon, txtSourceGatePosition, _sourceDotPosition);
            UpdateGatePosition(TargetSectorHexagon, txtTargetGatePosition, _targetDotPosition);

            // Reset objects, make sure to call reset BEFORE assigning not after
            SourceCluster = null;
            SourceSector = null;
        }

        private void InitializeHexagon()
        {
            int centerX = SourceSectorHexagon.Width / 2;
            int centerY = SourceSectorHexagon.Height / 2;

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i;
                _hexagonPoints[i] = new PointF(
                    centerX + (_hexRadius * (float)Math.Cos(angle)),
                    centerY + (_hexRadius * (float)Math.Sin(angle))
                );
            }

            // Both hexagons are shared
            _sourceDotPosition = SourceSectorHexagon.ClientRectangle.Center();
            _targetDotPosition = TargetSectorHexagon.ClientRectangle.Center();
        }

        private void SourceSectorHexagon_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw hexagon
            using (Pen pen = new(Color.Black, 2))
            {
                g.DrawPolygon(pen, _hexagonPoints);
            }

            // Draw draggable dot
            using Brush brush = new SolidBrush(Color.Red);
            g.FillEllipse(brush, _sourceDotPosition.X - 5, _sourceDotPosition.Y - 5, 10, 10);
        }

        private void SourceSectorHexagon_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsPointInsideHexagon(e.Location))
            {
                _dragging = true;
            }
        }

        private void SourceSectorHexagon_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                if (IsPointInsideHexagon(e.Location))
                {
                    _sourceDotPosition = e.Location;
                    SourceSectorHexagon.Invalidate();
                    UpdateGatePosition(SourceSectorHexagon, txtSourceGatePosition, _sourceDotPosition);
                }
            }
        }

        private void SourceSectorHexagon_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void TargetSectorHexagon_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw hexagon
            using (Pen pen = new(Color.Black, 2))
            {
                g.DrawPolygon(pen, _hexagonPoints);
            }

            // Draw draggable dot
            using Brush brush = new SolidBrush(Color.Red);
            g.FillEllipse(brush, _targetDotPosition.X - 5, _targetDotPosition.Y - 5, 10, 10);
        }

        private void TargetSectorHexagon_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsPointInsideHexagon(e.Location))
            {
                _dragging = true;
            }
        }

        private void TargetSectorHexagon_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                if (IsPointInsideHexagon(e.Location))
                {
                    _targetDotPosition = e.Location;
                    TargetSectorHexagon.Invalidate();
                    UpdateGatePosition(TargetSectorHexagon, txtTargetGatePosition, _targetDotPosition);
                }
            }
        }

        private void TargetSectorHexagon_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void UpdateGatePosition(PictureBox sectorHexagon, TextBox textBox, Point dotPosition)
        {
            int centerX = sectorHexagon.Width / 2;
            int centerY = sectorHexagon.Height / 2;

            float normalizedX = (dotPosition.X - centerX) / (float)_hexRadius;
            float normalizedY = -(dotPosition.Y - centerY) / (float)_hexRadius;

            float worldX = normalizedX * _worldRadius / 2;
            float worldY = normalizedY * _worldRadius / 2;

            textBox.Text = $"({worldX:0}, {worldY:0})";
        }

        private bool IsPointInsideHexagon(Point point)
        {
            using GraphicsPath path = new();
            path.AddPolygon(_hexagonPoints);
            return path.IsVisible(point);
        }

        private void BtnCreateConnection_Click(object sender, EventArgs e)
        {
            #region Source Gate Connection
            string selectedSourceType = cmbSourceType.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedSourceType))
            {
                _ = MessageBox.Show("Please select a valid Source Gate Type.");
                return;
            }

            System.Text.RegularExpressions.Match sourceGatePosMatch = RegexHelper.TupleLocationRegex().Match(txtSourceGatePosition.Text);
            if (!sourceGatePosMatch.Success)
            {
                _ = MessageBox.Show("Unable to parse source gate position.");
                return;
            }

            // Gate Position
            (int GatePosX, int GatePosY) = (int.Parse(sourceGatePosMatch.Groups[1].Value), int.Parse(sourceGatePosMatch.Groups[2].Value));

            // Create a new gate connection in the source
            Gate sourceGate = new()
            {
                Id = 1,
                ParentSectorName = SourceSector.Name,
                Type = selectedSourceType.Equals("Gate", StringComparison.OrdinalIgnoreCase) ?
                    Gate.GateType.props_gates_anc_gate_macro : Gate.GateType.props_gates_orb_accelerator_01_macro,
                Yaw = int.Parse(txtSourceGateYaw.Text),
                Pitch = int.Parse(txtSourceGatePitch.Text),
                Roll = int.Parse(txtSourceGateRoll.Text)
            };

            // Create a new source zone
            Zone sourceZone = new()
            {
                Id = SourceSector.Zones.Count + 1,
                Name = "Zone " + (SourceSector.Zones.Count + 1),
                Position = new Point(GatePosX, GatePosY),
                Gates = [sourceGate]
            };
            SourceSector.Zones.Add(sourceZone);
            #endregion

            #region Target Gate Connection
            System.Text.RegularExpressions.Match targetSectorLocationMatch = RegexHelper.TupleLocationChildIndexRegex().Match(txtTargetSectorLocation.Text);
            if (!targetSectorLocationMatch.Success)
            {
                _ = SourceSector.Zones.Remove(sourceZone);
                _ = MessageBox.Show($"Invalid sector selected, cannot properly parse \"{txtTargetSectorLocation.Text}\".");
                return;
            }

            string selectedTargetType = cmbTargetType.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedTargetType))
            {
                _ = SourceSector.Zones.Remove(sourceZone);
                _ = MessageBox.Show("Please select a valid Target Gate Type.");
                return;
            }

            (int targetSectorX, int targetSectorY, int targetSectorIndex) = (int.Parse(targetSectorLocationMatch.Groups[1].Value),
                int.Parse(targetSectorLocationMatch.Groups[2].Value),
                int.Parse(targetSectorLocationMatch.Groups[3].Value));

            // Find target cluster / sector
            if (!MainForm.Instance.AllClusters.TryGetValue((targetSectorX, targetSectorY), out Cluster targetCluster))
            {
                _ = SourceSector.Zones.Remove(sourceZone);
                _ = MessageBox.Show("Invalid sector selection.");
                return;
            }

            // Find sector
            Sector targetSector = targetCluster.Sectors[targetSectorIndex];


            // Validate that target sector != source sector
            if (targetSector == SourceSector)
            {
                _ = SourceSector.Zones.Remove(sourceZone);
                _ = MessageBox.Show("Target sector cannot be the same as the source sector.");
                return;
            }

            // Create a new gate connection in the target
            System.Text.RegularExpressions.Match targetGatePosMatch = RegexHelper.TupleLocationRegex().Match(txtTargetGatePosition.Text);
            if (!targetGatePosMatch.Success)
            {
                _ = SourceSector.Zones.Remove(sourceZone);
                _ = MessageBox.Show("Unable to parse target gate position.");
                return;
            }

            // Gate Position
            (GatePosX, GatePosY) = (int.Parse(targetGatePosMatch.Groups[1].Value), int.Parse(targetGatePosMatch.Groups[2].Value));

            // Create a new gate connection in the target
            Gate targetGate = new()
            {
                Id = 1,
                ParentSectorName = targetSector.Name,
                DestinationSectorName = SourceSector.Name,
                Type = selectedTargetType.Equals("Gate", StringComparison.OrdinalIgnoreCase) ?
                    Gate.GateType.props_gates_anc_gate_macro : Gate.GateType.props_gates_orb_accelerator_01_macro,
                Yaw = int.Parse(txtTargetGateYaw.Text),
                Pitch = int.Parse(txtTargetGatePitch.Text),
                Roll = int.Parse(txtTargetGateRoll.Text)
            };

            // Create new target zone
            Zone targetZone = new()
            {
                Id = targetSector.Zones.Count + 1,
                Name = "Zone " + (targetSector.Zones.Count + 1),
                Position = new Point(GatePosX, GatePosY),
                Gates = [targetGate]
            };
            targetSector.Zones.Add(targetZone);
            #endregion

            // SourceGate source / destination
            sourceGate.Source = $"c{SourceCluster.Id:D3}_s{SourceSector.Id:D3}_z{sourceZone.Id:D3}";
            sourceGate.Destination = $"c{targetCluster.Id:D3}_s{targetSector.Id:D3}_z{targetZone.Id:D3}";

            // TargetGate source / destination
            targetGate.Source = $"c{targetCluster.Id:D3}_s{targetSector.Id:D3}_z{targetZone.Id:D3}";
            targetGate.Destination = $"c{SourceCluster.Id:D3}_s{SourceSector.Id:D3}_z{sourceZone.Id:D3}";

            // Paths must be set at the end
            targetGate.SetSourcePath("PREFIX", targetCluster, targetSector, targetZone);
            sourceGate.SetSourcePath("PREFIX", SourceCluster, SourceSector, sourceZone);
            sourceGate.SetDestinationPath("PREFIX", targetCluster, targetSector, targetZone, targetGate);
            targetGate.SetDestinationPath("PREFIX", SourceCluster, SourceSector, sourceZone, sourceGate);

            // Add target gate to listbox
            sourceGate.DestinationSectorName = targetSector.Name;
            _ = MainForm.Instance.GatesListBox.Items.Add(targetGate);
            MainForm.Instance.GatesListBox.SelectedItem = targetGate;

            Reset();
            Close();
        }

        private void BtnSelectSector_Click(object sender, EventArgs e)
        {
            MainForm.Instance.SectorMapForm.GateSectorSelection = true;
            MainForm.Instance.SectorMapForm.BtnSelectLocation.Enabled = false;
            MainForm.Instance.SectorMapForm.ControlPanel.Size = new Size(204, 144);
            MainForm.Instance.SectorMapForm.BtnSelectLocation.Show();
            MainForm.Instance.SectorMapForm.Reset();
            MainForm.Instance.SectorMapForm.Show();
        }
    }
}
