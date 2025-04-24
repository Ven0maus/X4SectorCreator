namespace X4SectorCreator.Forms.General
{
    partial class TemplateGroupsForm
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
            TemplateGroupsListBox = new ListBox();
            label1 = new Label();
            label2 = new Label();
            TemplatesInGroupListBox = new ListBox();
            BtnCreateNewGroup = new Button();
            BtnDeleteGroup = new Button();
            BtnAddTemplate = new Button();
            BtnDeleteTemplate = new Button();
            SuspendLayout();
            // 
            // TemplateGroupsListBox
            // 
            TemplateGroupsListBox.FormattingEnabled = true;
            TemplateGroupsListBox.Location = new Point(6, 33);
            TemplateGroupsListBox.Name = "TemplateGroupsListBox";
            TemplateGroupsListBox.Size = new Size(236, 454);
            TemplateGroupsListBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(6, 9);
            label1.Name = "label1";
            label1.Size = new Size(130, 21);
            label1.TabIndex = 1;
            label1.Text = "Template Groups:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(248, 9);
            label2.Name = "label2";
            label2.Size = new Size(147, 21);
            label2.TabIndex = 3;
            label2.Text = "Templates In Group:";
            // 
            // TemplatesInGroupListBox
            // 
            TemplatesInGroupListBox.FormattingEnabled = true;
            TemplatesInGroupListBox.Location = new Point(248, 33);
            TemplatesInGroupListBox.Name = "TemplatesInGroupListBox";
            TemplatesInGroupListBox.Size = new Size(457, 454);
            TemplatesInGroupListBox.TabIndex = 2;
            // 
            // BtnCreateNewGroup
            // 
            BtnCreateNewGroup.Location = new Point(6, 493);
            BtnCreateNewGroup.Name = "BtnCreateNewGroup";
            BtnCreateNewGroup.Size = new Size(127, 36);
            BtnCreateNewGroup.TabIndex = 4;
            BtnCreateNewGroup.Text = "Create New Group";
            BtnCreateNewGroup.UseVisualStyleBackColor = true;
            BtnCreateNewGroup.Click += BtnCreateNewGroup_Click;
            // 
            // BtnDeleteGroup
            // 
            BtnDeleteGroup.Location = new Point(139, 493);
            BtnDeleteGroup.Name = "BtnDeleteGroup";
            BtnDeleteGroup.Size = new Size(103, 36);
            BtnDeleteGroup.TabIndex = 5;
            BtnDeleteGroup.Text = "Delete";
            BtnDeleteGroup.UseVisualStyleBackColor = true;
            BtnDeleteGroup.Click += BtnDeleteGroup_Click;
            // 
            // BtnAddTemplate
            // 
            BtnAddTemplate.Location = new Point(248, 493);
            BtnAddTemplate.Name = "BtnAddTemplate";
            BtnAddTemplate.Size = new Size(325, 36);
            BtnAddTemplate.TabIndex = 6;
            BtnAddTemplate.Text = "Add new template";
            BtnAddTemplate.UseVisualStyleBackColor = true;
            BtnAddTemplate.Click += BtnAddTemplate_Click;
            // 
            // BtnDeleteTemplate
            // 
            BtnDeleteTemplate.Location = new Point(579, 493);
            BtnDeleteTemplate.Name = "BtnDeleteTemplate";
            BtnDeleteTemplate.Size = new Size(126, 36);
            BtnDeleteTemplate.TabIndex = 7;
            BtnDeleteTemplate.Text = "Delete";
            BtnDeleteTemplate.UseVisualStyleBackColor = true;
            BtnDeleteTemplate.Click += BtnDeleteTemplate_Click;
            // 
            // TemplateGroupsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(710, 534);
            Controls.Add(BtnDeleteTemplate);
            Controls.Add(BtnAddTemplate);
            Controls.Add(BtnDeleteGroup);
            Controls.Add(BtnCreateNewGroup);
            Controls.Add(label2);
            Controls.Add(TemplatesInGroupListBox);
            Controls.Add(label1);
            Controls.Add(TemplateGroupsListBox);
            Name = "TemplateGroupsForm";
            Text = "Template Groups Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox TemplateGroupsListBox;
        private Label label1;
        private Label label2;
        private ListBox TemplatesInGroupListBox;
        private Button BtnCreateNewGroup;
        private Button BtnDeleteGroup;
        private Button BtnAddTemplate;
        private Button BtnDeleteTemplate;
    }
}