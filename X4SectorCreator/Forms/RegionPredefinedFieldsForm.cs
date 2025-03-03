using System.Text.Json;
using X4SectorCreator.CustomComponents;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class RegionPredefinedFieldsForm : Form
    {
        private bool _suppressEvents = false;
        private MultiSelectCombo _current = null;
        private readonly string _predefinedFieldMappingFilePath = Path.Combine(Application.StartupPath, "Mappings/predefinedfield_mappings.json");

        private readonly MultiSelectCombo _mscAsteroids, _mscDebris, _mscGravidar, _mscNebula, _mscPositional, _mscObjects, _mscVolumetricfog, _mscAmbientSound;

        public RegionPredefinedFieldsForm()
        {
            InitializeComponent();

            // Init each field with its mapping
            var json = File.ReadAllText(_predefinedFieldMappingFilePath);
            var mappingGroups = JsonSerializer.Deserialize<List<FieldObj>>(json).GroupBy(a => a.Type);
            foreach (var group in mappingGroups)
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
                foreach (var item in group.OrderBy(a => a.GroupRef ?? "")
                    .ThenBy(a => a.Resources ?? "")
                    .ThenBy(a => a.Ref ?? "")
                    .ThenBy(a => a.SoundId ?? ""))
                {
                    cmb.Items.Add(item);
                }

                // Init multi-select wrappers after item init
                _mscAsteroids = new MultiSelectCombo(cmbAsteroids);
                _mscAsteroids.OnItemChecked += CmbAsteroids_OnItemChecked;
                _mscDebris = new MultiSelectCombo(cmbDebris);
                _mscDebris.OnItemChecked += CmbDebris_OnItemChecked;
                _mscGravidar = new MultiSelectCombo(cmbGravidar);
                _mscGravidar.OnItemChecked += CmbGravidar_OnItemChecked;
                _mscNebula = new MultiSelectCombo(cmbNebula);
                _mscNebula.OnItemChecked += CmbNebula_OnItemChecked;
                _mscObjects = new MultiSelectCombo(cmbObjects);
                _mscObjects.OnItemChecked += CmbObjects_OnItemChecked;
                _mscPositional = new MultiSelectCombo(cmbPositional);
                _mscPositional.OnItemChecked += CmbPositional_OnItemChecked;
                _mscVolumetricfog = new MultiSelectCombo(cmbVolumetricfog);
                _mscVolumetricfog.OnItemChecked += CmbVolumetricfog_OnItemChecked;
                _mscAmbientSound = new MultiSelectCombo(cmbAmbientSound);
                _mscAmbientSound.OnItemChecked += CmbAmbientSound_OnItemChecked;
            }
        }

        private FieldObj[] FindSelectedFieldObjects()
        {
            return _current?.SelectedItems.Cast<FieldObj>().ToArray();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var selectedFieldObjects = FindSelectedFieldObjects();
            if (selectedFieldObjects == null || selectedFieldObjects.Length == 0)
            {
                _ = MessageBox.Show("Please select atleast one predefined field.");
                return;
            }

            foreach (var item in selectedFieldObjects)
                MainForm.Instance.RegionForm.ListBoxFields.Items.Add(item);
            if (selectedFieldObjects.Length == 1)
                MainForm.Instance.RegionForm.ListBoxFields.SelectedItem = selectedFieldObjects[0];

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CmbAsteroids_OnItemChecked(object sender, EventArgs e)
        {
            ResetAllExcept(_mscAsteroids);
        }

        private void CmbNebula_OnItemChecked(object sender, EventArgs e)
        {
            ResetAllExcept(_mscNebula);
        }

        private void CmbVolumetricfog_OnItemChecked(object sender, EventArgs e)
        {
            ResetAllExcept(_mscVolumetricfog);
        }

        private void CmbObjects_OnItemChecked(object sender, EventArgs e)
        {
            ResetAllExcept(_mscObjects);
        }

        private void CmbGravidar_OnItemChecked(object sender, EventArgs e)
        {
            ResetAllExcept(_mscGravidar);
        }

        private void CmbPositional_OnItemChecked(object sender, EventArgs e)
        {
            ResetAllExcept(_mscPositional);
        }

        private void CmbDebris_OnItemChecked(object sender, EventArgs e)
        {
            ResetAllExcept(_mscDebris);
        }

        private void CmbAmbientSound_OnItemChecked(object sender, EventArgs e)
        {
            ResetAllExcept(_mscAmbientSound);
        }

        private void ResetAllExcept(MultiSelectCombo cmb)
        {
            if (_suppressEvents) return;
            _current = cmb.SelectedItems.Count > 0 ? cmb : null;
            _suppressEvents = true;
            var fields = new List<MultiSelectCombo>
            {
                _mscAsteroids,
                _mscNebula,
                _mscObjects,
                _mscVolumetricfog,
                _mscPositional,
                _mscGravidar,
                _mscDebris,
                _mscAmbientSound
            };
            fields.Remove(cmb);
            foreach (var field in fields)
                field.ResetSelection();
            _suppressEvents = false;
        }
    }
}
