using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace X4SectorCreator.Helpers
{
    internal static partial class Localisation
    {
        // Must be case-sensitive
        private static readonly Dictionary<string, int> _localisations = [];
        private static int _pageId, _currentIndex = 1;
        private static bool _initialized;

        // FNV prime and offset basis for 32-bit hash
        private const uint FnvPrime = 16777619;
        private const uint FnvOffsetBasis = 2166136261;

        private static readonly Regex LocalisationRegex = LocaliseRegex();

        /// <summary>
        /// Initializes the page id.
        /// </summary>
        /// <param name="pageId"></param>
        public static void Initialize(int pageId)
        {
            _pageId = pageId;
            _currentIndex = 1;
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
            _currentIndex = 1;
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
        /// Formats a value with {local:} tag.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Localize(string value)
        {
            if (value != null && !value.StartsWith("{local:", StringComparison.OrdinalIgnoreCase))
            {
                return $"{{local:{value}}}";
            }
            return value;
        }

        public static void LocaliseAllFiles(string modFolder)
        {
            var files = Directory.GetFiles(modFolder, "*.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);

                // Replace all {local:something} with the localized value
                var newContent = LocalisationRegex.Replace(content, match =>
                {
                    var key = match.Value;
                    var localizedValue = Get(key);
                    return localizedValue ?? match.Value; // Fallback: if not found, keep original
                });

                if (newContent != content)
                {
                    File.WriteAllText(file, newContent);
                }
            }
            BuildTFiles(modFolder);
        }

        /// <summary>
        /// Converts a {local:textvalue} to {pageId, index} localisation format. 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string Get(string code)
        {
            if (!_initialized)
                throw new Exception("Localisation class is not yet initialized.");

            // If not a valid local then just simply return the string itself
            if (!code.StartsWith("{local:", StringComparison.OrdinalIgnoreCase))
                return code;

            // Extracting the value from the local tag
            var value = ExtractLocalValue(code);

            if (_localisations.TryGetValue(value, out var index))
            {
                return $"{{{_pageId},{index}}}"; ;
            }

            // Generate a new local code
            var local = $"{{{_pageId},{_currentIndex}}}";

            // Store local for re-use
            _localisations[value] = _currentIndex;

            // Increase index for next use
            _currentIndex++;

            return local;
        }

        private static string ExtractLocalValue(string input)
        {
            int braceCount = 1;
            var builder = new StringBuilder(input.Length - 7);
            for (int i = input.IndexOf("{local:")+7; i < input.Length; i++)
            {
                var value = input[i];
                if (value == '}')
                    braceCount--;
                else if (value == '{')
                    braceCount++;

                if (braceCount == 0)
                {
                    return builder.ToString();
                }
                else
                {
                    builder.Append(value);
                }
            }
            return string.Empty;
        }

        private static void BuildTFiles(string modFolder)
        {
            if (_localisations.Count <= 0) return;

            var modName = Path.GetFileNameWithoutExtension(modFolder);
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("language",
                    new XElement("page",
                        new XAttribute("id", _pageId),
                        new XAttribute("title", $"{modName} T Files"),
                        new XAttribute("descr", $""),
                        new XAttribute("voice", "yes"),
                        _localisations.Select(a => new XElement("t", new XAttribute("id", a.Value), a.Key))
                    )
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(modFolder, $"t/0001.xml")));
        }

        /// <summary>
        /// A FNV hash restricted to 8 digits.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetFnvHash(string value)
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
            return (folded & 0x7FFFFFFF) % 100_000_000;
        }

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }

        [GeneratedRegex(@"\{local:(?:[^{}]|\{[^{}]*\})*\}", RegexOptions.Compiled)]
        private static partial Regex LocaliseRegex();
    }
}
