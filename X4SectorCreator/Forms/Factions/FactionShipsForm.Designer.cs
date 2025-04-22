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
            BtnClearAllGroups = new Button();
            BtnClearAllShips = new Button();
            SuspendLayout();
            // 
            // BtnUseFactionPreset
            // 
            BtnUseFactionPreset.Location = new Point(8, 530);
            BtnUseFactionPreset.Name = "BtnUseFactionPreset";
            BtnUseFactionPreset.Size = new Size(619, 36);
            BtnUseFactionPreset.TabIndex = 0;
            BtnUseFactionPreset.Text = "Use Faction Preset";
            BtnUseFactionPreset.UseVisualStyleBackColor = true;
            BtnUseFactionPreset.Click += BtnUseFactionPreset_Click;
            // 
            // ShipGroupsListBox
            // 
            ShipGroupsListBox.FormattingEnabled = true;
            ShipGroupsListBox.HorizontalScrollbar = true;
            ShipGroupsListBox.Location = new Point(8, 28);
            ShipGroupsListBox.Name = "ShipGroupsListBox";
            ShipGroupsListBox.Size = new Size(450, 454);
            ShipGroupsListBox.TabIndex = 1;
            ShipGroupsListBox.DoubleClick += ShipGroupsListBox_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 10);
            label1.Name = "label1";
            label1.Size = new Size(71, 15);
            label1.TabIndex = 2;
            label1.Text = "Ship Groups";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(465, 10);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 4;
            label2.Text = "Ships";
            // 
            // ShipsListBox
            // 
            ShipsListBox.FormattingEnabled = true;
            ShipsListBox.HorizontalScrollbar = true;
            ShipsListBox.Location = new Point(465, 28);
            ShipsListBox.Name = "ShipsListBox";
            ShipsListBox.Size = new Size(450, 454);
            ShipsListBox.TabIndex = 3;
            ShipsListBox.DoubleClick += ShipsListBox_DoubleClick;
            // 
            // BtnCreateGroup
            // 
            BtnCreateGroup.Location = new Point(8, 488);
            BtnCreateGroup.Name = "BtnCreateGroup";
            BtnCreateGroup.Size = new Size(352, 36);
            BtnCreateGroup.TabIndex = 5;
            BtnCreateGroup.Text = "Create New Group";
            BtnCreateGroup.UseVisualStyleBackColor = true;
            BtnCreateGroup.Click += BtnCreateGroup_Click;
            // 
            // BtnDeleteGroup
            // 
            BtnDeleteGroup.Location = new Point(366, 488);
            BtnDeleteGroup.Name = "BtnDeleteGroup";
            BtnDeleteGroup.Size = new Size(92, 36);
            BtnDeleteGroup.TabIndex = 6;
            BtnDeleteGroup.Text = "Delete";
            BtnDeleteGroup.UseVisualStyleBackColor = true;
            BtnDeleteGroup.Click += BtnDeleteGroup_Click;
            // 
            // BtnDeleteShip
            // 
            BtnDeleteShip.Location = new Point(823, 488);
            BtnDeleteShip.Name = "BtnDeleteShip";
            BtnDeleteShip.Size = new Size(92, 36);
            BtnDeleteShip.TabIndex = 8;
            BtnDeleteShip.Text = "Delete";
            BtnDeleteShip.UseVisualStyleBackColor = true;
            BtnDeleteShip.Click += BtnDeleteShip_Click;
            // 
            // BtnCreateShip
            // 
            BtnCreateShip.Location = new Point(466, 488);
            BtnCreateShip.Name = "BtnCreateShip";
            BtnCreateShip.Size = new Size(351, 36);
            BtnCreateShip.TabIndex = 7;
            BtnCreateShip.Text = "Create New Ship";
            BtnCreateShip.UseVisualStyleBackColor = true;
            BtnCreateShip.Click += BtnCreateShip_Click;
            // 
            // BtnExit
            // 
            BtnExit.Location = new Point(823, 530);
            BtnExit.Name = "BtnExit";
            BtnExit.Size = new Size(92, 36);
            BtnExit.TabIndex = 9;
            BtnExit.Text = "Exit";
            BtnExit.UseVisualStyleBackColor = true;
            BtnExit.Click += BtnExit_Click;
            // 
            // BtnConfirm
            // 
            BtnConfirm.Location = new Point(633, 530);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(184, 36);
            BtnConfirm.TabIndex = 10;
            BtnConfirm.Text = "Confirm";
            BtnConfirm.UseVisualStyleBackColor = true;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // BtnClearAllGroups
            // 
            BtnClearAllGroups.Location = new Point(395, 3);
            BtnClearAllGroups.Name = "BtnClearAllGroups";
            BtnClearAllGroups.Size = new Size(63, 22);
            BtnClearAllGroups.TabIndex = 11;
            BtnClearAllGroups.Text = "Clear All";
            BtnClearAllGroups.UseVisualStyleBackColor = true;
            BtnClearAllGroups.Click += BtnClearAllGroups_Click;
            // 
            // BtnClearAllShips
            // 
            BtnClearAllShips.Location = new Point(852, 3);
            BtnClearAllShips.Name = "BtnClearAllShips";
            BtnClearAllShips.Size = new Size(63, 22);
            BtnClearAllShips.TabIndex = 12;
            BtnClearAllShips.Text = "Clear All";
            BtnClearAllShips.UseVisualStyleBackColor = true;
            BtnClearAllShips.Click += BtnClearAllShips_Click;
            // 
            // FactionShipsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(920, 571);
            Controls.Add(BtnClearAllShips);
            Controls.Add(BtnClearAllGroups);
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
        private Button BtnClearAllGroups;
        private Button BtnClearAllShips;
    }
}