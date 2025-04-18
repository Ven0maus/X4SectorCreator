namespace X4SectorCreator.Forms
{
    partial class FactionShipsForm
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
            BtnUseFactionPreset = new Button();
            ShipGroupsListBox = new ListBox();
            label1 = new Label();
            label2 = new Label();
            ShipsListBox = new ListBox();
            BtnCreateGroup = new Button();
            BtnDeleteGroup = new Button();
            BtnDeleteShip = new Button();
            BtnCreateShip = new Button();
            BtnExit = new Button();
            BtnConfirm = new Button();
            SuspendLayout();
            // 
            // BtnUseFactionPreset
            // 
            BtnUseFactionPreset.Location = new Point(12, 350);
            BtnUseFactionPreset.Name = "BtnUseFactionPreset";
            BtnUseFactionPreset.Size = new Size(230, 36);
            BtnUseFactionPreset.TabIndex = 0;
            BtnUseFactionPreset.Text = "Use Faction Preset";
            BtnUseFactionPreset.UseVisualStyleBackColor = true;
            BtnUseFactionPreset.Click += BtnUseFactionPreset_Click;
            // 
            // ShipGroupsListBox
            // 
            ShipGroupsListBox.FormattingEnabled = true;
            ShipGroupsListBox.Location = new Point(12, 28);
            ShipGroupsListBox.Name = "ShipGroupsListBox";
            ShipGroupsListBox.Size = new Size(230, 274);
            ShipGroupsListBox.TabIndex = 1;
            ShipGroupsListBox.DoubleClick += ShipGroupsListBox_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 10);
            label1.Name = "label1";
            label1.Size = new Size(71, 15);
            label1.TabIndex = 2;
            label1.Text = "Ship Groups";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(248, 10);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 4;
            label2.Text = "Ships";
            // 
            // ShipsListBox
            // 
            ShipsListBox.FormattingEnabled = true;
            ShipsListBox.Location = new Point(248, 28);
            ShipsListBox.Name = "ShipsListBox";
            ShipsListBox.Size = new Size(230, 274);
            ShipsListBox.TabIndex = 3;
            ShipsListBox.SelectedIndexChanged += ShipsListBox_SelectedIndexChanged;
            // 
            // BtnCreateGroup
            // 
            BtnCreateGroup.Location = new Point(12, 308);
            BtnCreateGroup.Name = "BtnCreateGroup";
            BtnCreateGroup.Size = new Size(132, 36);
            BtnCreateGroup.TabIndex = 5;
            BtnCreateGroup.Text = "Create New Group";
            BtnCreateGroup.UseVisualStyleBackColor = true;
            BtnCreateGroup.Click += BtnCreateGroup_Click;
            // 
            // BtnDeleteGroup
            // 
            BtnDeleteGroup.Location = new Point(150, 308);
            BtnDeleteGroup.Name = "BtnDeleteGroup";
            BtnDeleteGroup.Size = new Size(92, 36);
            BtnDeleteGroup.TabIndex = 6;
            BtnDeleteGroup.Text = "Delete";
            BtnDeleteGroup.UseVisualStyleBackColor = true;
            BtnDeleteGroup.Click += BtnDeleteGroup_Click;
            // 
            // BtnDeleteShip
            // 
            BtnDeleteShip.Location = new Point(386, 308);
            BtnDeleteShip.Name = "BtnDeleteShip";
            BtnDeleteShip.Size = new Size(92, 36);
            BtnDeleteShip.TabIndex = 8;
            BtnDeleteShip.Text = "Delete";
            BtnDeleteShip.UseVisualStyleBackColor = true;
            BtnDeleteShip.Click += BtnDeleteShip_Click;
            // 
            // BtnCreateShip
            // 
            BtnCreateShip.Location = new Point(248, 308);
            BtnCreateShip.Name = "BtnCreateShip";
            BtnCreateShip.Size = new Size(132, 36);
            BtnCreateShip.TabIndex = 7;
            BtnCreateShip.Text = "Create New Ship";
            BtnCreateShip.UseVisualStyleBackColor = true;
            BtnCreateShip.Click += BtnCreateShip_Click;
            // 
            // BtnExit
            // 
            BtnExit.Location = new Point(248, 350);
            BtnExit.Name = "BtnExit";
            BtnExit.Size = new Size(92, 36);
            BtnExit.TabIndex = 9;
            BtnExit.Text = "Exit";
            BtnExit.UseVisualStyleBackColor = true;
            BtnExit.Click += BtnExit_Click;
            // 
            // BtnConfirm
            // 
            BtnConfirm.Location = new Point(346, 350);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(132, 36);
            BtnConfirm.TabIndex = 10;
            BtnConfirm.Text = "Confirm";
            BtnConfirm.UseVisualStyleBackColor = true;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // FactionShipsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(483, 390);
            Controls.Add(BtnConfirm);
            Controls.Add(BtnExit);
            Controls.Add(BtnDeleteShip);
            Controls.Add(BtnCreateShip);
            Controls.Add(BtnDeleteGroup);
            Controls.Add(BtnCreateGroup);
            Controls.Add(label2);
            Controls.Add(ShipsListBox);
            Controls.Add(label1);
            Controls.Add(ShipGroupsListBox);
            Controls.Add(BtnUseFactionPreset);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactionShipsForm";
            Text = "Faction Ships Editor";
            Load += FactionShipsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnUseFactionPreset;
        private Label label1;
        private Label label2;
        private Button BtnCreateGroup;
        private Button BtnDeleteGroup;
        private Button BtnDeleteShip;
        private Button BtnCreateShip;
        private Button BtnExit;
        internal ListBox ShipGroupsListBox;
        internal ListBox ShipsListBox;
        private Button BtnConfirm;
    }
}