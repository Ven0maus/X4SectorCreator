namespace X4SectorCreator.Forms
{
    partial class GalaxySettingsForm
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
            chkCustomGalaxy = new CheckBox();
            txtGalaxyName = new TextBox();
            BtnSave = new Button();
            BtnCancel = new Button();
            chkDisableAllStorylines = new CheckBox();
            label2 = new Label();
            cmbStartSector = new ComboBox();
            LblStartingSector = new Label();
            BtnGenerateProceduralGalaxy = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 42);
            label1.Name = "label1";
            label1.Size = new Size(105, 21);
            label1.TabIndex = 0;
            label1.Text = "Galaxy Name:";
            // 
            // chkCustomGalaxy
            // 
            chkCustomGalaxy.AutoSize = true;
            chkCustomGalaxy.Location = new Point(255, 12);
            chkCustomGalaxy.Name = "chkCustomGalaxy";
            chkCustomGalaxy.Size = new Size(105, 19);
            chkCustomGalaxy.TabIndex = 1;
            chkCustomGalaxy.Text = "Custom Galaxy";
            chkCustomGalaxy.UseVisualStyleBackColor = true;
            chkCustomGalaxy.CheckedChanged += ChkCustomGalaxy_CheckedChanged;
            // 
            // txtGalaxyName
            // 
            txtGalaxyName.Enabled = false;
            txtGalaxyName.Location = new Point(119, 42);
            txtGalaxyName.Name = "txtGalaxyName";
            txtGalaxyName.Size = new Size(241, 23);
            txtGalaxyName.TabIndex = 2;
            txtGalaxyName.Text = "xu_ep2_universe";
            txtGalaxyName.KeyPress += TxtGalaxyName_KeyPress;
            // 
            // BtnSave
            // 
            BtnSave.Location = new Point(119, 136);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new Size(241, 33);
            BtnSave.TabIndex = 3;
            BtnSave.Text = "Save";
            BtnSave.UseVisualStyleBackColor = true;
            BtnSave.Click += BtnSave_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 136);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(98, 33);
            BtnCancel.TabIndex = 4;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // chkDisableAllStorylines
            // 
            chkDisableAllStorylines.AutoSize = true;
            chkDisableAllStorylines.Location = new Point(12, 12);
            chkDisableAllStorylines.Name = "chkDisableAllStorylines";
            chkDisableAllStorylines.Size = new Size(132, 19);
            chkDisableAllStorylines.TabIndex = 5;
            chkDisableAllStorylines.Text = "Disable all storylines";
            chkDisableAllStorylines.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(2, 75);
            label2.Name = "label2";
            label2.Size = new Size(115, 21);
            label2.TabIndex = 6;
            label2.Text = "Starting Sector:";
            // 
            // cmbStartSector
            // 
            cmbStartSector.Enabled = false;
            cmbStartSector.FormattingEnabled = true;
            cmbStartSector.Location = new Point(119, 75);
            cmbStartSector.Name = "cmbStartSector";
            cmbStartSector.Size = new Size(241, 23);
            cmbStartSector.TabIndex = 7;
            // 
            // LblStartingSector
            // 
            LblStartingSector.Font = new Font("Segoe UI", 9F);
            LblStartingSector.Location = new Point(119, 100);
            LblStartingSector.Name = "LblStartingSector";
            LblStartingSector.Size = new Size(247, 32);
            LblStartingSector.TabIndex = 8;
            LblStartingSector.Text = "(Selection becomes active for Custom Galaxy once atleast one custom sector exists.)";
            // 
            // BtnGenerateProceduralGalaxy
            // 
            BtnGenerateProceduralGalaxy.Location = new Point(12, 172);
            BtnGenerateProceduralGalaxy.Name = "BtnGenerateProceduralGalaxy";
            BtnGenerateProceduralGalaxy.Size = new Size(348, 33);
            BtnGenerateProceduralGalaxy.TabIndex = 9;
            BtnGenerateProceduralGalaxy.Text = "Generate Procedural Galaxy";
            BtnGenerateProceduralGalaxy.UseVisualStyleBackColor = true;
            BtnGenerateProceduralGalaxy.Click += BtnGenerateProceduralGalaxy_Click;
            // 
            // GalaxySettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(366, 209);
            Controls.Add(BtnGenerateProceduralGalaxy);
            Controls.Add(LblStartingSector);
            Controls.Add(cmbStartSector);
            Controls.Add(label2);
            Controls.Add(chkDisableAllStorylines);
            Controls.Add(BtnCancel);
            Controls.Add(BtnSave);
            Controls.Add(txtGalaxyName);
            Controls.Add(chkCustomGalaxy);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GalaxySettingsForm";
            Text = "Galaxy Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private CheckBox chkCustomGalaxy;
        private TextBox txtGalaxyName;
        private Button BtnSave;
        private Button BtnCancel;
        private CheckBox chkDisableAllStorylines;
        private Label label2;
        private ComboBox cmbStartSector;
        private Label LblStartingSector;
        private Button BtnGenerateProceduralGalaxy;
    }
}