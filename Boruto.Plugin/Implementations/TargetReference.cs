using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Implementations
{
    internal class TargetReference : ITargetReference
    {
        protected Microsoft.Xrm.Sdk.EntityReference re;

        internal TargetReference(Microsoft.Xrm.Sdk.EntityReference re)
        {
            this.re = re;
        }

        public Guid Id => re.Id;

        public string LogicalName => re.LogicalName;

        public string Name => re.Name;
    }

    internal class TargetReference<T> : TargetReference, ITargetReference<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        internal TargetReference(Microsoft.Xrm.Sdk.EntityReference re) : base(re)
        {
        }
    }
}
