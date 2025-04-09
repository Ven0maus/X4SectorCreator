using System.Data;
using System.ComponentModel;
using X4SectorCreator.Objects;
using X4SectorCreator.CustomComponents;

namespace X4SectorCreator.Forms
{
    public partial class FactionSelectionForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactoryForm FactoryForm { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Factory Factory { get; set; }

        private readonly MultiSelectCombo _mscFactions;

        public FactionSelectionForm()
        {
            InitializeComponent();

            var factions = MainForm.Instance.FactionColorMapping.Keys
                .Append("Ownerless")
                .OrderBy(a => a);

            foreach (var faction in factions)
            {
                CmbFactions.Items.Add(faction);
                CmbOwner.Items.Add(faction);
            }

            _mscFactions = new MultiSelectCombo(CmbFactions);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (CmbOwner.SelectedItem == null)
            {
                _ = MessageBox.Show("Please fill in the faction fields.");
                return;
            }

            // Set owner faction
            Factory.Owner = CmbOwner.SelectedItem as string;
            Factory.Location ??= new Factory.LocationObj();
            Factory.Location.Faction = "[" + string.Join(",", _mscFactions.SelectedItems.Cast<string>()) + "]";

            FactoryForm.TxtFactoryXml.Text = Factory.SerializeFactory();
            FactoryForm.TxtFactoryXml.SelectionStart = FactoryForm.TxtFactoryXml.Text.Length;

            Close();
        }
    }
}
