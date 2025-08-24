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
            SuspendLayout();
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(167, 351);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(236, 30);
            BtnCreate.TabIndex = 0;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 351);
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
            label6.Location = new Point(69, 290);
            label6.Name = "label6";
            label6.Size = new Size(92, 21);
            label6.TabIndex = 34;
            label6.Text = "Soundtrack:";
            // 
            // TxtSoundtrack
            // 
            TxtSoundtrack.Location = new Point(167, 290);
            TxtSoundtrack.Name = "TxtSoundtrack";
            TxtSoundtrack.PlaceholderText = "Select..";
            TxtSoundtrack.ReadOnly = true;
            TxtSoundtrack.Size = new Size(236, 23);
            TxtSoundtrack.TabIndex = 37;
            TxtSoundtrack.MouseClick += TxtSoundtrack_MouseClick;
            // 
            // BtnEditClusterXml
            // 
            BtnEditClusterXml.Location = new Point(167, 319);
            BtnEditClusterXml.Name = "BtnEditClusterXml";
            BtnEditClusterXml.Size = new Size(236, 30);
            BtnEditClusterXml.TabIndex = 38;
            BtnEditClusterXml.Text = "Edit Cluster XML (Advanced)";
            BtnEditClusterXml.UseVisualStyleBackColor = true;
            BtnEditClusterXml.Click += BtnEditClusterXml_Click;
            // 
            // ClusterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(415, 385);
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
    }
}