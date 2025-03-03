using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using X4SectorCreator.Objects;
using Region = X4SectorCreator.Objects.Region;

namespace X4SectorCreator.Forms
{
    public partial class RegionForm : Form
    {
        private RegionResourcesForm _regionResourcesForm;
        public RegionResourcesForm RegionResourcesForm => _regionResourcesForm != null && !_regionResourcesForm.IsDisposed
            ? _regionResourcesForm
            : (_regionResourcesForm = new RegionResourcesForm());

        private RegionFalloffForm _regionFalloffForm;
        public RegionFalloffForm RegionFalloffForm => _regionFalloffForm != null && !_regionFalloffForm.IsDisposed
            ? _regionFalloffForm
            : (_regionFalloffForm = new RegionFalloffForm());

        private RegionFieldsForm _regionFieldsForm;
        public RegionFieldsForm RegionFieldsForm => _regionFieldsForm != null && !_regionFieldsForm.IsDisposed
            ? _regionFieldsForm
            : (_regionFieldsForm = new RegionFieldsForm());

        private RegionPredefinedFieldsForm _regionPredefinedFieldsForm;
        public RegionPredefinedFieldsForm RegionPredefinedFieldsForm => _regionPredefinedFieldsForm != null && !_regionPredefinedFieldsForm.IsDisposed
            ? _regionPredefinedFieldsForm
            : (_regionPredefinedFieldsForm = new RegionPredefinedFieldsForm());

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Cluster Cluster { get; set; }

        private Sector _sector;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Sector Sector
        {
            get => _sector;
            set
            {
                _sector = value;
                if (_sector != null)
                    InitSectorValues();
            }
        }

        #region Hexagon Data
        private readonly int _hexRadius;
        private readonly PointF[] _hexagonPoints;
        private Point _circlePosition, _lastMousePos;
        private int _circleRadius = 150;
        private bool _dragging = false, _resizing = false;
        #endregion

        public RegionForm()
        {
            InitializeComponent();

            // Create and define hexagon
            _hexagonPoints = new PointF[6];
            _hexRadius = (int)Math.Min(SectorHexagon.Width / 2, SectorHexagon.Height / (float)Math.Sqrt(3));

            SectorHexagon.Paint += SectorHexagon_Paint;
            SectorHexagon.MouseMove += SectorHexagon_MouseMove;
            SectorHexagon.MouseDown += SectorHexagon_MouseDown;
            SectorHexagon.MouseUp += SectorHexagon_MouseUp;
            SectorHexagon.MouseClick += SectorHexagon_MouseClick;

            txtRegionLinear.Enabled = cmbBoundaryType.SelectedItem is string selected &&
                selected.Equals("Cylinder", StringComparison.OrdinalIgnoreCase);

            // Init hexagon
            InitializeHexagon();
            InitDefaultFalloff();
        }

        private void InitDefaultFalloff()
        {
            // Some defaults to make configurating easier
            var lateral = new List<StepObj>
            {
                new() { Position = "0.0", Value = "0.0" },
                new() { Position = "0.1", Value = "1.0" },
                new() { Position = "0.9", Value = "1.0" },
                new() { Position = "1.0", Value = "0.0" }
            };

            var radial = new List<StepObj>
            {
                new() { Position = "0.0", Value = "1.0" },
                new() { Position = "0.8", Value = "1.0" },
                new() { Position = "1.0", Value = "0.0" }
            };

            foreach (var lat in lateral)
            {
                lat.Type = "Lateral";
                ListBoxLateral.Items.Add(lat);
            }

            foreach (var rad in radial)
            {
                rad.Type = "Radial";
                ListBoxRadial.Items.Add(rad);
            }
        }

        #region Hexagon Methods
        private void InitializeHexagon()
        {
            int centerX = SectorHexagon.Width / 2;
            int centerY = SectorHexagon.Height / 2;

            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3 * i;
                _hexagonPoints[i] = new PointF(
                    centerX + (_hexRadius * (float)Math.Cos(angle)),
                    centerY + (_hexRadius * (float)Math.Sin(angle))
                );
            }

            // Both hexagons are shared
            _circlePosition = SectorHexagon.ClientRectangle.Center();

