namespace X4SectorCreator.Forms.Factions
{
    partial class FactionCreationHelpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FactionCreationHelpForm));
            label1 = new Label();
            BtnClose = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 11F);
            label1.Location = new Point(7, 9);
            label1.Name = "label1";
            label1.Size = new Size(645, 565);
            label1.TabIndex = 0;
            label1.Text = resources.GetString("label1.Text");
            // 
            // BtnClose
            // 
            BtnClose.Location = new Point(7, 577);
            BtnClose.Name = "BtnClose";
            BtnClose.Size = new Size(645, 33);
            BtnClose.TabIndex = 1;
            BtnClose.Text = "Close";
            BtnClose.UseVisualStyleBackColor = true;
            BtnClose.Click += BtnClose_Click;
            // 
            // FactionCreationHelpForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(656, 614);
            Controls.Add(BtnClose);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactionCreationHelpForm";
            Text = "Faction Creation: How does it work?";
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Button BtnClose;
    }
}