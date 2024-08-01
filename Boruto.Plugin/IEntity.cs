using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    /// <summary>
    /// Geneal properties for all objects devired from entity payload. 
    /// Any implementation of the IEntity interface must be an extension of Microsoft.Xrm.Sdk.Entity
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; }
        string LogicalName { get; }
        Microsoft.Xrm.Sdk.AttributeCollection Attributes { get; set; }
    }
}
