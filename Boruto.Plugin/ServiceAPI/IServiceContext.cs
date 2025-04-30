using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.ServiceAPI
{
    public interface IServiceContext
    {
        Microsoft.Xrm.Sdk.IOrganizationService UserOrganizationService { get; }
        Microsoft.Xrm.Sdk.IOrganizationService InitiatingUserOrganizationService { get; }
        Microsoft.Xrm.Sdk.IOrganizationService AdminOrganizationService { get; }
        Microsoft.Xrm.Sdk.IOrganizationServiceFactory OrganizationServiceFactory { get; }
        Microsoft.Xrm.Sdk.ITracingService TraceService { get; }
        Microsoft.Xrm.Sdk.IPluginExecutionContext PluginExecutionContext { get; }
        System.IServiceProvider SdkServiceProvider { get; }
    }
}
