
namespace X4SectorCreator.CustomComponents
{
    internal class MultiSelectCombo
    {
        private readonly ComboBox _comboBox;
        private readonly ToolStripDropDown _dropDown;
        private readonly CheckedListBox _container;

        public event EventHandler<ItemCheckEventArgs> OnItemChecked;

        private readonly List<object> _selectedItems = [];
        public IReadOnlyList<object> SelectedItems => _selectedItems;
        public IReadOnlyList<object> Items => GetItems().ToList();

        public MultiSelectCombo(ComboBox combobox)
        {
            _comboBox = combobox;
            _comboBox.KeyDown += ComboBoxKeyDown;
            _comboBox.DropDown += OnDropDown;

            // Setup value container
            _container = new CheckedListBox { CheckOnClick = true };
            _container.ItemCheck += OnItemSelect;

            // Init container items
            foreach (object item in combobox.Items)
            {
                _ = _container.Items.Add(item);
            }

            // Setup toolstrip
            ToolStripControlHost host = new(_container) { AutoSize = false, Width = combobox.Width, Height = 200 };
            _dropDown = new ToolStripDropDown();
            _ = _dropDown.Items.Add(host);
        }

        public void ReInit()
        {
            _container.Items.Clear();
            foreach (object item in _comboBox.Items)
            {
                _ = _container.Items.Add(item);
            }
        }

        public void Select(params object[] objs)
        {
            foreach (object obj in objs)
            {
                if (_container.Items.Contains(obj))
                {
                    int index = _container.Items.IndexOf(obj);
                    CheckState prevState = _container.GetItemCheckState(index);
                    if (prevState == CheckState.Checked)
                    {
                        return;
                    }

                    _container.SetItemChecked(index, true);
                }
            }
        }

        private void ComboBoxKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void OnDropDown(object sender, EventArgs e)
        {
            _ = _container.Focus();
            _dropDown.Show(_comboBox, 0, _comboBox.Height);
        }

        private IEnumerable<object> GetItems()
        {
            foreach (object item in _comboBox.Items)
            {
                yield return item;
            }
        }

        private void OnItemSelect(object sender, ItemCheckEventArgs e)
        {
            // Adjust selected items
            object item = _container.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
            {
                _selectedItems.Add(item);
            }
            else
            {
                _ = _selectedItems.Remove(item);
            }

            _comboBox.Text = string.Join(", ", _selectedItems.Select(a => a.ToString()).ToList());

            // Raise event
            OnItemChecked?.Invoke(this, e);
        }

        public void ResetSelection()
        {
            _selectedItems.Clear();
            for (int i = 0; i < _container.Items.Count; i++)
            {
                _container.SetItemChecked(i, false);
            }

            _comboBox.Text = string.Empty;
        }

        public class NoDropDownComboBox : ComboBox
        {
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x020A)
                {
                    return; // mousewheel
                }

                if (m.Msg is 0x201 or 0x203)
                {
                    OnDropDown(null);
                    return;
                }
                base.WndProc(ref m);
            }
        }
    }
}
