using System.ComponentModel;
using X4SectorCreator.Forms.Galaxy.Regions;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class RegionFieldsForm : Form
    {
        private FieldObj _fieldObj;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FieldObj FieldObj
        {
            get => _fieldObj;
            set
            {
                _fieldObj = value;
                if (_fieldObj != null)
                {
                    InitializeFields();
                }
            }
        }

        public RegionFieldsForm()
        {
            InitializeComponent();
        }

        private void InitializeFields()
        {
            txtBackgroundFog.Text = FieldObj.BackgroundFog?.ToString() ?? string.Empty;
            txtDensityFactor.Text = FieldObj.DensityFactor?.ToString("0.##") ?? string.Empty;
            txtDistanceFactor.Text = FieldObj.DistanceFactor?.ToString("0.##") ?? string.Empty;
            txtFactor.Text = FieldObj.Factor?.ToString("0.##") ?? string.Empty;
            txtGroupRef.Text = FieldObj.GroupRef ?? string.Empty;
            txtLocalDensity.Text = FieldObj.LocalDensity?.ToString("0.##") ?? string.Empty;
            txtLodRule.Text = FieldObj.LodRule ?? string.Empty;
            txtMaxNoiseValue.Text = FieldObj.MaxNoiseValue?.ToString("0.##") ?? string.Empty;
            txtMedium.Text = FieldObj.Medium ?? string.Empty;
            txtMinNoiseValue.Text = FieldObj.MinNoiseValue?.ToString("0.##") ?? string.Empty;
            txtMultiplier.Text = FieldObj.Multiplier?.ToString("0.##") ?? string.Empty;
            txtNoiseScale.Text = FieldObj.NoiseScale?.ToString("0.##") ?? string.Empty;
            txtPlaytime.Text = FieldObj.Playtime?.ToString() ?? string.Empty;
            txtRef.Text = FieldObj.Ref ?? string.Empty;
            txtResources.Text = FieldObj.Resources ?? string.Empty;
            txtRotation.Text = FieldObj.Rotation?.ToString("0.##") ?? string.Empty;
            txtRotationVariation.Text = FieldObj.RotationVariation?.ToString("0.##") ?? string.Empty;
            txtSeed.Text = FieldObj.Seed?.ToString("0.##") ?? string.Empty;
            txtSize.Text = FieldObj.Size?.ToString("0.##") ?? string.Empty;
            txtSizeVariation.Text = FieldObj.SizeVariation?.ToString("0.##") ?? string.Empty;
            txtSoundId.Text = FieldObj.SoundId ?? string.Empty;
            txtTexture.Text = FieldObj.Texture ?? string.Empty;
            txtUniformDensity.Text = FieldObj.UniformDensity?.ToString("0.##") ?? string.Empty;
            if (FieldObj.UniformRed != null && FieldObj.UniformGreen != null && FieldObj.UniformBlue != null)
            {
                txtUniformRGB.Text = $"{FieldObj.UniformRed},{FieldObj.UniformGreen},{FieldObj.UniformBlue}";
            }

            if (FieldObj.LocalRed != null && FieldObj.LocalGreen != null && FieldObj.LocalBlue != null)
            {
                txtLocalRgb.Text = $"{FieldObj.LocalRed},{FieldObj.LocalGreen},{FieldObj.LocalBlue}";
            }

            BtnAdd.Text = "Update";
        }

        private void ConvertFromTextboxes(FieldObj fieldObj)
        {
            fieldObj.BackgroundFog = string.IsNullOrWhiteSpace(txtBackgroundFog.Text) ? null : bool.Parse(txtBackgroundFog.Text);
            fieldObj.DensityFactor = string.IsNullOrWhiteSpace(txtDensityFactor.Text) ? null : float.Parse(txtDensityFactor.Text);
            fieldObj.DistanceFactor = string.IsNullOrWhiteSpace(txtDistanceFactor.Text) ? null : float.Parse(txtDistanceFactor.Text);
            fieldObj.Factor = string.IsNullOrWhiteSpace(txtFactor.Text) ? null : float.Parse(txtFactor.Text);
            fieldObj.GroupRef = txtGroupRef.Text;
            fieldObj.LocalDensity = string.IsNullOrWhiteSpace(txtLocalDensity.Text) ? null : float.Parse(txtLocalDensity.Text);
            fieldObj.LodRule = txtLodRule.Text;
            fieldObj.MaxNoiseValue = string.IsNullOrWhiteSpace(txtMaxNoiseValue.Text) ? null : float.Parse(txtMaxNoiseValue.Text);
            fieldObj.Medium = txtMedium.Text;
            fieldObj.MinNoiseValue = string.IsNullOrWhiteSpace(txtMinNoiseValue.Text) ? null : float.Parse(txtMinNoiseValue.Text);
            fieldObj.Multiplier = string.IsNullOrWhiteSpace(txtMultiplier.Text) ? null : float.Parse(txtMultiplier.Text);
            fieldObj.NoiseScale = string.IsNullOrWhiteSpace(txtNoiseScale.Text) ? null : float.Parse(txtNoiseScale.Text);
            fieldObj.Playtime = string.IsNullOrWhiteSpace(txtPlaytime.Text) ? null : int.Parse(txtPlaytime.Text);
            fieldObj.Ref = txtRef.Text;
            fieldObj.Resources = txtResources.Text;
            fieldObj.Rotation = string.IsNullOrWhiteSpace(txtRotation.Text) ? null : float.Parse(txtRotation.Text);
            fieldObj.RotationVariation = string.IsNullOrWhiteSpace(txtRotationVariation.Text) ? null : float.Parse(txtRotationVariation.Text);
            fieldObj.Seed = string.IsNullOrWhiteSpace(txtSeed.Text) ? null : int.Parse(txtSeed.Text);
            fieldObj.Size = string.IsNullOrWhiteSpace(txtSize.Text) ? null : float.Parse(txtSize.Text);
            fieldObj.SizeVariation = string.IsNullOrWhiteSpace(txtSizeVariation.Text) ? null : float.Parse(txtSizeVariation.Text);
            fieldObj.SoundId = txtSoundId.Text;
            fieldObj.Texture = txtTexture.Text;
            fieldObj.UniformDensity = string.IsNullOrWhiteSpace(txtUniformDensity.Text) ? null : float.Parse(txtUniformDensity.Text);

            // Parsing RGB values
            if (!string.IsNullOrWhiteSpace(txtUniformRGB.Text))
            {
                string[] rgb = txtUniformRGB.Text.Split(',');
                if (rgb.Length == 3)
                {
                    fieldObj.UniformRed = int.TryParse(rgb[0], out int red) ? red : null;
                    fieldObj.UniformGreen = int.TryParse(rgb[1], out int green) ? green : null;
                    fieldObj.UniformBlue = int.TryParse(rgb[2], out int blue) ? blue : null;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtLocalRgb.Text))
            {
                string[] rgb = txtLocalRgb.Text.Split(',');
                if (rgb.Length == 3)
                {
                    fieldObj.LocalRed = int.TryParse(rgb[0], out int red) ? red : null;
                    fieldObj.LocalGreen = int.TryParse(rgb[1], out int green) ? green : null;
                    fieldObj.LocalBlue = int.TryParse(rgb[2], out int blue) ? blue : null;
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Validation for types
            if (!DoFieldValidation(out FieldObj newField))
            {
                return;
            }

            switch (BtnAdd.Text)
            {
                case "Add":
                    RegionDefinitionForm regionForm = MainForm.Instance.RegionForm.Value.RegionDefinitionsForm.Value.RegionDefinitionForm.Value;
                    _ = regionForm.ListBoxFields.Items.Add(newField);
                    regionForm.ListBoxFields.SelectedItem = newField;
                    break;
                case "Update":
                    ConvertFromTextboxes(FieldObj);
                    break;
            }



            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool DoFieldValidation(out FieldObj fieldObj)
        {
            HashSet<string> invalidFields = new(StringComparer.OrdinalIgnoreCase);

            // Floats
            IsValidFloat(invalidFields, txtDensityFactor, out float? densityFactor);
            IsValidFloat(invalidFields, txtDistanceFactor, out float? distanceFactor);
            IsValidFloat(invalidFields, txtFactor, out float? factor);
            IsValidFloat(invalidFields, txtMaxNoiseValue, out float? maxNoiseValue);
            IsValidFloat(invalidFields, txtMinNoiseValue, out float? minNoiseValue);
            IsValidFloat(invalidFields, txtMultiplier, out float? multiplier);
            IsValidFloat(invalidFields, txtNoiseScale, out float? noiseScale);
            IsValidFloat(invalidFields, txtRotation, out float? rotation);
            IsValidFloat(invalidFields, txtRotationVariation, out float? rotationVariation);
            IsValidFloat(invalidFields, txtSeed, out float? seed);
            IsValidFloat(invalidFields, txtSize, out float? size);
            IsValidFloat(invalidFields, txtSizeVariation, out float? sizeVariation);
            IsValidFloat(invalidFields, txtLocalDensity, out float? localDensity);
            IsValidFloat(invalidFields, txtUniformDensity, out float? uniformDensity);
            IsValidFloat(invalidFields, txtPlaytime, out float? playtime);

            // Strings
            IsValidString(invalidFields, txtRef, out string tRef);
            IsValidString(invalidFields, txtTexture, out string texture);
            IsValidString(invalidFields, txtMedium, out string medium);
            IsValidString(invalidFields, txtLodRule, out string lodRule);
            IsValidString(invalidFields, txtGroupRef, out string groupRef);
            IsValidString(invalidFields, cmbFieldType, out string fieldType);
            IsValidString(invalidFields, txtResources, out string resources);
            IsValidString(invalidFields, txtSoundId, out string soundId);

            // Exceptional cases
            IsValidString(invalidFields, txtLocalRgb, out _);
            IsValidString(invalidFields, txtUniformRGB, out _);

            // Bools
            IsValidBool(invalidFields, txtBackgroundFog, out bool? backgroundFog);

            if (invalidFields.Count != 0)
            {
                _ = MessageBox.Show("Following fields have invalid values:\n- " + string.Join("\n- ", invalidFields));
                fieldObj = null;
                return false;
            }

            bool localRgbSucces = SeperateRGB(txtLocalRgb.Text, out int lr, out int lg, out int lb);
            bool uniformRgbSucces = SeperateRGB(txtUniformRGB.Text, out int ur, out int ug, out int ub);

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
                Resources = resources,
                SoundId = soundId,
                Playtime = playtime != null ? (int)playtime : null
            };

            return true;
        }

        private static void IsValidBool(HashSet<string> invalidFields, TextBox control, out bool? b)
        {
            b = null;
            if (string.IsNullOrEmpty(control.Text))
            {
                return;
            }

            if (!bool.TryParse(control.Text, out bool boolValue))
            {
                _ = invalidFields.Add(control.Name.Replace("txt", string.Empty).Replace("cmb", string.Empty));
                return;
            }
            b = boolValue;
        }

        private static void IsValidString(HashSet<string> invalidFields, Control control, out string s)
        {
            string cName = control.Name.Replace("txt", string.Empty).Replace("cmb", string.Empty);
            s = control.Text;
            if (control is ComboBox cmb)
            {
                string text = cmb.SelectedItem as string;
                if (string.IsNullOrWhiteSpace(text))
                {
                    _ = invalidFields.Add(cName);
                }
                else
                {
                    s = text;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(control.Text))
                {
                    return;
                }

                // Special cases
                switch (control.Name)
                {
                    case "txtLocalRGB":
                    case "txtUniformRGB":
                        if (!SeperateRGB(control.Text, out _, out _, out _))
                        {
                            _ = invalidFields.Add(cName);
                        }

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
            {
                return;
            }

            if (!float.TryParse(control.Text, out float fl))
            {
                _ = invalidFields.Add(control.Name.Replace("txt", string.Empty));
                return;
            }
            f = fl;
        }

        private static bool SeperateRGB(string rgb, out int r, out int g, out int b)
        {
            r = 0; g = 0; b = 0;
            string[] split = rgb.Split(',');
            return split.Length == 3
&& int.TryParse(split[0], out r) &&
                int.TryParse(split[1], out g) &&
                int.TryParse(split[2], out b);
        }
    }
}
