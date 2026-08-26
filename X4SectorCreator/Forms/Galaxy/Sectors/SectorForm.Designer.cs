namespace X4SectorCreator.Forms
{
    partial class SectorForm
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
            components = new System.ComponentModel.Container();
            label1 = new Label();
            TxtName = new TextBox();
            BtnCancel = new Button();
            BtnCreate = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            chkAllowRandomAnomalies = new CheckBox();
            txtSunlight = new TextBox();
            txtSecurity = new TextBox();
            txtEconomy = new TextBox();
            label6 = new Label();
            txtDescription = new TextBox();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            chkDisableFactionLogic = new CheckBox();
            txtSectorRadius = new TextBox();
            label10 = new Label();
            lblRadiusUnderText = new Label();
            cmbPlacement = new ComboBox();
            label11 = new Label();
            RAListBox = new ListBox();
            BtnAddRA = new Button();
            BtnDeleteRA = new Button();
            label12 = new Label();
            Tooltip = new ToolTip(components);
            tabControl1 = new TabControl();
            Resources = new TabPage();
            Worlds = new TabPage();
            label5 = new Label();
            BtnDeleteWorldLink = new Button();
            BtnLinkWorld = new Button();
            ListBoxWorlds = new ListBox();
            tabControl1.SuspendLayout();
            Resources.SuspendLayout();
            Worlds.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(51, 17);
            label1.Name = "label1";
            label1.Size = new Size(55, 21);
            label1.TabIndex = 10;
            label1.Text = "Name:";
            // 
            // TxtName
            // 
            TxtName.Location = new Point(112, 17);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(196, 23);
            TxtName.TabIndex = 9;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(10, 404);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(138, 30);
            BtnCancel.TabIndex = 8;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(154, 404);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(516, 30);
            BtnCreate.TabIndex = 7;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(34, 269);
            label2.Name = "label2";
            label2.Size = new Size(71, 21);
            label2.TabIndex = 11;
            label2.Text = "Sunlight:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(28, 297);
            label3.Name = "label3";
            label3.Size = new Size(77, 21);
            label3.TabIndex = 12;
            label3.Text = "Economy:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(36, 326);
            label4.Name = "label4";
            label4.Size = new Size(69, 21);
            label4.TabIndex = 13;
            label4.Text = "Security:";
            // 
            // chkAllowRandomAnomalies
            // 
            chkAllowRandomAnomalies.AutoSize = true;
            chkAllowRandomAnomalies.Checked = true;
            chkAllowRandomAnomalies.CheckState = CheckState.Checked;
            chkAllowRandomAnomalies.Location = new Point(19, 358);
            chkAllowRandomAnomalies.Name = "chkAllowRandomAnomalies";
            chkAllowRandomAnomalies.Size = new Size(199, 19);
            chkAllowRandomAnomalies.TabIndex = 14;
            chkAllowRandomAnomalies.Text = "Allow Random Sector Anomalies";
            chkAllowRandomAnomalies.UseVisualStyleBackColor = true;
            // 
            // txtSunlight
            // 
            txtSunlight.Location = new Point(111, 269);
            txtSunlight.Name = "txtSunlight";
            txtSunlight.Size = new Size(196, 23);
            txtSunlight.TabIndex = 15;
            txtSunlight.Text = "100";
            // 
            // txtSecurity
            // 
            txtSecurity.Location = new Point(111, 326);
            txtSecurity.Name = "txtSecurity";
            txtSecurity.Size = new Size(196, 23);
            txtSecurity.TabIndex = 17;
            txtSecurity.Text = "100";
            // 
            // txtEconomy
            // 
            txtEconomy.Location = new Point(111, 297);
            txtEconomy.Name = "txtEconomy";
            txtEconomy.Size = new Size(196, 23);
            txtEconomy.TabIndex = 16;
            txtEconomy.Text = "100";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(14, 46);
            label6.Name = "label6";
            label6.Size = new Size(92, 21);
            label6.TabIndex = 21;
            label6.Text = "Description:";
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(111, 46);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(196, 129);
            txtDescription.TabIndex = 20;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(307, 269);
            label7.Name = "label7";
            label7.Size = new Size(23, 21);
            label7.TabIndex = 22;
            label7.Text = "%";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(307, 297);
            label8.Name = "label8";
            label8.Size = new Size(23, 21);
            label8.TabIndex = 23;
            label8.Text = "%";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(307, 326);
            label9.Name = "label9";
            label9.Size = new Size(23, 21);
            label9.TabIndex = 24;
            label9.Text = "%";
            // 
            // chkDisableFactionLogic
            // 
            chkDisableFactionLogic.AutoSize = true;
            chkDisableFactionLogic.Location = new Point(19, 379);
            chkDisableFactionLogic.Name = "chkDisableFactionLogic";
            chkDisableFactionLogic.Size = new Size(303, 19);
            chkDisableFactionLogic.TabIndex = 25;
            chkDisableFactionLogic.Text = "Disable Faction Logic (factions can't take ownership)";
            chkDisableFactionLogic.UseVisualStyleBackColor = true;
            // 
            // txtSectorRadius
            // 
            txtSectorRadius.Location = new Point(111, 212);
            txtSectorRadius.Name = "txtSectorRadius";
            txtSectorRadius.Size = new Size(196, 23);
            txtSectorRadius.TabIndex = 27;
            txtSectorRadius.Text = "250";
            txtSectorRadius.TextChanged += TxtSectorRadius_TextChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F);
            label10.Location = new Point(45, 212);
            label10.Name = "label10";
            label10.Size = new Size(60, 21);
            label10.TabIndex = 26;
            label10.Text = "Radius:";
            // 
            // lblRadiusUnderText
            // 
            lblRadiusUnderText.Font = new Font("Segoe UI", 8F);
            lblRadiusUnderText.Location = new Point(110, 237);
            lblRadiusUnderText.Name = "lblRadiusUnderText";
            lblRadiusUnderText.Size = new Size(201, 32);
            lblRadiusUnderText.TabIndex = 28;
            lblRadiusUnderText.Text = "From the center, 250km in every direction. 500km diameter.";
            // 
            // cmbPlacement
            // 
            cmbPlacement.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPlacement.Enabled = false;
            cmbPlacement.FormattingEnabled = true;
            cmbPlacement.Location = new Point(111, 181);
            cmbPlacement.Name = "cmbPlacement";
            cmbPlacement.Size = new Size(196, 23);
            cmbPlacement.TabIndex = 29;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(20, 181);
            label11.Name = "label11";
            label11.Size = new Size(85, 21);
            label11.TabIndex = 30;
            label11.Text = "Placement:";
            // 
            // RAListBox
            // 
            RAListBox.DrawMode = DrawMode.OwnerDrawFixed;
            RAListBox.FormattingEnabled = true;
            RAListBox.Location = new Point(6, 6);
            RAListBox.Name = "RAListBox";
            RAListBox.Size = new Size(318, 244);
            RAListBox.TabIndex = 31;
            RAListBox.DrawItem += RAListBox_DrawItem;
            RAListBox.DoubleClick += RAListBox_DoubleClick;
            // 
            // BtnAddRA
            // 
            BtnAddRA.Location = new Point(119, 253);
            BtnAddRA.Name = "BtnAddRA";
            BtnAddRA.Size = new Size(205, 30);
            BtnAddRA.TabIndex = 33;
            BtnAddRA.Text = "Add Resource Area";
            BtnAddRA.UseVisualStyleBackColor = true;
            BtnAddRA.Click += BtnAddRA_Click;
            // 
            // BtnDeleteRA
            // 
            BtnDeleteRA.Location = new Point(6, 253);
            BtnDeleteRA.Name = "BtnDeleteRA";
            BtnDeleteRA.Size = new Size(107, 30);
            BtnDeleteRA.TabIndex = 34;
            BtnDeleteRA.Text = "Delete Area";
            BtnDeleteRA.UseVisualStyleBackColor = true;
            BtnDeleteRA.Click += BtnDeleteRA_Click;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9F);
            label12.Location = new Point(16, 288);
            label12.Name = "label12";
            label12.Size = new Size(297, 45);
            label12.TabIndex = 35;
            label12.Text = "Note: For each resource type a matching region \r\nneeds to be defined with the correct mineral/gas fields.\r\nVanilla resource areas are colored cyan.";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(Resources);
            tabControl1.Controls.Add(Worlds);
            tabControl1.Location = new Point(331, 17);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(343, 372);
            tabControl1.TabIndex = 38;
            // 
            // Resources
            // 
            Resources.Controls.Add(RAListBox);
            Resources.Controls.Add(BtnAddRA);
            Resources.Controls.Add(label12);
            Resources.Controls.Add(BtnDeleteRA);
            Resources.Location = new Point(4, 24);
            Resources.Name = "Resources";
            Resources.Padding = new Padding(3);
            Resources.Size = new Size(335, 344);
            Resources.TabIndex = 0;
            Resources.Text = "Resources";
            Resources.UseVisualStyleBackColor = true;
            // 
            // Worlds
            // 
            Worlds.Controls.Add(label5);
            Worlds.Controls.Add(BtnDeleteWorldLink);
            Worlds.Controls.Add(BtnLinkWorld);
            Worlds.Controls.Add(ListBoxWorlds);
            Worlds.Location = new Point(4, 24);
            Worlds.Name = "Worlds";
            Worlds.Padding = new Padding(3);
            Worlds.Size = new Size(335, 344);
            Worlds.TabIndex = 1;
            Worlds.Text = "Worlds";
            Worlds.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F);
            label5.Location = new Point(12, 306);
            label5.Name = "label5";
            label5.Size = new Size(310, 30);
            label5.TabIndex = 36;
            label5.Text = "Note: Star System Information on the cluster\r\nmust have atleast one planet to link a world to this sector.";
            // 
            // BtnDeleteWorldLink
            // 
            BtnDeleteWorldLink.Location = new Point(6, 268);
            BtnDeleteWorldLink.Name = "BtnDeleteWorldLink";
            BtnDeleteWorldLink.Size = new Size(103, 33);
            BtnDeleteWorldLink.TabIndex = 2;
            BtnDeleteWorldLink.Text = "Delete Link";
            BtnDeleteWorldLink.UseVisualStyleBackColor = true;
            BtnDeleteWorldLink.Click += BtnDeleteWorldLink_Click;
            // 
            // BtnLinkWorld
            // 
            BtnLinkWorld.Location = new Point(115, 268);
            BtnLinkWorld.Name = "BtnLinkWorld";
            BtnLinkWorld.Size = new Size(214, 33);
            BtnLinkWorld.TabIndex = 1;
            BtnLinkWorld.Text = "Link World";
            BtnLinkWorld.UseVisualStyleBackColor = true;
            BtnLinkWorld.Click += BtnLinkWorld_Click;
            // 
            // ListBoxWorlds
            // 
            ListBoxWorlds.FormattingEnabled = true;
            ListBoxWorlds.Location = new Point(6, 6);
            ListBoxWorlds.Name = "ListBoxWorlds";
            ListBoxWorlds.Size = new Size(323, 259);
            ListBoxWorlds.TabIndex = 0;
            ListBoxWorlds.DoubleClick += ListBoxWorlds_DoubleClick;
            // 
            // SectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(681, 445);
            Controls.Add(tabControl1);
            Controls.Add(label11);
            Controls.Add(cmbPlacement);
            Controls.Add(lblRadiusUnderText);
            Controls.Add(txtSectorRadius);
            Controls.Add(label10);
            Controls.Add(chkDisableFactionLogic);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(txtDescription);
            Controls.Add(txtSecurity);
            Controls.Add(txtEconomy);
            Controls.Add(txtSunlight);
            Controls.Add(chkAllowRandomAnomalies);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(TxtName);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SectorForm";
            Text = "Sector Editor";
            tabControl1.ResumeLayout(false);
            Resources.ResumeLayout(false);
            Resources.PerformLayout();
            Worlds.ResumeLayout(false);
            Worlds.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button BtnCancel;
        internal Button BtnCreate;
        private Label label2;
        private Label label3;
        private Label label4;
        private CheckBox chkAllowRandomAnomalies;
        internal TextBox txtSunlight;
        internal TextBox txtSecurity;
        internal TextBox txtEconomy;
        private Label label6;
        internal TextBox txtDescription;
        private Label label7;
        private Label label8;
        private Label label9;
        private TextBox TxtName;
        private CheckBox chkDisableFactionLogic;
        internal TextBox txtSectorRadius;
        private Label label10;
        private Label lblRadiusUnderText;
        private ComboBox cmbPlacement;
        private Label label11;
        internal Button BtnAddRA;
        internal Button BtnDeleteRA;
        public ListBox RAListBox;
        private Label label12;
        private ToolTip Tooltip;
        private TabControl tabControl1;
        private TabPage Resources;
        private TabPage Worlds;
        private Button BtnLinkWorld;
        private Label label5;
        private Button BtnDeleteWorldLink;
        public ListBox ListBoxWorlds;
    }
}