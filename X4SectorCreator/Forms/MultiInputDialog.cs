using System.ComponentModel;

namespace X4SectorCreator.Forms
{
    internal class MultiInputDialog : Form
    {
        private readonly Dictionary<string, TextBox> _inputs = new();
        private readonly Button _btnOk;
        private readonly Button _btnCancel;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, string> InputValues { get; private set; }

        public MultiInputDialog(string title, params string[] labels)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;

            int padding = 40; // Padding for aesthetics
            int maxLabelWidth = 0; // Track longest label width

            using (Graphics g = CreateGraphics())
            {
                foreach (var label in labels)
                {
                    int labelWidth = (int)g.MeasureString(label, this.Font).Width;
                    maxLabelWidth = Math.Max(maxLabelWidth, labelWidth);
                }
            }

            int textBoxWidth = 200;
            int formWidth = maxLabelWidth + textBoxWidth + padding; // Adjust for spacing

            Width = formWidth;
            int y = 20;

            foreach (var label in labels)
            {
                var lbl = new Label() { Text = label, Left = 10, Top = y, Width = maxLabelWidth };
                var txtBox = new TextBox() { Left = maxLabelWidth + 10, Top = y, Width = textBoxWidth };
                _inputs[label] = txtBox;
                Controls.Add(lbl);
                Controls.Add(txtBox);
                y += 30;
            }

            _btnOk = new Button() { Text = "OK", Left = 50, Width = 80, Top = y + 10, DialogResult = DialogResult.OK };
            _btnCancel = new Button() { Text = "Cancel", Left = _btnOk.Right + 10, Width = 80, Top = y + 10, DialogResult = DialogResult.Cancel };
            Controls.Add(_btnOk);
            Controls.Add(_btnCancel);

            Height = y + 80; // Adjust height dynamically

            _btnOk.Click += (sender, e) =>
            {
                InputValues = new Dictionary<string, string>();
                foreach (var kvp in _inputs)
                    InputValues[kvp.Key] = kvp.Value.Text;
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        public static Dictionary<string, string> Show(string title, params string[] labels)
        {
            using (var form = new MultiInputDialog(title, labels))
            {
                return form.ShowDialog() == DialogResult.OK ? form.InputValues : null;
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MultiInputDialog
            // 
            ClientSize = new Size(284, 261);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MultiInputDialog";
            ResumeLayout(false);
        }
    }
}
