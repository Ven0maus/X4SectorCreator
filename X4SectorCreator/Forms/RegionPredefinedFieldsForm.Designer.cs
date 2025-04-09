using X4SectorCreator.CustomComponents;
using X4SectorCreator.Objects;

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
            label1 = new Label();
            label2 = new Label();
            cmbNebula = new MultiSelectCombo.NoDropDownComboBox();
            label3 = new Label();
            cmbVolumetricfog = new MultiSelectCombo.NoDropDownComboBox();
            label4 = new Label();
            cmbObjects = new MultiSelectCombo.NoDropDownComboBox();
            label5 = new Label();
            cmbGravidar = new MultiSelectCombo.NoDropDownComboBox();
            label6 = new Label();
            cmbPositional = new MultiSelectCombo.NoDropDownComboBox();
            BtnAdd = new Button();
            BtnCancel = new Button();
            label7 = new Label();
            cmbDebris = new MultiSelectCombo.NoDropDownComboBox();
            label8 = new Label();
            cmbAmbientSound = new MultiSelectCombo.NoDropDownComboBox();
            cmbAsteroids = new MultiSelectCombo.NoDropDownComboBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(11, 13);
            label1.Name = "label1";
            label1.Size = new Size(149, 21);
            label1.TabIndex = 1;
            label1.Text = "Asteroids (Minerals)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(11, 68);
            label2.Name = "label2";
            label2.Size = new Size(115, 21);
            label2.TabIndex = 3;
            label2.Text = "Nebula (Gases)";
            // 
            // cmbNebula
            // 
            cmbNebula.Font = new Font("Segoe UI", 11F);
            cmbNebula.FormattingEnabled = true;
            cmbNebula.Location = new Point(11, 92);
            cmbNebula.Name = "cmbNebula";
            cmbNebula.Size = new Size(328, 28);
            cmbNebula.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(11, 123);
            label3.Name = "label3";
            label3.Size = new Size(198, 21);
            label3.TabIndex = 5;
            label3.Text = "Volumetric Fog (Visual fog)";
            // 
            // cmbVolumetricfog
            // 
            cmbVolumetricfog.Font = new Font("Segoe UI", 11F);
            cmbVolumetricfog.FormattingEnabled = true;
            cmbVolumetricfog.Location = new Point(11, 147);
            cmbVolumetricfog.Name = "cmbVolumetricfog";
            cmbVolumetricfog.Size = new Size(328, 28);
            cmbVolumetricfog.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(11, 178);
            label4.Name = "label4";
            label4.Size = new Size(148, 21);
            label4.TabIndex = 7;
            label4.Text = "Objects (Lockboxes)";
            // 
            // cmbObjects
            // 
            cmbObjects.Font = new Font("Segoe UI", 11F);
            cmbObjects.FormattingEnabled = true;
            cmbObjects.Location = new Point(11, 202);
            cmbObjects.Name = "cmbObjects";
            cmbObjects.Size = new Size(328, 28);
            cmbObjects.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(11, 233);
            label5.Name = "label5";
            label5.Size = new Size(196, 21);
            label5.TabIndex = 9;
            label5.Text = "Gravidar (Radar distortion)";
            // 
            // cmbGravidar
            // 
            cmbGravidar.Font = new Font("Segoe UI", 11F);
            cmbGravidar.FormattingEnabled = true;
            cmbGravidar.Location = new Point(11, 257);
            cmbGravidar.Name = "cmbGravidar";
            cmbGravidar.Size = new Size(328, 28);
            cmbGravidar.TabIndex = 8;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(11, 288);
            label6.Name = "label6";
            label6.Size = new Size(140, 21);
            label6.TabIndex = 11;
            label6.Text = "Positional (Visuals)";
            // 
            // cmbPositional
            // 
            cmbPositional.Font = new Font("Segoe UI", 11F);
            cmbPositional.FormattingEnabled = true;
            cmbPositional.Location = new Point(11, 312);
            cmbPositional.Name = "cmbPositional";
            cmbPositional.Size = new Size(328, 28);
            cmbPositional.TabIndex = 10;
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(138, 456);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(201, 36);
            BtnAdd.TabIndex = 12;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 456);
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
            label7.Size = new Size(120, 21);
            label7.TabIndex = 15;
            label7.Text = "Debris (Wrecks)";
            // 
            // cmbDebris
            // 
            cmbDebris.Font = new Font("Segoe UI", 11F);
            cmbDebris.FormattingEnabled = true;
            cmbDebris.Location = new Point(11, 367);
            cmbDebris.Name = "cmbDebris";
            cmbDebris.Size = new Size(328, 28);
            cmbDebris.TabIndex = 14;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(12, 398);
            label8.Name = "label8";
            label8.Size = new Size(114, 21);
            label8.TabIndex = 17;
            label8.Text = "AmbientSound";
            // 
            // cmbAmbientSound
            // 
            cmbAmbientSound.Font = new Font("Segoe UI", 11F);
            cmbAmbientSound.FormattingEnabled = true;
            cmbAmbientSound.Location = new Point(11, 422);
            cmbAmbientSound.Name = "cmbAmbientSound";
            cmbAmbientSound.Size = new Size(328, 28);
            cmbAmbientSound.TabIndex = 16;
            // 
            // cmbAsteroids
            // 
            cmbAsteroids.Font = new Font("Segoe UI", 11F);
            cmbAsteroids.FormattingEnabled = true;
            cmbAsteroids.Location = new Point(11, 37);
            cmbAsteroids.Name = "cmbAsteroids";
            cmbAsteroids.Size = new Size(328, 28);
            cmbAsteroids.TabIndex = 18;
            // 
            // RegionPredefinedFieldsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(349, 498);
            Controls.Add(cmbAsteroids);
            Controls.Add(label8);
            Controls.Add(cmbAmbientSound);
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionPredefinedFieldsForm";
            Text = "Region Predefined Fields Selector";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private MultiSelectCombo.NoDropDownComboBox cmbNebula;
        private Label label3;
        private MultiSelectCombo.NoDropDownComboBox cmbVolumetricfog;
        private Label label4;
        private MultiSelectCombo.NoDropDownComboBox cmbObjects;
        private Label label5;
        private MultiSelectCombo.NoDropDownComboBox cmbGravidar;
        private Label label6;
        private MultiSelectCombo.NoDropDownComboBox cmbPositional;
        private Button BtnAdd;
        private Button BtnCancel;
        private Label label7;
        private MultiSelectCombo.NoDropDownComboBox cmbDebris;
        private Label label8;
        private MultiSelectCombo.NoDropDownComboBox cmbAmbientSound;
        private MultiSelectCombo.NoDropDownComboBox cmbAsteroids;
    }
}