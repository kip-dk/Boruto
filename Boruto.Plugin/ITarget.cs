using Boruto.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Boruto
{
    public interface ITarget : IEntity { }
    public interface ITarget<T> : ITarget where T : Microsoft.Xrm.Sdk.Entity, new() { }
} 
