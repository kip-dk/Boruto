using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class CyclicalDependencyException : BaseException
    {
        public CyclicalDependencyException(Type type, Type[] types) : base($"Cyclical Dependency Exception found for: { type.FullName}: { string.Join(" => ", types.Select(r => r.FullName)) }")
        {
        }
    }
}
