namespace X4SectorCreator.Forms
{
    partial class ShipMacrosForm
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
            ShipMacroListBox = new ListBox();
            label1 = new Label();
            CmbFaction = new ComboBox();
            label2 = new Label();
            TxtSearch = new TextBox();
            label4 = new Label();
            BtnAdd = new Button();
            BtnCancel = new Button();
            SelectedShipMacroListBox = new ListBox();
            label5 = new Label();
            BtnRemove = new Button();
            BtnConfirm = new Button();
            SuspendLayout();
            // 
            // ShipMacroListBox
            // 
            ShipMacroListBox.FormattingEnabled = true;
            ShipMacroListBox.HorizontalScrollbar = true;
            ShipMacroListBox.Location = new Point(12, 60);
            ShipMacroListBox.Name = "ShipMacroListBox";
            ShipMacroListBox.Size = new Size(331, 364);
            ShipMacroListBox.TabIndex = 0;
            ShipMacroListBox.DoubleClick += ShipMacroListBox_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 36);
            label1.Name = "label1";
            label1.Size = new Size(161, 21);
            label1.TabIndex = 1;
            label1.Text = "Available ship macros";
            // 
            // CmbFaction
            // 
            CmbFaction.FormattingEnabled = true;
            CmbFaction.Location = new Point(74, 9);
            CmbFaction.Name = "CmbFaction";
            CmbFaction.Size = new Size(128, 23);
            CmbFaction.TabIndex = 2;
            CmbFaction.SelectedIndexChanged += CmbFaction_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(62, 21);
            label2.TabIndex = 3;
            label2.Text = "Faction:";
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(299, 8);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.PlaceholderText = "Search..";
            TxtSearch.Size = new Size(454, 23);
            TxtSearch.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(207, 8);
            label4.Name = "label4";
            label4.Size = new Size(90, 21);
            label4.TabIndex = 6;
            label4.Text = "Text Search:";
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(349, 196);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(69, 36);
            BtnAdd.TabIndex = 7;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 430);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(202, 36);
            BtnCancel.TabIndex = 8;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // SelectedShipMacroListBox
            // 
            SelectedShipMacroListBox.FormattingEnabled = true;
            SelectedShipMacroListBox.HorizontalScrollbar = true;
            SelectedShipMacroListBox.Location = new Point(422, 60);
            SelectedShipMacroListBox.Name = "SelectedShipMacroListBox";
            SelectedShipMacroListBox.Size = new Size(331, 364);
            SelectedShipMacroListBox.TabIndex = 9;
            SelectedShipMacroListBox.DoubleClick += SelectedShipMacroListBox_DoubleClick;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(422, 36);
            label5.Name = "label5";
            label5.Size = new Size(156, 21);
            label5.TabIndex = 10;
            label5.Text = "Selected ship macros";
            // 
            // BtnRemove
            // 
            BtnRemove.Location = new Point(349, 238);
            BtnRemove.Name = "BtnRemove";
            BtnRemove.Size = new Size(69, 36);
            BtnRemove.TabIndex = 11;
            BtnRemove.Text = "Remove";
            BtnRemove.UseVisualStyleBackColor = true;
            BtnRemove.Click += BtnRemove_Click;
            // 
            // BtnConfirm
            // 
            BtnConfirm.Location = new Point(220, 430);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(533, 36);
            BtnConfirm.TabIndex = 12;
            BtnConfirm.Text = "Confirm Selection";
            BtnConfirm.UseVisualStyleBackColor = true;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // ShipMacrosForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(758, 469);
            Controls.Add(BtnConfirm);
            Controls.Add(BtnRemove);
            Controls.Add(label5);
            Controls.Add(SelectedShipMacroListBox);
            Controls.Add(BtnCancel);
            Controls.Add(BtnAdd);
            Controls.Add(label4);
            Controls.Add(TxtSearch);
            Controls.Add(label2);
            Controls.Add(CmbFaction);
            Controls.Add(label1);
            Controls.Add(ShipMacroListBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ShipMacrosForm";
            Text = "Ship Macro Selector";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ShipMacroListBox;
        private Label label1;
        private ComboBox CmbFaction;
        private Label label2;
        private TextBox TxtSearch;
        private Label label4;
        private Button BtnAdd;
        private Button BtnCancel;
        private ListBox SelectedShipMacroListBox;
        private Label label5;
        private Button BtnRemove;
        private Button BtnConfirm;
    }
}