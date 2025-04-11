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

        protected static int GetMatchScore(string text, string search)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(search))
                return 0;

            text = text.ToLower();
            search = search.ToLower();

            string normalizedId = NormalizeRegex().Replace(text, "");
            string tokenizedId = TokenizeRegex().Replace(text, "$1 $2").Replace("_", " ").Replace("-", " ");

            if (text == search) return 100;
            if (normalizedId.StartsWith(search)) return 90;
            if (tokenizedId.Contains(search)) return 75;
            if (normalizedId.Contains(search)) return 60;

            int distance = LevenshteinDistance(text, search);
            return distance <= 2 ? 50 - distance * 10 : 0;
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

        [GeneratedRegex(@"[_\-]")]
        private static partial Regex NormalizeRegex();
        [GeneratedRegex(@"([a-z])([A-Z])")]
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

        private List<T> FilterItems()
        {
            // Return all items if empty
            if (string.IsNullOrWhiteSpace(TextBox.Text))
                return [.. _items ?? _itemGetter.Invoke()]; // Create new list instance always

            return [.. (_items ?? _itemGetter.Invoke())
                .Select(item => new
                {
                    Item = item,
                    Score = GetMatchScore(_filterCriteriaSelector(item), TextBox.Text)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Item)];
        }
    }
}
