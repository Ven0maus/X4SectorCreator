namespace X4SectorCreator.Objects
{
    internal class VanillaChanges
    {
        public List<(Cluster Old, Cluster New)> ModifiedClusters { get; set; } = [];
        public List<Cluster> RemovedClusters { get; set; } = [];
        public List<(Cluster VanillaCluster, Sector Old, Sector New)> ModifiedSectors { get; set; } = [];
        public List<(Cluster VanillaCluster, Sector Sector)> RemovedSectors { get; set; } = [];
    }
}
