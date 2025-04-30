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
        bool IsAdmin { get; }
        string Message { get; }
        T  Target<T>() where T: Microsoft.Xrm.Sdk.Entity;
        T Preimage<T>() where T : Microsoft.Xrm.Sdk.Entity;
        T PostImage<T>() where T : Microsoft.Xrm.Sdk.Entity;
        T MergedImage<T>() where T : Microsoft.Xrm.Sdk.Entity;
        T OrganizationRequest<T>() where T: Microsoft.Xrm.Sdk.OrganizationRequest, new();
    }
}
