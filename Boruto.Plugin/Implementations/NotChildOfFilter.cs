using Boruto.Attributes;
using Boruto.Extensions.SDK;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Implementations
{
    internal class NotChildOfFilter : IMethodCondition
    {
        public bool Execute(IfAttribute ifAttr, IPluginExecutionContext currentCtx)
        {
            if (ifAttr is Attributes.Filter.NotChildOfAttribute coa)
            {
                if (currentCtx.Depth == 1)
                {
                    return true;
                }

                if (currentCtx.ParentContext == null)
                {
                    return true;
                }

                var ctx = currentCtx.ParentContext;

                Microsoft.Xrm.Sdk.EntityReference refId = null;
                if (coa.ReferenceAttributeName != null)
                {
                    var field = coa.ReferenceAttributeName.ToLower();
                    if (ctx.InputParameters.Contains("Target") && ctx.InputParameters["Target"] is Entity target && target.Attributes.ContainsKey(field))
                    {
                        refId = (Microsoft.Xrm.Sdk.EntityReference)target[field];
                    }
                    else
                    {
                        refId = ctx.PreValueOf<Microsoft.Xrm.Sdk.EntityReference>(field);
                    }

                    if (refId == null)
                    {
                        return true;
                    }

                    return !ctx.IsChildOf(coa.Message, coa.EntityLogicalName, refId.Id);
                }

                return !ctx.IsChildOf(coa.Message, coa.EntityLogicalName, null);
            }

            throw new InvalidPluginExecutionException($"Unexpected type of ifAttr {ifAttr.GetType().FullName}, expected NotChildOfAttribute");
        }
    }
}
