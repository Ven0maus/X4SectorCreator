namespace X4SectorCreator.Forms
{
    partial class ShipGroupsForm
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
            BtnCreateGroup = new Button();
            label1 = new Label();
            TxtGroupName = new TextBox();
            ShipMacroListBox = new ListBox();
            label2 = new Label();
            BtnAddCustomMacro = new Button();
            BtnDeleteMacro = new Button();
            BtnCancel = new Button();
            ButtonSelectVanillaMacros = new Button();
            SuspendLayout();
            // 
            // BtnCreateGroup
            // 
            BtnCreateGroup.Location = new Point(132, 285);
            BtnCreateGroup.Name = "BtnCreateGroup";
            BtnCreateGroup.Size = new Size(354, 40);
            BtnCreateGroup.TabIndex = 0;
            BtnCreateGroup.Text = "Create";
            BtnCreateGroup.UseVisualStyleBackColor = true;
            BtnCreateGroup.Click += BtnCreateGroup_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(103, 21);
            label1.TabIndex = 1;
            label1.Text = "Group Name:";
            // 
            // TxtGroupName
            // 
            TxtGroupName.Location = new Point(115, 9);
            TxtGroupName.Name = "TxtGroupName";
            TxtGroupName.Size = new Size(371, 23);
            TxtGroupName.TabIndex = 2;
            // 
            // ShipMacroListBox
            // 
            ShipMacroListBox.FormattingEnabled = true;
            ShipMacroListBox.HorizontalScrollbar = true;
            ShipMacroListBox.Location = new Point(12, 65);
            ShipMacroListBox.Name = "ShipMacroListBox";
            ShipMacroListBox.Size = new Size(331, 214);
            ShipMacroListBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 41);
            label2.Name = "label2";
            label2.Size = new Size(96, 21);
            label2.TabIndex = 4;
            label2.Text = "Ship macros";
            // 
            // BtnAddCustomMacro
            // 
            BtnAddCustomMacro.Location = new Point(349, 65);
            BtnAddCustomMacro.Name = "BtnAddCustomMacro";
            BtnAddCustomMacro.Size = new Size(137, 34);
            BtnAddCustomMacro.TabIndex = 5;
            BtnAddCustomMacro.Text = "Add Custom Macro";
            BtnAddCustomMacro.UseVisualStyleBackColor = true;
            BtnAddCustomMacro.Click += BtnAddCustomMacro_Click;
            // 
            // BtnDeleteMacro
            // 
            BtnDeleteMacro.Location = new Point(349, 145);
            BtnDeleteMacro.Name = "BtnDeleteMacro";
            BtnDeleteMacro.Size = new Size(137, 34);
            BtnDeleteMacro.TabIndex = 6;
            BtnDeleteMacro.Text = "Delete Macro";
            BtnDeleteMacro.UseVisualStyleBackColor = true;
            BtnDeleteMacro.Click += BtnDeleteMacro_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 285);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(114, 40);
            BtnCancel.TabIndex = 7;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // ButtonSelectVanillaMacros
            // 
            ButtonSelectVanillaMacros.Location = new Point(349, 105);
            ButtonSelectVanillaMacros.Name = "ButtonSelectVanillaMacros";
            ButtonSelectVanillaMacros.Size = new Size(137, 34);
            ButtonSelectVanillaMacros.TabIndex = 8;
            ButtonSelectVanillaMacros.Text = "Select Vanilla Macros";
            ButtonSelectVanillaMacros.UseVisualStyleBackColor = true;
            ButtonSelectVanillaMacros.Click += ButtonSelectVanillaMacros_Click;
            // 
            // ShipGroupsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(490, 329);
            Controls.Add(ButtonSelectVanillaMacros);
            Controls.Add(BtnCancel);
            Controls.Add(BtnDeleteMacro);
            Controls.Add(BtnAddCustomMacro);
            Controls.Add(label2);
            Controls.Add(ShipMacroListBox);
            Controls.Add(TxtGroupName);
            Controls.Add(label1);
            Controls.Add(BtnCreateGroup);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ShipGroupsForm";
            Text = "Ship Group Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnCreateGroup;
        private Label label1;
        private TextBox TxtGroupName;
        private Label label2;
        private Button BtnAddCustomMacro;
        private Button BtnDeleteMacro;
        private Button BtnCancel;
        private Button ButtonSelectVanillaMacros;
        internal ListBox ShipMacroListBox;
    }
}