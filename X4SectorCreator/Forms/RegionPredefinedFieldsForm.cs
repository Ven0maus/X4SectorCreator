using System.Text.Json;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class RegionPredefinedFieldsForm : Form
    {
        private bool _suppressEvents = false;
        private ComboBox _current = null;
        private readonly string _predefinedFieldMappingFilePath = Path.Combine(Application.StartupPath, "Mappings/predefinedfield_mappings.json");

        public RegionPredefinedFieldsForm()
        {
            InitializeComponent();

            // Init each field with its mapping
            var json = File.ReadAllText(_predefinedFieldMappingFilePath);
            var mappingGroups = JsonSerializer.Deserialize<List<FieldObj>>(json).GroupBy(a => a.Type);
            foreach (var group in mappingGroups)
            {
                ComboBox cmb;
                switch (group.Key.ToLower())
                {
                    case "asteroid":
                        cmb = cmbAsteroids;
                        break;
                    case "debris":
                        cmb = cmbDebris;
                        break;
                    case "gravidar":
                        cmb = cmbGravidar;
                        break;
                    case "nebula":
                        cmb = cmbNebula;
                        break;
                    case "positional":
                        cmb = cmbPositional;
                        break;
                    case "object":
                        cmb = cmbObjects;
                        break;
                    case "volumetricfog":
                        cmb = cmbVolumetricfog;
                        break;
                    case "ambientsound":
                        cmb = cmbAmbientSound;
                        break;
                    default:
                        throw new NotSupportedException(group.Key);
                }

                foreach (var item in group.OrderBy(a => a.GroupRef ?? "")
                    .ThenBy(a => a.Resources ?? "")
                    .ThenBy(a => a.Ref ?? "")
                    .ThenBy(a => a.SoundId ?? ""))
                {
                    cmb.Items.Add(item);
                }
            }
        }

        private FieldObj FindSelectedFieldObj()
        {
            return _current != null ? _current.SelectedItem as FieldObj : null;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var selectedFieldObj = FindSelectedFieldObj();
            if (selectedFieldObj == null)
            {
                _ = MessageBox.Show("Please select one predefined field.");
                return;
            }

            MainForm.Instance.RegionForm.ListBoxFields.Items.Add(selectedFieldObj);
            MainForm.Instance.RegionForm.ListBoxFields.SelectedItem = selectedFieldObj;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CmbAsteroids_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbAsteroids);
        }

        private void CmbNebula_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbNebula);
        }

        private void CmbVolumetricfog_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbVolumetricfog);
        }

        private void CmbObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbObjects);
        }

        private void CmbGravidar_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbGravidar);
        }

        private void CmbPositional_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbPositional);
        }

        private void CmbDebris_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbDebris);
        }

        private void cmbAmbientSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAllExcept(cmbAmbientSound);
        }

        private void ResetAllExcept(ComboBox cmb)
        {
            if (_suppressEvents) return;
            _current = cmb.SelectedItem != null ? cmb : null;
            _suppressEvents = true;
            var fields = new List<ComboBox>
            {
                cmbAsteroids,
                cmbNebula,
                cmbObjects,
                cmbVolumetricfog,
                cmbPositional,
                cmbGravidar,
                cmbDebris,
                cmbAmbientSound
            };
            fields.Remove(cmb);
            foreach (var field in fields)
                field.SelectedItem = null;
            _suppressEvents = false;
        }
    }
}
