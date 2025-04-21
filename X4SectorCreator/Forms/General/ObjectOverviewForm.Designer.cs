using X4SectorCreator.CustomComponents;

namespace X4SectorCreator.Forms
{
    partial class ObjectOverviewForm
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
            ObjectView = new DataGridView();
            TypeDataGridColumn = new DataGridViewTextBoxColumn();
            NameDataGridColumn = new DataGridViewTextBoxColumn();
            CodeDataGridColumn = new DataGridViewTextBoxColumn();
            BtnExit = new Button();
            label1 = new Label();
            TxtSearch = new TextBox();
            label2 = new Label();
            CmbFilterType = new MultiSelectCombo.NoDropDownComboBox();
            label3 = new Label();
            ChkExcludeVanillaObjects = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)ObjectView).BeginInit();
            SuspendLayout();
            // 
            // ObjectView
            // 
            ObjectView.AllowUserToAddRows = false;
            ObjectView.AllowUserToDeleteRows = false;
            ObjectView.AllowUserToResizeRows = false;
            ObjectView.BorderStyle = BorderStyle.Fixed3D;
            ObjectView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ObjectView.Columns.AddRange(new DataGridViewColumn[] { TypeDataGridColumn, NameDataGridColumn, CodeDataGridColumn });
            ObjectView.Location = new Point(9, 63);
            ObjectView.Name = "ObjectView";
            ObjectView.Size = new Size(1042, 570);
            ObjectView.TabIndex = 0;
            // 
            // TypeDataGridColumn
            // 
            TypeDataGridColumn.HeaderText = "Type";
            TypeDataGridColumn.Name = "TypeDataGridColumn";
            TypeDataGridColumn.ReadOnly = true;
            TypeDataGridColumn.Width = 150;
            // 
            // NameDataGridColumn
            // 
            NameDataGridColumn.HeaderText = "Name";
            NameDataGridColumn.Name = "NameDataGridColumn";
            NameDataGridColumn.ReadOnly = true;
            NameDataGridColumn.Width = 300;
            // 
            // CodeDataGridColumn
            // 
            CodeDataGridColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            CodeDataGridColumn.HeaderText = "Code";
            CodeDataGridColumn.Name = "CodeDataGridColumn";
            CodeDataGridColumn.ReadOnly = true;
            CodeDataGridColumn.Width = 60;
            // 
            // BtnExit
            // 
            BtnExit.Location = new Point(936, 6);
            BtnExit.Name = "BtnExit";
            BtnExit.Size = new Size(115, 28);
            BtnExit.TabIndex = 1;
            BtnExit.Text = "Exit";
            BtnExit.UseVisualStyleBackColor = true;
            BtnExit.Click += BtnExit_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(9, 9);
            label1.Name = "label1";
            label1.Size = new Size(60, 21);
            label1.TabIndex = 2;
            label1.Text = "Search:";
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(70, 9);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.Size = new Size(608, 23);
            TxtSearch.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(681, 9);
            label2.Name = "label2";
            label2.Size = new Size(84, 21);
            label2.TabIndex = 4;
            label2.Text = "Filter Type:";
            // 
            // CmbFilterType
            // 
            CmbFilterType.FormattingEnabled = true;
            CmbFilterType.Location = new Point(765, 9);
            CmbFilterType.Name = "CmbFilterType";
            CmbFilterType.Size = new Size(165, 23);
            CmbFilterType.TabIndex = 5;
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(317, 35);
            label3.Name = "label3";
            label3.Size = new Size(734, 25);
            label3.TabIndex = 6;
            label3.Text = "This form is mainly intended for modders to be able to quickly lookup xml codes from the available data.";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ChkExcludeVanillaObjects
            // 
            ChkExcludeVanillaObjects.AutoSize = true;
            ChkExcludeVanillaObjects.Checked = true;
            ChkExcludeVanillaObjects.CheckState = CheckState.Checked;
            ChkExcludeVanillaObjects.Font = new Font("Segoe UI", 12F);
            ChkExcludeVanillaObjects.Location = new Point(12, 35);
            ChkExcludeVanillaObjects.Name = "ChkExcludeVanillaObjects";
            ChkExcludeVanillaObjects.Size = new Size(183, 25);
            ChkExcludeVanillaObjects.TabIndex = 7;
            ChkExcludeVanillaObjects.Text = "Exclude vanilla objects";
            ChkExcludeVanillaObjects.UseVisualStyleBackColor = true;
            ChkExcludeVanillaObjects.CheckedChanged += ChkExcludeVanillaObjects_CheckedChanged;
            // 
            // ObjectOverviewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1055, 637);
            Controls.Add(ChkExcludeVanillaObjects);
            Controls.Add(label3);
            Controls.Add(CmbFilterType);
            Controls.Add(label2);
            Controls.Add(TxtSearch);
            Controls.Add(label1);
            Controls.Add(BtnExit);
            Controls.Add(ObjectView);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ObjectOverviewForm";
            Text = "Mod Content Overview";
            Load += ObjectOverviewForm_Load;
            ((System.ComponentModel.ISupportInitialize)ObjectView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView ObjectView;
        private Button BtnExit;
        private Label label1;
        private TextBox TxtSearch;
        private Label label2;
        private MultiSelectCombo.NoDropDownComboBox CmbFilterType;
        private Label label3;
        private DataGridViewTextBoxColumn TypeDataGridColumn;
        private DataGridViewTextBoxColumn NameDataGridColumn;
        private DataGridViewTextBoxColumn CodeDataGridColumn;
        private CheckBox ChkExcludeVanillaObjects;
    }
}