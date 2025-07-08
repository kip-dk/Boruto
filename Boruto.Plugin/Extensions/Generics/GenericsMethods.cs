using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
