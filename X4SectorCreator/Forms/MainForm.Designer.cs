namespace X4SectorCreator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BtnRemoveCluster = new Button();
            ClustersListBox = new ListBox();
            lblClusters = new Label();
            BtnNewCluster = new Button();
            BtnNewSector = new Button();
            SectorsListBox = new ListBox();
            lblSectors = new Label();
            BtnRemoveSector = new Button();
            BtnGenerateDiffs = new Button();
            BtnNewGate = new Button();
            GatesListBox = new ListBox();
            lblGates = new Label();
            BtnRemoveGate = new Button();
            lblSelectionInfo = new Label();
            LblDetails = new Label();
            BtnExportConfig = new Button();
            BtnImportConfig = new Button();
            BtnGalaxySettings = new Button();
            BtnReset = new Button();
            BtnShowSectorMap = new Button();
            BtnOpenFolder = new Button();
            BtnNewRegion = new Button();
            RegionsListBox = new ListBox();
            label1 = new Label();
            BtnRemoveRegion = new Button();
            SuspendLayout();
            // 
            // BtnRemoveCluster
            // 
            BtnRemoveCluster.Location = new Point(93, 250);
            BtnRemoveCluster.Name = "BtnRemoveCluster";
            BtnRemoveCluster.Size = new Size(75, 23);
            BtnRemoveCluster.TabIndex = 0;
            BtnRemoveCluster.Text = "Remove";
            BtnRemoveCluster.UseVisualStyleBackColor = true;
            BtnRemoveCluster.Click += BtnRemoveCluster_Click;
            // 
            // ClustersListBox
            // 
            ClustersListBox.FormattingEnabled = true;
            ClustersListBox.Location = new Point(12, 75);
            ClustersListBox.Name = "ClustersListBox";
            ClustersListBox.Size = new Size(156, 169);
            ClustersListBox.TabIndex = 8;
            ClustersListBox.SelectedIndexChanged += ClustersListBox_SelectedIndexChanged;
            ClustersListBox.DoubleClick += ClustersListBox_DoubleClick;
            // 
            // lblClusters
            // 
            lblClusters.AutoSize = true;
            lblClusters.Font = new Font("Segoe UI", 15F);
            lblClusters.Location = new Point(12, 44);
            lblClusters.Name = "lblClusters";
            lblClusters.Size = new Size(80, 28);
            lblClusters.TabIndex = 7;
            lblClusters.Text = "Clusters";
            // 
            // BtnNewCluster
            // 
            BtnNewCluster.Location = new Point(12, 250);
            BtnNewCluster.Name = "BtnNewCluster";
            BtnNewCluster.Size = new Size(75, 23);
            BtnNewCluster.TabIndex = 9;
            BtnNewCluster.Text = "New";
            BtnNewCluster.UseVisualStyleBackColor = true;
            BtnNewCluster.Click += BtnNewCluster_Click;
            // 
            // BtnNewSector
            // 
            BtnNewSector.Location = new Point(174, 250);
            BtnNewSector.Name = "BtnNewSector";
            BtnNewSector.Size = new Size(75, 23);
            BtnNewSector.TabIndex = 13;
            BtnNewSector.Text = "New";
            BtnNewSector.UseVisualStyleBackColor = true;
            BtnNewSector.Click += BtnNewSector_Click;
            // 
            // SectorsListBox
            // 
            SectorsListBox.FormattingEnabled = true;
            SectorsListBox.Location = new Point(174, 75);
            SectorsListBox.Name = "SectorsListBox";
            SectorsListBox.Size = new Size(156, 169);
            SectorsListBox.TabIndex = 12;
            SectorsListBox.SelectedIndexChanged += SectorsListBox_SelectedIndexChanged;
            SectorsListBox.DoubleClick += SectorsListBox_DoubleClick;
            // 
            // lblSectors
            // 
            lblSectors.AutoSize = true;
            lblSectors.Font = new Font("Segoe UI", 15F);
            lblSectors.Location = new Point(174, 44);
            lblSectors.Name = "lblSectors";
            lblSectors.Size = new Size(76, 28);
            lblSectors.TabIndex = 11;
            lblSectors.Text = "Sectors";
            // 
            // BtnRemoveSector
            // 
            BtnRemoveSector.Location = new Point(255, 250);
            BtnRemoveSector.Name = "BtnRemoveSector";
            BtnRemoveSector.Size = new Size(75, 23);
            BtnRemoveSector.TabIndex = 10;
            BtnRemoveSector.Text = "Remove";
            BtnRemoveSector.UseVisualStyleBackColor = true;
            BtnRemoveSector.Click += BtnRemoveSector_Click;
            // 
            // BtnGenerateDiffs
            // 
            BtnGenerateDiffs.Location = new Point(336, 511);
            BtnGenerateDiffs.Name = "BtnGenerateDiffs";
            BtnGenerateDiffs.Size = new Size(156, 23);
            BtnGenerateDiffs.TabIndex = 18;
            BtnGenerateDiffs.Text = "Generate XML";
            BtnGenerateDiffs.UseVisualStyleBackColor = true;
            BtnGenerateDiffs.Click += BtnGenerateDiffs_Click;
            // 
            // BtnNewGate
            // 
            BtnNewGate.Location = new Point(336, 250);
            BtnNewGate.Name = "BtnNewGate";
            BtnNewGate.Size = new Size(75, 23);
            BtnNewGate.TabIndex = 22;
            BtnNewGate.Text = "New";
            BtnNewGate.UseVisualStyleBackColor = true;
            BtnNewGate.Click += BtnNewGate_Click;
            // 
            // GatesListBox
            // 
            GatesListBox.FormattingEnabled = true;
            GatesListBox.Location = new Point(336, 75);
            GatesListBox.Name = "GatesListBox";
            GatesListBox.Size = new Size(156, 169);
            GatesListBox.TabIndex = 21;
            GatesListBox.DoubleClick += GatesListBox_DoubleClick;
            // 
            // lblGates
            // 
            lblGates.AutoSize = true;
            lblGates.Font = new Font("Segoe UI", 15F);
            lblGates.Location = new Point(336, 44);
            lblGates.Name = "lblGates";
            lblGates.Size = new Size(120, 28);
            lblGates.TabIndex = 20;
            lblGates.Text = "Connections";
            // 
            // BtnRemoveGate
            // 
            BtnRemoveGate.Location = new Point(417, 250);
            BtnRemoveGate.Name = "BtnRemoveGate";
            BtnRemoveGate.Size = new Size(75, 23);
            BtnRemoveGate.TabIndex = 19;
            BtnRemoveGate.Text = "Remove";
            BtnRemoveGate.UseVisualStyleBackColor = true;
            BtnRemoveGate.Click += BtnRemoveGate_Click;
            // 
            // lblSelectionInfo
            // 
            lblSelectionInfo.AutoSize = true;
            lblSelectionInfo.Font = new Font("Segoe UI", 15F, FontStyle.Underline);
            lblSelectionInfo.Location = new Point(12, 276);
            lblSelectionInfo.Name = "lblSelectionInfo";
            lblSelectionInfo.Size = new Size(80, 28);
            lblSelectionInfo.TabIndex = 24;
            lblSelectionInfo.Text = "Details: ";
            // 
            // LblDetails
            // 
            LblDetails.BackColor = SystemColors.ButtonHighlight;
            LblDetails.BorderStyle = BorderStyle.FixedSingle;
            LblDetails.Font = new Font("Segoe UI", 12F);
            LblDetails.Location = new Point(12, 307);
            LblDetails.Name = "LblDetails";
            LblDetails.Size = new Size(318, 256);
            LblDetails.TabIndex = 25;
            // 
            // BtnExportConfig
            // 
            BtnExportConfig.Location = new Point(272, 10);
            BtnExportConfig.Name = "BtnExportConfig";
            BtnExportConfig.Size = new Size(107, 31);
            BtnExportConfig.TabIndex = 29;
            BtnExportConfig.Text = "Export Config";
            BtnExportConfig.UseVisualStyleBackColor = true;
            BtnExportConfig.Click += BtnExportConfig_Click;
            // 
            // BtnImportConfig
            // 
            BtnImportConfig.Location = new Point(385, 10);
            BtnImportConfig.Name = "BtnImportConfig";
            BtnImportConfig.Size = new Size(107, 31);
            BtnImportConfig.TabIndex = 30;
            BtnImportConfig.Text = "Import Config";
            BtnImportConfig.UseVisualStyleBackColor = true;
            BtnImportConfig.Click += BtnImportConfig_Click;
            // 
            // BtnGalaxySettings
            // 
            BtnGalaxySettings.Location = new Point(12, 10);
            BtnGalaxySettings.Name = "BtnGalaxySettings";
            BtnGalaxySettings.Size = new Size(141, 31);
            BtnGalaxySettings.TabIndex = 31;
            BtnGalaxySettings.Text = "Galaxy Settings";
            BtnGalaxySettings.UseVisualStyleBackColor = true;
            BtnGalaxySettings.Click += BtnGalaxySettings_Click;
            // 
            // BtnReset
            // 
            BtnReset.Location = new Point(159, 10);
            BtnReset.Name = "BtnReset";
            BtnReset.Size = new Size(107, 31);
            BtnReset.TabIndex = 32;
            BtnReset.Text = "Reset Config";
            BtnReset.UseVisualStyleBackColor = true;
            BtnReset.Click += BtnReset_Click;
            // 
            // BtnShowSectorMap
            // 
            BtnShowSectorMap.Location = new Point(93, 276);
            BtnShowSectorMap.Name = "BtnShowSectorMap";
            BtnShowSectorMap.Size = new Size(237, 28);
            BtnShowSectorMap.TabIndex = 33;
            BtnShowSectorMap.Text = "Show Sector Map";
            BtnShowSectorMap.UseVisualStyleBackColor = true;
            BtnShowSectorMap.Click += BtnShowSectorMap_Click;
            // 
            // BtnOpenFolder
            // 
            BtnOpenFolder.Location = new Point(336, 540);
            BtnOpenFolder.Name = "BtnOpenFolder";
            BtnOpenFolder.Size = new Size(156, 23);
            BtnOpenFolder.TabIndex = 34;
            BtnOpenFolder.Text = "Open XML Folder";
            BtnOpenFolder.UseVisualStyleBackColor = true;
            BtnOpenFolder.Click += BtnOpenFolder_Click;
            // 
            // BtnNewRegion
            // 
            BtnNewRegion.Enabled = false;
            BtnNewRegion.Location = new Point(335, 482);
            BtnNewRegion.Name = "BtnNewRegion";
            BtnNewRegion.Size = new Size(75, 23);
            BtnNewRegion.TabIndex = 38;
            BtnNewRegion.Text = "New";
            BtnNewRegion.UseVisualStyleBackColor = true;
            // 
            // RegionsListBox
            // 
            RegionsListBox.FormattingEnabled = true;
            RegionsListBox.Location = new Point(335, 307);
            RegionsListBox.Name = "RegionsListBox";
            RegionsListBox.Size = new Size(156, 169);
            RegionsListBox.TabIndex = 37;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(335, 276);
            label1.Name = "label1";
            label1.Size = new Size(81, 28);
            label1.TabIndex = 36;
            label1.Text = "Regions";
            // 
            // BtnRemoveRegion
            // 
            BtnRemoveRegion.Enabled = false;
            BtnRemoveRegion.Location = new Point(416, 482);
            BtnRemoveRegion.Name = "BtnRemoveRegion";
            BtnRemoveRegion.Size = new Size(75, 23);
            BtnRemoveRegion.TabIndex = 35;
            BtnRemoveRegion.Text = "Remove";
            BtnRemoveRegion.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(503, 570);
            Controls.Add(BtnNewRegion);
            Controls.Add(RegionsListBox);
            Controls.Add(label1);
            Controls.Add(BtnRemoveRegion);
            Controls.Add(BtnOpenFolder);
            Controls.Add(BtnShowSectorMap);
            Controls.Add(BtnReset);
            Controls.Add(BtnGalaxySettings);
            Controls.Add(BtnImportConfig);
            Controls.Add(BtnExportConfig);
            Controls.Add(LblDetails);
            Controls.Add(lblSelectionInfo);
            Controls.Add(BtnNewGate);
            Controls.Add(GatesListBox);
            Controls.Add(lblGates);
            Controls.Add(BtnRemoveGate);
            Controls.Add(BtnGenerateDiffs);
            Controls.Add(BtnNewSector);
            Controls.Add(SectorsListBox);
            Controls.Add(lblSectors);
            Controls.Add(BtnRemoveSector);
            Controls.Add(BtnNewCluster);
            Controls.Add(ClustersListBox);
            Controls.Add(lblClusters);
            Controls.Add(BtnRemoveCluster);
            MaximizeBox = false;
            Name = "MainForm";
            Text = "X4 Sector Creator";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnRemoveCluster;
        private Label lblSelectionInfo;
        private Label lblClusters;
        private Button BtnNewCluster;
        private Button BtnNewSector;
        private Label lblSectors;
        private Button BtnRemoveSector;
        private Button BtnGenerateDiffs;
        private Button BtnNewGate;
        private Label lblGates;
        private Button BtnRemoveGate;
        private Label LblDetails;
        private Button BtnExportConfig;
        private Button BtnImportConfig;
        private Button BtnGalaxySettings;
        private Button BtnReset;
        private Button BtnShowSectorMap;
        internal ListBox ClustersListBox;
        internal ListBox SectorsListBox;
        internal ListBox GatesListBox;
        private Button BtnOpenFolder;
        private Button BtnNewRegion;
        internal ListBox RegionsListBox;
        private Label label1;
        private Button BtnRemoveRegion;
    }
}
