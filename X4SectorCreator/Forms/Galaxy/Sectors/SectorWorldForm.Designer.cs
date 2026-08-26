namespace X4SectorCreator.Forms.Galaxy.Sectors
{
    partial class SectorWorldForm
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
            CmbPartSelection = new ComboBox();
            label1 = new Label();
            NrFactor = new NumericUpDown();
            label2 = new Label();
            BtnCancel = new Button();
            BtnCreate = new Button();
            ((System.ComponentModel.ISupportInitialize)NrFactor).BeginInit();
            SuspendLayout();
            // 
            // CmbPartSelection
            // 
            CmbPartSelection.FormattingEnabled = true;
            CmbPartSelection.Location = new Point(125, 16);
            CmbPartSelection.Name = "CmbPartSelection";
            CmbPartSelection.Size = new Size(243, 23);
            CmbPartSelection.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 14);
            label1.Name = "label1";
            label1.Size = new Size(107, 21);
            label1.TabIndex = 1;
            label1.Text = "Part Selection:";
            // 
            // NrFactor
            // 
            NrFactor.DecimalPlaces = 2;
            NrFactor.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            NrFactor.Location = new Point(125, 46);
            NrFactor.Name = "NrFactor";
            NrFactor.Size = new Size(65, 23);
            NrFactor.TabIndex = 2;
            NrFactor.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(64, 45);
            label2.Name = "label2";
            label2.Size = new Size(55, 21);
            label2.TabIndex = 3;
            label2.Text = "Factor:";
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 77);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(107, 36);
            BtnCancel.TabIndex = 4;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(125, 77);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(243, 36);
            BtnCreate.TabIndex = 5;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // SectorWorldForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(380, 124);
            Controls.Add(BtnCreate);
            Controls.Add(BtnCancel);
            Controls.Add(label2);
            Controls.Add(NrFactor);
            Controls.Add(label1);
            Controls.Add(CmbPartSelection);
            Name = "SectorWorldForm";
            Text = "Sector World Editor";
            ((System.ComponentModel.ISupportInitialize)NrFactor).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox CmbPartSelection;
        private Label label1;
        private NumericUpDown NrFactor;
        private Label label2;
        private Button BtnCancel;
        private Button BtnCreate;
    }
}