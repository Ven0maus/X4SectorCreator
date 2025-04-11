namespace X4SectorCreator.Objects
{
    public class VanillaChanges
    {
        public List<ModifiedCluster> ModifiedClusters { get; set; } = [];
        public List<Cluster> RemovedClusters { get; set; } = [];
        public List<ModifiedSector> ModifiedSectors { get; set; } = [];
        public List<RemovedSector> RemovedSectors { get; set; } = [];
        public List<RemovedConnection> RemovedConnections { get; set; } = [];

        public IEnumerable<string> GetModifiedDlcContent()
        {
            // Modifications
            foreach (ModifiedCluster modification in ModifiedClusters)
            {
                if (modification.New.Soundtrack != null)
                {
                    var kvp = SoundtrackCollection.Instance.Soundtracks.FirstOrDefault(a => a.Value.Contains(modification.New.Soundtrack));
                    if (kvp.Key != null && kvp.Value != null && !kvp.Key.Equals("vanilla", StringComparison.OrdinalIgnoreCase))
                        yield return kvp.Key;
                }
                yield return modification.New.Dlc;
            }

            foreach (ModifiedSector modification in ModifiedSectors)
            {
                yield return modification.VanillaCluster.Dlc;
            }

            // Deletions
            foreach (RemovedSector modification in RemovedSectors)
            {
                yield return modification.VanillaCluster.Dlc;
            }

            foreach (Cluster modification in RemovedClusters)
            {
                yield return modification.Dlc;
            }

            foreach (RemovedConnection modification in RemovedConnections)
            {
                yield return modification.VanillaCluster.Dlc;
            }
        }

        internal void RemoveExportBloating()
        {
            // Remove bloating from export file
            foreach (Cluster rcc in RemovedClusters)
            {
                rcc.Sectors = null;
            }

            foreach (ModifiedCluster mc in ModifiedClusters)
            {
                mc.Old.Sectors = null;
                mc.New.Sectors = null;
            }
            foreach (ModifiedSector ms in ModifiedSectors)
            {
                ms.VanillaCluster.Sectors = null;
                ms.Old.Zones = null;
                ms.New.Zones = null;
            }
            foreach (RemovedSector rs in RemovedSectors)
            {
                rs.VanillaCluster.Sectors = null;
                rs.Sector.Zones = null;
            }
            foreach (RemovedConnection rc in RemovedConnections)
            {
                rc.VanillaCluster.Sectors = null;
            }
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

    public class RemovedConnection
    {
        public Cluster VanillaCluster { get; set; }
        public Sector Sector { get; set; }
        public Zone Zone { get; set; }
        public Gate Gate { get; set; }
    }
}
