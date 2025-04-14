namespace X4SectorCreator.Forms
{
    partial class FactionForm
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
            label1 = new Label();
            TxtFactionName = new TextBox();
            BtnPickColor = new Button();
            label3 = new Label();
            TxtDescription = new TextBox();
            label2 = new Label();
            TxtPrefix = new TextBox();
            CmbRace = new ComboBox();
            label4 = new Label();
            label5 = new Label();
            CmbPoliceFaction = new ComboBox();
            BtnFactionRelations = new Button();
            BtnCreate = new Button();
            TagsListBox = new ListBox();
            label6 = new Label();
            BtnAddTag = new Button();
            BtnDeleteTag = new Button();
            BtnUseTagsPreset = new Button();
            BtnDeleteLicense = new Button();
            BtnAddLicense = new Button();
            label7 = new Label();
            LicensesListBox = new ListBox();
            BtnCancel = new Button();
            IconBox = new PictureBox();
            BtnSetIcon = new Button();
            BtnUseLicensesPreset = new Button();
            ((System.ComponentModel.ISupportInitialize)IconBox).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11F);
            label1.Location = new Point(61, 16);
            label1.Name = "label1";
            label1.Size = new Size(52, 20);
            label1.TabIndex = 0;
            label1.Text = "Name:";
            // 
            // TxtFactionName
            // 
            TxtFactionName.Location = new Point(119, 17);
            TxtFactionName.Name = "TxtFactionName";
            TxtFactionName.Size = new Size(199, 23);
            TxtFactionName.TabIndex = 1;
            // 
            // BtnPickColor
            // 
            BtnPickColor.Location = new Point(119, 168);
            BtnPickColor.Name = "BtnPickColor";
            BtnPickColor.Size = new Size(199, 33);
            BtnPickColor.TabIndex = 4;
            BtnPickColor.Text = "Pick Faction Color";
            BtnPickColor.UseVisualStyleBackColor = true;
            BtnPickColor.Click += BtnPickColor_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 11F);
            label3.Location = new Point(25, 41);
            label3.Name = "label3";
            label3.Size = new Size(88, 20);
            label3.TabIndex = 6;
            label3.Text = "Description:";
            // 
            // TxtDescription
            // 
            TxtDescription.Location = new Point(119, 42);
            TxtDescription.Multiline = true;
            TxtDescription.Name = "TxtDescription";
            TxtDescription.Size = new Size(199, 90);
            TxtDescription.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11F);
            label2.Location = new Point(64, 139);
            label2.Name = "label2";
            label2.Size = new Size(49, 20);
            label2.TabIndex = 8;
            label2.Text = "Prefix:";
            // 
            // TxtPrefix
            // 
            TxtPrefix.Location = new Point(119, 139);
            TxtPrefix.Name = "TxtPrefix";
            TxtPrefix.Size = new Size(199, 23);
            TxtPrefix.TabIndex = 9;
            // 
            // CmbRace
            // 
            CmbRace.FormattingEnabled = true;
            CmbRace.Items.AddRange(new object[] { "argon", "paranid", "teladi", "terran", "split", "xenon" });
            CmbRace.Location = new Point(119, 207);
            CmbRace.Name = "CmbRace";
            CmbRace.Size = new Size(199, 23);
            CmbRace.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 11F);
            label4.Location = new Point(69, 207);
            label4.Name = "label4";
            label4.Size = new Size(44, 20);
            label4.TabIndex = 11;
            label4.Text = "Race:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11F);
            label5.Location = new Point(11, 236);
            label5.Name = "label5";
            label5.Size = new Size(102, 20);
            label5.TabIndex = 13;
            label5.Text = "Police Faction:";
            // 
            // CmbPoliceFaction
            // 
            CmbPoliceFaction.FormattingEnabled = true;
            CmbPoliceFaction.Items.AddRange(new object[] { "argon", "paranid", "teladi", "terran", "split", "xenon" });
            CmbPoliceFaction.Location = new Point(119, 236);
            CmbPoliceFaction.Name = "CmbPoliceFaction";
            CmbPoliceFaction.Size = new Size(199, 23);
            CmbPoliceFaction.TabIndex = 12;
            // 
            // BtnFactionRelations
            // 
            BtnFactionRelations.Location = new Point(119, 265);
            BtnFactionRelations.Name = "BtnFactionRelations";
            BtnFactionRelations.Size = new Size(199, 33);
            BtnFactionRelations.TabIndex = 14;
            BtnFactionRelations.Text = "Set Faction Relations";
            BtnFactionRelations.UseVisualStyleBackColor = true;
            BtnFactionRelations.Click += BtnFactionRelations_Click;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(324, 419);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(303, 33);
            BtnCreate.TabIndex = 15;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // TagsListBox
            // 
            TagsListBox.FormattingEnabled = true;
            TagsListBox.Location = new Point(324, 39);
            TagsListBox.Name = "TagsListBox";
            TagsListBox.Size = new Size(194, 109);
            TagsListBox.TabIndex = 16;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 11F);
            label6.Location = new Point(324, 17);
            label6.Name = "label6";
            label6.Size = new Size(38, 20);
            label6.TabIndex = 17;
            label6.Text = "Tags";
            // 
            // BtnAddTag
            // 
            BtnAddTag.Location = new Point(524, 39);
            BtnAddTag.Name = "BtnAddTag";
            BtnAddTag.Size = new Size(103, 33);
            BtnAddTag.TabIndex = 18;
            BtnAddTag.Text = "Add Tag";
            BtnAddTag.UseVisualStyleBackColor = true;
            // 
            // BtnDeleteTag
            // 
            BtnDeleteTag.Location = new Point(524, 78);
            BtnDeleteTag.Name = "BtnDeleteTag";
            BtnDeleteTag.Size = new Size(103, 33);
            BtnDeleteTag.TabIndex = 19;
            BtnDeleteTag.Text = "Delete Tag";
            BtnDeleteTag.UseVisualStyleBackColor = true;
            // 
            // BtnUseTagsPreset
            // 
            BtnUseTagsPreset.Location = new Point(524, 117);
            BtnUseTagsPreset.Name = "BtnUseTagsPreset";
            BtnUseTagsPreset.Size = new Size(103, 33);
            BtnUseTagsPreset.TabIndex = 20;
            BtnUseTagsPreset.Text = "Use Preset";
            BtnUseTagsPreset.UseVisualStyleBackColor = true;
            // 
            // BtnDeleteLicense
            // 
            BtnDeleteLicense.Location = new Point(524, 224);
            BtnDeleteLicense.Name = "BtnDeleteLicense";
            BtnDeleteLicense.Size = new Size(103, 33);
            BtnDeleteLicense.TabIndex = 24;
            BtnDeleteLicense.Text = "Delete License";
            BtnDeleteLicense.UseVisualStyleBackColor = true;
            // 
            // BtnAddLicense
            // 
            BtnAddLicense.Location = new Point(524, 185);
            BtnAddLicense.Name = "BtnAddLicense";
            BtnAddLicense.Size = new Size(103, 33);
            BtnAddLicense.TabIndex = 23;
            BtnAddLicense.Text = "Add License";
            BtnAddLicense.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 11F);
            label7.Location = new Point(324, 161);
            label7.Name = "label7";
            label7.Size = new Size(63, 20);
            label7.TabIndex = 22;
            label7.Text = "Licenses";
            // 
            // LicensesListBox
            // 
            LicensesListBox.FormattingEnabled = true;
            LicensesListBox.Location = new Point(324, 183);
            LicensesListBox.Name = "LicensesListBox";
            LicensesListBox.Size = new Size(194, 229);
            LicensesListBox.TabIndex = 21;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(11, 419);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(307, 33);
            BtnCancel.TabIndex = 25;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // IconBox
            // 
            IconBox.BorderStyle = BorderStyle.FixedSingle;
            IconBox.Location = new Point(218, 304);
            IconBox.Name = "IconBox";
            IconBox.Size = new Size(100, 100);
            IconBox.TabIndex = 26;
            IconBox.TabStop = false;
            // 
            // BtnSetIcon
            // 
            BtnSetIcon.Location = new Point(119, 371);
            BtnSetIcon.Name = "BtnSetIcon";
            BtnSetIcon.Size = new Size(93, 33);
            BtnSetIcon.TabIndex = 27;
            BtnSetIcon.Text = "Set Icon";
            BtnSetIcon.UseVisualStyleBackColor = true;
            // 
            // BtnUseLicensesPreset
            // 
            BtnUseLicensesPreset.Location = new Point(524, 263);
            BtnUseLicensesPreset.Name = "BtnUseLicensesPreset";
            BtnUseLicensesPreset.Size = new Size(103, 33);
            BtnUseLicensesPreset.TabIndex = 28;
            BtnUseLicensesPreset.Text = "Use Preset";
            BtnUseLicensesPreset.UseVisualStyleBackColor = true;
            // 
            // FactionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(639, 460);
            Controls.Add(BtnUseLicensesPreset);
            Controls.Add(BtnSetIcon);
            Controls.Add(IconBox);
            Controls.Add(BtnCancel);
            Controls.Add(BtnDeleteLicense);
            Controls.Add(BtnAddLicense);
            Controls.Add(label7);
            Controls.Add(LicensesListBox);
            Controls.Add(BtnUseTagsPreset);
            Controls.Add(BtnDeleteTag);
            Controls.Add(BtnAddTag);
            Controls.Add(label6);
            Controls.Add(TagsListBox);
            Controls.Add(BtnCreate);
            Controls.Add(BtnFactionRelations);
            Controls.Add(label5);
            Controls.Add(CmbPoliceFaction);
            Controls.Add(label4);
            Controls.Add(CmbRace);
            Controls.Add(TxtPrefix);
            Controls.Add(label2);
            Controls.Add(TxtDescription);
            Controls.Add(label3);
            Controls.Add(BtnPickColor);
            Controls.Add(TxtFactionName);
            Controls.Add(label1);
            Name = "FactionForm";
            Text = "Custom Faction Editor";
            ((System.ComponentModel.ISupportInitialize)IconBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox TxtFactionName;
        private Button BtnPickColor;
        private Label label3;
        private TextBox TxtDescription;
        private Label label2;
        private TextBox TxtPrefix;
        private ComboBox CmbRace;
        private Label label4;
        private Label label5;
        private ComboBox CmbPoliceFaction;
        private Button BtnFactionRelations;
        private Button BtnCreate;
        private ListBox TagsListBox;
        private Label label6;
        private Button BtnAddTag;
        private Button BtnDeleteTag;
        private Button BtnUseTagsPreset;
        private Button BtnDeleteLicense;
        private Button BtnAddLicense;
        private Label label7;
        private ListBox LicensesListBox;
        private Button BtnCancel;
        private PictureBox IconBox;
        private Button BtnSetIcon;
        private Button BtnUseLicensesPreset;
    }
}