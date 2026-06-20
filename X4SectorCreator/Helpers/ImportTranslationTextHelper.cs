using System.Text.RegularExpressions;

namespace X4SectorCreator.Helpers
{
    internal static partial class ImportTranslationTextHelper
    {
        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            string normalized = TranslationTokenRegex().Replace(value, string.Empty);
            normalized = normalized.Replace("\\(", "(");
            normalized = normalized.Replace("\\)", ")");
            normalized = normalized.Replace("()", string.Empty);
            normalized = normalized.Replace("\t", " ");
            normalized = WhitespaceRegex().Replace(normalized, " ").Trim();

            if (normalized.StartsWith('(') && normalized.EndsWith(')'))
            {
                string inner = normalized[1..^1].Trim();
                if (!string.IsNullOrWhiteSpace(inner) && inner.IndexOf('(') < 0)
                    normalized = inner;
            }

            Match suffixDuplicate = DuplicateSuffixRegex().Match(normalized);
            if (suffixDuplicate.Success &&
                string.Equals(suffixDuplicate.Groups["name"].Value.Trim(), suffixDuplicate.Groups["dup"].Value.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                normalized = suffixDuplicate.Groups["name"].Value.Trim();
            }

            Match prefixDuplicate = DuplicatePrefixRegex().Match(normalized);
            if (prefixDuplicate.Success &&
                string.Equals(prefixDuplicate.Groups["name"].Value.Trim(), prefixDuplicate.Groups["dup"].Value.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                normalized = prefixDuplicate.Groups["name"].Value.Trim();
            }

            string parentheticalFlattened = ParentheticalGroupRegex().Replace(normalized, match =>
            {
                string inner = match.Groups["text"].Value.Trim();
                return string.IsNullOrWhiteSpace(inner) ? string.Empty : inner + " ";
            });
            parentheticalFlattened = WhitespaceRegex().Replace(parentheticalFlattened, " ").Trim();
            if (!string.IsNullOrWhiteSpace(parentheticalFlattened) && parentheticalFlattened.Any(char.IsLetterOrDigit))
                normalized = parentheticalFlattened;

            normalized = CollapseRepeatedPhrase(normalized);

            if (!normalized.Any(char.IsLetterOrDigit))
                return null;

            return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
        }

        private static string CollapseRepeatedPhrase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            string[] parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && parts.Length % 2 == 0)
            {
                int half = parts.Length / 2;
                bool halvesMatch = true;
                for (int i = 0; i < half; i++)
                {
                    if (!string.Equals(parts[i], parts[i + half], StringComparison.OrdinalIgnoreCase))
                    {
                        halvesMatch = false;
                        break;
                    }
                }

                if (halvesMatch)
                    return string.Join(' ', parts.Take(half));
            }

            return value;
        }

        [GeneratedRegex(@"\{\s*\d+\s*,\s*\d+\s*\}")]
        private static partial Regex TranslationTokenRegex();

        [GeneratedRegex("\\s+")]
        private static partial Regex WhitespaceRegex();

        [GeneratedRegex(@"^(?<name>.+?)\((?<dup>.+)\)$")]
        private static partial Regex DuplicateSuffixRegex();

        [GeneratedRegex(@"^\((?<dup>.+)\)(?<name>.+)$")]
        private static partial Regex DuplicatePrefixRegex();

        [GeneratedRegex(@"\((?<text>[^()]+)\)")]
        private static partial Regex ParentheticalGroupRegex();
    }
}
