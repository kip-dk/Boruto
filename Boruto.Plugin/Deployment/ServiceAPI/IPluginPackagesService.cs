using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.ServiceAPI
{
    internal interface IPluginPackagesService
    {
        Entities.pluginpackage GetPluginPackage(string name);
        Guid Create(string displayName, string uniqueName, string version, byte[] nugetpackage);
        void Update(Guid pluginPackageId, string version, byte[] nugetpackage);
        void Delete(Guid packageId);
    }
}
