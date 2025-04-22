using X4SectorCreator.CustomComponents;

namespace X4SectorCreator.Forms.Factions
{
    partial class FactionStationForm
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
            AvailableStationTypesListBox = new ListBox();
            label1 = new Label();
            label2 = new Label();
            SelectedStationTypesListBox = new ListBox();
            BtnAdd = new Button();
            BtnRemove = new Button();
            BtnConfirm = new Button();
            BtnCancel = new Button();
            CmbHQTypes = new MultiSelectCombo.NoDropDownComboBox();
            label3 = new Label();
            label4 = new Label();
            TxtDesiredWharfs = new TextBox();
            TxtDesiredEquipmentDocks = new TextBox();
            label5 = new Label();
            TxtDesiredTradeStations = new TextBox();
            label6 = new Label();
            TxtDesiredShipyards = new TextBox();
            label7 = new Label();
            SuspendLayout();
            // 
            // AvailableStationTypesListBox
            // 
            AvailableStationTypesListBox.FormattingEnabled = true;
            AvailableStationTypesListBox.HorizontalScrollbar = true;
            AvailableStationTypesListBox.Items.AddRange(new object[] { "shipyard", "wharf", "equipmentdock", "tradestation", "defence", "piratedock", "piratebase", "freeport" });
            AvailableStationTypesListBox.Location = new Point(12, 33);
            AvailableStationTypesListBox.Name = "AvailableStationTypesListBox";
            AvailableStationTypesListBox.Size = new Size(201, 199);
            AvailableStationTypesListBox.TabIndex = 0;
            AvailableStationTypesListBox.DoubleClick += AvailableStationTypesListBox_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(168, 21);
            label1.TabIndex = 1;
            label1.Text = "Available Station Types";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(305, 9);
            label2.Name = "label2";
            label2.Size = new Size(163, 21);
            label2.TabIndex = 3;
            label2.Text = "Selected Station Types";
            // 
            // SelectedStationTypesListBox
            // 
            SelectedStationTypesListBox.FormattingEnabled = true;
            SelectedStationTypesListBox.HorizontalScrollbar = true;
            SelectedStationTypesListBox.Location = new Point(305, 33);
            SelectedStationTypesListBox.Name = "SelectedStationTypesListBox";
            SelectedStationTypesListBox.Size = new Size(201, 199);
            SelectedStationTypesListBox.TabIndex = 2;
            SelectedStationTypesListBox.DoubleClick += SelectedStationTypesListBox_DoubleClick;
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(219, 91);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(80, 34);
            BtnAdd.TabIndex = 4;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnRemove
            // 
            BtnRemove.Location = new Point(219, 131);
            BtnRemove.Name = "BtnRemove";
            BtnRemove.Size = new Size(80, 34);
            BtnRemove.TabIndex = 5;
            BtnRemove.Text = "Remove";
            BtnRemove.UseVisualStyleBackColor = true;
            BtnRemove.Click += BtnRemove_Click;
            // 
            // BtnConfirm
            // 
            BtnConfirm.Location = new Point(180, 374);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(320, 34);
            BtnConfirm.TabIndex = 6;
            BtnConfirm.Text = "Confirm";
            BtnConfirm.UseVisualStyleBackColor = true;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(6, 374);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(168, 34);
            BtnCancel.TabIndex = 7;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // CmbHQTypes
            // 
            CmbHQTypes.FormattingEnabled = true;
            CmbHQTypes.Location = new Point(252, 239);
            CmbHQTypes.Name = "CmbHQTypes";
            CmbHQTypes.Size = new Size(201, 23);
            CmbHQTypes.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(41, 239);
            label3.Name = "label3";
            label3.Size = new Size(205, 21);
            label3.TabIndex = 9;
            label3.Text = "Possible faction rep stations:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(41, 271);
            label4.Name = "label4";
            label4.Size = new Size(120, 21);
            label4.TabIndex = 10;
            label4.Text = "Desired Wharfs:";
            // 
            // TxtDesiredWharfs
            // 
            TxtDesiredWharfs.Location = new Point(41, 295);
            TxtDesiredWharfs.Name = "TxtDesiredWharfs";
            TxtDesiredWharfs.Size = new Size(221, 23);
            TxtDesiredWharfs.TabIndex = 11;
            TxtDesiredWharfs.Text = "0";
            // 
            // TxtDesiredEquipmentDocks
            // 
            TxtDesiredEquipmentDocks.Location = new Point(41, 345);
            TxtDesiredEquipmentDocks.Name = "TxtDesiredEquipmentDocks";
            TxtDesiredEquipmentDocks.Size = new Size(221, 23);
            TxtDesiredEquipmentDocks.TabIndex = 13;
            TxtDesiredEquipmentDocks.Text = "0";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(41, 321);
            label5.Name = "label5";
            label5.Size = new Size(187, 21);
            label5.TabIndex = 12;
            label5.Text = "Desired EquipmentDocks:";
            // 
            // TxtDesiredTradeStations
            // 
            TxtDesiredTradeStations.Location = new Point(271, 345);
            TxtDesiredTradeStations.Name = "TxtDesiredTradeStations";
            TxtDesiredTradeStations.Size = new Size(221, 23);
            TxtDesiredTradeStations.TabIndex = 17;
            TxtDesiredTradeStations.Text = "0";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(271, 321);
            label6.Name = "label6";
            label6.Size = new Size(163, 21);
            label6.TabIndex = 16;
            label6.Text = "Desired TradeStations:";
            // 
            // TxtDesiredShipyards
            // 
            TxtDesiredShipyards.Location = new Point(271, 295);
            TxtDesiredShipyards.Name = "TxtDesiredShipyards";
            TxtDesiredShipyards.Size = new Size(221, 23);
            TxtDesiredShipyards.TabIndex = 15;
            TxtDesiredShipyards.Text = "0";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(271, 271);
            label7.Name = "label7";
            label7.Size = new Size(139, 21);
            label7.TabIndex = 14;
            label7.Text = "Desired Shipyards:";
            // 
            // FactionStationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(512, 414);
            Controls.Add(TxtDesiredTradeStations);
            Controls.Add(label6);
            Controls.Add(TxtDesiredShipyards);
            Controls.Add(label7);
            Controls.Add(TxtDesiredEquipmentDocks);
            Controls.Add(label5);
            Controls.Add(TxtDesiredWharfs);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(CmbHQTypes);
            Controls.Add(BtnCancel);
            Controls.Add(BtnConfirm);
            Controls.Add(BtnRemove);
            Controls.Add(BtnAdd);
            Controls.Add(label2);
            Controls.Add(SelectedStationTypesListBox);
            Controls.Add(label1);
            Controls.Add(AvailableStationTypesListBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactionStationForm";
            Text = "Faction Stations Editor";
            Load += FactionStationForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox AvailableStationTypesListBox;
        private Label label1;
        private Label label2;
        private ListBox SelectedStationTypesListBox;
        private Button BtnAdd;
        private Button BtnRemove;
        private Button BtnConfirm;
        private Button BtnCancel;
        private MultiSelectCombo.NoDropDownComboBox CmbHQTypes;
        private Label label3;
        private Label label4;
        private TextBox TxtDesiredWharfs;
        private TextBox TxtDesiredEquipmentDocks;
        private Label label5;
        private TextBox TxtDesiredTradeStations;
        private Label label6;
        private TextBox TxtDesiredShipyards;
        private Label label7;
    }
}