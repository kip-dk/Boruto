using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class NamingConventionViolationException : BaseException
    {
        public NamingConventionViolationException(string usedName, params string[] expectedNames) : base($"Name: { usedName } is not valid in this context, expected one of the following: {string.Join(",", expectedNames)}")
        {
        }
    }
}
