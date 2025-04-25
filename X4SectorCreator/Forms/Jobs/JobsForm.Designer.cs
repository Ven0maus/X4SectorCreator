namespace X4SectorCreator.Forms
{
    partial class JobsForm
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
            ListJobs = new ListBox();
            label1 = new Label();
            BtnCreateFromTemplate = new Button();
            BtnRemoveJob = new Button();
            BtnExitJobWindow = new Button();
            cmbFaction = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            cmbOrder = new ComboBox();
            label6 = new Label();
            cmbBasket = new ComboBox();
            cmbCluster = new ComboBox();
            label7 = new Label();
            cmbSector = new ComboBox();
            label8 = new Label();
            BtnResetFilter = new Button();
            BtnBaskets = new Button();
            label5 = new Label();
            BtnQuickQuotaEditor = new Button();
            label9 = new Label();
            TxtSearch = new TextBox();
            BtnCreateJobsFromPreset = new Button();
            BtnClearAllJobs = new Button();
            BtnAddSelection = new Button();
            SuspendLayout();
            // 
            // ListJobs
            // 
            ListJobs.FormattingEnabled = true;
            ListJobs.HorizontalScrollbar = true;
            ListJobs.Location = new Point(9, 70);
            ListJobs.Name = "ListJobs";
            ListJobs.Size = new Size(426, 424);
            ListJobs.TabIndex = 0;
            ListJobs.DoubleClick += ListJobs_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(9, 9);
            label1.Name = "label1";
            label1.Size = new Size(51, 28);
            label1.TabIndex = 1;
            label1.Text = "Jobs";
            // 
            // BtnCreateFromTemplate
            // 
            BtnCreateFromTemplate.Location = new Point(441, 32);
            BtnCreateFromTemplate.Name = "BtnCreateFromTemplate";
            BtnCreateFromTemplate.Size = new Size(321, 36);
            BtnCreateFromTemplate.TabIndex = 2;
            BtnCreateFromTemplate.Text = "Create Custom Job";
            BtnCreateFromTemplate.UseVisualStyleBackColor = true;
            BtnCreateFromTemplate.Click += BtnCreateFromTemplate_Click;
            // 
            // BtnRemoveJob
            // 
            BtnRemoveJob.Location = new Point(441, 146);
            BtnRemoveJob.Name = "BtnRemoveJob";
            BtnRemoveJob.Size = new Size(191, 36);
            BtnRemoveJob.TabIndex = 4;
            BtnRemoveJob.Text = "Remove Selected Job";
            BtnRemoveJob.UseVisualStyleBackColor = true;
            BtnRemoveJob.Click += BtnRemoveJob_Click;
            // 
            // BtnExitJobWindow
            // 
            BtnExitJobWindow.Location = new Point(441, 458);
            BtnExitJobWindow.Name = "BtnExitJobWindow";
            BtnExitJobWindow.Size = new Size(321, 36);
            BtnExitJobWindow.TabIndex = 5;
            BtnExitJobWindow.Text = "Exit Jobs Window";
            BtnExitJobWindow.UseVisualStyleBackColor = true;
            BtnExitJobWindow.Click += BtnExitJobWindow_Click;
            // 
            // cmbFaction
            // 
            cmbFaction.FormattingEnabled = true;
            cmbFaction.Location = new Point(509, 296);
            cmbFaction.Name = "cmbFaction";
            cmbFaction.Size = new Size(247, 23);
            cmbFaction.TabIndex = 6;
            cmbFaction.SelectedIndexChanged += CmbFaction_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14F, FontStyle.Underline);
            label2.Location = new Point(441, 264);
            label2.Name = "label2";
            label2.Size = new Size(152, 25);
            label2.TabIndex = 7;
            label2.Text = "Filtering Options";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(441, 294);
            label3.Name = "label3";
            label3.Size = new Size(62, 21);
            label3.TabIndex = 8;
            label3.Text = "Faction:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(449, 325);
            label4.Name = "label4";
            label4.Size = new Size(54, 21);
            label4.TabIndex = 9;
            label4.Text = "Order:";
            // 
            // cmbOrder
            // 
            cmbOrder.FormattingEnabled = true;
            cmbOrder.Location = new Point(509, 325);
            cmbOrder.Name = "cmbOrder";
            cmbOrder.Size = new Size(247, 23);
            cmbOrder.TabIndex = 10;
            cmbOrder.SelectedIndexChanged += CmbOrder_SelectedIndexChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(445, 355);
            label6.Name = "label6";
            label6.Size = new Size(58, 21);
            label6.TabIndex = 13;
            label6.Text = "Basket:";
            // 
            // cmbBasket
            // 
            cmbBasket.FormattingEnabled = true;
            cmbBasket.Location = new Point(509, 355);
            cmbBasket.Name = "cmbBasket";
            cmbBasket.Size = new Size(247, 23);
            cmbBasket.TabIndex = 14;
            cmbBasket.SelectedIndexChanged += CmbBasket_SelectedIndexChanged;
            // 
            // cmbCluster
            // 
            cmbCluster.FormattingEnabled = true;
            cmbCluster.Location = new Point(509, 386);
            cmbCluster.Name = "cmbCluster";
            cmbCluster.Size = new Size(247, 23);
            cmbCluster.TabIndex = 16;
            cmbCluster.SelectedIndexChanged += CmbCluster_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(441, 387);
            label7.Name = "label7";
            label7.Size = new Size(62, 21);
            label7.TabIndex = 15;
            label7.Text = "Cluster:";
            // 
            // cmbSector
            // 
            cmbSector.Enabled = false;
            cmbSector.FormattingEnabled = true;
            cmbSector.Location = new Point(509, 417);
            cmbSector.Name = "cmbSector";
            cmbSector.Size = new Size(247, 23);
            cmbSector.TabIndex = 18;
            cmbSector.SelectedIndexChanged += CmbSector_SelectedIndexChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(446, 415);
            label8.Name = "label8";
            label8.Size = new Size(57, 21);
            label8.TabIndex = 17;
            label8.Text = "Sector:";
            // 
            // BtnResetFilter
            // 
            BtnResetFilter.Location = new Point(599, 264);
            BtnResetFilter.Name = "BtnResetFilter";
            BtnResetFilter.Size = new Size(157, 25);
            BtnResetFilter.TabIndex = 19;
            BtnResetFilter.Text = "Reset Filter";
            BtnResetFilter.UseVisualStyleBackColor = true;
            BtnResetFilter.Click += BtnResetFilter_Click;
            // 
            // BtnBaskets
            // 
            BtnBaskets.Location = new Point(441, 185);
            BtnBaskets.Name = "BtnBaskets";
            BtnBaskets.Size = new Size(321, 36);
            BtnBaskets.TabIndex = 20;
            BtnBaskets.Text = "See Available Baskets";
            BtnBaskets.UseVisualStyleBackColor = true;
            BtnBaskets.Click += BtnBaskets_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10F);
            label5.Location = new Point(58, 15);
            label5.Name = "label5";
            label5.Size = new Size(377, 19);
            label5.TabIndex = 21;
            label5.Text = "(determines the amount and purpose of ships in the galaxy.)";
            // 
            // BtnQuickQuotaEditor
            // 
            BtnQuickQuotaEditor.Location = new Point(441, 224);
            BtnQuickQuotaEditor.Name = "BtnQuickQuotaEditor";
            BtnQuickQuotaEditor.Size = new Size(321, 36);
            BtnQuickQuotaEditor.TabIndex = 23;
            BtnQuickQuotaEditor.Text = "Quick Quota Editor";
            BtnQuickQuotaEditor.UseVisualStyleBackColor = true;
            BtnQuickQuotaEditor.Click += BtnQuickQuotaEditor_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(9, 42);
            label9.Name = "label9";
            label9.Size = new Size(60, 21);
            label9.TabIndex = 24;
            label9.Text = "Search:";
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(75, 42);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.Size = new Size(360, 23);
            TxtSearch.TabIndex = 25;
            // 
            // BtnCreateJobsFromPreset
            // 
            BtnCreateJobsFromPreset.Location = new Point(441, 108);
            BtnCreateJobsFromPreset.Name = "BtnCreateJobsFromPreset";
            BtnCreateJobsFromPreset.Size = new Size(321, 36);
            BtnCreateJobsFromPreset.TabIndex = 26;
            BtnCreateJobsFromPreset.Text = "Select Vanilla Preset";
            BtnCreateJobsFromPreset.UseVisualStyleBackColor = true;
            BtnCreateJobsFromPreset.Click += BtnCreateJobsFromPreset_Click;
            // 
            // BtnClearAllJobs
            // 
            BtnClearAllJobs.Location = new Point(638, 146);
            BtnClearAllJobs.Name = "BtnClearAllJobs";
            BtnClearAllJobs.Size = new Size(124, 36);
            BtnClearAllJobs.TabIndex = 27;
            BtnClearAllJobs.Text = "Clear All Jobs";
            BtnClearAllJobs.UseVisualStyleBackColor = true;
            BtnClearAllJobs.Click += BtnClearAllJobs_Click;
            // 
            // BtnAddSelection
            // 
            BtnAddSelection.Location = new Point(441, 70);
            BtnAddSelection.Name = "BtnAddSelection";
            BtnAddSelection.Size = new Size(321, 36);
            BtnAddSelection.TabIndex = 28;
            BtnAddSelection.Text = "Add Selection Of Jobs";
            BtnAddSelection.UseVisualStyleBackColor = true;
            BtnAddSelection.Click += BtnAddSelection_Click;
            // 
            // JobsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(766, 499);
            Controls.Add(BtnAddSelection);
            Controls.Add(BtnClearAllJobs);
            Controls.Add(BtnCreateJobsFromPreset);
            Controls.Add(TxtSearch);
            Controls.Add(label9);
            Controls.Add(BtnQuickQuotaEditor);
            Controls.Add(label5);
            Controls.Add(BtnBaskets);
            Controls.Add(BtnResetFilter);
            Controls.Add(cmbSector);
            Controls.Add(label8);
            Controls.Add(cmbCluster);
            Controls.Add(label7);
            Controls.Add(cmbBasket);
            Controls.Add(label6);
            Controls.Add(cmbOrder);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(cmbFaction);
            Controls.Add(BtnExitJobWindow);
            Controls.Add(BtnRemoveJob);
            Controls.Add(BtnCreateFromTemplate);
            Controls.Add(label1);
            Controls.Add(ListJobs);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "JobsForm";
            Text = "Jobs";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ListJobs;
        private Label label1;
        private Button BtnCreateFromTemplate;
        private Button BtnRemoveJob;
        private Button BtnExitJobWindow;
        private ComboBox cmbFaction;
        private Label label2;
        private Label label3;
        private Label label4;
        private ComboBox cmbOrder;
        private Label label6;
        private ComboBox cmbBasket;
        private ComboBox cmbCluster;
        private Label label7;
        private ComboBox cmbSector;
        private Label label8;
        private Button BtnResetFilter;
        private Button BtnBaskets;
        private Label label5;
        private Button BtnQuickQuotaEditor;
        private Label label9;
        private TextBox TxtSearch;
        private Button BtnCreateJobsFromPreset;
        private Button BtnClearAllJobs;
        private Button BtnAddSelection;
    }
}