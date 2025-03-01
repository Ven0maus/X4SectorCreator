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
            IsValidFloat(invalidFields, txtLocalDensity, out var localDensity);
            IsValidFloat(invalidFields, txtUniformDensity, out var uniformDensity);

            // Strings
            IsValidString(invalidFields, txtRef, out var tRef);
            IsValidString(invalidFields, txtTexture, out var texture);
            IsValidString(invalidFields, txtMedium, out var medium);
            IsValidString(invalidFields, txtLodRule, out var lodRule);
            IsValidString(invalidFields, txtGroupRef, out var groupRef);
            IsValidString(invalidFields, cmbFieldType, out var fieldType);
            IsValidString(invalidFields, txtResources, out var resources);

            // Exceptional cases
            IsValidString(invalidFields, txtLocalRgb, out _);
            IsValidString(invalidFields, txtUniformRGB, out _);

            // Bools
            IsValidBool(invalidFields, txtBackgroundFog, out var backgroundFog);

            if (invalidFields.Count != 0)
            {
                _ = MessageBox.Show("Following fields have invalid values:\n- " + string.Join("\n- ", invalidFields));
                fieldObj = null;
                return false;
            }

            var localRgbSucces = SeperateRGB(txtLocalRgb.Text, out int lr, out int lg, out int lb);
            var uniformRgbSucces = SeperateRGB(txtUniformRGB.Text, out int ur, out int ug, out int ub);

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
                Type = fieldType,
                LocalBlue = localRgbSucces ? lb : null,
                LocalGreen = localRgbSucces ? lg : null,
                LocalRed = localRgbSucces ? lr : null,
                LocalDensity = localDensity,
                UniformBlue = uniformRgbSucces ? ub : null,
                UniformGreen = uniformRgbSucces ? ug : null,
                UniformRed = uniformRgbSucces ? ur : null,
                UniformDensity = uniformDensity,
                BackgroundFog = backgroundFog,
                Resources = resources
            };

            return true;
        }

        private static void IsValidBool(HashSet<string> invalidFields, Control control, out bool? b)
        {
            b = null;
            if (string.IsNullOrEmpty(control.Text)) return;
            if (!bool.TryParse(control.Text, out var boolValue))
            {
                invalidFields.Add(control.Name.Replace("txt", string.Empty).Replace("cmb", string.Empty));
                return;
            }
            b = boolValue;
        }

        private static void IsValidString(HashSet<string> invalidFields, Control control, out string s)
        {
            var cName = control.Name.Replace("txt", string.Empty).Replace("cmb", string.Empty);
            s = control.Text;
            if (control is ComboBox cmb)
            {
                var text = cmb.SelectedItem as string;
                if (string.IsNullOrWhiteSpace(text))
                    invalidFields.Add(cName);
                else
                    s = text;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(control.Text))
                    return;

                // Special cases
                switch(control.Name)
                {
                    case "txtLocalRGB":
                    case "txtUniformRGB":
                        if (!SeperateRGB(control.Text, out _, out _, out _))
                            invalidFields.Add(cName);
                        break;
                    default:
                        break;
                }
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

        private static bool SeperateRGB(string rgb, out int r, out int g, out int b)
        {
            r = 0; g = 0; b = 0;
            var split = rgb.Split(',');
            if (split.Length != 3) 
                return false;
            if (!int.TryParse(split[0], out r) || 
                !int.TryParse(split[1], out g) || 
                !int.TryParse(split[2], out b)) 
                return false;
            return true;
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

            // Nebulas
            // Map these as one field LocalRGB
            public int? LocalRed { get; set; }
            public int? LocalGreen { get; set; }
            public int? LocalBlue { get; set; }
            public float? LocalDensity { get; set; }

            // Map these as one field UniformRGB
            public int? UniformRed { get; set; }
            public int? UniformGreen { get; set; }
            public int? UniformBlue { get; set; }
            public float? UniformDensity { get; set; }

            public bool? BackgroundFog { get; set; }
            public string Resources { get; set; }

            public override string ToString()
            {
                if (!string.IsNullOrWhiteSpace(Medium))
                    return $"{Type}=\"{Medium}\"";
                if (!string.IsNullOrWhiteSpace(GroupRef))
                    return $"{Type}=\"{GroupRef}\"";
                if (!string.IsNullOrWhiteSpace(Ref))
                {
                    if (!string.IsNullOrWhiteSpace(Resources))
                        return $"{Type}=\"{Ref}\"=\"{Resources}\"";
                    return $"{Type}=\"{Ref}\"";
                }
                return Type;
            }
        }
    }
}
