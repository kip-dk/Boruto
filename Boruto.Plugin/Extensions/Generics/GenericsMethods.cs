using Boruto.Extensions.TypeConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Boruto.Extensions.Generics
{
    public static class GenericsMethods
    {
        public static object Safe(this Dictionary<string, object> index, string name)
        {
            if (index.TryGetValue(name, out object v))
            {
                return v;
            }
            return null;
        }

        public static V GetSafe<K, V>(this Dictionary<K, V> index, K k)
        {
            if (index.ContainsKey(k)) return index[k];
            return default(V);
        }


        public static IEnumerable<T[]> Pages<T>(this IEnumerable<T> input, int pageSize)
        {
            var result = new List<T[]>();

            var arr = input.ToArray();

            var next = arr.Take(pageSize).ToArray();
            while (next.Length > 0)
            {
                result.Add(next);
                arr = arr.Skip(pageSize).ToArray();
                next = arr.Take(pageSize).ToArray();
            }
            return result;
        }

        public static T[] OrderChildrenFirst<T>(this IEnumerable<T> nodes, Func<T, object> key, Func<T, object> parent)
        {
            var dict = nodes.ToDictionary(n => key.Invoke(n));
            var visited = new HashSet<T>();
            var result = new List<T>();

            var parentIds = nodes.Select(r => parent(r)).ToArray();

            void Visit(T n)
            {
                if (!visited.Add(n))
                {
                    return;
                }

                var nKey = key(n);

                // find children
                var children = nodes.Where(c => parent(c).IsSame(nKey)).ToArray();
                foreach (var child in children)
                {
                    Visit(child);
                }

                // add parent after children
                result.Add(n);
            }

            foreach (var node in nodes)
            {
                Visit(node);
            }

            return result.ToArray();
        }
    }
}
