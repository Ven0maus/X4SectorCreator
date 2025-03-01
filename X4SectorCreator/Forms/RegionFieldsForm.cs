namespace X4SectorCreator.Forms
{
    public partial class RegionFieldsForm : Form
    {
        public RegionFieldsForm()
        {
            InitializeComponent();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Validation for types
            if (!DoFieldValidation(out var newField))
                return;

            var regionForm = MainForm.Instance.RegionForm;
            regionForm.ListBoxFields.Items.Add(newField);
            regionForm.ListBoxFields.SelectedItem = newField;

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool DoFieldValidation(out FieldObj fieldObj)
        {
            var invalidFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Floats
            IsValidFloat(invalidFields, txtDensityFactor, out var densityFactor);
            IsValidFloat(invalidFields, txtDistanceFactor, out var distanceFactor);
            IsValidFloat(invalidFields, txtFactor, out var factor);
            IsValidFloat(invalidFields, txtMaxNoiseValue, out var maxNoiseValue);
            IsValidFloat(invalidFields, txtMinNoiseValue, out var minNoiseValue);
            IsValidFloat(invalidFields, txtMultiplier, out var multiplier);
            IsValidFloat(invalidFields, txtNoiseScale, out var noiseScale);
            IsValidFloat(invalidFields, txtRotation, out var rotation);
            IsValidFloat(invalidFields, txtRotationVariation, out var rotationVariation);
            IsValidFloat(invalidFields, txtSeed, out var seed);
            IsValidFloat(invalidFields, txtSize, out var size);
            IsValidFloat(invalidFields, txtSizeVariation, out var sizeVariation);

            // Strings
            IsValidString(invalidFields, txtRef, out var tRef);
            IsValidString(invalidFields, txtTexture, out var texture);
            IsValidString(invalidFields, txtMedium, out var medium);
            IsValidString(invalidFields, txtLodRule, out var lodRule);
            IsValidString(invalidFields, txtGroupRef, out var groupRef);
            IsValidString(invalidFields, cmbFieldType, out var fieldType);

            if (invalidFields.Count != 0)
            {
                _ = MessageBox.Show("Following fields have invalid values:\n- " + string.Join("\n- ", invalidFields));
                fieldObj = null;
                return false;
            }

            fieldObj = new FieldObj
            {
                DensityFactor = densityFactor,
                DistanceFactor = distanceFactor,
                Factor = factor,
                GroupRef = groupRef,
                LodRule = lodRule,
                MaxNoiseValue = maxNoiseValue,
                Medium = medium,
                MinNoiseValue = minNoiseValue,
                Multiplier = multiplier,
                NoiseScale = noiseScale,
                Ref = tRef,
                Rotation = rotation,
                RotationVariation = rotationVariation,
                Seed = seed,
                Size = size,
                SizeVariation = sizeVariation,
                Texture = texture,
                Type = fieldType
            };

            return true;
        }

        private static void IsValidString(HashSet<string> invalidFields, Control control, out string s)
        {
            s = control.Text;
            if (control is ComboBox cmb)
            {
                var text = cmb.SelectedItem as string;
                if (string.IsNullOrWhiteSpace(text))
                    invalidFields.Add(control.Name.Replace("txt", string.Empty).Replace("cmb", string.Empty));
                else
                    s = text;
            }
        }

        private static void IsValidFloat(HashSet<string> invalidFields, TextBox control, out float? f)
        {
            f = null;
            if (string.IsNullOrWhiteSpace(control.Text))
                return; 
            if (!float.TryParse(control.Text, out var fl))
            {
                invalidFields.Add(control.Name.Replace("txt", string.Empty));
                return;
            }
            f = fl;
        }

        public class FieldObj
        {
            public string Type { get; set; }

            public string GroupRef { get; set; }
            public float? DensityFactor { get; set; }
            public float? Rotation { get; set; }
            public float? RotationVariation { get; set; }
            public float? NoiseScale { get; set; }
            public float? Seed { get; set; }
            public float? MinNoiseValue { get; set; }
            public float? MaxNoiseValue { get; set; }

            // Volumetric fog
            public float? Multiplier { get; set; }
            public string Medium { get; set; }
            public string Texture { get; set; }
            public string LodRule { get; set; }
            public float? Size { get; set; }
            public float? SizeVariation { get; set; }
            public float? DistanceFactor { get; set; }
            public string Ref { get; set; }
            public float? Factor { get; set; }

            public override string ToString()
            {
                // TODO: Write custom entries for each type
                return base.ToString();
            }
        }
    }
}
