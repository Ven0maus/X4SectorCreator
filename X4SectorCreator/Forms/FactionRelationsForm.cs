using System.ComponentModel;
using System.Globalization;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class FactionRelationsForm : Form
    {
        private Faction _faction;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Faction Faction
        {
            get => _faction;
            set
            {
                _faction = value;
                InitFactions();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactionForm FactionForm { get; set; }

        private readonly Dictionary<string, int> _indexMapping = new(StringComparer.OrdinalIgnoreCase);

        public FactionRelationsForm()
        {
            InitializeComponent();
        }

        private void InitFactions()
        {
            _indexMapping.Clear();
            FactionRelationsDataGrid.Rows.Clear();

            var factions = FactionsForm.GetAllFactions(true);
            int index = 0;
            foreach (var faction in factions)
            {
                FactionRelationsDataGrid.Rows.Add(faction, (double)0);
                _indexMapping[faction] = index;
                index++;
            }

            // Overwrite values for each faction that already has a mapping
            if (Faction.Relations?.Relation != null)
            {
                foreach (var relation in Faction.Relations.Relation)
                {
                    if (!_indexMapping.TryGetValue(relation.Faction, out index))
                    {
                        index = FactionRelationsDataGrid.Rows.Count;
                        FactionRelationsDataGrid.Rows.Add(relation.Faction.ToLower(), 0);
                        _indexMapping[relation.Faction] = index;
                    }
                    else
                    {
                        _ = double.TryParse(relation.RelationValue, CultureInfo.InvariantCulture, out double result);
                        FactionRelationsDataGrid.Rows[index].Cells[1].Value = result;
                    }
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // Init
            if (Faction.Relations == null)
                Faction.Relations = new Relations();
            if (Faction.Relations.Relation == null)
                Faction.Relations.Relation = [];

            foreach (DataGridViewRow row in FactionRelationsDataGrid.Rows)
            {
                var factionName = row.Cells[0].Value as string;
                if (string.IsNullOrWhiteSpace(factionName)) continue;
                _ = double.TryParse(row.Cells[1].Value as string, out var factionValue);

                // Find match index
                var faction = Faction.Relations.Relation
                    .FirstOrDefault(a => a.Faction.Equals(factionName, StringComparison.OrdinalIgnoreCase));
                if (faction == null)
                {
                    if (factionValue == 0) continue;
                    Faction.Relations.Relation.Add(new Relation
                    {
                        Faction = factionName.ToLower(),
                        RelationValue = factionValue.ToString()
                    });
                }
                else
                {
                    if (factionValue == 0)
                    {
                        Faction.Relations.Relation.Remove(faction);
                        continue;
                    }
                    faction.RelationValue = factionValue.ToString();
                }
            }

            // This will re-apply the setter of the property to properly serialize the value in the other form
            FactionForm.SetFactionXml(Faction);
            Close();
        }
    }
}