using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Implementations
{
    internal class RelationshipMethodCondition : IMethodCondition
    {

        public bool Execute(Attributes.IfAttribute ifAttr, IPluginExecutionContext ctx)
        {
            if (ifAttr is Attributes.RelationshipAttribute r)
            {
                var rel = ctx.InputParameters["Relationship"] as Microsoft.Xrm.Sdk.Relationship;
                if (rel != null)
                {
                    return rel.SchemaName == r.SchemaName;
                }
            }
            return false;
        }
    }
}
