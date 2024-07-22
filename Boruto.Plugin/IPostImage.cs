using Boruto.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    public interface IPostImage : IEntity { }

    public interface IPostImage<T> : IPostImage where T: Microsoft.Xrm.Sdk.Entity  { }
}
