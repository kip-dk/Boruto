using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Entities
{
    [Export(typeof(IUnitOfWork))]
    public class UnitOfWork : IUnitOfWork
    {
        private ServiceContext ctx;

        [ImportingConstructor]
        public UnitOfWork(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {

            this.ctx = new ServiceContext(orgService);
            this.orgService = orgService;
        }

        public IRepository<Solution> Solutions => this.Get<Solution>();
        public IRepository<SolutionComponent> SolutionComponents => this.Get<SolutionComponent>();
        public IRepository<SdkMessage> SdkMessages => this.Get<SdkMessage>();
        public IRepository<SdkMessageFilter> SdkMessageFilters => this.Get<SdkMessageFilter>();
        public IRepository<PluginAssembly> PluginAssemblies => this.Get<PluginAssembly>();
        public IRepository<pluginpackage> PluginPackages => this.Get<pluginpackage>();
        public IRepository<PluginType> PluginTypes => this.Get<PluginType>();
        public IRepository<SdkMessageProcessingStep> SdkMessageProcessingSteps => this.Get<SdkMessageProcessingStep>();
        public IRepository<SdkMessageProcessingStepImage> SdkMessageProcessingStepImages => this.Get<SdkMessageProcessingStepImage>();
        public IRepository<Publisher> Publishers => this.Get<Publisher>();

        private Dictionary<Type, object> index = new Dictionary<Type, object>();
        private readonly IOrganizationService orgService;

        private IRepository<T> Get<T>() where T: Microsoft.Xrm.Sdk.Entity, new()
        {
            if (index.TryGetValue(typeof(T), out object v))
            {
                return (IRepository<T>) v; 
            }

            index[typeof(T)] = new Repository<T>(this.ctx);
            return (IRepository<T>)index[typeof(T)];
        }

        public Guid Create(Microsoft.Xrm.Sdk.Entity entity)
        {
            return this.orgService.Create(entity);
        }
        public void Update(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.orgService.Update(entity);
        }
        public void Delete(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.orgService.Delete(entity.LogicalName, entity.Id);
        }

        public Microsoft.Xrm.Sdk.OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request)
        {
            return this.orgService.Execute(request);
        }
    }
}
