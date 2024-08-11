using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.ServiceAPI
{
    public interface INamingService
    {
        string NameOf(Microsoft.Xrm.Sdk.EntityReference re);
    }
}
