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
            button1 = new Button();
            BtnGenerate = new Button();
            label1 = new Label();
            ChkRegions = new CheckBox();
            ChkCustomFactions = new CheckBox();
            TxtSeed = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            NrRegionSpawnChance = new NumericUpDown();
            NrMaxRegionPerSector = new NumericUpDown();
            label5 = new Label();
            RegionResourceRarity = new DataGridView();
            Resource = new DataGridViewTextBoxColumn();
            Rarity = new DataGridViewTextBoxColumn();
            label6 = new Label();
            label7 = new Label();
            NrFactionMin = new NumericUpDown();
            NrFactionMax = new NumericUpDown();
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
            panel1 = new Panel();
            ChkAutoSeed = new CheckBox();
            panel2 = new Panel();
            NrGridHeight = new NumericUpDown();
            NrGridWidth = new NumericUpDown();
            label11 = new Label();
            label10 = new Label();
            panel3 = new Panel();
            label12 = new Label();
            label20 = new Label();
            comboBox1 = new ComboBox();
            NrMinGates = new NumericUpDown();
            NrMaxGates = new NumericUpDown();
            label22 = new Label();
            label24 = new Label();
            ChkGenerateConnections = new CheckBox();
            panel4 = new Panel();
            label21 = new Label();
            CmbVanillaFactionDistribution = new ComboBox();
            label25 = new Label();
            label27 = new Label();
            NrVanillaClusterMax = new NumericUpDown();
            label29 = new Label();
            NrVanillaClusterMin = new NumericUpDown();
            ChkGenerateVanillaFactions = new CheckBox();
            NrClusterChance = new NumericUpDown();
            label23 = new Label();
            ((System.ComponentModel.ISupportInitialize)NrRegionSpawnChance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxRegionPerSector).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RegionResourceRarity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFactionMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFactionMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrChanceMultiSectors).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMin).BeginInit();
            RegionPanel.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NrGridHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrGridWidth).BeginInit();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NrMinGates).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxGates).BeginInit();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NrVanillaClusterMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrVanillaClusterMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrClusterChance).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 417);
            button1.Name = "button1";
            button1.Size = new Size(182, 35);
            button1.TabIndex = 0;
            button1.Text = "Open Sector Map";
            button1.UseVisualStyleBackColor = true;
            // 
            // BtnGenerate
            // 
            BtnGenerate.Location = new Point(200, 417);
            BtnGenerate.Name = "BtnGenerate";
            BtnGenerate.Size = new Size(376, 35);
            BtnGenerate.TabIndex = 1;
            BtnGenerate.Text = "Generate";
            BtnGenerate.UseVisualStyleBackColor = true;
            BtnGenerate.Click += BtnGenerate_Click;
            // 
            // label1
            // 
            label1.Location = new Point(297, 352);
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
            // ChkCustomFactions
            // 
            ChkCustomFactions.AutoSize = true;
            ChkCustomFactions.Checked = true;
            ChkCustomFactions.CheckState = CheckState.Checked;
            ChkCustomFactions.Location = new Point(17, 293);
            ChkCustomFactions.Name = "ChkCustomFactions";
            ChkCustomFactions.Size = new Size(165, 19);
            ChkCustomFactions.TabIndex = 4;
            ChkCustomFactions.Text = "Generate Custom Factions";
            ChkCustomFactions.UseVisualStyleBackColor = true;
            // 
            // TxtSeed
            // 
            TxtSeed.Location = new Point(48, 10);
            TxtSeed.Name = "TxtSeed";
            TxtSeed.Size = new Size(202, 23);
            TxtSeed.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 14);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 6;
            label2.Text = "Seed:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label3.Location = new Point(7, 39);
            label3.Name = "label3";
            label3.Size = new Size(146, 21);
            label3.TabIndex = 7;
            label3.Text = "Generation Options";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 7);
            label4.Name = "label4";
            label4.Size = new Size(105, 15);
            label4.TabIndex = 9;
            label4.Text = "Chance per sector:";
            // 
            // NrRegionSpawnChance
            // 
            NrRegionSpawnChance.Location = new Point(113, 4);
            NrRegionSpawnChance.Name = "NrRegionSpawnChance";
            NrRegionSpawnChance.Size = new Size(41, 23);
            NrRegionSpawnChance.TabIndex = 10;
            NrRegionSpawnChance.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // NrMaxRegionPerSector
            // 
            NrMaxRegionPerSector.Location = new Point(113, 30);
            NrMaxRegionPerSector.Name = "NrMaxRegionPerSector";
            NrMaxRegionPerSector.Size = new Size(41, 23);
            NrMaxRegionPerSector.TabIndex = 12;
            NrMaxRegionPerSector.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(23, 32);
            label5.Name = "label5";
            label5.Size = new Size(88, 15);
            label5.TabIndex = 11;
            label5.Text = "Max Per Sector:";
            // 
            // RegionResourceRarity
            // 
            RegionResourceRarity.AllowUserToAddRows = false;
            RegionResourceRarity.AllowUserToDeleteRows = false;
            RegionResourceRarity.AllowUserToOrderColumns = true;
            RegionResourceRarity.AllowUserToResizeRows = false;
            RegionResourceRarity.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            RegionResourceRarity.Columns.AddRange(new DataGridViewColumn[] { Resource, Rarity });
            RegionResourceRarity.Location = new Point(6, 76);
            RegionResourceRarity.Name = "RegionResourceRarity";
            RegionResourceRarity.Size = new Size(249, 137);
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
            label6.Location = new Point(6, 58);
            label6.Name = "label6";
            label6.Size = new Size(91, 15);
            label6.TabIndex = 14;
            label6.Text = "Resource Rarity:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(7, 10);
            label7.Name = "label7";
            label7.Size = new Size(83, 15);
            label7.TabIndex = 15;
            label7.Text = "Total Factions:";
            // 
            // NrFactionMin
            // 
            NrFactionMin.Location = new Point(154, 6);
            NrFactionMin.Name = "NrFactionMin";
            NrFactionMin.Size = new Size(41, 23);
            NrFactionMin.TabIndex = 16;
            NrFactionMin.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // NrFactionMax
            // 
            NrFactionMax.Location = new Point(231, 6);
            NrFactionMax.Name = "NrFactionMax";
            NrFactionMax.Size = new Size(41, 23);
            NrFactionMax.TabIndex = 17;
            NrFactionMax.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(100, 10);
            label8.Name = "label8";
            label8.Size = new Size(52, 15);
            label8.TabIndex = 18;
            label8.Text = "between";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(199, 10);
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
            NrChanceMultiSectors.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(103, 59);
            label14.Name = "label14";
            label14.Size = new Size(145, 15);
            label14.TabIndex = 27;
            label14.Text = "0 non-existant 1 abundant";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(4, 95);
            label15.Name = "label15";
            label15.Size = new Size(64, 15);
            label15.TabIndex = 28;
            label15.Text = "Algorithm:";
            // 
            // CmbClusterDistribution
            // 
            CmbClusterDistribution.FormattingEnabled = true;
            CmbClusterDistribution.Location = new Point(70, 91);
            CmbClusterDistribution.Name = "CmbClusterDistribution";
            CmbClusterDistribution.Size = new Size(165, 23);
            CmbClusterDistribution.TabIndex = 29;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(7, 38);
            label16.Name = "label16";
            label16.Size = new Size(90, 15);
            label16.TabIndex = 30;
            label16.Text = "Cluster Control:";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(199, 38);
            label17.Name = "label17";
            label17.Size = new Size(27, 15);
            label17.TabIndex = 34;
            label17.Text = "and";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(100, 38);
            label18.Name = "label18";
            label18.Size = new Size(52, 15);
            label18.TabIndex = 33;
            label18.Text = "between";
            // 
            // NrFacControlMax
            // 
            NrFacControlMax.Location = new Point(231, 34);
            NrFacControlMax.Name = "NrFacControlMax";
            NrFacControlMax.Size = new Size(41, 23);
            NrFacControlMax.TabIndex = 32;
            NrFacControlMax.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // NrFacControlMin
            // 
            NrFacControlMin.Location = new Point(154, 34);
            NrFacControlMin.Name = "NrFacControlMin";
            NrFacControlMin.Size = new Size(41, 23);
            NrFacControlMin.TabIndex = 31;
            NrFacControlMin.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(8, 65);
            label19.Name = "label19";
            label19.Size = new Size(72, 15);
            label19.TabIndex = 35;
            label19.Text = "Distribution:";
            // 
            // CmbCustomFactionDistribution
            // 
            CmbCustomFactionDistribution.FormattingEnabled = true;
            CmbCustomFactionDistribution.Location = new Point(100, 61);
            CmbCustomFactionDistribution.Name = "CmbCustomFactionDistribution";
            CmbCustomFactionDistribution.Size = new Size(172, 23);
            CmbCustomFactionDistribution.TabIndex = 36;
            // 
            // RegionPanel
            // 
            RegionPanel.BackColor = SystemColors.ButtonHighlight;
            RegionPanel.Controls.Add(RegionResourceRarity);
            RegionPanel.Controls.Add(label4);
            RegionPanel.Controls.Add(NrRegionSpawnChance);
            RegionPanel.Controls.Add(label5);
            RegionPanel.Controls.Add(NrMaxRegionPerSector);
            RegionPanel.Controls.Add(label6);
            RegionPanel.Controls.Add(label14);
            RegionPanel.Location = new Point(302, 32);
            RegionPanel.Name = "RegionPanel";
            RegionPanel.Size = new Size(259, 220);
            RegionPanel.TabIndex = 37;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ButtonHighlight;
            panel1.Controls.Add(label19);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(CmbCustomFactionDistribution);
            panel1.Controls.Add(NrFactionMin);
            panel1.Controls.Add(NrFactionMax);
            panel1.Controls.Add(label17);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label18);
            panel1.Controls.Add(label9);
            panel1.Controls.Add(NrFacControlMax);
            panel1.Controls.Add(label16);
            panel1.Controls.Add(NrFacControlMin);
            panel1.Location = new Point(12, 318);
            panel1.Name = "panel1";
            panel1.Size = new Size(279, 92);
            panel1.TabIndex = 38;
            // 
            // ChkAutoSeed
            // 
            ChkAutoSeed.AutoSize = true;
            ChkAutoSeed.Checked = true;
            ChkAutoSeed.CheckState = CheckState.Checked;
            ChkAutoSeed.Location = new Point(170, 41);
            ChkAutoSeed.Name = "ChkAutoSeed";
            ChkAutoSeed.Size = new Size(82, 19);
            ChkAutoSeed.TabIndex = 39;
            ChkAutoSeed.Text = "Auto-Seed";
            ChkAutoSeed.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ButtonHighlight;
            panel2.Controls.Add(NrClusterChance);
            panel2.Controls.Add(label23);
            panel2.Controls.Add(NrGridHeight);
            panel2.Controls.Add(NrGridWidth);
            panel2.Controls.Add(label11);
            panel2.Controls.Add(label10);
            panel2.Controls.Add(CmbClusterDistribution);
            panel2.Controls.Add(label15);
            panel2.Controls.Add(label13);
            panel2.Controls.Add(NrChanceMultiSectors);
            panel2.Location = new Point(12, 64);
            panel2.Name = "panel2";
            panel2.Size = new Size(279, 123);
            panel2.TabIndex = 40;
            // 
            // NrGridHeight
            // 
            NrGridHeight.Location = new Point(197, 5);
            NrGridHeight.Name = "NrGridHeight";
            NrGridHeight.Size = new Size(41, 23);
            NrGridHeight.TabIndex = 33;
            NrGridHeight.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // NrGridWidth
            // 
            NrGridWidth.Location = new Point(77, 5);
            NrGridWidth.Name = "NrGridWidth";
            NrGridWidth.Size = new Size(41, 23);
            NrGridWidth.TabIndex = 32;
            NrGridWidth.Value = new decimal(new int[] { 10, 0, 0, 0 });
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
            panel3.Controls.Add(label12);
            panel3.Controls.Add(label20);
            panel3.Controls.Add(comboBox1);
            panel3.Controls.Add(NrMinGates);
            panel3.Controls.Add(NrMaxGates);
            panel3.Controls.Add(label22);
            panel3.Controls.Add(label24);
            panel3.Location = new Point(12, 218);
            panel3.Name = "panel3";
            panel3.Size = new Size(279, 66);
            panel3.TabIndex = 40;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(8, 39);
            label12.Name = "label12";
            label12.Size = new Size(72, 15);
            label12.TabIndex = 35;
            label12.Text = "Distribution:";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(7, 10);
            label20.Name = "label20";
            label20.Size = new Size(88, 15);
            label20.TabIndex = 15;
            label20.Text = "Max Per Sector:";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(100, 35);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(172, 23);
            comboBox1.TabIndex = 36;
            // 
            // NrMinGates
            // 
            NrMinGates.Location = new Point(154, 6);
            NrMinGates.Name = "NrMinGates";
            NrMinGates.Size = new Size(41, 23);
            NrMinGates.TabIndex = 16;
            NrMinGates.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // NrMaxGates
            // 
            NrMaxGates.Location = new Point(231, 6);
            NrMaxGates.Name = "NrMaxGates";
            NrMaxGates.Size = new Size(41, 23);
            NrMaxGates.TabIndex = 17;
            NrMaxGates.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(100, 10);
            label22.Name = "label22";
            label22.Size = new Size(52, 15);
            label22.TabIndex = 18;
            label22.Text = "between";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new Point(199, 10);
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
            ChkGenerateConnections.Location = new Point(17, 193);
            ChkGenerateConnections.Name = "ChkGenerateConnections";
            ChkGenerateConnections.Size = new Size(143, 19);
            ChkGenerateConnections.TabIndex = 39;
            ChkGenerateConnections.Text = "Generate Connections";
            ChkGenerateConnections.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.ButtonHighlight;
            panel4.Controls.Add(label21);
            panel4.Controls.Add(CmbVanillaFactionDistribution);
            panel4.Controls.Add(label25);
            panel4.Controls.Add(label27);
            panel4.Controls.Add(NrVanillaClusterMax);
            panel4.Controls.Add(label29);
            panel4.Controls.Add(NrVanillaClusterMin);
            panel4.Location = new Point(297, 286);
            panel4.Name = "panel4";
            panel4.Size = new Size(279, 62);
            panel4.TabIndex = 40;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(8, 35);
            label21.Name = "label21";
            label21.Size = new Size(72, 15);
            label21.TabIndex = 35;
            label21.Text = "Distribution:";
            // 
            // CmbVanillaFactionDistribution
            // 
            CmbVanillaFactionDistribution.FormattingEnabled = true;
            CmbVanillaFactionDistribution.Location = new Point(100, 31);
            CmbVanillaFactionDistribution.Name = "CmbVanillaFactionDistribution";
            CmbVanillaFactionDistribution.Size = new Size(172, 23);
            CmbVanillaFactionDistribution.TabIndex = 36;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new Point(199, 8);
            label25.Name = "label25";
            label25.Size = new Size(27, 15);
            label25.TabIndex = 34;
            label25.Text = "and";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new Point(100, 8);
            label27.Name = "label27";
            label27.Size = new Size(52, 15);
            label27.TabIndex = 33;
            label27.Text = "between";
            // 
            // NrVanillaClusterMax
            // 
            NrVanillaClusterMax.Location = new Point(231, 4);
            NrVanillaClusterMax.Name = "NrVanillaClusterMax";
            NrVanillaClusterMax.Size = new Size(41, 23);
            NrVanillaClusterMax.TabIndex = 32;
            NrVanillaClusterMax.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(7, 8);
            label29.Name = "label29";
            label29.Size = new Size(90, 15);
            label29.TabIndex = 30;
            label29.Text = "Cluster Control:";
            // 
            // NrVanillaClusterMin
            // 
            NrVanillaClusterMin.Location = new Point(154, 4);
            NrVanillaClusterMin.Name = "NrVanillaClusterMin";
            NrVanillaClusterMin.Size = new Size(41, 23);
            NrVanillaClusterMin.TabIndex = 31;
            NrVanillaClusterMin.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // ChkGenerateVanillaFactions
            // 
            ChkGenerateVanillaFactions.AutoSize = true;
            ChkGenerateVanillaFactions.Location = new Point(302, 261);
            ChkGenerateVanillaFactions.Name = "ChkGenerateVanillaFactions";
            ChkGenerateVanillaFactions.Size = new Size(157, 19);
            ChkGenerateVanillaFactions.TabIndex = 39;
            ChkGenerateVanillaFactions.Text = "Generate Vanilla Factions";
            ChkGenerateVanillaFactions.UseVisualStyleBackColor = true;
            // 
            // NrClusterChance
            // 
            NrClusterChance.Location = new Point(138, 61);
            NrClusterChance.Name = "NrClusterChance";
            NrClusterChance.Size = new Size(41, 23);
            NrClusterChance.TabIndex = 35;
            NrClusterChance.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(4, 65);
            label23.Name = "label23";
            label23.Size = new Size(133, 15);
            label23.TabIndex = 34;
            label23.Text = "Cluster Chance Per Hex:";
            // 
            // ProceduralGalaxyForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 457);
            Controls.Add(panel4);
            Controls.Add(ChkGenerateVanillaFactions);
            Controls.Add(panel3);
            Controls.Add(ChkGenerateConnections);
            Controls.Add(panel2);
            Controls.Add(ChkAutoSeed);
            Controls.Add(panel1);
            Controls.Add(RegionPanel);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(TxtSeed);
            Controls.Add(ChkCustomFactions);
            Controls.Add(ChkRegions);
            Controls.Add(label1);
            Controls.Add(BtnGenerate);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProceduralGalaxyForm";
            Text = "Procedural Galaxy Generator";
            ((System.ComponentModel.ISupportInitialize)NrRegionSpawnChance).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxRegionPerSector).EndInit();
            ((System.ComponentModel.ISupportInitialize)RegionResourceRarity).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrFactionMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrFactionMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrChanceMultiSectors).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMin).EndInit();
            RegionPanel.ResumeLayout(false);
            RegionPanel.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NrGridHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrGridWidth).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NrMinGates).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxGates).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NrVanillaClusterMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrVanillaClusterMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrClusterChance).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button BtnGenerate;
        private Label label1;
        private CheckBox ChkRegions;
        private CheckBox ChkCustomFactions;
        private TextBox TxtSeed;
        private Label label2;
        private Label label3;
        private Label label4;
        private NumericUpDown NrRegionSpawnChance;
        private NumericUpDown NrMaxRegionPerSector;
        private Label label5;
        private DataGridView RegionResourceRarity;
        private Label label6;
        private Label label7;
        private NumericUpDown NrFactionMin;
        private NumericUpDown NrFactionMax;
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
        private CheckBox ChkAutoSeed;
        private Panel panel2;
        private NumericUpDown NrGridHeight;
        private NumericUpDown NrGridWidth;
        private Label label11;
        private Label label10;
        private Panel panel3;
        private Label label12;
        private Label label20;
        private ComboBox comboBox1;
        private NumericUpDown NrMinGates;
        private NumericUpDown NrMaxGates;
        private Label label22;
        private Label label24;
        private CheckBox ChkGenerateConnections;
        private Panel panel4;
        private Label label21;
        private ComboBox CmbVanillaFactionDistribution;
        private Label label25;
        private Label label27;
        private NumericUpDown NrVanillaClusterMax;
        private Label label29;
        private NumericUpDown NrVanillaClusterMin;
        private CheckBox ChkGenerateVanillaFactions;
        private NumericUpDown NrClusterChance;
        private Label label23;
    }
}