using Boruto.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    public interface IMerged : IEntity { }
    public interface IMerged<T> : IMerged where T : Microsoft.Xrm.Sdk.Entity { }
}
