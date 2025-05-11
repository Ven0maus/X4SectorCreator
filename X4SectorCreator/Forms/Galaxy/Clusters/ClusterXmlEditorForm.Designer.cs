namespace X4SectorCreator.Forms.Galaxy.Clusters
{
    partial class ClusterXmlEditorForm
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
            BtnCancel = new Button();
            BtnCreate = new Button();
            label2 = new Label();
            TxtClusterXml = new TextBox();
            SuspendLayout();
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(7, 659);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(235, 38);
            BtnCancel.TabIndex = 11;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(248, 659);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(506, 38);
            BtnCreate.TabIndex = 10;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 15F);
            label2.Location = new Point(7, 6);
            label2.Name = "label2";
            label2.Size = new Size(116, 28);
            label2.TabIndex = 9;
            label2.Text = "Cluster XML";
            // 
            // TxtClusterXml
            // 
            TxtClusterXml.Font = new Font("Segoe UI", 10F);
            TxtClusterXml.Location = new Point(7, 37);
            TxtClusterXml.Multiline = true;
            TxtClusterXml.Name = "TxtClusterXml";
            TxtClusterXml.Size = new Size(747, 619);
            TxtClusterXml.TabIndex = 8;
            // 
            // ClusterXmlEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(759, 700);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            Controls.Add(label2);
            Controls.Add(TxtClusterXml);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ClusterXmlEditor";
            Text = "Cluster XML Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnCancel;
        private Button BtnCreate;
        private Label label2;
        internal TextBox TxtClusterXml;
    }
}