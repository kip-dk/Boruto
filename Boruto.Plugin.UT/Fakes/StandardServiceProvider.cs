using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Fakes
{
    public class StandardServiceProvider : IServiceProvider, IDisposable
    {
        private Microsoft.Xrm.Sdk.ITracingService traceService = new TraceService();
        private Microsoft.Xrm.Sdk.IPluginExecutionContext pluginExecutionContext;
        private Microsoft.Xrm.Sdk.IOrganizationService organizationService = new OrganizationService();
        private Microsoft.Xrm.Sdk.IOrganizationServiceFactory factoryService;
        public StandardServiceProvider(int stage, bool async, string message, string logicalname, Guid? id)
        {
            this.pluginExecutionContext = new PluginExecutionContext(stage, async, message, logicalname, id);
            this.factoryService = new OrganizationServiceFactory(organizationService);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(Microsoft.Xrm.Sdk.ITracingService))
            {
                return this.traceService;
            }

            if (serviceType == typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext))
            {
                return this.pluginExecutionContext;
            }

            if (serviceType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
            {
                return this.organizationService;
            }

            if (serviceType == typeof(Microsoft.Xrm.Sdk.IOrganizationServiceFactory))
            {
                return this.factoryService;
            }
            return null;
        }

        public void Dispose()
        {
        }
    }
}
