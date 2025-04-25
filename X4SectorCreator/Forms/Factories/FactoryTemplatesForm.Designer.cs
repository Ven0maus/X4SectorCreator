namespace X4SectorCreator.Forms
{
    partial class FactoryTemplatesForm
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
            ListTemplateFactories = new ListBox();
            label1 = new Label();
            TxtExampleFactory = new TextBox();
            label2 = new Label();
            BtnSelectExampleFactory = new Button();
            BtnCancel = new Button();
            TxtSearch = new TextBox();
            label3 = new Label();
            CmbTemplatesGroup = new ComboBox();
            BtnViewTemplateGroups = new Button();
            BtnCopyXml = new Button();
            SuspendLayout();
            // 
            // ListTemplateFactories
            // 
            ListTemplateFactories.FormattingEnabled = true;
            ListTemplateFactories.HorizontalScrollbar = true;
            ListTemplateFactories.Location = new Point(12, 66);
            ListTemplateFactories.Name = "ListTemplateFactories";
            ListTemplateFactories.Size = new Size(389, 469);
            ListTemplateFactories.TabIndex = 0;
            ListTemplateFactories.SelectedIndexChanged += ListTemplateJobs_SelectedIndexChanged;
            ListTemplateFactories.DoubleClick += ListTemplateJobs_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 14);
            label1.Name = "label1";
            label1.Size = new Size(79, 21);
            label1.TabIndex = 1;
            label1.Text = "Templates";
            // 
            // TxtExampleFactory
            // 
            TxtExampleFactory.Font = new Font("Segoe UI", 10F);
            TxtExampleFactory.Location = new Point(407, 36);
            TxtExampleFactory.Multiline = true;
            TxtExampleFactory.Name = "TxtExampleFactory";
            TxtExampleFactory.ReadOnly = true;
            TxtExampleFactory.ScrollBars = ScrollBars.Vertical;
            TxtExampleFactory.Size = new Size(654, 500);
            TxtExampleFactory.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(407, 12);
            label2.Name = "label2";
            label2.Size = new Size(95, 21);
            label2.TabIndex = 3;
            label2.Text = "Factory XML";
            // 
            // BtnSelectExampleFactory
            // 
            BtnSelectExampleFactory.Location = new Point(574, 542);
            BtnSelectExampleFactory.Name = "BtnSelectExampleFactory";
            BtnSelectExampleFactory.Size = new Size(487, 38);
            BtnSelectExampleFactory.TabIndex = 4;
            BtnSelectExampleFactory.Text = "Select";
            BtnSelectExampleFactory.UseVisualStyleBackColor = true;
            BtnSelectExampleFactory.Click += BtnSelectExampleFactory_Click;
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
            // TxtSearch
            // 
            TxtSearch.Location = new Point(97, 12);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.PlaceholderText = "Search..";
            TxtSearch.Size = new Size(304, 23);
            TxtSearch.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(12, 39);
            label3.Name = "label3";
            label3.Size = new Size(130, 21);
            label3.TabIndex = 11;
            label3.Text = "Templates Group:";
            // 
            // CmbTemplatesGroup
            // 
            CmbTemplatesGroup.FormattingEnabled = true;
            CmbTemplatesGroup.Location = new Point(148, 39);
            CmbTemplatesGroup.Name = "CmbTemplatesGroup";
            CmbTemplatesGroup.Size = new Size(253, 23);
            CmbTemplatesGroup.TabIndex = 10;
            CmbTemplatesGroup.SelectedIndexChanged += CmbTemplatesGroup_SelectedIndexChanged;
            // 
            // BtnViewTemplateGroups
            // 
            BtnViewTemplateGroups.Location = new Point(12, 541);
            BtnViewTemplateGroups.Name = "BtnViewTemplateGroups";
            BtnViewTemplateGroups.Size = new Size(389, 38);
            BtnViewTemplateGroups.TabIndex = 12;
            BtnViewTemplateGroups.Text = "View Template Groups";
            BtnViewTemplateGroups.UseVisualStyleBackColor = true;
            BtnViewTemplateGroups.Click += BtnViewTemplateGroups_Click;
            // 
            // BtnCopyXml
            // 
            BtnCopyXml.Location = new Point(911, 4);
            BtnCopyXml.Name = "BtnCopyXml";
            BtnCopyXml.Size = new Size(150, 30);
            BtnCopyXml.TabIndex = 13;
            BtnCopyXml.Text = "Copy XML To Clipboard";
            BtnCopyXml.UseVisualStyleBackColor = true;
            BtnCopyXml.Click += BtnCopyXml_Click;
            // 
            // FactoryTemplatesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1066, 585);
            Controls.Add(BtnCopyXml);
            Controls.Add(BtnViewTemplateGroups);
            Controls.Add(label3);
            Controls.Add(CmbTemplatesGroup);
            Controls.Add(TxtSearch);
            Controls.Add(BtnCancel);
            Controls.Add(BtnSelectExampleFactory);
            Controls.Add(label2);
            Controls.Add(TxtExampleFactory);
            Controls.Add(label1);
            Controls.Add(ListTemplateFactories);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactoryTemplatesForm";
            Text = "Factory Template Selector";
            Load += FactoryTemplatesForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ListTemplateFactories;
        private Label label1;
        private TextBox TxtExampleFactory;
        private Label label2;
        private Button BtnSelectExampleFactory;
        private Button BtnCancel;
        private TextBox TxtSearch;
        private Label label3;
        private ComboBox CmbTemplatesGroup;
        private Button BtnViewTemplateGroups;
        private Button BtnCopyXml;
    }
}