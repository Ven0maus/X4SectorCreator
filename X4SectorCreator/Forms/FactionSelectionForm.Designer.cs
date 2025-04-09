﻿using X4SectorCreator.CustomComponents;

namespace X4SectorCreator.Forms
{
    partial class FactionSelectionForm
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
            BtnSave = new Button();
            CmbOwner = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            CmbFactions = new MultiSelectCombo.NoDropDownComboBox();
            SuspendLayout();
            // 
            // BtnSave
            // 
            BtnSave.Location = new Point(12, 110);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new Size(249, 30);
            BtnSave.TabIndex = 0;
            BtnSave.Text = "Save";
            BtnSave.UseVisualStyleBackColor = true;
            BtnSave.Click += BtnSave_Click;
            // 
            // CmbOwner
            // 
            CmbOwner.FormattingEnabled = true;
            CmbOwner.Location = new Point(12, 32);
            CmbOwner.Name = "CmbOwner";
            CmbOwner.Size = new Size(249, 23);
            CmbOwner.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(106, 20);
            label1.TabIndex = 2;
            label1.Text = "Owner Faction:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11F);
            label2.Location = new Point(12, 58);
            label2.Name = "label2";
            label2.Size = new Size(249, 20);
            label2.TabIndex = 3;
            label2.Text = "Spawn in space owned by faction(s):";
            // 
            // CmbFactions
            // 
            CmbFactions.FormattingEnabled = true;
            CmbFactions.Location = new Point(12, 81);
            CmbFactions.Name = "CmbFactions";
            CmbFactions.Size = new Size(249, 23);
            CmbFactions.TabIndex = 4;
            // 
            // FactionSelectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(271, 151);
            Controls.Add(CmbFactions);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(CmbOwner);
            Controls.Add(BtnSave);
            Name = "FactionSelectionForm";
            Text = "Select Faction";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnSave;
        private ComboBox CmbOwner;
        private Label label1;
        private Label label2;
        private MultiSelectCombo.NoDropDownComboBox CmbFactions;
    }
}