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
            TxtShortName = new TextBox();
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
            BtnCancel = new Button();
            IconBox = new PictureBox();
            BtnSetIcon = new Button();
            BtnEditXml = new Button();
            TxtPrefix = new TextBox();
            label7 = new Label();
            BtnEditFactionShips = new Button();
            LblIconSize = new Label();
            BtnFactionStations = new Button();
            TxtPreferredHqSpace = new TextBox();
            label8 = new Label();
            label9 = new Label();
            CmbAggression = new ComboBox();
            CmbAvarice = new ComboBox();
            label10 = new Label();
            label11 = new Label();
            TxtLawfulness = new TextBox();
            BtnGenRandomIcon = new Button();
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
            BtnPickColor.Location = new Point(430, 154);
            BtnPickColor.Name = "BtnPickColor";
            BtnPickColor.Size = new Size(197, 34);
            BtnPickColor.TabIndex = 4;
            BtnPickColor.Text = "Set Faction Color";
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
            label2.Location = new Point(26, 140);
            label2.Name = "label2";
            label2.Size = new Size(87, 20);
            label2.TabIndex = 8;
            label2.Text = "ShortName:";
            // 
            // TxtShortName
            // 
            TxtShortName.Location = new Point(119, 139);
            TxtShortName.Name = "TxtShortName";
            TxtShortName.Size = new Size(199, 23);
            TxtShortName.TabIndex = 9;
            // 
            // CmbRace
            // 
            CmbRace.FormattingEnabled = true;
            CmbRace.Location = new Point(119, 199);
            CmbRace.Name = "CmbRace";
            CmbRace.Size = new Size(199, 23);
            CmbRace.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 11F);
            label4.Location = new Point(69, 199);
            label4.Name = "label4";
            label4.Size = new Size(44, 20);
            label4.TabIndex = 11;
            label4.Text = "Race:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11F);
            label5.Location = new Point(11, 228);
            label5.Name = "label5";
            label5.Size = new Size(102, 20);
            label5.TabIndex = 13;
            label5.Text = "Police Faction:";
            // 
            // CmbPoliceFaction
            // 
            CmbPoliceFaction.FormattingEnabled = true;
            CmbPoliceFaction.Location = new Point(119, 228);
            CmbPoliceFaction.Name = "CmbPoliceFaction";
            CmbPoliceFaction.Size = new Size(199, 23);
            CmbPoliceFaction.TabIndex = 12;
            // 
            // BtnFactionRelations
            // 
            BtnFactionRelations.Location = new Point(323, 310);
            BtnFactionRelations.Name = "BtnFactionRelations";
            BtnFactionRelations.Size = new Size(306, 33);
            BtnFactionRelations.TabIndex = 14;
            BtnFactionRelations.Text = "Setup Faction Relations";
            BtnFactionRelations.UseVisualStyleBackColor = true;
            BtnFactionRelations.Click += BtnFactionRelations_Click;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(323, 390);
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
            TagsListBox.HorizontalScrollbar = true;
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
            BtnAddTag.Click += BtnAddTag_Click;
            // 
            // BtnDeleteTag
            // 
            BtnDeleteTag.Location = new Point(524, 78);
            BtnDeleteTag.Name = "BtnDeleteTag";
            BtnDeleteTag.Size = new Size(103, 33);
            BtnDeleteTag.TabIndex = 19;
            BtnDeleteTag.Text = "Delete Tag";
            BtnDeleteTag.UseVisualStyleBackColor = true;
            BtnDeleteTag.Click += BtnDeleteTag_Click;
            // 
            // BtnUseTagsPreset
            // 
            BtnUseTagsPreset.Location = new Point(524, 117);
            BtnUseTagsPreset.Name = "BtnUseTagsPreset";
            BtnUseTagsPreset.Size = new Size(103, 33);
            BtnUseTagsPreset.TabIndex = 20;
            BtnUseTagsPreset.Text = "Use Preset";
            BtnUseTagsPreset.UseVisualStyleBackColor = true;
            BtnUseTagsPreset.Click += BtnUseTagsPreset_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(11, 390);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(306, 33);
            BtnCancel.TabIndex = 25;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // IconBox
            // 
            IconBox.BorderStyle = BorderStyle.FixedSingle;
            IconBox.Location = new Point(324, 154);
            IconBox.Name = "IconBox";
            IconBox.Size = new Size(100, 100);
            IconBox.SizeMode = PictureBoxSizeMode.StretchImage;
            IconBox.TabIndex = 26;
            IconBox.TabStop = false;
            // 
            // BtnSetIcon
            // 
            BtnSetIcon.Location = new Point(324, 259);
            BtnSetIcon.Name = "BtnSetIcon";
            BtnSetIcon.Size = new Size(80, 33);
            BtnSetIcon.TabIndex = 27;
            BtnSetIcon.Text = "Upload Icon";
            BtnSetIcon.UseVisualStyleBackColor = true;
            BtnSetIcon.Click += BtnSetIcon_Click;
            // 
            // BtnEditXml
            // 
            BtnEditXml.Location = new Point(324, 351);
            BtnEditXml.Name = "BtnEditXml";
            BtnEditXml.Size = new Size(302, 33);
            BtnEditXml.TabIndex = 28;
            BtnEditXml.Text = "Edit XML (Advanced)";
            BtnEditXml.UseVisualStyleBackColor = true;
            BtnEditXml.Click += BtnEditXml_Click;
            // 
            // TxtPrefix
            // 
            TxtPrefix.Location = new Point(119, 168);
            TxtPrefix.Name = "TxtPrefix";
            TxtPrefix.Size = new Size(199, 23);
            TxtPrefix.TabIndex = 30;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 11F);
            label7.Location = new Point(64, 168);
            label7.Name = "label7";
            label7.Size = new Size(49, 20);
            label7.TabIndex = 29;
            label7.Text = "Prefix:";
            // 
            // BtnEditFactionShips
            // 
            BtnEditFactionShips.Location = new Point(162, 351);
            BtnEditFactionShips.Name = "BtnEditFactionShips";
            BtnEditFactionShips.Size = new Size(155, 33);
            BtnEditFactionShips.TabIndex = 31;
            BtnEditFactionShips.Text = "Setup Faction Ships";
            BtnEditFactionShips.UseVisualStyleBackColor = true;
            BtnEditFactionShips.Click += BtnEditFactionShips_Click;
            // 
            // LblIconSize
            // 
            LblIconSize.Font = new Font("Segoe UI", 11F);
            LblIconSize.Location = new Point(325, 155);
            LblIconSize.Name = "LblIconSize";
            LblIconSize.Size = new Size(98, 98);
            LblIconSize.TabIndex = 32;
            LblIconSize.Text = "256x256";
            LblIconSize.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // BtnFactionStations
            // 
            BtnFactionStations.Location = new Point(11, 351);
            BtnFactionStations.Name = "BtnFactionStations";
            BtnFactionStations.Size = new Size(145, 33);
            BtnFactionStations.TabIndex = 34;
            BtnFactionStations.Text = "Setup Faction Stations";
            BtnFactionStations.UseVisualStyleBackColor = true;
            BtnFactionStations.Click += BtnFactionStations_Click;
            // 
            // TxtPreferredHqSpace
            // 
            TxtPreferredHqSpace.Location = new Point(430, 214);
            TxtPreferredHqSpace.Name = "TxtPreferredHqSpace";
            TxtPreferredHqSpace.PlaceholderText = "Select Preferred HeadQuaters";
            TxtPreferredHqSpace.ReadOnly = true;
            TxtPreferredHqSpace.Size = new Size(197, 23);
            TxtPreferredHqSpace.TabIndex = 36;
            TxtPreferredHqSpace.MouseClick += TxtPreferredHqSpace_MouseClick;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 11F);
            label8.Location = new Point(429, 191);
            label8.Name = "label8";
            label8.Size = new Size(169, 20);
            label8.TabIndex = 37;
            label8.Text = "Preferred HeadQuarters:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 11F);
            label9.Location = new Point(11, 262);
            label9.Name = "label9";
            label9.Size = new Size(124, 20);
            label9.TabIndex = 39;
            label9.Text = "Aggression Level:";
            // 
            // CmbAggression
            // 
            CmbAggression.FormattingEnabled = true;
            CmbAggression.Location = new Point(141, 262);
            CmbAggression.Name = "CmbAggression";
            CmbAggression.Size = new Size(177, 23);
            CmbAggression.TabIndex = 40;
            // 
            // CmbAvarice
            // 
            CmbAvarice.FormattingEnabled = true;
            CmbAvarice.Location = new Point(141, 292);
            CmbAvarice.Name = "CmbAvarice";
            CmbAvarice.Size = new Size(177, 23);
            CmbAvarice.TabIndex = 42;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 11F);
            label10.Location = new Point(36, 292);
            label10.Name = "label10";
            label10.Size = new Size(99, 20);
            label10.TabIndex = 41;
            label10.Text = "Avarice Level:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 11F);
            label11.Location = new Point(14, 322);
            label11.Name = "label11";
            label11.Size = new Size(121, 20);
            label11.TabIndex = 43;
            label11.Text = "Lawfulness Level:";
            // 
            // TxtLawfulness
            // 
            TxtLawfulness.Location = new Point(141, 320);
            TxtLawfulness.Name = "TxtLawfulness";
            TxtLawfulness.PlaceholderText = "(Between 0-1)";
            TxtLawfulness.Size = new Size(177, 23);
            TxtLawfulness.TabIndex = 44;
            // 
            // BtnGenRandomIcon
            // 
            BtnGenRandomIcon.Font = new Font("Segoe UI", 10F);
            BtnGenRandomIcon.Location = new Point(404, 259);
            BtnGenRandomIcon.Name = "BtnGenRandomIcon";
            BtnGenRandomIcon.Size = new Size(22, 33);
            BtnGenRandomIcon.TabIndex = 45;
            BtnGenRandomIcon.Text = "🎲";
            BtnGenRandomIcon.UseVisualStyleBackColor = true;
            BtnGenRandomIcon.Click += BtnGenRandomIcon_Click;
            // 
            // FactionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(632, 427);
            Controls.Add(BtnGenRandomIcon);
            Controls.Add(TxtLawfulness);
            Controls.Add(label11);
            Controls.Add(CmbAvarice);
            Controls.Add(label10);
            Controls.Add(CmbAggression);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(TxtPreferredHqSpace);
            Controls.Add(BtnFactionStations);
            Controls.Add(LblIconSize);
            Controls.Add(BtnEditFactionShips);
            Controls.Add(TxtPrefix);
            Controls.Add(label7);
            Controls.Add(BtnEditXml);
            Controls.Add(BtnSetIcon);
            Controls.Add(IconBox);
            Controls.Add(BtnCancel);
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
            Controls.Add(TxtShortName);
            Controls.Add(label2);
            Controls.Add(TxtDescription);
            Controls.Add(label3);
            Controls.Add(BtnPickColor);
            Controls.Add(TxtFactionName);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
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
        private TextBox TxtShortName;
        private ComboBox CmbRace;
        private Label label4;
        private Label label5;
        private ComboBox CmbPoliceFaction;
        private Button BtnFactionRelations;
        private ListBox TagsListBox;
        private Label label6;
        private Button BtnAddTag;
        private Button BtnDeleteTag;
        private Button BtnUseTagsPreset;
        private Button BtnCancel;
        private PictureBox IconBox;
        private Button BtnSetIcon;
        private Button BtnEditXml;
        private TextBox TxtPrefix;
        private Label label7;
        internal Button BtnCreate;
        private Button BtnEditFactionShips;
        private Label LblIconSize;
        private Button BtnFactionStations;
        private TextBox TxtPreferredHqSpace;
        private Label label8;
        private Label label9;
        private ComboBox CmbAggression;
        private ComboBox CmbAvarice;
        private Label label10;
        private Label label11;
        private TextBox TxtLawfulness;
        private Button BtnGenRandomIcon;
    }
}