namespace X4SectorCreator.Forms
{
    partial class RegionResourcesForm
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
            cmbWare = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            cmbYield = new ComboBox();
            BtnAdd = new Button();
            BtnCancel = new Button();
            SuspendLayout();
            // 
            // cmbWare
            // 
            cmbWare.FormattingEnabled = true;
            cmbWare.Items.AddRange(new object[] { "ore", "silicon", "ice", "nividium", "hydrogen", "helium", "methane", "rawscrap" });
            cmbWare.Location = new Point(68, 12);
            cmbWare.Name = "cmbWare";
            cmbWare.Size = new Size(174, 23);
            cmbWare.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(49, 21);
            label1.TabIndex = 1;
            label1.Text = "Ware:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(14, 44);
            label2.Name = "label2";
            label2.Size = new Size(47, 21);
            label2.TabIndex = 2;
            label2.Text = "Yield:";
            // 
            // cmbYield
            // 
            cmbYield.FormattingEnabled = true;
            cmbYield.Items.AddRange(new object[] { "lowest", "verylow", "lowminus", "low", "lowplus", "medlow", "medium", "medplus", "medhigh", "highlow", "high", "highplus", "veryhigh", "highest" });
            cmbYield.Location = new Point(68, 44);
            cmbYield.Name = "cmbYield";
            cmbYield.Size = new Size(174, 23);
            cmbYield.TabIndex = 3;
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(106, 73);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(136, 31);
            BtnAdd.TabIndex = 4;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 73);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(88, 31);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // RegionResourcesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(256, 114);
            Controls.Add(BtnCancel);
            Controls.Add(BtnAdd);
            Controls.Add(cmbYield);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cmbWare);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionResourcesForm";
            Text = "Region Resource Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbWare;
        private Label label1;
        private Label label2;
        private ComboBox cmbYield;
        private Button BtnAdd;
        private Button BtnCancel;
    }
}