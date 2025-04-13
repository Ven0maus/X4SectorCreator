namespace X4SectorCreator.Forms
{
    partial class FactionForm
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
            TxtFactionName = new TextBox();
            BtnPickColor = new Button();
            label3 = new Label();
            TxtDescription = new TextBox();
            label2 = new Label();
            TxtPrefix = new TextBox();
            CmbRace = new ComboBox();
            label4 = new Label();
            label5 = new Label();
            CmbPoliceFaction = new ComboBox();
            BtnFactionRelations = new Button();
            BtnCreate = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11F);
            label1.Location = new Point(61, 13);
            label1.Name = "label1";
            label1.Size = new Size(52, 20);
            label1.TabIndex = 0;
            label1.Text = "Name:";
            // 
            // TxtFactionName
            // 
            TxtFactionName.Location = new Point(119, 14);
            TxtFactionName.Name = "TxtFactionName";
            TxtFactionName.Size = new Size(199, 23);
            TxtFactionName.TabIndex = 1;
            // 
            // BtnPickColor
            // 
            BtnPickColor.Location = new Point(119, 162);
            BtnPickColor.Name = "BtnPickColor";
            BtnPickColor.Size = new Size(199, 33);
            BtnPickColor.TabIndex = 4;
            BtnPickColor.Text = "Pick Faction Color";
            BtnPickColor.UseVisualStyleBackColor = true;
            BtnPickColor.Click += BtnPickColor_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 11F);
            label3.Location = new Point(25, 38);
            label3.Name = "label3";
            label3.Size = new Size(88, 20);
            label3.TabIndex = 6;
            label3.Text = "Description:";
            // 
            // TxtDescription
            // 
            TxtDescription.Location = new Point(119, 39);
            TxtDescription.Multiline = true;
            TxtDescription.Name = "TxtDescription";
            TxtDescription.Size = new Size(199, 88);
            TxtDescription.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11F);
            label2.Location = new Point(64, 133);
            label2.Name = "label2";
            label2.Size = new Size(49, 20);
            label2.TabIndex = 8;
            label2.Text = "Prefix:";
            // 
            // TxtPrefix
            // 
            TxtPrefix.Location = new Point(119, 133);
            TxtPrefix.Name = "TxtPrefix";
            TxtPrefix.Size = new Size(199, 23);
            TxtPrefix.TabIndex = 9;
            // 
            // CmbRace
            // 
            CmbRace.FormattingEnabled = true;
            CmbRace.Items.AddRange(new object[] { "argon", "paranid", "teladi", "terran", "split", "xenon" });
            CmbRace.Location = new Point(119, 201);
            CmbRace.Name = "CmbRace";
            CmbRace.Size = new Size(199, 23);
            CmbRace.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 11F);
            label4.Location = new Point(69, 201);
            label4.Name = "label4";
            label4.Size = new Size(44, 20);
            label4.TabIndex = 11;
            label4.Text = "Race:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11F);
            label5.Location = new Point(11, 230);
            label5.Name = "label5";
            label5.Size = new Size(102, 20);
            label5.TabIndex = 13;
            label5.Text = "Police Faction:";
            // 
            // CmbPoliceFaction
            // 
            CmbPoliceFaction.FormattingEnabled = true;
            CmbPoliceFaction.Items.AddRange(new object[] { "argon", "paranid", "teladi", "terran", "split", "xenon" });
            CmbPoliceFaction.Location = new Point(119, 230);
            CmbPoliceFaction.Name = "CmbPoliceFaction";
            CmbPoliceFaction.Size = new Size(199, 23);
            CmbPoliceFaction.TabIndex = 12;
            // 
            // BtnFactionRelations
            // 
            BtnFactionRelations.Location = new Point(119, 259);
            BtnFactionRelations.Name = "BtnFactionRelations";
            BtnFactionRelations.Size = new Size(199, 33);
            BtnFactionRelations.TabIndex = 14;
            BtnFactionRelations.Text = "Set Faction Relations";
            BtnFactionRelations.UseVisualStyleBackColor = true;
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(12, 298);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(306, 33);
            BtnCreate.TabIndex = 15;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // FactionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(336, 337);
            Controls.Add(BtnCreate);
            Controls.Add(BtnFactionRelations);
            Controls.Add(label5);
            Controls.Add(CmbPoliceFaction);
            Controls.Add(label4);
            Controls.Add(CmbRace);
            Controls.Add(TxtPrefix);
            Controls.Add(label2);
            Controls.Add(TxtDescription);
            Controls.Add(label3);
            Controls.Add(BtnPickColor);
            Controls.Add(TxtFactionName);
            Controls.Add(label1);
            Name = "FactionForm";
            Text = "Custom Faction Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox TxtFactionName;
        private Button BtnPickColor;
        private Label label3;
        private TextBox TxtDescription;
        private Label label2;
        private TextBox TxtPrefix;
        private ComboBox CmbRace;
        private Label label4;
        private Label label5;
        private ComboBox CmbPoliceFaction;
        private Button BtnFactionRelations;
        private Button BtnCreate;
    }
}