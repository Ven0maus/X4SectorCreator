using System.Text.RegularExpressions;
using Timer = System.Windows.Forms.Timer;

namespace X4SectorCreator.CustomComponents
{
    public class TextSearchComponent<T> : IDisposable
    {
        private readonly Func<T, string> _filterCriteriaSelector;
        private readonly List<T> _items;
        private readonly Action<List<T>> _onFiltered;
        private readonly Timer _debounceTimer;
        private readonly TextBox _textBox;

        public TextSearchComponent(TextBox textBox, List<T> items, 
            Func<T, string> filterCriteriaSelector, 
            Action<List<T>> onFiltered, 
            int debounceDelayMilliseconds = 500)
        {
            ArgumentNullException.ThrowIfNull(filterCriteriaSelector, nameof(filterCriteriaSelector));
            ArgumentNullException.ThrowIfNull(onFiltered, nameof(onFiltered));
            ArgumentNullException.ThrowIfNull(items, nameof(items));

            _filterCriteriaSelector = filterCriteriaSelector;
            _onFiltered = onFiltered;
            _items = items;
            _textBox = textBox;
            _debounceTimer = new Timer { Interval = debounceDelayMilliseconds };

            // Hook events
            _debounceTimer.Tick += DebounceTimer_Tick;
            _textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            _onFiltered.Invoke(FilterItems());
        }

        private List<T> FilterItems()
        {
            // Return all items if empty
            if (string.IsNullOrWhiteSpace(_textBox.Text))
                return [.. _items]; // Create new list instance always

            return [.. _items
                .Select(item => new
                {
                    Item = item,
                    Score = GetMatchScore(_filterCriteriaSelector(item), _textBox.Text)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Item)];
        }

        private static int GetMatchScore(string text, string search)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(search))
                return 0;

            text = text.ToLower();
            search = search.ToLower();

            string normalizedId = Regex.Replace(text, @"[_\-]", "");
            string tokenizedId = Regex.Replace(text, @"([a-z])([A-Z])", "$1 $2").Replace("_", " ").Replace("-", " ");

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

        private bool _isDisposed = false;
        public void Dispose()
        {
            if (_isDisposed) return;
            GC.SuppressFinalize(this);
            _textBox.TextChanged -= TextBox_TextChanged;
            _debounceTimer.Stop();
            _debounceTimer.Dispose();
            _isDisposed = true;
        }

        ~TextSearchComponent()
        {
            Dispose();
        }
    }
}
