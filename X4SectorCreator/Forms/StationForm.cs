﻿using System.ComponentModel;
using System.Drawing.Drawing2D;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class StationForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Cluster Cluster { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Sector Sector { get; set; }

        private Station _station;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Station Station
        {
            get => _station;
            set
            {
                _station = value;
                if (_station != null)
                {
                    cmbStationType.SelectedItem = _station.Type;
                    cmbFaction.SelectedItem = _station.Faction;
                    cmbOwner.SelectedItem = _station.Owner;
                    txtPosition.Text = (_station.Position.X, _station.Position.Y).ToString();
                    _dotPosition = ConvertWorldToScreen(_station.Position);
                    txtSector.Text = Sector.Name.ToString();
                    txtName.Text = _station.Name.ToString();
                    cmbRace.SelectedItem = _station.Race;
                    BtnCreate.Text = "Update";
                }
                else
                {
                    cmbStationType.SelectedItem = null;
                    cmbFaction.SelectedItem = null;
                    cmbOwner.SelectedItem = null;
                    cmbRace.SelectedItem = null;
                    txtPosition.Text = "(0, 0)";
                    txtSector.Text = Sector.Name.ToString();
                    _dotPosition = SectorHexagon.ClientRectangle.Center();
                    txtName.ResetText();
                    BtnCreate.Text = "Create";
                }
            }
        }

        private readonly HashSet<string> _races = new(StringComparer.OrdinalIgnoreCase)
        {
            "argon",
            "split",
            "teladi",
            "terran",
            "boron",
            "paranid",
            "xenon"
        };

        #region Hexagon Data
        private readonly int _hexRadius;
        private readonly PointF[] _hexagonPoints;
        private Point _dotPosition;
        private bool _dragging = false;
        #endregion

        public StationForm()
        {
            InitializeComponent();

            // Init factions and races
            foreach (var faction in MainForm.Instance.FactionColorMapping
                .Where(a => !a.Key.Equals("None", StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.Key))
            {
                cmbFaction.Items.Add(faction.Key);
                cmbOwner.Items.Add(faction.Key);
            }
            foreach (var race in _races.OrderBy(a => a))
                cmbRace.Items.Add(race);

            // Create and define hexagon
            _hexagonPoints = new PointF[6];
            _hexRadius = (int)Math.Min(SectorHexagon.Width / 2, SectorHexagon.Height / (float)Math.Sqrt(3));

            SectorHexagon.Paint += SectorHexagon_Paint;
            SectorHexagon.MouseMove += SectorHexagon_MouseMove;
            SectorHexagon.MouseDown += SectorHexagon_MouseDown;
            SectorHexagon.MouseUp += SectorHexagon_MouseUp;
            SectorHexagon.MouseClick += SectorHexagon_MouseClick;

            InitializeHexagon();
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            var name = txtName.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                _ = MessageBox.Show("Station name cannot be empty.");
                return;
            }

            if (Sector.Zones.SelectMany(a => a.Stations).Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && a != Station))
            {
                _ = MessageBox.Show("The selected station name must be unique in the sector the station belongs to, please try a name that is not yet used in the selected sector.");
                return;
            }

            if (cmbStationType.SelectedItem == null)
            {
                _ = MessageBox.Show("Please select a valid station type.");
                return;
            }

            if (cmbFaction.SelectedItem == null)
            {
                _ = MessageBox.Show("Please select a valid faction for which to take the station blueprint.");
                return;
            }

            if (cmbOwner.SelectedItem == null)
            {
                _ = MessageBox.Show("Please select a valid owner.");
                return;
            }

            if (cmbRace.SelectedItem == null)
            {
                _ = MessageBox.Show("Please select a valid race.");
                return;
            }

            System.Text.RegularExpressions.Match stationPosition = RegexHelper.TupleLocationRegex().Match(txtPosition.Text);
            if (!stationPosition.Success)
            {
                _ = MessageBox.Show("Unable to parse station position.");
                return;
            }

            // Station Position
            (int StationPosX, int StationPosY) = (int.Parse(stationPosition.Groups[1].Value), int.Parse(stationPosition.Groups[2].Value));

            switch (BtnCreate.Text)
            {
                case "Create":
                    var station = new Station
                    {
                        Id = Sector.Zones.SelectMany(a => a.Stations).DefaultIfEmpty(new Station()).Max(a => a.Id) + 1,
                        Name = txtName.Text,
                        Faction = cmbFaction.SelectedItem as string,
                        Owner = cmbOwner.SelectedItem as string,
                        Type = cmbStationType.SelectedItem as string,
                        Race = cmbRace.SelectedItem as string,
                        Position = new Point(StationPosX, StationPosY)
                    };
                    var zone = new Zone
                    {
                        Id = Sector.Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
                        Position = new Point(StationPosX, StationPosY),
                    };
                    zone.Stations.Add(station);
                    Sector.Zones.Add(zone);
                    MainForm.Instance.ListStations.Items.Add(station);
                    break;

                case "Update":
                    Station.Name = txtName.Text;
                    Station.Faction = cmbFaction.SelectedItem as string;
                    Station.Owner = cmbOwner.SelectedItem as string;
                    Station.Type = cmbStationType.SelectedItem as string;
                    Station.Race = cmbRace.SelectedItem as string;
                    Station.Position = new Point(StationPosX, StationPosY);

                    var existingZone = Sector.Zones.First(a => a.Stations.Contains(Station));
                    existingZone.Position = Station.Position;
                    break;
            }

            MainForm.Instance.SetDetailsText(Cluster, Sector);
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
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

            _dotPosition = SectorHexagon.ClientRectangle.Center();
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
            g.FillEllipse(brush, _dotPosition.X - 5, _dotPosition.Y - 5, 10, 10);
        }

        private void SectorHexagon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsPointInsideHexagon(e.Location))
            {
                _dragging = true;
            }
        }

        private void SectorHexagon_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                if (IsPointInsideHexagon(e.Location))
                {
                    _dotPosition = e.Location;
                    UpdateStationPosition();
                }
                SectorHexagon.Invalidate();
            }
        }

        private void SectorHexagon_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void SectorHexagon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsPointInsideHexagon(e.Location))
            {
                _dotPosition = e.Location;
                UpdateStationPosition();
                SectorHexagon.Invalidate();
            }
        }

        private void UpdateStationPosition()
        {
            Point worldPos = ConvertScreenToWorld(_dotPosition);
            txtPosition.Text = $"({worldPos.X:0}, {worldPos.Y:0})";
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
        #endregion
    }
}
