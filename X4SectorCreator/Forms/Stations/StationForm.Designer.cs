namespace X4SectorCreator.Forms
{
    partial class StationForm
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
            BtnCreate = new Button();
            cmbStationType = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            cmbFaction = new ComboBox();
            BtnCancel = new Button();
            SectorHexagon = new PictureBox();
            txtPosition = new TextBox();
            label3 = new Label();
            label4 = new Label();
            txtSector = new TextBox();
            label5 = new Label();
            txtName = new TextBox();
            label6 = new Label();
            cmbRace = new ComboBox();
            label7 = new Label();
            cmbOwner = new ComboBox();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            CmbConstructionPlan = new ComboBox();
            BtnViewConstructionPlans = new Button();
            label11 = new Label();
            label12 = new Label();
            ((System.ComponentModel.ISupportInitialize)SectorHexagon).BeginInit();
            SuspendLayout();
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(112, 285);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(158, 33);
            BtnCreate.TabIndex = 0;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // cmbStationType
            // 
            cmbStationType.FormattingEnabled = true;
            cmbStationType.Items.AddRange(new object[] { "factory", "defence", "equipmentdock", "shipyard", "tradestation", "wharf", "piratebase", "piratedock", "freeport" });
            cmbStationType.Location = new Point(112, 67);
            cmbStationType.Name = "cmbStationType";
            cmbStationType.Size = new Size(158, 23);
            cmbStationType.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 67);
            label1.Name = "label1";
            label1.Size = new Size(97, 21);
            label1.TabIndex = 2;
            label1.Text = "Station Type:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(47, 96);
            label2.Name = "label2";
            label2.Size = new Size(62, 21);
            label2.TabIndex = 4;
            label2.Text = "Faction:";
            // 
            // cmbFaction
            // 
            cmbFaction.FormattingEnabled = true;
            cmbFaction.Location = new Point(112, 96);
            cmbFaction.Name = "cmbFaction";
            cmbFaction.Size = new Size(158, 23);
            cmbFaction.TabIndex = 3;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 285);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(97, 33);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // SectorHexagon
            // 
            SectorHexagon.Location = new Point(276, 18);
            SectorHexagon.Name = "SectorHexagon";
            SectorHexagon.Size = new Size(300, 300);
            SectorHexagon.TabIndex = 6;
            SectorHexagon.TabStop = false;
            // 
            // txtPosition
            // 
            txtPosition.Location = new Point(112, 227);
            txtPosition.Name = "txtPosition";
            txtPosition.ReadOnly = true;
            txtPosition.Size = new Size(158, 23);
            txtPosition.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(41, 227);
            label3.Name = "label3";
            label3.Size = new Size(68, 21);
            label3.TabIndex = 8;
            label3.Text = "Position:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(52, 259);
            label4.Name = "label4";
            label4.Size = new Size(57, 21);
            label4.TabIndex = 10;
            label4.Text = "Sector:";
            // 
            // txtSector
            // 
            txtSector.Location = new Point(112, 259);
            txtSector.Name = "txtSector";
            txtSector.ReadOnly = true;
            txtSector.Size = new Size(158, 23);
            txtSector.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(54, 36);
            label5.Name = "label5";
            label5.Size = new Size(55, 21);
            label5.TabIndex = 12;
            label5.Text = "Name:";
            // 
            // txtName
            // 
            txtName.Location = new Point(112, 38);
            txtName.Name = "txtName";
            txtName.Size = new Size(158, 23);
            txtName.TabIndex = 11;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(63, 198);
            label6.Name = "label6";
            label6.Size = new Size(46, 21);
            label6.TabIndex = 14;
            label6.Text = "Race:";
            // 
            // cmbRace
            // 
            cmbRace.FormattingEnabled = true;
            cmbRace.Location = new Point(112, 198);
            cmbRace.Name = "cmbRace";
            cmbRace.Size = new Size(158, 23);
            cmbRace.TabIndex = 13;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(47, 146);
            label7.Name = "label7";
            label7.Size = new Size(60, 21);
            label7.TabIndex = 16;
            label7.Text = "Owner:";
            // 
            // cmbOwner
            // 
            cmbOwner.FormattingEnabled = true;
            cmbOwner.Location = new Point(112, 146);
            cmbOwner.Name = "cmbOwner";
            cmbOwner.Size = new Size(158, 23);
            cmbOwner.TabIndex = 15;
            // 
            // label8
            // 
            label8.Location = new Point(65, 122);
            label8.Name = "label8";
            label8.Size = new Size(205, 21);
            label8.TabIndex = 17;
            label8.Text = "(The faction for the station blueprint)";
            // 
            // label9
            // 
            label9.Location = new Point(87, 172);
            label9.Name = "label9";
            label9.Size = new Size(183, 19);
            label9.TabIndex = 18;
            label9.Text = "(The actual owner of the station)";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F);
            label10.Location = new Point(276, 321);
            label10.Name = "label10";
            label10.Size = new Size(220, 21);
            label10.TabIndex = 19;
            label10.Text = "Use Custom Constructionplan:";
            // 
            // CmbConstructionPlan
            // 
            CmbConstructionPlan.FormattingEnabled = true;
            CmbConstructionPlan.Location = new Point(276, 346);
            CmbConstructionPlan.Name = "CmbConstructionPlan";
            CmbConstructionPlan.Size = new Size(294, 23);
            CmbConstructionPlan.TabIndex = 20;
            CmbConstructionPlan.Text = "None";
            CmbConstructionPlan.SelectedIndexChanged += CmbConstructionPlan_SelectedIndexChanged;
            // 
            // BtnViewConstructionPlans
            // 
            BtnViewConstructionPlans.Location = new Point(12, 328);
            BtnViewConstructionPlans.Name = "BtnViewConstructionPlans";
            BtnViewConstructionPlans.Size = new Size(258, 55);
            BtnViewConstructionPlans.TabIndex = 21;
            BtnViewConstructionPlans.Text = "View Custom Construction Plans";
            BtnViewConstructionPlans.UseVisualStyleBackColor = true;
            BtnViewConstructionPlans.Click += BtnViewConstructionPlans_Click;
            // 
            // label11
            // 
            label11.Location = new Point(276, 372);
            label11.Name = "label11";
            label11.Size = new Size(294, 19);
            label11.TabIndex = 22;
            label11.Text = "(leave as 'None' to use vanilla stations)";
            label11.TextAlign = ContentAlignment.TopCenter;
            // 
            // label12
            // 
            label12.Location = new Point(3, 3);
            label12.Name = "label12";
            label12.Size = new Size(267, 32);
            label12.TabIndex = 23;
            label12.Text = "(Note: If you don't see the station in game, check if the faction actually has this type of blueprint.)";
            // 
            // StationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 391);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(BtnViewConstructionPlans);
            Controls.Add(CmbConstructionPlan);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(cmbOwner);
            Controls.Add(label6);
            Controls.Add(cmbRace);
            Controls.Add(label5);
            Controls.Add(txtName);
            Controls.Add(label4);
            Controls.Add(txtSector);
            Controls.Add(label3);
            Controls.Add(txtPosition);
            Controls.Add(SectorHexagon);
            Controls.Add(BtnCancel);
            Controls.Add(label2);
            Controls.Add(cmbFaction);
            Controls.Add(label1);
            Controls.Add(cmbStationType);
            Controls.Add(BtnCreate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "StationForm";
            Text = "Station Editor";
            Load += StationForm_Load;
            ((System.ComponentModel.ISupportInitialize)SectorHexagon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnCreate;
        private ComboBox cmbStationType;
        private Label label1;
        private Label label2;
        private ComboBox cmbFaction;
        private Button BtnCancel;
        private PictureBox SectorHexagon;
        private TextBox txtPosition;
        private Label label3;
        private Label label4;
        private TextBox txtSector;
        private Label label5;
        private TextBox txtName;
        private Label label6;
        private ComboBox cmbRace;
        private Label label7;
        private ComboBox cmbOwner;
        private Label label8;
        private Label label9;
        private Label label10;
        private Button BtnViewConstructionPlans;
        private Label label11;
        internal ComboBox CmbConstructionPlan;
        private Label label12;
    }
}