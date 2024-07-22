using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Reflection
{
    internal class PluginServiceResolver
    {
        private readonly Type pluginType;
        private Dictionary<string, Model.Method[]> methodIndex = new Dictionary<string, Model.Method[]>(); 

        internal PluginServiceResolver(Type pluginType)
        {
            this.pluginType = pluginType;
        }

        internal Model.Method[] GetMethods(string pattern, string primaryLogicalName)
        {
            var key = this.Key(pattern, primaryLogicalName);
            if (this.methodIndex.TryGetValue(key, out Model.Method[] ms))
            {
                return ms;
            }
            return this.ResolveMethods(key, primaryLogicalName);
        }

        private Model.Method[] ResolveMethods(string pattern, string primaryLogicalName)
        {
            var result = new List<Model.Method>();

            var methods = this.pluginType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(r => r.Name == pattern).ToArray();

            foreach (var method in methods)
            {
                var next = new Model.Method(this.pluginType, method, primaryLogicalName);

                if (primaryLogicalName == null || primaryLogicalName == next.LogicalName)
                {
                    result.Add(next);
                }
            }

            var key = this.Key(pattern, primaryLogicalName);
            this.methodIndex[key] = result.OrderBy(r => r.Sort).ToArray();
            return this.methodIndex[key];
        }

        private string Key(string pattern, string primaryLogicalName)
        {
            if (string.IsNullOrEmpty(primaryLogicalName))
            {
                return pattern;
            }
            return $"{pattern}:{primaryLogicalName}";
        }
    }
}
