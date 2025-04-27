namespace X4SectorCreator.Forms.General
{
    partial class TemplateGroupsForm
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
            TemplateGroupsListBox = new ListBox();
            label1 = new Label();
            label2 = new Label();
            TemplatesInGroupListBox = new ListBox();
            BtnCreateNewGroup = new Button();
            BtnDeleteGroup = new Button();
            BtnAddTemplate = new Button();
            BtnDeleteTemplate = new Button();
            BtnConfirm = new Button();
            BtnCancel = new Button();
            TxtSearch = new TextBox();
            BtnImportGroup = new Button();
            CmbGroupFilter = new ComboBox();
            SuspendLayout();
            // 
            // TemplateGroupsListBox
            // 
            TemplateGroupsListBox.FormattingEnabled = true;
            TemplateGroupsListBox.HorizontalScrollbar = true;
            TemplateGroupsListBox.Location = new Point(6, 33);
            TemplateGroupsListBox.Name = "TemplateGroupsListBox";
            TemplateGroupsListBox.Size = new Size(236, 454);
            TemplateGroupsListBox.TabIndex = 0;
            TemplateGroupsListBox.SelectedIndexChanged += TemplateGroupsListBox_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(6, 9);
            label1.Name = "label1";
            label1.Size = new Size(130, 21);
            label1.TabIndex = 1;
            label1.Text = "Template Groups:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(248, 9);
            label2.Name = "label2";
            label2.Size = new Size(147, 21);
            label2.TabIndex = 3;
            label2.Text = "Templates In Group:";
            // 
            // TemplatesInGroupListBox
            // 
            TemplatesInGroupListBox.FormattingEnabled = true;
            TemplatesInGroupListBox.HorizontalScrollbar = true;
            TemplatesInGroupListBox.Location = new Point(248, 33);
            TemplatesInGroupListBox.Name = "TemplatesInGroupListBox";
            TemplatesInGroupListBox.Size = new Size(457, 454);
            TemplatesInGroupListBox.TabIndex = 2;
            TemplatesInGroupListBox.DoubleClick += TemplatesInGroupListBox_DoubleClick;
            // 
            // BtnCreateNewGroup
            // 
            BtnCreateNewGroup.Location = new Point(6, 493);
            BtnCreateNewGroup.Name = "BtnCreateNewGroup";
            BtnCreateNewGroup.Size = new Size(127, 36);
            BtnCreateNewGroup.TabIndex = 4;
            BtnCreateNewGroup.Text = "Create New Group";
            BtnCreateNewGroup.UseVisualStyleBackColor = true;
            BtnCreateNewGroup.Click += BtnCreateNewGroup_Click;
            // 
            // BtnDeleteGroup
            // 
            BtnDeleteGroup.Location = new Point(139, 493);
            BtnDeleteGroup.Name = "BtnDeleteGroup";
            BtnDeleteGroup.Size = new Size(103, 36);
            BtnDeleteGroup.TabIndex = 5;
            BtnDeleteGroup.Text = "Delete";
            BtnDeleteGroup.UseVisualStyleBackColor = true;
            BtnDeleteGroup.Click += BtnDeleteGroup_Click;
            // 
            // BtnAddTemplate
            // 
            BtnAddTemplate.Location = new Point(248, 493);
            BtnAddTemplate.Name = "BtnAddTemplate";
            BtnAddTemplate.Size = new Size(325, 36);
            BtnAddTemplate.TabIndex = 6;
            BtnAddTemplate.Text = "Add new template";
            BtnAddTemplate.UseVisualStyleBackColor = true;
            BtnAddTemplate.Click += BtnAddTemplate_Click;
            // 
            // BtnDeleteTemplate
            // 
            BtnDeleteTemplate.Location = new Point(579, 493);
            BtnDeleteTemplate.Name = "BtnDeleteTemplate";
            BtnDeleteTemplate.Size = new Size(126, 36);
            BtnDeleteTemplate.TabIndex = 7;
            BtnDeleteTemplate.Text = "Delete";
            BtnDeleteTemplate.UseVisualStyleBackColor = true;
            BtnDeleteTemplate.Click += BtnDeleteTemplate_Click;
            // 
            // BtnConfirm
            // 
            BtnConfirm.Location = new Point(375, 532);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(330, 36);
            BtnConfirm.TabIndex = 8;
            BtnConfirm.Text = "Confirm Changes";
            BtnConfirm.UseVisualStyleBackColor = true;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(248, 532);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(121, 36);
            BtnCancel.TabIndex = 9;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(395, 7);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.PlaceholderText = "Search..";
            TxtSearch.Size = new Size(310, 23);
            TxtSearch.TabIndex = 10;
            // 
            // BtnImportGroup
            // 
            BtnImportGroup.Location = new Point(6, 532);
            BtnImportGroup.Name = "BtnImportGroup";
            BtnImportGroup.Size = new Size(236, 36);
            BtnImportGroup.TabIndex = 11;
            BtnImportGroup.Text = "Import Group From File";
            BtnImportGroup.UseVisualStyleBackColor = true;
            BtnImportGroup.Click += BtnImportGroup_Click;
            // 
            // CmbGroupFilter
            // 
            CmbGroupFilter.FormattingEnabled = true;
            CmbGroupFilter.Items.AddRange(new object[] { "Show Standard", "Show Custom", "Show All" });
            CmbGroupFilter.Location = new Point(139, 7);
            CmbGroupFilter.Name = "CmbGroupFilter";
            CmbGroupFilter.Size = new Size(103, 23);
            CmbGroupFilter.TabIndex = 12;
            CmbGroupFilter.Text = "Show All";
            CmbGroupFilter.SelectedIndexChanged += CmbGroupFilter_SelectedIndexChanged;
            // 
            // TemplateGroupsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(710, 572);
            Controls.Add(CmbGroupFilter);
            Controls.Add(BtnImportGroup);
            Controls.Add(TxtSearch);
            Controls.Add(BtnCancel);
            Controls.Add(BtnConfirm);
            Controls.Add(BtnDeleteTemplate);
            Controls.Add(BtnAddTemplate);
            Controls.Add(BtnDeleteGroup);
            Controls.Add(BtnCreateNewGroup);
            Controls.Add(label2);
            Controls.Add(TemplatesInGroupListBox);
            Controls.Add(label1);
            Controls.Add(TemplateGroupsListBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TemplateGroupsForm";
            Text = "Template Groups Editor";
            Load += TemplateGroupsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox TemplateGroupsListBox;
        private Label label1;
        private Label label2;
        private ListBox TemplatesInGroupListBox;
        private Button BtnCreateNewGroup;
        private Button BtnDeleteGroup;
        private Button BtnAddTemplate;
        private Button BtnDeleteTemplate;
        private Button BtnConfirm;
        private Button BtnCancel;
        private TextBox TxtSearch;
        private Button BtnImportGroup;
        private ComboBox CmbGroupFilter;
    }
}