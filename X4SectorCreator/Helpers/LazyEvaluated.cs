namespace X4SectorCreator.Helpers
{
    /// <summary>
    /// Lazy initializes a value, and re-evaluates if it needs to be re-initialized on every acces.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="initializer"></param>
    /// <param name="evaluator"></param>
    public sealed class LazyEvaluated<T>(Func<T> initializer, Func<T, bool> evaluator = null)
    {
        private readonly Func<T> _initializer = initializer;
        private readonly Func<T, bool> _evaluator = evaluator ?? (a => true);

        private T _value;
        /// <summary>
        /// The lazy evaluated value.
        /// </summary>
        public T Value
        {
            get
            {
                if (_value == null || !_evaluator.Invoke(_value))
                {
                    _value = _initializer.Invoke();
                }

                return _value;
            }
        }

        /// <summary>
        /// Returns true if the value is initialized and valid according to the evaluator.
        /// </summary>
        public bool IsInitialized => _value != null && _evaluator.Invoke(_value);
    }
}
