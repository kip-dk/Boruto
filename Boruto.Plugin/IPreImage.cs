using Boruto.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    public interface IPreImage : IEntity { }
    public interface IPreImage<T> : IPreImage where T : Microsoft.Xrm.Sdk.Entity, new() { }
}
