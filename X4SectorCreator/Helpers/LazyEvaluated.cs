namespace X4SectorCreator.Helpers
{
    public sealed class LazyEvaluated<T>(Func<T> initializer, Func<T, bool> validityChecker)
    {
        private readonly Func<T> _initializer = initializer;
        private readonly Func<T, bool> _validityChecker = validityChecker;

        private T _value;
        public T Value
        {
            get
            {
                if (_value == null || !_validityChecker.Invoke(_value))
                    _value = _initializer.Invoke();
                return _value;
            }
        }

        public bool IsInitializedAndValid => _value != null && _validityChecker.Invoke(_value);
    }
}
