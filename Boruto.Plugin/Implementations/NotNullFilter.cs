using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Implementations
{
    internal class NotNullFilter : IMethodCondition
    {
        public bool Execute(Attributes.IfAttribute ifAttr, IPluginExecutionContext ctx)
        {
            if (ifAttr is Attributes.Filter.NotNullAttribute nn)
            {
                if (nn.Attributes != null && nn.Attributes.Length > 0)
                {
                    var target = ctx.InputParameters["Target"] as Entity;
                    if (target != null)
                    {
                        foreach (var att in nn.Attributes.Select(r => r.ToLower()))
                        {
                            if (target.Attributes.ContainsKey(att) && target[att] != null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
