namespace X4SectorCreator.Forms
{
    partial class FactionRelationsForm
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
            BtnUpdate = new Button();
            BtnCancel = new Button();
            label1 = new Label();
            ChkLockRelations = new CheckBox();
            CmbSelectedFaction = new ComboBox();
            RelationsPanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // BtnUpdate
            // 
            BtnUpdate.Location = new Point(184, 368);
            BtnUpdate.Name = "BtnUpdate";
            BtnUpdate.Size = new Size(274, 32);
            BtnUpdate.TabIndex = 0;
            BtnUpdate.Text = "Update";
            BtnUpdate.UseVisualStyleBackColor = true;
            BtnUpdate.Click += BtnUpdate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(6, 368);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(172, 32);
            BtnCancel.TabIndex = 1;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(6, 10);
            label1.Name = "label1";
            label1.Size = new Size(124, 21);
            label1.TabIndex = 3;
            label1.Text = "Selected Faction:";
            // 
            // ChkLockRelations
            // 
            ChkLockRelations.AutoSize = true;
            ChkLockRelations.Location = new Point(356, 13);
            ChkLockRelations.Name = "ChkLockRelations";
            ChkLockRelations.Size = new Size(102, 19);
            ChkLockRelations.TabIndex = 4;
            ChkLockRelations.Text = "Lock Relations";
            ChkLockRelations.UseVisualStyleBackColor = true;
            // 
            // CmbSelectedFaction
            // 
            CmbSelectedFaction.FormattingEnabled = true;
            CmbSelectedFaction.Location = new Point(135, 11);
            CmbSelectedFaction.Name = "CmbSelectedFaction";
            CmbSelectedFaction.Size = new Size(215, 23);
            CmbSelectedFaction.TabIndex = 5;
            CmbSelectedFaction.SelectedIndexChanged += CmbSelectedFaction_SelectedIndexChanged;
            // 
            // RelationsPanel
            // 
            RelationsPanel.AutoScroll = true;
            RelationsPanel.FlowDirection = FlowDirection.TopDown;
            RelationsPanel.Location = new Point(6, 40);
            RelationsPanel.Name = "RelationsPanel";
            RelationsPanel.Size = new Size(452, 322);
            RelationsPanel.TabIndex = 6;
            RelationsPanel.WrapContents = false;
            // 
            // FactionRelationsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(465, 405);
            Controls.Add(RelationsPanel);
            Controls.Add(CmbSelectedFaction);
            Controls.Add(ChkLockRelations);
            Controls.Add(label1);
            Controls.Add(BtnCancel);
            Controls.Add(BtnUpdate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactionRelationsForm";
            Text = "Faction Relations Editor";
            Load += FactionRelationsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnUpdate;
        private Button BtnCancel;
        private Label label1;
        private CheckBox ChkLockRelations;
        private ComboBox CmbSelectedFaction;
        private FlowLayoutPanel RelationsPanel;
    }
}