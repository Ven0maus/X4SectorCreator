namespace X4SectorCreator.Forms
{
    partial class RegionFieldsForm
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
            cmbFieldType = new ComboBox();
            label1 = new Label();
            BtnAdd = new Button();
            BtnCancel = new Button();
            txtGroupRef = new TextBox();
            label2 = new Label();
            label3 = new Label();
            txtDensityFactor = new TextBox();
            label4 = new Label();
            txtRotation = new TextBox();
            label5 = new Label();
            txtRotationVariation = new TextBox();
            label6 = new Label();
            txtNoiseScale = new TextBox();
            label7 = new Label();
            txtSeed = new TextBox();
            label8 = new Label();
            txtMinNoiseValue = new TextBox();
            label9 = new Label();
            txtMaxNoiseValue = new TextBox();
            label10 = new Label();
            txtMultiplier = new TextBox();
            label11 = new Label();
            txtMedium = new TextBox();
            label12 = new Label();
            txtTexture = new TextBox();
            label13 = new Label();
            txtLodRule = new TextBox();
            label14 = new Label();
            txtSize = new TextBox();
            label15 = new Label();
            txtSizeVariation = new TextBox();
            label16 = new Label();
            txtDistanceFactor = new TextBox();
            label17 = new Label();
            txtRef = new TextBox();
            label18 = new Label();
            txtFactor = new TextBox();
            SuspendLayout();
            // 
            // cmbFieldType
            // 
            cmbFieldType.Font = new Font("Segoe UI", 10F);
            cmbFieldType.FormattingEnabled = true;
            cmbFieldType.Items.AddRange(new object[] { "asteroid", "nebula", "volumetricfog", "positional", "gravidar", "object", "debris" });
            cmbFieldType.Location = new Point(12, 37);
            cmbFieldType.Name = "cmbFieldType";
            cmbFieldType.Size = new Size(191, 25);
            cmbFieldType.TabIndex = 0;
            cmbFieldType.Text = "asteroid";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 10);
            label1.Name = "label1";
            label1.Size = new Size(89, 21);
            label1.TabIndex = 1;
            label1.Text = "Field Type*:";
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(152, 354);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(456, 45);
            BtnAdd.TabIndex = 2;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 354);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(134, 45);
            BtnCancel.TabIndex = 3;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // txtGroupRef
            // 
            txtGroupRef.Font = new Font("Segoe UI", 10F);
            txtGroupRef.Location = new Point(12, 94);
            txtGroupRef.Name = "txtGroupRef";
            txtGroupRef.Size = new Size(191, 25);
            txtGroupRef.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 67);
            label2.Name = "label2";
            label2.Size = new Size(80, 21);
            label2.TabIndex = 5;
            label2.Text = "GroupRef:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(12, 124);
            label3.Name = "label3";
            label3.Size = new Size(107, 21);
            label3.TabIndex = 7;
            label3.Text = "DensityFactor:";
            // 
            // txtDensityFactor
            // 
            txtDensityFactor.Font = new Font("Segoe UI", 10F);
            txtDensityFactor.Location = new Point(12, 151);
            txtDensityFactor.Name = "txtDensityFactor";
            txtDensityFactor.Size = new Size(191, 25);
            txtDensityFactor.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(12, 180);
            label4.Name = "label4";
            label4.Size = new Size(72, 21);
            label4.TabIndex = 9;
            label4.Text = "Rotation:";
            // 
            // txtRotation
            // 
            txtRotation.Font = new Font("Segoe UI", 10F);
            txtRotation.Location = new Point(12, 207);
            txtRotation.Name = "txtRotation";
            txtRotation.Size = new Size(191, 25);
            txtRotation.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(12, 237);
            label5.Name = "label5";
            label5.Size = new Size(134, 21);
            label5.TabIndex = 11;
            label5.Text = "RotationVariation:";
            // 
            // txtRotationVariation
            // 
            txtRotationVariation.Font = new Font("Segoe UI", 10F);
            txtRotationVariation.Location = new Point(12, 264);
            txtRotationVariation.Name = "txtRotationVariation";
            txtRotationVariation.Size = new Size(191, 25);
            txtRotationVariation.TabIndex = 10;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(12, 294);
            label6.Name = "label6";
            label6.Size = new Size(89, 21);
            label6.TabIndex = 13;
            label6.Text = "NoiseScale:";
            // 
            // txtNoiseScale
            // 
            txtNoiseScale.Font = new Font("Segoe UI", 10F);
            txtNoiseScale.Location = new Point(12, 321);
            txtNoiseScale.Name = "txtNoiseScale";
            txtNoiseScale.Size = new Size(191, 25);
            txtNoiseScale.TabIndex = 12;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(220, 294);
            label7.Name = "label7";
            label7.Size = new Size(47, 21);
            label7.TabIndex = 15;
            label7.Text = "Seed:";
            // 
            // txtSeed
            // 
            txtSeed.Font = new Font("Segoe UI", 10F);
            txtSeed.Location = new Point(220, 321);
            txtSeed.Name = "txtSeed";
            txtSeed.Size = new Size(191, 25);
            txtSeed.TabIndex = 14;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(417, 237);
            label8.Name = "label8";
            label8.Size = new Size(118, 21);
            label8.TabIndex = 17;
            label8.Text = "MinNoiseValue:";
            // 
            // txtMinNoiseValue
            // 
            txtMinNoiseValue.Font = new Font("Segoe UI", 10F);
            txtMinNoiseValue.Location = new Point(417, 264);
            txtMinNoiseValue.Name = "txtMinNoiseValue";
            txtMinNoiseValue.Size = new Size(191, 25);
            txtMinNoiseValue.TabIndex = 16;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(417, 294);
            label9.Name = "label9";
            label9.Size = new Size(120, 21);
            label9.TabIndex = 19;
            label9.Text = "MaxNoiseValue:";
            // 
            // txtMaxNoiseValue
            // 
            txtMaxNoiseValue.Font = new Font("Segoe UI", 10F);
            txtMaxNoiseValue.Location = new Point(417, 321);
            txtMaxNoiseValue.Name = "txtMaxNoiseValue";
            txtMaxNoiseValue.Size = new Size(191, 25);
            txtMaxNoiseValue.TabIndex = 18;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F);
            label10.Location = new Point(220, 10);
            label10.Name = "label10";
            label10.Size = new Size(80, 21);
            label10.TabIndex = 21;
            label10.Text = "Multiplier:";
            // 
            // txtMultiplier
            // 
            txtMultiplier.Font = new Font("Segoe UI", 10F);
            txtMultiplier.Location = new Point(220, 37);
            txtMultiplier.Name = "txtMultiplier";
            txtMultiplier.Size = new Size(191, 25);
            txtMultiplier.TabIndex = 20;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(220, 67);
            label11.Name = "label11";
            label11.Size = new Size(71, 21);
            label11.TabIndex = 23;
            label11.Text = "Medium:";
            // 
            // txtMedium
            // 
            txtMedium.Font = new Font("Segoe UI", 10F);
            txtMedium.Location = new Point(220, 94);
            txtMedium.Name = "txtMedium";
            txtMedium.Size = new Size(191, 25);
            txtMedium.TabIndex = 22;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 12F);
            label12.Location = new Point(220, 124);
            label12.Name = "label12";
            label12.Size = new Size(62, 21);
            label12.TabIndex = 25;
            label12.Text = "Texture:";
            // 
            // txtTexture
            // 
            txtTexture.Font = new Font("Segoe UI", 10F);
            txtTexture.Location = new Point(220, 151);
            txtTexture.Name = "txtTexture";
            txtTexture.Size = new Size(191, 25);
            txtTexture.TabIndex = 24;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 12F);
            label13.Location = new Point(220, 180);
            label13.Name = "label13";
            label13.Size = new Size(70, 21);
            label13.TabIndex = 27;
            label13.Text = "LodRule:";
            // 
            // txtLodRule
            // 
            txtLodRule.Font = new Font("Segoe UI", 10F);
            txtLodRule.Location = new Point(220, 207);
            txtLodRule.Name = "txtLodRule";
            txtLodRule.Size = new Size(191, 25);
            txtLodRule.TabIndex = 26;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 12F);
            label14.Location = new Point(220, 237);
            label14.Name = "label14";
            label14.Size = new Size(41, 21);
            label14.TabIndex = 29;
            label14.Text = "Size:";
            // 
            // txtSize
            // 
            txtSize.Font = new Font("Segoe UI", 10F);
            txtSize.Location = new Point(220, 264);
            txtSize.Name = "txtSize";
            txtSize.Size = new Size(191, 25);
            txtSize.TabIndex = 28;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 12F);
            label15.Location = new Point(417, 10);
            label15.Name = "label15";
            label15.Size = new Size(103, 21);
            label15.TabIndex = 31;
            label15.Text = "SizeVariation:";
            // 
            // txtSizeVariation
            // 
            txtSizeVariation.Font = new Font("Segoe UI", 10F);
            txtSizeVariation.Location = new Point(417, 37);
            txtSizeVariation.Name = "txtSizeVariation";
            txtSizeVariation.Size = new Size(191, 25);
            txtSizeVariation.TabIndex = 30;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 12F);
            label16.Location = new Point(417, 67);
            label16.Name = "label16";
            label16.Size = new Size(114, 21);
            label16.TabIndex = 33;
            label16.Text = "DistanceFactor:";
            // 
            // txtDistanceFactor
            // 
            txtDistanceFactor.Font = new Font("Segoe UI", 10F);
            txtDistanceFactor.Location = new Point(417, 94);
            txtDistanceFactor.Name = "txtDistanceFactor";
            txtDistanceFactor.Size = new Size(191, 25);
            txtDistanceFactor.TabIndex = 32;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 12F);
            label17.Location = new Point(417, 124);
            label17.Name = "label17";
            label17.Size = new Size(36, 21);
            label17.TabIndex = 35;
            label17.Text = "Ref:";
            // 
            // txtRef
            // 
            txtRef.Font = new Font("Segoe UI", 10F);
            txtRef.Location = new Point(417, 151);
            txtRef.Name = "txtRef";
            txtRef.Size = new Size(191, 25);
            txtRef.TabIndex = 34;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 12F);
            label18.Location = new Point(417, 180);
            label18.Name = "label18";
            label18.Size = new Size(55, 21);
            label18.TabIndex = 37;
            label18.Text = "Factor:";
            // 
            // txtFactor
            // 
            txtFactor.Font = new Font("Segoe UI", 10F);
            txtFactor.Location = new Point(417, 207);
            txtFactor.Name = "txtFactor";
            txtFactor.Size = new Size(191, 25);
            txtFactor.TabIndex = 36;
            // 
            // RegionFieldsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(614, 405);
            Controls.Add(label18);
            Controls.Add(txtFactor);
            Controls.Add(label17);
            Controls.Add(txtRef);
            Controls.Add(label16);
            Controls.Add(txtDistanceFactor);
            Controls.Add(label15);
            Controls.Add(txtSizeVariation);
            Controls.Add(label14);
            Controls.Add(txtSize);
            Controls.Add(label13);
            Controls.Add(txtLodRule);
            Controls.Add(label12);
            Controls.Add(txtTexture);
            Controls.Add(label11);
            Controls.Add(txtMedium);
            Controls.Add(label10);
            Controls.Add(txtMultiplier);
            Controls.Add(label9);
            Controls.Add(txtMaxNoiseValue);
            Controls.Add(label8);
            Controls.Add(txtMinNoiseValue);
            Controls.Add(label7);
            Controls.Add(txtSeed);
            Controls.Add(label6);
            Controls.Add(txtNoiseScale);
            Controls.Add(label5);
            Controls.Add(txtRotationVariation);
            Controls.Add(label4);
            Controls.Add(txtRotation);
            Controls.Add(label3);
            Controls.Add(txtDensityFactor);
            Controls.Add(label2);
            Controls.Add(txtGroupRef);
            Controls.Add(BtnCancel);
            Controls.Add(BtnAdd);
            Controls.Add(label1);
            Controls.Add(cmbFieldType);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionFieldsForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbFieldType;
        private Label label1;
        private Button BtnAdd;
        private Button BtnCancel;
        private TextBox txtGroupRef;
        private Label label2;
        private Label label3;
        private TextBox txtDensityFactor;
        private Label label4;
        private TextBox txtRotation;
        private Label label5;
        private TextBox txtRotationVariation;
        private Label label6;
        private TextBox txtNoiseScale;
        private Label label7;
        private TextBox txtSeed;
        private Label label8;
        private TextBox txtMinNoiseValue;
        private Label label9;
        private TextBox txtMaxNoiseValue;
        private Label label10;
        private TextBox txtMultiplier;
        private Label label11;
        private TextBox txtMedium;
        private Label label12;
        private TextBox txtTexture;
        private Label label13;
        private TextBox txtLodRule;
        private Label label14;
        private TextBox txtSize;
        private Label label15;
        private TextBox txtSizeVariation;
        private Label label16;
        private TextBox txtDistanceFactor;
        private Label label17;
        private TextBox txtRef;
        private Label label18;
        private TextBox txtFactor;
    }
}