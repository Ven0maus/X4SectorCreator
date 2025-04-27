using System.Data;
using System.ComponentModel;
using X4SectorCreator.Objects;
using X4SectorCreator.CustomComponents;
using X4SectorCreator.XmlGeneration;

namespace X4SectorCreator.Forms
{
    public partial class FactionSelectionForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactoryForm FactoryForm { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Factory Factory { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobForm JobForm { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Job Job { get; set; }

        private readonly MultiSelectCombo _mscFactions;

        public FactionSelectionForm()
        {
            InitializeComponent();

            var factions = FactionsForm.GetAllFactions(true, true);
            foreach (var faction in factions)
            {
                CmbFactions.Items.Add(faction);
                CmbOwner.Items.Add(faction);
            }

            _mscFactions = new MultiSelectCombo(CmbFactions);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (CmbOwner.SelectedItem == null || _mscFactions.SelectedItems.Count == 0)
            {
                _ = MessageBox.Show("Please fill in the faction fields.");
                return;
            }

            if (JobForm != null)
                HandleJobSave();
            else if (FactoryForm != null)
                HandleFactorySave();
            else
                throw new Exception("No JobForm or FactoryForm set on FactionSelectionForm. Invalid state, report a bug!");

            Close();
        }

        private void HandleFactorySave()
        {
            // Set owner faction
            Factory.Owner = GodGeneration.CorrectFactionName(CmbOwner.SelectedItem as string);
            if (Factory.Module?.Select?.Faction != null)
                Factory.Module.Select.Faction = Factory.Owner;
            Factory.Location ??= new Factory.LocationObj();
            Factory.Location.Faction = "[" + string.Join(",", _mscFactions.SelectedItems.Cast<string>().Select(GodGeneration.CorrectFactionName)) + "]";

            FactoryForm.TxtFactoryXml.Text = Factory.SerializeFactory().Replace("{faction.Id}", Factory.Owner, StringComparison.OrdinalIgnoreCase);
            FactoryForm.TxtFactoryXml.SelectionStart = FactoryForm.TxtFactoryXml.Text.Length;
        }

        private void HandleJobSave()
        {
            var owner = GodGeneration.CorrectFactionName(CmbOwner.SelectedItem as string);

            // Set faction on various objects
            if (Job.Category != null)
            {
                Job.Category.Faction = owner;
            }

            if (Job.Location != null)
            {
                Job.Location.Faction = "[" + string.Join(",", _mscFactions.SelectedItems.Cast<string>().Select(GodGeneration.CorrectFactionName)) + "]";
            }

            if (Job.Ship?.Select != null)
            {
                Job.Ship.Select.Faction = owner;
            }

            if (Job.Ship?.Owner != null)
            {
                Job.Ship.Owner.Exact = owner;
            }

            if (Job.Location?.Policefaction != null)
            {
                Job.Location.Policefaction = owner;
            }

            JobForm.TxtJobXml.Text = Job.SerializeJob().Replace("{faction.Id}", owner, StringComparison.OrdinalIgnoreCase);
            JobForm.TxtJobXml.SelectionStart = JobForm.TxtJobXml.Text.Length;
        }
    }
}
