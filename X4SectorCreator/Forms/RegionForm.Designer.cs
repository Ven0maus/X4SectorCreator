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
            SectorHexagon = new PictureBox();
            txtRegionName = new TextBox();
            label14 = new Label();
            TabControlFalloff = new TabControl();
            tabLateral = new TabPage();
            ListBoxLateral = new ListBox();
            tabRadial = new TabPage();
            ListBoxRadial = new ListBox();
            BtnFalloffAdd = new Button();
            BtnFalloffDel = new Button();
            BtnFalloffDown = new Button();
            BtnFalloffUp = new Button();
            label3 = new Label();
            cmbBoundaryType = new ComboBox();
            ListBoxResources = new ListBox();
            label4 = new Label();
            BtnResourcesDel = new Button();
            BtnResourcesAdd = new Button();
            label2 = new Label();
            label5 = new Label();
            ListBoxFields = new ListBox();
            label6 = new Label();
            BtnFieldsDel = new Button();
            BtnFieldsAdd = new Button();
            txtRegionPosition = new TextBox();
            label7 = new Label();
            label8 = new Label();
            txtRegionRadius = new TextBox();
            BtnCreateRegion = new Button();
            label1 = new Label();
            txtDensity = new TextBox();
            label9 = new Label();
            txtRotation = new TextBox();
            label10 = new Label();
            txtSeed = new TextBox();
            label11 = new Label();
            txtNoiseScale = new TextBox();
            label12 = new Label();
            txtMinNoiseValue = new TextBox();
            label13 = new Label();
            txtMaxNoiseValue = new TextBox();
            ((System.ComponentModel.ISupportInitialize)SectorHexagon).BeginInit();
            TabControlFalloff.SuspendLayout();
            tabLateral.SuspendLayout();
            tabRadial.SuspendLayout();
            SuspendLayout();
            // 
            // SectorHexagon
            // 
            SectorHexagon.Location = new Point(458, 293);
            SectorHexagon.Name = "SectorHexagon";
            SectorHexagon.Size = new Size(300, 300);
            SectorHexagon.TabIndex = 17;
            SectorHexagon.TabStop = false;
            // 
            // txtRegionName
            // 
            txtRegionName.Location = new Point(12, 33);
            txtRegionName.Name = "txtRegionName";
            txtRegionName.Size = new Size(230, 23);
            txtRegionName.TabIndex = 39;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 12F);
            label14.Location = new Point(12, 9);
            label14.Name = "label14";
            label14.Size = new Size(105, 21);
            label14.TabIndex = 40;
            label14.Text = "Region name:";
            // 
            // TabControlFalloff
            // 
            TabControlFalloff.Controls.Add(tabLateral);
            TabControlFalloff.Controls.Add(tabRadial);
            TabControlFalloff.HotTrack = true;
            TabControlFalloff.ImeMode = ImeMode.NoControl;
            TabControlFalloff.Location = new Point(461, 18);
            TabControlFalloff.Name = "TabControlFalloff";
            TabControlFalloff.SelectedIndex = 0;
            TabControlFalloff.Size = new Size(250, 245);
            TabControlFalloff.TabIndex = 45;
            // 
            // tabLateral
            // 
            tabLateral.Controls.Add(ListBoxLateral);
            tabLateral.Location = new Point(4, 24);
            tabLateral.Name = "tabLateral";
            tabLateral.Padding = new Padding(3);
            tabLateral.Size = new Size(242, 217);
            tabLateral.TabIndex = 0;
            tabLateral.Text = "Lateral";
            tabLateral.UseVisualStyleBackColor = true;
            // 
            // ListBoxLateral
            // 
            ListBoxLateral.DrawMode = DrawMode.OwnerDrawVariable;
            ListBoxLateral.FormattingEnabled = true;
            ListBoxLateral.Items.AddRange(new object[] { "step 0 | position: 1.0 | value: 1.2" });
            ListBoxLateral.Location = new Point(0, 0);
            ListBoxLateral.Name = "ListBoxLateral";
            ListBoxLateral.Size = new Size(242, 216);
            ListBoxLateral.TabIndex = 47;
            // 
            // tabRadial
            // 
            tabRadial.Controls.Add(ListBoxRadial);
            tabRadial.Location = new Point(4, 24);
            tabRadial.Name = "tabRadial";
            tabRadial.Padding = new Padding(3);
            tabRadial.Size = new Size(242, 217);
            tabRadial.TabIndex = 1;
            tabRadial.Text = "Radial";
            tabRadial.UseVisualStyleBackColor = true;
            // 
            // ListBoxRadial
            // 
            ListBoxRadial.DrawMode = DrawMode.OwnerDrawVariable;
            ListBoxRadial.FormattingEnabled = true;
            ListBoxRadial.Items.AddRange(new object[] { "step 0 | position: 1.0 | value: 1.2" });
            ListBoxRadial.Location = new Point(0, 0);
            ListBoxRadial.Name = "ListBoxRadial";
            ListBoxRadial.Size = new Size(242, 216);
            ListBoxRadial.TabIndex = 48;
            // 
            // BtnFalloffAdd
            // 
            BtnFalloffAdd.Location = new Point(713, 154);
            BtnFalloffAdd.Name = "BtnFalloffAdd";
            BtnFalloffAdd.Size = new Size(41, 31);
            BtnFalloffAdd.TabIndex = 47;
            BtnFalloffAdd.Text = "Add";
            BtnFalloffAdd.UseVisualStyleBackColor = true;
            // 
            // BtnFalloffDel
            // 
            BtnFalloffDel.Location = new Point(713, 191);
            BtnFalloffDel.Name = "BtnFalloffDel";
            BtnFalloffDel.Size = new Size(41, 31);
            BtnFalloffDel.TabIndex = 48;
            BtnFalloffDel.Text = "Del";
            BtnFalloffDel.UseVisualStyleBackColor = true;
            // 
            // BtnFalloffDown
            // 
            BtnFalloffDown.Font = new Font("Segoe UI", 8F);
            BtnFalloffDown.Location = new Point(713, 117);
            BtnFalloffDown.Name = "BtnFalloffDown";
            BtnFalloffDown.Size = new Size(41, 31);
            BtnFalloffDown.TabIndex = 49;
            BtnFalloffDown.Text = "V";
            BtnFalloffDown.UseVisualStyleBackColor = true;
            // 
            // BtnFalloffUp
            // 
            BtnFalloffUp.Font = new Font("Segoe UI", 13F);
            BtnFalloffUp.Location = new Point(713, 80);
            BtnFalloffUp.Name = "BtnFalloffUp";
            BtnFalloffUp.Size = new Size(41, 31);
            BtnFalloffUp.TabIndex = 50;
            BtnFalloffUp.Text = "^";
            BtnFalloffUp.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(246, 9);
            label3.Name = "label3";
            label3.Size = new Size(116, 21);
            label3.TabIndex = 51;
            label3.Text = "Boundary Type:";
            // 
            // cmbBoundaryType
            // 
            cmbBoundaryType.Font = new Font("Segoe UI", 11F);
            cmbBoundaryType.FormattingEnabled = true;
            cmbBoundaryType.Items.AddRange(new object[] { "Cylinder", "Sphere" });
            cmbBoundaryType.Location = new Point(248, 33);
            cmbBoundaryType.Name = "cmbBoundaryType";
            cmbBoundaryType.Size = new Size(204, 28);
            cmbBoundaryType.TabIndex = 52;
            cmbBoundaryType.Text = "Cylinder";
            // 
            // ListBoxResources
            // 
            ListBoxResources.FormattingEnabled = true;
            ListBoxResources.Location = new Point(248, 92);
            ListBoxResources.Name = "ListBoxResources";
            ListBoxResources.Size = new Size(204, 214);
            ListBoxResources.TabIndex = 53;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 15F);
            label4.Location = new Point(248, 61);
            label4.Name = "label4";
            label4.Size = new Size(160, 28);
            label4.TabIndex = 54;
            label4.Text = "Region resources";
            // 
            // BtnResourcesDel
            // 
            BtnResourcesDel.Location = new Point(248, 312);
            BtnResourcesDel.Name = "BtnResourcesDel";
            BtnResourcesDel.Size = new Size(99, 31);
            BtnResourcesDel.TabIndex = 56;
            BtnResourcesDel.Text = "Delete resource";
            BtnResourcesDel.UseVisualStyleBackColor = true;
            // 
            // BtnResourcesAdd
            // 
            BtnResourcesAdd.Location = new Point(353, 312);
            BtnResourcesAdd.Name = "BtnResourcesAdd";
            BtnResourcesAdd.Size = new Size(99, 31);
            BtnResourcesAdd.TabIndex = 55;
            BtnResourcesAdd.Text = "Add resource";
            BtnResourcesAdd.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 15F);
            label2.Location = new Point(581, 10);
            label2.Name = "label2";
            label2.Size = new Size(128, 28);
            label2.TabIndex = 57;
            label2.Text = "Region falloff";
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 15F);
            label5.Location = new Point(458, 262);
            label5.Name = "label5";
            label5.Size = new Size(300, 28);
            label5.TabIndex = 58;
            label5.Text = "Sector";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ListBoxFields
            // 
            ListBoxFields.FormattingEnabled = true;
            ListBoxFields.Location = new Point(12, 377);
            ListBoxFields.Name = "ListBoxFields";
            ListBoxFields.Size = new Size(436, 229);
            ListBoxFields.TabIndex = 59;
            // 
            // label6
            // 
            label6.Font = new Font("Segoe UI", 15F);
            label6.Location = new Point(246, 346);
            label6.Name = "label6";
            label6.Size = new Size(202, 28);
            label6.TabIndex = 60;
            label6.Text = "Region Fields";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // BtnFieldsDel
            // 
            BtnFieldsDel.Location = new Point(12, 612);
            BtnFieldsDel.Name = "BtnFieldsDel";
            BtnFieldsDel.Size = new Size(88, 31);
            BtnFieldsDel.TabIndex = 62;
            BtnFieldsDel.Text = "Delete Field";
            BtnFieldsDel.UseVisualStyleBackColor = true;
            // 
            // BtnFieldsAdd
            // 
            BtnFieldsAdd.Location = new Point(106, 612);
            BtnFieldsAdd.Name = "BtnFieldsAdd";
            BtnFieldsAdd.Size = new Size(90, 31);
            BtnFieldsAdd.TabIndex = 61;
            BtnFieldsAdd.Text = "Add Field";
            BtnFieldsAdd.UseVisualStyleBackColor = true;
            // 
            // txtRegionPosition
            // 
            txtRegionPosition.Enabled = false;
            txtRegionPosition.Location = new Point(581, 599);
            txtRegionPosition.Name = "txtRegionPosition";
            txtRegionPosition.Size = new Size(173, 23);
            txtRegionPosition.TabIndex = 63;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(454, 599);
            label7.Name = "label7";
            label7.Size = new Size(121, 21);
            label7.TabIndex = 64;
            label7.Text = "Region Position:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(461, 626);
            label8.Name = "label8";
            label8.Size = new Size(113, 21);
            label8.TabIndex = 66;
            label8.Text = "Region Radius:";
            // 
            // txtRegionRadius
            // 
            txtRegionRadius.Enabled = false;
            txtRegionRadius.Location = new Point(581, 626);
            txtRegionRadius.Name = "txtRegionRadius";
            txtRegionRadius.Size = new Size(173, 23);
            txtRegionRadius.TabIndex = 65;
            // 
            // BtnCreateRegion
            // 
            BtnCreateRegion.Location = new Point(202, 613);
            BtnCreateRegion.Name = "BtnCreateRegion";
            BtnCreateRegion.Size = new Size(246, 31);
            BtnCreateRegion.TabIndex = 67;
            BtnCreateRegion.Text = "Create Region";
            BtnCreateRegion.UseVisualStyleBackColor = true;
            BtnCreateRegion.Click += BtnCreateRegion_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 59);
            label1.Name = "label1";
            label1.Size = new Size(116, 21);
            label1.TabIndex = 69;
            label1.Text = "Region density:";
            // 
            // txtDensity
            // 
            txtDensity.Location = new Point(12, 83);
            txtDensity.Name = "txtDensity";
            txtDensity.Size = new Size(230, 23);
            txtDensity.TabIndex = 68;
            txtDensity.Text = "1.5";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(12, 109);
            label9.Name = "label9";
            label9.Size = new Size(121, 21);
            label9.TabIndex = 71;
            label9.Text = "Region rotation:";
            // 
            // txtRotation
            // 
            txtRotation.Location = new Point(12, 133);
            txtRotation.Name = "txtRotation";
            txtRotation.Size = new Size(230, 23);
            txtRotation.TabIndex = 70;
            txtRotation.Text = "0";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F);
            label10.Location = new Point(12, 159);
            label10.Name = "label10";
            label10.Size = new Size(98, 21);
            label10.TabIndex = 73;
            label10.Text = "Region seed:";
            // 
            // txtSeed
            // 
            txtSeed.Location = new Point(12, 183);
            txtSeed.Name = "txtSeed";
            txtSeed.Size = new Size(230, 23);
            txtSeed.TabIndex = 72;
            txtSeed.Text = "1337";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(12, 209);
            label11.Name = "label11";
            label11.Size = new Size(137, 21);
            label11.TabIndex = 75;
            label11.Text = "Region noisescale:";
            // 
            // txtNoiseScale
            // 
            txtNoiseScale.Location = new Point(12, 233);
            txtNoiseScale.Name = "txtNoiseScale";
            txtNoiseScale.Size = new Size(230, 23);
            txtNoiseScale.TabIndex = 74;
            txtNoiseScale.Text = "10000";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 12F);
            label12.Location = new Point(12, 259);
            label12.Name = "label12";
            label12.Size = new Size(167, 21);
            label12.TabIndex = 77;
            label12.Text = "Region minnoisevalue:";
            // 
            // txtMinNoiseValue
            // 
            txtMinNoiseValue.Location = new Point(12, 283);
            txtMinNoiseValue.Name = "txtMinNoiseValue";
            txtMinNoiseValue.Size = new Size(230, 23);
            txtMinNoiseValue.TabIndex = 76;
            txtMinNoiseValue.Text = "0.15";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 12F);
            label13.Location = new Point(12, 309);
            label13.Name = "label13";
            label13.Size = new Size(169, 21);
            label13.TabIndex = 79;
            label13.Text = "Region maxnoisevalue:";
            // 
            // txtMaxNoiseValue
            // 
            txtMaxNoiseValue.Location = new Point(12, 333);
            txtMaxNoiseValue.Name = "txtMaxNoiseValue";
            txtMaxNoiseValue.Size = new Size(230, 23);
            txtMaxNoiseValue.TabIndex = 78;
            txtMaxNoiseValue.Text = "1";
            // 
            // RegionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(763, 656);
            Controls.Add(label13);
            Controls.Add(txtMaxNoiseValue);
            Controls.Add(label12);
            Controls.Add(txtMinNoiseValue);
            Controls.Add(label11);
            Controls.Add(txtNoiseScale);
            Controls.Add(label10);
            Controls.Add(txtSeed);
            Controls.Add(label9);
            Controls.Add(txtRotation);
            Controls.Add(label1);
            Controls.Add(txtDensity);
            Controls.Add(BtnCreateRegion);
            Controls.Add(label8);
            Controls.Add(txtRegionRadius);
            Controls.Add(label7);
            Controls.Add(txtRegionPosition);
            Controls.Add(BtnFieldsDel);
            Controls.Add(BtnFieldsAdd);
            Controls.Add(label6);
            Controls.Add(ListBoxFields);
            Controls.Add(label5);
            Controls.Add(label2);
            Controls.Add(BtnResourcesDel);
            Controls.Add(BtnResourcesAdd);
            Controls.Add(label4);
            Controls.Add(ListBoxResources);
            Controls.Add(cmbBoundaryType);
            Controls.Add(label3);
            Controls.Add(BtnFalloffUp);
            Controls.Add(BtnFalloffDown);
            Controls.Add(BtnFalloffDel);
            Controls.Add(BtnFalloffAdd);
            Controls.Add(TabControlFalloff);
            Controls.Add(label14);
            Controls.Add(txtRegionName);
            Controls.Add(SectorHexagon);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionForm";
            Text = "X4 Sector Creator";
            ((System.ComponentModel.ISupportInitialize)SectorHexagon).EndInit();
            TabControlFalloff.ResumeLayout(false);
            tabLateral.ResumeLayout(false);
            tabRadial.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox SectorHexagon;
        private TextBox txtRegionName;
        private Label label14;
        private TabControl TabControlFalloff;
        private TabPage tabLateral;
        private TabPage tabRadial;
        private ListBox ListBoxLateral;
        private ListBox ListBoxRadial;
        private Button BtnFalloffAdd;
        private Button BtnFalloffDel;
        private Button BtnFalloffDown;
        private Button BtnFalloffUp;
        private Label label3;
        private ComboBox cmbBoundaryType;
        private ListBox ListBoxResources;
        private Label label4;
        private Button BtnResourcesDel;
        private Button BtnResourcesAdd;
        private Label label2;
        private Label label5;
        private ListBox ListBoxFields;
        private Label label6;
        private Button BtnFieldsDel;
        private Button BtnFieldsAdd;
        private TextBox txtRegionPosition;
        private Label label7;
        private Label label8;
        private TextBox txtRegionRadius;
        private Button BtnCreateRegion;
        private Label label1;
        private TextBox txtDensity;
        private Label label9;
        private TextBox txtRotation;
        private Label label10;
        private TextBox txtSeed;
        private Label label11;
        private TextBox txtNoiseScale;
        private Label label12;
        private TextBox txtMinNoiseValue;
        private Label label13;
        private TextBox txtMaxNoiseValue;
    }
}