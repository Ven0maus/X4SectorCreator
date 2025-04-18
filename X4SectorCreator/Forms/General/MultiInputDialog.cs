using System.ComponentModel;

namespace X4SectorCreator.Forms.General
{
    internal class MultiInputDialog : Form
    {
        private readonly Dictionary<string, Control> _inputs = [];
        private readonly Button _btnOk;
        private readonly Button _btnCancel;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, string> InputValues { get; private set; }

        public MultiInputDialog(string title, params (string label, string[] values, string defaultValue)[] labels)
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
                foreach ((string label, string[] values, string defaultValue) label in labels)
                {
                    int labelWidth = (int)g.MeasureString(label.label, Font).Width + 5;
                    maxLabelWidth = Math.Max(maxLabelWidth, labelWidth);
                }
            }

            int textBoxWidth = 200;
            int formWidth = maxLabelWidth + textBoxWidth + padding; // Adjust for spacing

            Width = formWidth;
            int y = 20;

            foreach ((string label, string[] values, string defaultValue) label in labels)
            {
                Label lbl = new() { Text = label.label, Left = 10, Top = y, Width = maxLabelWidth };
                Control ctrl;
                if (label.values != null)
                {
                    ComboBox cmb = new() { Left = maxLabelWidth + 10, Top = y, Width = textBoxWidth };
                    ctrl = cmb;
                    foreach (string value in label.values)
                    {
                        _ = cmb.Items.Add(value);
                    }

                    if (!string.IsNullOrWhiteSpace(label.defaultValue))
                    {
                        cmb.SelectedItem = label.defaultValue;
                    }
                }
                else
                {
                    ctrl = new TextBox() { Left = maxLabelWidth + 10, Top = y, Width = textBoxWidth };
                    if (!string.IsNullOrWhiteSpace(label.defaultValue))
                    {
                        ctrl.Text = label.defaultValue;
                    }
                }

                _inputs[label.label] = ctrl;
                Controls.Add(lbl);
                Controls.Add(ctrl);
                y += 30;
            }

            _btnOk = new Button() { Text = "OK", Left = 50, Width = 80, Top = y + 10, DialogResult = DialogResult.OK };
            _btnCancel = new Button() { Text = "Cancel", Left = _btnOk.Right + 10, Width = 80, Top = y + 10, DialogResult = DialogResult.Cancel };
            Controls.Add(_btnOk);
            Controls.Add(_btnCancel);

            Height = y + 80; // Adjust height dynamically

            _btnOk.Click += (sender, e) =>
            {
                InputValues = [];
                foreach (KeyValuePair<string, Control> kvp in _inputs)
                {
                    if (kvp.Value is TextBox txt)
                    {
                        InputValues[kvp.Key] = string.IsNullOrWhiteSpace(txt.Text) ? null : txt.Text;
                    }
                    else if (kvp.Value is ComboBox cmb)
                    {
                        InputValues[kvp.Key] = cmb.SelectedItem as string;
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            };
        }

        public static Dictionary<string, string> Show(string title, params (string label, string[] values, string defaultValue)[] labels)
        {
            using MultiInputDialog form = new(title, labels);
            return form.ShowDialog() == DialogResult.OK ? form.InputValues : null;
        }
    }
}
