using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class MissingPublicConstructorException : BaseException
    {
        public MissingPublicConstructorException(Type type): base($"{ type.FullName } is missing a public constructor")
        {

        }
    }
}
