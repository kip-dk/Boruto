using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class UnresolveableEntityTypeException : BaseException
    {
        public UnresolveableEntityTypeException(Type type, string source) : base($"{ type.FullName } could not be resolved to an strongly typed entity with a default constructor, { source }")
        {
        }

        public UnresolveableEntityTypeException(string logicalname) : base($"{ logicalname } could not be resolved to an strongly typed entity with a default constructor")
        {
        }
    }
}
