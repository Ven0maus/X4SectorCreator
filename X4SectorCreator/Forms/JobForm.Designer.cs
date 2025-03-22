namespace X4SectorCreator.Forms
{
    partial class JobForm
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
            label2 = new Label();
            TxtJobXml = new TextBox();
            BtnCreate = new Button();
            BtnCancel = new Button();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 15F);
            label2.Location = new Point(12, 14);
            label2.Name = "label2";
            label2.Size = new Size(87, 28);
            label2.TabIndex = 5;
            label2.Text = "Job XML";
            // 
            // TxtJobXml
            // 
            TxtJobXml.Location = new Point(12, 45);
            TxtJobXml.Multiline = true;
            TxtJobXml.Name = "TxtJobXml";
            TxtJobXml.Size = new Size(747, 619);
            TxtJobXml.TabIndex = 4;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(329, 9);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(430, 30);
            BtnCreate.TabIndex = 6;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(155, 9);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(168, 30);
            BtnCancel.TabIndex = 7;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // JobForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(764, 670);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            Controls.Add(label2);
            Controls.Add(TxtJobXml);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "JobForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private TextBox TxtJobXml;
        private Button BtnCreate;
        private Button BtnCancel;
    }
}