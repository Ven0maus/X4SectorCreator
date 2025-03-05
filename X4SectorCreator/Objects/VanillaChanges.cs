namespace X4SectorCreator.Objects
{
    public class VanillaChanges
    {
        public List<ModifiedCluster> ModifiedClusters { get; set; } = [];
        public List<Cluster> RemovedClusters { get; set; } = [];
        public List<ModifiedSector> ModifiedSectors { get; set; } = [];
        public List<RemovedSector> RemovedSectors { get; set; } = [];

        public IEnumerable<string> GetModifiedDlcContent()
        {
            foreach (var modification in ModifiedClusters)
                yield return modification.New.Dlc;
            foreach (var modification in ModifiedSectors)
                yield return modification.VanillaCluster.Dlc;
            foreach (var modification in RemovedSectors)
                yield return modification.VanillaCluster.Dlc;
            foreach (var modification in RemovedClusters)
                yield return modification.Dlc;
        }
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
