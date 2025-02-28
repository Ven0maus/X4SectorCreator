using System.ComponentModel;
using System.Drawing.Drawing2D;
using X4SectorCreator.Objects;

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
        private int _circleRadius = 200;
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
                SectorHexagon.Invalidate();
            }
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

        }

        private void BtnCreateRegion_Click(object sender, EventArgs e)
        {

        }

        private void BtnResourcesAdd_Click(object sender, EventArgs e)
        {
            RegionResourcesForm.Show();
        }

        private void BtnResourcesDel_Click(object sender, EventArgs e)
        {
            if (ListBoxResources.SelectedItem is not RegionResourcesForm.Resource selectedResource) return;
            ListBoxResources.Items.Remove(selectedResource);
            ListBoxResources.SelectedItem = null;
        }

        private void BtnFalloffAdd_Click(object sender, EventArgs e)
        {
            RegionFalloffForm.Show();
        }

        private void BtnFalloffDel_Click(object sender, EventArgs e)
        {
            var listBox = GetActiveFalloffListbox();
            if (listBox.SelectedItem is not RegionFalloffForm.StepObj lateral) return;
            listBox.Items.Remove(lateral);
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

        private void BtnFieldsAdd_Click(object sender, EventArgs e)
        {
            RegionFieldsForm.Show();
        }

        private void BtnFieldsDel_Click(object sender, EventArgs e)
        {

        }
    }
}
