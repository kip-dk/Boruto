using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Implementations
{
    internal class TargetReference : ITargetReference
    {
        internal TargetReference(Microsoft.Xrm.Sdk.EntityReference re)
        {
            this.Id = re.Id;
            this.LogicalName = re.LogicalName;
        }

        public Guid Id { get; private set; }

        public string LogicalName { get; private set; }

        internal static object CreateInstance(Microsoft.Xrm.Sdk.EntityReference re, Type type)
        {
            Type resultType = typeof(TargetReference<>).MakeGenericType(type);
            return Activator.CreateInstance(resultType, re.LogicalName, re.Id);
        }
    }

    internal class TargetReference<T> : ITargetReference<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        internal TargetReference(string logicalName, Guid id)
        {
            LogicalName = logicalName;
            Id = id;
        }

        public string LogicalName { get; }
        public Guid Id { get; }
    }
}
