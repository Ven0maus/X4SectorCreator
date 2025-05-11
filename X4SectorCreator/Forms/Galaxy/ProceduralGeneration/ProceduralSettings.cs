namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration
{
    public class ProceduralSettings
    {
        public int Seed { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MultiSectorChance { get; set; }
        public string MapAlgorithm { get; set; }
        public int MaxSectorRadius { get; set; }

        /* Algorithm Options */
        // Pure Random
        public int ClusterChance { get; set; }

        // Noise
        public int NoiseOctaves { get; set; }
        public float NoisePersistance { get; set; }
        public float NoiseLacunarity { get; set; }
        public float NoiseScale { get; set; }
        public float NoiseThreshold { get; set; }
        public Point NoiseOffset { get; set; }

        /* Gate Options */
        public int MinGatesPerSector { get; set; }
        public int MaxGatesPerSector { get; set; }
        public int GateMultiChancePerSector { get; set; }

        /* Region Options */
        public Dictionary<string, string> Resources { get; set; }

        /* Faction Options */
        public int MinMainFactions { get; set; }
        public int MaxMainFactions { get; set; }
        public int MinPirateFactions { get; set; }
        public int MaxPirateFactions { get; set; }
        public int MinSectorOwnership { get; set; }
        public int MaxSectorOwnership { get; set; }
    }
}
