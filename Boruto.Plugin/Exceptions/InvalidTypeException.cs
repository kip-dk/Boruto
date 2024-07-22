using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class InvalidTypeException : BaseException
    {
        public InvalidTypeException(Type source, Type expected) : base($"{ source.FullName } is not of type: { expected.FullName }")
        {
        }
    }
}
