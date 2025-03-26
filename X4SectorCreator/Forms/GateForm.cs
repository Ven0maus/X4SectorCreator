using System.ComponentModel;
using System.Drawing.Drawing2D;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class GateForm : Form
    {
        private readonly int _hexRadius;
        private readonly PointF[] _hexagonPoints;

        private Point _sourceDotPosition, _targetDotPosition;
        private bool _dragging = false, _rotating = false;
        private float _sourceYaw, _targetYaw;

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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Sector TargetSectorSelection { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateInfo UpdateInfoObject { get; set; }

        public class UpdateInfo
        {
            public Gate SourceGate { get; set; }
            public Zone SourceZone { get; set; }
            public Sector SourceSector { get; set; }
            public Cluster SourceCluster { get; set; }
            public Gate TargetGate { get; set; }
            public Zone TargetZone { get; set; }
            public Sector TargetSector { get; set; }
            public Cluster TargetCluster { get; set; }
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
            if (SourceSector != null)
            {
                UpdateGatePosition(SourceSectorHexagon, txtSourceGatePosition, _sourceDotPosition, SourceSector.DiameterRadius);
                // Inherit the source diameter for now, it will adapt when sector is selected automatically
                UpdateGatePosition(TargetSectorHexagon, txtTargetGatePosition, _targetDotPosition, SourceSector.DiameterRadius);
            }

            // Attach events
            SourceSectorHexagon.Paint += SourceSectorHexagon_Paint;
            SourceSectorHexagon.MouseDown += SourceSectorHexagon_MouseDown;
            SourceSectorHexagon.MouseMove += SourceSectorHexagon_MouseMove;
            SourceSectorHexagon.MouseUp += SourceSectorHexagon_MouseUp;
            SourceSectorHexagon.MouseClick += SourceSectorHexagon_MouseClick;

            TargetSectorHexagon.Paint += TargetSectorHexagon_Paint;
            TargetSectorHexagon.MouseDown += TargetSectorHexagon_MouseDown;
            TargetSectorHexagon.MouseMove += TargetSectorHexagon_MouseMove;
            TargetSectorHexagon.MouseUp += TargetSectorHexagon_MouseUp;
            TargetSectorHexagon.MouseClick += TargetSectorHexagon_MouseClick;
        }

        public void PrepareForUpdate()
        {
            // Source gate
            txtSourceGatePitch.Text = UpdateInfoObject.SourceGate.Pitch.ToString();
            txtSourceGateRoll.Text = UpdateInfoObject.SourceGate.Roll.ToString();
            txtSourceGateYaw.Text = UpdateInfoObject.SourceGate.Yaw.ToString();

            // Target gate
            txtTargetGatePitch.Text = UpdateInfoObject.TargetGate.Pitch.ToString();
            txtTargetGateRoll.Text = UpdateInfoObject.TargetGate.Roll.ToString();
            txtTargetGateYaw.Text = UpdateInfoObject.TargetGate.Yaw.ToString();

            // Sectors
            txtTargetSector.Text = UpdateInfoObject.TargetSector.Name;
            int targetIndex = UpdateInfoObject.TargetCluster.Sectors.IndexOf(UpdateInfoObject.TargetSector);
            txtTargetSectorLocation.Text = (UpdateInfoObject.TargetCluster.Position.X, UpdateInfoObject.TargetCluster.Position.Y).ToString() + $" [{targetIndex}]";
            txtSourceSector.Text = UpdateInfoObject.SourceSector.Name;
            int sourceIndex = UpdateInfoObject.SourceCluster.Sectors.IndexOf(UpdateInfoObject.SourceSector);
            txtSourceSectorLocation.Text = (UpdateInfoObject.SourceCluster.Position.X, UpdateInfoObject.SourceCluster.Position.Y).ToString() + $" [{sourceIndex}]";

            // Revert dot positions from world coordinates
            _sourceDotPosition = ConvertFromWorldCoordinate(UpdateInfoObject.SourceZone.Position, UpdateInfoObject.SourceSector.DiameterRadius);
            _targetDotPosition = ConvertFromWorldCoordinate(UpdateInfoObject.TargetZone.Position, UpdateInfoObject.TargetSector.DiameterRadius);

            // Set rotation of dot
            _sourceYaw = UpdateInfoObject.SourceGate.Yaw;
            _targetYaw = UpdateInfoObject.TargetGate.Yaw;

            // Reset gate positions
            txtSourceGatePosition.Text = $"({UpdateInfoObject.SourceZone.Position.X:0}, {UpdateInfoObject.SourceZone.Position.Y:0})";
            txtTargetGatePosition.Text = $"({UpdateInfoObject.TargetZone.Position.X:0}, {UpdateInfoObject.TargetZone.Position.Y:0})";

            // Invalidate
            SourceSectorHexagon.Invalidate();
            TargetSectorHexagon.Invalidate();
        }

        private Point ConvertFromWorldCoordinate(Point coordinate, int diameter)
        {
            int centerX = SourceSectorHexagon.Width / 2;
            int centerY = SourceSectorHexagon.Height / 2;

            // Reverse world scaling
            float normalizedX = coordinate.X * 2f / diameter;
            float normalizedY = coordinate.Y * 2f / diameter;

            // Reverse normalization and centering
            float screenX = (normalizedX * _hexRadius) + centerX;
            float screenY = (-normalizedY * _hexRadius) + centerY; // Correct Y-axis negation

            return new Point((int)Math.Round(screenX), (int)Math.Round(screenY));
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

            // Draw direction indicator (arrow)
            DrawArrow(g, _sourceDotPosition, _sourceYaw);
        }

        private void SourceSectorHexagon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsPointInsideHexagon(e.Location))
            {
                _dragging = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _rotating = true;
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
                    UpdateGatePosition(SourceSectorHexagon, txtSourceGatePosition, _sourceDotPosition, UpdateInfoObject?.SourceSector != null ?
                        UpdateInfoObject.SourceSector.DiameterRadius : SourceSector.DiameterRadius);
                }
            }
            else if (_rotating) // If right-click is held, adjust yaw
            {
                _sourceYaw = (float)(Math.Atan2(e.Y - _sourceDotPosition.Y, e.X - _sourceDotPosition.X) * (180.0 / Math.PI)) + 90;
                if (_sourceYaw < 0)
                {
                    _sourceYaw += 360;
                }

                txtSourceGateYaw.Text = $"{_sourceYaw:0}";
                SourceSectorHexagon.Invalidate();
            }
        }

        private void SourceSectorHexagon_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
            _rotating = false;
        }

        private void SourceSectorHexagon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsPointInsideHexagon(e.Location))
            {
                _sourceDotPosition = e.Location;
                SourceSectorHexagon.Invalidate();
                UpdateGatePosition(SourceSectorHexagon, txtSourceGatePosition, _sourceDotPosition, UpdateInfoObject?.SourceSector != null ?
                    UpdateInfoObject.SourceSector.DiameterRadius : SourceSector.DiameterRadius);
            }
            else if (e.Button == MouseButtons.Right)
            {
                _sourceYaw = (float)(Math.Atan2(e.Y - _sourceDotPosition.Y, e.X - _sourceDotPosition.X) * (180.0 / Math.PI)) + 90;
                if (_sourceYaw < 0)
                {
                    _sourceYaw += 360;
                }

                txtSourceGateYaw.Text = $"{_sourceYaw:0}";
                SourceSectorHexagon.Invalidate();
            }
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

            // Draw direction indicator (arrow)
            DrawArrow(g, _targetDotPosition, _targetYaw);
        }

        private void TargetSectorHexagon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsPointInsideHexagon(e.Location))
            {
                _dragging = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _rotating = true;
            }
        }

        private void TargetSectorHexagon_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                if (IsPointInsideHexagon(e.Location))
                {
                    Point prev = _targetDotPosition;
                    _targetDotPosition = e.Location;


                    if (UpdateInfoObject?.TargetSector != null)
                    {
                        UpdateGatePosition(TargetSectorHexagon, txtTargetGatePosition, _targetDotPosition, UpdateInfoObject.TargetSector.DiameterRadius);
                    }
                    else
                    {
                        if (TargetSectorSelection == null)
                        {
                            // Check if we have a target selected
                            if (!GetTargetSectorSelection(out Sector targetSectorSelection, out _))
                            {
                                _targetDotPosition = prev;
                                _dragging = false;
                                return;
                            }
                            TargetSectorSelection = targetSectorSelection;
                        }
                        UpdateGatePosition(TargetSectorHexagon, txtTargetGatePosition, _targetDotPosition, TargetSectorSelection.DiameterRadius);
                    }

                    TargetSectorHexagon.Invalidate();
                }
            }
            else if (_rotating) // If right-click is held, adjust yaw
            {
                _targetYaw = (float)(Math.Atan2(e.Y - _targetDotPosition.Y, e.X - _targetDotPosition.X) * (180.0 / Math.PI)) + 90;
                if (_targetYaw < 0)
                {
                    _targetYaw += 360;
                }

                txtTargetGateYaw.Text = $"{_targetYaw:0}";
                TargetSectorHexagon.Invalidate();
            }
        }

        private void TargetSectorHexagon_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
            _rotating = false;
        }

        private void TargetSectorHexagon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsPointInsideHexagon(e.Location))
            {
                Point prev = _targetDotPosition;
                _targetDotPosition = e.Location;

                if (UpdateInfoObject?.TargetSector != null)
                {
                    UpdateGatePosition(TargetSectorHexagon, txtTargetGatePosition, _targetDotPosition, UpdateInfoObject.TargetSector.DiameterRadius);
                }
                else
                {
                    if (TargetSectorSelection == null)
                    {
                        // Check if we have a target selected
                        if (!GetTargetSectorSelection(out Sector targetSectorSelection, out _))
                        {
                            _targetDotPosition = prev;
                            return;
                        }
                        TargetSectorSelection = targetSectorSelection;
                    }
                    UpdateGatePosition(TargetSectorHexagon, txtTargetGatePosition, _targetDotPosition, TargetSectorSelection.DiameterRadius);
                }
                TargetSectorHexagon.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                _targetYaw = (float)(Math.Atan2(e.Y - _targetDotPosition.Y, e.X - _targetDotPosition.X) * (180.0 / Math.PI)) + 90;
                if (_targetYaw < 0)
                {
                    _targetYaw += 360;
                }

                txtTargetGateYaw.Text = $"{_targetYaw:0}";
                TargetSectorHexagon.Invalidate();
            }
        }

        private void UpdateGatePosition(PictureBox sectorHexagon, TextBox positionTextBox, Point dotPosition, int diameter)
        {
            int centerX = sectorHexagon.Width / 2;
            int centerY = sectorHexagon.Height / 2;

            float normalizedX = (dotPosition.X - centerX) / (float)_hexRadius;
            float normalizedY = -(dotPosition.Y - centerY) / (float)_hexRadius;

            float worldX = normalizedX * diameter / 2;
            float worldY = normalizedY * diameter / 2;

            positionTextBox.Text = $"({worldX:0}, {worldY:0})";
        }

        private static void DrawArrow(Graphics g, Point dotPosition, float angleDegrees)
        {
            double angleRadians = (angleDegrees - 90) * (Math.PI / 180.0); // Adjust for new system
            int arrowLength = 20; // Adjust length

            Point arrowEnd = new(
                (int)(dotPosition.X + (arrowLength * Math.Cos(angleRadians))),
                (int)(dotPosition.Y + (arrowLength * Math.Sin(angleRadians)))
            );

            using Pen arrowPen = new(Color.Blue, 2);
            g.DrawLine(arrowPen, dotPosition, arrowEnd);
        }

        private bool IsPointInsideHexagon(Point point)
        {
            using GraphicsPath path = new();
            path.AddPolygon(_hexagonPoints);
            return path.IsVisible(point);
        }

        private bool GetTargetSectorSelection(out Sector targetSector, out Cluster targetCluster)
        {
            targetSector = null;
            targetCluster = null;
            // Validate if the select target is still the same
            System.Text.RegularExpressions.Match targetSectorLocationMatch = RegexHelper.TupleLocationChildIndexRegex().Match(txtTargetSectorLocation.Text);
            if (!targetSectorLocationMatch.Success)
            {
                _ = MessageBox.Show($"Please select a valid target sector first.");
                return false;
            }

            (int targetSectorX, int targetSectorY, int targetSectorIndex) = (int.Parse(targetSectorLocationMatch.Groups[1].Value),
                int.Parse(targetSectorLocationMatch.Groups[2].Value),
                int.Parse(targetSectorLocationMatch.Groups[3].Value));

            // Find target cluster / sector
            if (!MainForm.Instance.AllClusters.TryGetValue((targetSectorX, targetSectorY), out targetCluster))
            {
                _ = MessageBox.Show("Invalid sector selection.");
                return false;
            }

            targetSector = targetCluster.Sectors[targetSectorIndex];
            return true;
        }

        private void BtnUpdateConnection_Click()
        {
            string selectedTargetType = cmbTargetType.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedTargetType))
            {
                _ = MessageBox.Show("Please select a valid Target Gate Type.");
                return;
            }

            string selectedSourceType = cmbSourceType.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedTargetType))
            {
                _ = MessageBox.Show("Please select a valid Source Gate Type.");
                return;
            }

            if (!GetTargetSectorSelection(out Sector targetSector, out Cluster targetCluster))
            {
                return;
            }

            // Validate that target sector != source sector
            if (targetSector == SourceSector)
            {
                _ = MessageBox.Show("Target sector cannot be the same as the source sector.");
                return;
            }

            System.Text.RegularExpressions.Match targetGatePosMatch = RegexHelper.TupleLocationRegex().Match(txtTargetGatePosition.Text);
            if (!targetGatePosMatch.Success)
            {
                _ = MessageBox.Show("Unable to parse target gate position.");
                return;
            }

            System.Text.RegularExpressions.Match sourceGatePosMatch = RegexHelper.TupleLocationRegex().Match(txtSourceGatePosition.Text);
            if (!sourceGatePosMatch.Success)
            {
                _ = MessageBox.Show("Unable to parse source gate position.");
                return;
            }

            // Gate Position
            (int GatePosX, int GatePosY) = (int.Parse(targetGatePosMatch.Groups[1].Value), int.Parse(targetGatePosMatch.Groups[2].Value));

            Gate targetGate = UpdateInfoObject.TargetGate;
            Zone targetZone = UpdateInfoObject.TargetZone;

            if (targetSector == UpdateInfoObject.TargetSector)
            {
                // No Target sector change, just update the target gate properties
                targetGate.Type = selectedTargetType.Equals("Gate", StringComparison.OrdinalIgnoreCase) ?
                    Gate.GateType.props_gates_anc_gate_macro : Gate.GateType.props_gates_orb_accelerator_01_macro;
                targetGate.Yaw = int.Parse(txtTargetGateYaw.Text);
                targetGate.Pitch = int.Parse(txtTargetGatePitch.Text);
                targetGate.Roll = int.Parse(txtTargetGateRoll.Text);
                targetZone.Position = new Point(GatePosX, GatePosY);
            }
            else
            {
                // Target Sector was changed, remove the target zone + gate, and create a new target gate
                _ = UpdateInfoObject.TargetSector.Zones.Remove(targetZone);
                UpdateInfoObject.TargetSector = targetSector; // Update to new prevent confusion and mistakes
                UpdateInfoObject.TargetCluster = targetCluster;

                UpdateInfoObject.TargetGate = new()
                {
                    Id = 1,
                    ParentSectorName = targetSector.Name,
                    DestinationSectorName = UpdateInfoObject.SourceSector.Name,
                    Type = selectedTargetType.Equals("Gate", StringComparison.OrdinalIgnoreCase) ?
                    Gate.GateType.props_gates_anc_gate_macro : Gate.GateType.props_gates_orb_accelerator_01_macro,
                    Yaw = int.Parse(txtTargetGateYaw.Text),
                    Pitch = int.Parse(txtTargetGatePitch.Text),
                    Roll = int.Parse(txtTargetGateRoll.Text)
                };

                UpdateInfoObject.TargetZone = new()
                {
                    Id = targetSector.Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
                    Position = new Point(GatePosX, GatePosY),
                    Gates = [UpdateInfoObject.TargetGate]
                };

                // Add to the new zone
                UpdateInfoObject.TargetSector.Zones.Add(UpdateInfoObject.TargetZone);

                // Set paths correctly
                UpdateInfoObject.TargetGate.Source = UpdateInfoObject.SourceGate.Destination;
                UpdateInfoObject.TargetGate.Destination = UpdateInfoObject.SourceGate.Source;
                UpdateInfoObject.TargetGate.SetDestinationPath("PREFIX", UpdateInfoObject.SourceCluster,
                    UpdateInfoObject.SourceSector, UpdateInfoObject.SourceZone, UpdateInfoObject.SourceGate);

                // Make sure soure gate points to new destination sector name
                UpdateInfoObject.SourceGate.SetDestinationPath("PREFIX", UpdateInfoObject.TargetCluster, UpdateInfoObject.TargetSector, UpdateInfoObject.TargetZone, UpdateInfoObject.TargetGate);
                UpdateInfoObject.SourceGate.DestinationSectorName = UpdateInfoObject.TargetSector.Name;

                // Remove old target gate
                int index = MainForm.Instance.GatesListBox.SelectedIndex;
                MainForm.Instance.GatesListBox.Items.Remove(targetGate);
                // Add new gate
                MainForm.Instance.GatesListBox.Items.Insert(index, UpdateInfoObject.TargetGate);
                MainForm.Instance.GatesListBox.SelectedItem = UpdateInfoObject.TargetGate;
            }

            // Set source gate info
            (GatePosX, GatePosY) = (int.Parse(sourceGatePosMatch.Groups[1].Value), int.Parse(sourceGatePosMatch.Groups[2].Value));

            Gate sourceGate = UpdateInfoObject.SourceGate;
            Zone sourceZone = UpdateInfoObject.SourceZone;

            // Update source gate properties
            // No sector change, just update the target gate properties
            sourceGate.Type = selectedSourceType.Equals("Gate", StringComparison.OrdinalIgnoreCase) ?
                Gate.GateType.props_gates_anc_gate_macro : Gate.GateType.props_gates_orb_accelerator_01_macro;
            sourceGate.Yaw = int.Parse(txtSourceGateYaw.Text);
            sourceGate.Pitch = int.Parse(txtSourceGatePitch.Text);
            sourceGate.Roll = int.Parse(txtSourceGateRoll.Text);
            sourceZone.Position = new Point(GatePosX, GatePosY);

            UpdateInfoObject = null;
            Close();
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

            // Redirect to update method
            if (UpdateInfoObject != null)
            {
                BtnUpdateConnection_Click();
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
                Id = SourceSector.Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
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
                Id = targetSector.Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
                Position = new Point(GatePosX, GatePosY),
                Gates = [targetGate]
            };
            targetSector.Zones.Add(targetZone);
            #endregion

            // SourceGate source / destination
            sourceGate.Source = ConvertToPath(SourceCluster, SourceSector, sourceZone);
            sourceGate.Destination = ConvertToPath(targetCluster, targetSector, targetZone);

            // TargetGate source / destination
            targetGate.Source = sourceGate.Destination;
            targetGate.Destination = sourceGate.Source;

            // Paths must be set at the end
            targetGate.SetSourcePath("PREFIX", targetCluster, targetSector, targetZone);
            sourceGate.SetSourcePath("PREFIX", SourceCluster, SourceSector, sourceZone);
            sourceGate.SetDestinationPath("PREFIX", targetCluster, targetSector, targetZone, targetGate);
            targetGate.SetDestinationPath("PREFIX", SourceCluster, SourceSector, sourceZone, sourceGate);

            // Add target gate to listbox
            sourceGate.DestinationSectorName = targetSector.Name;
            _ = MainForm.Instance.GatesListBox.Items.Add(targetGate);
            MainForm.Instance.GatesListBox.SelectedItem = targetGate;

            Close();
        }

        private static string ConvertToPath(Cluster cluster, Sector sector, Zone zone)
        {
            string value = string.Empty;
            if (cluster.IsBaseGame)
            {
                value += $"{cluster.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "")}";
            }
            else
            {
                value += $"c{cluster.Id:D3}";
            }

            if (sector.IsBaseGame)
            {
                value += $"_{sector.BaseGameMapping.CapitalizeFirstLetter().Replace("_", "")}";
            }
            else
            {
                value += $"_s{sector.Id:D3}";
            }

            value += $"_z{zone.Id:D3}";

            return value;
        }

        private void BtnSelectSector_Click(object sender, EventArgs e)
        {
            MainForm.Instance.SectorMapForm.Value.GateSectorSelection = true;
            MainForm.Instance.SectorMapForm.Value.BtnSelectLocation.Enabled = false;
            MainForm.Instance.SectorMapForm.Value.ControlPanel.Size = new Size(176, 277);
            MainForm.Instance.SectorMapForm.Value.BtnSelectLocation.Show();
            MainForm.Instance.SectorMapForm.Value.Reset();
            MainForm.Instance.SectorMapForm.Value.Show();
        }
    }
}
