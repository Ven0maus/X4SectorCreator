namespace X4SectorCreator.Forms
{
    partial class QuickQuotaEditorForm
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
            QuotaView = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Galaxy = new DataGridViewTextBoxColumn();
            Cluster = new DataGridViewTextBoxColumn();
            Sector = new DataGridViewTextBoxColumn();
            BtnSave = new Button();
            BtnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)QuotaView).BeginInit();
            SuspendLayout();
            // 
            // QuotaView
            // 
            QuotaView.AllowUserToAddRows = false;
            QuotaView.AllowUserToDeleteRows = false;
            QuotaView.AllowUserToOrderColumns = true;
            QuotaView.AllowUserToResizeColumns = false;
            QuotaView.AllowUserToResizeRows = false;
            QuotaView.BorderStyle = BorderStyle.Fixed3D;
            QuotaView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            QuotaView.Columns.AddRange(new DataGridViewColumn[] { Column1, Galaxy, Cluster, Sector });
            QuotaView.Location = new Point(12, 12);
            QuotaView.Name = "QuotaView";
            QuotaView.ScrollBars = ScrollBars.Vertical;
            QuotaView.Size = new Size(601, 413);
            QuotaView.TabIndex = 0;
            // 
            // Column1
            // 
            Column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column1.HeaderText = "Name";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            // 
            // Galaxy
            // 
            Galaxy.HeaderText = "Galaxy";
            Galaxy.Name = "Galaxy";
            // 
            // Cluster
            // 
            Cluster.HeaderText = "Cluster";
            Cluster.Name = "Cluster";
            // 
            // Sector
            // 
            Sector.HeaderText = "Sector";
            Sector.Name = "Sector";
            // 
            // BtnSave
            // 
            BtnSave.Location = new Point(203, 431);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new Size(410, 30);
            BtnSave.TabIndex = 1;
            BtnSave.Text = "Save";
            BtnSave.UseVisualStyleBackColor = true;
            BtnSave.Click += BtnSave_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 431);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(185, 30);
            BtnCancel.TabIndex = 2;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // QuickQuotaEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(625, 466);
            Controls.Add(BtnCancel);
            Controls.Add(BtnSave);
            Controls.Add(QuotaView);
            Name = "QuickQuotaEditorForm";
            Text = "Quick Quota Editor";
            ((System.ComponentModel.ISupportInitialize)QuotaView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView QuotaView;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Galaxy;
        private DataGridViewTextBoxColumn Cluster;
        private DataGridViewTextBoxColumn Sector;
        private Button BtnSave;
        private Button BtnCancel;
    }
}