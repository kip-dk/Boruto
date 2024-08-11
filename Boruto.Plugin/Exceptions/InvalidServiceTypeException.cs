using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class InvalidServiceTypeException : BaseException
    {
        public InvalidServiceTypeException(Type type) : base($"Interfaces and Abstract classes cannot be created as Service, and the service must have a single public constructor")
        {
        }

        public InvalidServiceTypeException(string message) : base(message)
        {
        }
    }
}
