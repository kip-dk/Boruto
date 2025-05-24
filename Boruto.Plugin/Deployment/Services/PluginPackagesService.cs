using Boruto.Deployment.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boruto.Extensions.FilterExpression;
using Boruto.Extensions.QueryExpression;

namespace Boruto.Deployment.Services
{
    internal class PluginPackagesService : ServiceAPI.IPluginPackagesService
    {
        private readonly IOrganizationService orgService;

        public PluginPackagesService(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public pluginpackage GetPluginPackage(string name)
        {
            var query = Entities.pluginpackage.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(nameof(Entities.pluginpackage.UniqueName).ToLower(), name);

            var result = this.orgService.RetrieveMultiple(query);
            if (result.Entities != null && result.Entities.Count == 1)
            {
                return new pluginpackage(result.Entities.First());
            }
            return null;
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
            this.orgService.Create(clean.ToEntity());

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
            this.orgService.Update(clean.ToEntity());
        }

        public void Delete(Guid packageId)
        {
            this.orgService.Delete(Entities.pluginpackage.EntityLogicalName, packageId );
        }
    }
}
