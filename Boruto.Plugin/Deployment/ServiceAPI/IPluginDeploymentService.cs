using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.ServiceAPI
{
    public interface IPluginDeploymentService
    {
        Models.Plugin[] ForAssembly(System.Reflection.Assembly[] assembly);

    }
}
