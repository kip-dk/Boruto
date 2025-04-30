using Boruto.Deployment.Entities;
using Boruto.Deployment.ServiceAPI;
using Boruto.Extensions.FilterExpression;
using Boruto.Extensions.QueryExpression;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Services
{
    [Export(typeof(ServiceAPI.ISdkMessageProcessingStepService))]
    internal class SdkMessageProcessingStepService : ServiceAPI.ISdkMessageProcessingStepService
    {
        private ServiceAPI.IMessageService messageService;
        private readonly ISolutionService solutionService;
        private readonly IOrganizationService orgService;
        private const string IMAGE_NAME = "BorutoImage";

        [ImportingConstructor]
        public SdkMessageProcessingStepService(
            ServiceAPI.IMessageService messageService, 
            ServiceAPI.ISolutionService solutionService,
            Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.messageService = messageService;
            this.solutionService = solutionService;
            this.orgService = orgService;
        }

        public SdkMessageProcessingStep[] ForPluginAssembly(Guid pluginassemblyid)
        {
            var query = Entities.SdkMessageProcessingStep.EntityLogicalName.ToQueryExpression();
            var link = query.Inner("pt", Entities.PluginType.EntityLogicalName, Entities.SdkMessageProcessingStep.Fields.EventHandler, Entities.PluginType.Fields.PluginTypeId);
            link.LinkCriteria.Equal(Entities.PluginType.Fields.PluginAssemblyId, pluginassemblyid);

            return this.orgService.RetrieveMultiple(query).Entities.Select(r => new SdkMessageProcessingStep(r)).ToArray();


        }


        public Entities.SdkMessageProcessingStep[] Cleanup(Entities.SdkMessageProcessingStep[] steps, Models.Plugin[] plugins)
        {
            List<Entities.SdkMessageProcessingStep> remains = new List<SdkMessageProcessingStep>();

            foreach (var plugin in plugins)
            {
                if (plugin.CurrentCrmInstance != null)
                {
                    foreach (var step in plugin.Steps)
                    {
                        var name = plugin.NameOf(step.Stage, step.Message, step.IsAsync, step.PrimaryEntityLogicalName);
                        var exists = (from s in steps
                                      where s.Name == name
                                      select s).ToArray();

                        if (exists.Length > 0)
                        {
                            remains.Add(exists[0]);
                        }
                    }
                }
            }

            var removes = (from s in steps where !remains.Contains(s) && (int)s.Stage.Value != 30 select s).ToArray();
            foreach (var remove in removes)
            {
                this.orgService.Delete(remove.LogicalName, remove.Id);
                this.messageService.Inform($"Removed step {remove.Name} on {remove.LogicalName}.");
            }

            return remains.ToArray();
        }

        public Entities.SdkMessageProcessingStep[] CreateOrUpdateSteps(Entities.SdkMessageProcessingStep[] steps, Models.Plugin[] plugins)
        {
            var result = new List<SdkMessageProcessingStep>();

            foreach (var plugin in plugins)
            {
                if (plugin.IsVirtualEntityPlugin) continue;

                foreach (var step in plugin.Steps)
                {
                    var name = plugin.NameOf(step.Stage, step.Message, step.IsAsync, step.PrimaryEntityLogicalName);
                    var crmStep = (from s in steps where s.Name == name select s).SingleOrDefault();

                    if (crmStep == null)
                    {
                        result.Add(this.Create(plugin, step, name));
                    }
                    else
                    {
                        result.Add(crmStep);
                        this.Update(plugin, step, crmStep);
                    }
                }
            }
            return result.ToArray();
        }

        public Entities.SdkMessageProcessingStep[] ForSolution(string name)
        {
            var solutionId = this.solutionService.Get(name)?.SolutionId.Value;

            if (solutionId == null)
            {
                throw new ArgumentException($"No solution found with uniquename {name}");
            }

            var query = Entities.SdkMessageProcessingStep.EntityLogicalName.ToQueryExpression();
            query.Inner("sc", Entities.SolutionComponent.EntityLogicalName, Entities.SdkMessageProcessingStep.Fields.SdkMessageProcessingStepId, Entities.SolutionComponent.Fields.ObjectId);

            return this.orgService.RetrieveMultiple(query).Entities.Select(r => new SdkMessageProcessingStep(r)).ToArray();
        }

        public Entities.SdkMessageProcessingStepImage[] ImagesForSolution(string name)
        {
            var solution = this.solutionService.Get(name);

            if (solution == null)
            {
                throw new ArgumentException($"No solution found with uniquename {name}");
            }

            var query = Entities.SdkMessageProcessingStepImage.EntityLogicalName.ToQueryExpression();
            var compLink = query.Inner("sc", Entities.SolutionComponent.EntityLogicalName, Entities.SdkMessageProcessingStepImage.Fields.SdkMessageProcessingStepId, Entities.SolutionComponent.Fields.ObjectId);
            compLink.LinkCriteria.Equal(Entities.SolutionComponent.Fields.SolutionId, solution.Id);

            return this.orgService.RetrieveMultiple(query).Entities.Select(r => new SdkMessageProcessingStepImage(r)).ToArray();

        }

        public Entities.SdkMessageProcessingStepImage[] ImagesForPluginAssembly(Guid id)
        {
            var query = Entities.SdkMessageProcessingStepImage.EntityLogicalName.ToQueryExpression();
            query.Distinct = true;
            var stepLink = query.Inner("ST", Entities.SdkMessageProcessingStep.EntityLogicalName, Entities.SdkMessageProcessingStepImage.Fields.SdkMessageProcessingStepId, Entities.SdkMessageProcessingStep.Fields.SdkMessageProcessingStepId);
            var pluginType = stepLink.Inner("PT", Entities.PluginType.EntityLogicalName, Entities.SdkMessageProcessingStep.Fields.EventExpander, Entities.PluginType.Fields.PluginTypeId);
            pluginType.LinkCriteria.Equal(Entities.PluginType.Fields.PluginAssemblyId, id);

            return this.orgService.RetrieveMultiple(query).Entities.Select(r => new Entities.SdkMessageProcessingStepImage(r)).ToArray();
        }

        public Entities.SdkMessageFilter[] FiltersForAssembly(Guid id)
        {
            var query = Entities.SdkMessageFilter.EntityLogicalName.ToQueryExpression();
            var stepLink = query.Inner("ST", Entities.SdkMessageProcessingStep.EntityLogicalName, Entities.SdkMessageFilter.Fields.SdkMessageFilterId, Entities.SdkMessageProcessingStep.Fields.SdkMessageFilterId);
            var typeLink = stepLink.Inner("TY", Entities.PluginType.EntityLogicalName, SdkMessageProcessingStep.Fields.EventHandler, Entities.PluginType.Fields.PluginTypeId);
            typeLink.LinkCriteria.Equal(Entities.PluginType.Fields.PluginAssemblyId, id);

            return this.orgService.RetrieveMultiple(query).Entities.Select(r => new SdkMessageFilter(r)).ToArray();
        }


        private void Update(Models.Plugin plugin, Models.Step step, Entities.SdkMessageProcessingStep crmStep)
        {
            var updated = false;
            var clean = new Entities.SdkMessageProcessingStep
            {
                SdkMessageProcessingStepId = crmStep.SdkMessageProcessingStepId
            };

            if (crmStep.FilteringAttributes != null && step.FilteringAttributesString == null)
            {
                clean.FilteringAttributes = null;
                updated = true;
            }

            if (step.FilteringAttributesString != null && step.FilteringAttributesString != crmStep.FilteringAttributes)
            {
                clean.FilteringAttributes = step.FilteringAttributesString;
                updated = true;
            }

            if (updated)
            {
                this.orgService.Update(clean.ToEntity());
                this.messageService.Inform($"Updated step {crmStep.Name.Split('.').Last()} on {crmStep.LogicalName}.");
            }

            this.UpdateImage(crmStep, 1, step.Stage, step.IsAsync, step.Message, step.PreImage);
            this.UpdateImage(crmStep, 2, step.Stage, step.IsAsync, step.Message, step.PostImage);
        }

        private Entities.SdkMessageProcessingStep Create(Models.Plugin plugin, Models.Step step, string name)
        {
            var next = new Entities.SdkMessageProcessingStep
            {
                SdkMessageProcessingStepId = Guid.NewGuid(),
                Name = name,
                Mode = step.IsAsync ? sdkmessageprocessingstep_mode.Asynchronous : sdkmessageprocessingstep_mode.Synchronous,
                Rank = step.ExecutionOrder <= 0 ? 1 : step.ExecutionOrder,
                Stage = (sdkmessageprocessingstep_stage)step.Stage,
                SupportedDeployment = sdkmessageprocessingstep_supporteddeployment.ServerOnly,
                EventHandler = new Microsoft.Xrm.Sdk.EntityReference(Entities.PluginType.EntityLogicalName, plugin.CurrentCrmInstance.PluginTypeId.Value),
                SdkMessageId = this.GetSdkMessage(step.Message).ToEntityReference(),
                SdkMessageFilterId = this.GetFilterFor(this.GetSdkMessage(step.Message), step.PrimaryEntityLogicalName)
            };

            if (next.Mode.Value == sdkmessageprocessingstep_mode.Asynchronous)
            {
                next.AsyncAutoDelete = true;
            }

            if (step.TargetFilterAttributes != null && !step.TargetFilterAttributes.AllAttributes)
            {
                next.FilteringAttributes = string.Join(",", step.TargetFilterAttributes.FilteredAttributes);
            }

            this.orgService.Create(next.ToEntity());
            this.messageService.Inform($"Created step: {next.Name.Split('.').Last()}");

            if (step.PreImage != null)
            {
                CreateImage(next, 1, step.Stage, step.IsAsync, step.Message, step.PreImage);
            }

            if (step.PostImage != null)
            {
                CreateImage(next, 2, step.Stage, step.IsAsync, step.Message, step.PostImage);
            }
            return next;
        }

        private string ImageName(int pre1post2)
        {
            return $"{IMAGE_NAME}-{pre1post2}";
        }
        private void UpdateImage(SdkMessageProcessingStep crmStep, int pre1post2, int stage, bool async, string message, Models.Image imgDef)
        {
            var query = Entities.SdkMessageProcessingStep.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(Entities.SdkMessageProcessingStepImage.Fields.Name, this.ImageName(pre1post2));

            var existingImage = this.orgService.RetrieveMultiple(query).Entities.Select(r => new Entities.SdkMessageProcessingStepImage(r)).SingleOrDefault();


            if (imgDef == null)
            {
                if (existingImage != null)
                {
                    this.orgService.Delete(existingImage.LogicalName, existingImage.Id);
                    this.messageService.Inform($"Removed images {IMAGE_NAME} from {crmStep.Name.Split('.').Last()}");
                }
                return;
            }

            if (existingImage == null)
            {
                this.CreateImage(crmStep, pre1post2, stage, async, message, imgDef);
            }
            else
            {
                string filterAttr = null;
                if (!imgDef.AllAttributes && imgDef.FilteredAttributes != null && imgDef.FilteredAttributes.Length > 0)
                {
                    filterAttr = string.Join(",", imgDef.FilteredAttributes);
                }

                if (existingImage.Attributes1 == null && filterAttr == null)
                {
                    return;
                }

                if (existingImage.Attributes1 != null && filterAttr == null)
                {
                    this.orgService.Delete(existingImage.LogicalName, existingImage.Id);
                    this.CreateImage(crmStep, pre1post2, stage, async, message, imgDef);
                    return;
                }

                if (existingImage.Attributes1 != filterAttr)
                {
                    this.orgService.Delete(existingImage.LogicalName, existingImage.Id);
                    this.CreateImage(crmStep, pre1post2, stage, async, message, imgDef);
                    return;
                }
            }
        }

        private void CreateImage(SdkMessageProcessingStep crmStep, int pre1post2, int stage, bool async, string message, Models.Image imgDef)
        {
            var image = new SdkMessageProcessingStepImage
            {
                SdkMessageProcessingStepImageId = Guid.NewGuid(),
                SdkMessageProcessingStepId = new Microsoft.Xrm.Sdk.EntityReference(Entities.SdkMessageProcessingStep.EntityLogicalName, crmStep.SdkMessageProcessingStepId.Value),
                Name = $"{IMAGE_NAME}-{pre1post2}",
                EntityAlias = this.ImageName(pre1post2),
                Description = "Image generated by Boruto plugin deployment tool",
                ImageType = (sdkmessageprocessingstepimage_imagetype)(pre1post2 - 1),
                MessagePropertyName = this.MessagePropertyName(message),
            };

            string filterAttr = null;
            if (!imgDef.AllAttributes && imgDef.FilteredAttributes != null && imgDef.FilteredAttributes.Length > 0)
            {
                filterAttr = string.Join(",", imgDef.FilteredAttributes);
            }

            if (!string.IsNullOrEmpty(filterAttr))
            {
                image.Attributes1 = filterAttr;
            }

            this.orgService.Create(image.ToEntity());
            messageService.Inform($"Created image {this.ImageName(pre1post2)} on step {crmStep.Name.Split('.').Last()}");
        }

        private Dictionary<string, SdkMessage> sdkmessages;
        private Entities.SdkMessage GetSdkMessage(string message)
        {
            if (this.sdkmessages == null)
            {
                var query = Entities.SdkMessage.EntityLogicalName.ToQueryExpression();
                this.sdkmessages = this.orgService.RetrieveMultiple(query).Entities.Select(r => new SdkMessage(r)).ToDictionary(r => r.Name);
            }
            return this.sdkmessages[message];
        }

        private Dictionary<string, SdkMessageFilter> filters = new Dictionary<string, SdkMessageFilter>();
        public Microsoft.Xrm.Sdk.EntityReference GetFilterFor(SdkMessage sdkMessage, string logicalname)
        {
            var key = $"{sdkMessage.SdkMessageId.Value.ToString()}.{logicalname}";

            if (filters.ContainsKey(key))
            {
                var v = filters[key];
                if (v != null)
                {
                    return v.ToEntityReference();
                }
                else
                {
                    return null;
                }
            }

            var query = Entities.SdkMessageFilter.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(Entities.SdkMessageFilter.Fields.SdkMessageId, sdkMessage.SdkMessageId);

            if (!string.IsNullOrEmpty(logicalname))
            {
                query.Criteria.Equal(Entities.SdkMessageFilter.Fields.PrimaryObjectTypeCode, logicalname);
            } else
            {
                query.Criteria.IsNull(Entities.SdkMessageFilter.Fields.PrimaryObjectTypeCode);

            }

            filters[key] = this.orgService.RetrieveMultiple(query).Entities.Select(r => new Entities.SdkMessageFilter(r)).SingleOrDefault();

            return GetFilterFor(sdkMessage, logicalname);
        }


        private string MessagePropertyName(string message)
        {
            switch (message)
            {
                case "Associate":
                case "Disassociate":
                case "SetState":
                case "SetStateDynamicEntity":
                case "Close":
                    return "EntityMoniker";
                case "Delete":
                case "Update":
                    return "Target";
                case "Create":
                    return "Id";
                default: throw new ArgumentException("MessagePropertyName has not been maped for " + message);
            }
        }

    }

}
