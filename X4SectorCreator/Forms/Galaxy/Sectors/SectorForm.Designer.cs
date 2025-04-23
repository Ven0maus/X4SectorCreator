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
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(57, 17);
            label1.Name = "label1";
            label1.Size = new Size(55, 21);
            label1.TabIndex = 10;
            label1.Text = "Name:";
            // 
            // TxtName
            // 
            TxtName.Location = new Point(118, 17);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(196, 23);
            TxtName.TabIndex = 9;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(13, 400);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(97, 30);
            BtnCancel.TabIndex = 8;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(116, 400);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(196, 30);
            BtnCreate.TabIndex = 7;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(40, 269);
            label2.Name = "label2";
            label2.Size = new Size(71, 21);
            label2.TabIndex = 11;
            label2.Text = "Sunlight:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(34, 297);
            label3.Name = "label3";
            label3.Size = new Size(77, 21);
            label3.TabIndex = 12;
            label3.Text = "Economy:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(42, 326);
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
            chkAllowRandomAnomalies.Location = new Point(13, 354);
            chkAllowRandomAnomalies.Name = "chkAllowRandomAnomalies";
            chkAllowRandomAnomalies.Size = new Size(199, 19);
            chkAllowRandomAnomalies.TabIndex = 14;
            chkAllowRandomAnomalies.Text = "Allow Random Sector Anomalies";
            chkAllowRandomAnomalies.UseVisualStyleBackColor = true;
            // 
            // txtSunlight
            // 
            txtSunlight.Location = new Point(117, 269);
            txtSunlight.Name = "txtSunlight";
            txtSunlight.Size = new Size(196, 23);
            txtSunlight.TabIndex = 15;
            txtSunlight.Text = "100";
            // 
            // txtSecurity
            // 
            txtSecurity.Location = new Point(117, 326);
            txtSecurity.Name = "txtSecurity";
            txtSecurity.Size = new Size(196, 23);
            txtSecurity.TabIndex = 17;
            txtSecurity.Text = "100";
            // 
            // txtEconomy
            // 
            txtEconomy.Location = new Point(117, 297);
            txtEconomy.Name = "txtEconomy";
            txtEconomy.Size = new Size(196, 23);
            txtEconomy.TabIndex = 16;
            txtEconomy.Text = "100";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(20, 46);
            label6.Name = "label6";
            label6.Size = new Size(92, 21);
            label6.TabIndex = 21;
            label6.Text = "Description:";
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(117, 46);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(196, 129);
            txtDescription.TabIndex = 20;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(309, 270);
            label7.Name = "label7";
            label7.Size = new Size(23, 21);
            label7.TabIndex = 22;
            label7.Text = "%";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(309, 298);
            label8.Name = "label8";
            label8.Size = new Size(23, 21);
            label8.TabIndex = 23;
            label8.Text = "%";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(309, 327);
            label9.Name = "label9";
            label9.Size = new Size(23, 21);
            label9.TabIndex = 24;
            label9.Text = "%";
            // 
            // chkDisableFactionLogic
            // 
            chkDisableFactionLogic.AutoSize = true;
            chkDisableFactionLogic.Location = new Point(13, 375);
            chkDisableFactionLogic.Name = "chkDisableFactionLogic";
            chkDisableFactionLogic.Size = new Size(303, 19);
            chkDisableFactionLogic.TabIndex = 25;
            chkDisableFactionLogic.Text = "Disable Faction Logic (factions can't take ownership)";
            chkDisableFactionLogic.UseVisualStyleBackColor = true;
            // 
            // txtSectorRadius
            // 
            txtSectorRadius.Location = new Point(117, 212);
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
            label10.Location = new Point(51, 212);
            label10.Name = "label10";
            label10.Size = new Size(60, 21);
            label10.TabIndex = 26;
            label10.Text = "Radius:";
            // 
            // lblRadiusUnderText
            // 
            lblRadiusUnderText.Font = new Font("Segoe UI", 8F);
            lblRadiusUnderText.Location = new Point(116, 238);
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
            cmbPlacement.Location = new Point(117, 181);
            cmbPlacement.Name = "cmbPlacement";
            cmbPlacement.Size = new Size(196, 23);
            cmbPlacement.TabIndex = 29;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(26, 181);
            label11.Name = "label11";
            label11.Size = new Size(85, 21);
            label11.TabIndex = 30;
            label11.Text = "Placement:";
            // 
            // SectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(333, 434);
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
    }
}