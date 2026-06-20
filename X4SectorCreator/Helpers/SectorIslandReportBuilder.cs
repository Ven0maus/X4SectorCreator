using System.Text;

namespace X4SectorCreator.Helpers
{
    internal static class SectorIslandReportBuilder
    {
        public static string BuildUnresolvedIslandReport(
            string modName,
            int islandsDetected,
            int islandsFixed,
            IReadOnlyList<SectorIslandAnalyzer.SectorIslandResult> remainingIslands)
        {
            StringBuilder builder = new();
            builder.AppendLine("Unresolved sector islands");
            builder.AppendLine($"Mod: {modName ?? "<unknown>"}");
            builder.AppendLine($"Islands detected: {islandsDetected}");
            builder.AppendLine($"Islands fixed: {islandsFixed}");
            builder.AppendLine($"Unresolved islands: {remainingIslands?.Count ?? 0}");
            builder.AppendLine();

            if (remainingIslands == null || remainingIslands.Count == 0)
            {
                builder.Append("No unresolved sector islands remain.");
                return builder.ToString();
            }

            foreach (SectorIslandAnalyzer.SectorIslandResult island in remainingIslands
                .OrderBy(a => a.ClusterName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(a => a.SectorName, StringComparer.OrdinalIgnoreCase))
            {
                builder.AppendLine($"- {island.SectorName} (cluster: {island.ClusterName})");
            }

            return builder.ToString().TrimEnd();
        }
    }
}
