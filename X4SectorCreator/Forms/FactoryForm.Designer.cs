namespace X4SectorCreator.Forms
{
    partial class FactoryForm
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
            label2 = new Label();
            TxtFactoryXml = new TextBox();
            BtnCreate = new Button();
            BtnCancel = new Button();
            BtnSelectFactoryLocation = new Button();
            BtnSelectFaction = new Button();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 15F);
            label2.Location = new Point(8, 7);
            label2.Name = "label2";
            label2.Size = new Size(120, 28);
            label2.TabIndex = 5;
            label2.Text = "Factory XML";
            // 
            // TxtFactoryXml
            // 
            TxtFactoryXml.Font = new Font("Segoe UI", 10F);
            TxtFactoryXml.Location = new Point(8, 38);
            TxtFactoryXml.Multiline = true;
            TxtFactoryXml.Name = "TxtFactoryXml";
            TxtFactoryXml.Size = new Size(747, 619);
            TxtFactoryXml.TabIndex = 4;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(249, 660);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(506, 38);
            BtnCreate.TabIndex = 6;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(8, 660);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(235, 38);
            BtnCancel.TabIndex = 7;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnSelectFactoryLocation
            // 
            BtnSelectFactoryLocation.Location = new Point(602, 7);
            BtnSelectFactoryLocation.Name = "BtnSelectFactoryLocation";
            BtnSelectFactoryLocation.Size = new Size(153, 28);
            BtnSelectFactoryLocation.TabIndex = 8;
            BtnSelectFactoryLocation.Text = "Select Factory Location";
            BtnSelectFactoryLocation.UseVisualStyleBackColor = true;
            BtnSelectFactoryLocation.Click += BtnSelectFactoryLocation_Click;
            // 
            // BtnSelectFaction
            // 
            BtnSelectFaction.Location = new Point(443, 7);
            BtnSelectFaction.Name = "BtnSelectFaction";
            BtnSelectFaction.Size = new Size(153, 28);
            BtnSelectFaction.TabIndex = 10;
            BtnSelectFaction.Text = "Select Factory Faction";
            BtnSelectFaction.UseVisualStyleBackColor = true;
            BtnSelectFaction.Click += BtnSelectFaction_Click;
            // 
            // FactoryForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(761, 701);
            Controls.Add(BtnSelectFaction);
            Controls.Add(BtnSelectFactoryLocation);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            Controls.Add(label2);
            Controls.Add(TxtFactoryXml);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactoryForm";
            Text = "Factory Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private Button BtnCreate;
        private Button BtnCancel;
        private Button BtnSelectFactoryLocation;
        private Button BtnSelectFaction;
        internal TextBox TxtFactoryXml;
    }
}