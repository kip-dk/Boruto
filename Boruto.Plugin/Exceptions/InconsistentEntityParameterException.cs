using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class InconsistentEntityParameterException : BaseException
    {
        public InconsistentEntityParameterException(Type pluginType, System.Reflection.MethodInfo method, string primary, string other) 
            : base($"{ pluginType.FullName }.{ method.Name } has inconsistent entity parameter type: { primary } / { other }: { string.Join(",", method.GetParameters().Select(r => $"{ r.Name }:{ r.ParameterType.FullName}")) }")
        {
        }
    }
}
