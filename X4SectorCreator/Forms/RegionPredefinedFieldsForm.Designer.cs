namespace X4SectorCreator.Forms
{
    partial class RegionPredefinedFieldsForm
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
            cmbAsteroids = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            cmbNebula = new ComboBox();
            label3 = new Label();
            cmbVolumetricfog = new ComboBox();
            label4 = new Label();
            cmbObjects = new ComboBox();
            label5 = new Label();
            cmbGravidar = new ComboBox();
            label6 = new Label();
            cmbPositional = new ComboBox();
            BtnAdd = new Button();
            BtnCancel = new Button();
            label7 = new Label();
            cmbDebris = new ComboBox();
            SuspendLayout();
            // 
            // cmbAsteroids
            // 
            cmbAsteroids.Font = new Font("Segoe UI", 11F);
            cmbAsteroids.FormattingEnabled = true;
            cmbAsteroids.Location = new Point(11, 37);
            cmbAsteroids.Name = "cmbAsteroids";
            cmbAsteroids.Size = new Size(328, 28);
            cmbAsteroids.TabIndex = 0;
            cmbAsteroids.SelectedIndexChanged += CmbAsteroids_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(11, 13);
            label1.Name = "label1";
            label1.Size = new Size(75, 21);
            label1.TabIndex = 1;
            label1.Text = "Asteroids";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(11, 68);
            label2.Name = "label2";
            label2.Size = new Size(60, 21);
            label2.TabIndex = 3;
            label2.Text = "Nebula";
            // 
            // cmbNebula
            // 
            cmbNebula.Font = new Font("Segoe UI", 11F);
            cmbNebula.FormattingEnabled = true;
            cmbNebula.Location = new Point(11, 92);
            cmbNebula.Name = "cmbNebula";
            cmbNebula.Size = new Size(328, 28);
            cmbNebula.TabIndex = 2;
            cmbNebula.SelectedIndexChanged += CmbNebula_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(11, 123);
            label3.Name = "label3";
            label3.Size = new Size(115, 21);
            label3.TabIndex = 5;
            label3.Text = "Volumetric Fog";
            // 
            // cmbVolumetricfog
            // 
            cmbVolumetricfog.Font = new Font("Segoe UI", 11F);
            cmbVolumetricfog.FormattingEnabled = true;
            cmbVolumetricfog.Location = new Point(11, 147);
            cmbVolumetricfog.Name = "cmbVolumetricfog";
            cmbVolumetricfog.Size = new Size(328, 28);
            cmbVolumetricfog.TabIndex = 4;
            cmbVolumetricfog.SelectedIndexChanged += CmbVolumetricfog_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(11, 178);
            label4.Name = "label4";
            label4.Size = new Size(62, 21);
            label4.TabIndex = 7;
            label4.Text = "Objects";
            // 
            // cmbObjects
            // 
            cmbObjects.Font = new Font("Segoe UI", 11F);
            cmbObjects.FormattingEnabled = true;
            cmbObjects.Location = new Point(11, 202);
            cmbObjects.Name = "cmbObjects";
            cmbObjects.Size = new Size(328, 28);
            cmbObjects.TabIndex = 6;
            cmbObjects.SelectedIndexChanged += CmbObjects_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(11, 233);
            label5.Name = "label5";
            label5.Size = new Size(70, 21);
            label5.TabIndex = 9;
            label5.Text = "Gravidar";
            // 
            // cmbGravidar
            // 
            cmbGravidar.Font = new Font("Segoe UI", 11F);
            cmbGravidar.FormattingEnabled = true;
            cmbGravidar.Location = new Point(11, 257);
            cmbGravidar.Name = "cmbGravidar";
            cmbGravidar.Size = new Size(328, 28);
            cmbGravidar.TabIndex = 8;
            cmbGravidar.SelectedIndexChanged += CmbGravidar_SelectedIndexChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(11, 288);
            label6.Name = "label6";
            label6.Size = new Size(77, 21);
            label6.TabIndex = 11;
            label6.Text = "Positional";
            // 
            // cmbPositional
            // 
            cmbPositional.Font = new Font("Segoe UI", 11F);
            cmbPositional.FormattingEnabled = true;
            cmbPositional.Location = new Point(11, 312);
            cmbPositional.Name = "cmbPositional";
            cmbPositional.Size = new Size(328, 28);
            cmbPositional.TabIndex = 10;
            cmbPositional.SelectedIndexChanged += CmbPositional_SelectedIndexChanged;
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(138, 401);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(201, 36);
            BtnAdd.TabIndex = 12;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 401);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(120, 36);
            BtnCancel.TabIndex = 13;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(12, 343);
            label7.Name = "label7";
            label7.Size = new Size(55, 21);
            label7.TabIndex = 15;
            label7.Text = "Debris";
            // 
            // cmbDebris
            // 
            cmbDebris.Font = new Font("Segoe UI", 11F);
            cmbDebris.FormattingEnabled = true;
            cmbDebris.Location = new Point(11, 367);
            cmbDebris.Name = "cmbDebris";
            cmbDebris.Size = new Size(328, 28);
            cmbDebris.TabIndex = 14;
            cmbDebris.SelectedIndexChanged += CmbDebris_SelectedIndexChanged;
            // 
            // RegionPredefinedFieldsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(351, 444);
            Controls.Add(label7);
            Controls.Add(cmbDebris);
            Controls.Add(BtnCancel);
            Controls.Add(BtnAdd);
            Controls.Add(label6);
            Controls.Add(cmbPositional);
            Controls.Add(label5);
            Controls.Add(cmbGravidar);
            Controls.Add(label4);
            Controls.Add(cmbObjects);
            Controls.Add(label3);
            Controls.Add(cmbVolumetricfog);
            Controls.Add(label2);
            Controls.Add(cmbNebula);
            Controls.Add(label1);
            Controls.Add(cmbAsteroids);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionPredefinedFieldsForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbAsteroids;
        private Label label1;
        private Label label2;
        private ComboBox cmbNebula;
        private Label label3;
        private ComboBox cmbVolumetricfog;
        private Label label4;
        private ComboBox cmbObjects;
        private Label label5;
        private ComboBox cmbGravidar;
        private Label label6;
        private ComboBox cmbPositional;
        private Button BtnAdd;
        private Button BtnCancel;
        private Label label7;
        private ComboBox cmbDebris;
    }
}