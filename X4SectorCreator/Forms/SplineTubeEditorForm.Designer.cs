namespace X4SectorCreator.Forms
{
    partial class SplineTubeEditorForm
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
            SplineTubeRenderer = new PictureBox();
            ListBoxSplinePositions = new ListBox();
            BtnRemovePoint = new Button();
            BtnAddPoint = new Button();
            BtnImportSplineTube = new Button();
            BtnExportSplineTube = new Button();
            BtnSelect = new Button();
            ((System.ComponentModel.ISupportInitialize)SplineTubeRenderer).BeginInit();
            SuspendLayout();
            // 
            // SplineTubeRenderer
            // 
            SplineTubeRenderer.Location = new Point(269, 53);
            SplineTubeRenderer.Name = "SplineTubeRenderer";
            SplineTubeRenderer.Size = new Size(400, 400);
            SplineTubeRenderer.TabIndex = 0;
            SplineTubeRenderer.TabStop = false;
            // 
            // ListBoxSplinePositions
            // 
            ListBoxSplinePositions.FormattingEnabled = true;
            ListBoxSplinePositions.HorizontalScrollbar = true;
            ListBoxSplinePositions.Location = new Point(10, 47);
            ListBoxSplinePositions.Name = "ListBoxSplinePositions";
            ListBoxSplinePositions.Size = new Size(253, 409);
            ListBoxSplinePositions.TabIndex = 1;
            // 
            // BtnRemovePoint
            // 
            BtnRemovePoint.Location = new Point(83, 8);
            BtnRemovePoint.Name = "BtnRemovePoint";
            BtnRemovePoint.Size = new Size(64, 33);
            BtnRemovePoint.TabIndex = 2;
            BtnRemovePoint.Text = "Remove";
            BtnRemovePoint.UseVisualStyleBackColor = true;
            BtnRemovePoint.Click += BtnRemovePoint_Click;
            // 
            // BtnAddPoint
            // 
            BtnAddPoint.Location = new Point(10, 9);
            BtnAddPoint.Name = "BtnAddPoint";
            BtnAddPoint.Size = new Size(67, 33);
            BtnAddPoint.TabIndex = 3;
            BtnAddPoint.Text = "Add";
            BtnAddPoint.UseVisualStyleBackColor = true;
            BtnAddPoint.Click += BtnAddPoint_Click;
            // 
            // BtnImportSplineTube
            // 
            BtnImportSplineTube.Location = new Point(153, 8);
            BtnImportSplineTube.Name = "BtnImportSplineTube";
            BtnImportSplineTube.Size = new Size(130, 33);
            BtnImportSplineTube.TabIndex = 4;
            BtnImportSplineTube.Text = "Import SplineTube";
            BtnImportSplineTube.UseVisualStyleBackColor = true;
            BtnImportSplineTube.Click += BtnImportSplineTube_Click;
            // 
            // BtnExportSplineTube
            // 
            BtnExportSplineTube.Location = new Point(289, 8);
            BtnExportSplineTube.Name = "BtnExportSplineTube";
            BtnExportSplineTube.Size = new Size(124, 33);
            BtnExportSplineTube.TabIndex = 5;
            BtnExportSplineTube.Text = "Export SplineTube";
            BtnExportSplineTube.UseVisualStyleBackColor = true;
            BtnExportSplineTube.Click += BtnExportSplineTube_Click;
            // 
            // BtnSelect
            // 
            BtnSelect.Location = new Point(560, 8);
            BtnSelect.Name = "BtnSelect";
            BtnSelect.Size = new Size(109, 33);
            BtnSelect.TabIndex = 6;
            BtnSelect.Text = "Select";
            BtnSelect.UseVisualStyleBackColor = true;
            BtnSelect.Click += BtnSelect_Click;
            // 
            // SplineTubeEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(680, 461);
            Controls.Add(BtnSelect);
            Controls.Add(BtnExportSplineTube);
            Controls.Add(BtnImportSplineTube);
            Controls.Add(BtnAddPoint);
            Controls.Add(BtnRemovePoint);
            Controls.Add(ListBoxSplinePositions);
            Controls.Add(SplineTubeRenderer);
            Name = "SplineTubeEditorForm";
            Text = "X4 Sector Creator";
            ((System.ComponentModel.ISupportInitialize)SplineTubeRenderer).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox SplineTubeRenderer;
        private ListBox ListBoxSplinePositions;
        private Button BtnRemovePoint;
        private Button BtnAddPoint;
        private Button BtnImportSplineTube;
        private Button BtnExportSplineTube;
        private Button BtnSelect;
    }
}