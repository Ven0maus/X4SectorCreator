using X4SectorCreator.CustomComponents;

namespace X4SectorCreator.Forms
{
    partial class BasketForm
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
            TxtName = new TextBox();
            label1 = new Label();
            CmbWares = new MultiSelectCombo.NoDropDownComboBox();
            label2 = new Label();
            BtnCreate = new Button();
            BtnCancel = new Button();
            SuspendLayout();
            // 
            // TxtName
            // 
            TxtName.Location = new Point(12, 32);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(263, 23);
            TxtName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(96, 20);
            label1.TabIndex = 1;
            label1.Text = "Basket Name";
            // 
            // CmbWares
            // 
            CmbWares.FormattingEnabled = true;
            CmbWares.Location = new Point(12, 81);
            CmbWares.Name = "CmbWares";
            CmbWares.Size = new Size(263, 23);
            CmbWares.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11F);
            label2.Location = new Point(12, 58);
            label2.Name = "label2";
            label2.Size = new Size(49, 20);
            label2.TabIndex = 3;
            label2.Text = "Wares";
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(114, 113);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(161, 32);
            BtnCreate.TabIndex = 4;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 113);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(96, 32);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BasketForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(285, 152);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            Controls.Add(label2);
            Controls.Add(CmbWares);
            Controls.Add(label1);
            Controls.Add(TxtName);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BasketForm";
            Text = "Basket Editor";
            Load += BasketForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TxtName;
        private Label label1;
        private MultiSelectCombo.NoDropDownComboBox CmbWares;
        private Label label2;
        private Button BtnCreate;
        private Button BtnCancel;
    }
}