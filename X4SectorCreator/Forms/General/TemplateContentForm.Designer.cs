namespace X4SectorCreator.Forms.General
{
    partial class TemplateContentForm
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
            TxtXml = new TextBox();
            label1 = new Label();
            BtnConfirm = new Button();
            BtnCancel = new Button();
            SuspendLayout();
            // 
            // TxtXml
            // 
            TxtXml.Location = new Point(6, 31);
            TxtXml.Multiline = true;
            TxtXml.Name = "TxtXml";
            TxtXml.ScrollBars = ScrollBars.Vertical;
            TxtXml.Size = new Size(667, 523);
            TxtXml.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(6, 7);
            label1.Name = "label1";
            label1.Size = new Size(107, 21);
            label1.TabIndex = 1;
            label1.Text = "Template XML";
            // 
            // BtnConfirm
            // 
            BtnConfirm.Location = new Point(224, 558);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(449, 35);
            BtnConfirm.TabIndex = 2;
            BtnConfirm.Text = "Confirm Changes";
            BtnConfirm.UseVisualStyleBackColor = true;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(6, 558);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(212, 35);
            BtnCancel.TabIndex = 3;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // TemplateContentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(679, 597);
            Controls.Add(BtnCancel);
            Controls.Add(BtnConfirm);
            Controls.Add(label1);
            Controls.Add(TxtXml);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TemplateContentForm";
            Text = "Template XML Editor";
            Load += LoadContent;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TxtXml;
        private Label label1;
        private Button BtnConfirm;
        private Button BtnCancel;
    }
}