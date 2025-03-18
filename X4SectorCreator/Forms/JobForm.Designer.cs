namespace X4SectorCreator.Forms
{
    partial class JobForm
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
            BtnCreateJob = new Button();
            BtnCancelJob = new Button();
            label1 = new Label();
            TxtShipName = new TextBox();
            TxtJobName = new TextBox();
            label2 = new Label();
            ChkRebuild = new CheckBox();
            ChkCommandeerable = new CheckBox();
            ChkBuildAtShipyard = new CheckBox();
            label3 = new Label();
            CmbOrder = new ComboBox();
            ListOrderParams = new ListBox();
            label4 = new Label();
            BtnAddParam = new Button();
            BtnRemoveParam = new Button();
            ChkSubordinate = new CheckBox();
            CmbBasket = new ComboBox();
            label5 = new Label();
            label6 = new Label();
            CmbCategoryFaction = new ComboBox();
            label7 = new Label();
            CmbCategorySize = new ComboBox();
            label8 = new Label();
            TxtCategoryTags = new TextBox();
            label9 = new Label();
            TxtQuotaGalaxy = new TextBox();
            label10 = new Label();
            TxtQuotaCluster = new TextBox();
            label11 = new Label();
            TxtQuotaSector = new TextBox();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            CmbLocationClass = new ComboBox();
            label15 = new Label();
            CmbLocation = new ComboBox();
            label16 = new Label();
            CmbLocationFaction = new ComboBox();
            label17 = new Label();
            CmbRelation = new ComboBox();
            label18 = new Label();
            TxtQuotaWing = new TextBox();
            label19 = new Label();
            TxtComparison = new TextBox();
            label20 = new Label();
            label21 = new Label();
            TxtShipTags = new TextBox();
            label22 = new Label();
            CmbShipSize = new ComboBox();
            label23 = new Label();
            CmbShipFaction = new ComboBox();
            label24 = new Label();
            CmbOwner = new ComboBox();
            label25 = new Label();
            TxtShipExact = new TextBox();
            label26 = new Label();
            TxtShipMax = new TextBox();
            label27 = new Label();
            TxtShipMin = new TextBox();
            label28 = new Label();
            BtnEditBaskets = new Button();
            label29 = new Label();
            CmbRegionBasket = new ComboBox();
            SuspendLayout();
            // 
            // BtnCreateJob
            // 
            BtnCreateJob.Location = new Point(418, 522);
            BtnCreateJob.Name = "BtnCreateJob";
            BtnCreateJob.Size = new Size(190, 47);
            BtnCreateJob.TabIndex = 0;
            BtnCreateJob.Text = "Create Job";
            BtnCreateJob.UseVisualStyleBackColor = true;
            // 
            // BtnCancelJob
            // 
            BtnCancelJob.Location = new Point(418, 575);
            BtnCancelJob.Name = "BtnCancelJob";
            BtnCancelJob.Size = new Size(190, 24);
            BtnCancelJob.TabIndex = 1;
            BtnCancelJob.Text = "Cancel";
            BtnCancelJob.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 59);
            label1.Name = "label1";
            label1.Size = new Size(133, 21);
            label1.TabIndex = 2;
            label1.Text = "Ship Suffix Name:";
            // 
            // TxtShipName
            // 
            TxtShipName.Location = new Point(12, 83);
            TxtShipName.Name = "TxtShipName";
            TxtShipName.Size = new Size(193, 23);
            TxtShipName.TabIndex = 3;
            // 
            // TxtJobName
            // 
            TxtJobName.Location = new Point(12, 33);
            TxtJobName.Name = "TxtJobName";
            TxtJobName.Size = new Size(193, 23);
            TxtJobName.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(83, 21);
            label2.TabIndex = 4;
            label2.Text = "Job Name:";
            // 
            // ChkRebuild
            // 
            ChkRebuild.AutoSize = true;
            ChkRebuild.Font = new Font("Segoe UI", 12F);
            ChkRebuild.Location = new Point(434, 398);
            ChkRebuild.Name = "ChkRebuild";
            ChkRebuild.Size = new Size(82, 25);
            ChkRebuild.TabIndex = 6;
            ChkRebuild.Text = "Rebuild";
            ChkRebuild.UseVisualStyleBackColor = true;
            // 
            // ChkCommandeerable
            // 
            ChkCommandeerable.AutoSize = true;
            ChkCommandeerable.Font = new Font("Segoe UI", 12F);
            ChkCommandeerable.Location = new Point(433, 429);
            ChkCommandeerable.Name = "ChkCommandeerable";
            ChkCommandeerable.Size = new Size(153, 25);
            ChkCommandeerable.TabIndex = 7;
            ChkCommandeerable.Text = "Commandeerable";
            ChkCommandeerable.UseVisualStyleBackColor = true;
            // 
            // ChkBuildAtShipyard
            // 
            ChkBuildAtShipyard.AutoSize = true;
            ChkBuildAtShipyard.Font = new Font("Segoe UI", 12F);
            ChkBuildAtShipyard.Location = new Point(433, 460);
            ChkBuildAtShipyard.Name = "ChkBuildAtShipyard";
            ChkBuildAtShipyard.Size = new Size(149, 25);
            ChkBuildAtShipyard.TabIndex = 8;
            ChkBuildAtShipyard.Text = "Build At Shipyard";
            ChkBuildAtShipyard.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label3.Location = new Point(213, 375);
            label3.Name = "label3";
            label3.Size = new Size(86, 21);
            label3.TabIndex = 10;
            label3.Text = "Ship Order";
            // 
            // CmbOrder
            // 
            CmbOrder.FormattingEnabled = true;
            CmbOrder.Location = new Point(213, 399);
            CmbOrder.Name = "CmbOrder";
            CmbOrder.Size = new Size(196, 23);
            CmbOrder.TabIndex = 11;
            // 
            // ListOrderParams
            // 
            ListOrderParams.FormattingEnabled = true;
            ListOrderParams.Location = new Point(213, 449);
            ListOrderParams.Name = "ListOrderParams";
            ListOrderParams.Size = new Size(196, 109);
            ListOrderParams.TabIndex = 12;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(213, 425);
            label4.Name = "label4";
            label4.Size = new Size(136, 21);
            label4.TabIndex = 13;
            label4.Text = "Order Parameters:";
            // 
            // BtnAddParam
            // 
            BtnAddParam.Location = new Point(285, 564);
            BtnAddParam.Name = "BtnAddParam";
            BtnAddParam.Size = new Size(124, 27);
            BtnAddParam.TabIndex = 14;
            BtnAddParam.Text = "Add";
            BtnAddParam.UseVisualStyleBackColor = true;
            // 
            // BtnRemoveParam
            // 
            BtnRemoveParam.Location = new Point(213, 564);
            BtnRemoveParam.Name = "BtnRemoveParam";
            BtnRemoveParam.Size = new Size(66, 27);
            BtnRemoveParam.TabIndex = 15;
            BtnRemoveParam.Text = "Remove";
            BtnRemoveParam.UseVisualStyleBackColor = true;
            // 
            // ChkSubordinate
            // 
            ChkSubordinate.AutoSize = true;
            ChkSubordinate.Font = new Font("Segoe UI", 12F);
            ChkSubordinate.Location = new Point(433, 491);
            ChkSubordinate.Name = "ChkSubordinate";
            ChkSubordinate.Size = new Size(114, 25);
            ChkSubordinate.TabIndex = 16;
            ChkSubordinate.Text = "Subordinate";
            ChkSubordinate.UseVisualStyleBackColor = true;
            // 
            // CmbBasket
            // 
            CmbBasket.FormattingEnabled = true;
            CmbBasket.Location = new Point(12, 144);
            CmbBasket.Name = "CmbBasket";
            CmbBasket.Size = new Size(193, 23);
            CmbBasket.TabIndex = 18;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(12, 120);
            label5.Name = "label5";
            label5.Size = new Size(58, 21);
            label5.TabIndex = 17;
            label5.Text = "Basket:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label6.Location = new Point(12, 171);
            label6.Name = "label6";
            label6.Size = new Size(73, 21);
            label6.TabIndex = 19;
            label6.Text = "Category";
            // 
            // CmbCategoryFaction
            // 
            CmbCategoryFaction.FormattingEnabled = true;
            CmbCategoryFaction.Location = new Point(12, 224);
            CmbCategoryFaction.Name = "CmbCategoryFaction";
            CmbCategoryFaction.Size = new Size(193, 23);
            CmbCategoryFaction.TabIndex = 21;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(12, 200);
            label7.Name = "label7";
            label7.Size = new Size(62, 21);
            label7.TabIndex = 20;
            label7.Text = "Faction:";
            // 
            // CmbCategorySize
            // 
            CmbCategorySize.FormattingEnabled = true;
            CmbCategorySize.Location = new Point(12, 323);
            CmbCategorySize.Name = "CmbCategorySize";
            CmbCategorySize.Size = new Size(193, 23);
            CmbCategorySize.TabIndex = 23;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(12, 299);
            label8.Name = "label8";
            label8.Size = new Size(41, 21);
            label8.TabIndex = 22;
            label8.Text = "Size:";
            // 
            // TxtCategoryTags
            // 
            TxtCategoryTags.Location = new Point(12, 274);
            TxtCategoryTags.Name = "TxtCategoryTags";
            TxtCategoryTags.Size = new Size(193, 23);
            TxtCategoryTags.TabIndex = 25;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(12, 250);
            label9.Name = "label9";
            label9.Size = new Size(43, 21);
            label9.TabIndex = 24;
            label9.Text = "Tags:";
            // 
            // TxtQuotaGalaxy
            // 
            TxtQuotaGalaxy.Location = new Point(12, 412);
            TxtQuotaGalaxy.Name = "TxtQuotaGalaxy";
            TxtQuotaGalaxy.Size = new Size(193, 23);
            TxtQuotaGalaxy.TabIndex = 27;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F);
            label10.Location = new Point(12, 388);
            label10.Name = "label10";
            label10.Size = new Size(59, 21);
            label10.TabIndex = 26;
            label10.Text = "Galaxy:";
            // 
            // TxtQuotaCluster
            // 
            TxtQuotaCluster.Location = new Point(12, 462);
            TxtQuotaCluster.Name = "TxtQuotaCluster";
            TxtQuotaCluster.Size = new Size(193, 23);
            TxtQuotaCluster.TabIndex = 29;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(12, 438);
            label11.Name = "label11";
            label11.Size = new Size(62, 21);
            label11.TabIndex = 28;
            label11.Text = "Cluster:";
            // 
            // TxtQuotaSector
            // 
            TxtQuotaSector.Location = new Point(12, 512);
            TxtQuotaSector.Name = "TxtQuotaSector";
            TxtQuotaSector.Size = new Size(193, 23);
            TxtQuotaSector.TabIndex = 31;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 12F);
            label12.Location = new Point(12, 488);
            label12.Name = "label12";
            label12.Size = new Size(57, 21);
            label12.TabIndex = 30;
            label12.Text = "Sector:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label13.Location = new Point(12, 360);
            label13.Name = "label13";
            label13.Size = new Size(53, 21);
            label13.TabIndex = 32;
            label13.Text = "Quota";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label14.Location = new Point(217, 9);
            label14.Name = "label14";
            label14.Size = new Size(69, 21);
            label14.TabIndex = 33;
            label14.Text = "Location";
            // 
            // CmbLocationClass
            // 
            CmbLocationClass.FormattingEnabled = true;
            CmbLocationClass.Location = new Point(214, 62);
            CmbLocationClass.Name = "CmbLocationClass";
            CmbLocationClass.Size = new Size(193, 23);
            CmbLocationClass.TabIndex = 35;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 12F);
            label15.Location = new Point(214, 38);
            label15.Name = "label15";
            label15.Size = new Size(49, 21);
            label15.TabIndex = 34;
            label15.Text = "Class:";
            // 
            // CmbLocation
            // 
            CmbLocation.FormattingEnabled = true;
            CmbLocation.Location = new Point(214, 112);
            CmbLocation.Name = "CmbLocation";
            CmbLocation.Size = new Size(193, 23);
            CmbLocation.TabIndex = 37;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 12F);
            label16.Location = new Point(214, 88);
            label16.Name = "label16";
            label16.Size = new Size(72, 21);
            label16.TabIndex = 36;
            label16.Text = "Location:";
            // 
            // CmbLocationFaction
            // 
            CmbLocationFaction.FormattingEnabled = true;
            CmbLocationFaction.Location = new Point(214, 161);
            CmbLocationFaction.Name = "CmbLocationFaction";
            CmbLocationFaction.Size = new Size(193, 23);
            CmbLocationFaction.TabIndex = 39;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 12F);
            label17.Location = new Point(214, 137);
            label17.Name = "label17";
            label17.Size = new Size(62, 21);
            label17.TabIndex = 38;
            label17.Text = "Faction:";
            // 
            // CmbRelation
            // 
            CmbRelation.FormattingEnabled = true;
            CmbRelation.Location = new Point(214, 212);
            CmbRelation.Name = "CmbRelation";
            CmbRelation.Size = new Size(193, 23);
            CmbRelation.TabIndex = 41;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 12F);
            label18.Location = new Point(214, 188);
            label18.Name = "label18";
            label18.Size = new Size(70, 21);
            label18.TabIndex = 40;
            label18.Text = "Relation:";
            // 
            // TxtQuotaWing
            // 
            TxtQuotaWing.Location = new Point(12, 566);
            TxtQuotaWing.Name = "TxtQuotaWing";
            TxtQuotaWing.Size = new Size(193, 23);
            TxtQuotaWing.TabIndex = 43;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 12F);
            label19.Location = new Point(12, 542);
            label19.Name = "label19";
            label19.Size = new Size(50, 21);
            label19.TabIndex = 42;
            label19.Text = "Wing:";
            // 
            // TxtComparison
            // 
            TxtComparison.Location = new Point(214, 262);
            TxtComparison.Name = "TxtComparison";
            TxtComparison.Size = new Size(193, 23);
            TxtComparison.TabIndex = 45;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new Font("Segoe UI", 12F);
            label20.Location = new Point(214, 238);
            label20.Name = "label20";
            label20.Size = new Size(98, 21);
            label20.TabIndex = 44;
            label20.Text = "Comparison:";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label21.Location = new Point(418, 11);
            label21.Name = "label21";
            label21.Size = new Size(41, 21);
            label21.TabIndex = 46;
            label21.Text = "Ship";
            // 
            // TxtShipTags
            // 
            TxtShipTags.Location = new Point(415, 112);
            TxtShipTags.Name = "TxtShipTags";
            TxtShipTags.Size = new Size(193, 23);
            TxtShipTags.TabIndex = 52;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Font = new Font("Segoe UI", 12F);
            label22.Location = new Point(415, 88);
            label22.Name = "label22";
            label22.Size = new Size(43, 21);
            label22.TabIndex = 51;
            label22.Text = "Tags:";
            // 
            // CmbShipSize
            // 
            CmbShipSize.FormattingEnabled = true;
            CmbShipSize.Location = new Point(415, 161);
            CmbShipSize.Name = "CmbShipSize";
            CmbShipSize.Size = new Size(193, 23);
            CmbShipSize.TabIndex = 50;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Font = new Font("Segoe UI", 12F);
            label23.Location = new Point(415, 137);
            label23.Name = "label23";
            label23.Size = new Size(41, 21);
            label23.TabIndex = 49;
            label23.Text = "Size:";
            // 
            // CmbShipFaction
            // 
            CmbShipFaction.FormattingEnabled = true;
            CmbShipFaction.Location = new Point(415, 62);
            CmbShipFaction.Name = "CmbShipFaction";
            CmbShipFaction.Size = new Size(193, 23);
            CmbShipFaction.TabIndex = 48;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Font = new Font("Segoe UI", 12F);
            label24.Location = new Point(415, 38);
            label24.Name = "label24";
            label24.Size = new Size(62, 21);
            label24.TabIndex = 47;
            label24.Text = "Faction:";
            // 
            // CmbOwner
            // 
            CmbOwner.FormattingEnabled = true;
            CmbOwner.Location = new Point(412, 212);
            CmbOwner.Name = "CmbOwner";
            CmbOwner.Size = new Size(193, 23);
            CmbOwner.TabIndex = 54;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Font = new Font("Segoe UI", 12F);
            label25.Location = new Point(412, 188);
            label25.Name = "label25";
            label25.Size = new Size(60, 21);
            label25.TabIndex = 53;
            label25.Text = "Owner:";
            // 
            // TxtShipExact
            // 
            TxtShipExact.Location = new Point(412, 366);
            TxtShipExact.Name = "TxtShipExact";
            TxtShipExact.Size = new Size(193, 23);
            TxtShipExact.TabIndex = 60;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Font = new Font("Segoe UI", 12F);
            label26.Location = new Point(412, 342);
            label26.Name = "label26";
            label26.Size = new Size(48, 21);
            label26.TabIndex = 59;
            label26.Text = "Exact:";
            // 
            // TxtShipMax
            // 
            TxtShipMax.Location = new Point(412, 312);
            TxtShipMax.Name = "TxtShipMax";
            TxtShipMax.Size = new Size(193, 23);
            TxtShipMax.TabIndex = 58;
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Font = new Font("Segoe UI", 12F);
            label27.Location = new Point(412, 288);
            label27.Name = "label27";
            label27.Size = new Size(42, 21);
            label27.TabIndex = 57;
            label27.Text = "Max:";
            // 
            // TxtShipMin
            // 
            TxtShipMin.Location = new Point(412, 262);
            TxtShipMin.Name = "TxtShipMin";
            TxtShipMin.Size = new Size(193, 23);
            TxtShipMin.TabIndex = 56;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Font = new Font("Segoe UI", 12F);
            label28.Location = new Point(412, 238);
            label28.Name = "label28";
            label28.Size = new Size(40, 21);
            label28.TabIndex = 55;
            label28.Text = "Min:";
            // 
            // BtnEditBaskets
            // 
            BtnEditBaskets.Location = new Point(107, 112);
            BtnEditBaskets.Name = "BtnEditBaskets";
            BtnEditBaskets.Size = new Size(98, 29);
            BtnEditBaskets.TabIndex = 61;
            BtnEditBaskets.Text = "Edit Baskets";
            BtnEditBaskets.UseVisualStyleBackColor = true;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Font = new Font("Segoe UI", 12F);
            label29.Location = new Point(213, 292);
            label29.Name = "label29";
            label29.Size = new Size(107, 21);
            label29.TabIndex = 62;
            label29.Text = "RegionBasket:";
            // 
            // CmbRegionBasket
            // 
            CmbRegionBasket.FormattingEnabled = true;
            CmbRegionBasket.Items.AddRange(new object[] { "minerals", "gases" });
            CmbRegionBasket.Location = new Point(214, 323);
            CmbRegionBasket.Name = "CmbRegionBasket";
            CmbRegionBasket.Size = new Size(193, 23);
            CmbRegionBasket.TabIndex = 63;
            // 
            // JobForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(623, 603);
            Controls.Add(CmbRegionBasket);
            Controls.Add(label29);
            Controls.Add(BtnEditBaskets);
            Controls.Add(TxtShipExact);
            Controls.Add(label26);
            Controls.Add(TxtShipMax);
            Controls.Add(label27);
            Controls.Add(TxtShipMin);
            Controls.Add(label28);
            Controls.Add(CmbOwner);
            Controls.Add(label25);
            Controls.Add(TxtShipTags);
            Controls.Add(label22);
            Controls.Add(CmbShipSize);
            Controls.Add(label23);
            Controls.Add(CmbShipFaction);
            Controls.Add(label24);
            Controls.Add(label21);
            Controls.Add(TxtComparison);
            Controls.Add(label20);
            Controls.Add(TxtQuotaWing);
            Controls.Add(label19);
            Controls.Add(CmbRelation);
            Controls.Add(label18);
            Controls.Add(CmbLocationFaction);
            Controls.Add(label17);
            Controls.Add(CmbLocation);
            Controls.Add(label16);
            Controls.Add(CmbLocationClass);
            Controls.Add(label15);
            Controls.Add(label14);
            Controls.Add(label13);
            Controls.Add(TxtQuotaSector);
            Controls.Add(label12);
            Controls.Add(TxtQuotaCluster);
            Controls.Add(label11);
            Controls.Add(TxtQuotaGalaxy);
            Controls.Add(label10);
            Controls.Add(TxtCategoryTags);
            Controls.Add(label9);
            Controls.Add(CmbCategorySize);
            Controls.Add(label8);
            Controls.Add(CmbCategoryFaction);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(CmbBasket);
            Controls.Add(label5);
            Controls.Add(ChkSubordinate);
            Controls.Add(BtnRemoveParam);
            Controls.Add(BtnAddParam);
            Controls.Add(label4);
            Controls.Add(ListOrderParams);
            Controls.Add(CmbOrder);
            Controls.Add(label3);
            Controls.Add(ChkBuildAtShipyard);
            Controls.Add(ChkCommandeerable);
            Controls.Add(ChkRebuild);
            Controls.Add(TxtJobName);
            Controls.Add(label2);
            Controls.Add(TxtShipName);
            Controls.Add(label1);
            Controls.Add(BtnCancelJob);
            Controls.Add(BtnCreateJob);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "JobForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnCreateJob;
        private Button BtnCancelJob;
        private Label label1;
        private TextBox TxtShipName;
        private TextBox TxtJobName;
        private Label label2;
        private CheckBox ChkRebuild;
        private CheckBox ChkCommandeerable;
        private CheckBox ChkBuildAtShipyard;
        private Label label3;
        private ComboBox CmbOrder;
        private ListBox ListOrderParams;
        private Label label4;
        private Button BtnAddParam;
        private Button BtnRemoveParam;
        private CheckBox ChkSubordinate;
        private ComboBox CmbBasket;
        private Label label5;
        private Label label6;
        private ComboBox CmbCategoryFaction;
        private Label label7;
        private ComboBox CmbCategorySize;
        private Label label8;
        private TextBox TxtCategoryTags;
        private Label label9;
        private TextBox TxtQuotaGalaxy;
        private Label label10;
        private TextBox TxtQuotaCluster;
        private Label label11;
        private TextBox TxtQuotaSector;
        private Label label12;
        private Label label13;
        private Label label14;
        private ComboBox CmbLocationClass;
        private Label label15;
        private ComboBox CmbLocation;
        private Label label16;
        private ComboBox CmbLocationFaction;
        private Label label17;
        private ComboBox CmbRelation;
        private Label label18;
        private TextBox TxtQuotaWing;
        private Label label19;
        private TextBox TxtComparison;
        private Label label20;
        private Label label21;
        private TextBox TxtShipTags;
        private Label label22;
        private ComboBox CmbShipSize;
        private Label label23;
        private ComboBox CmbShipFaction;
        private Label label24;
        private ComboBox CmbOwner;
        private Label label25;
        private TextBox TxtShipExact;
        private Label label26;
        private TextBox TxtShipMax;
        private Label label27;
        private TextBox TxtShipMin;
        private Label label28;
        private Button BtnEditBaskets;
        private Label label29;
        private ComboBox CmbRegionBasket;
    }
}