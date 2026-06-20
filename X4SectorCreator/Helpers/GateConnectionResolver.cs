namespace X4SectorCreator.Helpers
{
    public static class GateConnectionResolver
    {
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            string value = path.Trim().Replace('\\', '/');
            while (value.StartsWith("../", StringComparison.Ordinal))
            {
                value = value[3..];
            }

            return value;
        }

        public static Dictionary<string, T> BuildSourcePathLookup<T>(
            IEnumerable<T> items,
            Func<T, string> sourcePathSelector)
        {
            return items
                .Select(a => (Item: a, SourcePath: NormalizePath(sourcePathSelector(a))))
                .Where(a => !string.IsNullOrWhiteSpace(a.SourcePath))
                .GroupBy(a => a.SourcePath, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(a => a.Key, a => a.First().Item, StringComparer.OrdinalIgnoreCase);
        }

        public static bool TryResolveTarget<T>(
            IReadOnlyDictionary<string, T> sourcePathLookup,
            string destinationPath,
            out T target)
        {
            string normalizedDestinationPath = NormalizePath(destinationPath);
            if (string.IsNullOrWhiteSpace(normalizedDestinationPath))
            {
                target = default;
                return false;
            }

            return sourcePathLookup.TryGetValue(normalizedDestinationPath, out target);
        }
    }
}
