using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Reflection.Model
{
    internal class PluginMethod
    {
        private readonly Type pluginType;
        internal MethodInfo method { get; }
        private PluginMethodArgument[] arguments;

        internal bool IsMatch { get; private set; }
        private Attributes.IfAttribute[] ifTypes;

        Assembly[] assemblies;

        internal PluginMethod(Type pluginType, System.Reflection.MethodInfo method, string primaryLogicalName, Assembly[] assemblies)
        {
            this.pluginType = pluginType;
            this.method = method;
            this.IsMatch = true;
            this.LogicalName = primaryLogicalName;
            this.assemblies = assemblies;
            this.Resolve();
            this.ResolveIf();
        }

        internal string LogicalName { get; }
        internal PluginMethodArgument[] Arguments => this.arguments;

        private bool? _allTargetFilter;
        internal bool AllTargetFilter
        {
            get
            {
                if (this._allTargetFilter == null)
                {
                    this._allTargetFilter = false;
                    foreach (var arg in this.arguments)
                    {
                        if (arg.FilteredAllAttributes == true)
                        {
                            this._allTargetFilter = true;
                            break;
                        }
                    }
                }
                return this._allTargetFilter.Value;
            }
        }

        private string[] _targetFilter;
        internal string[] TargetFilter
        {
            get
            {
                if (this.AllTargetFilter == true)
                {
                    return null;
                }

                if (this._targetFilter == null)
                {
                    _targetFilter = this.arguments.Where(r => r.FilteredAttributes != null).SelectMany(r => r.FilteredAttributes).Distinct().ToArray();
                }

                return this._targetFilter;
            }
        }


        private int? _sort;
        internal int Sort
        {
            get
            {
                if (_sort == null)
                {
                    _sort = 9999;
                    var sortAttr = this.method.GetCustomAttribute<Attributes.SortAttribute>();
                    if (sortAttr != null)
                    {
                        this._sort = sortAttr.Value;
                    }
                }
                return _sort.Value;
            }
        }

        internal bool IsRelevant(Microsoft.Xrm.Sdk.IPluginExecutionContext ctx)
        {
            if (this.ifTypes != null && this.ifTypes.Length > 0)
            {
                foreach (var ifType in this.ifTypes)
                {
                    var next = Implementations.IfEvaluaterService.Evaluate(ifType, ctx);
                    if (next == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void Resolve()
        {
            var result = new List<PluginMethodArgument>();
            var pms = this.method.GetParameters();
            foreach (var pm in pms)
            {
                var next = new PluginMethodArgument(this.pluginType, this.method, pm, this.LogicalName, this.assemblies);


                if (next.IsEntityMatch != null && next.IsEntityMatch == false)
                {
                    this.IsMatch = false;
                    return;
                }
                result.Add(next);
            }
            this.arguments = result.ToArray();
        }

        private void ResolveIf()
        {
            var result = new List<Attributes.IfAttribute>();
            var attrs = this.method.GetCustomAttributes();

            foreach (var att in attrs)
            {
                if (att is Attributes.IfAttribute ifAtt)
                {
                    result.Add(ifAtt);
                }
            }
            this.ifTypes = result.ToArray();
        }
    }
}
