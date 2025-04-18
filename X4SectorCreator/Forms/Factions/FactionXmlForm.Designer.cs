namespace X4SectorCreator.Forms
{
    partial class FactionXmlForm
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
            TxtFactionXml = new TextBox();
            label1 = new Label();
            BtnCancel = new Button();
            BtnUpdate = new Button();
            SuspendLayout();
            // 
            // TxtFactionXml
            // 
            TxtFactionXml.Location = new Point(12, 33);
            TxtFactionXml.Multiline = true;
            TxtFactionXml.Name = "TxtFactionXml";
            TxtFactionXml.ScrollBars = ScrollBars.Both;
            TxtFactionXml.Size = new Size(728, 574);
            TxtFactionXml.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(90, 21);
            label1.TabIndex = 1;
            label1.Text = "Faction Xml";
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 613);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(198, 34);
            BtnCancel.TabIndex = 2;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnUpdate
            // 
            BtnUpdate.Location = new Point(216, 613);
            BtnUpdate.Name = "BtnUpdate";
            BtnUpdate.Size = new Size(524, 34);
            BtnUpdate.TabIndex = 3;
            BtnUpdate.Text = "Update";
            BtnUpdate.UseVisualStyleBackColor = true;
            BtnUpdate.Click += BtnUpdate_Click;
            // 
            // FactionXmlForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(747, 652);
            Controls.Add(BtnUpdate);
            Controls.Add(BtnCancel);
            Controls.Add(label1);
            Controls.Add(TxtFactionXml);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactionXmlForm";
            Text = "Faction Xml Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal TextBox TxtFactionXml;
        private Label label1;
        private Button BtnCancel;
        private Button BtnUpdate;
    }
}