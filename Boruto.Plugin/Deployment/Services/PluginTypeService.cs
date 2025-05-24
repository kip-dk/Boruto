using Boruto.Deployment.Entities;
using Boruto.Deployment.Models;
using Boruto.Extensions.FilterExpression;
using Boruto.Extensions.QueryExpression;
using Microsoft.Win32;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Boruto.Deployment.Services
{
    internal class PluginTypeService : ServiceAPI.IPluginTypeService
    {
        private readonly IOrganizationService orgService;
        private ServiceAPI.IMessageService messageService;

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

                var imChanged = this.ImagesChanged(current, inTobee);

                if (inTobee != null && !imChanged)
                {
                    inTobee.CurrentCrmInstance = current;
                    continue;
                }

                if (!imChanged)
                {
                    this.Delete(current, false);
                }
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

        private bool ImagesChanged(Entities.PluginType pluginType, Models.Plugin tobee)
        {
            if (tobee == null)
            {
                return false;
            }

            var query = Entities.SdkMessageProcessingStep.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(Entities.SdkMessageProcessingStep.Fields.EventHandler, pluginType.PluginTypeId);

            var crmSteps = this.orgService.RetrieveMultiple(query).Entities.Select(r => new Entities.SdkMessageProcessingStep(r)).ToArray();

            if (crmSteps.Length == 0)
            {
                return false;
            }

            var deletedSteps = 0;

            foreach (var nextStep in tobee.Steps)
            {
                if (nextStep.Message != "Create" && nextStep.Message != "Update" && nextStep.Message != "Delete")
                {
                    continue;
                }

                var name = tobee.NameOf(nextStep.Stage, nextStep.Message, nextStep.IsAsync, nextStep.PrimaryEntityLogicalName);

                var nsl = crmSteps.Where(r =>  r.Name == name).ToArray();
                if (nsl.Length == 0)
                {
                    // it is a new step, no worry abount images
                    continue;
                }

                var crmStep = nsl[0];

                query = Entities.SdkMessageProcessingStepImage.EntityLogicalName.ToQueryExpression();
                query.Criteria.Equal(Entities.SdkMessageProcessingStepImage.Fields.SdkMessageProcessingStepId, crmStep.SdkMessageProcessingStepId);
                var images = this.orgService.RetrieveMultiple(query).Entities.Select(r => new SdkMessageProcessingStepImage(r)).ToArray();

                if (images.Length == 0 && nextStep.PreImage == null && nextStep.PostImage == null)
                {
                    // no worry, no images in current setup, no images in new setup
                    continue;
                }

                if (images.Length == 0 && (nextStep.PreImage != null || nextStep.PostImage != null))
                {
                    this.Delete(crmStep, null);
                    deletedSteps++;
                    continue;
                }

                if (nextStep.PreImage != null)
                {
                    var curPre = images.Where(r => r.ImageType == sdkmessageprocessingstepimage_imagetype.PreImage).SingleOrDefault();

                    if (curPre == null)
                    {
                        this.Delete(crmStep, null);
                        deletedSteps++;
                        continue;
                    }

                    if (nextStep.PreImage.AllAttributes && !string.IsNullOrEmpty(curPre.Attributes1))
                    {
                        this.Delete(crmStep, curPre);
                        deletedSteps++;
                        continue;
                    }


                    var curFilter = string.Join(",", curPre.Attributes1?.Split(',').OrderBy(r => r).ToArray() ?? new string[0]);
                    var nexFilter = string.Join(",",nextStep.PreImage.FilteredAttributes?.OrderBy(r => r).ToArray() ?? new string[0]);

                    if (curFilter != nexFilter)
                    {
                        this.Delete(crmStep, curPre);
                        deletedSteps++;
                        continue;
                    }
                }

                if (nextStep.PostImage != null)
                {
                    var curPos = images.Where(r => r.ImageType == sdkmessageprocessingstepimage_imagetype.PostImage).SingleOrDefault();

                    if (curPos == null)
                    {
                        this.Delete(crmStep, null);
                        deletedSteps++;
                    }

                    if (nextStep.PreImage.AllAttributes && !string.IsNullOrEmpty(curPos.Attributes1))
                    {
                        this.Delete(crmStep, curPos);
                        deletedSteps++;
                        continue;
                    }

                    var curFilter = string.Join(",", curPos.Attributes1?.Split(',').OrderBy(r => r).ToArray() ?? new string[0]);
                    var nexFilter = string.Join(",", nextStep.PostImage.FilteredAttributes?.OrderBy(r => r).ToArray() ?? new string[0]);

                    if (curFilter != nexFilter)
                    {
                        this.Delete(crmStep, curPos);
                        deletedSteps++;
                        continue;
                    }
                }
            }
            return false;
        }


        private void Delete(Entities.SdkMessageProcessingStep step, Entities.SdkMessageProcessingStepImage image)
        {
            if (image != null)
            {
                this.orgService.Delete(image.LogicalName, image.Id);
            }
            this.orgService.Delete(step.LogicalName, step.Id);
            messageService.Inform($"Image changed for step: {step.Name}");
        }

        private void Delete(Entities.PluginType pluginType, bool imagesChanged)
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

            if (imagesChanged)
            {
                messageService.Inform($"Image changed for plugin: {pluginType.Name}");
            }
            else
            {
                messageService.Inform($"Removed plugin: {pluginType.Name}");
            }
        }
    }
}
