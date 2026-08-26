namespace X4SectorCreator.Forms.Galaxy.Clusters
{
    partial class SunForm
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
            label2 = new Label();
            TxtName = new TextBox();
            CmbClass = new ComboBox();
            BtnCreate = new Button();
            BtnCancel = new Button();
            label3 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(55, 21);
            label1.TabIndex = 0;
            label1.Text = "Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 59);
            label2.Name = "label2";
            label2.Size = new Size(49, 21);
            label2.TabIndex = 1;
            label2.Text = "Class:";
            // 
            // TxtName
            // 
            TxtName.Location = new Point(12, 33);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(283, 23);
            TxtName.TabIndex = 2;
            // 
            // CmbClass
            // 
            CmbClass.FormattingEnabled = true;
            CmbClass.Location = new Point(12, 83);
            CmbClass.Name = "CmbClass";
            CmbClass.Size = new Size(283, 23);
            CmbClass.TabIndex = 3;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(107, 112);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(188, 29);
            BtnCreate.TabIndex = 4;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 112);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(89, 29);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(61, 14);
            label3.Name = "label3";
            label3.Size = new Size(61, 15);
            label3.TabIndex = 6;
            label3.Text = "(Optional)";
            // 
            // SunForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(306, 146);
            Controls.Add(label3);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            Controls.Add(CmbClass);
            Controls.Add(TxtName);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "SunForm";
            Text = "Cluster Sun Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox TxtName;
        private ComboBox CmbClass;
        private Button BtnCreate;
        private Button BtnCancel;
        private Label label3;
    }
}