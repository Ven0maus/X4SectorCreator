namespace X4SectorCreator.Forms.Factories
{
    partial class PresetSelectionForm
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
            label1 = new Label();
            CmbFaction = new ComboBox();
            label2 = new Label();
            TxtSectorCoverage = new TextBox();
            label3 = new Label();
            CmbFactions = new CustomComponents.MultiSelectCombo.NoDropDownComboBox();
            label4 = new Label();
            label5 = new Label();
            CmbOwner = new ComboBox();
            label6 = new Label();
            BtnConfirm = new Button();
            BtnCancel = new Button();
            label7 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(10, 30);
            label1.Name = "label1";
            label1.Size = new Size(155, 21);
            label1.TabIndex = 0;
            label1.Text = "Use preset of faction:";
            // 
            // CmbFaction
            // 
            CmbFaction.FormattingEnabled = true;
            CmbFaction.Location = new Point(10, 54);
            CmbFaction.Name = "CmbFaction";
            CmbFaction.Size = new Size(249, 23);
            CmbFaction.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(10, 202);
            label2.Name = "label2";
            label2.Size = new Size(180, 21);
            label2.TabIndex = 2;
            label2.Text = "Total Sector(s) Coverage:";
            // 
            // TxtSectorCoverage
            // 
            TxtSectorCoverage.Location = new Point(10, 254);
            TxtSectorCoverage.Name = "TxtSectorCoverage";
            TxtSectorCoverage.Size = new Size(249, 23);
            TxtSectorCoverage.TabIndex = 3;
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 8F);
            label3.Location = new Point(10, 223);
            label3.Name = "label3";
            label3.Size = new Size(180, 28);
            label3.TabIndex = 4;
            label3.Text = "(The total expected sector ownership for the faction)";
            // 
            // CmbFactions
            // 
            CmbFactions.FormattingEnabled = true;
            CmbFactions.Location = new Point(10, 176);
            CmbFactions.Name = "CmbFactions";
            CmbFactions.Size = new Size(249, 23);
            CmbFactions.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 11F);
            label4.Location = new Point(10, 153);
            label4.Name = "label4";
            label4.Size = new Size(249, 20);
            label4.TabIndex = 7;
            label4.Text = "Spawn in space owned by faction(s):";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11F);
            label5.Location = new Point(10, 104);
            label5.Name = "label5";
            label5.Size = new Size(106, 20);
            label5.TabIndex = 6;
            label5.Text = "Owner Faction:";
            // 
            // CmbOwner
            // 
            CmbOwner.FormattingEnabled = true;
            CmbOwner.Location = new Point(10, 127);
            CmbOwner.Name = "CmbOwner";
            CmbOwner.Size = new Size(249, 23);
            CmbOwner.TabIndex = 5;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label6.Location = new Point(10, 83);
            label6.Name = "label6";
            label6.Size = new Size(131, 21);
            label6.TabIndex = 9;
            label6.Text = "New Faction Data";
            // 
            // BtnConfirm
            // 
            BtnConfirm.Location = new Point(99, 283);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(160, 38);
            BtnConfirm.TabIndex = 10;
            BtnConfirm.Text = "Confirm Selection";
            BtnConfirm.UseVisualStyleBackColor = true;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(10, 283);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(83, 38);
            BtnCancel.TabIndex = 11;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // label7
            // 
            label7.Font = new Font("Segoe UI", 8F);
            label7.Location = new Point(6, 9);
            label7.Name = "label7";
            label7.Size = new Size(258, 21);
            label7.TabIndex = 12;
            label7.Text = "(Selects all templates with location class \"galaxy\")";
            // 
            // PresetSelectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(271, 326);
            Controls.Add(label7);
            Controls.Add(BtnCancel);
            Controls.Add(BtnConfirm);
            Controls.Add(label6);
            Controls.Add(CmbFactions);
            Controls.Add(label4);
            Controls.Add(label5);
            Controls.Add(CmbOwner);
            Controls.Add(label3);
            Controls.Add(TxtSectorCoverage);
            Controls.Add(label2);
            Controls.Add(CmbFaction);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PresetSelectionForm";
            Text = "Preset Selector";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox CmbFaction;
        private Label label2;
        private TextBox TxtSectorCoverage;
        private Label label3;
        private CustomComponents.MultiSelectCombo.NoDropDownComboBox CmbFactions;
        private Label label4;
        private Label label5;
        private ComboBox CmbOwner;
        private Label label6;
        private Button BtnConfirm;
        private Button BtnCancel;
        private Label label7;
    }
}