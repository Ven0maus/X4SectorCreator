using X4SectorCreator.Objects;

namespace X4SectorCreator.Helpers
{
    internal static class ImportIdentityAssigner
    {
        public static void AssignImportedIdsPreservingOrder(
            List<Cluster> importedClusters,
            int maxClusterId,
            Func<string, int> getNextSectorId,
            Func<string, string, int> getSectorId,
            Func<string, string, int> getNextZoneId,
            Func<string, string, string, int> getZoneId)
        {
            int nextClusterId = maxClusterId + 1;
            foreach (Cluster cluster in importedClusters.Where(a => !a.IsBaseGame))
            {
                cluster.Id = nextClusterId++;
            }

            foreach (Cluster cluster in importedClusters)
            {
                int nextSectorId = cluster.IsBaseGame
                    ? getNextSectorId(cluster.BaseGameMapping)
                    : 1;

                foreach (Sector sector in cluster.Sectors.Where(a => !a.IsBaseGame))
                {
                    sector.Id = nextSectorId++;
                }

                foreach (Sector sector in cluster.Sectors.Where(a => a.IsBaseGame))
                {
                    sector.Id = getSectorId(cluster.BaseGameMapping, sector.BaseGameMapping);
                }

                foreach (Sector sector in cluster.Sectors)
                {
                    int nextZoneId = cluster.IsBaseGame && sector.IsBaseGame
                        ? getNextZoneId(cluster.BaseGameMapping, sector.BaseGameMapping)
                        : 1;

                    foreach (Zone zone in sector.Zones.Where(a => !a.IsBaseGame))
                    {
                        zone.Id = nextZoneId++;
                    }

                    foreach (Zone zone in sector.Zones.Where(a => a.IsBaseGame))
                    {
                        zone.Id = getZoneId(cluster.BaseGameMapping, sector.BaseGameMapping, zone.Name);
                    }

                    int nextGateId = 1;
                    foreach (Gate gate in sector.Zones.SelectMany(a => a.Gates))
                    {
                        gate.Id = nextGateId++;
                    }
                }

                cluster.Sectors = cluster.Sectors
                    .OrderBy(a => a.IsBaseGame ? 0 : 1)
                    .ThenBy(a => a.Id)
                    .ToList();
            }
        }
    }
}
