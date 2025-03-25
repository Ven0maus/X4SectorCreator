﻿namespace X4SectorCreator.Forms
{
    partial class FactoriesForm
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
            ListFactories = new ListBox();
            label1 = new Label();
            BtnCreateFromTemplate = new Button();
            BtnRemoveFactory = new Button();
            BtnExitFactoriesWindow = new Button();
            cmbFaction = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            cmbCluster = new ComboBox();
            label7 = new Label();
            cmbSector = new ComboBox();
            label8 = new Label();
            BtnResetFilter = new Button();
            label5 = new Label();
            SuspendLayout();
            // 
            // ListFactories
            // 
            ListFactories.FormattingEnabled = true;
            ListFactories.Location = new Point(12, 40);
            ListFactories.Name = "ListFactories";
            ListFactories.Size = new Size(259, 394);
            ListFactories.TabIndex = 0;
            ListFactories.DoubleClick += ListJobs_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(89, 28);
            label1.TabIndex = 1;
            label1.Text = "Factories";
            // 
            // BtnCreateFromTemplate
            // 
            BtnCreateFromTemplate.Location = new Point(277, 40);
            BtnCreateFromTemplate.Name = "BtnCreateFromTemplate";
            BtnCreateFromTemplate.Size = new Size(321, 78);
            BtnCreateFromTemplate.TabIndex = 2;
            BtnCreateFromTemplate.Text = "Create New Factory From Template";
            BtnCreateFromTemplate.UseVisualStyleBackColor = true;
            BtnCreateFromTemplate.Click += BtnCreateFromTemplate_Click;
            // 
            // BtnRemoveFactory
            // 
            BtnRemoveFactory.Location = new Point(277, 124);
            BtnRemoveFactory.Name = "BtnRemoveFactory";
            BtnRemoveFactory.Size = new Size(321, 36);
            BtnRemoveFactory.TabIndex = 4;
            BtnRemoveFactory.Text = "Remove Selected Factory";
            BtnRemoveFactory.UseVisualStyleBackColor = true;
            BtnRemoveFactory.Click += BtnRemoveJob_Click;
            // 
            // BtnExitFactoriesWindow
            // 
            BtnExitFactoriesWindow.Location = new Point(277, 284);
            BtnExitFactoriesWindow.Name = "BtnExitFactoriesWindow";
            BtnExitFactoriesWindow.Size = new Size(321, 36);
            BtnExitFactoriesWindow.TabIndex = 5;
            BtnExitFactoriesWindow.Text = "Exit Factories Window";
            BtnExitFactoriesWindow.UseVisualStyleBackColor = true;
            BtnExitFactoriesWindow.Click += BtnExitJobWindow_Click;
            // 
            // cmbFaction
            // 
            cmbFaction.FormattingEnabled = true;
            cmbFaction.Location = new Point(345, 195);
            cmbFaction.Name = "cmbFaction";
            cmbFaction.Size = new Size(247, 23);
            cmbFaction.TabIndex = 6;
            cmbFaction.SelectedIndexChanged += CmbFaction_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14F, FontStyle.Underline);
            label2.Location = new Point(277, 163);
            label2.Name = "label2";
            label2.Size = new Size(152, 25);
            label2.TabIndex = 7;
            label2.Text = "Filtering Options";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(277, 193);
            label3.Name = "label3";
            label3.Size = new Size(62, 21);
            label3.TabIndex = 8;
            label3.Text = "Faction:";
            // 
            // cmbCluster
            // 
            cmbCluster.FormattingEnabled = true;
            cmbCluster.Location = new Point(345, 223);
            cmbCluster.Name = "cmbCluster";
            cmbCluster.Size = new Size(247, 23);
            cmbCluster.TabIndex = 16;
            cmbCluster.SelectedIndexChanged += CmbCluster_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(277, 223);
            label7.Name = "label7";
            label7.Size = new Size(62, 21);
            label7.TabIndex = 15;
            label7.Text = "Cluster:";
            // 
            // cmbSector
            // 
            cmbSector.Enabled = false;
            cmbSector.FormattingEnabled = true;
            cmbSector.Location = new Point(345, 255);
            cmbSector.Name = "cmbSector";
            cmbSector.Size = new Size(247, 23);
            cmbSector.TabIndex = 18;
            cmbSector.SelectedIndexChanged += CmbSector_SelectedIndexChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(282, 256);
            label8.Name = "label8";
            label8.Size = new Size(57, 21);
            label8.TabIndex = 17;
            label8.Text = "Sector:";
            // 
            // BtnResetFilter
            // 
            BtnResetFilter.Location = new Point(435, 163);
            BtnResetFilter.Name = "BtnResetFilter";
            BtnResetFilter.Size = new Size(157, 25);
            BtnResetFilter.TabIndex = 19;
            BtnResetFilter.Text = "Reset Filter";
            BtnResetFilter.UseVisualStyleBackColor = true;
            BtnResetFilter.Click += BtnResetFilter_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10F);
            label5.Location = new Point(103, 15);
            label5.Name = "label5";
            label5.Size = new Size(359, 19);
            label5.TabIndex = 21;
            label5.Text = "(determines the amount of factory stations in the galaxy.)";
            // 
            // ProductsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(604, 440);
            Controls.Add(label5);
            Controls.Add(BtnResetFilter);
            Controls.Add(cmbSector);
            Controls.Add(label8);
            Controls.Add(cmbCluster);
            Controls.Add(label7);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(cmbFaction);
            Controls.Add(BtnExitFactoriesWindow);
            Controls.Add(BtnRemoveFactory);
            Controls.Add(BtnCreateFromTemplate);
            Controls.Add(label1);
            Controls.Add(ListFactories);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProductsForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ListFactories;
        private Label label1;
        private Button BtnCreateFromTemplate;
        private Button BtnRemoveFactory;
        private Button BtnExitFactoriesWindow;
        private ComboBox cmbFaction;
        private Label label2;
        private Label label3;
        private ComboBox cmbCluster;
        private Label label7;
        private ComboBox cmbSector;
        private Label label8;
        private Button BtnResetFilter;
        private Label label5;
    }
}