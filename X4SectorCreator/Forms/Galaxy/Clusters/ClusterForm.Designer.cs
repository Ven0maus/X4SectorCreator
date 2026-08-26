namespace X4SectorCreator.Forms
{
    partial class ClusterForm
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
            components = new System.ComponentModel.Container();
            BtnCreate = new Button();
            BtnCancel = new Button();
            TxtName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            TxtLocation = new TextBox();
            label3 = new Label();
            txtDescription = new TextBox();
            label4 = new Label();
            cmbBackgroundVisual = new ComboBox();
            ChkAutoPlacement = new CheckBox();
            label5 = new Label();
            label6 = new Label();
            TxtSoundtrack = new TextBox();
            BtnEditClusterXml = new Button();
            label7 = new Label();
            panel1 = new Panel();
            BtnSector4 = new RadioButton();
            BtnSector3 = new RadioButton();
            BtnSector2 = new RadioButton();
            BtnSector1 = new RadioButton();
            panel2 = new Panel();
            CmbSpaceEnvironment = new ComboBox();
            label15 = new Label();
            label13 = new Label();
            label12 = new Label();
            BtnRemoveMoon = new Button();
            BtnCreateMoon = new Button();
            label11 = new Label();
            ListBoxMoons = new ListBox();
            BtnRemovePlanet = new Button();
            BtnCreatePlanet = new Button();
            label10 = new Label();
            ListBoxPlanets = new ListBox();
            BtnRemoveSun = new Button();
            BtnCreateSun = new Button();
            label9 = new Label();
            ListBoxSuns = new ListBox();
            label8 = new Label();
            label14 = new Label();
            Tooltip = new ToolTip(components);
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(167, 452);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(236, 30);
            BtnCreate.TabIndex = 0;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 452);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(149, 30);
            BtnCancel.TabIndex = 1;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // TxtName
            // 
            TxtName.Location = new Point(167, 12);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(236, 23);
            TxtName.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(106, 11);
            label1.Name = "label1";
            label1.Size = new Size(55, 21);
            label1.TabIndex = 3;
            label1.Text = "Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(89, 261);
            label2.Name = "label2";
            label2.Size = new Size(72, 21);
            label2.TabIndex = 4;
            label2.Text = "Location:";
            // 
            // TxtLocation
            // 
            TxtLocation.Location = new Point(167, 261);
            TxtLocation.Name = "TxtLocation";
            TxtLocation.PlaceholderText = "Select..";
            TxtLocation.ReadOnly = true;
            TxtLocation.Size = new Size(236, 23);
            TxtLocation.TabIndex = 6;
            TxtLocation.MouseClick += TxtLocation_MouseClick;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(69, 40);
            label3.Name = "label3";
            label3.Size = new Size(92, 21);
            label3.TabIndex = 8;
            label3.Text = "Description:";
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(167, 41);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(236, 122);
            txtDescription.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(19, 231);
            label4.Name = "label4";
            label4.Size = new Size(142, 21);
            label4.TabIndex = 9;
            label4.Text = "Background Visual:";
            // 
            // cmbBackgroundVisual
            // 
            cmbBackgroundVisual.FormattingEnabled = true;
            cmbBackgroundVisual.Location = new Point(167, 232);
            cmbBackgroundVisual.Name = "cmbBackgroundVisual";
            cmbBackgroundVisual.Size = new Size(236, 23);
            cmbBackgroundVisual.TabIndex = 10;
            // 
            // ChkAutoPlacement
            // 
            ChkAutoPlacement.AutoSize = true;
            ChkAutoPlacement.Checked = true;
            ChkAutoPlacement.CheckState = CheckState.Checked;
            ChkAutoPlacement.Location = new Point(167, 169);
            ChkAutoPlacement.Name = "ChkAutoPlacement";
            ChkAutoPlacement.Size = new Size(242, 19);
            ChkAutoPlacement.TabIndex = 32;
            ChkAutoPlacement.Text = "Determine sector positions automatically";
            ChkAutoPlacement.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 10F);
            label5.Location = new Point(167, 189);
            label5.Name = "label5";
            label5.Size = new Size(236, 43);
            label5.TabIndex = 33;
            label5.Text = "(Sector auto positioning is only used when a cluster has multiple sectors.)";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(69, 291);
            label6.Name = "label6";
            label6.Size = new Size(92, 21);
            label6.TabIndex = 34;
            label6.Text = "Soundtrack:";
            // 
            // TxtSoundtrack
            // 
            TxtSoundtrack.Location = new Point(167, 291);
            TxtSoundtrack.Name = "TxtSoundtrack";
            TxtSoundtrack.PlaceholderText = "Select..";
            TxtSoundtrack.ReadOnly = true;
            TxtSoundtrack.Size = new Size(236, 23);
            TxtSoundtrack.TabIndex = 37;
            TxtSoundtrack.MouseClick += TxtSoundtrack_MouseClick;
            // 
            // BtnEditClusterXml
            // 
            BtnEditClusterXml.Location = new Point(166, 406);
            BtnEditClusterXml.Name = "BtnEditClusterXml";
            BtnEditClusterXml.Size = new Size(236, 40);
            BtnEditClusterXml.TabIndex = 38;
            BtnEditClusterXml.Text = "Edit Cluster Assets XML (Advanced)";
            BtnEditClusterXml.UseVisualStyleBackColor = true;
            BtnEditClusterXml.Click += BtnEditClusterXml_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(19, 324);
            label7.Name = "label7";
            label7.Size = new Size(142, 21);
            label7.TabIndex = 39;
            label7.Text = "Amount of Sectors:";
            // 
            // panel1
            // 
            panel1.Controls.Add(BtnSector4);
            panel1.Controls.Add(BtnSector3);
            panel1.Controls.Add(BtnSector2);
            panel1.Controls.Add(BtnSector1);
            panel1.Location = new Point(167, 320);
            panel1.Name = "panel1";
            panel1.Size = new Size(227, 30);
            panel1.TabIndex = 40;
            // 
            // BtnSector4
            // 
            BtnSector4.AutoSize = true;
            BtnSector4.Location = new Point(115, 6);
            BtnSector4.Name = "BtnSector4";
            BtnSector4.Size = new Size(31, 19);
            BtnSector4.TabIndex = 3;
            BtnSector4.Text = "4";
            BtnSector4.UseVisualStyleBackColor = true;
            // 
            // BtnSector3
            // 
            BtnSector3.AutoSize = true;
            BtnSector3.Location = new Point(80, 6);
            BtnSector3.Name = "BtnSector3";
            BtnSector3.Size = new Size(31, 19);
            BtnSector3.TabIndex = 2;
            BtnSector3.Text = "3";
            BtnSector3.UseVisualStyleBackColor = true;
            // 
            // BtnSector2
            // 
            BtnSector2.AutoSize = true;
            BtnSector2.Location = new Point(44, 6);
            BtnSector2.Name = "BtnSector2";
            BtnSector2.Size = new Size(31, 19);
            BtnSector2.TabIndex = 1;
            BtnSector2.Text = "2";
            BtnSector2.UseVisualStyleBackColor = true;
            // 
            // BtnSector1
            // 
            BtnSector1.AutoSize = true;
            BtnSector1.Checked = true;
            BtnSector1.Location = new Point(8, 6);
            BtnSector1.Name = "BtnSector1";
            BtnSector1.Size = new Size(31, 19);
            BtnSector1.TabIndex = 0;
            BtnSector1.TabStop = true;
            BtnSector1.Text = "1";
            BtnSector1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ControlLight;
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel2.Controls.Add(CmbSpaceEnvironment);
            panel2.Controls.Add(label15);
            panel2.Controls.Add(label13);
            panel2.Controls.Add(label12);
            panel2.Controls.Add(BtnRemoveMoon);
            panel2.Controls.Add(BtnCreateMoon);
            panel2.Controls.Add(label11);
            panel2.Controls.Add(ListBoxMoons);
            panel2.Controls.Add(BtnRemovePlanet);
            panel2.Controls.Add(BtnCreatePlanet);
            panel2.Controls.Add(label10);
            panel2.Controls.Add(ListBoxPlanets);
            panel2.Controls.Add(BtnRemoveSun);
            panel2.Controls.Add(BtnCreateSun);
            panel2.Controls.Add(label9);
            panel2.Controls.Add(ListBoxSuns);
            panel2.Controls.Add(label8);
            panel2.Location = new Point(409, 12);
            panel2.Name = "panel2";
            panel2.Size = new Size(390, 470);
            panel2.TabIndex = 41;
            // 
            // CmbSpaceEnvironment
            // 
            CmbSpaceEnvironment.FormattingEnabled = true;
            CmbSpaceEnvironment.Items.AddRange(new object[] { "Clear Space", "Thin Nebula", "Nebula", "Asteroids", "Protoplanetary Disc", "Heavy Radiation", "Emission Nebula" });
            CmbSpaceEnvironment.Location = new Point(14, 64);
            CmbSpaceEnvironment.Name = "CmbSpaceEnvironment";
            CmbSpaceEnvironment.Size = new Size(253, 23);
            CmbSpaceEnvironment.TabIndex = 57;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 8F);
            label15.Location = new Point(25, 451);
            label15.Name = "label15";
            label15.Size = new Size(242, 13);
            label15.TabIndex = 56;
            label15.Text = "(Moons shown belong to the selected planet)";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 11F);
            label13.Location = new Point(0, 2);
            label13.Name = "label13";
            label13.Size = new Size(77, 20);
            label13.TabIndex = 55;
            label13.Text = "(Optional)";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 12F);
            label12.Location = new Point(14, 40);
            label12.Name = "label12";
            label12.Size = new Size(147, 21);
            label12.TabIndex = 53;
            label12.Text = "Space Environment:";
            // 
            // BtnRemoveMoon
            // 
            BtnRemoveMoon.Location = new Point(273, 383);
            BtnRemoveMoon.Name = "BtnRemoveMoon";
            BtnRemoveMoon.Size = new Size(75, 23);
            BtnRemoveMoon.TabIndex = 52;
            BtnRemoveMoon.Text = "Remove Sun";
            BtnRemoveMoon.UseVisualStyleBackColor = true;
            // 
            // BtnCreateMoon
            // 
            BtnCreateMoon.Location = new Point(273, 354);
            BtnCreateMoon.Name = "BtnCreateMoon";
            BtnCreateMoon.Size = new Size(75, 23);
            BtnCreateMoon.TabIndex = 51;
            BtnCreateMoon.Text = "Create";
            BtnCreateMoon.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(14, 330);
            label11.Name = "label11";
            label11.Size = new Size(61, 21);
            label11.TabIndex = 50;
            label11.Text = "Moons:";
            // 
            // ListBoxMoons
            // 
            ListBoxMoons.FormattingEnabled = true;
            ListBoxMoons.Location = new Point(14, 354);
            ListBoxMoons.Name = "ListBoxMoons";
            ListBoxMoons.Size = new Size(253, 94);
            ListBoxMoons.TabIndex = 49;
            // 
            // BtnRemovePlanet
            // 
            BtnRemovePlanet.Location = new Point(273, 263);
            BtnRemovePlanet.Name = "BtnRemovePlanet";
            BtnRemovePlanet.Size = new Size(75, 23);
            BtnRemovePlanet.TabIndex = 48;
            BtnRemovePlanet.Text = "Remove Sun";
            BtnRemovePlanet.UseVisualStyleBackColor = true;
            // 
            // BtnCreatePlanet
            // 
            BtnCreatePlanet.Location = new Point(273, 234);
            BtnCreatePlanet.Name = "BtnCreatePlanet";
            BtnCreatePlanet.Size = new Size(75, 23);
            BtnCreatePlanet.TabIndex = 47;
            BtnCreatePlanet.Text = "Create";
            BtnCreatePlanet.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F);
            label10.Location = new Point(14, 211);
            label10.Name = "label10";
            label10.Size = new Size(63, 21);
            label10.TabIndex = 46;
            label10.Text = "Planets:";
            // 
            // ListBoxPlanets
            // 
            ListBoxPlanets.FormattingEnabled = true;
            ListBoxPlanets.Location = new Point(14, 234);
            ListBoxPlanets.Name = "ListBoxPlanets";
            ListBoxPlanets.Size = new Size(253, 94);
            ListBoxPlanets.TabIndex = 45;
            // 
            // BtnRemoveSun
            // 
            BtnRemoveSun.Location = new Point(273, 145);
            BtnRemoveSun.Name = "BtnRemoveSun";
            BtnRemoveSun.Size = new Size(75, 23);
            BtnRemoveSun.TabIndex = 44;
            BtnRemoveSun.Text = "Remove Sun";
            BtnRemoveSun.UseVisualStyleBackColor = true;
            // 
            // BtnCreateSun
            // 
            BtnCreateSun.Location = new Point(273, 116);
            BtnCreateSun.Name = "BtnCreateSun";
            BtnCreateSun.Size = new Size(75, 23);
            BtnCreateSun.TabIndex = 43;
            BtnCreateSun.Text = "Create";
            BtnCreateSun.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(14, 92);
            label9.Name = "label9";
            label9.Size = new Size(47, 21);
            label9.TabIndex = 42;
            label9.Text = "Suns:";
            // 
            // ListBoxSuns
            // 
            ListBoxSuns.FormattingEnabled = true;
            ListBoxSuns.Location = new Point(14, 116);
            ListBoxSuns.Name = "ListBoxSuns";
            ListBoxSuns.Size = new Size(253, 94);
            ListBoxSuns.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label8.Location = new Point(83, 0);
            label8.Name = "label8";
            label8.Size = new Size(263, 30);
            label8.TabIndex = 0;
            label8.Text = "Star System Information";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 11F);
            label14.Location = new Point(16, 358);
            label14.Name = "label14";
            label14.Size = new Size(388, 40);
            label14.TabIndex = 56;
            label14.Text = "Note: for a workforce bonus in this cluster you must setup\r\nthe star system information with atleast one planet.";
            label14.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ClusterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(811, 489);
            Controls.Add(label14);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(label7);
            Controls.Add(BtnEditClusterXml);
            Controls.Add(TxtSoundtrack);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(ChkAutoPlacement);
            Controls.Add(cmbBackgroundVisual);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtDescription);
            Controls.Add(TxtLocation);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(TxtName);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ClusterForm";
            Text = "Cluster Editor";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button BtnCancel;
        private Label label1;
        private Label label2;
        internal TextBox TxtLocation;
        internal Button BtnCreate;
        internal TextBox TxtName;
        private Label label3;
        internal TextBox txtDescription;
        private Label label4;
        internal ComboBox cmbBackgroundVisual;
        internal CheckBox ChkAutoPlacement;
        private Label label5;
        private Label label6;
        internal TextBox TxtSoundtrack;
        internal Button BtnEditClusterXml;
        private Label label7;
        private Panel panel1;
        internal RadioButton BtnSector4;
        internal RadioButton BtnSector3;
        internal RadioButton BtnSector2;
        internal RadioButton BtnSector1;
        private Panel panel2;
        private Label label8;
        private Button BtnRemoveMoon;
        private Button BtnCreateMoon;
        private Label label11;
        private ListBox ListBoxMoons;
        private Button BtnRemovePlanet;
        private Button BtnCreatePlanet;
        private Label label10;
        private ListBox ListBoxPlanets;
        private Button BtnRemoveSun;
        private Button BtnCreateSun;
        private Label label9;
        private ListBox ListBoxSuns;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private ToolTip Tooltip;
        private ComboBox CmbSpaceEnvironment;
    }
}