using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    /// <summary>
    /// Geneal properties for all objects devired from entity payload. 
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; }
        string LogicalName { get; }
        Microsoft.Xrm.Sdk.AttributeCollection Attributes { get; set; }
    }
}
