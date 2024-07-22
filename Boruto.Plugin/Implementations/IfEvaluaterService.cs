using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Implementations
{
    internal static class IfEvaluaterService
    {
        private static Dictionary<Attributes.IfAttribute, IMethodCondition> conditions = new Dictionary<Attributes.IfAttribute, IMethodCondition>();

        internal static bool Evaluate(Attributes.IfAttribute ifType, Microsoft.Xrm.Sdk.IPluginExecutionContext ctx)
        {
            var con = Get(ifType);
            return con.Execute(ifType, ctx);
        }

        private static IMethodCondition Get(Attributes.IfAttribute type)
        {
            if (conditions.TryGetValue(type, out IMethodCondition me))
            {
                return me;
            }

            try
            {
                conditions[type] = (IMethodCondition)System.Activator.CreateInstance(type.Type);
                return conditions[type];
            }
            catch (Exception)
            {
                throw new Exceptions.MissingDefaultConstructorException(type.Type);
            }
        }
    }
}
