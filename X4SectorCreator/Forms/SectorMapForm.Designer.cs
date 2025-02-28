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
            DlcListBox = new CheckedListBox();
            panel1 = new Panel();
            label1 = new Label();
            ControlPanel.SuspendLayout();
            panel1.SuspendLayout();
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
            BtnSelectLocation.Location = new Point(4, 214);
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
            chkShowCoordinates.Location = new Point(4, 61);
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
            ControlPanel.Controls.Add(DlcListBox);
            ControlPanel.Controls.Add(BtnSelectLocation);
            ControlPanel.Controls.Add(chkShowCustomSectors);
            ControlPanel.Controls.Add(chkShowCoordinates);
            ControlPanel.Controls.Add(chkShowX4Sectors);
            ControlPanel.Location = new Point(512, 12);
            ControlPanel.Name = "ControlPanel";
            ControlPanel.Size = new Size(176, 250);
            ControlPanel.TabIndex = 4;
            // 
            // DlcListBox
            // 
            DlcListBox.BackColor = SystemColors.MenuText;
            DlcListBox.CheckOnClick = true;
            DlcListBox.ForeColor = Color.White;
            DlcListBox.FormattingEnabled = true;
            DlcListBox.Location = new Point(4, 116);
            DlcListBox.Name = "DlcListBox";
            DlcListBox.ScrollAlwaysVisible = true;
            DlcListBox.Size = new Size(167, 94);
            DlcListBox.TabIndex = 4;
            DlcListBox.ItemCheck += DlcListBox_ItemCheck;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel1.BackColor = Color.DimGray;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(label1);
            panel1.Location = new Point(512, 103);
            panel1.Name = "panel1";
            panel1.Size = new Size(176, 20);
            panel1.TabIndex = 5;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(-1, -2);
            label1.Name = "label1";
            label1.Size = new Size(180, 23);
            label1.TabIndex = 0;
            label1.Text = "Game DLCs";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // SectorMapForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 569);
            Controls.Add(panel1);
            Controls.Add(ControlPanel);
            MinimizeBox = false;
            Name = "SectorMapForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "X4 Sector Map";
            WindowState = FormWindowState.Maximized;
            ControlPanel.ResumeLayout(false);
            ControlPanel.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private CheckBox chkShowX4Sectors;
        private CheckBox chkShowCoordinates;
        private CheckBox chkShowCustomSectors;
        internal Button BtnSelectLocation;
        internal Panel ControlPanel;
        internal Panel panel1;
        private Label label1;
        private CheckedListBox DlcListBox;
    }
}