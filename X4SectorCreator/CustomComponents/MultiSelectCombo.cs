
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
            foreach (var item in combobox.Items)
                _container.Items.Add(item);

            // Setup toolstrip
            var host = new ToolStripControlHost(_container) { AutoSize = false, Width = combobox.Width, Height = 100 };
            _dropDown = new ToolStripDropDown();
            _dropDown.Items.Add(host);
        }

        private void ComboBoxKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void OnDropDown(object sender, EventArgs e)
        {
            _container.Focus();
            _dropDown.Show(_comboBox, 0, _comboBox.Height);
        }

        private IEnumerable<object> GetItems()
        {
            foreach (var item in _comboBox.Items)
                yield return item;
        }

        private void OnItemSelect(object sender, ItemCheckEventArgs e)
        {
            // Adjust selected items
            var item = _container.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
                _selectedItems.Add(item);
            else
                _selectedItems.Remove(item);

            _comboBox.Text = string.Join(", ", _selectedItems.Select(a => a.ToString()).ToList());

            // Raise event
            OnItemChecked?.Invoke(this, e);
        }

        public void ResetSelection()
        {
            _selectedItems.Clear();
            for (int i = 0; i < _container.Items.Count; i++)
                _container.SetItemChecked(i, false);
            _comboBox.Text = string.Empty;
        }

        public class NoDropDownComboBox : ComboBox
        {
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x020A) return; // mousewheel
                if (m.Msg == 0x201 || m.Msg == 0x203)
                {
                    OnDropDown(null);
                    return;
                }
                base.WndProc(ref m);
            }
        }
    }
}