            // Default linear
            if (string.IsNullOrWhiteSpace(txtRegionLinear.Text))
                txtRegionLinear.Text = "5000";
        }

        private void SectorHexagon_Paint(object sender, PaintEventArgs e)
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
            g.FillEllipse(brush, _circlePosition.X - (int)(_circleRadius / 2f), _circlePosition.Y - (int)(_circleRadius / 2f), _circleRadius, _circleRadius);
        }

        private void SectorHexagon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsFullyInsideHexagon(e.Location, _circleRadius))
            {
                _dragging = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _resizing = true;
                _lastMousePos = e.Location;
            }
        }

        private void SectorHexagon_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                if (IsFullyInsideHexagon(e.Location, _circleRadius))
                {
                    _circlePosition = e.Location;
                    UpdateRegionPosition();
                }
                SectorHexagon.Invalidate();
            }
            else if (_resizing)
            {
                // Calculate the radius change based on mouse movement
                int delta = (int)((_lastMousePos.Y - e.Y) / 50f); // Dragging up reduces radius, down increases
                int newRadius = Math.Clamp(_circleRadius + delta, 30, 250);

                // Ensure the circle stays fully inside the hexagon
                if (IsFullyInsideHexagon(_circlePosition, newRadius))
                {
                    _circleRadius = newRadius;
                    UpdateRegionPosition();
                }
                SectorHexagon.Invalidate();
            }
        }

        private void SectorHexagon_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
            _resizing = false;
        }

        private void SectorHexagon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsFullyInsideHexagon(e.Location, _circleRadius))
            {
                _circlePosition = e.Location;
                UpdateRegionPosition();
                SectorHexagon.Invalidate();
            }
        }

        private void UpdateRegionPosition()
        {
            var worldPos = ConvertScreenToWorld(_circlePosition);
            txtRegionPosition.Text = $"({worldPos.X:0}, {worldPos.Y:0})";
            txtRegionRadius.Text = _circleRadius.ToString(); // Don't need to convert so the user can read it better
        }

        private int ConvertScreenRadiusToWorld(int screenRadius)
        {
            return (int)Math.Round((screenRadius * Sector.DiameterRadius) / (2f * _hexRadius));
        }

        private int ConvertWorldRadiusToScreen(int worldRadius)
        {
            return (int)Math.Round((worldRadius * 2f * _hexRadius) / Sector.DiameterRadius);
        }

        private Point ConvertScreenToWorld(Point point)
        {
            int centerX = SectorHexagon.Width / 2;
            int centerY = SectorHexagon.Height / 2;

            float normalizedX = (point.X - centerX) / (float)_hexRadius;
            float normalizedY = -(point.Y - centerY) / (float)_hexRadius;

            float worldX = normalizedX * Sector.DiameterRadius / 2;
            float worldY = normalizedY * Sector.DiameterRadius / 2;

            return new Point((int)Math.Round(worldX), (int)Math.Round(worldY));
        }

        private Point ConvertWorldToScreen(Point coordinate)
        {
            int centerX = SectorHexagon.Width / 2;
            int centerY = SectorHexagon.Height / 2;

            // Reverse world scaling
            float normalizedX = (coordinate.X * 2f) / Sector.DiameterRadius;
            float normalizedY = (coordinate.Y * 2f) / Sector.DiameterRadius;

            // Reverse normalization and centering
            float screenX = (normalizedX * _hexRadius) + centerX;
            float screenY = (-normalizedY * _hexRadius) + centerY; // Correct Y-axis negation

            return new Point((int)Math.Round(screenX), (int)Math.Round(screenY));
        }

        private bool IsPointInsideHexagon(Point point)
        {
            using GraphicsPath path = new();
            path.AddPolygon(_hexagonPoints);
            return path.IsVisible(point);
        }

        private bool IsFullyInsideHexagon(Point center, int radius)
        {
            int steps = 12; // More steps give a better check
            double angleStep = 2 * Math.PI / steps;

            for (int i = 0; i < steps; i++)
            {
                double angle = i * angleStep;
                Point edgePoint = new(
                    center.X + (int)(radius / 2 * Math.Cos(angle)),
                    center.Y + (int)(radius / 2 * Math.Sin(angle))
                );

                if (!IsPointInsideHexagon(edgePoint))
                {
                    return false; // If any point is outside, it's not fully inside
                }
            }

            return true; // If all edge points are inside, it's fully inside
        }
        #endregion

        private void InitSectorValues()
        {
            // Set region positions
            UpdateRegionPosition();
        }

        private void BtnCreateRegion_Click(object sender, EventArgs e)
        {
            if (!ValidationChecks(out var region)) return;

            // Convert region radius and position to world space
            var convertedRegionRadius = ConvertScreenRadiusToWorld(_circleRadius);
            var convertedRegionPosition = ConvertScreenToWorld(_circlePosition);

            // Set converted worlspace coords & radius
            region.BoundaryRadius = convertedRegionRadius.ToString();
            region.Position = convertedRegionPosition;

            // Fields
            region.Fields.AddRange(ListBoxFields.Items.Cast<FieldObj>());

            // Falloff lateral & radial
            region.Falloff.AddRange(ListBoxLateral.Items.Cast<StepObj>());
            region.Falloff.AddRange(ListBoxRadial.Items.Cast<StepObj>());

            // Resources
            region.Resources.AddRange(ListBoxResources.Items.Cast<Resource>());

            // Assign ID
            region.Id = Sector.Regions.DefaultIfEmpty(new Region()).Max(a => a.Id) + 1;

            // Add region to sector
            Sector.Regions.Add(region);

            MainForm.Instance.RegionsListBox.Items.Add(region);
            MainForm.Instance.RegionsListBox.SelectedItem = region;
            Close();
        }

        private bool ValidationChecks(out Region region)
        {
            region = null;

            // Name check
            if (string.IsNullOrWhiteSpace(txtRegionName.Text))
            {
                _ = MessageBox.Show("Please insert a valid name for the region.");
                return false;
            }

            // Boundary check
            var selectedBoundaryType = cmbBoundaryType.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedBoundaryType))
            {
                _ = MessageBox.Show("Please select a valid boundary type for the region.");
                return false;
            }

            // Linear check
            int regionLinear = default;
            if (selectedBoundaryType.Equals("Cylinder", StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrWhiteSpace(txtRegionLinear.Text) ||
                !int.TryParse(txtRegionLinear.Text, out regionLinear) ||
                regionLinear <= 0))
            {
                _ = MessageBox.Show("Region linear must be a valid numeric value higher than 0 for the region.");
                return false;
            }

            var messages = new List<string>();
            IsValidInteger(messages, txtRotation, out var rotation);
            IsValidInteger(messages, txtSeed, out var seed);
            IsValidInteger(messages, txtNoiseScale, out var noiseScale);
            IsValidFloat(messages, txtDensity, out var density);
            IsValidFloat(messages, txtMinNoiseValue, out var minNoiseValue);
            IsValidFloat(messages, txtMaxNoiseValue, out var maxNoiseValue);
            if (messages.Count > 0)
            {
                _ = MessageBox.Show(string.Join("\n", messages));
                return false;
            }

            region = new Region
            {
                BoundaryLinear = regionLinear.ToString(),
                Density = density.ToString(),
                MaxNoiseValue = maxNoiseValue.ToString(),
                MinNoiseValue = minNoiseValue.ToString(),
                Rotation = rotation.ToString(),
                NoiseScale = noiseScale.ToString(),
                Seed = seed.ToString(),
                Name = txtRegionName.Text,
                BoundaryType = selectedBoundaryType.ToLower(),
            };

            return true;
        }

        private static void IsValidFloat(List<string> messages, TextBox textBox, out float value)
        {
            if (!float.TryParse(textBox.Text, out value))
            {
                messages.Add($"{textBox.Name.Replace("txt", string.Empty)} must be a valid numeric value.");
            }
        }

        private static void IsValidInteger(List<string> messages, TextBox textBox, out int value)
        {
            if (!int.TryParse(textBox.Text, out value))
            {
                messages.Add($"{textBox.Name.Replace("txt", string.Empty)} must be a valid numeric integer value.");
            }
        }

        private void BtnResourcesAdd_Click(object sender, EventArgs e)
        {
            RegionResourcesForm.Show();
        }

        private void BtnResourcesDel_Click(object sender, EventArgs e)
        {
            if (ListBoxResources.SelectedItem is not Resource selectedResource) return;

            var index = ListBoxResources.Items.IndexOf(selectedResource);
            ListBoxResources.Items.Remove(selectedResource);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            if (index >= 0 && ListBoxResources.Items.Count > 0)
                ListBoxResources.SelectedItem = ListBoxResources.Items[index];
            else
                ListBoxResources.SelectedItem = null;
        }

        private void BtnFalloffAdd_Click(object sender, EventArgs e)
        {
            RegionFalloffForm.Show();
        }

        private void BtnFalloffDel_Click(object sender, EventArgs e)
        {
            var listBox = GetActiveFalloffListbox();
            if (listBox.SelectedItem is not StepObj lateral) return;

            var index = listBox.Items.IndexOf(lateral);
            listBox.Items.Remove(lateral);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            if (index >= 0 && listBox.Items.Count > 0)
                listBox.SelectedItem = listBox.Items[index];
            else
                listBox.SelectedItem = null;
        }

        private void BtnFalloffUp_Click(object sender, EventArgs e)
        {
            // Determine which tab is active
            var listBox = GetActiveFalloffListbox();
            int index = listBox.SelectedIndex;
            if (index > 0) // Ensure it's not already at the top
            {
                var item = listBox.Items[index];
                listBox.Items.RemoveAt(index);
                listBox.Items.Insert(index - 1, item);
                listBox.SelectedIndex = index - 1; // Keep selection
            }
        }

        private void BtnFalloffDown_Click(object sender, EventArgs e)
        {
            // Determine which tab is active
            var listBox = GetActiveFalloffListbox();
            int index = listBox.SelectedIndex;
            if (index < listBox.Items.Count - 1 && index >= 0) // Ensure it's not already at the bottom
            {
                var item = listBox.Items[index];
                listBox.Items.RemoveAt(index);
                listBox.Items.Insert(index + 1, item);
                listBox.SelectedIndex = index + 1; // Keep selection
            }
        }

        private ListBox GetActiveFalloffListbox()
        {
            // Determine which tab is active
            var sT = TabControlFalloff.SelectedTab;
            return sT.Name switch
            {
                "tabLateral" => ListBoxLateral,
                "tabRadial" => ListBoxRadial,
                _ => throw new NotSupportedException(sT.Name),
            };
        }

        private void BtnFieldsAddCustom_Click(object sender, EventArgs e)
        {
            RegionFieldsForm.Show();
        }

        private void BtnFieldsDel_Click(object sender, EventArgs e)
        {
            if (ListBoxFields.SelectedItem is not FieldObj fieldObj) return;

            var index = ListBoxFields.Items.IndexOf(fieldObj);
            ListBoxFields.Items.Remove(fieldObj);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            if (index >= 0 && ListBoxFields.Items.Count > 0)
                ListBoxFields.SelectedItem = ListBoxFields.Items[index];
            else
                ListBoxFields.SelectedItem = null;
        }

        private void BtnAddPredefined_Click(object sender, EventArgs e)
        {
            RegionPredefinedFieldsForm.Show();
        }

        private void CmbBoundaryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRegionLinear.Enabled = cmbBoundaryType.SelectedItem is string selected &&
                selected.Equals("Cylinder", StringComparison.OrdinalIgnoreCase);
        }

        private void ListBoxFields_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxFields.SelectedItem is not FieldObj selectedField) return;

            RegionFieldsForm.FieldObj = selectedField;
            RegionFieldsForm.Show();
        }

        private void ListBoxResources_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxResources.SelectedItem is not Resource selectedResource) return;

            RegionResourcesForm.Resource = selectedResource;
            RegionResourcesForm.Show();
        }

        private void ListBoxLateral_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxLateral.SelectedItem is not StepObj selectedStep) return;

            RegionFalloffForm.StepObj = selectedStep;
            RegionFalloffForm.Show();
        }

        private void ListBoxRadial_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxRadial.SelectedItem is not StepObj selectedStep) return;

            RegionFalloffForm.StepObj = selectedStep;
            RegionFalloffForm.Show();
        }
    }
}
