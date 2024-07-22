using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class MissingDefaultConstructorException : BaseException
    {
        public MissingDefaultConstructorException(Type type) : base($"Type: { type.FullName } is missing default constructor")
        {
        }
    }
}
