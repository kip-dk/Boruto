using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class MoreThanOnPlublicConstructorFoundException : BaseException
    {
        public MoreThanOnPlublicConstructorFoundException(Type type): base($"{ type.FullName } has more than one constructor with multi parameters. Reduce the number of public constructors with parameters to one only")
        {

        }
    }
}
