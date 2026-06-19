using X4SectorCreator.Objects;

namespace X4SectorCreator.Helpers
{
    internal static class ImportNameNormalizer
    {
        public static void EnsureImportedSectorNamesPreservingIdentity(List<Cluster> importedClusters)
        {
            foreach (Cluster cluster in importedClusters)
            {
                for (int index = 0; index < cluster.Sectors.Count; index++)
                {
                    Sector sector = cluster.Sectors[index];
                    if (!string.IsNullOrWhiteSpace(sector.Name))
                        continue;

                    string clusterName = NormalizeDisplayName(cluster.Name) ?? "Unnamed Cluster";
                    sector.Name = cluster.Sectors.Count == 1
                        ? clusterName
                        : $"{clusterName} {ToRomanNumeral(index + 1)}";
                }
            }
        }

        private static string NormalizeDisplayName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }

        private static string ToRomanNumeral(int number)
        {
            if (number <= 0)
                return number.ToString();

            (int Value, string Numeral)[] numerals =
            [
                (1000, "M"),
                (900, "CM"),
                (500, "D"),
                (400, "CD"),
                (100, "C"),
                (90, "XC"),
                (50, "L"),
                (40, "XL"),
                (10, "X"),
                (9, "IX"),
                (5, "V"),
                (4, "IV"),
                (1, "I"),
            ];

            var result = new System.Text.StringBuilder();
            int remaining = number;
            foreach (var (value, numeral) in numerals)
            {
                while (remaining >= value)
                {
                    result.Append(numeral);
                    remaining -= value;
                }
            }

            return result.ToString();
        }
    }
}
