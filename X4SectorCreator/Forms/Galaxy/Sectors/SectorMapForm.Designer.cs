namespace X4SectorCreator
{
    partial class SectorMapForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            chkShowX4Sectors = new CheckBox();
            BtnSelectLocation = new Button();
            chkShowCoordinates = new CheckBox();
            chkShowCustomSectors = new CheckBox();
            ControlPanel = new Panel();
            chkShowConnections = new CheckBox();
            panel1 = new Panel();
            label1 = new Label();
            DlcListBox = new CheckedListBox();
            LegendPanel = new Panel();
            BtnHideLegend = new Button();
            LegendTree = new TreeView();
            label2 = new Label();
            ControlPanel.SuspendLayout();
            panel1.SuspendLayout();
            LegendPanel.SuspendLayout();
            SuspendLayout();
            // 
            // chkShowX4Sectors
            // 
            chkShowX4Sectors.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkShowX4Sectors.AutoSize = true;
            chkShowX4Sectors.BackColor = Color.Transparent;
            chkShowX4Sectors.Checked = true;
            chkShowX4Sectors.CheckState = CheckState.Checked;
            chkShowX4Sectors.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            chkShowX4Sectors.ForeColor = SystemColors.ButtonFace;
            chkShowX4Sectors.Location = new Point(4, 3);
            chkShowX4Sectors.Name = "chkShowX4Sectors";
            chkShowX4Sectors.Size = new Size(138, 23);
            chkShowX4Sectors.TabIndex = 0;
            chkShowX4Sectors.Text = "Show X4 Sectors";
            chkShowX4Sectors.UseVisualStyleBackColor = false;
            chkShowX4Sectors.CheckedChanged += ChkShowX4Sectors_CheckedChanged;
            // 
            // BtnSelectLocation
            // 
            BtnSelectLocation.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnSelectLocation.Enabled = false;
            BtnSelectLocation.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            BtnSelectLocation.Location = new Point(4, 239);
            BtnSelectLocation.Name = "BtnSelectLocation";
            BtnSelectLocation.Size = new Size(167, 30);
            BtnSelectLocation.TabIndex = 1;
            BtnSelectLocation.Text = "Select Location";
            BtnSelectLocation.UseVisualStyleBackColor = true;
            BtnSelectLocation.Click += BtnSelectLocation_Click;
            // 
            // chkShowCoordinates
            // 
            chkShowCoordinates.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkShowCoordinates.AutoSize = true;
            chkShowCoordinates.BackColor = Color.Transparent;
            chkShowCoordinates.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            chkShowCoordinates.ForeColor = SystemColors.ButtonFace;
            chkShowCoordinates.Location = new Point(4, 86);
            chkShowCoordinates.Name = "chkShowCoordinates";
            chkShowCoordinates.Size = new Size(149, 23);
            chkShowCoordinates.TabIndex = 2;
            chkShowCoordinates.Text = "Show Coordinates";
            chkShowCoordinates.UseVisualStyleBackColor = false;
            chkShowCoordinates.CheckedChanged += ChkShowCoordinates_CheckedChanged;
            // 
            // chkShowCustomSectors
            // 
            chkShowCustomSectors.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkShowCustomSectors.AutoSize = true;
            chkShowCustomSectors.BackColor = Color.Transparent;
            chkShowCustomSectors.Checked = true;
            chkShowCustomSectors.CheckState = CheckState.Checked;
            chkShowCustomSectors.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            chkShowCustomSectors.ForeColor = SystemColors.ButtonFace;
            chkShowCustomSectors.Location = new Point(4, 32);
            chkShowCustomSectors.Name = "chkShowCustomSectors";
            chkShowCustomSectors.Size = new Size(171, 23);
            chkShowCustomSectors.TabIndex = 3;
            chkShowCustomSectors.Text = "Show Custom Sectors";
            chkShowCustomSectors.UseVisualStyleBackColor = false;
            chkShowCustomSectors.CheckedChanged += ChkShowCustomSectors_CheckedChanged;
            // 
            // ControlPanel
            // 
            ControlPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ControlPanel.BackColor = Color.Black;
            ControlPanel.BorderStyle = BorderStyle.FixedSingle;
            ControlPanel.Controls.Add(chkShowConnections);
            ControlPanel.Controls.Add(chkShowCoordinates);
            ControlPanel.Controls.Add(panel1);
            ControlPanel.Controls.Add(DlcListBox);
            ControlPanel.Controls.Add(BtnSelectLocation);
            ControlPanel.Controls.Add(chkShowCustomSectors);
            ControlPanel.Controls.Add(chkShowX4Sectors);
            ControlPanel.Location = new Point(795, 12);
            ControlPanel.Name = "ControlPanel";
            ControlPanel.Size = new Size(176, 277);
            ControlPanel.TabIndex = 4;
            // 
            // chkShowConnections
            // 
            chkShowConnections.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkShowConnections.AutoSize = true;
            chkShowConnections.BackColor = Color.Transparent;
            chkShowConnections.Checked = true;
            chkShowConnections.CheckState = CheckState.Checked;
            chkShowConnections.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            chkShowConnections.ForeColor = SystemColors.ButtonFace;
            chkShowConnections.Location = new Point(4, 59);
            chkShowConnections.Name = "chkShowConnections";
            chkShowConnections.Size = new Size(149, 23);
            chkShowConnections.TabIndex = 6;
            chkShowConnections.Text = "Show Connections";
            chkShowConnections.UseVisualStyleBackColor = false;
            chkShowConnections.CheckedChanged += ChkShowConnections_CheckedChanged;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel1.BackColor = Color.DimGray;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(label1);
            panel1.Location = new Point(-1, 113);
            panel1.Name = "panel1";
            panel1.Size = new Size(176, 20);
            panel1.TabIndex = 5;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(-1, -2);
            label1.Name = "label1";
            label1.Size = new Size(176, 23);
            label1.TabIndex = 0;
            label1.Text = "Game DLCs";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // DlcListBox
            // 
            DlcListBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DlcListBox.BackColor = SystemColors.MenuText;
            DlcListBox.CheckOnClick = true;
            DlcListBox.ForeColor = Color.White;
            DlcListBox.FormattingEnabled = true;
            DlcListBox.Location = new Point(4, 139);
            DlcListBox.Name = "DlcListBox";
            DlcListBox.ScrollAlwaysVisible = true;
            DlcListBox.Size = new Size(167, 94);
            DlcListBox.TabIndex = 4;
            DlcListBox.ItemCheck += DlcListBox_ItemCheck;
            // 
            // LegendPanel
            // 
            LegendPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LegendPanel.BackColor = Color.Black;
            LegendPanel.BorderStyle = BorderStyle.FixedSingle;
            LegendPanel.Controls.Add(BtnHideLegend);
            LegendPanel.Controls.Add(LegendTree);
            LegendPanel.Controls.Add(label2);
            LegendPanel.Location = new Point(711, 339);
            LegendPanel.Name = "LegendPanel";
            LegendPanel.Size = new Size(265, 298);
            LegendPanel.TabIndex = 5;
            // 
            // BtnHideLegend
            // 
            BtnHideLegend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnHideLegend.BackColor = Color.Black;
            BtnHideLegend.FlatStyle = FlatStyle.Flat;
            BtnHideLegend.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Pixel);
            BtnHideLegend.ForeColor = Color.White;
            BtnHideLegend.Location = new Point(232, 3);
            BtnHideLegend.Name = "BtnHideLegend";
            BtnHideLegend.Size = new Size(26, 26);
            BtnHideLegend.TabIndex = 7;
            BtnHideLegend.Text = "V";
            BtnHideLegend.UseVisualStyleBackColor = false;
            BtnHideLegend.Click += BtnHideLegend_Click;
            // 
            // LegendTree
            // 
            LegendTree.Anchor = AnchorStyles.None;
            LegendTree.BackColor = Color.Black;
            LegendTree.BorderStyle = BorderStyle.FixedSingle;
            LegendTree.DrawMode = TreeViewDrawMode.OwnerDrawText;
            LegendTree.Font = new Font("Segoe UI", 11F);
            LegendTree.ForeColor = Color.White;
            LegendTree.Location = new Point(3, 33);
            LegendTree.Name = "LegendTree";
            LegendTree.Size = new Size(257, 258);
            LegendTree.TabIndex = 1;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.CausesValidation = false;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold | FontStyle.Underline);
            label2.ForeColor = Color.White;
            label2.Location = new Point(45, 3);
            label2.Name = "label2";
            label2.Size = new Size(181, 27);
            label2.TabIndex = 0;
            label2.Text = "Map Legend";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // SectorMapForm
            // 
            ClientSize = new Size(983, 643);
            Controls.Add(LegendPanel);
            Controls.Add(ControlPanel);
            MinimizeBox = false;
            Name = "SectorMapForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "X4 Sector Map";
            WindowState = FormWindowState.Maximized;
            ControlPanel.ResumeLayout(false);
            ControlPanel.PerformLayout();
            panel1.ResumeLayout(false);
            LegendPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private CheckBox chkShowCoordinates;
        private CheckBox chkShowCustomSectors;
        internal Button BtnSelectLocation;
        internal Panel ControlPanel;
        internal Panel panel1;
        private Label label1;
        internal CheckBox chkShowX4Sectors;
        internal CheckedListBox DlcListBox;
        private CheckBox chkShowConnections;
        private Panel LegendPanel;
        private Label label2;
        private TreeView LegendTree;
        internal Button BtnHideLegend;
    }
}