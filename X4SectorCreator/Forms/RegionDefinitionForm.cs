﻿using System.ComponentModel;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class RegionDefinitionForm : Form
    {
        private RegionResourcesForm _regionResourcesForm;
        public RegionResourcesForm RegionResourcesForm => _regionResourcesForm != null && !_regionResourcesForm.IsDisposed
            ? _regionResourcesForm
            : (_regionResourcesForm = new RegionResourcesForm());

        private RegionFalloffForm _regionFalloffForm;
        public RegionFalloffForm RegionFalloffForm => _regionFalloffForm != null && !_regionFalloffForm.IsDisposed
            ? _regionFalloffForm
            : (_regionFalloffForm = new RegionFalloffForm());

        private RegionFieldsForm _regionFieldsForm;
        public RegionFieldsForm RegionFieldsForm => _regionFieldsForm != null && !_regionFieldsForm.IsDisposed
            ? _regionFieldsForm
            : (_regionFieldsForm = new RegionFieldsForm());

        private RegionPredefinedFieldsForm _regionPredefinedFieldsForm;
        public RegionPredefinedFieldsForm RegionPredefinedFieldsForm => _regionPredefinedFieldsForm != null && !_regionPredefinedFieldsForm.IsDisposed
            ? _regionPredefinedFieldsForm
            : (_regionPredefinedFieldsForm = new RegionPredefinedFieldsForm());

        public static readonly List<RegionDefinition> RegionDefinitions = [];

        private RegionDefinition _regionDefinition;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RegionDefinition RegionDefinition
        {
            get => _regionDefinition;
            set
            {
                _regionDefinition = value;
                if (_regionDefinition != null)
                {
                    txtDensity.Text = _regionDefinition.Density;
                    txtMaxNoiseValue.Text = _regionDefinition.MaxNoiseValue;
                    txtMinNoiseValue.Text = _regionDefinition.MinNoiseValue;
                    txtNoiseScale.Text = _regionDefinition.NoiseScale;
                    txtRegionDefinitionName.Text = _regionDefinition.Name;
                    txtRotation.Text = _regionDefinition.Rotation;
                    txtSeed.Text = _regionDefinition.Seed;
                    cmbBoundaryType.SelectedItem = _regionDefinition.BoundaryType;

                    foreach (FieldObj field in _regionDefinition.Fields)
                    {
                        _ = ListBoxFields.Items.Add(field);
                    }

                    foreach (StepObj fallOff in _regionDefinition.Falloff)
                    {
                        _ = fallOff.Type.Equals("Lateral", StringComparison.OrdinalIgnoreCase)
                            ? ListBoxLateral.Items.Add(fallOff)
                            : ListBoxRadial.Items.Add(fallOff);
                    }
                    foreach (Resource resource in _regionDefinition.Resources)
                    {
                        _ = ListBoxResources.Items.Add(resource);
                    }

                    BtnCreateRegionDefinition.Text = "Update Region Definition";
                }
            }
        }

        public RegionDefinitionForm()
        {
            InitializeComponent();
        }

        public void InitDefaultFalloff()
        {
            // Some defaults to make configurating easier
            List<StepObj> lateral =
            [
                new() { Position = "0.0", Value = "0.0" },
                new() { Position = "0.1", Value = "1.0" },
                new() { Position = "0.9", Value = "1.0" },
                new() { Position = "1.0", Value = "0.0" }
            ];

            List<StepObj> radial =
            [
                new() { Position = "0.0", Value = "1.0" },
                new() { Position = "0.8", Value = "1.0" },
                new() { Position = "1.0", Value = "0.0" }
            ];

            foreach (StepObj lat in lateral)
            {
                lat.Type = "Lateral";
                _ = ListBoxLateral.Items.Add(lat);
            }

            foreach (StepObj rad in radial)
            {
                rad.Type = "Radial";
                _ = ListBoxRadial.Items.Add(rad);
            }
        }

        private void BtnCreateRegion_Click(object sender, EventArgs e)
        {
            // Name check
            if (string.IsNullOrWhiteSpace(txtRegionDefinitionName.Text))
            {
                _ = MessageBox.Show("Please insert a valid name for the region definition.");
                return;
            }

            // Boundary check
            string selectedBoundaryType = cmbBoundaryType.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedBoundaryType))
            {
                _ = MessageBox.Show("Please select a valid boundary type for the region definition.");
                return;
            }

            List<string> messages = [];
            IsValidInteger(messages, txtRotation, out int rotation);
            IsValidInteger(messages, txtSeed, out int seed);
            IsValidInteger(messages, txtNoiseScale, out int noiseScale);
            IsValidFloat(messages, txtDensity, out float density);
            IsValidFloat(messages, txtMinNoiseValue, out float minNoiseValue);
            IsValidFloat(messages, txtMaxNoiseValue, out float maxNoiseValue);
            if (messages.Count > 0)
            {
                _ = MessageBox.Show(string.Join("\n", messages));
                return;
            }

            switch (BtnCreateRegionDefinition.Text)
            {
                case "Create Region Definition":
                    RegionDefinition regionDefinition = new()
                    {
                        Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                        Density = density.ToString("0.##"),
                        MaxNoiseValue = maxNoiseValue.ToString("0.##"),
                        MinNoiseValue = minNoiseValue.ToString("0.##"),
                        Rotation = rotation.ToString(),
                        NoiseScale = noiseScale.ToString(),
                        Seed = seed.ToString(),
                        Name = txtRegionDefinitionName.Text,
                        BoundaryType = selectedBoundaryType.ToLower(),
                    };

                    // Fields
                    regionDefinition.Fields.AddRange(ListBoxFields.Items.Cast<FieldObj>());

                    // Falloff lateral & radial
                    regionDefinition.Falloff.AddRange(ListBoxLateral.Items.Cast<StepObj>());
                    regionDefinition.Falloff.AddRange(ListBoxRadial.Items.Cast<StepObj>());

                    // Resources
                    regionDefinition.Resources.AddRange(ListBoxResources.Items.Cast<Resource>());

                    RegionDefinitions.Add(regionDefinition);
                    _ = MainForm.Instance.RegionForm.Value.ListBoxRegionDefinitions.Items.Add(regionDefinition);
                    MainForm.Instance.RegionForm.Value.ListBoxRegionDefinitions.SelectedItem = regionDefinition;
                    break;
                case "Update Region Definition":
                    RegionDefinition.Density = density.ToString("0.##");
                    RegionDefinition.Rotation = rotation.ToString();
                    RegionDefinition.MaxNoiseValue = maxNoiseValue.ToString("0.##");
                    RegionDefinition.MinNoiseValue = minNoiseValue.ToString("0.##");
                    RegionDefinition.BoundaryType = selectedBoundaryType.ToLower();
                    RegionDefinition.NoiseScale = noiseScale.ToString();
                    RegionDefinition.Seed = seed.ToString();
                    RegionDefinition.Name = txtRegionDefinitionName.Text;

                    // Collections
                    RegionDefinition.Fields = ListBoxFields.Items.Cast<FieldObj>().ToList();
                    RegionDefinition.Falloff = ListBoxLateral.Items.Cast<StepObj>()
                        .Concat(ListBoxRadial.Items.Cast<StepObj>())
                        .ToList();
                    RegionDefinition.Resources = ListBoxResources.Items.Cast<Resource>().ToList();

                    int index = MainForm.Instance.RegionForm.Value.ListBoxRegionDefinitions.SelectedIndex;
                    MainForm.Instance.RegionForm.Value.ListBoxRegionDefinitions.Items.Remove(RegionDefinition);
                    MainForm.Instance.RegionForm.Value.ListBoxRegionDefinitions.Items.Insert(index, RegionDefinition);
                    MainForm.Instance.RegionForm.Value.ListBoxRegionDefinitions.SelectedItem = RegionDefinition;
                    break;
            }

            Close();
        }

        private static void IsValidFloat(List<string> messages, TextBox textBox, out float value)
        {
            if (!float.TryParse(textBox.Text, out value))
            {
                messages.Add($"{textBox.Name.Replace("txt", string.Empty)} must be a valid numeric value.");
            }
        }

        private static void IsValidInteger(List<string> messages, TextBox textBox, out int value)
        {
            if (!int.TryParse(textBox.Text, out value))
            {
                messages.Add($"{textBox.Name.Replace("txt", string.Empty)} must be a valid numeric integer value.");
            }
        }

        private void BtnResourcesAdd_Click(object sender, EventArgs e)
        {
            RegionResourcesForm.Show();
        }

        private void BtnResourcesDel_Click(object sender, EventArgs e)
        {
            if (ListBoxResources.SelectedItem is not Resource selectedResource)
            {
                return;
            }

            int index = ListBoxResources.Items.IndexOf(selectedResource);
            ListBoxResources.Items.Remove(selectedResource);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            ListBoxResources.SelectedItem = index >= 0 && ListBoxResources.Items.Count > 0 ? ListBoxResources.Items[index] : null;
        }

        private void BtnFalloffAdd_Click(object sender, EventArgs e)
        {
            RegionFalloffForm.Show();
        }

        private void BtnFalloffDel_Click(object sender, EventArgs e)
        {
            ListBox listBox = GetActiveFalloffListbox();
            if (listBox.SelectedItem is not StepObj lateral)
            {
                return;
            }

            int index = listBox.Items.IndexOf(lateral);
            listBox.Items.Remove(lateral);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            listBox.SelectedItem = index >= 0 && listBox.Items.Count > 0 ? listBox.Items[index] : null;
        }

        private void BtnFalloffUp_Click(object sender, EventArgs e)
        {
            // Determine which tab is active
            ListBox listBox = GetActiveFalloffListbox();
            int index = listBox.SelectedIndex;
            if (index > 0) // Ensure it's not already at the top
            {
                object item = listBox.Items[index];
                listBox.Items.RemoveAt(index);
                listBox.Items.Insert(index - 1, item);
                listBox.SelectedIndex = index - 1; // Keep selection
            }
        }

        private void BtnFalloffDown_Click(object sender, EventArgs e)
        {
            // Determine which tab is active
            ListBox listBox = GetActiveFalloffListbox();
            int index = listBox.SelectedIndex;
            if (index < listBox.Items.Count - 1 && index >= 0) // Ensure it's not already at the bottom
            {
                object item = listBox.Items[index];
                listBox.Items.RemoveAt(index);
                listBox.Items.Insert(index + 1, item);
                listBox.SelectedIndex = index + 1; // Keep selection
            }
        }

        private ListBox GetActiveFalloffListbox()
        {
            // Determine which tab is active
            TabPage sT = TabControlFalloff.SelectedTab;
            return sT.Name switch
            {
                "tabLateral" => ListBoxLateral,
                "tabRadial" => ListBoxRadial,
                _ => throw new NotSupportedException(sT.Name),
            };
        }

        private void BtnFieldsAddCustom_Click(object sender, EventArgs e)
        {
            RegionFieldsForm.Show();
        }

        private void BtnFieldsDel_Click(object sender, EventArgs e)
        {
            if (ListBoxFields.SelectedItem is not FieldObj fieldObj)
            {
                return;
            }

            int index = ListBoxFields.Items.IndexOf(fieldObj);
            ListBoxFields.Items.Remove(fieldObj);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            ListBoxFields.SelectedItem = index >= 0 && ListBoxFields.Items.Count > 0 ? ListBoxFields.Items[index] : null;
        }

        private void BtnAddPredefined_Click(object sender, EventArgs e)
        {
            RegionPredefinedFieldsForm.Show();
        }

        private void ListBoxFields_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxFields.SelectedItem is not FieldObj selectedField)
            {
                return;
            }

            RegionFieldsForm.FieldObj = selectedField;
            RegionFieldsForm.Show();
        }

        private void ListBoxResources_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxResources.SelectedItem is not Resource selectedResource)
            {
                return;
            }

            RegionResourcesForm.Resource = selectedResource;
            RegionResourcesForm.Show();
        }

        private void ListBoxLateral_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxLateral.SelectedItem is not StepObj selectedStep)
            {
                return;
            }

            RegionFalloffForm.StepObj = selectedStep;
            RegionFalloffForm.Show();
        }

        private void ListBoxRadial_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxRadial.SelectedItem is not StepObj selectedStep)
            {
                return;
            }

            RegionFalloffForm.StepObj = selectedStep;
            RegionFalloffForm.Show();
        }
    }
}
