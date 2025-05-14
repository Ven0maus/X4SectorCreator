namespace X4SectorCreator.Forms.General
{
    partial class MultiTemplateSelectorForm
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
            label3 = new Label();
            CmbTemplatesGroup = new ComboBox();
            TxtSearch = new TextBox();
            label2 = new Label();
            ListTemplates = new ListBox();
            SelectedTemplatesListBox = new ListBox();
            label1 = new Label();
            BtnSelect = new Button();
            BtnDeselect = new Button();
            label6 = new Label();
            CmbFactions = new CustomComponents.MultiSelectCombo.NoDropDownComboBox();
            label4 = new Label();
            label5 = new Label();
            CmbOwner = new ComboBox();
            BtnConfirm = new Button();
            BtnCancel = new Button();
            SelectGroup = new Button();
            BtnDeselectGroup = new Button();
            BtnViewTemplateGroups = new Button();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(7, 33);
            label3.Name = "label3";
            label3.Size = new Size(130, 21);
            label3.TabIndex = 14;
            label3.Text = "Templates Group:";
            // 
            // CmbTemplatesGroup
            // 
            CmbTemplatesGroup.FormattingEnabled = true;
            CmbTemplatesGroup.Location = new Point(143, 33);
            CmbTemplatesGroup.Name = "CmbTemplatesGroup";
            CmbTemplatesGroup.Size = new Size(253, 23);
            CmbTemplatesGroup.TabIndex = 13;
            CmbTemplatesGroup.SelectedIndexChanged += CmbTemplatesGroup_SelectedIndexChanged;
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(92, 6);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.PlaceholderText = "Search..";
            TxtSearch.Size = new Size(304, 23);
            TxtSearch.TabIndex = 12;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(7, 8);
            label2.Name = "label2";
            label2.Size = new Size(79, 21);
            label2.TabIndex = 11;
            label2.Text = "Templates";
            // 
            // ListTemplates
            // 
            ListTemplates.FormattingEnabled = true;
            ListTemplates.HorizontalScrollbar = true;
            ListTemplates.Location = new Point(7, 59);
            ListTemplates.Name = "ListTemplates";
            ListTemplates.Size = new Size(389, 214);
            ListTemplates.TabIndex = 10;
            ListTemplates.DoubleClick += ListTemplates_DoubleClick;
            // 
            // SelectedTemplatesListBox
            // 
            SelectedTemplatesListBox.FormattingEnabled = true;
            SelectedTemplatesListBox.HorizontalScrollbar = true;
            SelectedTemplatesListBox.Location = new Point(9, 314);
            SelectedTemplatesListBox.Name = "SelectedTemplatesListBox";
            SelectedTemplatesListBox.Size = new Size(389, 199);
            SelectedTemplatesListBox.TabIndex = 15;
            SelectedTemplatesListBox.DoubleClick += SelectedTemplatesListBox_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(9, 288);
            label1.Name = "label1";
            label1.Size = new Size(144, 21);
            label1.TabIndex = 16;
            label1.Text = "Selected Templates:";
            // 
            // BtnSelect
            // 
            BtnSelect.Location = new Point(248, 276);
            BtnSelect.Name = "BtnSelect";
            BtnSelect.Size = new Size(149, 35);
            BtnSelect.TabIndex = 17;
            BtnSelect.Text = "Select";
            BtnSelect.UseVisualStyleBackColor = true;
            BtnSelect.Click += BtnSelect_Click;
            // 
            // BtnDeselect
            // 
            BtnDeselect.Location = new Point(158, 276);
            BtnDeselect.Name = "BtnDeselect";
            BtnDeselect.Size = new Size(84, 35);
            BtnDeselect.TabIndex = 18;
            BtnDeselect.Text = "Deselect";
            BtnDeselect.UseVisualStyleBackColor = true;
            BtnDeselect.Click += BtnDeselect_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label6.Location = new Point(402, 8);
            label6.Name = "label6";
            label6.Size = new Size(131, 21);
            label6.TabIndex = 27;
            label6.Text = "New Faction Data";
            // 
            // CmbFactions
            // 
            CmbFactions.FormattingEnabled = true;
            CmbFactions.Location = new Point(402, 101);
            CmbFactions.Name = "CmbFactions";
            CmbFactions.Size = new Size(249, 23);
            CmbFactions.TabIndex = 26;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 11F);
            label4.Location = new Point(402, 78);
            label4.Name = "label4";
            label4.Size = new Size(249, 20);
            label4.TabIndex = 25;
            label4.Text = "Spawn in space owned by faction(s):";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11F);
            label5.Location = new Point(402, 29);
            label5.Name = "label5";
            label5.Size = new Size(106, 20);
            label5.TabIndex = 24;
            label5.Text = "Owner Faction:";
            // 
            // CmbOwner
            // 
            CmbOwner.FormattingEnabled = true;
            CmbOwner.Location = new Point(402, 52);
            CmbOwner.Name = "CmbOwner";
            CmbOwner.Size = new Size(249, 23);
            CmbOwner.TabIndex = 23;
            // 
            // BtnConfirm
            // 
            BtnConfirm.Location = new Point(402, 474);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(249, 39);
            BtnConfirm.TabIndex = 28;
            BtnConfirm.Text = "Confirm Selection";
            BtnConfirm.UseVisualStyleBackColor = true;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(402, 429);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(249, 39);
            BtnCancel.TabIndex = 29;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // SelectGroup
            // 
            SelectGroup.Location = new Point(402, 130);
            SelectGroup.Name = "SelectGroup";
            SelectGroup.Size = new Size(249, 35);
            SelectGroup.TabIndex = 30;
            SelectGroup.Text = "Select Entire Group";
            SelectGroup.UseVisualStyleBackColor = true;
            SelectGroup.Click += SelectGroup_Click;
            // 
            // BtnDeselectGroup
            // 
            BtnDeselectGroup.Location = new Point(402, 171);
            BtnDeselectGroup.Name = "BtnDeselectGroup";
            BtnDeselectGroup.Size = new Size(249, 35);
            BtnDeselectGroup.TabIndex = 31;
            BtnDeselectGroup.Text = "Deselect Entire Group";
            BtnDeselectGroup.UseVisualStyleBackColor = true;
            BtnDeselectGroup.Click += BtnDeselectGroup_Click;
            // 
            // BtnViewTemplateGroups
            // 
            BtnViewTemplateGroups.Location = new Point(402, 212);
            BtnViewTemplateGroups.Name = "BtnViewTemplateGroups";
            BtnViewTemplateGroups.Size = new Size(249, 35);
            BtnViewTemplateGroups.TabIndex = 32;
            BtnViewTemplateGroups.Text = "View Template Groups";
            BtnViewTemplateGroups.UseVisualStyleBackColor = true;
            BtnViewTemplateGroups.Click += BtnViewTemplateGroups_Click;
            // 
            // MultiTemplateSelectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(660, 519);
            Controls.Add(BtnViewTemplateGroups);
            Controls.Add(BtnDeselectGroup);
            Controls.Add(SelectGroup);
            Controls.Add(BtnCancel);
            Controls.Add(BtnConfirm);
            Controls.Add(label6);
            Controls.Add(CmbFactions);
            Controls.Add(label4);
            Controls.Add(label5);
            Controls.Add(CmbOwner);
            Controls.Add(BtnDeselect);
            Controls.Add(BtnSelect);
            Controls.Add(label1);
            Controls.Add(SelectedTemplatesListBox);
            Controls.Add(label3);
            Controls.Add(CmbTemplatesGroup);
            Controls.Add(TxtSearch);
            Controls.Add(label2);
            Controls.Add(ListTemplates);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MultiTemplateSelectorForm";
            Text = "Template Selection";
            Load += MultiTemplateSelectorForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label3;
        private ComboBox CmbTemplatesGroup;
        private TextBox TxtSearch;
        private Label label2;
        private ListBox ListTemplates;
        private ListBox SelectedTemplatesListBox;
        private Label label1;
        private Button BtnSelect;
        private Button BtnDeselect;
        private Label label6;
        private CustomComponents.MultiSelectCombo.NoDropDownComboBox CmbFactions;
        private Label label4;
        private Label label5;
        private ComboBox CmbOwner;
        private Button BtnConfirm;
        private Button BtnCancel;
        private Button SelectGroup;
        private Button BtnDeselectGroup;
        private Button BtnViewTemplateGroups;
    }
}