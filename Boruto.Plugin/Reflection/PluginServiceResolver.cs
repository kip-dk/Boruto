using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Reflection
{
    internal class PluginServiceResolver
    {
        private readonly Type pluginType;
        private Dictionary<string, Model.PluginMethod[]> methodIndex = new Dictionary<string, Model.PluginMethod[]>();
        private Assembly[] assemblies;

        internal PluginServiceResolver(Type pluginType, Assembly[] assemblies)
        {
            this.pluginType = pluginType;
            this.assemblies = assemblies;
        }

        internal Model.PluginMethod[] GetMethods(string pattern, string primaryLogicalName)
        {
            var key = this.Key(pattern, primaryLogicalName);
            if (this.methodIndex.TryGetValue(key, out Model.PluginMethod[] ms))
            {
                return ms;
            }
            return this.ResolveMethods(pattern, primaryLogicalName);
        }

        private Model.PluginMethod[] ResolveMethods(string pattern, string primaryLogicalName)
        {
            var result = new List<Model.PluginMethod>();

            var methods = this.pluginType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(r => r.Name == pattern).ToArray();

            foreach (var method in methods)
            {
                var next = new Model.PluginMethod(this.pluginType, method, primaryLogicalName, this.assemblies);

                if (next.IsMatch)
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
