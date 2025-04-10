namespace X4SectorCreator.Forms
{
    partial class RegionFalloffForm
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
            txtPosition = new TextBox();
            txtValue = new TextBox();
            label2 = new Label();
            BtnAdd = new Button();
            BtnCancel = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(17, 20);
            label1.Name = "label1";
            label1.Size = new Size(68, 21);
            label1.TabIndex = 0;
            label1.Text = "Position:";
            // 
            // txtPosition
            // 
            txtPosition.Location = new Point(91, 20);
            txtPosition.Name = "txtPosition";
            txtPosition.Size = new Size(144, 23);
            txtPosition.TabIndex = 1;
            // 
            // txtValue
            // 
            txtValue.Location = new Point(91, 49);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(144, 23);
            txtValue.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(34, 49);
            label2.Name = "label2";
            label2.Size = new Size(51, 21);
            label2.TabIndex = 2;
            label2.Text = "Value:";
            // 
            // BtnAdd
            // 
            BtnAdd.Location = new Point(91, 78);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(144, 32);
            BtnAdd.TabIndex = 4;
            BtnAdd.Text = "Add";
            BtnAdd.UseVisualStyleBackColor = true;
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(17, 78);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(68, 32);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // RegionFalloffForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(244, 122);
            Controls.Add(BtnCancel);
            Controls.Add(BtnAdd);
            Controls.Add(txtValue);
            Controls.Add(label2);
            Controls.Add(txtPosition);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RegionFalloffForm";
            Text = "Region Falloff Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtPosition;
        private TextBox txtValue;
        private Label label2;
        private Button BtnAdd;
        private Button BtnCancel;
    }
}