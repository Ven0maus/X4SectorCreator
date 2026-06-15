namespace X4SectorCreator.Forms
{
    partial class ResourceAreaForm
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
            cmbSize = new ComboBox();
            label3 = new Label();
            label4 = new Label();
            nrAmount = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)nrAmount).BeginInit();
            SuspendLayout();
            // 
            // cmbWare
            // 
            cmbWare.FormattingEnabled = true;
            cmbWare.Items.AddRange(new object[] { "ore", "silicon", "ice", "nividium", "hydrogen", "helium", "methane", "rawscrap" });
            cmbWare.Location = new Point(91, 12);
            cmbWare.Name = "cmbWare";
            cmbWare.Size = new Size(174, 23);
            cmbWare.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(14, 14);
            label1.Name = "label1";
            label1.Size = new Size(49, 21);
            label1.TabIndex = 1;
            label1.Text = "Ware:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(14, 46);
            label2.Name = "label2";
            label2.Size = new Size(47, 21);
            label2.TabIndex = 2;
            label2.Text = "Yield:";
            // 
            // cmbYield
            // 
            cmbYield.FormattingEnabled = true;
            cmbYield.Items.AddRange(new object[] { "verylow", "low", "medium", "high", "veryhigh" });
            cmbYield.Location = new Point(91, 44);
            cmbYield.Name = "cmbYield";
            cmbYield.Size = new Size(174, 23);
            cmbYield.TabIndex = 3;
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(119, 134);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(136, 31);
            BtnAdd.TabIndex = 4;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(25, 134);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(88, 31);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // cmbSize
            // 
            cmbSize.FormattingEnabled = true;
            cmbSize.Items.AddRange(new object[] { "verylow", "low", "medium", "high", "veryhigh" });
            cmbSize.Location = new Point(91, 73);
            cmbSize.Name = "cmbSize";
            cmbSize.Size = new Size(174, 23);
            cmbSize.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(14, 75);
            label3.Name = "label3";
            label3.Size = new Size(41, 21);
            label3.TabIndex = 6;
            label3.Text = "Size:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(14, 105);
            label4.Name = "label4";
            label4.Size = new Size(69, 21);
            label4.TabIndex = 8;
            label4.Text = "Amount:";
            // 
            // nrAmount
            // 
            nrAmount.Location = new Point(91, 105);
            nrAmount.Name = "nrAmount";
            nrAmount.Size = new Size(174, 23);
            nrAmount.TabIndex = 9;
            // 
            // RegionResourcesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(280, 175);
            Controls.Add(nrAmount);
            Controls.Add(label4);
            Controls.Add(cmbSize);
            Controls.Add(label3);
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
            Text = "Resource Area Editor";
            ((System.ComponentModel.ISupportInitialize)nrAmount).EndInit();
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
        private ComboBox cmbSize;
        private Label label3;
        private Label label4;
        private NumericUpDown nrAmount;
    }
}