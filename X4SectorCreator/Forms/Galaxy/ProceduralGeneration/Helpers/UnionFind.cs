namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Helpers
{
    public class UnionFind<T>
    {
        private readonly Dictionary<T, T> parent = [];

        public UnionFind(IEnumerable<T> items)
        {
            foreach (var item in items)
                parent[item] = item;
        }

        public T Find(T x)
        {
            if (!parent.TryGetValue(x, out T value)) return x;
            if (!value.Equals(x))
                parent[x] = Find(value);
            return parent[x];
        }

        public bool Union(T a, T b)
        {
            T rootA = Find(a);
            T rootB = Find(b);
            if (rootA.Equals(rootB)) return false;
            parent[rootB] = rootA;
            return true;
        }
    }
}
