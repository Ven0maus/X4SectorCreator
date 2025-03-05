namespace X4SectorCreator.Objects
{
    public class VanillaChanges
    {
        public List<ModifiedCluster> ModifiedClusters { get; set; } = [];
        public List<Cluster> RemovedClusters { get; set; } = [];
        public List<ModifiedSector> ModifiedSectors { get; set; } = [];
        public List<RemovedSector> RemovedSectors { get; set; } = [];
    }

    // Define explicit classes instead of tuples
    public class ModifiedCluster
    {
        public Cluster Old { get; set; }
        public Cluster New { get; set; }
    }

    public class ModifiedSector
    {
        public Cluster VanillaCluster { get; set; }
        public Sector Old { get; set; }
        public Sector New { get; set; }
    }

    public class RemovedSector
    {
        public Cluster VanillaCluster { get; set; }
        public Sector Sector { get; set; }
    }
}
