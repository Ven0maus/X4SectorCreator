namespace X4SectorCreator.Forms
{
    partial class JobTemplatesForm
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
            ListTemplateJobs = new ListBox();
            label1 = new Label();
            TxtExampleJob = new TextBox();
            label2 = new Label();
            BtnSelectExampleJob = new Button();
            BtnCancel = new Button();
            TxtSearch = new TextBox();
            CmbTemplatesGroup = new ComboBox();
            label3 = new Label();
            BtnViewTemplateGroups = new Button();
            BtnCopyXml = new Button();
            SuspendLayout();
            // 
            // ListTemplateJobs
            // 
            ListTemplateJobs.FormattingEnabled = true;
            ListTemplateJobs.HorizontalScrollbar = true;
            ListTemplateJobs.Location = new Point(9, 67);
            ListTemplateJobs.Name = "ListTemplateJobs";
            ListTemplateJobs.Size = new Size(389, 469);
            ListTemplateJobs.TabIndex = 0;
            ListTemplateJobs.SelectedIndexChanged += ListTemplateJobs_SelectedIndexChanged;
            ListTemplateJobs.DoubleClick += ListTemplateJobs_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(9, 16);
            label1.Name = "label1";
            label1.Size = new Size(79, 21);
            label1.TabIndex = 1;
            label1.Text = "Templates";
            // 
            // TxtExampleJob
            // 
            TxtExampleJob.Font = new Font("Segoe UI", 10F);
            TxtExampleJob.Location = new Point(404, 36);
            TxtExampleJob.Multiline = true;
            TxtExampleJob.Name = "TxtExampleJob";
            TxtExampleJob.ReadOnly = true;
            TxtExampleJob.ScrollBars = ScrollBars.Vertical;
            TxtExampleJob.Size = new Size(654, 500);
            TxtExampleJob.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(404, 12);
            label2.Name = "label2";
            label2.Size = new Size(69, 21);
            label2.TabIndex = 3;
            label2.Text = "Job XML";
            // 
            // BtnSelectExampleJob
            // 
            BtnSelectExampleJob.Location = new Point(572, 539);
            BtnSelectExampleJob.Name = "BtnSelectExampleJob";
            BtnSelectExampleJob.Size = new Size(487, 38);
            BtnSelectExampleJob.TabIndex = 4;
            BtnSelectExampleJob.Text = "Select";
            BtnSelectExampleJob.UseVisualStyleBackColor = true;
            BtnSelectExampleJob.Click += BtnSelectExampleJob_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(404, 539);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(161, 38);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(94, 14);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.PlaceholderText = "Search..";
            TxtSearch.Size = new Size(304, 23);
            TxtSearch.TabIndex = 7;
            // 
            // CmbTemplatesGroup
            // 
            CmbTemplatesGroup.FormattingEnabled = true;
            CmbTemplatesGroup.Location = new Point(145, 41);
            CmbTemplatesGroup.Name = "CmbTemplatesGroup";
            CmbTemplatesGroup.Size = new Size(253, 23);
            CmbTemplatesGroup.TabIndex = 8;
            CmbTemplatesGroup.SelectedIndexChanged += CmbTemplatesGroup_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(9, 41);
            label3.Name = "label3";
            label3.Size = new Size(130, 21);
            label3.TabIndex = 9;
            label3.Text = "Templates Group:";
            // 
            // BtnViewTemplateGroups
            // 
            BtnViewTemplateGroups.Location = new Point(9, 539);
            BtnViewTemplateGroups.Name = "BtnViewTemplateGroups";
            BtnViewTemplateGroups.Size = new Size(389, 38);
            BtnViewTemplateGroups.TabIndex = 10;
            BtnViewTemplateGroups.Text = "View Template Groups";
            BtnViewTemplateGroups.UseVisualStyleBackColor = true;
            BtnViewTemplateGroups.Click += BtnViewTemplateGroups_Click;
            // 
            // BtnCopyXml
            // 
            BtnCopyXml.Location = new Point(908, 5);
            BtnCopyXml.Name = "BtnCopyXml";
            BtnCopyXml.Size = new Size(150, 30);
            BtnCopyXml.TabIndex = 11;
            BtnCopyXml.Text = "Copy XML To Clipboard";
            BtnCopyXml.UseVisualStyleBackColor = true;
            BtnCopyXml.Click += BtnCopyXml_Click;
            // 
            // JobTemplatesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1066, 581);
            Controls.Add(BtnCopyXml);
            Controls.Add(BtnViewTemplateGroups);
            Controls.Add(label3);
            Controls.Add(CmbTemplatesGroup);
            Controls.Add(TxtSearch);
            Controls.Add(BtnCancel);
            Controls.Add(BtnSelectExampleJob);
            Controls.Add(label2);
            Controls.Add(TxtExampleJob);
            Controls.Add(label1);
            Controls.Add(ListTemplateJobs);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "JobTemplatesForm";
            Text = "Job Template Selector";
            Load += JobTemplatesForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ListTemplateJobs;
        private Label label1;
        private TextBox TxtExampleJob;
        private Label label2;
        private Button BtnSelectExampleJob;
        private Button BtnCancel;
        private TextBox TxtSearch;
        private ComboBox CmbTemplatesGroup;
        private Label label3;
        private Button BtnViewTemplateGroups;
        private Button BtnCopyXml;
    }
}