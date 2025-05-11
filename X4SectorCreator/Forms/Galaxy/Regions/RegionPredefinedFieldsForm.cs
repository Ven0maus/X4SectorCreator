using System.Text.Json;
using X4SectorCreator.Configuration;
using X4SectorCreator.CustomComponents;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class RegionPredefinedFieldsForm : Form
    {
        private readonly MultiSelectCombo _mscAsteroids, _mscDebris, _mscGravidar, _mscNebula, _mscPositional, _mscObjects, _mscVolumetricfog, _mscAmbientSound;

        public RegionPredefinedFieldsForm()
        {
            InitializeComponent();

            // Init each field with its mapping
            string json = File.ReadAllText(Constants.DataPaths.PredefinedFieldMappingFilePath);
            IEnumerable<IGrouping<string, FieldObj>> mappingGroups = JsonSerializer.Deserialize<List<FieldObj>>(json, ConfigSerializer.JsonSerializerOptions).GroupBy(a => a.Type);
            foreach (IGrouping<string, FieldObj> group in mappingGroups)
            {
                ComboBox cmb = group.Key.ToLower() switch
                {
                    "asteroid" => cmbAsteroids,
                    "debris" => cmbDebris,
                    "gravidar" => cmbGravidar,
                    "nebula" => cmbNebula,
                    "positional" => cmbPositional,
                    "object" => cmbObjects,
                    "volumetricfog" => cmbVolumetricfog,
                    "ambientsound" => cmbAmbientSound,
                    _ => throw new NotSupportedException(group.Key),
                };
                foreach (FieldObj item in group.OrderBy(a => a.GroupRef ?? "")
                    .ThenBy(a => a.Resources ?? "")
                    .ThenBy(a => a.Ref ?? "")
                    .ThenBy(a => a.SoundId ?? ""))
                {
                    _ = cmb.Items.Add(item);
                }

                // Init multi-select wrappers after item init
                _mscAsteroids = new MultiSelectCombo(cmbAsteroids);
                _mscDebris = new MultiSelectCombo(cmbDebris);
                _mscGravidar = new MultiSelectCombo(cmbGravidar);
                _mscNebula = new MultiSelectCombo(cmbNebula);
                _mscObjects = new MultiSelectCombo(cmbObjects);
                _mscPositional = new MultiSelectCombo(cmbPositional);
                _mscVolumetricfog = new MultiSelectCombo(cmbVolumetricfog);
                _mscAmbientSound = new MultiSelectCombo(cmbAmbientSound);
            }
        }

        private FieldObj[] GetSelectedFieldObjects()
        {
            return _mscAsteroids.SelectedItems
                .Concat(_mscDebris.SelectedItems)
                .Concat(_mscGravidar.SelectedItems)
                .Concat(_mscNebula.SelectedItems)
                .Concat(_mscObjects.SelectedItems)
                .Concat(_mscPositional.SelectedItems)
                .Concat(_mscVolumetricfog.SelectedItems)
                .Concat(_mscAmbientSound.SelectedItems)
                .Cast<FieldObj>()
                .ToArray();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            FieldObj[] selectedFieldObjects = GetSelectedFieldObjects();
            if (selectedFieldObjects == null || selectedFieldObjects.Length == 0)
            {
                _ = MessageBox.Show("Please select atleast one predefined field.");
                return;
            }

            foreach (FieldObj item in selectedFieldObjects)
            {
                _ = MainForm.Instance.RegionForm.Value.RegionDefinitionsForm.Value.RegionDefinitionForm.Value.ListBoxFields.Items.Add(item);
            }

            if (selectedFieldObjects.Length == 1)
            {
                MainForm.Instance.RegionForm.Value.RegionDefinitionsForm.Value.RegionDefinitionForm.Value.ListBoxFields.SelectedItem = selectedFieldObjects[0];
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
