using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.ServiceAPI
{
    public interface IPluginAssemblyService
    {
        System.Reflection.Assembly Assembly { get; }
        Entities.PluginAssembly FindOrCreate(string assemblyfilename);
        void UploadAssembly();
        Entities.PluginAssembly GetPluginAssembly(string name);
        Entities.PluginAssembly[] ForPackage(Guid pluginPackageId);
    }
}
