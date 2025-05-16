using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Timer = System.Windows.Forms.Timer;

namespace X4SectorCreator.CustomComponents
{
    internal abstract partial class TextSearchComponent : IDisposable
    {
        protected TextBox TextBox { get; }
        protected Timer DebounceTimer { get; }

        private bool _isDisposed = false;

        public TextSearchComponent(TextBox textBox, int debounceDelayMilliseconds = 500)
        {
            TextBox = textBox;
            TextBox.TextChanged += TextBox_TextChanged;
            DebounceTimer = new Timer { Interval = debounceDelayMilliseconds };
            DebounceTimer.Tick += DebounceTimer_Tick;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            DebounceTimer.Stop();
            DebounceTimer.Start();
        }

        protected virtual void DebounceTimer_Tick(object sender, EventArgs e)
        {
            DebounceTimer.Stop();
        }

        /// <summary>
        /// Forces a calculation to happen on the current Text value and calls OnFiltered.
        /// </summary>
        public virtual void ForceCalculate()
        {

        }

        protected static string RemoveDiacritics(string text) =>
            new(text.Normalize(NormalizationForm.FormD)
                           .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                           .ToArray());

        protected static int GetMatchScore(string text, string search)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(search))
                return 0;

            // Lowercase and remove diacritics
            string rawText = RemoveDiacritics(text.ToLowerInvariant());
            string rawSearch = RemoveDiacritics(search.ToLowerInvariant());

            // Tokenize (split camelCase, _, -), then normalize (remove special chars, diacritics)
            string tokenized = TokenizeRegex()
                .Replace(text, "$1 $2")
                .Replace("_", " ")
                .Replace("-", " ")
                .ToLowerInvariant();

            string normalizedTokenized = NormalizeRegex().Replace(RemoveDiacritics(tokenized), "");

            int score = 0;

            // 1. Exact match
            if (rawText == rawSearch)
                return 100;

            // 2. Prefix match
            if (rawText.StartsWith(rawSearch))
                score += 50;

            // 3. Tokenized contains search
            if (normalizedTokenized.Contains(rawSearch))
                score += 30;

            // 4. Raw contains search
            if (rawText.Contains(rawSearch))
                score += 20;

            // 5. Bonus for prefix position in tokenized string
            int tokenIndex = normalizedTokenized.IndexOf(rawSearch);
            if (tokenIndex == 0)
                score += 15;
            else if (tokenIndex > 0 && tokenIndex <= 10)
                score += 5;

            // 6. Fuzzy match fallback if score is still low
            if (score < 40)
            {
                int distance = LevenshteinDistance(rawText, rawSearch);
                score += Math.Max(0, 30 - distance * 10);
            }

            return score;
        }

        private static int LevenshteinDistance(string s, string t)
        {
            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            GC.SuppressFinalize(this);
            TextBox.TextChanged -= TextBox_TextChanged;
            DebounceTimer.Stop();
            DebounceTimer.Dispose();
            _isDisposed = true;
        }

        ~TextSearchComponent()
        {
            Dispose();
        }

        [GeneratedRegex(@"[\W_]+", RegexOptions.Compiled)]
        private static partial Regex NormalizeRegex();
        [GeneratedRegex(@"(?<=[a-z0-9])(?=[A-Z])", RegexOptions.Compiled)]
        private static partial Regex TokenizeRegex();
    }

    internal sealed class TextSearchComponent<T> : TextSearchComponent, IDisposable
    {
        private readonly Func<T, string> _filterCriteriaSelector;
        private readonly List<T> _items;
        private readonly Action<List<T>> _onFiltered;
        private readonly Func<List<T>> _itemGetter;

        public TextSearchComponent(TextBox textBox, List<T> items, 
            Func<T, string> filterCriteriaSelector, 
            Action<List<T>> onFiltered, 
            int debounceDelayMilliseconds = 500) : base(textBox, debounceDelayMilliseconds)
        {
            ArgumentNullException.ThrowIfNull(filterCriteriaSelector, nameof(filterCriteriaSelector));
            ArgumentNullException.ThrowIfNull(onFiltered, nameof(onFiltered));
            ArgumentNullException.ThrowIfNull(items, nameof(items));

            _filterCriteriaSelector = filterCriteriaSelector;
            _onFiltered = onFiltered;
            _items = items;
        }

        public TextSearchComponent(TextBox textBox, Func<List<T>> itemGetter,
            Func<T, string> filterCriteriaSelector,
            Action<List<T>> onFiltered,
            int debounceDelayMilliseconds = 500) : base(textBox, debounceDelayMilliseconds)
        {
            ArgumentNullException.ThrowIfNull(filterCriteriaSelector, nameof(filterCriteriaSelector));
            ArgumentNullException.ThrowIfNull(onFiltered, nameof(onFiltered));
            ArgumentNullException.ThrowIfNull(itemGetter, nameof(itemGetter));

            _filterCriteriaSelector = filterCriteriaSelector;
            _onFiltered = onFiltered;
            _itemGetter = itemGetter;
        }

        /// <inheritdoc/>
        public override void ForceCalculate()
        {
            _onFiltered.Invoke(FilterItems());
        }

        protected override void DebounceTimer_Tick(object sender, EventArgs e)
        {
            base.DebounceTimer_Tick(sender, e);
            _onFiltered.Invoke(FilterItems());
        }

        private readonly Dictionary<(string itemKey, string search), int> _scoreCache = [];

        private List<T> FilterItems()
        {
            string search = TextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(search))
                return [.. (_items ?? _itemGetter.Invoke())]; // Return new list always

            search = RemoveDiacritics(search.ToLowerInvariant()); // Normalize once

            var items = _items ?? _itemGetter.Invoke();

            return [.. items
                .Select(item =>
                {
                    string key = _filterCriteriaSelector(item);
                    var cacheKey = (key, search);

                    if (!_scoreCache.TryGetValue(cacheKey, out int score))
                    {
                        score = GetMatchScore(key, search);
                        _scoreCache[cacheKey] = score;
                    }

                    return new { Item = item, Score = score };
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Item)];
        }
    }
}
