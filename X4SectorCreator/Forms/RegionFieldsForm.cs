namespace X4SectorCreator.Forms
{
    public partial class RegionFieldsForm : Form
    {
        public RegionFieldsForm()
        {
            InitializeComponent();
        }

        public class FieldObj
        {
            public string Type { get; set; }

            public string GroupRef { get; set; }
            public float DensityFactor { get; set; }
            public float Rotation { get; set; }
            public float RotationVariation { get; set; }
            public float NoiseScale { get; set; }
            public float Seed { get; set; }
            public float MinNoiseValue { get; set; }
            public float MaxNoiseValue { get; set; }

            // Volumetric fog
            public float Multiplier { get; set; }
            public string Medium { get; set; }
            public string Texture { get; set; }
            public string LodRule { get; set; }
            public float Size { get; set; }
            public float SizeVariation { get; set; }
            public float DistanceFactor { get; set; }
            public string Ref { get; set; }
            public float Factor { get; set; }

            public override string ToString()
            {
                // TODO
                return base.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // TODO: Validation

            var regionForm = MainForm.Instance.RegionForm;

            // Add new field object
            var newField = new FieldObj
            {
                // TODO
            };

            regionForm.ListBoxFields.Items.Add(newField);
            regionForm.ListBoxFields.SelectedItem = newField;

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
