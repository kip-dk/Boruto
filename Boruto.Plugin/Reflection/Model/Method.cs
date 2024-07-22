using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Reflection.Model
{
    internal class Method
    {
        private readonly Type pluginType;
        internal MethodInfo method { get; }
        private Argument[] arguments;
        private Attributes.IfAttribute[] ifTypes;

        internal Method(Type pluginType, System.Reflection.MethodInfo method, string primaryLogicalName)
        {
            this.pluginType = pluginType;
            this.method = method;
            this.LogicalName = primaryLogicalName;
            this.Resolve();
            this.ResolveIf();
        }

        internal string LogicalName { get; }
        internal Argument[] Arguments => this.arguments;

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
            var result = new List<Argument>();
            var pms = this.method.GetParameters();
            foreach (var pm in pms)
            {
                var next = new Argument(this.pluginType, this.method, pm, this.LogicalName);

                if (this.LogicalName != null && next.LogicalName != null && next.LogicalName != this.LogicalName)
                {
                    throw new Exceptions.InconsistentEntityParameterException(this.pluginType, this.method, this.LogicalName, next.LogicalName);
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
