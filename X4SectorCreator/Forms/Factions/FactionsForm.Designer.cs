namespace X4SectorCreator.Forms
{
    partial class FactionsForm
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
            CustomFactionsListBox = new ListBox();
            BtnCreate = new Button();
            BtnDelete = new Button();
            BtnExit = new Button();
            BtnFactionCreationHelp = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(124, 21);
            label1.TabIndex = 0;
            label1.Text = "Custom Factions";
            // 
            // CustomFactionsListBox
            // 
            CustomFactionsListBox.FormattingEnabled = true;
            CustomFactionsListBox.HorizontalScrollbar = true;
            CustomFactionsListBox.Location = new Point(12, 33);
            CustomFactionsListBox.Name = "CustomFactionsListBox";
            CustomFactionsListBox.Size = new Size(228, 244);
            CustomFactionsListBox.TabIndex = 1;
            CustomFactionsListBox.DoubleClick += CustomFactionsListBox_DoubleClick;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(246, 33);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(169, 33);
            BtnCreate.TabIndex = 2;
            BtnCreate.Text = "Create New Faction";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnDelete
            // 
            BtnDelete.Location = new Point(246, 72);
            BtnDelete.Name = "BtnDelete";
            BtnDelete.Size = new Size(169, 33);
            BtnDelete.TabIndex = 3;
            BtnDelete.Text = "Delete Faction";
            BtnDelete.UseVisualStyleBackColor = true;
            BtnDelete.Click += BtnDelete_Click;
            // 
            // BtnExit
            // 
            BtnExit.Location = new Point(246, 244);
            BtnExit.Name = "BtnExit";
            BtnExit.Size = new Size(169, 33);
            BtnExit.TabIndex = 4;
            BtnExit.Text = "Exit";
            BtnExit.UseVisualStyleBackColor = true;
            BtnExit.Click += BtnExit_Click;
            // 
            // BtnFactionCreationHelp
            // 
            BtnFactionCreationHelp.Location = new Point(246, 111);
            BtnFactionCreationHelp.Name = "BtnFactionCreationHelp";
            BtnFactionCreationHelp.Size = new Size(169, 33);
            BtnFactionCreationHelp.TabIndex = 5;
            BtnFactionCreationHelp.Text = "How does it work?";
            BtnFactionCreationHelp.UseVisualStyleBackColor = true;
            BtnFactionCreationHelp.Click += BtnFactionCreationHelp_Click;
            // 
            // FactionsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(420, 282);
            Controls.Add(BtnFactionCreationHelp);
            Controls.Add(BtnExit);
            Controls.Add(BtnDelete);
            Controls.Add(BtnCreate);
            Controls.Add(CustomFactionsListBox);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FactionsForm";
            Text = "Custom Factions";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ListBox CustomFactionsListBox;
        private Button BtnCreate;
        private Button BtnDelete;
        private Button BtnExit;
        private Button BtnFactionCreationHelp;
    }
}