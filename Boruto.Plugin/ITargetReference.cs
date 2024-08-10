using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    public interface ITargetReference
    {
        Guid Id { get; }
        string LogicalName { get; }
        string Name { get; }
    }

    public interface ITargetReference<T> : ITargetReference where T: Microsoft.Xrm.Sdk.Entity, new()
    {
    }
}
