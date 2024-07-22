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
    }

    public interface ITargetReference<T> where T: Microsoft.Xrm.Sdk.Entity, new()
    {
    }
}
