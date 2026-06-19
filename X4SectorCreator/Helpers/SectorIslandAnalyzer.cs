namespace X4SectorCreator.Helpers
{
    public static class SectorIslandAnalyzer
    {
        public sealed record SectorConnectivityEntry(
            string ClusterName,
            string SectorName,
            bool IsBaseSector,
            IReadOnlyCollection<string> OutboundSectorNames);

        public sealed record SectorIslandResult(
            string ClusterName,
            string SectorName,
            int IncomingConnections,
            int OutgoingConnections);

        public static IReadOnlyList<SectorIslandResult> FindIsolatedSectors(
            IEnumerable<SectorConnectivityEntry> sectors,
            bool customSectorsOnly = true)
        {
            SectorConnectivityEntry[] entries = sectors.ToArray();
            Dictionary<string, int> inboundCounts = entries
                .SelectMany(a => a.OutboundSectorNames.Distinct(StringComparer.OrdinalIgnoreCase))
                .GroupBy(a => a, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(a => a.Key, a => a.Count(), StringComparer.OrdinalIgnoreCase);

            return entries
                .Where(a => !customSectorsOnly || !a.IsBaseSector)
                .Select(a => new SectorIslandResult(
                    a.ClusterName,
                    a.SectorName,
                    inboundCounts.GetValueOrDefault(a.SectorName, 0),
                    a.OutboundSectorNames.Distinct(StringComparer.OrdinalIgnoreCase).Count()))
                .Where(a => a.IncomingConnections == 0 && a.OutgoingConnections == 0)
                .OrderBy(a => a.ClusterName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(a => a.SectorName, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
