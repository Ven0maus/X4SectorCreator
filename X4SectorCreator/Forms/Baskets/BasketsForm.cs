using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class BasketsForm : Form
    {
        public readonly LazyEvaluated<BasketForm> BasketForm = new(() => new BasketForm(), a => !a.IsDisposed);

        // Remember the selected option through app use
        private static string _selectedFilterOption = "Custom";

        public static readonly Lazy<List<Basket>> VanillaBaskets = new(() =>
        {
            List<Basket> list = [];
            if (File.Exists(Constants.DataPaths.VanillaBasketsPath))
            {
                string xml = File.ReadAllText(Constants.DataPaths.VanillaBasketsPath);
                Baskets baskets = Baskets.DeserializeBaskets(xml);
                foreach (Basket basket in baskets.BasketList)
                {
                    basket.IsBaseGame = true;
                    list.Add(basket);
                }
            }
            return list;
        });

        public BasketsForm()
        {
            InitializeComponent();

            // Set default
            CmbFilterOptions.SelectedItem = _selectedFilterOption;
        }

        public void UpdateBaskets(Basket selected = null)
        {
            string option = CmbFilterOptions.SelectedItem as string;
            _selectedFilterOption = option;

            ListBaskets.Items.Clear();
            switch (_selectedFilterOption.ToLower())
            {
                case "vanilla":
                    foreach (Basket basket in VanillaBaskets.Value.OrderBy(a => a.Id))
                    {
                        _ = ListBaskets.Items.Add(basket);
                    }

                    break;
                case "custom":
                    foreach (Basket basket in JobsForm.AllBaskets.Values.Where(a => !a.IsBaseGame).OrderBy(a => a.Id))
                    {
                        _ = ListBaskets.Items.Add(basket);
                    }

                    break;
                case "both":
                    foreach (Basket basket in JobsForm.AllBaskets.Values.Concat(VanillaBaskets.Value).OrderBy(a => a.Id))
                    {
                        _ = ListBaskets.Items.Add(basket);
                    }

                    break;
            }

            ListBaskets.SelectedItem = selected;
        }

        private void BtnNewBasket_Click(object sender, EventArgs e)
        {
            BasketForm.Value.BasketsForm = this;
            BasketForm.Value.Show();
        }

        private void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (ListBaskets.SelectedItem is not Basket basket)
            {
                return;
            }

            if (basket.IsBaseGame)
            {
                _ = MessageBox.Show("Vanilla baskets cannot be removed or modified.");
                return;
            }

            int index = ListBaskets.Items.IndexOf(ListBaskets.SelectedItem);
            ListBaskets.Items.Remove(ListBaskets.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            ListBaskets.SelectedItem = index >= 0 && ListBaskets.Items.Count > 0 ? ListBaskets.Items[index] : null;

            // Remove also from baskets collection itself
            _ = JobsForm.AllBaskets.Remove(basket.Id);
        }

        private void BtnExitBasketsWindow_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CmbFilterOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBaskets();
        }

        private void ListBaskets_DoubleClick(object sender, EventArgs e)
        {
            if (ListBaskets.SelectedItem is not Basket basket)
            {
                return;
            }

            BasketForm.Value.BasketsForm = this;
            BasketForm.Value.Basket = basket;
            BasketForm.Value.Show();
        }

        private void BtnCopyToClipboard_Click(object sender, EventArgs e)
        {
            if (ListBaskets.SelectedItem is not Basket basket)
            {
                return;
            }

            Clipboard.SetText(basket.IsBaseGame ? basket.ToString() : $"PREFIX_{basket}");
        }
    }
}
