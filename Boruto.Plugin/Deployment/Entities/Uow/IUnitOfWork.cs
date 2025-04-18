using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Entities
{
    public interface IUnitOfWork
    {
        IRepository<Entities.Solution> Solutions { get; }
        IRepository<Entities.SolutionComponent> SolutionComponents { get; }
        IRepository<Entities.SdkMessage> SdkMessages { get; }
        IRepository<Entities.SdkMessageFilter> SdkMessageFilters { get; }
        IRepository<Entities.PluginAssembly> PluginAssemblies { get; }
        IRepository<Entities.pluginpackage> PluginPackages { get; }
        IRepository<Entities.PluginType> PluginTypes { get; }
        IRepository<Entities.SdkMessageProcessingStep> SdkMessageProcessingSteps { get; }
        IRepository<Entities.SdkMessageProcessingStepImage> SdkMessageProcessingStepImages { get; }
        IRepository<Entities.Publisher> Publishers { get; }

        Guid Create(Microsoft.Xrm.Sdk.Entity entity);
        void Update(Microsoft.Xrm.Sdk.Entity entity);
        void Delete(Microsoft.Xrm.Sdk.Entity entity);
        Microsoft.Xrm.Sdk.OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request);
    }
}
