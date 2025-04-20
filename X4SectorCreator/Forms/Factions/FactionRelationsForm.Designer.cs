namespace X4SectorCreator.Forms
{
    partial class FactionRelationsForm
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
            BtnUpdate = new Button();
            BtnCancel = new Button();
            FactionRelationsDataGrid = new DataGridView();
            FactionName = new DataGridViewTextBoxColumn();
            RelationValue = new DataGridViewTextBoxColumn();
            label1 = new Label();
            ChkLockRelations = new CheckBox();
            BtnRelationValueHelper = new Button();
            ((System.ComponentModel.ISupportInitialize)FactionRelationsDataGrid).BeginInit();
            SuspendLayout();
            // 
            // BtnUpdate
            // 
            BtnUpdate.Location = new Point(184, 368);
            BtnUpdate.Name = "BtnUpdate";
            BtnUpdate.Size = new Size(274, 32);
            BtnUpdate.TabIndex = 0;
            BtnUpdate.Text = "Update";
            BtnUpdate.UseVisualStyleBackColor = true;
            BtnUpdate.Click += BtnUpdate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 368);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(166, 32);
            BtnCancel.TabIndex = 1;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // FactionRelationsDataGrid
            // 
            FactionRelationsDataGrid.AllowUserToAddRows = false;
            FactionRelationsDataGrid.AllowUserToDeleteRows = false;
            FactionRelationsDataGrid.AllowUserToResizeColumns = false;
            FactionRelationsDataGrid.AllowUserToResizeRows = false;
            FactionRelationsDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            FactionRelationsDataGrid.Columns.AddRange(new DataGridViewColumn[] { FactionName, RelationValue });
            FactionRelationsDataGrid.Location = new Point(12, 33);
            FactionRelationsDataGrid.Name = "FactionRelationsDataGrid";
            FactionRelationsDataGrid.ScrollBars = ScrollBars.Vertical;
            FactionRelationsDataGrid.Size = new Size(446, 329);
            FactionRelationsDataGrid.TabIndex = 2;
            // 
            // FactionName
            // 
            FactionName.FillWeight = 250F;
            FactionName.HeaderText = "Faction";
            FactionName.Name = "FactionName";
            FactionName.ReadOnly = true;
            FactionName.Resizable = DataGridViewTriState.False;
            FactionName.Width = 250;
            // 
            // RelationValue
            // 
            RelationValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            RelationValue.HeaderText = "Value";
            RelationValue.Name = "RelationValue";
            RelationValue.Resizable = DataGridViewTriState.False;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(127, 21);
            label1.TabIndex = 3;
            label1.Text = "Faction Relations";
            // 
            // ChkLockRelations
            // 
            ChkLockRelations.AutoSize = true;
            ChkLockRelations.Location = new Point(356, 8);
            ChkLockRelations.Name = "ChkLockRelations";
            ChkLockRelations.Size = new Size(102, 19);
            ChkLockRelations.TabIndex = 4;
            ChkLockRelations.Text = "Lock Relations";
            ChkLockRelations.UseVisualStyleBackColor = true;
            // 
            // BtnRelationValueHelper
            // 
            BtnRelationValueHelper.Location = new Point(166, 4);
            BtnRelationValueHelper.Name = "BtnRelationValueHelper";
            BtnRelationValueHelper.Size = new Size(144, 23);
            BtnRelationValueHelper.TabIndex = 5;
            BtnRelationValueHelper.Text = "Relation Value Helper";
            BtnRelationValueHelper.UseVisualStyleBackColor = true;
            BtnRelationValueHelper.Click += BtnRelationValueHelper_Click;
            // 
            // FactionRelationsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(465, 405);
            Controls.Add(BtnRelationValueHelper);
            Controls.Add(ChkLockRelations);
            Controls.Add(label1);
            Controls.Add(FactionRelationsDataGrid);
            Controls.Add(BtnCancel);
            Controls.Add(BtnUpdate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactionRelationsForm";
            Text = "Faction Relations Editor";
            ((System.ComponentModel.ISupportInitialize)FactionRelationsDataGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnUpdate;
        private Button BtnCancel;
        private DataGridView FactionRelationsDataGrid;
        private Label label1;
        private DataGridViewTextBoxColumn FactionName;
        private DataGridViewTextBoxColumn RelationValue;
        private CheckBox ChkLockRelations;
        private Button BtnRelationValueHelper;
    }
}