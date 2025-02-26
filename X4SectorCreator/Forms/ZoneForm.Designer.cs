namespace X4SectorCreator.Forms
{
    partial class ZoneForm
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
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(8, 12);
            label1.Name = "label1";
            label1.Size = new Size(55, 21);
            label1.TabIndex = 14;
            label1.Text = "Name:";
            // 
            // TxtName
            // 
            TxtName.Location = new Point(62, 12);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(216, 23);
            TxtName.TabIndex = 13;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(173, 41);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(105, 30);
            BtnCancel.TabIndex = 12;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(62, 41);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(105, 30);
            BtnCreate.TabIndex = 11;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // ZoneForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(294, 82);
            Controls.Add(label1);
            Controls.Add(TxtName);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ZoneForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        internal TextBox TxtName;
        private Button BtnCancel;
        internal Button BtnCreate;
    }
}