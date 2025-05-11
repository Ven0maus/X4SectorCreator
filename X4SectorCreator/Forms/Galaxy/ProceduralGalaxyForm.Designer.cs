namespace X4SectorCreator.Forms.Galaxy
{
    partial class ProceduralGalaxyForm
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
            BtnOpenSectorMap = new Button();
            label1 = new Label();
            ChkRegions = new CheckBox();
            ChkFactions = new CheckBox();
            TxtMapSeed = new TextBox();
            label2 = new Label();
            label3 = new Label();
            RegionResourceRarity = new DataGridView();
            Resource = new DataGridViewTextBoxColumn();
            Rarity = new DataGridViewTextBoxColumn();
            label6 = new Label();
            label7 = new Label();
            NrMinMainFactions = new NumericUpDown();
            NrMaxMainFactions = new NumericUpDown();
            label8 = new Label();
            label9 = new Label();
            label13 = new Label();
            NrChanceMultiSectors = new NumericUpDown();
            label14 = new Label();
            label15 = new Label();
            CmbClusterDistribution = new ComboBox();
            label16 = new Label();
            label17 = new Label();
            label18 = new Label();
            NrFacControlMax = new NumericUpDown();
            NrFacControlMin = new NumericUpDown();
            label19 = new Label();
            CmbCustomFactionDistribution = new ComboBox();
            RegionPanel = new Panel();
            ChkRegionRandomizeSeed = new CheckBox();
            CmbRegionDistribution = new ComboBox();
            label4 = new Label();
            label33 = new Label();
            TxtRegionSeed = new TextBox();
            panel1 = new Panel();
            label35 = new Label();
            NrMinPirateFactions = new NumericUpDown();
            NrMaxPirateFactions = new NumericUpDown();
            label36 = new Label();
            label37 = new Label();
            ChkFactionsRandomizeSeed = new CheckBox();
            label5 = new Label();
            TxtFactionSeed = new TextBox();
            panel2 = new Panel();
            ChkMapRandomizeSeed = new CheckBox();
            MapAlgorithmOptions = new TabControl();
            TabNoise = new TabPage();
            NrNoiseThreshold = new NumericUpDown();
            label31 = new Label();
            NrNoiseOffsetY = new NumericUpDown();
            label30 = new Label();
            NrNoiseOffsetX = new NumericUpDown();
            label29 = new Label();
            NrNoiseScale = new NumericUpDown();
            label28 = new Label();
            NrNoiseLacunarity = new NumericUpDown();
            label27 = new Label();
            NrNoisePersistance = new NumericUpDown();
            label26 = new Label();
            NrNoiseOctaves = new NumericUpDown();
            label25 = new Label();
            label21 = new Label();
            NoiseVisual = new PictureBox();
            TabRandom = new TabPage();
            label23 = new Label();
            NrClusterChance = new NumericUpDown();
            NrGridHeight = new NumericUpDown();
            NrGridWidth = new NumericUpDown();
            label11 = new Label();
            label10 = new Label();
            panel3 = new Panel();
            ChkConnectionsRandomizeSeed = new CheckBox();
            label32 = new Label();
            label34 = new Label();
            NrMultiConnectionChance = new NumericUpDown();
            TxtConnectionSeed = new TextBox();
            label12 = new Label();
            label20 = new Label();
            CmbGateConnectionDistribution = new ComboBox();
            NrMinGates = new NumericUpDown();
            NrMaxGates = new NumericUpDown();
            label22 = new Label();
            label24 = new Label();
            ChkGenerateConnections = new CheckBox();
            BtnGenerate = new Button();
            BtnExit = new Button();
            ProcGenProcess = new ProgressBar();
            label38 = new Label();
            NrMaxSectorRadius = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)RegionResourceRarity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrMinMainFactions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxMainFactions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrChanceMultiSectors).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMin).BeginInit();
            RegionPanel.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NrMinPirateFactions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxPirateFactions).BeginInit();
            panel2.SuspendLayout();
            MapAlgorithmOptions.SuspendLayout();
            TabNoise.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NrNoiseThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseOffsetY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseOffsetX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseScale).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseLacunarity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrNoisePersistance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseOctaves).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NoiseVisual).BeginInit();
            TabRandom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NrClusterChance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrGridHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrGridWidth).BeginInit();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NrMultiConnectionChance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrMinGates).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxGates).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxSectorRadius).BeginInit();
            SuspendLayout();
            // 
            // BtnOpenSectorMap
            // 
            BtnOpenSectorMap.Location = new Point(13, 394);
            BtnOpenSectorMap.Name = "BtnOpenSectorMap";
            BtnOpenSectorMap.Size = new Size(279, 35);
            BtnOpenSectorMap.TabIndex = 0;
            BtnOpenSectorMap.Text = "Open Sector Map";
            BtnOpenSectorMap.UseVisualStyleBackColor = true;
            BtnOpenSectorMap.Click += BtnOpenSectorMap_Click;
            // 
            // label1
            // 
            label1.Location = new Point(17, 527);
            label1.Name = "label1";
            label1.Size = new Size(273, 63);
            label1.TabIndex = 2;
            label1.Text = "Steps:\r\n1. Set Generation Options\r\n2. Open sector map to view live updates.\r\n3. Click generate as many times as you want.";
            // 
            // ChkRegions
            // 
            ChkRegions.AutoSize = true;
            ChkRegions.Checked = true;
            ChkRegions.CheckState = CheckState.Checked;
            ChkRegions.Location = new Point(302, 8);
            ChkRegions.Name = "ChkRegions";
            ChkRegions.Size = new Size(118, 19);
            ChkRegions.TabIndex = 3;
            ChkRegions.Text = "Generate Regions";
            ChkRegions.UseVisualStyleBackColor = true;
            // 
            // ChkFactions
            // 
            ChkFactions.AutoSize = true;
            ChkFactions.Checked = true;
            ChkFactions.CheckState = CheckState.Checked;
            ChkFactions.Location = new Point(307, 301);
            ChkFactions.Name = "ChkFactions";
            ChkFactions.Size = new Size(120, 19);
            ChkFactions.TabIndex = 4;
            ChkFactions.Text = "Generate Factions";
            ChkFactions.UseVisualStyleBackColor = true;
            // 
            // TxtMapSeed
            // 
            TxtMapSeed.Location = new Point(44, 311);
            TxtMapSeed.Name = "TxtMapSeed";
            TxtMapSeed.Size = new Size(224, 23);
            TxtMapSeed.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 316);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 6;
            label2.Text = "Seed:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label3.Location = new Point(7, 8);
            label3.Name = "label3";
            label3.Size = new Size(146, 21);
            label3.TabIndex = 7;
            label3.Text = "Generation Options";
            // 
            // RegionResourceRarity
            // 
            RegionResourceRarity.AllowUserToAddRows = false;
            RegionResourceRarity.AllowUserToDeleteRows = false;
            RegionResourceRarity.AllowUserToOrderColumns = true;
            RegionResourceRarity.AllowUserToResizeRows = false;
            RegionResourceRarity.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            RegionResourceRarity.Columns.AddRange(new DataGridViewColumn[] { Resource, Rarity });
            RegionResourceRarity.Location = new Point(6, 52);
            RegionResourceRarity.Name = "RegionResourceRarity";
            RegionResourceRarity.Size = new Size(263, 161);
            RegionResourceRarity.TabIndex = 13;
            // 
            // Resource
            // 
            Resource.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Resource.HeaderText = "Resource";
            Resource.Name = "Resource";
            Resource.ReadOnly = true;
            Resource.ToolTipText = "The name of the resource.";
            // 
            // Rarity
            // 
            Rarity.HeaderText = "Rarity";
            Rarity.Name = "Rarity";
            Rarity.ToolTipText = "Distribution is based on the rarity of the resource.";
            Rarity.Width = 75;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 34);
            label6.Name = "label6";
            label6.Size = new Size(91, 15);
            label6.TabIndex = 14;
            label6.Text = "Resource Rarity:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(4, 8);
            label7.Name = "label7";
            label7.Size = new Size(84, 15);
            label7.TabIndex = 15;
            label7.Text = "Main Factions:";
            // 
            // NrMinMainFactions
            // 
            NrMinMainFactions.Location = new Point(151, 4);
            NrMinMainFactions.Name = "NrMinMainFactions";
            NrMinMainFactions.Size = new Size(41, 23);
            NrMinMainFactions.TabIndex = 16;
            NrMinMainFactions.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // NrMaxMainFactions
            // 
            NrMaxMainFactions.Location = new Point(228, 4);
            NrMaxMainFactions.Name = "NrMaxMainFactions";
            NrMaxMainFactions.Size = new Size(41, 23);
            NrMaxMainFactions.TabIndex = 17;
            NrMaxMainFactions.Value = new decimal(new int[] { 12, 0, 0, 0 });
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(97, 8);
            label8.Name = "label8";
            label8.Size = new Size(52, 15);
            label8.TabIndex = 18;
            label8.Text = "between";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(196, 8);
            label9.Name = "label9";
            label9.Size = new Size(27, 15);
            label9.TabIndex = 19;
            label9.Text = "and";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(4, 35);
            label13.Name = "label13";
            label13.Size = new Size(159, 15);
            label13.TabIndex = 25;
            label13.Text = "Multi-Sector Cluster Chance:";
            // 
            // NrChanceMultiSectors
            // 
            NrChanceMultiSectors.Location = new Point(164, 32);
            NrChanceMultiSectors.Name = "NrChanceMultiSectors";
            NrChanceMultiSectors.Size = new Size(41, 23);
            NrChanceMultiSectors.TabIndex = 26;
            NrChanceMultiSectors.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(124, 34);
            label14.Name = "label14";
            label14.Size = new Size(145, 15);
            label14.TabIndex = 27;
            label14.Text = "0 non-existant 1 abundant";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(5, 64);
            label15.Name = "label15";
            label15.Size = new Size(72, 15);
            label15.TabIndex = 28;
            label15.Text = "Distribution:";
            // 
            // CmbClusterDistribution
            // 
            CmbClusterDistribution.FormattingEnabled = true;
            CmbClusterDistribution.Items.AddRange(new object[] { "Noise", "Random" });
            CmbClusterDistribution.Location = new Point(81, 60);
            CmbClusterDistribution.Name = "CmbClusterDistribution";
            CmbClusterDistribution.Size = new Size(124, 23);
            CmbClusterDistribution.TabIndex = 29;
            CmbClusterDistribution.Text = "Noise";
            CmbClusterDistribution.SelectedIndexChanged += CmbClusterDistribution_SelectedIndexChanged;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(4, 66);
            label16.Name = "label16";
            label16.Size = new Size(86, 15);
            label16.TabIndex = 30;
            label16.Text = "Sector Control:";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(196, 66);
            label17.Name = "label17";
            label17.Size = new Size(27, 15);
            label17.TabIndex = 34;
            label17.Text = "and";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(97, 66);
            label18.Name = "label18";
            label18.Size = new Size(52, 15);
            label18.TabIndex = 33;
            label18.Text = "between";
            // 
            // NrFacControlMax
            // 
            NrFacControlMax.Location = new Point(228, 62);
            NrFacControlMax.Name = "NrFacControlMax";
            NrFacControlMax.Size = new Size(41, 23);
            NrFacControlMax.TabIndex = 32;
            NrFacControlMax.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // NrFacControlMin
            // 
            NrFacControlMin.Location = new Point(151, 62);
            NrFacControlMin.Name = "NrFacControlMin";
            NrFacControlMin.Size = new Size(41, 23);
            NrFacControlMin.TabIndex = 31;
            NrFacControlMin.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(5, 94);
            label19.Name = "label19";
            label19.Size = new Size(72, 15);
            label19.TabIndex = 35;
            label19.Text = "Distribution:";
            // 
            // CmbCustomFactionDistribution
            // 
            CmbCustomFactionDistribution.FormattingEnabled = true;
            CmbCustomFactionDistribution.Items.AddRange(new object[] { "Balanced" });
            CmbCustomFactionDistribution.Location = new Point(97, 90);
            CmbCustomFactionDistribution.Name = "CmbCustomFactionDistribution";
            CmbCustomFactionDistribution.Size = new Size(172, 23);
            CmbCustomFactionDistribution.TabIndex = 36;
            CmbCustomFactionDistribution.Text = "Balanced";
            // 
            // RegionPanel
            // 
            RegionPanel.BackColor = SystemColors.ButtonHighlight;
            RegionPanel.Controls.Add(ChkRegionRandomizeSeed);
            RegionPanel.Controls.Add(CmbRegionDistribution);
            RegionPanel.Controls.Add(label4);
            RegionPanel.Controls.Add(label33);
            RegionPanel.Controls.Add(TxtRegionSeed);
            RegionPanel.Controls.Add(RegionResourceRarity);
            RegionPanel.Controls.Add(label6);
            RegionPanel.Controls.Add(label14);
            RegionPanel.Location = new Point(302, 32);
            RegionPanel.Name = "RegionPanel";
            RegionPanel.Size = new Size(279, 263);
            RegionPanel.TabIndex = 37;
            // 
            // ChkRegionRandomizeSeed
            // 
            ChkRegionRandomizeSeed.AutoSize = true;
            ChkRegionRandomizeSeed.Checked = true;
            ChkRegionRandomizeSeed.CheckState = CheckState.Checked;
            ChkRegionRandomizeSeed.Location = new Point(87, 239);
            ChkRegionRandomizeSeed.Name = "ChkRegionRandomizeSeed";
            ChkRegionRandomizeSeed.Size = new Size(184, 19);
            ChkRegionRandomizeSeed.TabIndex = 45;
            ChkRegionRandomizeSeed.Text = "Randomize seed on Generate?";
            ChkRegionRandomizeSeed.UseVisualStyleBackColor = true;
            // 
            // CmbRegionDistribution
            // 
            CmbRegionDistribution.FormattingEnabled = true;
            CmbRegionDistribution.Items.AddRange(new object[] { "Balanced" });
            CmbRegionDistribution.Location = new Point(79, 6);
            CmbRegionDistribution.Name = "CmbRegionDistribution";
            CmbRegionDistribution.Size = new Size(113, 23);
            CmbRegionDistribution.TabIndex = 41;
            CmbRegionDistribution.Text = "Balanced";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(9, 221);
            label4.Name = "label4";
            label4.Size = new Size(35, 15);
            label4.TabIndex = 44;
            label4.Text = "Seed:";
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Location = new Point(6, 10);
            label33.Name = "label33";
            label33.Size = new Size(72, 15);
            label33.TabIndex = 41;
            label33.Text = "Distribution:";
            // 
            // TxtRegionSeed
            // 
            TxtRegionSeed.Location = new Point(45, 216);
            TxtRegionSeed.Name = "TxtRegionSeed";
            TxtRegionSeed.Size = new Size(224, 23);
            TxtRegionSeed.TabIndex = 43;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ButtonHighlight;
            panel1.Controls.Add(label35);
            panel1.Controls.Add(NrMinPirateFactions);
            panel1.Controls.Add(NrMaxPirateFactions);
            panel1.Controls.Add(label36);
            panel1.Controls.Add(label37);
            panel1.Controls.Add(ChkFactionsRandomizeSeed);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label19);
            panel1.Controls.Add(TxtFactionSeed);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(CmbCustomFactionDistribution);
            panel1.Controls.Add(NrMinMainFactions);
            panel1.Controls.Add(NrMaxMainFactions);
            panel1.Controls.Add(label17);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label18);
            panel1.Controls.Add(label9);
            panel1.Controls.Add(NrFacControlMax);
            panel1.Controls.Add(label16);
            panel1.Controls.Add(NrFacControlMin);
            panel1.Location = new Point(302, 322);
            panel1.Name = "panel1";
            panel1.Size = new Size(279, 162);
            panel1.TabIndex = 38;
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Location = new Point(4, 37);
            label35.Name = "label35";
            label35.Size = new Size(87, 15);
            label35.TabIndex = 49;
            label35.Text = "Pirate Factions:";
            // 
            // NrMinPirateFactions
            // 
            NrMinPirateFactions.Location = new Point(151, 33);
            NrMinPirateFactions.Name = "NrMinPirateFactions";
            NrMinPirateFactions.Size = new Size(41, 23);
            NrMinPirateFactions.TabIndex = 50;
            NrMinPirateFactions.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // NrMaxPirateFactions
            // 
            NrMaxPirateFactions.Location = new Point(228, 33);
            NrMaxPirateFactions.Name = "NrMaxPirateFactions";
            NrMaxPirateFactions.Size = new Size(41, 23);
            NrMaxPirateFactions.TabIndex = 51;
            NrMaxPirateFactions.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Location = new Point(97, 37);
            label36.Name = "label36";
            label36.Size = new Size(52, 15);
            label36.TabIndex = 52;
            label36.Text = "between";
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new Point(196, 37);
            label37.Name = "label37";
            label37.Size = new Size(27, 15);
            label37.TabIndex = 53;
            label37.Text = "and";
            // 
            // ChkFactionsRandomizeSeed
            // 
            ChkFactionsRandomizeSeed.AutoSize = true;
            ChkFactionsRandomizeSeed.Checked = true;
            ChkFactionsRandomizeSeed.CheckState = CheckState.Checked;
            ChkFactionsRandomizeSeed.Location = new Point(87, 138);
            ChkFactionsRandomizeSeed.Name = "ChkFactionsRandomizeSeed";
            ChkFactionsRandomizeSeed.Size = new Size(184, 19);
            ChkFactionsRandomizeSeed.TabIndex = 48;
            ChkFactionsRandomizeSeed.Text = "Randomize seed on Generate?";
            ChkFactionsRandomizeSeed.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(9, 120);
            label5.Name = "label5";
            label5.Size = new Size(35, 15);
            label5.TabIndex = 47;
            label5.Text = "Seed:";
            // 
            // TxtFactionSeed
            // 
            TxtFactionSeed.Location = new Point(45, 115);
            TxtFactionSeed.Name = "TxtFactionSeed";
            TxtFactionSeed.Size = new Size(224, 23);
            TxtFactionSeed.TabIndex = 46;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ButtonHighlight;
            panel2.Controls.Add(NrMaxSectorRadius);
            panel2.Controls.Add(label38);
            panel2.Controls.Add(ChkMapRandomizeSeed);
            panel2.Controls.Add(MapAlgorithmOptions);
            panel2.Controls.Add(NrGridHeight);
            panel2.Controls.Add(NrGridWidth);
            panel2.Controls.Add(label11);
            panel2.Controls.Add(label10);
            panel2.Controls.Add(CmbClusterDistribution);
            panel2.Controls.Add(label15);
            panel2.Controls.Add(label13);
            panel2.Controls.Add(NrChanceMultiSectors);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(TxtMapSeed);
            panel2.Location = new Point(12, 33);
            panel2.Name = "panel2";
            panel2.Size = new Size(279, 357);
            panel2.TabIndex = 40;
            // 
            // ChkMapRandomizeSeed
            // 
            ChkMapRandomizeSeed.AutoSize = true;
            ChkMapRandomizeSeed.Checked = true;
            ChkMapRandomizeSeed.CheckState = CheckState.Checked;
            ChkMapRandomizeSeed.Location = new Point(86, 334);
            ChkMapRandomizeSeed.Name = "ChkMapRandomizeSeed";
            ChkMapRandomizeSeed.Size = new Size(184, 19);
            ChkMapRandomizeSeed.TabIndex = 42;
            ChkMapRandomizeSeed.Text = "Randomize seed on Generate?";
            ChkMapRandomizeSeed.UseVisualStyleBackColor = true;
            // 
            // MapAlgorithmOptions
            // 
            MapAlgorithmOptions.Controls.Add(TabNoise);
            MapAlgorithmOptions.Controls.Add(TabRandom);
            MapAlgorithmOptions.Location = new Point(8, 85);
            MapAlgorithmOptions.Name = "MapAlgorithmOptions";
            MapAlgorithmOptions.SelectedIndex = 0;
            MapAlgorithmOptions.Size = new Size(264, 224);
            MapAlgorithmOptions.TabIndex = 36;
            // 
            // TabNoise
            // 
            TabNoise.Controls.Add(NrNoiseThreshold);
            TabNoise.Controls.Add(label31);
            TabNoise.Controls.Add(NrNoiseOffsetY);
            TabNoise.Controls.Add(label30);
            TabNoise.Controls.Add(NrNoiseOffsetX);
            TabNoise.Controls.Add(label29);
            TabNoise.Controls.Add(NrNoiseScale);
            TabNoise.Controls.Add(label28);
            TabNoise.Controls.Add(NrNoiseLacunarity);
            TabNoise.Controls.Add(label27);
            TabNoise.Controls.Add(NrNoisePersistance);
            TabNoise.Controls.Add(label26);
            TabNoise.Controls.Add(NrNoiseOctaves);
            TabNoise.Controls.Add(label25);
            TabNoise.Controls.Add(label21);
            TabNoise.Controls.Add(NoiseVisual);
            TabNoise.Location = new Point(4, 24);
            TabNoise.Name = "TabNoise";
            TabNoise.Padding = new Padding(3);
            TabNoise.Size = new Size(256, 196);
            TabNoise.TabIndex = 1;
            TabNoise.Text = "Noise";
            TabNoise.UseVisualStyleBackColor = true;
            // 
            // NrNoiseThreshold
            // 
            NrNoiseThreshold.DecimalPlaces = 2;
            NrNoiseThreshold.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            NrNoiseThreshold.Location = new Point(74, 159);
            NrNoiseThreshold.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            NrNoiseThreshold.Name = "NrNoiseThreshold";
            NrNoiseThreshold.Size = new Size(55, 23);
            NrNoiseThreshold.TabIndex = 49;
            NrNoiseThreshold.Value = new decimal(new int[] { 3, 0, 0, 65536 });
            NrNoiseThreshold.ValueChanged += NoiseProperty_ValueChanged;
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Location = new Point(7, 161);
            label31.Name = "label31";
            label31.Size = new Size(63, 15);
            label31.TabIndex = 48;
            label31.Text = "Threshold:";
            // 
            // NrNoiseOffsetY
            // 
            NrNoiseOffsetY.Location = new Point(74, 134);
            NrNoiseOffsetY.Name = "NrNoiseOffsetY";
            NrNoiseOffsetY.Size = new Size(55, 23);
            NrNoiseOffsetY.TabIndex = 46;
            NrNoiseOffsetY.ValueChanged += NoiseProperty_ValueChanged;
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Location = new Point(17, 138);
            label30.Name = "label30";
            label30.Size = new Size(52, 15);
            label30.TabIndex = 47;
            label30.Text = "Offset Y:";
            // 
            // NrNoiseOffsetX
            // 
            NrNoiseOffsetX.Location = new Point(74, 109);
            NrNoiseOffsetX.Name = "NrNoiseOffsetX";
            NrNoiseOffsetX.Size = new Size(55, 23);
            NrNoiseOffsetX.TabIndex = 44;
            NrNoiseOffsetX.ValueChanged += NoiseProperty_ValueChanged;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(17, 113);
            label29.Name = "label29";
            label29.Size = new Size(52, 15);
            label29.TabIndex = 45;
            label29.Text = "Offset X:";
            // 
            // NrNoiseScale
            // 
            NrNoiseScale.DecimalPlaces = 2;
            NrNoiseScale.Location = new Point(74, 83);
            NrNoiseScale.Name = "NrNoiseScale";
            NrNoiseScale.Size = new Size(74, 23);
            NrNoiseScale.TabIndex = 43;
            NrNoiseScale.Value = new decimal(new int[] { 8, 0, 0, 0 });
            NrNoiseScale.ValueChanged += NoiseProperty_ValueChanged;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new Point(32, 86);
            label28.Name = "label28";
            label28.Size = new Size(37, 15);
            label28.TabIndex = 42;
            label28.Text = "Scale:";
            // 
            // NrNoiseLacunarity
            // 
            NrNoiseLacunarity.DecimalPlaces = 2;
            NrNoiseLacunarity.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            NrNoiseLacunarity.Location = new Point(74, 58);
            NrNoiseLacunarity.Name = "NrNoiseLacunarity";
            NrNoiseLacunarity.Size = new Size(74, 23);
            NrNoiseLacunarity.TabIndex = 41;
            NrNoiseLacunarity.Value = new decimal(new int[] { 7, 0, 0, 65536 });
            NrNoiseLacunarity.ValueChanged += NoiseProperty_ValueChanged;
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new Point(4, 60);
            label27.Name = "label27";
            label27.Size = new Size(65, 15);
            label27.TabIndex = 40;
            label27.Text = "Lacunarity:";
            // 
            // NrNoisePersistance
            // 
            NrNoisePersistance.DecimalPlaces = 2;
            NrNoisePersistance.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            NrNoisePersistance.Location = new Point(74, 32);
            NrNoisePersistance.Name = "NrNoisePersistance";
            NrNoisePersistance.Size = new Size(74, 23);
            NrNoisePersistance.TabIndex = 38;
            NrNoisePersistance.Value = new decimal(new int[] { 115, 0, 0, 131072 });
            NrNoisePersistance.ValueChanged += NoiseProperty_ValueChanged;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(4, 35);
            label26.Name = "label26";
            label26.Size = new Size(69, 15);
            label26.TabIndex = 39;
            label26.Text = "Persistance:";
            // 
            // NrNoiseOctaves
            // 
            NrNoiseOctaves.Location = new Point(74, 6);
            NrNoiseOctaves.Name = "NrNoiseOctaves";
            NrNoiseOctaves.Size = new Size(55, 23);
            NrNoiseOctaves.TabIndex = 37;
            NrNoiseOctaves.Value = new decimal(new int[] { 3, 0, 0, 0 });
            NrNoiseOctaves.ValueChanged += NoiseProperty_ValueChanged;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new Point(4, 9);
            label25.Name = "label25";
            label25.Size = new Size(52, 15);
            label25.TabIndex = 37;
            label25.Text = "Octaves:";
            // 
            // label21
            // 
            label21.Location = new Point(154, 69);
            label21.Name = "label21";
            label21.Size = new Size(98, 20);
            label21.TabIndex = 1;
            label21.Text = "Noise Result:";
            label21.TextAlign = ContentAlignment.BottomCenter;
            // 
            // NoiseVisual
            // 
            NoiseVisual.Location = new Point(153, 92);
            NoiseVisual.Name = "NoiseVisual";
            NoiseVisual.Size = new Size(100, 100);
            NoiseVisual.SizeMode = PictureBoxSizeMode.StretchImage;
            NoiseVisual.TabIndex = 0;
            NoiseVisual.TabStop = false;
            // 
            // TabRandom
            // 
            TabRandom.Controls.Add(label23);
            TabRandom.Controls.Add(NrClusterChance);
            TabRandom.Location = new Point(4, 24);
            TabRandom.Name = "TabRandom";
            TabRandom.Padding = new Padding(3);
            TabRandom.Size = new Size(256, 196);
            TabRandom.TabIndex = 0;
            TabRandom.Text = "Random";
            TabRandom.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(6, 10);
            label23.Name = "label23";
            label23.Size = new Size(133, 15);
            label23.TabIndex = 34;
            label23.Text = "Cluster Chance Per Hex:";
            // 
            // NrClusterChance
            // 
            NrClusterChance.Location = new Point(140, 6);
            NrClusterChance.Name = "NrClusterChance";
            NrClusterChance.Size = new Size(41, 23);
            NrClusterChance.TabIndex = 35;
            NrClusterChance.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // NrGridHeight
            // 
            NrGridHeight.Location = new Point(197, 5);
            NrGridHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NrGridHeight.Name = "NrGridHeight";
            NrGridHeight.Size = new Size(41, 23);
            NrGridHeight.TabIndex = 33;
            NrGridHeight.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // NrGridWidth
            // 
            NrGridWidth.Location = new Point(77, 5);
            NrGridWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NrGridWidth.Name = "NrGridWidth";
            NrGridWidth.Size = new Size(41, 23);
            NrGridWidth.TabIndex = 32;
            NrGridWidth.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(124, 8);
            label11.Name = "label11";
            label11.Size = new Size(71, 15);
            label11.TabIndex = 31;
            label11.Text = "Grid Height:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(4, 8);
            label10.Name = "label10";
            label10.Size = new Size(67, 15);
            label10.TabIndex = 30;
            label10.Text = "Grid Width:";
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.ButtonHighlight;
            panel3.Controls.Add(ChkConnectionsRandomizeSeed);
            panel3.Controls.Add(label32);
            panel3.Controls.Add(label34);
            panel3.Controls.Add(NrMultiConnectionChance);
            panel3.Controls.Add(TxtConnectionSeed);
            panel3.Controls.Add(label12);
            panel3.Controls.Add(label20);
            panel3.Controls.Add(CmbGateConnectionDistribution);
            panel3.Controls.Add(NrMinGates);
            panel3.Controls.Add(NrMaxGates);
            panel3.Controls.Add(label22);
            panel3.Controls.Add(label24);
            panel3.Location = new Point(302, 512);
            panel3.Name = "panel3";
            panel3.Size = new Size(279, 120);
            panel3.TabIndex = 40;
            // 
            // ChkConnectionsRandomizeSeed
            // 
            ChkConnectionsRandomizeSeed.AutoSize = true;
            ChkConnectionsRandomizeSeed.Checked = true;
            ChkConnectionsRandomizeSeed.CheckState = CheckState.Checked;
            ChkConnectionsRandomizeSeed.Location = new Point(87, 99);
            ChkConnectionsRandomizeSeed.Name = "ChkConnectionsRandomizeSeed";
            ChkConnectionsRandomizeSeed.Size = new Size(184, 19);
            ChkConnectionsRandomizeSeed.TabIndex = 51;
            ChkConnectionsRandomizeSeed.Text = "Randomize seed on Generate?";
            ChkConnectionsRandomizeSeed.UseVisualStyleBackColor = true;
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new Point(6, 28);
            label32.Name = "label32";
            label32.Size = new Size(88, 15);
            label32.TabIndex = 37;
            label32.Text = "Chance for > 1:";
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Location = new Point(9, 81);
            label34.Name = "label34";
            label34.Size = new Size(35, 15);
            label34.TabIndex = 50;
            label34.Text = "Seed:";
            // 
            // NrMultiConnectionChance
            // 
            NrMultiConnectionChance.Location = new Point(100, 26);
            NrMultiConnectionChance.Name = "NrMultiConnectionChance";
            NrMultiConnectionChance.Size = new Size(41, 23);
            NrMultiConnectionChance.TabIndex = 38;
            NrMultiConnectionChance.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // TxtConnectionSeed
            // 
            TxtConnectionSeed.Location = new Point(45, 76);
            TxtConnectionSeed.Name = "TxtConnectionSeed";
            TxtConnectionSeed.Size = new Size(224, 23);
            TxtConnectionSeed.TabIndex = 49;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(8, 55);
            label12.Name = "label12";
            label12.Size = new Size(72, 15);
            label12.TabIndex = 35;
            label12.Text = "Distribution:";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(7, 8);
            label20.Name = "label20";
            label20.Size = new Size(88, 15);
            label20.TabIndex = 15;
            label20.Text = "Max Per Sector:";
            // 
            // CmbGateConnectionDistribution
            // 
            CmbGateConnectionDistribution.FormattingEnabled = true;
            CmbGateConnectionDistribution.Items.AddRange(new object[] { "MST" });
            CmbGateConnectionDistribution.Location = new Point(100, 51);
            CmbGateConnectionDistribution.Name = "CmbGateConnectionDistribution";
            CmbGateConnectionDistribution.Size = new Size(172, 23);
            CmbGateConnectionDistribution.TabIndex = 36;
            CmbGateConnectionDistribution.Text = "MST";
            // 
            // NrMinGates
            // 
            NrMinGates.Location = new Point(154, 4);
            NrMinGates.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NrMinGates.Name = "NrMinGates";
            NrMinGates.Size = new Size(41, 23);
            NrMinGates.TabIndex = 16;
            NrMinGates.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // NrMaxGates
            // 
            NrMaxGates.Location = new Point(231, 4);
            NrMaxGates.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NrMaxGates.Name = "NrMaxGates";
            NrMaxGates.Size = new Size(41, 23);
            NrMaxGates.TabIndex = 17;
            NrMaxGates.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(100, 8);
            label22.Name = "label22";
            label22.Size = new Size(52, 15);
            label22.TabIndex = 18;
            label22.Text = "between";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new Point(199, 8);
            label24.Name = "label24";
            label24.Size = new Size(27, 15);
            label24.TabIndex = 19;
            label24.Text = "and";
            // 
            // ChkGenerateConnections
            // 
            ChkGenerateConnections.AutoSize = true;
            ChkGenerateConnections.Checked = true;
            ChkGenerateConnections.CheckState = CheckState.Checked;
            ChkGenerateConnections.Location = new Point(307, 490);
            ChkGenerateConnections.Name = "ChkGenerateConnections";
            ChkGenerateConnections.Size = new Size(170, 19);
            ChkGenerateConnections.TabIndex = 39;
            ChkGenerateConnections.Text = "Generate Gate Connections";
            ChkGenerateConnections.UseVisualStyleBackColor = true;
            // 
            // BtnGenerate
            // 
            BtnGenerate.Location = new Point(12, 433);
            BtnGenerate.Name = "BtnGenerate";
            BtnGenerate.Size = new Size(279, 59);
            BtnGenerate.TabIndex = 39;
            BtnGenerate.Text = "Generate";
            BtnGenerate.UseVisualStyleBackColor = true;
            BtnGenerate.Click += BtnGenerate_Click;
            // 
            // BtnExit
            // 
            BtnExit.Location = new Point(13, 593);
            BtnExit.Name = "BtnExit";
            BtnExit.Size = new Size(278, 42);
            BtnExit.TabIndex = 41;
            BtnExit.Text = "Exit";
            BtnExit.UseVisualStyleBackColor = true;
            BtnExit.Click += BtnExit_Click;
            // 
            // ProcGenProcess
            // 
            ProcGenProcess.Location = new Point(13, 497);
            ProcGenProcess.Name = "ProcGenProcess";
            ProcGenProcess.Size = new Size(278, 21);
            ProcGenProcess.TabIndex = 42;
            // 
            // label38
            // 
            label38.Location = new Point(209, 30);
            label38.Name = "label38";
            label38.Size = new Size(67, 30);
            label38.TabIndex = 43;
            label38.Text = "Max Sector Radius:";
            // 
            // NrMaxSectorRadius
            // 
            NrMaxSectorRadius.Location = new Point(213, 60);
            NrMaxSectorRadius.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            NrMaxSectorRadius.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            NrMaxSectorRadius.Name = "NrMaxSectorRadius";
            NrMaxSectorRadius.Size = new Size(59, 23);
            NrMaxSectorRadius.TabIndex = 44;
            NrMaxSectorRadius.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // ProceduralGalaxyForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(585, 638);
            Controls.Add(ProcGenProcess);
            Controls.Add(BtnExit);
            Controls.Add(BtnGenerate);
            Controls.Add(panel3);
            Controls.Add(ChkGenerateConnections);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(RegionPanel);
            Controls.Add(label3);
            Controls.Add(ChkFactions);
            Controls.Add(ChkRegions);
            Controls.Add(label1);
            Controls.Add(BtnOpenSectorMap);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProceduralGalaxyForm";
            Text = "Procedural Galaxy Generator";
            ((System.ComponentModel.ISupportInitialize)RegionResourceRarity).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrMinMainFactions).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxMainFactions).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrChanceMultiSectors).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMin).EndInit();
            RegionPanel.ResumeLayout(false);
            RegionPanel.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NrMinPirateFactions).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxPirateFactions).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            MapAlgorithmOptions.ResumeLayout(false);
            TabNoise.ResumeLayout(false);
            TabNoise.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NrNoiseThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseOffsetY).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseOffsetX).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseScale).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseLacunarity).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrNoisePersistance).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrNoiseOctaves).EndInit();
            ((System.ComponentModel.ISupportInitialize)NoiseVisual).EndInit();
            TabRandom.ResumeLayout(false);
            TabRandom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NrClusterChance).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrGridHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrGridWidth).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NrMultiConnectionChance).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrMinGates).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxGates).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxSectorRadius).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnOpenSectorMap;
        private Label label1;
        private CheckBox ChkRegions;
        private CheckBox ChkFactions;
        private TextBox TxtMapSeed;
        private Label label2;
        private Label label3;
        private DataGridView RegionResourceRarity;
        private Label label6;
        private Label label7;
        private NumericUpDown NrMinMainFactions;
        private NumericUpDown NrMaxMainFactions;
        private Label label8;
        private Label label9;
        private Label label13;
        private NumericUpDown NrChanceMultiSectors;
        private Label label14;
        private DataGridViewTextBoxColumn Resource;
        private DataGridViewTextBoxColumn Rarity;
        private Label label15;
        private ComboBox CmbClusterDistribution;
        private Label label16;
        private Label label17;
        private Label label18;
        private NumericUpDown NrFacControlMax;
        private NumericUpDown NrFacControlMin;
        private Label label19;
        private ComboBox CmbCustomFactionDistribution;
        private Panel RegionPanel;
        private Panel panel1;
        private Panel panel2;
        private NumericUpDown NrGridHeight;
        private NumericUpDown NrGridWidth;
        private Label label11;
        private Label label10;
        private Panel panel3;
        private Label label12;
        private Label label20;
        private ComboBox CmbGateConnectionDistribution;
        private NumericUpDown NrMinGates;
        private NumericUpDown NrMaxGates;
        private Label label22;
        private Label label24;
        private CheckBox ChkGenerateConnections;
        private NumericUpDown NrClusterChance;
        private Label label23;
        private Button BtnGenerate;
        private Button BtnExit;
        private TabControl MapAlgorithmOptions;
        private TabPage TabRandom;
        private TabPage TabNoise;
        private Label label21;
        private PictureBox NoiseVisual;
        private NumericUpDown NrNoisePersistance;
        private Label label26;
        private NumericUpDown NrNoiseOctaves;
        private Label label25;
        private Label label27;
        private NumericUpDown NrNoiseScale;
        private Label label28;
        private NumericUpDown NrNoiseLacunarity;
        private NumericUpDown NrNoiseOffsetY;
        private Label label30;
        private NumericUpDown NrNoiseOffsetX;
        private Label label29;
        private NumericUpDown NrNoiseThreshold;
        private Label label31;
        private Label label32;
        private NumericUpDown NrMultiConnectionChance;
        private ComboBox CmbRegionDistribution;
        private Label label33;
        private CheckBox ChkRegionRandomizeSeed;
        private Label label4;
        private TextBox TxtRegionSeed;
        private CheckBox ChkMapRandomizeSeed;
        private CheckBox ChkFactionsRandomizeSeed;
        private Label label5;
        private TextBox TxtFactionSeed;
        private CheckBox ChkConnectionsRandomizeSeed;
        private Label label34;
        private TextBox TxtConnectionSeed;
        private Label label35;
        private NumericUpDown NrMinPirateFactions;
        private NumericUpDown NrMaxPirateFactions;
        private Label label36;
        private Label label37;
        private ProgressBar ProcGenProcess;
        private Label label38;
        private NumericUpDown NrMaxSectorRadius;
    }
}