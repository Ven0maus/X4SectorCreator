namespace X4SectorCreator.Forms
{
    partial class RegionForm
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
            label15 = new Label();
            txtRegionLinear = new TextBox();
            label8 = new Label();
            txtRegionRadius = new TextBox();
            label7 = new Label();
            txtRegionPosition = new TextBox();
            label5 = new Label();
            label14 = new Label();
            txtRegionName = new TextBox();
            SectorHexagon = new PictureBox();
            BtnCreateRegion = new Button();
            label2 = new Label();
            label3 = new Label();
            CmbRegionDefinition = new ComboBox();
            BtnEditRegionDefinitions = new Button();
            ((System.ComponentModel.ISupportInitialize)SectorHexagon).BeginInit();
            SuspendLayout();
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 12F);
            label15.Location = new Point(10, 63);
            label15.Name = "label15";
            label15.Size = new Size(109, 21);
            label15.TabIndex = 91;
            label15.Text = "Region Linear:";
            // 
            // txtRegionLinear
            // 
            txtRegionLinear.Enabled = false;
            txtRegionLinear.Location = new Point(10, 87);
            txtRegionLinear.Name = "txtRegionLinear";
            txtRegionLinear.Size = new Size(230, 23);
            txtRegionLinear.TabIndex = 90;
            txtRegionLinear.TextAlign = HorizontalAlignment.Center;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(10, 164);
            label8.Name = "label8";
            label8.Size = new Size(113, 21);
            label8.TabIndex = 89;
            label8.Text = "Region Radius:";
            // 
            // txtRegionRadius
            // 
            txtRegionRadius.Location = new Point(10, 188);
            txtRegionRadius.Name = "txtRegionRadius";
            txtRegionRadius.ReadOnly = true;
            txtRegionRadius.Size = new Size(230, 23);
            txtRegionRadius.TabIndex = 88;
            txtRegionRadius.TextAlign = HorizontalAlignment.Center;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(10, 113);
            label7.Name = "label7";
            label7.Size = new Size(121, 21);
            label7.TabIndex = 87;
            label7.Text = "Region Position:";
            // 
            // txtRegionPosition
            // 
            txtRegionPosition.Location = new Point(10, 137);
            txtRegionPosition.Name = "txtRegionPosition";
            txtRegionPosition.ReadOnly = true;
            txtRegionPosition.Size = new Size(230, 23);
            txtRegionPosition.TabIndex = 86;
            txtRegionPosition.TextAlign = HorizontalAlignment.Center;
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 15F);
            label5.Location = new Point(250, 6);
            label5.Name = "label5";
            label5.Size = new Size(300, 28);
            label5.TabIndex = 85;
            label5.Text = "Sector";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 12F);
            label14.Location = new Point(10, 13);
            label14.Name = "label14";
            label14.Size = new Size(105, 21);
            label14.TabIndex = 84;
            label14.Text = "Region name:";
            // 
            // txtRegionName
            // 
            txtRegionName.Location = new Point(10, 37);
            txtRegionName.Name = "txtRegionName";
            txtRegionName.Size = new Size(230, 23);
            txtRegionName.TabIndex = 83;
            // 
            // SectorHexagon
            // 
            SectorHexagon.Location = new Point(250, 37);
            SectorHexagon.Name = "SectorHexagon";
            SectorHexagon.Size = new Size(300, 300);
            SectorHexagon.TabIndex = 82;
            SectorHexagon.TabStop = false;
            // 
            // BtnCreateRegion
            // 
            BtnCreateRegion.Location = new Point(248, 343);
            BtnCreateRegion.Name = "BtnCreateRegion";
            BtnCreateRegion.Size = new Size(304, 43);
            BtnCreateRegion.TabIndex = 96;
            BtnCreateRegion.Text = "Create Region";
            BtnCreateRegion.UseVisualStyleBackColor = true;
            BtnCreateRegion.Click += BtnCreateRegion_Click;
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 8F);
            label2.Location = new Point(10, 214);
            label2.Name = "label2";
            label2.Size = new Size(230, 27);
            label2.TabIndex = 97;
            label2.Text = "(Hold right-click and drag the region to resize the radius.)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(10, 246);
            label3.Name = "label3";
            label3.Size = new Size(134, 21);
            label3.TabIndex = 98;
            label3.Text = "Region Definition:";
            // 
            // CmbRegionDefinition
            // 
            CmbRegionDefinition.FormattingEnabled = true;
            CmbRegionDefinition.Location = new Point(12, 270);
            CmbRegionDefinition.Name = "CmbRegionDefinition";
            CmbRegionDefinition.Size = new Size(228, 23);
            CmbRegionDefinition.TabIndex = 99;
            CmbRegionDefinition.SelectedIndexChanged += CmbRegionDefinition_SelectedIndexChanged;
            // 
            // BtnEditRegionDefinitions
            // 
            BtnEditRegionDefinitions.Location = new Point(14, 343);
            BtnEditRegionDefinitions.Name = "BtnEditRegionDefinitions";
            BtnEditRegionDefinitions.Size = new Size(228, 43);
            BtnEditRegionDefinitions.TabIndex = 100;
            BtnEditRegionDefinitions.Text = "Edit Region Definitions";
            BtnEditRegionDefinitions.UseVisualStyleBackColor = true;
            BtnEditRegionDefinitions.Click += BtnEditRegionDefinitions_Click;
            // 
            // RegionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(556, 391);
            Controls.Add(BtnEditRegionDefinitions);
            Controls.Add(CmbRegionDefinition);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(BtnCreateRegion);
            Controls.Add(label15);
            Controls.Add(txtRegionLinear);
            Controls.Add(label8);
            Controls.Add(txtRegionRadius);
            Controls.Add(label7);
            Controls.Add(txtRegionPosition);
            Controls.Add(label5);
            Controls.Add(label14);
            Controls.Add(txtRegionName);
            Controls.Add(SectorHexagon);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionForm";
            Text = "Region Editor";
            ((System.ComponentModel.ISupportInitialize)SectorHexagon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label15;
        private TextBox txtRegionLinear;
        private Label label8;
        private TextBox txtRegionRadius;
        private Label label7;
        private TextBox txtRegionPosition;
        private Label label5;
        private Label label14;
        private TextBox txtRegionName;
        private PictureBox SectorHexagon;
        private Button BtnCreateRegion;
        private Label label2;
        private Label label3;
        private Button BtnEditRegionDefinitions;
        internal ComboBox CmbRegionDefinition;
    }
}