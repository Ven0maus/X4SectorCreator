using X4SectorCreator.CustomComponents;

namespace X4SectorCreator.Forms.Factions
{
    partial class ShipForm
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
            TxtId = new TextBox();
            TxtGroup = new TextBox();
            label2 = new Label();
            label3 = new Label();
            CmbCatTags = new MultiSelectCombo.NoDropDownComboBox();
            CmbCatFactions = new MultiSelectCombo.NoDropDownComboBox();
            label4 = new Label();
            CmbCatSize = new ComboBox();
            label5 = new Label();
            CmbPilotFaction = new ComboBox();
            label6 = new Label();
            CmbPilotTags = new MultiSelectCombo.NoDropDownComboBox();
            label7 = new Label();
            CmbBasket = new ComboBox();
            label8 = new Label();
            CmbDrop = new ComboBox();
            label9 = new Label();
            CmbPeople = new ComboBox();
            label10 = new Label();
            BtnCreate = new Button();
            BtnCancel = new Button();
            BtnSelectCustomGroup = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(58, 20);
            label1.TabIndex = 0;
            label1.Text = "Ship Id:";
            // 
            // TxtId
            // 
            TxtId.Location = new Point(12, 32);
            TxtId.Name = "TxtId";
            TxtId.Size = new Size(197, 23);
            TxtId.TabIndex = 1;
            // 
            // TxtGroup
            // 
            TxtGroup.Location = new Point(215, 32);
            TxtGroup.Name = "TxtGroup";
            TxtGroup.Size = new Size(197, 23);
            TxtGroup.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11F);
            label2.Location = new Point(215, 9);
            label2.Name = "label2";
            label2.Size = new Size(86, 20);
            label2.TabIndex = 2;
            label2.Text = "Ship Group:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 11F);
            label3.Location = new Point(12, 58);
            label3.Name = "label3";
            label3.Size = new Size(105, 20);
            label3.TabIndex = 4;
            label3.Text = "Category Tags:";
            // 
            // CmbCatTags
            // 
            CmbCatTags.FormattingEnabled = true;
            CmbCatTags.Location = new Point(12, 81);
            CmbCatTags.Name = "CmbCatTags";
            CmbCatTags.Size = new Size(197, 23);
            CmbCatTags.TabIndex = 5;
            // 
            // CmbCatFactions
            // 
            CmbCatFactions.FormattingEnabled = true;
            CmbCatFactions.Location = new Point(215, 81);
            CmbCatFactions.Name = "CmbCatFactions";
            CmbCatFactions.Size = new Size(197, 23);
            CmbCatFactions.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 11F);
            label4.Location = new Point(215, 58);
            label4.Name = "label4";
            label4.Size = new Size(129, 20);
            label4.TabIndex = 6;
            label4.Text = "Category Factions:";
            // 
            // CmbCatSize
            // 
            CmbCatSize.FormattingEnabled = true;
            CmbCatSize.Location = new Point(422, 81);
            CmbCatSize.Name = "CmbCatSize";
            CmbCatSize.Size = new Size(197, 23);
            CmbCatSize.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11F);
            label5.Location = new Point(422, 58);
            label5.Name = "label5";
            label5.Size = new Size(103, 20);
            label5.TabIndex = 8;
            label5.Text = "Category Size:";
            // 
            // CmbPilotFaction
            // 
            CmbPilotFaction.FormattingEnabled = true;
            CmbPilotFaction.Location = new Point(215, 130);
            CmbPilotFaction.Name = "CmbPilotFaction";
            CmbPilotFaction.Size = new Size(197, 23);
            CmbPilotFaction.TabIndex = 13;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 11F);
            label6.Location = new Point(215, 107);
            label6.Name = "label6";
            label6.Size = new Size(93, 20);
            label6.TabIndex = 12;
            label6.Text = "Pilot Faction:";
            // 
            // CmbPilotTags
            // 
            CmbPilotTags.FormattingEnabled = true;
            CmbPilotTags.Location = new Point(12, 130);
            CmbPilotTags.Name = "CmbPilotTags";
            CmbPilotTags.Size = new Size(197, 23);
            CmbPilotTags.TabIndex = 11;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 11F);
            label7.Location = new Point(12, 107);
            label7.Name = "label7";
            label7.Size = new Size(75, 20);
            label7.TabIndex = 10;
            label7.Text = "Pilot Tags:";
            // 
            // CmbBasket
            // 
            CmbBasket.FormattingEnabled = true;
            CmbBasket.Location = new Point(12, 179);
            CmbBasket.Name = "CmbBasket";
            CmbBasket.Size = new Size(197, 23);
            CmbBasket.TabIndex = 15;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 11F);
            label8.Location = new Point(12, 156);
            label8.Name = "label8";
            label8.Size = new Size(55, 20);
            label8.TabIndex = 14;
            label8.Text = "Basket:";
            // 
            // CmbDrop
            // 
            CmbDrop.FormattingEnabled = true;
            CmbDrop.Location = new Point(215, 179);
            CmbDrop.Name = "CmbDrop";
            CmbDrop.Size = new Size(197, 23);
            CmbDrop.TabIndex = 17;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 11F);
            label9.Location = new Point(215, 156);
            label9.Name = "label9";
            label9.Size = new Size(46, 20);
            label9.TabIndex = 16;
            label9.Text = "Drop:";
            // 
            // CmbPeople
            // 
            CmbPeople.FormattingEnabled = true;
            CmbPeople.Location = new Point(422, 179);
            CmbPeople.Name = "CmbPeople";
            CmbPeople.Size = new Size(197, 23);
            CmbPeople.TabIndex = 19;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 11F);
            label10.Location = new Point(422, 156);
            label10.Name = "label10";
            label10.Size = new Size(57, 20);
            label10.TabIndex = 18;
            label10.Text = "People:";
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(215, 212);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(404, 36);
            BtnCreate.TabIndex = 20;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 212);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(197, 36);
            BtnCancel.TabIndex = 21;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnSelectCustomGroup
            // 
            BtnSelectCustomGroup.Location = new Point(307, 6);
            BtnSelectCustomGroup.Name = "BtnSelectCustomGroup";
            BtnSelectCustomGroup.Size = new Size(105, 23);
            BtnSelectCustomGroup.TabIndex = 22;
            BtnSelectCustomGroup.Text = "Select Custom";
            BtnSelectCustomGroup.UseVisualStyleBackColor = true;
            BtnSelectCustomGroup.Click += BtnSelectCustomGroup_Click;
            // 
            // ShipForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(631, 256);
            Controls.Add(BtnSelectCustomGroup);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            Controls.Add(CmbPeople);
            Controls.Add(label10);
            Controls.Add(CmbDrop);
            Controls.Add(label9);
            Controls.Add(CmbBasket);
            Controls.Add(label8);
            Controls.Add(CmbPilotFaction);
            Controls.Add(label6);
            Controls.Add(CmbPilotTags);
            Controls.Add(label7);
            Controls.Add(CmbCatSize);
            Controls.Add(label5);
            Controls.Add(CmbCatFactions);
            Controls.Add(label4);
            Controls.Add(CmbCatTags);
            Controls.Add(label3);
            Controls.Add(TxtGroup);
            Controls.Add(label2);
            Controls.Add(TxtId);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ShipForm";
            Text = "Ship Editor Form";
            Load += ShipForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox TxtId;
        private TextBox TxtGroup;
        private Label label2;
        private Label label3;
        private MultiSelectCombo.NoDropDownComboBox CmbCatTags;
        private MultiSelectCombo.NoDropDownComboBox CmbCatFactions;
        private Label label4;
        private ComboBox CmbCatSize;
        private Label label5;
        private ComboBox CmbPilotFaction;
        private Label label6;
        private MultiSelectCombo.NoDropDownComboBox CmbPilotTags;
        private Label label7;
        private ComboBox CmbBasket;
        private Label label8;
        private ComboBox CmbDrop;
        private Label label9;
        private ComboBox CmbPeople;
        private Label label10;
        private Button BtnCreate;
        private Button BtnCancel;
        private Button BtnSelectCustomGroup;
    }
}