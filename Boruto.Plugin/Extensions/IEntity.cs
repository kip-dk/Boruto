using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Extensions
{
    public interface IEntity
    {
        Guid Id { get;}
        string LogicalName { get; }
    }
}
