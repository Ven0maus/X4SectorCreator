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
            SuspendLayout();
            // 
            // AvailableStationTypesListBox
            // 
            AvailableStationTypesListBox.FormattingEnabled = true;
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
            BtnConfirm.Location = new Point(186, 238);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(320, 34);
            BtnConfirm.TabIndex = 6;
            BtnConfirm.Text = "Confirm";
            BtnConfirm.UseVisualStyleBackColor = true;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 238);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(168, 34);
            BtnCancel.TabIndex = 7;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // FactionStationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(512, 277);
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
    }
}