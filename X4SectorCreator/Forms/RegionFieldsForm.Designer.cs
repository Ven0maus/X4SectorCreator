namespace X4SectorCreator.Forms
{
    partial class RegionFieldsForm
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
            cmbFieldType = new ComboBox();
            label1 = new Label();
            BtnAdd = new Button();
            BtnCancel = new Button();
            SuspendLayout();
            // 
            // cmbFieldType
            // 
            cmbFieldType.FormattingEnabled = true;
            cmbFieldType.Items.AddRange(new object[] { "asteroid", "nebula", "volumetricfog", "positional", "gravidar", "object", "debris" });
            cmbFieldType.Location = new Point(100, 9);
            cmbFieldType.Name = "cmbFieldType";
            cmbFieldType.Size = new Size(191, 23);
            cmbFieldType.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(82, 21);
            label1.TabIndex = 1;
            label1.Text = "Field Type:";
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(492, 443);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(158, 40);
            BtnAdd.TabIndex = 2;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(397, 443);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(89, 40);
            BtnCancel.TabIndex = 3;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // RegionFieldsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(662, 495);
            Controls.Add(BtnCancel);
            Controls.Add(BtnAdd);
            Controls.Add(label1);
            Controls.Add(cmbFieldType);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionFieldsForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbFieldType;
        private Label label1;
        private Button BtnAdd;
        private Button BtnCancel;
    }
}