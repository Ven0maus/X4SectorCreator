namespace X4SectorCreator.Forms
{
    partial class FactionRelationsForm
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
            BtnUpdate = new Button();
            BtnCancel = new Button();
            SuspendLayout();
            // 
            // BtnUpdate
            // 
            BtnUpdate.Location = new Point(464, 437);
            BtnUpdate.Name = "BtnUpdate";
            BtnUpdate.Size = new Size(114, 32);
            BtnUpdate.TabIndex = 0;
            BtnUpdate.Text = "Update";
            BtnUpdate.UseVisualStyleBackColor = true;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(344, 437);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(114, 32);
            BtnCancel.TabIndex = 1;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            // 
            // FactionRelationsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(590, 481);
            Controls.Add(BtnCancel);
            Controls.Add(BtnUpdate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactionRelationsForm";
            Text = "Faction Relations Editor";
            ResumeLayout(false);
        }

        #endregion

        private Button BtnUpdate;
        private Button BtnCancel;
    }
}