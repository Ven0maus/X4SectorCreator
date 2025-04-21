namespace X4SectorCreator.Forms
{
    partial class VersionUpdateForm
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
            BtnNavigate = new Button();
            BtnOk = new Button();
            txtCurrentVersion = new TextBox();
            txtCurrentX4Version = new TextBox();
            label1 = new Label();
            label2 = new Label();
            panel1 = new Panel();
            label3 = new Label();
            label4 = new Label();
            txtUpdateX4Version = new TextBox();
            txtUpdateVersion = new TextBox();
            label5 = new Label();
            label6 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // BtnNavigate
            // 
            BtnNavigate.Location = new Point(12, 178);
            BtnNavigate.Name = "BtnNavigate";
            BtnNavigate.Size = new Size(209, 29);
            BtnNavigate.TabIndex = 0;
            BtnNavigate.Text = "Navigate to release and exit";
            BtnNavigate.UseVisualStyleBackColor = true;
            BtnNavigate.Click += BtnNavigate_Click;
            // 
            // BtnOk
            // 
            BtnOk.Location = new Point(227, 178);
            BtnOk.Name = "BtnOk";
            BtnOk.Size = new Size(107, 29);
            BtnOk.TabIndex = 2;
            BtnOk.Text = "OK";
            BtnOk.UseVisualStyleBackColor = true;
            BtnOk.Click += BtnOk_Click;
            // 
            // txtCurrentVersion
            // 
            txtCurrentVersion.Location = new Point(182, 16);
            txtCurrentVersion.Name = "txtCurrentVersion";
            txtCurrentVersion.ReadOnly = true;
            txtCurrentVersion.Size = new Size(152, 23);
            txtCurrentVersion.TabIndex = 3;
            // 
            // txtCurrentX4Version
            // 
            txtCurrentX4Version.Location = new Point(182, 47);
            txtCurrentX4Version.Name = "txtCurrentX4Version";
            txtCurrentX4Version.ReadOnly = true;
            txtCurrentX4Version.Size = new Size(152, 23);
            txtCurrentX4Version.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 14);
            label1.Name = "label1";
            label1.Size = new Size(127, 21);
            label1.TabIndex = 5;
            label1.Text = "Installed Version:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 45);
            label2.Name = "label2";
            label2.Size = new Size(164, 21);
            label2.TabIndex = 6;
            label2.Text = "Supported X4 Version:";
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(txtUpdateX4Version);
            panel1.Controls.Add(txtUpdateVersion);
            panel1.Controls.Add(label5);
            panel1.Location = new Point(12, 78);
            panel1.Name = "panel1";
            panel1.Size = new Size(322, 94);
            panel1.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(0, 63);
            label3.Name = "label3";
            label3.Size = new Size(164, 21);
            label3.TabIndex = 16;
            label3.Text = "Supported X4 Version:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(0, 32);
            label4.Name = "label4";
            label4.Size = new Size(132, 21);
            label4.TabIndex = 15;
            label4.Text = "Available Version:";
            // 
            // txtUpdateX4Version
            // 
            txtUpdateX4Version.Location = new Point(170, 65);
            txtUpdateX4Version.Name = "txtUpdateX4Version";
            txtUpdateX4Version.ReadOnly = true;
            txtUpdateX4Version.Size = new Size(147, 23);
            txtUpdateX4Version.TabIndex = 14;
            // 
            // txtUpdateVersion
            // 
            txtUpdateVersion.Location = new Point(170, 34);
            txtUpdateVersion.Name = "txtUpdateVersion";
            txtUpdateVersion.ReadOnly = true;
            txtUpdateVersion.Size = new Size(147, 23);
            txtUpdateVersion.TabIndex = 13;
            // 
            // label5
            // 
            label5.BackColor = SystemColors.GradientActiveCaption;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(0, 0);
            label5.Name = "label5";
            label5.Size = new Size(322, 25);
            label5.TabIndex = 12;
            label5.Text = "AVAILABLE UPDATE";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            label6.Location = new Point(14, 210);
            label6.Name = "label6";
            label6.Size = new Size(321, 36);
            label6.TabIndex = 8;
            label6.Text = "It is highly advised to update the tool when possible to benefit from the latest bugfixes and features!";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // VersionUpdateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(346, 250);
            Controls.Add(label6);
            Controls.Add(panel1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtCurrentX4Version);
            Controls.Add(txtCurrentVersion);
            Controls.Add(BtnOk);
            Controls.Add(BtnNavigate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "VersionUpdateForm";
            Text = "New Version Available!";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnNavigate;
        private Button BtnOk;
        private Label label1;
        private Label label2;
        private Panel panel1;
        private Label label3;
        private Label label4;
        private Label label5;
        internal TextBox txtCurrentVersion;
        internal TextBox txtCurrentX4Version;
        internal TextBox txtUpdateX4Version;
        internal TextBox txtUpdateVersion;
        private Label label6;
    }
}