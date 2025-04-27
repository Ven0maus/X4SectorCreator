namespace X4SectorCreator.Forms
{
    partial class QuickQuotaEditorForm
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
            QuotaView = new DataGridView();
            BtnSave = new Button();
            BtnCancel = new Button();
            BtnResetFilter = new Button();
            cmbSector = new ComboBox();
            label8 = new Label();
            cmbCluster = new ComboBox();
            label7 = new Label();
            label3 = new Label();
            label2 = new Label();
            cmbFaction = new ComboBox();
            label1 = new Label();
            TxtSearch = new TextBox();
            Column1 = new DataGridViewTextBoxColumn();
            Galaxy = new DataGridViewTextBoxColumn();
            Cluster = new DataGridViewTextBoxColumn();
            Sector = new DataGridViewTextBoxColumn();
            MaxGalaxy = new DataGridViewTextBoxColumn();
            Wing = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)QuotaView).BeginInit();
            SuspendLayout();
            // 
            // QuotaView
            // 
            QuotaView.AllowUserToAddRows = false;
            QuotaView.AllowUserToDeleteRows = false;
            QuotaView.AllowUserToOrderColumns = true;
            QuotaView.AllowUserToResizeRows = false;
            QuotaView.BorderStyle = BorderStyle.Fixed3D;
            QuotaView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            QuotaView.Columns.AddRange(new DataGridViewColumn[] { Column1, Galaxy, Cluster, Sector, MaxGalaxy, Wing });
            QuotaView.Location = new Point(12, 42);
            QuotaView.Name = "QuotaView";
            QuotaView.Size = new Size(601, 535);
            QuotaView.TabIndex = 0;
            // 
            // BtnSave
            // 
            BtnSave.Location = new Point(619, 166);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new Size(289, 52);
            BtnSave.TabIndex = 1;
            BtnSave.Text = "Save Changes";
            BtnSave.UseVisualStyleBackColor = true;
            BtnSave.Click += BtnSave_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(619, 130);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(289, 30);
            BtnCancel.TabIndex = 2;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnResetFilter
            // 
            BtnResetFilter.Location = new Point(777, 12);
            BtnResetFilter.Name = "BtnResetFilter";
            BtnResetFilter.Size = new Size(131, 25);
            BtnResetFilter.TabIndex = 27;
            BtnResetFilter.Text = "Reset Filter";
            BtnResetFilter.UseVisualStyleBackColor = true;
            BtnResetFilter.Click += BtnResetFilter_Click;
            // 
            // cmbSector
            // 
            cmbSector.Enabled = false;
            cmbSector.FormattingEnabled = true;
            cmbSector.Location = new Point(687, 101);
            cmbSector.Name = "cmbSector";
            cmbSector.Size = new Size(221, 23);
            cmbSector.TabIndex = 26;
            cmbSector.SelectedIndexChanged += cmbSector_SelectedIndexChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(624, 100);
            label8.Name = "label8";
            label8.Size = new Size(57, 21);
            label8.TabIndex = 25;
            label8.Text = "Sector:";
            // 
            // cmbCluster
            // 
            cmbCluster.FormattingEnabled = true;
            cmbCluster.Location = new Point(687, 72);
            cmbCluster.Name = "cmbCluster";
            cmbCluster.Size = new Size(221, 23);
            cmbCluster.TabIndex = 24;
            cmbCluster.SelectedIndexChanged += cmbCluster_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(619, 72);
            label7.Name = "label7";
            label7.Size = new Size(62, 21);
            label7.TabIndex = 23;
            label7.Text = "Cluster:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(619, 42);
            label3.Name = "label3";
            label3.Size = new Size(62, 21);
            label3.TabIndex = 22;
            label3.Text = "Faction:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14F, FontStyle.Underline);
            label2.Location = new Point(619, 12);
            label2.Name = "label2";
            label2.Size = new Size(152, 25);
            label2.TabIndex = 21;
            label2.Text = "Filtering Options";
            // 
            // cmbFaction
            // 
            cmbFaction.FormattingEnabled = true;
            cmbFaction.Location = new Point(687, 44);
            cmbFaction.Name = "cmbFaction";
            cmbFaction.Size = new Size(221, 23);
            cmbFaction.TabIndex = 20;
            cmbFaction.SelectedIndexChanged += cmbFaction_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(68, 25);
            label1.TabIndex = 28;
            label1.Text = "Search:";
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(86, 12);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.Size = new Size(526, 23);
            TxtSearch.TabIndex = 29;
            // 
            // Column1
            // 
            Column1.FillWeight = 200F;
            Column1.HeaderText = "Name";
            Column1.MinimumWidth = 175;
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 175;
            // 
            // Galaxy
            // 
            Galaxy.FillWeight = 75F;
            Galaxy.HeaderText = "Galaxy";
            Galaxy.Name = "Galaxy";
            Galaxy.Width = 75;
            // 
            // Cluster
            // 
            Cluster.FillWeight = 75F;
            Cluster.HeaderText = "Cluster";
            Cluster.Name = "Cluster";
            Cluster.Width = 75;
            // 
            // Sector
            // 
            Sector.FillWeight = 75F;
            Sector.HeaderText = "Sector";
            Sector.Name = "Sector";
            Sector.Width = 75;
            // 
            // MaxGalaxy
            // 
            MaxGalaxy.FillWeight = 75F;
            MaxGalaxy.HeaderText = "MaxGalaxy";
            MaxGalaxy.Name = "MaxGalaxy";
            MaxGalaxy.Width = 75;
            // 
            // Wing
            // 
            Wing.FillWeight = 75F;
            Wing.HeaderText = "Wing";
            Wing.Name = "Wing";
            Wing.Width = 75;
            // 
            // QuickQuotaEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(920, 585);
            Controls.Add(TxtSearch);
            Controls.Add(label1);
            Controls.Add(BtnResetFilter);
            Controls.Add(cmbSector);
            Controls.Add(label8);
            Controls.Add(cmbCluster);
            Controls.Add(label7);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(cmbFaction);
            Controls.Add(BtnCancel);
            Controls.Add(BtnSave);
            Controls.Add(QuotaView);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "QuickQuotaEditorForm";
            Text = "Quick Quota Editor";
            ((System.ComponentModel.ISupportInitialize)QuotaView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView QuotaView;
        private Button BtnSave;
        private Button BtnCancel;
        private Button BtnResetFilter;
        private ComboBox cmbSector;
        private Label label8;
        private ComboBox cmbCluster;
        private Label label7;
        private Label label3;
        private Label label2;
        private ComboBox cmbFaction;
        private Label label1;
        private TextBox TxtSearch;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Galaxy;
        private DataGridViewTextBoxColumn Cluster;
        private DataGridViewTextBoxColumn Sector;
        private DataGridViewTextBoxColumn MaxGalaxy;
        private DataGridViewTextBoxColumn Wing;
    }
}