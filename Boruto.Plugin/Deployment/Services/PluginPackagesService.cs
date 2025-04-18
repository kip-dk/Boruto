using Boruto.Deployment.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Services
{
    [Export(typeof(ServiceAPI.IPluginPackagesService))]
    public class PluginPackagesService : ServiceAPI.IPluginPackagesService
    {
        private readonly Entities.IUnitOfWork uow;

        [ImportingConstructor]
        public PluginPackagesService(Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public pluginpackage GetPluginPackage(string name)
        {
            return (from p in this.uow.PluginPackages.GetQuery()
                    where p.UniqueName == name
                    select p).SingleOrDefault();
        }

        public Guid Create(string display, string name, string version, byte[] nugetpackage)
        {
            if (string.IsNullOrEmpty(display))
            {
                display = name;
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"You must provide a unique name for the plugin package to be created");
            }

            var clean = new Entities.pluginpackage
            {
                pluginpackageId = Guid.NewGuid(),
                UniqueName = name,
                Content = System.Convert.ToBase64String(nugetpackage),
                Version = version,
                name = name
            };
            uow.Create(clean);

            return clean.pluginpackageId.Value;
        }

        public void Update(Guid pluginPackageId, string version, byte[] nugetpackage)
        {
            var clean = new Entities.pluginpackage
            {
                pluginpackageId = pluginPackageId,
                Content = System.Convert.ToBase64String(nugetpackage),
                Version = version,
            };
            uow.Update(clean);
        }

        public void Delete(Guid packageId)
        {
            uow.Delete(new Entities.pluginpackage { pluginpackageId = packageId });
        }
    }
}
