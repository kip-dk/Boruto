using Boruto.Deployment.Entities;
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

        private Entities.IUnitOfWork uow;
        private ServiceAPI.IMessageService messageService;
        private const string IMAGE_NAME = "BorutoImage";

        [ImportingConstructor]
        public SdkMessageProcessingStepService(Entities.IUnitOfWork uow, ServiceAPI.IMessageService messageService)
        {
            this.uow = uow;
            this.messageService = messageService;
        }

        public SdkMessageProcessingStep[] ForPluginAssembly(Guid pluginassemblyid)
        {
            return (from sm in uow.SdkMessageProcessingSteps.GetQuery()
                    join pt in uow.PluginTypes.GetQuery() on sm.EventHandler.Id equals pt.PluginTypeId
                    where pt.PluginAssemblyId.Id == pluginassemblyid
                    select sm).ToArray();
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
                uow.Delete(remove);
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
            var solutionId = (from s in this.uow.Solutions.GetQuery()
                              where s.UniqueName == name
                              select s.SolutionId).SingleOrDefault();

            if (solutionId == null)
            {
                throw new ArgumentException($"No solution found with uniquename {name}");
            }

            return (from s in uow.SdkMessageProcessingSteps.GetQuery()
                    join c in uow.SolutionComponents.GetQuery() on s.SdkMessageProcessingStepId equals c.ObjectId
                    where c.SolutionId.Id == solutionId
                    select s).ToArray();
        }

        public Entities.SdkMessageProcessingStepImage[] ImagesForSolution(string name)
        {
            var solutionId = (from s in this.uow.Solutions.GetQuery()
                              where s.UniqueName == name
                              select s.SolutionId).SingleOrDefault();

            if (solutionId == null)
            {
                throw new ArgumentException($"No solution found with uniquename {name}");
            }

            return (from i in uow.SdkMessageProcessingStepImages.GetQuery()
                    join s in uow.SdkMessageProcessingSteps.GetQuery() on i.SdkMessageProcessingStepId.Id equals s.SdkMessageProcessingStepId
                    join c in uow.SolutionComponents.GetQuery() on i.SdkMessageProcessingStepId.Id equals c.ObjectId
                    select i).ToArray();
        }

        public Entities.SdkMessageProcessingStepImage[] ImagesForPluginAssembly(Guid id)
        {
            return (from i in uow.SdkMessageProcessingStepImages.GetQuery()
                    join s in uow.SdkMessageProcessingSteps.GetQuery() on i.SdkMessageProcessingStepId.Id equals s.SdkMessageProcessingStepId
                    join t in uow.PluginTypes.GetQuery() on s.EventHandler.Id equals t.PluginTypeId
                    where t.PluginAssemblyId.Id == id
                    select i).Distinct().ToArray();

        }

        public Entities.SdkMessageFilter[] FiltersForAssembly(Guid id)
        {
            return (from f in uow.SdkMessageFilters.GetQuery()
                    join s in uow.SdkMessageProcessingSteps.GetQuery() on f.SdkMessageFilterId equals s.SdkMessageFilterId.Id
                    join t in uow.PluginTypes.GetQuery() on s.EventHandler.Id equals t.PluginTypeId
                    select f).Distinct().ToArray();
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
                uow.Update(clean);
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

            uow.Create(next);
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
            var existingImage = (from ig in uow.SdkMessageProcessingStepImages.GetQuery()
                                 where ig.SdkMessageProcessingStepId.Id == crmStep.SdkMessageProcessingStepId
                                   && ig.Name == this.ImageName(pre1post2)
                                 select ig).SingleOrDefault();

            if (imgDef == null)
            {
                if (existingImage != null)
                {
                    uow.Delete(existingImage);
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
                    uow.Delete(existingImage);
                    this.CreateImage(crmStep, pre1post2, stage, async, message, imgDef);
                    return;
                }

                if (existingImage.Attributes1 != filterAttr)
                {
                    uow.Delete(existingImage);
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

            uow.Create(image);
            messageService.Inform($"Created image {this.ImageName(pre1post2)} on step {crmStep.Name.Split('.').Last()}");
        }

        private Dictionary<string, SdkMessage> sdkmessages;
        private Entities.SdkMessage GetSdkMessage(string message)
        {
            if (this.sdkmessages == null)
            {
                this.sdkmessages = (from s in uow.SdkMessages.GetQuery()
                                    select s).ToDictionary(r => r.Name);
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

            filters[key] = (from f in uow.SdkMessageFilters.GetQuery()
                            where f.SdkMessageId.Id == sdkMessage.SdkMessageId
                               && f.PrimaryObjectTypeCode == logicalname
                            select f).SingleOrDefault();

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
