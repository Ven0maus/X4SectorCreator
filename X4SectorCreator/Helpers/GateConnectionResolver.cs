namespace X4SectorCreator.Helpers
{
    public static class GateConnectionResolver
    {
        public static Dictionary<string, T> BuildSourcePathLookup<T>(
            IEnumerable<T> items,
            Func<T, string> sourcePathSelector)
        {
            return items
                .Select(a => (Item: a, SourcePath: sourcePathSelector(a)))
                .Where(a => !string.IsNullOrWhiteSpace(a.SourcePath))
                .GroupBy(a => a.SourcePath, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(a => a.Key, a => a.First().Item, StringComparer.OrdinalIgnoreCase);
        }

        public static bool TryResolveTarget<T>(
            IReadOnlyDictionary<string, T> sourcePathLookup,
            string destinationPath,
            out T target)
        {
            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                target = default;
                return false;
            }

            return sourcePathLookup.TryGetValue(destinationPath, out target);
        }
    }
}
