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
            ChkFactions = new CheckBox();
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
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            NrClustersMax = new NumericUpDown();
            NrClustersMin = new NumericUpDown();
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
            CmbFactionDistribution = new ComboBox();
            RegionPanel = new Panel();
            panel1 = new Panel();
            ChkAutoSeed = new CheckBox();
            panel2 = new Panel();
            ((System.ComponentModel.ISupportInitialize)NrRegionSpawnChance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrMaxRegionPerSector).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RegionResourceRarity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFactionMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFactionMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrClustersMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrClustersMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrChanceMultiSectors).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMin).BeginInit();
            RegionPanel.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 274);
            button1.Name = "button1";
            button1.Size = new Size(251, 35);
            button1.TabIndex = 0;
            button1.Text = "Show Sector Map";
            button1.UseVisualStyleBackColor = true;
            // 
            // BtnGenerate
            // 
            BtnGenerate.Location = new Point(12, 233);
            BtnGenerate.Name = "BtnGenerate";
            BtnGenerate.Size = new Size(251, 35);
            BtnGenerate.TabIndex = 1;
            BtnGenerate.Text = "Generate";
            BtnGenerate.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.Location = new Point(12, 314);
            label1.Name = "label1";
            label1.Size = new Size(251, 63);
            label1.TabIndex = 2;
            label1.Text = "Steps:\r\n1. Set Generation Options\r\n2. Open sector map to view live updates.\r\n3. Click generate as many times as you want.";
            // 
            // ChkRegions
            // 
            ChkRegions.AutoSize = true;
            ChkRegions.Checked = true;
            ChkRegions.CheckState = CheckState.Checked;
            ChkRegions.Location = new Point(289, 10);
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
            ChkFactions.Location = new Point(294, 260);
            ChkFactions.Name = "ChkFactions";
            ChkFactions.Size = new Size(120, 19);
            ChkFactions.TabIndex = 4;
            ChkFactions.Text = "Generate Factions";
            ChkFactions.UseVisualStyleBackColor = true;
            // 
            // TxtSeed
            // 
            TxtSeed.Location = new Point(48, 10);
            TxtSeed.Name = "TxtSeed";
            TxtSeed.Size = new Size(139, 23);
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
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(4, 10);
            label10.Name = "label10";
            label10.Size = new Size(81, 15);
            label10.TabIndex = 20;
            label10.Text = "Total Clusters:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(184, 10);
            label11.Name = "label11";
            label11.Size = new Size(27, 15);
            label11.TabIndex = 24;
            label11.Text = "and";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(85, 10);
            label12.Name = "label12";
            label12.Size = new Size(52, 15);
            label12.TabIndex = 23;
            label12.Text = "between";
            // 
            // NrClustersMax
            // 
            NrClustersMax.Location = new Point(216, 6);
            NrClustersMax.Name = "NrClustersMax";
            NrClustersMax.Size = new Size(41, 23);
            NrClustersMax.TabIndex = 22;
            NrClustersMax.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // NrClustersMin
            // 
            NrClustersMin.Location = new Point(139, 6);
            NrClustersMin.Name = "NrClustersMin";
            NrClustersMin.Size = new Size(41, 23);
            NrClustersMin.TabIndex = 21;
            NrClustersMin.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(4, 39);
            label13.Name = "label13";
            label13.Size = new Size(179, 15);
            label13.TabIndex = 25;
            label13.Text = "Chance for multi-sector clusters:";
            // 
            // NrChanceMultiSectors
            // 
            NrChanceMultiSectors.Location = new Point(184, 37);
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
            label15.Location = new Point(4, 69);
            label15.Name = "label15";
            label15.Size = new Size(72, 15);
            label15.TabIndex = 28;
            label15.Text = "Distribution:";
            // 
            // CmbClusterDistribution
            // 
            CmbClusterDistribution.FormattingEnabled = true;
            CmbClusterDistribution.Location = new Point(82, 66);
            CmbClusterDistribution.Name = "CmbClusterDistribution";
            CmbClusterDistribution.Size = new Size(143, 23);
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
            label19.Location = new Point(8, 64);
            label19.Name = "label19";
            label19.Size = new Size(72, 15);
            label19.TabIndex = 35;
            label19.Text = "Distribution:";
            // 
            // CmbFactionDistribution
            // 
            CmbFactionDistribution.FormattingEnabled = true;
            CmbFactionDistribution.Location = new Point(100, 61);
            CmbFactionDistribution.Name = "CmbFactionDistribution";
            CmbFactionDistribution.Size = new Size(172, 23);
            CmbFactionDistribution.TabIndex = 36;
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
            RegionPanel.Location = new Point(289, 34);
            RegionPanel.Name = "RegionPanel";
            RegionPanel.Size = new Size(259, 220);
            RegionPanel.TabIndex = 37;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ButtonHighlight;
            panel1.Controls.Add(label19);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(CmbFactionDistribution);
            panel1.Controls.Add(NrFactionMin);
            panel1.Controls.Add(NrFactionMax);
            panel1.Controls.Add(label17);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label18);
            panel1.Controls.Add(label9);
            panel1.Controls.Add(NrFacControlMax);
            panel1.Controls.Add(label16);
            panel1.Controls.Add(NrFacControlMin);
            panel1.Location = new Point(289, 285);
            panel1.Name = "panel1";
            panel1.Size = new Size(279, 92);
            panel1.TabIndex = 38;
            // 
            // ChkAutoSeed
            // 
            ChkAutoSeed.AutoSize = true;
            ChkAutoSeed.Checked = true;
            ChkAutoSeed.CheckState = CheckState.Checked;
            ChkAutoSeed.Location = new Point(192, 14);
            ChkAutoSeed.Name = "ChkAutoSeed";
            ChkAutoSeed.Size = new Size(82, 19);
            ChkAutoSeed.TabIndex = 39;
            ChkAutoSeed.Text = "Auto-Seed";
            ChkAutoSeed.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ButtonHighlight;
            panel2.Controls.Add(label10);
            panel2.Controls.Add(NrClustersMin);
            panel2.Controls.Add(NrClustersMax);
            panel2.Controls.Add(label12);
            panel2.Controls.Add(CmbClusterDistribution);
            panel2.Controls.Add(label11);
            panel2.Controls.Add(label15);
            panel2.Controls.Add(label13);
            panel2.Controls.Add(NrChanceMultiSectors);
            panel2.Location = new Point(12, 64);
            panel2.Name = "panel2";
            panel2.Size = new Size(262, 163);
            panel2.TabIndex = 40;
            // 
            // ProceduralGalaxyForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(573, 382);
            Controls.Add(panel2);
            Controls.Add(ChkAutoSeed);
            Controls.Add(panel1);
            Controls.Add(RegionPanel);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(TxtSeed);
            Controls.Add(ChkFactions);
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
            ((System.ComponentModel.ISupportInitialize)NrClustersMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrClustersMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrChanceMultiSectors).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)NrFacControlMin).EndInit();
            RegionPanel.ResumeLayout(false);
            RegionPanel.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button BtnGenerate;
        private Label label1;
        private CheckBox ChkRegions;
        private CheckBox ChkFactions;
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
        private Label label10;
        private Label label11;
        private Label label12;
        private NumericUpDown NrClustersMax;
        private NumericUpDown NrClustersMin;
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
        private ComboBox CmbFactionDistribution;
        private Panel RegionPanel;
        private Panel panel1;
        private CheckBox ChkAutoSeed;
        private Panel panel2;
    }
}