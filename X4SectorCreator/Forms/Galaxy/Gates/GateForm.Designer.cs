namespace X4SectorCreator.Forms
{
    partial class GateForm
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
            BtnSelectSector = new Button();
            label1 = new Label();
            txtTargetSector = new TextBox();
            txtTargetSectorLocation = new TextBox();
            label2 = new Label();
            txtSourceGatePosition = new TextBox();
            label3 = new Label();
            label4 = new Label();
            txtSourceGateYaw = new TextBox();
            label5 = new Label();
            label6 = new Label();
            txtSourceGatePitch = new TextBox();
            txtSourceGateRoll = new TextBox();
            BtnCreateConnection = new Button();
            SourceSectorHexagon = new PictureBox();
            TargetSectorHexagon = new PictureBox();
            label7 = new Label();
            label8 = new Label();
            txtTargetGateRoll = new TextBox();
            txtTargetGatePitch = new TextBox();
            label9 = new Label();
            label10 = new Label();
            txtTargetGateYaw = new TextBox();
            label11 = new Label();
            txtTargetGatePosition = new TextBox();
            label12 = new Label();
            label13 = new Label();
            txtSourceSectorLocation = new TextBox();
            txtSourceSector = new TextBox();
            label14 = new Label();
            label15 = new Label();
            label16 = new Label();
            cmbTargetType = new ComboBox();
            cmbSourceType = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)SourceSectorHexagon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TargetSectorHexagon).BeginInit();
            SuspendLayout();
            // 
            // BtnSelectSector
            // 
            BtnSelectSector.Location = new Point(12, 68);
            BtnSelectSector.Name = "BtnSelectSector";
            BtnSelectSector.Size = new Size(705, 29);
            BtnSelectSector.TabIndex = 0;
            BtnSelectSector.Text = "Select Target Sector";
            BtnSelectSector.UseVisualStyleBackColor = true;
            BtnSelectSector.Click += BtnSelectSector_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(370, 8);
            label1.Name = "label1";
            label1.Size = new Size(103, 21);
            label1.TabIndex = 1;
            label1.Text = "Target Sector:";
            // 
            // txtTargetSector
            // 
            txtTargetSector.Enabled = false;
            txtTargetSector.Location = new Point(514, 10);
            txtTargetSector.Name = "txtTargetSector";
            txtTargetSector.Size = new Size(203, 23);
            txtTargetSector.TabIndex = 2;
            txtTargetSector.TextAlign = HorizontalAlignment.Center;
            // 
            // txtTargetSectorLocation
            // 
            txtTargetSectorLocation.Enabled = false;
            txtTargetSectorLocation.Location = new Point(514, 39);
            txtTargetSectorLocation.Name = "txtTargetSectorLocation";
            txtTargetSectorLocation.Size = new Size(203, 23);
            txtTargetSectorLocation.TabIndex = 3;
            txtTargetSectorLocation.TextAlign = HorizontalAlignment.Center;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(11, 461);
            label2.Name = "label2";
            label2.Size = new Size(104, 21);
            label2.TabIndex = 5;
            label2.Text = "Gate Position:";
            // 
            // txtSourceGatePosition
            // 
            txtSourceGatePosition.Enabled = false;
            txtSourceGatePosition.Location = new Point(155, 461);
            txtSourceGatePosition.Name = "txtSourceGatePosition";
            txtSourceGatePosition.Size = new Size(203, 23);
            txtSourceGatePosition.TabIndex = 6;
            txtSourceGatePosition.Text = "(0, 0)";
            txtSourceGatePosition.TextAlign = HorizontalAlignment.Center;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(370, 39);
            label3.Name = "label3";
            label3.Size = new Size(138, 21);
            label3.TabIndex = 7;
            label3.Text = "Sector Coordinate:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(11, 493);
            label4.Name = "label4";
            label4.Size = new Size(77, 21);
            label4.TabIndex = 8;
            label4.Text = "Gate Yaw:";
            // 
            // txtSourceGateYaw
            // 
            txtSourceGateYaw.Location = new Point(155, 493);
            txtSourceGateYaw.Name = "txtSourceGateYaw";
            txtSourceGateYaw.Size = new Size(203, 23);
            txtSourceGateYaw.TabIndex = 9;
            txtSourceGateYaw.Text = "0";
            txtSourceGateYaw.TextAlign = HorizontalAlignment.Center;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(11, 524);
            label5.Name = "label5";
            label5.Size = new Size(83, 21);
            label5.TabIndex = 11;
            label5.Text = "Gate Pitch:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(11, 554);
            label6.Name = "label6";
            label6.Size = new Size(76, 21);
            label6.TabIndex = 12;
            label6.Text = "Gate Roll:";
            // 
            // txtSourceGatePitch
            // 
            txtSourceGatePitch.Location = new Point(155, 526);
            txtSourceGatePitch.Name = "txtSourceGatePitch";
            txtSourceGatePitch.Size = new Size(203, 23);
            txtSourceGatePitch.TabIndex = 13;
            txtSourceGatePitch.Text = "0";
            txtSourceGatePitch.TextAlign = HorizontalAlignment.Center;
            // 
            // txtSourceGateRoll
            // 
            txtSourceGateRoll.Location = new Point(155, 556);
            txtSourceGateRoll.Name = "txtSourceGateRoll";
            txtSourceGateRoll.Size = new Size(203, 23);
            txtSourceGateRoll.TabIndex = 14;
            txtSourceGateRoll.Text = "0";
            txtSourceGateRoll.TextAlign = HorizontalAlignment.Center;
            // 
            // BtnCreateConnection
            // 
            BtnCreateConnection.Location = new Point(12, 585);
            BtnCreateConnection.Name = "BtnCreateConnection";
            BtnCreateConnection.Size = new Size(708, 29);
            BtnCreateConnection.TabIndex = 15;
            BtnCreateConnection.Text = "Create Connection";
            BtnCreateConnection.UseVisualStyleBackColor = true;
            BtnCreateConnection.Click += BtnCreateConnection_Click;
            // 
            // SourceSectorHexagon
            // 
            SourceSectorHexagon.Location = new Point(11, 123);
            SourceSectorHexagon.Name = "SourceSectorHexagon";
            SourceSectorHexagon.Size = new Size(347, 302);
            SourceSectorHexagon.TabIndex = 16;
            SourceSectorHexagon.TabStop = false;
            // 
            // TargetSectorHexagon
            // 
            TargetSectorHexagon.Location = new Point(373, 123);
            TargetSectorHexagon.Name = "TargetSectorHexagon";
            TargetSectorHexagon.Size = new Size(347, 302);
            TargetSectorHexagon.TabIndex = 17;
            TargetSectorHexagon.TabStop = false;
            // 
            // label7
            // 
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(11, 99);
            label7.Name = "label7";
            label7.Size = new Size(347, 21);
            label7.TabIndex = 18;
            label7.Text = "Source Sector";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(373, 99);
            label8.Name = "label8";
            label8.Size = new Size(347, 21);
            label8.TabIndex = 19;
            label8.Text = "Target Sector";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtTargetGateRoll
            // 
            txtTargetGateRoll.Location = new Point(517, 556);
            txtTargetGateRoll.Name = "txtTargetGateRoll";
            txtTargetGateRoll.Size = new Size(203, 23);
            txtTargetGateRoll.TabIndex = 27;
            txtTargetGateRoll.Text = "0";
            txtTargetGateRoll.TextAlign = HorizontalAlignment.Center;
            // 
            // txtTargetGatePitch
            // 
            txtTargetGatePitch.Location = new Point(517, 526);
            txtTargetGatePitch.Name = "txtTargetGatePitch";
            txtTargetGatePitch.Size = new Size(203, 23);
            txtTargetGatePitch.TabIndex = 26;
            txtTargetGatePitch.Text = "0";
            txtTargetGatePitch.TextAlign = HorizontalAlignment.Center;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(373, 554);
            label9.Name = "label9";
            label9.Size = new Size(76, 21);
            label9.TabIndex = 25;
            label9.Text = "Gate Roll:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F);
            label10.Location = new Point(373, 524);
            label10.Name = "label10";
            label10.Size = new Size(83, 21);
            label10.TabIndex = 24;
            label10.Text = "Gate Pitch:";
            // 
            // txtTargetGateYaw
            // 
            txtTargetGateYaw.Location = new Point(517, 493);
            txtTargetGateYaw.Name = "txtTargetGateYaw";
            txtTargetGateYaw.Size = new Size(203, 23);
            txtTargetGateYaw.TabIndex = 23;
            txtTargetGateYaw.Text = "0";
            txtTargetGateYaw.TextAlign = HorizontalAlignment.Center;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(373, 493);
            label11.Name = "label11";
            label11.Size = new Size(77, 21);
            label11.TabIndex = 22;
            label11.Text = "Gate Yaw:";
            // 
            // txtTargetGatePosition
            // 
            txtTargetGatePosition.Enabled = false;
            txtTargetGatePosition.Location = new Point(517, 461);
            txtTargetGatePosition.Name = "txtTargetGatePosition";
            txtTargetGatePosition.Size = new Size(203, 23);
            txtTargetGatePosition.TabIndex = 21;
            txtTargetGatePosition.Text = "(0, 0)";
            txtTargetGatePosition.TextAlign = HorizontalAlignment.Center;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 12F);
            label12.Location = new Point(373, 461);
            label12.Name = "label12";
            label12.Size = new Size(104, 21);
            label12.TabIndex = 20;
            label12.Text = "Gate Position:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 12F);
            label13.Location = new Point(12, 39);
            label13.Name = "label13";
            label13.Size = new Size(138, 21);
            label13.TabIndex = 32;
            label13.Text = "Sector Coordinate:";
            // 
            // txtSourceSectorLocation
            // 
            txtSourceSectorLocation.Enabled = false;
            txtSourceSectorLocation.Location = new Point(156, 39);
            txtSourceSectorLocation.Name = "txtSourceSectorLocation";
            txtSourceSectorLocation.Size = new Size(203, 23);
            txtSourceSectorLocation.TabIndex = 31;
            txtSourceSectorLocation.TextAlign = HorizontalAlignment.Center;
            // 
            // txtSourceSector
            // 
            txtSourceSector.Enabled = false;
            txtSourceSector.Location = new Point(156, 10);
            txtSourceSector.Name = "txtSourceSector";
            txtSourceSector.Size = new Size(203, 23);
            txtSourceSector.TabIndex = 30;
            txtSourceSector.TextAlign = HorizontalAlignment.Center;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 12F);
            label14.Location = new Point(12, 8);
            label14.Name = "label14";
            label14.Size = new Size(109, 21);
            label14.TabIndex = 29;
            label14.Text = "Source Sector:";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 12F);
            label15.Location = new Point(11, 434);
            label15.Name = "label15";
            label15.Size = new Size(81, 21);
            label15.TabIndex = 33;
            label15.Text = "Gate Type:";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 12F);
            label16.Location = new Point(373, 434);
            label16.Name = "label16";
            label16.Size = new Size(81, 21);
            label16.TabIndex = 34;
            label16.Text = "Gate Type:";
            // 
            // cmbTargetType
            // 
            cmbTargetType.FormattingEnabled = true;
            cmbTargetType.Items.AddRange(new object[] { "Gate", "Accelerator" });
            cmbTargetType.Location = new Point(517, 432);
            cmbTargetType.MaxDropDownItems = 2;
            cmbTargetType.Name = "cmbTargetType";
            cmbTargetType.Size = new Size(203, 23);
            cmbTargetType.TabIndex = 35;
            cmbTargetType.Text = "Gate";
            // 
            // cmbSourceType
            // 
            cmbSourceType.FormattingEnabled = true;
            cmbSourceType.Items.AddRange(new object[] { "Gate", "Accelerator" });
            cmbSourceType.Location = new Point(155, 432);
            cmbSourceType.MaxDropDownItems = 2;
            cmbSourceType.Name = "cmbSourceType";
            cmbSourceType.Size = new Size(203, 23);
            cmbSourceType.TabIndex = 36;
            cmbSourceType.Text = "Gate";
            // 
            // GateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(729, 621);
            Controls.Add(cmbSourceType);
            Controls.Add(cmbTargetType);
            Controls.Add(label16);
            Controls.Add(label15);
            Controls.Add(label13);
            Controls.Add(txtSourceSectorLocation);
            Controls.Add(txtSourceSector);
            Controls.Add(label14);
            Controls.Add(txtTargetGateRoll);
            Controls.Add(txtTargetGatePitch);
            Controls.Add(label9);
            Controls.Add(label10);
            Controls.Add(txtTargetGateYaw);
            Controls.Add(label11);
            Controls.Add(txtTargetGatePosition);
            Controls.Add(label12);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(TargetSectorHexagon);
            Controls.Add(SourceSectorHexagon);
            Controls.Add(BtnCreateConnection);
            Controls.Add(txtSourceGateRoll);
            Controls.Add(txtSourceGatePitch);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(txtSourceGateYaw);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtSourceGatePosition);
            Controls.Add(label2);
            Controls.Add(txtTargetSectorLocation);
            Controls.Add(txtTargetSector);
            Controls.Add(label1);
            Controls.Add(BtnSelectSector);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GateForm";
            Text = "Gate Editor";
            ((System.ComponentModel.ISupportInitialize)SourceSectorHexagon).EndInit();
            ((System.ComponentModel.ISupportInitialize)TargetSectorHexagon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnSelectSector;
        private Label label1;
        private Label label2;
        private TextBox txtSourceGatePosition;
        private Label label3;
        private Label label4;
        private TextBox txtSourceGateYaw;
        private Label label5;
        private Label label6;
        private TextBox txtSourceGatePitch;
        private TextBox txtSourceGateRoll;
        private PictureBox SourceSectorHexagon;
        internal TextBox txtTargetSector;
        internal TextBox txtTargetSectorLocation;
        private PictureBox TargetSectorHexagon;
        private Label label7;
        private Label label8;
        private TextBox txtTargetGateRoll;
        private TextBox txtTargetGatePitch;
        private Label label9;
        private Label label10;
        private TextBox txtTargetGateYaw;
        private Label label11;
        private TextBox txtTargetGatePosition;
        private Label label12;
        private Label label13;
        internal TextBox txtSourceSectorLocation;
        internal TextBox txtSourceSector;
        private Label label14;
        private Label label15;
        private Label label16;
        private ComboBox cmbTargetType;
        private ComboBox cmbSourceType;
        internal Button BtnCreateConnection;
    }
}