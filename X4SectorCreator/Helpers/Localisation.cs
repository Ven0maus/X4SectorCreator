using System.Runtime.InteropServices;

namespace X4SectorCreator.Helpers
{
    internal static class Localisation
    {
        // Must be case-sensitive
        private static readonly Dictionary<string, string> _localisations = [];
        private static int _pageId, _currentIndex;
        private static bool _initialized;

        // FNV prime and offset basis for 32-bit hash
        private const uint FnvPrime = 16777619;
        private const uint FnvOffsetBasis = 2166136261;

        /// <summary>
        /// Initializes the page id.
        /// </summary>
        /// <param name="pageId"></param>
        public static void Initialize(int pageId)
        {
            _pageId = pageId;
            _currentIndex = 0;
            _initialized = true;
        }

        /// <summary>
        /// Initializes the page id.
        /// </summary>
        /// <param name="modName"></param>
        /// <param name="modPrefix"></param>
        public static void Initialize(string modName, string modPrefix)
        {
            // Generate a unique FNV hash to use as page id based on (prefix mod name)
            _pageId = GetFnvHash($"{modPrefix} {modName}");
            _currentIndex = 0;
            _initialized = true;
        }

        /// <summary>
        /// Clears out the localisation cache.
        /// </summary>
        /// <param name="requireReinit">This will reset initialization.</param>
        public static void ClearCache(bool requireReinit)
        {
            _localisations.Clear();

            if (_initialized)
                _initialized = !requireReinit;
        }

        /// <summary>
        /// Converts a {local:textvalue} to {pageId, index} localisation format. 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string Get(string code)
        {
            if (!_initialized)
                throw new Exception("Localisation class is not yet initialized.");

            // If not a valid local then just simply return the string itself
            if (!code.StartsWith("{local:", StringComparison.OrdinalIgnoreCase))
                return code;

            // Split : and skip the first entry (local), and re join all the rest)
            var value = string.Join(":", code.Split(':').Skip(1)).TrimEnd('}');
            if (_localisations.TryGetValue(value, out var localCode))
            {
                return localCode;
            }

            // Generate a new local code
            var local = $"{{{_pageId},{_currentIndex}}}";

            // Increase index for next use
            _currentIndex++;

            // Store local for re-use
            _localisations[value] = local;

            return local;
        }

        private static int GetFnvHash(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            uint hash = FnvOffsetBasis;

            ReadOnlySpan<char> chars = value.AsSpan();
            ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(chars);

            foreach (byte b in bytes)
            {
                hash ^= b;
                hash *= FnvPrime;
            }

            // Fold the high bit into the lower bits to improve distribution
            int folded = (int)(hash ^ (hash >> 1));
            return folded & 0x7FFFFFFF;
        }
    }
}
