using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.ServiceAPI
{
    internal interface INugetService
    {
        Models.NugetSpec GetSpec();
        Models.DLLCode[] GetLibNet64();
    }
}
