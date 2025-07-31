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
            cmbClusterOption = new ComboBox();
            BtnNewStation = new Button();
            ListStations = new ListBox();
            label2 = new Label();
            BtnRemoveStation = new Button();
            BtnJobs = new Button();
            BtnFactories = new Button();
            BtnObjectsOverview = new Button();
            TxtSearch = new TextBox();
            BtnCustomFactions = new Button();
            GenerateModProgressBar = new ProgressBar();
            BtnSaveSectorMapping = new Button();
            BtnGuide = new Button();
            SuspendLayout();
            // 
            // BtnRemoveCluster
            // 
            BtnRemoveCluster.Location = new Point(94, 279);
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
            ClustersListBox.HorizontalScrollbar = true;
            ClustersListBox.Location = new Point(13, 104);
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
            BtnNewCluster.Location = new Point(13, 279);
            BtnNewCluster.Name = "BtnNewCluster";
            BtnNewCluster.Size = new Size(75, 23);
            BtnNewCluster.TabIndex = 9;
            BtnNewCluster.Text = "New";
            BtnNewCluster.UseVisualStyleBackColor = true;
            BtnNewCluster.Click += BtnNewCluster_Click;
            // 
            // BtnNewSector
            // 
            BtnNewSector.Location = new Point(174, 279);
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
            SectorsListBox.HorizontalScrollbar = true;
            SectorsListBox.Location = new Point(174, 75);
            SectorsListBox.Name = "SectorsListBox";
            SectorsListBox.Size = new Size(156, 199);
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
            BtnRemoveSector.Location = new Point(255, 279);
            BtnRemoveSector.Name = "BtnRemoveSector";
            BtnRemoveSector.Size = new Size(75, 23);
            BtnRemoveSector.TabIndex = 10;
            BtnRemoveSector.Text = "Remove";
            BtnRemoveSector.UseVisualStyleBackColor = true;
            BtnRemoveSector.Click += BtnRemoveSector_Click;
            // 
            // BtnGenerateDiffs
            // 
            BtnGenerateDiffs.Location = new Point(336, 540);
            BtnGenerateDiffs.Name = "BtnGenerateDiffs";
            BtnGenerateDiffs.Size = new Size(318, 23);
            BtnGenerateDiffs.TabIndex = 18;
            BtnGenerateDiffs.Text = "Generate MOD";
            BtnGenerateDiffs.UseVisualStyleBackColor = true;
            BtnGenerateDiffs.Click += BtnGenerateDiffs_Click;
            // 
            // BtnNewGate
            // 
            BtnNewGate.Location = new Point(336, 279);
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
            GatesListBox.HorizontalScrollbar = true;
            GatesListBox.Location = new Point(336, 75);
            GatesListBox.Name = "GatesListBox";
            GatesListBox.Size = new Size(156, 199);
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
            BtnRemoveGate.Location = new Point(417, 279);
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
            lblSelectionInfo.Location = new Point(12, 305);
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
            LblDetails.Location = new Point(12, 336);
            LblDetails.Name = "LblDetails";
            LblDetails.Size = new Size(318, 256);
            LblDetails.TabIndex = 25;
            // 
            // BtnExportConfig
            // 
            BtnExportConfig.Location = new Point(336, 10);
            BtnExportConfig.Name = "BtnExportConfig";
            BtnExportConfig.Size = new Size(156, 31);
            BtnExportConfig.TabIndex = 29;
            BtnExportConfig.Text = "Export Config";
            BtnExportConfig.UseVisualStyleBackColor = true;
            BtnExportConfig.Click += BtnExportConfig_Click;
            // 
            // BtnImportConfig
            // 
            BtnImportConfig.Location = new Point(498, 10);
            BtnImportConfig.Name = "BtnImportConfig";
            BtnImportConfig.Size = new Size(156, 31);
            BtnImportConfig.TabIndex = 30;
            BtnImportConfig.Text = "Import Config";
            BtnImportConfig.UseVisualStyleBackColor = true;
            BtnImportConfig.Click += BtnImportConfig_Click;
            // 
            // BtnGalaxySettings
            // 
            BtnGalaxySettings.Location = new Point(12, 10);
            BtnGalaxySettings.Name = "BtnGalaxySettings";
            BtnGalaxySettings.Size = new Size(156, 31);
            BtnGalaxySettings.TabIndex = 31;
            BtnGalaxySettings.Text = "Galaxy Settings";
            BtnGalaxySettings.UseVisualStyleBackColor = true;
            BtnGalaxySettings.Click += BtnGalaxySettings_Click;
            // 
            // BtnReset
            // 
            BtnReset.Location = new Point(174, 10);
            BtnReset.Name = "BtnReset";
            BtnReset.Size = new Size(156, 31);
            BtnReset.TabIndex = 32;
            BtnReset.Text = "Reset All";
            BtnReset.UseVisualStyleBackColor = true;
            BtnReset.Click += BtnReset_Click;
            // 
            // BtnShowSectorMap
            // 
            BtnShowSectorMap.Location = new Point(93, 305);
            BtnShowSectorMap.Name = "BtnShowSectorMap";
            BtnShowSectorMap.Size = new Size(237, 28);
            BtnShowSectorMap.TabIndex = 33;
            BtnShowSectorMap.Text = "Show Sector Map";
            BtnShowSectorMap.UseVisualStyleBackColor = true;
            BtnShowSectorMap.Click += BtnShowSectorMap_Click;
            // 
            // BtnOpenFolder
            // 
            BtnOpenFolder.Location = new Point(336, 569);
            BtnOpenFolder.Name = "BtnOpenFolder";
            BtnOpenFolder.Size = new Size(318, 23);
            BtnOpenFolder.TabIndex = 34;
            BtnOpenFolder.Text = "Open XML Folder";
            BtnOpenFolder.UseVisualStyleBackColor = true;
            BtnOpenFolder.Click += BtnOpenFolder_Click;
            // 
            // BtnNewRegion
            // 
            BtnNewRegion.Location = new Point(335, 511);
            BtnNewRegion.Name = "BtnNewRegion";
            BtnNewRegion.Size = new Size(75, 23);
            BtnNewRegion.TabIndex = 38;
            BtnNewRegion.Text = "New";
            BtnNewRegion.UseVisualStyleBackColor = true;
            BtnNewRegion.Click += BtnNewRegion_Click;
            // 
            // RegionsListBox
            // 
            RegionsListBox.FormattingEnabled = true;
            RegionsListBox.HorizontalScrollbar = true;
            RegionsListBox.Location = new Point(335, 336);
            RegionsListBox.Name = "RegionsListBox";
            RegionsListBox.Size = new Size(156, 169);
            RegionsListBox.TabIndex = 37;
            RegionsListBox.DoubleClick += RegionsListBox_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(335, 305);
            label1.Name = "label1";
            label1.Size = new Size(81, 28);
            label1.TabIndex = 36;
            label1.Text = "Regions";
            // 
            // BtnRemoveRegion
            // 
            BtnRemoveRegion.Location = new Point(416, 511);
            BtnRemoveRegion.Name = "BtnRemoveRegion";
            BtnRemoveRegion.Size = new Size(75, 23);
            BtnRemoveRegion.TabIndex = 35;
            BtnRemoveRegion.Text = "Remove";
            BtnRemoveRegion.UseVisualStyleBackColor = true;
            BtnRemoveRegion.Click += BtnRemoveRegion_Click;
            // 
            // cmbClusterOption
            // 
            cmbClusterOption.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbClusterOption.FormattingEnabled = true;
            cmbClusterOption.Items.AddRange(new object[] { "Custom", "Vanilla", "Both" });
            cmbClusterOption.Location = new Point(93, 49);
            cmbClusterOption.MaxDropDownItems = 3;
            cmbClusterOption.Name = "cmbClusterOption";
            cmbClusterOption.Size = new Size(75, 23);
            cmbClusterOption.TabIndex = 39;
            cmbClusterOption.SelectedIndexChanged += CmbClusterOption_SelectedIndexChanged;
            // 
            // BtnNewStation
            // 
            BtnNewStation.Location = new Point(498, 279);
            BtnNewStation.Name = "BtnNewStation";
            BtnNewStation.Size = new Size(75, 23);
            BtnNewStation.TabIndex = 43;
            BtnNewStation.Text = "New";
            BtnNewStation.UseVisualStyleBackColor = true;
            BtnNewStation.Click += BtnNewStation_Click;
            // 
            // ListStations
            // 
            ListStations.FormattingEnabled = true;
            ListStations.HorizontalScrollbar = true;
            ListStations.Location = new Point(498, 75);
            ListStations.Name = "ListStations";
            ListStations.Size = new Size(156, 199);
            ListStations.TabIndex = 42;
            ListStations.DoubleClick += ListStations_DoubleClick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 15F);
            label2.Location = new Point(498, 44);
            label2.Name = "label2";
            label2.Size = new Size(82, 28);
            label2.TabIndex = 41;
            label2.Text = "Stations";
            // 
            // BtnRemoveStation
            // 
            BtnRemoveStation.Location = new Point(579, 279);
            BtnRemoveStation.Name = "BtnRemoveStation";
            BtnRemoveStation.Size = new Size(75, 23);
            BtnRemoveStation.TabIndex = 40;
            BtnRemoveStation.Text = "Remove";
            BtnRemoveStation.UseVisualStyleBackColor = true;
            BtnRemoveStation.Click += BtnRemoveStation_Click;
            // 
            // BtnJobs
            // 
            BtnJobs.Location = new Point(498, 336);
            BtnJobs.Name = "BtnJobs";
            BtnJobs.Size = new Size(156, 31);
            BtnJobs.TabIndex = 48;
            BtnJobs.Text = "Custom Jobs";
            BtnJobs.UseVisualStyleBackColor = true;
            BtnJobs.Click += BtnJobs_Click;
            // 
            // BtnFactories
            // 
            BtnFactories.Location = new Point(498, 373);
            BtnFactories.Name = "BtnFactories";
            BtnFactories.Size = new Size(156, 31);
            BtnFactories.TabIndex = 49;
            BtnFactories.Text = "Custom Factories";
            BtnFactories.UseVisualStyleBackColor = true;
            BtnFactories.Click += BtnFactories_Click;
            // 
            // BtnObjectsOverview
            // 
            BtnObjectsOverview.Location = new Point(498, 447);
            BtnObjectsOverview.Name = "BtnObjectsOverview";
            BtnObjectsOverview.Size = new Size(156, 31);
            BtnObjectsOverview.TabIndex = 50;
            BtnObjectsOverview.Text = "Objects Overview";
            BtnObjectsOverview.UseVisualStyleBackColor = true;
            BtnObjectsOverview.Click += BtnObjectsOverview_Click;
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(13, 77);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.PlaceholderText = "Search..";
            TxtSearch.Size = new Size(155, 23);
            TxtSearch.TabIndex = 51;
            // 
            // BtnCustomFactions
            // 
            BtnCustomFactions.Location = new Point(498, 410);
            BtnCustomFactions.Name = "BtnCustomFactions";
            BtnCustomFactions.Size = new Size(156, 31);
            BtnCustomFactions.TabIndex = 52;
            BtnCustomFactions.Text = "Custom Factions";
            BtnCustomFactions.UseVisualStyleBackColor = true;
            BtnCustomFactions.Click += BtnCustomFactions_Click;
            // 
            // GenerateModProgressBar
            // 
            GenerateModProgressBar.Location = new Point(498, 522);
            GenerateModProgressBar.Name = "GenerateModProgressBar";
            GenerateModProgressBar.Size = new Size(151, 12);
            GenerateModProgressBar.TabIndex = 53;
            // 
            // BtnSaveSectorMapping
            // 
            BtnSaveSectorMapping.Enabled = false;
            BtnSaveSectorMapping.Location = new Point(416, 305);
            BtnSaveSectorMapping.Name = "BtnSaveSectorMapping";
            BtnSaveSectorMapping.Size = new Size(238, 28);
            BtnSaveSectorMapping.TabIndex = 54;
            BtnSaveSectorMapping.Text = "Save Sector Mapping";
            BtnSaveSectorMapping.UseVisualStyleBackColor = true;
            BtnSaveSectorMapping.Visible = false;
            BtnSaveSectorMapping.Click += BtnSaveSectorMapping_Click;
            // 
            // BtnGuide
            // 
            BtnGuide.Location = new Point(498, 483);
            BtnGuide.Name = "BtnGuide";
            BtnGuide.Size = new Size(156, 31);
            BtnGuide.TabIndex = 55;
            BtnGuide.Text = "Tutorial / Guide";
            BtnGuide.UseVisualStyleBackColor = true;
            BtnGuide.Click += BtnGuide_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(661, 596);
            Controls.Add(BtnGuide);
            Controls.Add(BtnSaveSectorMapping);
            Controls.Add(GenerateModProgressBar);
            Controls.Add(BtnCustomFactions);
            Controls.Add(TxtSearch);
            Controls.Add(BtnObjectsOverview);
            Controls.Add(BtnFactories);
            Controls.Add(BtnJobs);
            Controls.Add(BtnNewStation);
            Controls.Add(ListStations);
            Controls.Add(label2);
            Controls.Add(BtnRemoveStation);
            Controls.Add(cmbClusterOption);
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
        private ComboBox cmbClusterOption;
        private Button BtnNewStation;
        internal ListBox ListStations;
        private Label label2;
        private Button BtnRemoveStation;
        private Button BtnJobs;
        private Button BtnFactories;
        private Button BtnObjectsOverview;
        private TextBox TxtSearch;
        private Button BtnCustomFactions;
        private ProgressBar GenerateModProgressBar;
        private Button BtnSaveSectorMapping;
        private Button BtnGuide;
    }
}
