namespace X4SectorCreator.Forms.Stations
{
    partial class ConstructionPlanViewForm
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
            ConstructionPlansListBox = new ListBox();
            label1 = new Label();
            BtnImport = new Button();
            BtnDelete = new Button();
            BtnUpdate = new Button();
            label2 = new Label();
            TxtName = new TextBox();
            ModulesListBox = new ListBox();
            label3 = new Label();
            TxtId = new TextBox();
            label4 = new Label();
            BtnExit = new Button();
            BtnReImport = new Button();
            label5 = new Label();
            SuspendLayout();
            // 
            // ConstructionPlansListBox
            // 
            ConstructionPlansListBox.FormattingEnabled = true;
            ConstructionPlansListBox.HorizontalScrollbar = true;
            ConstructionPlansListBox.Location = new Point(6, 32);
            ConstructionPlansListBox.Name = "ConstructionPlansListBox";
            ConstructionPlansListBox.Size = new Size(229, 244);
            ConstructionPlansListBox.TabIndex = 0;
            ConstructionPlansListBox.SelectedIndexChanged += ConstructionPlansListBox_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(6, 8);
            label1.Name = "label1";
            label1.Size = new Size(143, 21);
            label1.TabIndex = 1;
            label1.Text = "Construction Plans:";
            // 
            // BtnImport
            // 
            BtnImport.Location = new Point(100, 282);
            BtnImport.Name = "BtnImport";
            BtnImport.Size = new Size(135, 37);
            BtnImport.TabIndex = 2;
            BtnImport.Text = "Import Plan";
            BtnImport.UseVisualStyleBackColor = true;
            BtnImport.Click += BtnImport_Click;
            // 
            // BtnDelete
            // 
            BtnDelete.Location = new Point(6, 282);
            BtnDelete.Name = "BtnDelete";
            BtnDelete.Size = new Size(88, 37);
            BtnDelete.TabIndex = 3;
            BtnDelete.Text = "Delete";
            BtnDelete.UseVisualStyleBackColor = true;
            BtnDelete.Click += BtnDelete_Click;
            // 
            // BtnUpdate
            // 
            BtnUpdate.Location = new Point(241, 109);
            BtnUpdate.Name = "BtnUpdate";
            BtnUpdate.Size = new Size(190, 37);
            BtnUpdate.TabIndex = 4;
            BtnUpdate.Text = "Update Fields";
            BtnUpdate.UseVisualStyleBackColor = true;
            BtnUpdate.Click += BtnUpdate_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(241, 56);
            label2.Name = "label2";
            label2.Size = new Size(124, 21);
            label2.TabIndex = 5;
            label2.Text = "Name of station:";
            // 
            // TxtName
            // 
            TxtName.Location = new Point(241, 80);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(190, 23);
            TxtName.TabIndex = 6;
            // 
            // ModulesListBox
            // 
            ModulesListBox.FormattingEnabled = true;
            ModulesListBox.HorizontalScrollbar = true;
            ModulesListBox.Location = new Point(241, 216);
            ModulesListBox.Name = "ModulesListBox";
            ModulesListBox.Size = new Size(190, 139);
            ModulesListBox.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(241, 192);
            label3.Name = "label3";
            label3.Size = new Size(146, 21);
            label3.TabIndex = 8;
            label3.Text = "Modules on station:";
            // 
            // TxtId
            // 
            TxtId.Location = new Point(241, 33);
            TxtId.Name = "TxtId";
            TxtId.Size = new Size(190, 23);
            TxtId.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(241, 9);
            label4.Name = "label4";
            label4.Size = new Size(151, 21);
            label4.TabIndex = 9;
            label4.Text = "Constructionplan ID:";
            // 
            // BtnExit
            // 
            BtnExit.Location = new Point(6, 325);
            BtnExit.Name = "BtnExit";
            BtnExit.Size = new Size(229, 34);
            BtnExit.TabIndex = 11;
            BtnExit.Text = "Exit";
            BtnExit.UseVisualStyleBackColor = true;
            BtnExit.Click += BtnExit_Click;
            // 
            // BtnReImport
            // 
            BtnReImport.Location = new Point(241, 152);
            BtnReImport.Name = "BtnReImport";
            BtnReImport.Size = new Size(190, 34);
            BtnReImport.TabIndex = 12;
            BtnReImport.Text = "Re-Import Plan";
            BtnReImport.UseVisualStyleBackColor = true;
            BtnReImport.Click += BtnReImport_Click;
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(6, 362);
            label5.Name = "label5";
            label5.Size = new Size(425, 48);
            label5.TabIndex = 13;
            label5.Text = "(You can use the in-game station creator to build your own station and export the construction plan to import it here.)";
            label5.TextAlign = ContentAlignment.TopCenter;
            // 
            // ConstructionPlanViewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 410);
            Controls.Add(label5);
            Controls.Add(BtnReImport);
            Controls.Add(BtnExit);
            Controls.Add(TxtId);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(ModulesListBox);
            Controls.Add(TxtName);
            Controls.Add(label2);
            Controls.Add(BtnUpdate);
            Controls.Add(BtnDelete);
            Controls.Add(BtnImport);
            Controls.Add(label1);
            Controls.Add(ConstructionPlansListBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConstructionPlanViewForm";
            Text = "Construction Plan Editor";
            Load += ConstructionPlanViewForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ConstructionPlansListBox;
        private Label label1;
        private Button BtnImport;
        private Button BtnDelete;
        private Button BtnUpdate;
        private Label label2;
        private TextBox TxtName;
        private ListBox ModulesListBox;
        private Label label3;
        private TextBox TxtId;
        private Label label4;
        private Button BtnExit;
        private Button BtnReImport;
        private Label label5;
    }
}