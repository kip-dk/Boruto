using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class UnresolveableTypeException : BaseException
    {
        public UnresolveableTypeException(Type type) : base($"{ type.FullName } could not be resolved. No implementation with at least one public constructor was found")
        {
        }
    }
}
