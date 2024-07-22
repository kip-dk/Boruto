using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Exceptions
{
    public class TypeNotEntityType : BaseException
    {
        public TypeNotEntityType(Type type) : base($"Type: { type.FullName } does not extens Microsoft.Xrm.Sdk.Entity")
        {
        }
    }
}
