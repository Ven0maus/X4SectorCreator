namespace X4SectorCreator.Objects
{
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

        public string SoundId { get; set; }
        public int? Playtime { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(SoundId))
            {
                return $"{Type}=\"{SoundId}\"";
            }

            if (!string.IsNullOrWhiteSpace(Medium))
            {
                if (!string.IsNullOrWhiteSpace(Ref))
                    return $"{Type}=\"{Ref}\"=\"{Medium}\"";
                return $"{Type}=\"{Medium}\"";
            }

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
