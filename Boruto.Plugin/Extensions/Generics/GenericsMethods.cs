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
    }
}
