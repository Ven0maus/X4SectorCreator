namespace X4SectorCreator.Forms.Galaxy.Regions
{
    partial class RegionDefinitionsForm
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
            BtnRemoveDefinition = new Button();
            BtnNewDefinition = new Button();
            ListBoxRegionDefinitions = new ListBox();
            label1 = new Label();
            BtnExit = new Button();
            SuspendLayout();
            // 
            // BtnRemoveDefinition
            // 
            BtnRemoveDefinition.Location = new Point(213, 298);
            BtnRemoveDefinition.Name = "BtnRemoveDefinition";
            BtnRemoveDefinition.Size = new Size(94, 36);
            BtnRemoveDefinition.TabIndex = 98;
            BtnRemoveDefinition.Text = "Remove";
            BtnRemoveDefinition.UseVisualStyleBackColor = true;
            BtnRemoveDefinition.Click += BtnRemoveDefinition_Click;
            // 
            // BtnNewDefinition
            // 
            BtnNewDefinition.Location = new Point(8, 298);
            BtnNewDefinition.Name = "BtnNewDefinition";
            BtnNewDefinition.Size = new Size(199, 36);
            BtnNewDefinition.TabIndex = 97;
            BtnNewDefinition.Text = "New definition";
            BtnNewDefinition.UseVisualStyleBackColor = true;
            BtnNewDefinition.Click += BtnNewDefinition_Click;
            // 
            // ListBoxRegionDefinitions
            // 
            ListBoxRegionDefinitions.FormattingEnabled = true;
            ListBoxRegionDefinitions.HorizontalScrollbar = true;
            ListBoxRegionDefinitions.Location = new Point(8, 33);
            ListBoxRegionDefinitions.Name = "ListBoxRegionDefinitions";
            ListBoxRegionDefinitions.Size = new Size(300, 259);
            ListBoxRegionDefinitions.TabIndex = 96;
            ListBoxRegionDefinitions.DoubleClick += ListBoxRegionDefinitions_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(8, 9);
            label1.Name = "label1";
            label1.Size = new Size(141, 21);
            label1.TabIndex = 99;
            label1.Text = "Region Definitions:";
            // 
            // BtnExit
            // 
            BtnExit.Location = new Point(8, 340);
            BtnExit.Name = "BtnExit";
            BtnExit.Size = new Size(299, 36);
            BtnExit.TabIndex = 100;
            BtnExit.Text = "Exit";
            BtnExit.UseVisualStyleBackColor = true;
            BtnExit.Click += BtnExit_Click;
            // 
            // RegionDefinitionsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(316, 380);
            Controls.Add(BtnExit);
            Controls.Add(label1);
            Controls.Add(BtnRemoveDefinition);
            Controls.Add(BtnNewDefinition);
            Controls.Add(ListBoxRegionDefinitions);
            Name = "RegionDefinitionsForm";
            Text = "Region Definitions";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnRemoveDefinition;
        private Button BtnNewDefinition;
        internal ListBox ListBoxRegionDefinitions;
        private Label label1;
        private Button BtnExit;
    }
}