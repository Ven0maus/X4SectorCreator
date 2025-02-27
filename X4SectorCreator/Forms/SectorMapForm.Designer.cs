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
            ControlPanel.SuspendLayout();
            SuspendLayout();
            // 
            // chkShowX4Sectors
            // 
            chkShowX4Sectors.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkShowX4Sectors.AutoSize = true;
            chkShowX4Sectors.BackColor = Color.Transparent;
            chkShowX4Sectors.Checked = true;
            chkShowX4Sectors.CheckState = CheckState.Checked;
            chkShowX4Sectors.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            chkShowX4Sectors.ForeColor = SystemColors.ButtonFace;
            chkShowX4Sectors.Location = new Point(7, 7);
            chkShowX4Sectors.Name = "chkShowX4Sectors";
            chkShowX4Sectors.Size = new Size(153, 25);
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
            BtnSelectLocation.Location = new Point(7, 103);
            BtnSelectLocation.Name = "BtnSelectLocation";
            BtnSelectLocation.Size = new Size(192, 30);
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
            chkShowCoordinates.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            chkShowCoordinates.ForeColor = SystemColors.ButtonFace;
            chkShowCoordinates.Location = new Point(7, 72);
            chkShowCoordinates.Name = "chkShowCoordinates";
            chkShowCoordinates.Size = new Size(167, 25);
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
            chkShowCustomSectors.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            chkShowCustomSectors.ForeColor = SystemColors.ButtonFace;
            chkShowCustomSectors.Location = new Point(7, 41);
            chkShowCustomSectors.Name = "chkShowCustomSectors";
            chkShowCustomSectors.Size = new Size(192, 25);
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
            ControlPanel.Controls.Add(BtnSelectLocation);
            ControlPanel.Controls.Add(chkShowCustomSectors);
            ControlPanel.Controls.Add(chkShowCoordinates);
            ControlPanel.Controls.Add(chkShowX4Sectors);
            ControlPanel.Location = new Point(484, 12);
            ControlPanel.Name = "ControlPanel";
            ControlPanel.Size = new Size(204, 106);
            ControlPanel.TabIndex = 4;
            // 
            // SectorMapForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 569);
            Controls.Add(ControlPanel);
            MinimizeBox = false;
            Name = "SectorMapForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "X4 Sector Map";
            WindowState = FormWindowState.Maximized;
            ControlPanel.ResumeLayout(false);
            ControlPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private CheckBox chkShowX4Sectors;
        private CheckBox chkShowCoordinates;
        private CheckBox chkShowCustomSectors;
        internal Button BtnSelectLocation;
        internal Panel ControlPanel;
    }
}