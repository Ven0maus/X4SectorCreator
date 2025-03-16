namespace X4SectorCreator.Forms
{
    partial class StationForm
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
            BtnCreate = new Button();
            cmbStationType = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            cmbFaction = new ComboBox();
            BtnCancel = new Button();
            SectorHexagon = new PictureBox();
            txtPosition = new TextBox();
            label3 = new Label();
            label4 = new Label();
            txtSector = new TextBox();
            label5 = new Label();
            txtName = new TextBox();
            ((System.ComponentModel.ISupportInitialize)SectorHexagon).BeginInit();
            SuspendLayout();
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(110, 218);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(158, 33);
            BtnCreate.TabIndex = 0;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // cmbStationType
            // 
            cmbStationType.FormattingEnabled = true;
            cmbStationType.Items.AddRange(new object[] { "wharf", "shipyard", "equipmentdock", "tradestation", "defence" });
            cmbStationType.Location = new Point(110, 97);
            cmbStationType.Name = "cmbStationType";
            cmbStationType.Size = new Size(158, 23);
            cmbStationType.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(10, 97);
            label1.Name = "label1";
            label1.Size = new Size(97, 21);
            label1.TabIndex = 2;
            label1.Text = "Station Type:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(45, 126);
            label2.Name = "label2";
            label2.Size = new Size(62, 21);
            label2.TabIndex = 4;
            label2.Text = "Faction:";
            // 
            // cmbFaction
            // 
            cmbFaction.FormattingEnabled = true;
            cmbFaction.Location = new Point(110, 126);
            cmbFaction.Name = "cmbFaction";
            cmbFaction.Size = new Size(158, 23);
            cmbFaction.TabIndex = 3;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(10, 218);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(97, 33);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // SectorHexagon
            // 
            SectorHexagon.Location = new Point(276, 18);
            SectorHexagon.Name = "SectorHexagon";
            SectorHexagon.Size = new Size(300, 300);
            SectorHexagon.TabIndex = 6;
            SectorHexagon.TabStop = false;
            // 
            // txtPosition
            // 
            txtPosition.Location = new Point(110, 157);
            txtPosition.Name = "txtPosition";
            txtPosition.ReadOnly = true;
            txtPosition.Size = new Size(158, 23);
            txtPosition.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(39, 157);
            label3.Name = "label3";
            label3.Size = new Size(68, 21);
            label3.TabIndex = 8;
            label3.Text = "Position:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(50, 189);
            label4.Name = "label4";
            label4.Size = new Size(57, 21);
            label4.TabIndex = 10;
            label4.Text = "Sector:";
            // 
            // txtSector
            // 
            txtSector.Location = new Point(110, 189);
            txtSector.Name = "txtSector";
            txtSector.ReadOnly = true;
            txtSector.Size = new Size(158, 23);
            txtSector.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(52, 66);
            label5.Name = "label5";
            label5.Size = new Size(55, 21);
            label5.TabIndex = 12;
            label5.Text = "Name:";
            // 
            // txtName
            // 
            txtName.Location = new Point(110, 68);
            txtName.Name = "txtName";
            txtName.Size = new Size(158, 23);
            txtName.TabIndex = 11;
            // 
            // StationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 326);
            Controls.Add(label5);
            Controls.Add(txtName);
            Controls.Add(label4);
            Controls.Add(txtSector);
            Controls.Add(label3);
            Controls.Add(txtPosition);
            Controls.Add(SectorHexagon);
            Controls.Add(BtnCancel);
            Controls.Add(label2);
            Controls.Add(cmbFaction);
            Controls.Add(label1);
            Controls.Add(cmbStationType);
            Controls.Add(BtnCreate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "StationForm";
            Text = "X4 Sector Creator";
            ((System.ComponentModel.ISupportInitialize)SectorHexagon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnCreate;
        private ComboBox cmbStationType;
        private Label label1;
        private Label label2;
        private ComboBox cmbFaction;
        private Button BtnCancel;
        private PictureBox SectorHexagon;
        private TextBox txtPosition;
        private Label label3;
        private Label label4;
        private TextBox txtSector;
        private Label label5;
        private TextBox txtName;
    }
}