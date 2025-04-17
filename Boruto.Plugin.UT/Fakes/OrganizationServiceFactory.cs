using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Fakes
{
    public class OrganizationServiceFactory : Microsoft.Xrm.Sdk.IOrganizationServiceFactory
    {
        private readonly IOrganizationService orgService;

        public OrganizationServiceFactory(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.orgService = orgService;
        }
        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            return this.orgService;
        }
    }
}
