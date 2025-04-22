namespace X4SectorCreator.Forms
{
    partial class SoundtrackSelectorForm
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
            SoundtracksListBox = new ListBox();
            label1 = new Label();
            TxtSearch = new TextBox();
            BtnSelect = new Button();
            BtnCancel = new Button();
            SuspendLayout();
            // 
            // SoundtracksListBox
            // 
            SoundtracksListBox.FormattingEnabled = true;
            SoundtracksListBox.HorizontalScrollbar = true;
            SoundtracksListBox.Location = new Point(8, 38);
            SoundtracksListBox.Name = "SoundtracksListBox";
            SoundtracksListBox.Size = new Size(471, 394);
            SoundtracksListBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(8, 9);
            label1.Name = "label1";
            label1.Size = new Size(60, 21);
            label1.TabIndex = 1;
            label1.Text = "Search:";
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(68, 9);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.Size = new Size(411, 23);
            TxtSearch.TabIndex = 2;
            // 
            // BtnSelect
            // 
            BtnSelect.Location = new Point(151, 438);
            BtnSelect.Name = "BtnSelect";
            BtnSelect.Size = new Size(328, 33);
            BtnSelect.TabIndex = 3;
            BtnSelect.Text = "Select";
            BtnSelect.UseVisualStyleBackColor = true;
            BtnSelect.Click += BtnSelect_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(8, 438);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(137, 33);
            BtnCancel.TabIndex = 4;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // SoundtrackSelectorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(485, 476);
            Controls.Add(BtnCancel);
            Controls.Add(BtnSelect);
            Controls.Add(TxtSearch);
            Controls.Add(label1);
            Controls.Add(SoundtracksListBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SoundtrackSelectorForm";
            Text = "Soundtrack Selector";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox SoundtracksListBox;
        private Label label1;
        private TextBox TxtSearch;
        private Button BtnSelect;
        private Button BtnCancel;
    }
}