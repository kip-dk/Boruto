using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RelationshipAttribute : IfAttribute
    {
        public RelationshipAttribute(string schemaName) : base(typeof(Implementations.RelationshipMethodCondition))
        {
            this.SchemaName = schemaName;
        }

        public string SchemaName { get; private set; }
    }
}
