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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegionFieldsForm));
            cmbFieldType = new ComboBox();
            lblFieldType = new Label();
            BtnAdd = new Button();
            BtnCancel = new Button();
            txtGroupRef = new TextBox();
            lblGroupRef = new Label();
            lblDensityFactor = new Label();
            txtDensityFactor = new TextBox();
            lblRotation = new Label();
            txtRotation = new TextBox();
            lblRotationVariation = new Label();
            txtRotationVariation = new TextBox();
            lblNoiseScale = new Label();
            txtNoiseScale = new TextBox();
            lblSeed = new Label();
            txtSeed = new TextBox();
            lblMinNoiseValue = new Label();
            txtMinNoiseValue = new TextBox();
            lblMaxNoiseValue = new Label();
            txtMaxNoiseValue = new TextBox();
            lblMultiplier = new Label();
            txtMultiplier = new TextBox();
            lblMedium = new Label();
            txtMedium = new TextBox();
            lblTexture = new Label();
            txtTexture = new TextBox();
            lblLodRule = new Label();
            txtLodRule = new TextBox();
            lblSize = new Label();
            txtSize = new TextBox();
            lblSizeVariation = new Label();
            txtSizeVariation = new TextBox();
            lblDistanceFactor = new Label();
            txtDistanceFactor = new TextBox();
            lblRef = new Label();
            txtRef = new TextBox();
            lblFactor = new Label();
            txtFactor = new TextBox();
            label1 = new Label();
            lblUniformDensity = new Label();
            txtUniformDensity = new TextBox();
            lblLocalDensity = new Label();
            txtLocalDensity = new TextBox();
            lblUniformRGB = new Label();
            txtUniformRGB = new TextBox();
            lblLocalRGB = new Label();
            txtLocalRgb = new TextBox();
            lblResources = new Label();
            txtResources = new TextBox();
            lblBackgroundFog = new Label();
            txtBackgroundFog = new TextBox();
            label2 = new Label();
            txtPlaytime = new TextBox();
            label3 = new Label();
            txtSoundId = new TextBox();
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
            // lblFieldType
            // 
            lblFieldType.AutoSize = true;
            lblFieldType.Font = new Font("Segoe UI", 12F);
            lblFieldType.Location = new Point(12, 10);
            lblFieldType.Name = "lblFieldType";
            lblFieldType.Size = new Size(89, 21);
            lblFieldType.TabIndex = 1;
            lblFieldType.Text = "Field Type*:";
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(152, 493);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(642, 45);
            BtnAdd.TabIndex = 2;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 493);
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
            // lblGroupRef
            // 
            lblGroupRef.AutoSize = true;
            lblGroupRef.Font = new Font("Segoe UI", 12F);
            lblGroupRef.Location = new Point(12, 67);
            lblGroupRef.Name = "lblGroupRef";
            lblGroupRef.Size = new Size(80, 21);
            lblGroupRef.TabIndex = 5;
            lblGroupRef.Text = "GroupRef:";
            // 
            // lblDensityFactor
            // 
            lblDensityFactor.AutoSize = true;
            lblDensityFactor.Font = new Font("Segoe UI", 12F);
            lblDensityFactor.Location = new Point(12, 124);
            lblDensityFactor.Name = "lblDensityFactor";
            lblDensityFactor.Size = new Size(107, 21);
            lblDensityFactor.TabIndex = 7;
            lblDensityFactor.Text = "DensityFactor:";
            // 
            // txtDensityFactor
            // 
            txtDensityFactor.Font = new Font("Segoe UI", 10F);
            txtDensityFactor.Location = new Point(12, 151);
            txtDensityFactor.Name = "txtDensityFactor";
            txtDensityFactor.Size = new Size(191, 25);
            txtDensityFactor.TabIndex = 6;
            // 
            // lblRotation
            // 
            lblRotation.AutoSize = true;
            lblRotation.Font = new Font("Segoe UI", 12F);
            lblRotation.Location = new Point(12, 180);
            lblRotation.Name = "lblRotation";
            lblRotation.Size = new Size(72, 21);
            lblRotation.TabIndex = 9;
            lblRotation.Text = "Rotation:";
            // 
            // txtRotation
            // 
            txtRotation.Font = new Font("Segoe UI", 10F);
            txtRotation.Location = new Point(12, 207);
            txtRotation.Name = "txtRotation";
            txtRotation.Size = new Size(191, 25);
            txtRotation.TabIndex = 8;
            // 
            // lblRotationVariation
            // 
            lblRotationVariation.AutoSize = true;
            lblRotationVariation.Font = new Font("Segoe UI", 12F);
            lblRotationVariation.Location = new Point(12, 237);
            lblRotationVariation.Name = "lblRotationVariation";
            lblRotationVariation.Size = new Size(134, 21);
            lblRotationVariation.TabIndex = 11;
            lblRotationVariation.Text = "RotationVariation:";
            // 
            // txtRotationVariation
            // 
            txtRotationVariation.Font = new Font("Segoe UI", 10F);
            txtRotationVariation.Location = new Point(12, 264);
            txtRotationVariation.Name = "txtRotationVariation";
            txtRotationVariation.Size = new Size(191, 25);
            txtRotationVariation.TabIndex = 10;
            // 
            // lblNoiseScale
            // 
            lblNoiseScale.AutoSize = true;
            lblNoiseScale.Font = new Font("Segoe UI", 12F);
            lblNoiseScale.Location = new Point(12, 294);
            lblNoiseScale.Name = "lblNoiseScale";
            lblNoiseScale.Size = new Size(89, 21);
            lblNoiseScale.TabIndex = 13;
            lblNoiseScale.Text = "NoiseScale:";
            // 
            // txtNoiseScale
            // 
            txtNoiseScale.Font = new Font("Segoe UI", 10F);
            txtNoiseScale.Location = new Point(12, 321);
            txtNoiseScale.Name = "txtNoiseScale";
            txtNoiseScale.Size = new Size(191, 25);
            txtNoiseScale.TabIndex = 12;
            // 
            // lblSeed
            // 
            lblSeed.AutoSize = true;
            lblSeed.Font = new Font("Segoe UI", 12F);
            lblSeed.Location = new Point(209, 294);
            lblSeed.Name = "lblSeed";
            lblSeed.Size = new Size(47, 21);
            lblSeed.TabIndex = 15;
            lblSeed.Text = "Seed:";
            // 
            // txtSeed
            // 
            txtSeed.Font = new Font("Segoe UI", 10F);
            txtSeed.Location = new Point(209, 321);
            txtSeed.Name = "txtSeed";
            txtSeed.Size = new Size(191, 25);
            txtSeed.TabIndex = 14;
            // 
            // lblMinNoiseValue
            // 
            lblMinNoiseValue.AutoSize = true;
            lblMinNoiseValue.Font = new Font("Segoe UI", 12F);
            lblMinNoiseValue.Location = new Point(406, 237);
            lblMinNoiseValue.Name = "lblMinNoiseValue";
            lblMinNoiseValue.Size = new Size(118, 21);
            lblMinNoiseValue.TabIndex = 17;
            lblMinNoiseValue.Text = "MinNoiseValue:";
            // 
            // txtMinNoiseValue
            // 
            txtMinNoiseValue.Font = new Font("Segoe UI", 10F);
            txtMinNoiseValue.Location = new Point(406, 264);
            txtMinNoiseValue.Name = "txtMinNoiseValue";
            txtMinNoiseValue.Size = new Size(191, 25);
            txtMinNoiseValue.TabIndex = 16;
            // 
            // lblMaxNoiseValue
            // 
            lblMaxNoiseValue.AutoSize = true;
            lblMaxNoiseValue.Font = new Font("Segoe UI", 12F);
            lblMaxNoiseValue.Location = new Point(406, 294);
            lblMaxNoiseValue.Name = "lblMaxNoiseValue";
            lblMaxNoiseValue.Size = new Size(120, 21);
            lblMaxNoiseValue.TabIndex = 19;
            lblMaxNoiseValue.Text = "MaxNoiseValue:";
            // 
            // txtMaxNoiseValue
            // 
            txtMaxNoiseValue.Font = new Font("Segoe UI", 10F);
            txtMaxNoiseValue.Location = new Point(406, 321);
            txtMaxNoiseValue.Name = "txtMaxNoiseValue";
            txtMaxNoiseValue.Size = new Size(191, 25);
            txtMaxNoiseValue.TabIndex = 18;
            // 
            // lblMultiplier
            // 
            lblMultiplier.AutoSize = true;
            lblMultiplier.Font = new Font("Segoe UI", 12F);
            lblMultiplier.Location = new Point(209, 10);
            lblMultiplier.Name = "lblMultiplier";
            lblMultiplier.Size = new Size(80, 21);
            lblMultiplier.TabIndex = 21;
            lblMultiplier.Text = "Multiplier:";
            // 
            // txtMultiplier
            // 
            txtMultiplier.Font = new Font("Segoe UI", 10F);
            txtMultiplier.Location = new Point(209, 37);
            txtMultiplier.Name = "txtMultiplier";
            txtMultiplier.Size = new Size(191, 25);
            txtMultiplier.TabIndex = 20;
            // 
            // lblMedium
            // 
            lblMedium.AutoSize = true;
            lblMedium.Font = new Font("Segoe UI", 12F);
            lblMedium.Location = new Point(209, 67);
            lblMedium.Name = "lblMedium";
            lblMedium.Size = new Size(71, 21);
            lblMedium.TabIndex = 23;
            lblMedium.Text = "Medium:";
            // 
            // txtMedium
            // 
            txtMedium.Font = new Font("Segoe UI", 10F);
            txtMedium.Location = new Point(209, 94);
            txtMedium.Name = "txtMedium";
            txtMedium.Size = new Size(191, 25);
            txtMedium.TabIndex = 22;
            // 
            // lblTexture
            // 
            lblTexture.AutoSize = true;
            lblTexture.Font = new Font("Segoe UI", 12F);
            lblTexture.Location = new Point(209, 124);
            lblTexture.Name = "lblTexture";
            lblTexture.Size = new Size(62, 21);
            lblTexture.TabIndex = 25;
            lblTexture.Text = "Texture:";
            // 
            // txtTexture
            // 
            txtTexture.Font = new Font("Segoe UI", 10F);
            txtTexture.Location = new Point(209, 151);
            txtTexture.Name = "txtTexture";
            txtTexture.Size = new Size(191, 25);
            txtTexture.TabIndex = 24;
            // 
            // lblLodRule
            // 
            lblLodRule.AutoSize = true;
            lblLodRule.Font = new Font("Segoe UI", 12F);
            lblLodRule.Location = new Point(209, 180);
            lblLodRule.Name = "lblLodRule";
            lblLodRule.Size = new Size(70, 21);
            lblLodRule.TabIndex = 27;
            lblLodRule.Text = "LodRule:";
            // 
            // txtLodRule
            // 
            txtLodRule.Font = new Font("Segoe UI", 10F);
            txtLodRule.Location = new Point(209, 207);
            txtLodRule.Name = "txtLodRule";
            txtLodRule.Size = new Size(191, 25);
            txtLodRule.TabIndex = 26;
            // 
            // lblSize
            // 
            lblSize.AutoSize = true;
            lblSize.Font = new Font("Segoe UI", 12F);
            lblSize.Location = new Point(209, 237);
            lblSize.Name = "lblSize";
            lblSize.Size = new Size(41, 21);
            lblSize.TabIndex = 29;
            lblSize.Text = "Size:";
            // 
            // txtSize
            // 
            txtSize.Font = new Font("Segoe UI", 10F);
            txtSize.Location = new Point(209, 264);
            txtSize.Name = "txtSize";
            txtSize.Size = new Size(191, 25);
            txtSize.TabIndex = 28;
            // 
            // lblSizeVariation
            // 
            lblSizeVariation.AutoSize = true;
            lblSizeVariation.Font = new Font("Segoe UI", 12F);
            lblSizeVariation.Location = new Point(406, 10);
            lblSizeVariation.Name = "lblSizeVariation";
            lblSizeVariation.Size = new Size(103, 21);
            lblSizeVariation.TabIndex = 31;
            lblSizeVariation.Text = "SizeVariation:";
            // 
            // txtSizeVariation
            // 
            txtSizeVariation.Font = new Font("Segoe UI", 10F);
            txtSizeVariation.Location = new Point(406, 37);
            txtSizeVariation.Name = "txtSizeVariation";
            txtSizeVariation.Size = new Size(191, 25);
            txtSizeVariation.TabIndex = 30;
            // 
            // lblDistanceFactor
            // 
            lblDistanceFactor.AutoSize = true;
            lblDistanceFactor.Font = new Font("Segoe UI", 12F);
            lblDistanceFactor.Location = new Point(406, 67);
            lblDistanceFactor.Name = "lblDistanceFactor";
            lblDistanceFactor.Size = new Size(114, 21);
            lblDistanceFactor.TabIndex = 33;
            lblDistanceFactor.Text = "DistanceFactor:";
            // 
            // txtDistanceFactor
            // 
            txtDistanceFactor.Font = new Font("Segoe UI", 10F);
            txtDistanceFactor.Location = new Point(406, 94);
            txtDistanceFactor.Name = "txtDistanceFactor";
            txtDistanceFactor.Size = new Size(191, 25);
            txtDistanceFactor.TabIndex = 32;
            // 
            // lblRef
            // 
            lblRef.AutoSize = true;
            lblRef.Font = new Font("Segoe UI", 12F);
            lblRef.Location = new Point(406, 124);
            lblRef.Name = "lblRef";
            lblRef.Size = new Size(36, 21);
            lblRef.TabIndex = 35;
            lblRef.Text = "Ref:";
            // 
            // txtRef
            // 
            txtRef.Font = new Font("Segoe UI", 10F);
            txtRef.Location = new Point(406, 151);
            txtRef.Name = "txtRef";
            txtRef.Size = new Size(191, 25);
            txtRef.TabIndex = 34;
            // 
            // lblFactor
            // 
            lblFactor.AutoSize = true;
            lblFactor.Font = new Font("Segoe UI", 12F);
            lblFactor.Location = new Point(406, 180);
            lblFactor.Name = "lblFactor";
            lblFactor.Size = new Size(55, 21);
            lblFactor.TabIndex = 37;
            lblFactor.Text = "Factor:";
            // 
            // txtFactor
            // 
            txtFactor.Font = new Font("Segoe UI", 10F);
            txtFactor.Location = new Point(406, 207);
            txtFactor.Name = "txtFactor";
            txtFactor.Size = new Size(191, 25);
            txtFactor.TabIndex = 36;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 10F);
            label1.Location = new Point(12, 414);
            label1.Name = "label1";
            label1.Size = new Size(782, 76);
            label1.TabIndex = 38;
            label1.Text = resources.GetString("label1.Text");
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblUniformDensity
            // 
            lblUniformDensity.AutoSize = true;
            lblUniformDensity.Font = new Font("Segoe UI", 12F);
            lblUniformDensity.Location = new Point(603, 180);
            lblUniformDensity.Name = "lblUniformDensity";
            lblUniformDensity.Size = new Size(123, 21);
            lblUniformDensity.TabIndex = 50;
            lblUniformDensity.Text = "UniformDensity:";
            // 
            // txtUniformDensity
            // 
            txtUniformDensity.Font = new Font("Segoe UI", 10F);
            txtUniformDensity.Location = new Point(603, 207);
            txtUniformDensity.Name = "txtUniformDensity";
            txtUniformDensity.Size = new Size(191, 25);
            txtUniformDensity.TabIndex = 49;
            // 
            // lblLocalDensity
            // 
            lblLocalDensity.AutoSize = true;
            lblLocalDensity.Font = new Font("Segoe UI", 12F);
            lblLocalDensity.Location = new Point(603, 124);
            lblLocalDensity.Name = "lblLocalDensity";
            lblLocalDensity.Size = new Size(101, 21);
            lblLocalDensity.TabIndex = 48;
            lblLocalDensity.Text = "LocalDensity:";
            // 
            // txtLocalDensity
            // 
            txtLocalDensity.Font = new Font("Segoe UI", 10F);
            txtLocalDensity.Location = new Point(603, 151);
            txtLocalDensity.Name = "txtLocalDensity";
            txtLocalDensity.Size = new Size(191, 25);
            txtLocalDensity.TabIndex = 47;
            // 
            // lblUniformRGB
            // 
            lblUniformRGB.AutoSize = true;
            lblUniformRGB.Font = new Font("Segoe UI", 12F);
            lblUniformRGB.Location = new Point(603, 67);
            lblUniformRGB.Name = "lblUniformRGB";
            lblUniformRGB.Size = new Size(101, 21);
            lblUniformRGB.TabIndex = 46;
            lblUniformRGB.Text = "UniformRGB:";
            // 
            // txtUniformRGB
            // 
            txtUniformRGB.Font = new Font("Segoe UI", 10F);
            txtUniformRGB.Location = new Point(603, 94);
            txtUniformRGB.Name = "txtUniformRGB";
            txtUniformRGB.Size = new Size(191, 25);
            txtUniformRGB.TabIndex = 45;
            // 
            // lblLocalRGB
            // 
            lblLocalRGB.AutoSize = true;
            lblLocalRGB.Font = new Font("Segoe UI", 12F);
            lblLocalRGB.Location = new Point(603, 10);
            lblLocalRGB.Name = "lblLocalRGB";
            lblLocalRGB.Size = new Size(79, 21);
            lblLocalRGB.TabIndex = 44;
            lblLocalRGB.Text = "LocalRGB:";
            // 
            // txtLocalRgb
            // 
            txtLocalRgb.Font = new Font("Segoe UI", 10F);
            txtLocalRgb.Location = new Point(603, 37);
            txtLocalRgb.Name = "txtLocalRgb";
            txtLocalRgb.Size = new Size(191, 25);
            txtLocalRgb.TabIndex = 43;
            // 
            // lblResources
            // 
            lblResources.AutoSize = true;
            lblResources.Font = new Font("Segoe UI", 12F);
            lblResources.Location = new Point(603, 294);
            lblResources.Name = "lblResources";
            lblResources.Size = new Size(84, 21);
            lblResources.TabIndex = 42;
            lblResources.Text = "Resources:";
            // 
            // txtResources
            // 
            txtResources.Font = new Font("Segoe UI", 10F);
            txtResources.Location = new Point(603, 321);
            txtResources.Name = "txtResources";
            txtResources.Size = new Size(191, 25);
            txtResources.TabIndex = 41;
            // 
            // lblBackgroundFog
            // 
            lblBackgroundFog.AutoSize = true;
            lblBackgroundFog.Font = new Font("Segoe UI", 12F);
            lblBackgroundFog.Location = new Point(603, 237);
            lblBackgroundFog.Name = "lblBackgroundFog";
            lblBackgroundFog.Size = new Size(122, 21);
            lblBackgroundFog.TabIndex = 40;
            lblBackgroundFog.Text = "BackgroundFog:";
            // 
            // txtBackgroundFog
            // 
            txtBackgroundFog.Font = new Font("Segoe UI", 10F);
            txtBackgroundFog.Location = new Point(603, 264);
            txtBackgroundFog.Name = "txtBackgroundFog";
            txtBackgroundFog.Size = new Size(191, 25);
            txtBackgroundFog.TabIndex = 39;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(209, 349);
            label2.Name = "label2";
            label2.Size = new Size(73, 21);
            label2.TabIndex = 54;
            label2.Text = "Playtime:";
            // 
            // txtPlaytime
            // 
            txtPlaytime.Font = new Font("Segoe UI", 10F);
            txtPlaytime.Location = new Point(209, 373);
            txtPlaytime.Name = "txtPlaytime";
            txtPlaytime.Size = new Size(191, 25);
            txtPlaytime.TabIndex = 53;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(12, 349);
            label3.Name = "label3";
            label3.Size = new Size(71, 21);
            label3.TabIndex = 52;
            label3.Text = "SoundId:";
            // 
            // txtSoundId
            // 
            txtSoundId.Font = new Font("Segoe UI", 10F);
            txtSoundId.Location = new Point(12, 373);
            txtSoundId.Name = "txtSoundId";
            txtSoundId.Size = new Size(191, 25);
            txtSoundId.TabIndex = 51;
            // 
            // RegionFieldsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(801, 543);
            Controls.Add(label2);
            Controls.Add(txtPlaytime);
            Controls.Add(label3);
            Controls.Add(txtSoundId);
            Controls.Add(lblUniformDensity);
            Controls.Add(txtUniformDensity);
            Controls.Add(lblLocalDensity);
            Controls.Add(txtLocalDensity);
            Controls.Add(lblUniformRGB);
            Controls.Add(txtUniformRGB);
            Controls.Add(lblLocalRGB);
            Controls.Add(txtLocalRgb);
            Controls.Add(lblResources);
            Controls.Add(txtResources);
            Controls.Add(lblBackgroundFog);
            Controls.Add(txtBackgroundFog);
            Controls.Add(label1);
            Controls.Add(lblFactor);
            Controls.Add(txtFactor);
            Controls.Add(lblRef);
            Controls.Add(txtRef);
            Controls.Add(lblDistanceFactor);
            Controls.Add(txtDistanceFactor);
            Controls.Add(lblSizeVariation);
            Controls.Add(txtSizeVariation);
            Controls.Add(lblSize);
            Controls.Add(txtSize);
            Controls.Add(lblLodRule);
            Controls.Add(txtLodRule);
            Controls.Add(lblTexture);
            Controls.Add(txtTexture);
            Controls.Add(lblMedium);
            Controls.Add(txtMedium);
            Controls.Add(lblMultiplier);
            Controls.Add(txtMultiplier);
            Controls.Add(lblMaxNoiseValue);
            Controls.Add(txtMaxNoiseValue);
            Controls.Add(lblMinNoiseValue);
            Controls.Add(txtMinNoiseValue);
            Controls.Add(lblSeed);
            Controls.Add(txtSeed);
            Controls.Add(lblNoiseScale);
            Controls.Add(txtNoiseScale);
            Controls.Add(lblRotationVariation);
            Controls.Add(txtRotationVariation);
            Controls.Add(lblRotation);
            Controls.Add(txtRotation);
            Controls.Add(lblDensityFactor);
            Controls.Add(txtDensityFactor);
            Controls.Add(lblGroupRef);
            Controls.Add(txtGroupRef);
            Controls.Add(BtnCancel);
            Controls.Add(BtnAdd);
            Controls.Add(lblFieldType);
            Controls.Add(cmbFieldType);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionFieldsForm";
            Text = "Region Field Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbFieldType;
        private Label lblFieldType;
        private Button BtnAdd;
        private Button BtnCancel;
        private TextBox txtGroupRef;
        private Label lblGroupRef;
        private Label lblDensityFactor;
        private TextBox txtDensityFactor;
        private Label lblRotation;
        private TextBox txtRotation;
        private Label lblRotationVariation;
        private TextBox txtRotationVariation;
        private Label lblNoiseScale;
        private TextBox txtNoiseScale;
        private Label lblSeed;
        private TextBox txtSeed;
        private Label lblMinNoiseValue;
        private TextBox txtMinNoiseValue;
        private Label lblMaxNoiseValue;
        private TextBox txtMaxNoiseValue;
        private Label lblMultiplier;
        private TextBox txtMultiplier;
        private Label lblMedium;
        private TextBox txtMedium;
        private Label lblTexture;
        private TextBox txtTexture;
        private Label lblLodRule;
        private TextBox txtLodRule;
        private Label lblSize;
        private TextBox txtSize;
        private Label lblSizeVariation;
        private TextBox txtSizeVariation;
        private Label lblDistanceFactor;
        private TextBox txtDistanceFactor;
        private Label lblRef;
        private TextBox txtRef;
        private Label lblFactor;
        private TextBox txtFactor;
        private Label label1;
        private Label lblUniformDensity;
        private TextBox txtUniformDensity;
        private Label lblLocalDensity;
        private TextBox txtLocalDensity;
        private Label lblUniformRGB;
        private TextBox txtUniformRGB;
        private Label lblLocalRGB;
        private TextBox txtLocalRgb;
        private Label lblResources;
        private TextBox txtResources;
        private Label lblBackgroundFog;
        private TextBox txtBackgroundFog;
        private Label label2;
        private TextBox txtPlaytime;
        private Label label3;
        private TextBox txtSoundId;
    }
}