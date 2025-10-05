using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        internal Model.PluginMethod[] GetMethods(string pattern, string primaryLogicalName, string message)
        {
            var key = this.Key(pattern, primaryLogicalName, message);
            if (this.methodIndex.TryGetValue(key, out Model.PluginMethod[] ms))
            {
                return ms;
            }
            return this.ResolveMethods(pattern, primaryLogicalName, message);
        }

        private Model.PluginMethod[] ResolveMethods(string pattern, string primaryLogicalName, string message)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return new Model.PluginMethod[0];
            }

            var result = new List<Model.PluginMethod>();

            var methods = this.pluginType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(r => r.Name == pattern).ToArray();

            foreach (var method in methods)
            {
                var next = new Model.PluginMethod(this.pluginType, method, primaryLogicalName, this.assemblies);

                if (next.IsOrg)
                {
                    var arg = next.Arguments.Where(r => r.IsOrganizationRequest).First();
                    var req = (Microsoft.Xrm.Sdk.OrganizationRequest)System.Activator.CreateInstance(arg.FromType);
                    if (req.RequestName == message)
                    {
                        result.Add(next);
                    }
                    continue;
                }

                if (next.IsMatch)
                {
                    result.Add(next);
                }
            }

            if (result.Count > 0)
            {
                var key = this.Key(pattern, primaryLogicalName, message);
                this.methodIndex[key] = result.OrderBy(r => r.Sort).ToArray();
                return this.methodIndex[key];
            }

            return new Model.PluginMethod[0];
        }

        private string Key(string pattern, string primaryLogicalName, string message)
        {
            if (string.IsNullOrEmpty(primaryLogicalName))
            {
                return $"{pattern}::{message}";
            }
            return $"{pattern}:{primaryLogicalName}:{message}";
        }

        private void Trace(string message)
        {
            var ctx = PluginContext.Current;

            if (ctx != null)
            {
                ctx.Trace(message);
            } else
            {
                Console.WriteLine(message);
            }
        }
    }
}
