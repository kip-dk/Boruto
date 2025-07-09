using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Attributes.Filter
{
    public class NotNullAttribute : IfAttribute
    {
        public string[] Attributes { get; private set; }
        public NotNullAttribute(params string[] attributes) : base(typeof(Implementations.NotNullFilter))
        {
            this.Attributes = attributes;
        }
    }
}
