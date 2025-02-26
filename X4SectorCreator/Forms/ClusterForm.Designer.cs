namespace X4SectorCreator.Forms
{
    partial class ClusterForm
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
            BtnCreate = new Button();
            BtnCancel = new Button();
            TxtName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            BtnPick = new Button();
            TxtLocation = new TextBox();
            SuspendLayout();
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(83, 96);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(105, 30);
            BtnCreate.TabIndex = 0;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(194, 96);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(105, 30);
            BtnCancel.TabIndex = 1;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // TxtName
            // 
            TxtName.Location = new Point(83, 25);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(216, 23);
            TxtName.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(29, 25);
            label1.Name = "label1";
            label1.Size = new Size(55, 21);
            label1.TabIndex = 3;
            label1.Text = "Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 63);
            label2.Name = "label2";
            label2.Size = new Size(72, 21);
            label2.TabIndex = 4;
            label2.Text = "Location:";
            // 
            // BtnPick
            // 
            BtnPick.Location = new Point(194, 60);
            BtnPick.Name = "BtnPick";
            BtnPick.Size = new Size(105, 26);
            BtnPick.TabIndex = 5;
            BtnPick.Text = "Pick";
            BtnPick.UseVisualStyleBackColor = true;
            BtnPick.Click += BtnPick_Click;
            // 
            // TxtLocation
            // 
            TxtLocation.Enabled = false;
            TxtLocation.Location = new Point(83, 63);
            TxtLocation.Name = "TxtLocation";
            TxtLocation.Size = new Size(105, 23);
            TxtLocation.TabIndex = 6;
            // 
            // ClusterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(309, 135);
            Controls.Add(TxtLocation);
            Controls.Add(BtnPick);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(TxtName);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ClusterForm";
            Text = "X4 Sector Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button BtnCancel;
        private Label label1;
        private Label label2;
        private Button BtnPick;
        internal TextBox TxtLocation;
        internal Button BtnCreate;
        internal TextBox TxtName;
    }
}