﻿namespace X4SectorCreator.Forms
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
            CmbFilterOption = new ComboBox();
            SuspendLayout();
            // 
            // ListTemplateJobs
            // 
            ListTemplateJobs.FormattingEnabled = true;
            ListTemplateJobs.HorizontalScrollbar = true;
            ListTemplateJobs.Location = new Point(12, 36);
            ListTemplateJobs.Name = "ListTemplateJobs";
            ListTemplateJobs.Size = new Size(389, 544);
            ListTemplateJobs.TabIndex = 0;
            ListTemplateJobs.SelectedIndexChanged += ListTemplateJobs_SelectedIndexChanged;
            ListTemplateJobs.DoubleClick += ListTemplateJobs_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(79, 21);
            label1.TabIndex = 1;
            label1.Text = "Templates";
            // 
            // TxtExampleJob
            // 
            TxtExampleJob.Font = new Font("Segoe UI", 10F);
            TxtExampleJob.Location = new Point(407, 36);
            TxtExampleJob.Multiline = true;
            TxtExampleJob.Name = "TxtExampleJob";
            TxtExampleJob.ReadOnly = true;
            TxtExampleJob.Size = new Size(654, 500);
            TxtExampleJob.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(407, 12);
            label2.Name = "label2";
            label2.Size = new Size(69, 21);
            label2.TabIndex = 3;
            label2.Text = "Job XML";
            // 
            // BtnSelectExampleJob
            // 
            BtnSelectExampleJob.Location = new Point(574, 542);
            BtnSelectExampleJob.Name = "BtnSelectExampleJob";
            BtnSelectExampleJob.Size = new Size(487, 38);
            BtnSelectExampleJob.TabIndex = 4;
            BtnSelectExampleJob.Text = "Select";
            BtnSelectExampleJob.UseVisualStyleBackColor = true;
            BtnSelectExampleJob.Click += BtnSelectExampleJob_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(407, 542);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(161, 38);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // CmbFilterOption
            // 
            CmbFilterOption.FormattingEnabled = true;
            CmbFilterOption.Location = new Point(159, 11);
            CmbFilterOption.Name = "CmbFilterOption";
            CmbFilterOption.Size = new Size(242, 23);
            CmbFilterOption.TabIndex = 6;
            CmbFilterOption.SelectedIndexChanged += CmbFilterOption_SelectedIndexChanged;
            // 
            // JobTemplatesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1066, 586);
            Controls.Add(CmbFilterOption);
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
        private ComboBox CmbFilterOption;
    }
}