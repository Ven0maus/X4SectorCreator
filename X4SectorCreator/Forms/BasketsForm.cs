using System.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class BasketsForm : Form
    {
        private BasketForm _basketForm;
        public BasketForm BasketForm => _basketForm != null && !_basketForm.IsDisposed ? _basketForm : (_basketForm = new BasketForm());

        public static readonly Lazy<List<Basket>> VanillaBaskets = new(() =>
        {
            var vanillaBasketsPath = Path.Combine(Application.StartupPath, "Data/Mappings/vanilla_baskets.xml");
            var list = new List<Basket>();
            if (File.Exists(vanillaBasketsPath))
            {
                var xml = File.ReadAllText(vanillaBasketsPath);
                Baskets baskets = Baskets.DeserializeBaskets(xml);
                foreach (var basket in baskets.BasketList)
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
            CmbFilterOptions.SelectedItem = "Both";
        }

        public void UpdateBaskets()
        {
            var option = CmbFilterOptions.SelectedItem as string;

            ListBaskets.Items.Clear();
            switch (option.ToLower())
            {
                case "vanilla":
                    foreach (var basket in VanillaBaskets.Value.OrderBy(a => a.Id))
                        ListBaskets.Items.Add(basket);
                    break;
                case "custom":
                    foreach (var basket in JobsForm.AllBaskets.Values.Where(a => !a.IsBaseGame).OrderBy(a => a.Id))
                        ListBaskets.Items.Add(basket);
                    break;
                case "both":
                    foreach (var basket in JobsForm.AllBaskets.Values.Concat(VanillaBaskets.Value).OrderBy(a => a.Id))
                        ListBaskets.Items.Add(basket);
                    break;
            }

            ListBaskets.SelectedItem = null;
        }

        private void BtnNewBasket_Click(object sender, EventArgs e)
        {
            BasketForm.BasketsForm = this;
            BasketForm.Show();
        }

        private void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (ListBaskets.SelectedItem is not Basket basket) return;

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
            JobsForm.AllBaskets.Remove(basket.Id);
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
            if (ListBaskets.SelectedItem is not Basket basket) return;

            BasketForm.BasketsForm = this;
            BasketForm.Basket = basket;
            BasketForm.Show();
        }

        private void BtnCopyToClipboard_Click(object sender, EventArgs e)
        {
            if (ListBaskets.SelectedItem is not Basket basket) return;
            Clipboard.SetText(basket.ToString());
        }
    }
}
