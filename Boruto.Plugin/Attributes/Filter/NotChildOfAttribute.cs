using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Attributes.Filter
{
    public class NotChildOfAttribute : IfAttribute
    {
        public NotChildOfAttribute(string message, string entityLogicalName = null, string referenceAttributeName = null) : base(typeof(Implementations.NotChildOfFilter))
        {
            Message = message;
            EntityLogicalName = entityLogicalName;
            ReferenceAttributeName = referenceAttributeName;
        }

        public string Message { get; }
        public string EntityLogicalName { get; }
        public string ReferenceAttributeName { get; }
    }
}
