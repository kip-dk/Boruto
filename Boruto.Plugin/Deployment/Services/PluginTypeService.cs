using Boruto.Deployment.Entities;
using Boruto.Extensions.FilterExpression;
using Boruto.Extensions.QueryExpression;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Services
{
    [Export(typeof(ServiceAPI.IPluginTypeService))]
    internal class PluginTypeService : ServiceAPI.IPluginTypeService
    {
        private readonly IOrganizationService orgService;
        private ServiceAPI.IMessageService messageService;

        [ImportingConstructor]
        public PluginTypeService(Microsoft.Xrm.Sdk.IOrganizationService orgService, ServiceAPI.IMessageService messageService)
        {
            this.orgService = orgService;
            this.messageService = messageService;
        }

        public PluginType[] ForPluginAssembly(Guid pluginAssemblyId)
        {
            var query = Entities.PluginType.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(Entities.PluginType.Fields.PluginAssemblyId, pluginAssemblyId);

            return this.orgService.RetrieveMultiple(query).Entities.Select(r => new PluginType(r)).ToArray();
        }

        public void JoinAndCleanup(Entities.PluginType[] currents, Models.Plugin[] tobee)
        {
            foreach (var current in currents)
            {
                var inTobee = (from t in tobee
                               where t.Type.FullName == current.Name
                               select t).SingleOrDefault();

                if (inTobee != null)
                {
                    inTobee.CurrentCrmInstance = current;
                    continue;
                }
                this.Delete(current);
            }
        }

        public void CreateAndJoinMissing(Guid pluginassemblyId, Models.Plugin[] tobees)
        {
            foreach (var tobee in tobees)
            {
                if (tobee.CurrentCrmInstance == null)
                {
                    var next = new Entities.PluginType
                    {
                        PluginTypeId = Guid.NewGuid(),
                        PluginAssemblyId = new Microsoft.Xrm.Sdk.EntityReference(Entities.PluginAssembly.EntityLogicalName, pluginassemblyId),
                        FriendlyName = tobee.Type.FullName,
                        Name = tobee.Type.FullName,
                        TypeName = tobee.Type.FullName,
                    };
                    this.orgService.Create(next.ToEntity());
                    tobee.CurrentCrmInstance = next;
                }
            }
        }

        public void FindAndJoinMissing(Guid pluginassemblyId, Models.Plugin[] tobees)
        {
            foreach (var tobee in tobees)
            {
                if (tobee.CurrentCrmInstance == null)
                {
                    var query = Entities.PluginType.EntityLogicalName.ToQueryExpression();
                    query.Criteria.Equal(Entities.PluginType.Fields.PluginAssemblyId, pluginassemblyId);
                    query.Criteria.Equal(Entities.PluginType.Fields.Name, tobee.Type.FullName);

                    tobee.CurrentCrmInstance = this.orgService.RetrieveMultiple(query).Entities.Select(r => new Entities.PluginType(r)).SingleOrDefault();

                    if (tobee.CurrentCrmInstance == null)
                    {
                        throw new Exception($"Plugin Type with name: {tobee.Type.FullName} was not found. That is unexpected.");
                    }
                }
            }
        }


        private void Delete(Entities.PluginType pluginType)
        {
            var query = Entities.SdkMessageProcessingStep.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(Entities.SdkMessageProcessingStep.Fields.EventHandler, pluginType.PluginTypeId);

            var steps = this.orgService.RetrieveMultiple(query).Entities.Select(r => new SdkMessageProcessingStep(r)).ToArray();

            foreach (var step in steps)
            {
                query = Entities.SdkMessageProcessingStepImage.EntityLogicalName.ToQueryExpression();
                query.Criteria.Equal(Entities.SdkMessageProcessingStepImage.Fields.SdkMessageProcessingStepId, step.SdkMessageProcessingStepId);

                var images = this.orgService.RetrieveMultiple(query).Entities.Select(r => new SdkMessageProcessingStepImage(r)).ToArray();

                foreach (var img in images)
                {
                    this.orgService.Delete(img.LogicalName, img.Id);
                }
                this.orgService.Delete(step.LogicalName, step.Id);
            }
            this.orgService.Delete(pluginType.LogicalName, pluginType.Id);

            messageService.Inform($"Removed plugin: {pluginType.Name}");
        }
    }
}
