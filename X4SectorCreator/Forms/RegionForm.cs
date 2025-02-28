using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class RegionForm : Form
    {
        private RegionResourcesForm _regionPropertiesForm;
        public RegionResourcesForm RegionPropertiesForm => _regionPropertiesForm != null && !_regionPropertiesForm.IsDisposed
            ? _regionPropertiesForm
            : (_regionPropertiesForm = new RegionResourcesForm());

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
        private int _circleRadius = 250;
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

        private void InitSectorValues()
        {

        }

        private void BtnCreateRegion_Click(object sender, EventArgs e)
        {

        }

        private void BtnUpdateRegionProperties_Click(object sender, EventArgs e)
        {
            RegionPropertiesForm.Show();
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
                Point edgePoint = new Point(
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
    }
}
