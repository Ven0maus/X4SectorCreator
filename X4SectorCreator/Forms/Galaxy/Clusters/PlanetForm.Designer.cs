namespace X4SectorCreator.Forms.Galaxy.Clusters
{
    partial class PlanetForm
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
            label1 = new Label();
            TxtName = new TextBox();
            label2 = new Label();
            CmbClass = new ComboBox();
            CmbGeology = new ComboBox();
            label3 = new Label();
            CmbAtmosphere = new ComboBox();
            label4 = new Label();
            CmbPopulation = new ComboBox();
            label5 = new Label();
            CmbSettlements = new ComboBox();
            label6 = new Label();
            label7 = new Label();
            NrMaxPopulation = new NumericUpDown();
            CmbPart = new ComboBox();
            label8 = new Label();
            CmbAtmopart = new ComboBox();
            label9 = new Label();
            ((System.ComponentModel.ISupportInitialize)NrMaxPopulation).BeginInit();
            SuspendLayout();
            // 
            // BtnCreate
            // 
            BtnCreate.Location = new Point(98, 471);
            BtnCreate.Name = "BtnCreate";
            BtnCreate.Size = new Size(214, 32);
            BtnCreate.TabIndex = 0;
            BtnCreate.Text = "Create";
            BtnCreate.UseVisualStyleBackColor = true;
            BtnCreate.Click += BtnCreate_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(12, 471);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(80, 32);
            BtnCancel.TabIndex = 1;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(55, 21);
            label1.TabIndex = 2;
            label1.Text = "Name:";
            // 
            // TxtName
            // 
            TxtName.Location = new Point(12, 33);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(300, 23);
            TxtName.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 59);
            label2.Name = "label2";
            label2.Size = new Size(49, 21);
            label2.TabIndex = 4;
            label2.Text = "Class:";
            // 
            // CmbClass
            // 
            CmbClass.FormattingEnabled = true;
            CmbClass.Location = new Point(12, 83);
            CmbClass.Name = "CmbClass";
            CmbClass.Size = new Size(300, 23);
            CmbClass.TabIndex = 5;
            // 
            // CmbGeology
            // 
            CmbGeology.FormattingEnabled = true;
            CmbGeology.Location = new Point(12, 136);
            CmbGeology.Name = "CmbGeology";
            CmbGeology.Size = new Size(300, 23);
            CmbGeology.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(12, 112);
            label3.Name = "label3";
            label3.Size = new Size(71, 21);
            label3.TabIndex = 6;
            label3.Text = "Geology:";
            // 
            // CmbAtmosphere
            // 
            CmbAtmosphere.FormattingEnabled = true;
            CmbAtmosphere.Location = new Point(12, 189);
            CmbAtmosphere.Name = "CmbAtmosphere";
            CmbAtmosphere.Size = new Size(300, 23);
            CmbAtmosphere.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(12, 165);
            label4.Name = "label4";
            label4.Size = new Size(98, 21);
            label4.TabIndex = 8;
            label4.Text = "Atmosphere:";
            // 
            // CmbPopulation
            // 
            CmbPopulation.FormattingEnabled = true;
            CmbPopulation.Location = new Point(12, 240);
            CmbPopulation.Name = "CmbPopulation";
            CmbPopulation.Size = new Size(300, 23);
            CmbPopulation.TabIndex = 11;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(12, 216);
            label5.Name = "label5";
            label5.Size = new Size(87, 21);
            label5.TabIndex = 10;
            label5.Text = "Population:";
            // 
            // CmbSettlements
            // 
            CmbSettlements.FormattingEnabled = true;
            CmbSettlements.Location = new Point(12, 291);
            CmbSettlements.Name = "CmbSettlements";
            CmbSettlements.Size = new Size(300, 23);
            CmbSettlements.TabIndex = 13;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(12, 267);
            label6.Name = "label6";
            label6.Size = new Size(95, 21);
            label6.TabIndex = 12;
            label6.Text = "Settlements:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(12, 317);
            label7.Name = "label7";
            label7.Size = new Size(120, 21);
            label7.TabIndex = 14;
            label7.Text = "Max Population:";
            // 
            // NrMaxPopulation
            // 
            NrMaxPopulation.Location = new Point(12, 341);
            NrMaxPopulation.Maximum = new decimal(new int[] { 50000000, 0, 0, 0 });
            NrMaxPopulation.Name = "NrMaxPopulation";
            NrMaxPopulation.Size = new Size(300, 23);
            NrMaxPopulation.TabIndex = 15;
            // 
            // CmbPart
            // 
            CmbPart.FormattingEnabled = true;
            CmbPart.Location = new Point(12, 392);
            CmbPart.Name = "CmbPart";
            CmbPart.Size = new Size(300, 23);
            CmbPart.TabIndex = 17;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(12, 368);
            label8.Name = "label8";
            label8.Size = new Size(40, 21);
            label8.TabIndex = 16;
            label8.Text = "Part:";
            // 
            // CmbAtmopart
            // 
            CmbAtmopart.FormattingEnabled = true;
            CmbAtmopart.Location = new Point(12, 442);
            CmbAtmopart.Name = "CmbAtmopart";
            CmbAtmopart.Size = new Size(300, 23);
            CmbAtmopart.TabIndex = 19;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(12, 418);
            label9.Name = "label9";
            label9.Size = new Size(79, 21);
            label9.TabIndex = 18;
            label9.Text = "Atmopart:";
            // 
            // PlanetForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(331, 511);
            Controls.Add(CmbAtmopart);
            Controls.Add(label9);
            Controls.Add(CmbPart);
            Controls.Add(label8);
            Controls.Add(NrMaxPopulation);
            Controls.Add(label7);
            Controls.Add(CmbSettlements);
            Controls.Add(label6);
            Controls.Add(CmbPopulation);
            Controls.Add(label5);
            Controls.Add(CmbAtmosphere);
            Controls.Add(label4);
            Controls.Add(CmbGeology);
            Controls.Add(label3);
            Controls.Add(CmbClass);
            Controls.Add(label2);
            Controls.Add(TxtName);
            Controls.Add(label1);
            Controls.Add(BtnCancel);
            Controls.Add(BtnCreate);
            Name = "PlanetForm";
            Text = "Cluster Planet Editor";
            ((System.ComponentModel.ISupportInitialize)NrMaxPopulation).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnCreate;
        private Button BtnCancel;
        private Label label1;
        private TextBox TxtName;
        private Label label2;
        private ComboBox CmbClass;
        private ComboBox CmbGeology;
        private Label label3;
        private ComboBox CmbAtmosphere;
        private Label label4;
        private ComboBox CmbPopulation;
        private Label label5;
        private ComboBox CmbSettlements;
        private Label label6;
        private Label label7;
        private NumericUpDown NrMaxPopulation;
        private ComboBox CmbPart;
        private Label label8;
        private ComboBox CmbAtmopart;
        private Label label9;
    }
}