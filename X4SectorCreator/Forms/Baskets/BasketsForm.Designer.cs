namespace X4SectorCreator.Forms
{
    partial class BasketsForm
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
            BtnNewBasket = new Button();
            ListBaskets = new ListBox();
            label1 = new Label();
            BtnRemoveSelected = new Button();
            BtnExitBasketsWindow = new Button();
            CmbFilterOptions = new ComboBox();
            BtnCopyToClipboard = new Button();
            SuspendLayout();
            // 
            // BtnNewBasket
            // 
            BtnNewBasket.Location = new Point(238, 33);
            BtnNewBasket.Name = "BtnNewBasket";
            BtnNewBasket.Size = new Size(152, 35);
            BtnNewBasket.TabIndex = 0;
            BtnNewBasket.Text = "New Basket";
            BtnNewBasket.UseVisualStyleBackColor = true;
            BtnNewBasket.Click += BtnNewBasket_Click;
            // 
            // ListBaskets
            // 
            ListBaskets.FormattingEnabled = true;
            ListBaskets.HorizontalScrollbar = true;
            ListBaskets.Location = new Point(12, 33);
            ListBaskets.Name = "ListBaskets";
            ListBaskets.Size = new Size(220, 319);
            ListBaskets.TabIndex = 1;
            ListBaskets.DoubleClick += ListBaskets_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(62, 21);
            label1.TabIndex = 2;
            label1.Text = "Baskets";
            // 
            // BtnRemoveSelected
            // 
            BtnRemoveSelected.Location = new Point(238, 74);
            BtnRemoveSelected.Name = "BtnRemoveSelected";
            BtnRemoveSelected.Size = new Size(152, 35);
            BtnRemoveSelected.TabIndex = 3;
            BtnRemoveSelected.Text = "Remove Selected";
            BtnRemoveSelected.UseVisualStyleBackColor = true;
            BtnRemoveSelected.Click += BtnRemoveSelected_Click;
            // 
            // BtnExitBasketsWindow
            // 
            BtnExitBasketsWindow.Location = new Point(238, 168);
            BtnExitBasketsWindow.Name = "BtnExitBasketsWindow";
            BtnExitBasketsWindow.Size = new Size(152, 35);
            BtnExitBasketsWindow.TabIndex = 4;
            BtnExitBasketsWindow.Text = "Exit Baskets Window";
            BtnExitBasketsWindow.UseVisualStyleBackColor = true;
            BtnExitBasketsWindow.Click += BtnExitBasketsWindow_Click;
            // 
            // CmbFilterOptions
            // 
            CmbFilterOptions.FormattingEnabled = true;
            CmbFilterOptions.Items.AddRange(new object[] { "Vanilla", "Custom", "Both" });
            CmbFilterOptions.Location = new Point(80, 6);
            CmbFilterOptions.Name = "CmbFilterOptions";
            CmbFilterOptions.Size = new Size(152, 23);
            CmbFilterOptions.TabIndex = 5;
            CmbFilterOptions.SelectedIndexChanged += CmbFilterOptions_SelectedIndexChanged;
            // 
            // BtnCopyToClipboard
            // 
            BtnCopyToClipboard.Location = new Point(238, 115);
            BtnCopyToClipboard.Name = "BtnCopyToClipboard";
            BtnCopyToClipboard.Size = new Size(152, 47);
            BtnCopyToClipboard.TabIndex = 6;
            BtnCopyToClipboard.Text = "Copy Selected To Clipboard";
            BtnCopyToClipboard.UseVisualStyleBackColor = true;
            BtnCopyToClipboard.Click += BtnCopyToClipboard_Click;
            // 
            // BasketsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(396, 357);
            Controls.Add(BtnCopyToClipboard);
            Controls.Add(CmbFilterOptions);
            Controls.Add(BtnExitBasketsWindow);
            Controls.Add(BtnRemoveSelected);
            Controls.Add(label1);
            Controls.Add(ListBaskets);
            Controls.Add(BtnNewBasket);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BasketsForm";
            Text = "Baskets";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnNewBasket;
        private ListBox ListBaskets;
        private Label label1;
        private Button BtnRemoveSelected;
        private Button BtnExitBasketsWindow;
        private ComboBox CmbFilterOptions;
        private Button BtnCopyToClipboard;
    }
}