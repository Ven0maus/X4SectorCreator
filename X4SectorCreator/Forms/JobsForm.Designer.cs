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
            BtnCreateCustom = new Button();
            BtnRemoveJob = new Button();
            BtnExitJobWindow = new Button();
            cmbFaction = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            cmbOrder = new ComboBox();
            cmbCommandeerable = new ComboBox();
            label5 = new Label();
            label6 = new Label();
            cmbBasket = new ComboBox();
            cmbCluster = new ComboBox();
            label7 = new Label();
            cmbSector = new ComboBox();
            label8 = new Label();
            BtnResetFilter = new Button();
            BtnBaskets = new Button();
            SuspendLayout();
            // 
            // ListJobs
            // 
            ListJobs.FormattingEnabled = true;
            ListJobs.Location = new Point(12, 40);
            ListJobs.Name = "ListJobs";
            ListJobs.Size = new Size(259, 424);
            ListJobs.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(51, 28);
            label1.TabIndex = 1;
            label1.Text = "Jobs";
            // 
            // BtnCreateFromTemplate
            // 
            BtnCreateFromTemplate.Location = new Point(277, 83);
            BtnCreateFromTemplate.Name = "BtnCreateFromTemplate";
            BtnCreateFromTemplate.Size = new Size(321, 36);
            BtnCreateFromTemplate.TabIndex = 2;
            BtnCreateFromTemplate.Text = "Create New Job From Template";
            BtnCreateFromTemplate.UseVisualStyleBackColor = true;
            // 
            // BtnCreateCustom
            // 
            BtnCreateCustom.Location = new Point(277, 41);
            BtnCreateCustom.Name = "BtnCreateCustom";
            BtnCreateCustom.Size = new Size(321, 36);
            BtnCreateCustom.TabIndex = 3;
            BtnCreateCustom.Text = "Create New Custom Job";
            BtnCreateCustom.UseVisualStyleBackColor = true;
            // 
            // BtnRemoveJob
            // 
            BtnRemoveJob.Location = new Point(277, 125);
            BtnRemoveJob.Name = "BtnRemoveJob";
            BtnRemoveJob.Size = new Size(321, 36);
            BtnRemoveJob.TabIndex = 4;
            BtnRemoveJob.Text = "Remove Selected Job";
            BtnRemoveJob.UseVisualStyleBackColor = true;
            // 
            // BtnExitJobWindow
            // 
            BtnExitJobWindow.Location = new Point(277, 428);
            BtnExitJobWindow.Name = "BtnExitJobWindow";
            BtnExitJobWindow.Size = new Size(321, 36);
            BtnExitJobWindow.TabIndex = 5;
            BtnExitJobWindow.Text = "Exit Jobs Window";
            BtnExitJobWindow.UseVisualStyleBackColor = true;
            // 
            // cmbFaction
            // 
            cmbFaction.FormattingEnabled = true;
            cmbFaction.Location = new Point(345, 242);
            cmbFaction.Name = "cmbFaction";
            cmbFaction.Size = new Size(247, 23);
            cmbFaction.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14F, FontStyle.Underline);
            label2.Location = new Point(277, 210);
            label2.Name = "label2";
            label2.Size = new Size(152, 25);
            label2.TabIndex = 7;
            label2.Text = "Filtering Options";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(277, 240);
            label3.Name = "label3";
            label3.Size = new Size(62, 21);
            label3.TabIndex = 8;
            label3.Text = "Faction:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(285, 271);
            label4.Name = "label4";
            label4.Size = new Size(54, 21);
            label4.TabIndex = 9;
            label4.Text = "Order:";
            // 
            // cmbOrder
            // 
            cmbOrder.FormattingEnabled = true;
            cmbOrder.Location = new Point(345, 271);
            cmbOrder.Name = "cmbOrder";
            cmbOrder.Size = new Size(247, 23);
            cmbOrder.TabIndex = 10;
            // 
            // cmbCommandeerable
            // 
            cmbCommandeerable.FormattingEnabled = true;
            cmbCommandeerable.Location = new Point(421, 302);
            cmbCommandeerable.Name = "cmbCommandeerable";
            cmbCommandeerable.Size = new Size(171, 23);
            cmbCommandeerable.TabIndex = 12;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(278, 304);
            label5.Name = "label5";
            label5.Size = new Size(137, 21);
            label5.TabIndex = 11;
            label5.Text = "Commandeerable:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(281, 332);
            label6.Name = "label6";
            label6.Size = new Size(58, 21);
            label6.TabIndex = 13;
            label6.Text = "Basket:";
            // 
            // cmbBasket
            // 
            cmbBasket.FormattingEnabled = true;
            cmbBasket.Location = new Point(345, 332);
            cmbBasket.Name = "cmbBasket";
            cmbBasket.Size = new Size(247, 23);
            cmbBasket.TabIndex = 14;
            // 
            // cmbCluster
            // 
            cmbCluster.FormattingEnabled = true;
            cmbCluster.Location = new Point(345, 363);
            cmbCluster.Name = "cmbCluster";
            cmbCluster.Size = new Size(247, 23);
            cmbCluster.TabIndex = 16;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(277, 364);
            label7.Name = "label7";
            label7.Size = new Size(62, 21);
            label7.TabIndex = 15;
            label7.Text = "Cluster:";
            // 
            // cmbSector
            // 
            cmbSector.FormattingEnabled = true;
            cmbSector.Location = new Point(345, 394);
            cmbSector.Name = "cmbSector";
            cmbSector.Size = new Size(247, 23);
            cmbSector.TabIndex = 18;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(282, 392);
            label8.Name = "label8";
            label8.Size = new Size(57, 21);
            label8.TabIndex = 17;
            label8.Text = "Sector:";
            // 
            // BtnResetFilter
            // 
            BtnResetFilter.Location = new Point(435, 210);
            BtnResetFilter.Name = "BtnResetFilter";
            BtnResetFilter.Size = new Size(157, 25);
            BtnResetFilter.TabIndex = 19;
            BtnResetFilter.Text = "Reset Filter";
            BtnResetFilter.UseVisualStyleBackColor = true;
            // 
            // BtnBaskets
            // 
            BtnBaskets.Location = new Point(277, 167);
            BtnBaskets.Name = "BtnBaskets";
            BtnBaskets.Size = new Size(321, 36);
            BtnBaskets.TabIndex = 20;
            BtnBaskets.Text = "Baskets";
            BtnBaskets.UseVisualStyleBackColor = true;
            // 
            // JobsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(604, 470);
            Controls.Add(BtnBaskets);
            Controls.Add(BtnResetFilter);
            Controls.Add(cmbSector);
            Controls.Add(label8);
            Controls.Add(cmbCluster);
            Controls.Add(label7);
            Controls.Add(cmbBasket);
            Controls.Add(label6);
            Controls.Add(cmbCommandeerable);
            Controls.Add(label5);
            Controls.Add(cmbOrder);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(cmbFaction);
            Controls.Add(BtnExitJobWindow);
            Controls.Add(BtnRemoveJob);
            Controls.Add(BtnCreateCustom);
            Controls.Add(BtnCreateFromTemplate);
            Controls.Add(label1);
            Controls.Add(ListJobs);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "JobsForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ListJobs;
        private Label label1;
        private Button BtnCreateFromTemplate;
        private Button BtnCreateCustom;
        private Button BtnRemoveJob;
        private Button BtnExitJobWindow;
        private ComboBox cmbFaction;
        private Label label2;
        private Label label3;
        private Label label4;
        private ComboBox cmbOrder;
        private ComboBox cmbCommandeerable;
        private Label label5;
        private Label label6;
        private ComboBox cmbBasket;
        private ComboBox cmbCluster;
        private Label label7;
        private ComboBox cmbSector;
        private Label label8;
        private Button BtnResetFilter;
        private Button BtnBaskets;
    }
}