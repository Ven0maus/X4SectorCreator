﻿using System.ComponentModel;
using System.Drawing.Drawing2D;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Region = X4SectorCreator.Objects.Region;

namespace X4SectorCreator.Forms
{
    public partial class RegionForm : Form
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
                    InitSectorValues();
                }
            }
        }

        private Region _region;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Region CustomRegion
        {
            get => _region;
            set
            {
                _region = value;
                if (_region != null)
                {
                    _circlePosition = ConvertWorldToScreen(CustomRegion.Position);
                    _circleRadius = ConvertWorldRadiusToScreen(int.Parse(CustomRegion.BoundaryRadius));
                    txtRegionName.Text = CustomRegion.Name;
                    txtRegionLinear.Text = CustomRegion.BoundaryLinear;
                    txtRegionPosition.Text = (CustomRegion.Position.X, CustomRegion.Position.Y).ToString();
                    txtRegionRadius.Text = _circleRadius.ToString();

                    // Just incase it is somehow deleted, we must still display it here
                    if (!ListBoxRegionDefinitions.Items.Contains(CustomRegion.Definition))
                    {
                        _ = ListBoxRegionDefinitions.Items.Add(CustomRegion.Definition);
                    }

                    ListBoxRegionDefinitions.SelectedItem = CustomRegion.Definition;
                    BtnCreateRegion.Text = "Update Region";
                }
            }
        }

        public readonly LazyEvaluated<RegionDefinitionForm> RegionDefinitionForm = new(() => new RegionDefinitionForm(), a => !a.IsDisposed);

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

            txtRegionLinear.Enabled = false;

            // Init hexagon
            InitializeHexagon();
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
            {
                txtRegionLinear.Text = "5000";
            }
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
            Point worldPos = ConvertScreenToWorld(_circlePosition);
            txtRegionPosition.Text = $"({worldPos.X:0}, {worldPos.Y:0})";
            txtRegionRadius.Text = _circleRadius.ToString(); // Don't need to convert so the user can read it better
        }

        private int ConvertScreenRadiusToWorld(int screenRadius)
        {
            return (int)Math.Round(screenRadius * Sector.DiameterRadius / (2f * _hexRadius));
        }

        private int ConvertWorldRadiusToScreen(int worldRadius)
        {
            return (int)Math.Round(worldRadius * 2f * _hexRadius / Sector.DiameterRadius);
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
            float normalizedX = coordinate.X * 2f / Sector.DiameterRadius;
            float normalizedY = coordinate.Y * 2f / Sector.DiameterRadius;

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

            // Init listbox definitions stored by user
            foreach (RegionDefinition definition in Forms.RegionDefinitionForm.RegionDefinitions.OrderBy(a => a.Name))
            {
                _ = ListBoxRegionDefinitions.Items.Add(definition);
            }

            ListBoxRegionDefinitions.SelectedItem = Forms.RegionDefinitionForm.RegionDefinitions.Count > 0 ?
                Forms.RegionDefinitionForm.RegionDefinitions.OrderBy(a => a.Name).First() : null;
        }

        private void BtnCreateRegion_Click(object sender, EventArgs e)
        {
            // Name check
            if (string.IsNullOrWhiteSpace(txtRegionName.Text))
            {
                _ = MessageBox.Show("Please insert a valid name for the region.");
                return;
            }

            if (ListBoxRegionDefinitions.SelectedItem is not RegionDefinition selectedRegionDefinition)
            {
                _ = MessageBox.Show("Please select a valid region definition for this region.");
                return;
            }

            // Linear check
            int regionLinear = default;
            if (selectedRegionDefinition.BoundaryType.Equals("Cylinder", StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrWhiteSpace(txtRegionLinear.Text) ||
                !int.TryParse(txtRegionLinear.Text, out regionLinear) ||
                regionLinear <= 0))
            {
                _ = MessageBox.Show("Region linear must be a valid numeric value higher than 0 for the region.");
                return;
            }

            switch (BtnCreateRegion.Text)
            {
                case "Create Region":
                    Region region = new()
                    {
                        Id = Sector.Regions.DefaultIfEmpty(new Region()).Max(a => a.Id) + 1,
                        Name = txtRegionName.Text,
                        Definition = selectedRegionDefinition,
                        BoundaryLinear = regionLinear.ToString(),
                        BoundaryRadius = ConvertScreenRadiusToWorld(_circleRadius).ToString(),
                        Position = ConvertScreenToWorld(_circlePosition)
                    };

                    // Add region to sector
                    Sector.Regions.Add(region);

                    _ = MainForm.Instance.RegionsListBox.Items.Add(region);
                    MainForm.Instance.RegionsListBox.SelectedItem = region;
                    break;
                case "Update Region":
                    CustomRegion.Name = txtRegionName.Text;
                    CustomRegion.BoundaryLinear = regionLinear.ToString();
                    CustomRegion.BoundaryRadius = ConvertScreenRadiusToWorld(_circleRadius).ToString();
                    CustomRegion.Position = ConvertScreenToWorld(_circlePosition);
                    CustomRegion.Definition = selectedRegionDefinition;

                    int index = MainForm.Instance.RegionsListBox.SelectedIndex;
                    MainForm.Instance.RegionsListBox.Items.Remove(CustomRegion);
                    MainForm.Instance.RegionsListBox.Items.Insert(index, CustomRegion);
                    MainForm.Instance.RegionsListBox.SelectedItem = CustomRegion;
                    break;
            }


            Close();
        }

        private void ListBoxRegionDefinitions_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRegionLinear.Enabled = ListBoxRegionDefinitions.SelectedItem is RegionDefinition selectedRegionDefinition &&
                selectedRegionDefinition.BoundaryType.Equals("Cylinder", StringComparison.OrdinalIgnoreCase);
        }

        private void BtnNewDefinition_Click(object sender, EventArgs e)
        {
            RegionDefinitionForm.Value.InitDefaultFalloff();
            RegionDefinitionForm.Value.Show();
        }

        private void BtnRemoveDefinition_Click(object sender, EventArgs e)
        {
            if (ListBoxRegionDefinitions.SelectedItem is not RegionDefinition selectedRegionDefinition)
            {
                return;
            }

            // Check if regions exist that use this given region definition
            var regionsThatUseThisDefinition = MainForm.Instance.AllClusters.Values
                .SelectMany(cluster => cluster.Sectors, (cluster, sector) => new { cluster, sector })
                .SelectMany(cs => cs.sector.Regions, (cs, region) => new { cs.cluster, cs.sector, region })
                .Where(x => x.region.Definition.Equals(selectedRegionDefinition))
                .Where(x => CustomRegion == null || !x.region.Equals(CustomRegion))
                .ToArray();

            if (regionsThatUseThisDefinition.Length > 0)
            {
                string message = "Following regions use this region definition, they must first be changed or deleted before this definition can be deleted:\n" +
                    string.Join("\n", regionsThatUseThisDefinition
                    .OrderBy(a => a.cluster.Name)
                    .ThenBy(a => a.sector.Name)
                    .Select(x => $"- {x.region.Name} (Cluster: {x.cluster.Name}, Sector: {x.sector.Name})"));

                _ = MessageBox.Show(message);
                return;
            }

            int index = ListBoxRegionDefinitions.Items.IndexOf(selectedRegionDefinition);
            ListBoxRegionDefinitions.Items.Remove(selectedRegionDefinition);
            _ = Forms.RegionDefinitionForm.RegionDefinitions.Remove(selectedRegionDefinition); // remove from static cache

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            ListBoxRegionDefinitions.SelectedItem = index >= 0 && ListBoxRegionDefinitions.Items.Count > 0 ? ListBoxRegionDefinitions.Items[index] : null;
        }

        private void ListBoxRegionDefinitions_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxRegionDefinitions.SelectedItem is not RegionDefinition selectedRegionDefinition)
            {
                return;
            }

            RegionDefinitionForm.Value.RegionDefinition = selectedRegionDefinition;
            RegionDefinitionForm.Value.Show();
        }
    }
}
