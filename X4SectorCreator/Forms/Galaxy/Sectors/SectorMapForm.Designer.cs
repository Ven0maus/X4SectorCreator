namespace X4SectorCreator
{
    partial class SectorMapForm
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
            BtnSelectLocation = new Button();
            ControlPanel = new Panel();
            MapOptionsListBox = new CheckedListBox();
            BtnHideOptions = new Button();
            label1 = new Label();
            label3 = new Label();
            DlcListBox = new CheckedListBox();
            LegendPanel = new Panel();
            BtnHideLegend = new Button();
            LegendTree = new TreeView();
            label2 = new Label();
            TxtSearch = new TextBox();
            label4 = new Label();
            ControlPanel.SuspendLayout();
            LegendPanel.SuspendLayout();
            SuspendLayout();
            // 
            // BtnSelectLocation
            // 
            BtnSelectLocation.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnSelectLocation.Enabled = false;
            BtnSelectLocation.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            BtnSelectLocation.Location = new Point(4, 311);
            BtnSelectLocation.Name = "BtnSelectLocation";
            BtnSelectLocation.Size = new Size(167, 30);
            BtnSelectLocation.TabIndex = 1;
            BtnSelectLocation.Text = "Select Location";
            BtnSelectLocation.UseVisualStyleBackColor = true;
            BtnSelectLocation.Click += BtnSelectLocation_Click;
            // 
            // ControlPanel
            // 
            ControlPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ControlPanel.BackColor = Color.Black;
            ControlPanel.BorderStyle = BorderStyle.FixedSingle;
            ControlPanel.Controls.Add(MapOptionsListBox);
            ControlPanel.Controls.Add(BtnHideOptions);
            ControlPanel.Controls.Add(label1);
            ControlPanel.Controls.Add(label3);
            ControlPanel.Controls.Add(DlcListBox);
            ControlPanel.Controls.Add(BtnSelectLocation);
            ControlPanel.Location = new Point(795, 12);
            ControlPanel.Name = "ControlPanel";
            ControlPanel.Size = new Size(176, 347);
            ControlPanel.TabIndex = 4;
            // 
            // MapOptionsListBox
            // 
            MapOptionsListBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MapOptionsListBox.BackColor = SystemColors.MenuText;
            MapOptionsListBox.CheckOnClick = true;
            MapOptionsListBox.ForeColor = Color.White;
            MapOptionsListBox.FormattingEnabled = true;
            MapOptionsListBox.Items.AddRange(new object[] { "Show Vanilla Sectors", "Show Custom Sectors", "Show Connections", "Show Coordinates", "Show Regions", "Show Stations" });
            MapOptionsListBox.Location = new Point(3, 27);
            MapOptionsListBox.Name = "MapOptionsListBox";
            MapOptionsListBox.ScrollAlwaysVisible = true;
            MapOptionsListBox.Size = new Size(167, 148);
            MapOptionsListBox.TabIndex = 9;
            MapOptionsListBox.ItemCheck += MapOptionsListBox_ItemCheck;
            // 
            // BtnHideOptions
            // 
            BtnHideOptions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnHideOptions.BackColor = Color.DimGray;
            BtnHideOptions.FlatStyle = FlatStyle.Flat;
            BtnHideOptions.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            BtnHideOptions.ForeColor = Color.White;
            BtnHideOptions.Location = new Point(152, 0);
            BtnHideOptions.Name = "BtnHideOptions";
            BtnHideOptions.Size = new Size(21, 21);
            BtnHideOptions.TabIndex = 8;
            BtnHideOptions.Text = "^";
            BtnHideOptions.UseVisualStyleBackColor = false;
            BtnHideOptions.Click += BtnHideOptions_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.BackColor = Color.DimGray;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(-1, 183);
            label1.Name = "label1";
            label1.Size = new Size(176, 23);
            label1.TabIndex = 0;
            label1.Text = "Game DLCs";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.BackColor = Color.DimGray;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label3.ForeColor = Color.White;
            label3.Location = new Point(-2, 0);
            label3.Name = "label3";
            label3.Size = new Size(176, 23);
            label3.TabIndex = 1;
            label3.Text = "Options";
            label3.TextAlign = ContentAlignment.TopCenter;
            // 
            // DlcListBox
            // 
            DlcListBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DlcListBox.BackColor = SystemColors.MenuText;
            DlcListBox.CheckOnClick = true;
            DlcListBox.ForeColor = Color.White;
            DlcListBox.FormattingEnabled = true;
            DlcListBox.Location = new Point(4, 211);
            DlcListBox.Name = "DlcListBox";
            DlcListBox.ScrollAlwaysVisible = true;
            DlcListBox.Size = new Size(167, 94);
            DlcListBox.TabIndex = 4;
            DlcListBox.ItemCheck += DlcListBox_ItemCheck;
            // 
            // LegendPanel
            // 
            LegendPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LegendPanel.BackColor = Color.Black;
            LegendPanel.BorderStyle = BorderStyle.FixedSingle;
            LegendPanel.Controls.Add(BtnHideLegend);
            LegendPanel.Controls.Add(LegendTree);
            LegendPanel.Controls.Add(label2);
            LegendPanel.Location = new Point(711, 478);
            LegendPanel.Name = "LegendPanel";
            LegendPanel.Size = new Size(265, 298);
            LegendPanel.TabIndex = 5;
            // 
            // BtnHideLegend
            // 
            BtnHideLegend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnHideLegend.BackColor = Color.Black;
            BtnHideLegend.FlatStyle = FlatStyle.Flat;
            BtnHideLegend.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Pixel);
            BtnHideLegend.ForeColor = Color.White;
            BtnHideLegend.Location = new Point(232, 3);
            BtnHideLegend.Name = "BtnHideLegend";
            BtnHideLegend.Size = new Size(26, 26);
            BtnHideLegend.TabIndex = 7;
            BtnHideLegend.Text = "V";
            BtnHideLegend.UseVisualStyleBackColor = false;
            BtnHideLegend.Click += BtnHideLegend_Click;
            // 
            // LegendTree
            // 
            LegendTree.Anchor = AnchorStyles.None;
            LegendTree.BackColor = Color.Black;
            LegendTree.BorderStyle = BorderStyle.FixedSingle;
            LegendTree.DrawMode = TreeViewDrawMode.OwnerDrawText;
            LegendTree.Font = new Font("Segoe UI", 11F);
            LegendTree.ForeColor = Color.White;
            LegendTree.LineColor = Color.White;
            LegendTree.Location = new Point(3, 33);
            LegendTree.Name = "LegendTree";
            LegendTree.Size = new Size(257, 258);
            LegendTree.TabIndex = 1;
            LegendTree.BeforeSelect += LegendTree_BeforeSelect;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.CausesValidation = false;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold | FontStyle.Underline);
            label2.ForeColor = Color.White;
            label2.Location = new Point(45, 3);
            label2.Name = "label2";
            label2.Size = new Size(181, 27);
            label2.TabIndex = 0;
            label2.Text = "Map Legend";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TxtSearch
            // 
            TxtSearch.Location = new Point(147, 9);
            TxtSearch.Name = "TxtSearch";
            TxtSearch.PlaceholderText = "Search by name..";
            TxtSearch.Size = new Size(281, 23);
            TxtSearch.TabIndex = 6;
            // 
            // label4
            // 
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label4.ForeColor = Color.White;
            label4.Location = new Point(12, 9);
            label4.Name = "label4";
            label4.Size = new Size(138, 23);
            label4.TabIndex = 9;
            label4.Text = "Search by name:";
            label4.TextAlign = ContentAlignment.TopCenter;
            // 
            // SectorMapForm
            // 
            ClientSize = new Size(983, 782);
            Controls.Add(label4);
            Controls.Add(TxtSearch);
            Controls.Add(LegendPanel);
            Controls.Add(ControlPanel);
            MinimizeBox = false;
            Name = "SectorMapForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "X4 Sector Map";
            WindowState = FormWindowState.Maximized;
            Load += SectorMapForm_Load;
            ControlPanel.ResumeLayout(false);
            LegendPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        internal Button BtnSelectLocation;
        internal Panel ControlPanel;
        internal CheckedListBox DlcListBox;
        private Panel LegendPanel;
        private Label label2;
        private TreeView LegendTree;
        internal Button BtnHideLegend;
        private Label label1;
        private Label label3;
        internal Button BtnHideOptions;
        private TextBox TxtSearch;
        private Label label4;
        internal CheckedListBox MapOptionsListBox;
    }
}