namespace X4SectorCreator.Forms
{
    partial class RegionPropertiesForm
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
            txtMaxNoise = new TextBox();
            label7 = new Label();
            txtMinNoise = new TextBox();
            label6 = new Label();
            txtSeed = new TextBox();
            label5 = new Label();
            txtNoiseScale = new TextBox();
            label4 = new Label();
            txtRotation = new TextBox();
            label3 = new Label();
            txtDensity = new TextBox();
            label2 = new Label();
            BtnUpdate = new Button();
            SuspendLayout();
            // 
            // txtMaxNoise
            // 
            txtMaxNoise.Location = new Point(12, 285);
            txtMaxNoise.Name = "txtMaxNoise";
            txtMaxNoise.Size = new Size(215, 23);
            txtMaxNoise.TabIndex = 64;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(12, 261);
            label7.Name = "label7";
            label7.Size = new Size(120, 21);
            label7.TabIndex = 63;
            label7.Text = "MaxNoiseValue:";
            // 
            // txtMinNoise
            // 
            txtMinNoise.Location = new Point(12, 235);
            txtMinNoise.Name = "txtMinNoise";
            txtMinNoise.Size = new Size(215, 23);
            txtMinNoise.TabIndex = 62;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(12, 211);
            label6.Name = "label6";
            label6.Size = new Size(118, 21);
            label6.TabIndex = 61;
            label6.Text = "MinNoiseValue:";
            // 
            // txtSeed
            // 
            txtSeed.Location = new Point(12, 135);
            txtSeed.Name = "txtSeed";
            txtSeed.Size = new Size(215, 23);
            txtSeed.TabIndex = 60;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(12, 111);
            label5.Name = "label5";
            label5.Size = new Size(47, 21);
            label5.TabIndex = 59;
            label5.Text = "Seed:";
            // 
            // txtNoiseScale
            // 
            txtNoiseScale.Location = new Point(12, 185);
            txtNoiseScale.Name = "txtNoiseScale";
            txtNoiseScale.Size = new Size(215, 23);
            txtNoiseScale.TabIndex = 58;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(12, 161);
            label4.Name = "label4";
            label4.Size = new Size(89, 21);
            label4.TabIndex = 57;
            label4.Text = "NoiseScale:";
            // 
            // txtRotation
            // 
            txtRotation.Location = new Point(12, 85);
            txtRotation.Name = "txtRotation";
            txtRotation.Size = new Size(215, 23);
            txtRotation.TabIndex = 56;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(12, 61);
            label3.Name = "label3";
            label3.Size = new Size(72, 21);
            label3.TabIndex = 55;
            label3.Text = "Rotation:";
            // 
            // txtDensity
            // 
            txtDensity.Location = new Point(12, 35);
            txtDensity.Name = "txtDensity";
            txtDensity.Size = new Size(215, 23);
            txtDensity.TabIndex = 54;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 11);
            label2.Name = "label2";
            label2.Size = new Size(65, 21);
            label2.TabIndex = 53;
            label2.Text = "Density:";
            // 
            // BtnUpdate
            // 
            BtnUpdate.Location = new Point(12, 314);
            BtnUpdate.Name = "BtnUpdate";
            BtnUpdate.Size = new Size(215, 33);
            BtnUpdate.TabIndex = 65;
            BtnUpdate.Text = "Update";
            BtnUpdate.UseVisualStyleBackColor = true;
            BtnUpdate.Click += BtnUpdate_Click;
            // 
            // RegionPropertiesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(243, 357);
            Controls.Add(BtnUpdate);
            Controls.Add(txtMaxNoise);
            Controls.Add(label7);
            Controls.Add(txtMinNoise);
            Controls.Add(label6);
            Controls.Add(txtSeed);
            Controls.Add(label5);
            Controls.Add(txtNoiseScale);
            Controls.Add(label4);
            Controls.Add(txtRotation);
            Controls.Add(label3);
            Controls.Add(txtDensity);
            Controls.Add(label2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionPropertiesForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMaxNoise;
        private Label label7;
        private TextBox txtMinNoise;
        private Label label6;
        private TextBox txtSeed;
        private Label label5;
        private TextBox txtNoiseScale;
        private Label label4;
        private TextBox txtRotation;
        private Label label3;
        private TextBox txtDensity;
        private Label label2;
        private Button BtnUpdate;
    }
}