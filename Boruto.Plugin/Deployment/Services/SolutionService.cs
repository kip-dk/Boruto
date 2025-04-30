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
    [Export(typeof(ISolutionService))]
    internal class SolutionService : ISolutionService
    {
        private readonly IOrganizationService orgService;
        private readonly IMessageService messageService;
        private Solution solution;
        private bool initialized = false;

        private const string NO_SOLUTION_MESSAGE = "Unable to add components to solution. Add /solution:[solutionname] as parameter to the command tool to get components attached to a specific solution.";

        [ImportingConstructor]
        public SolutionService(Microsoft.Xrm.Sdk.IOrganizationService orgService, IMessageService messageService)
        {
            this.orgService = orgService;
            this.messageService = messageService;
        }

        public Solution Get(string unieuqName)
        {
            var query = Entities.Solution.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(Entities.Solution.Fields.UniqueName, unieuqName);

            return this.orgService.RetrieveMultiple(query).Entities.Select(r => new Solution(r)).SingleOrDefault();
        }

        public void AddMissingPluginPackage(pluginpackage package)
        {
            Initialize();
            if (solution != null)
            {
                AddSolutionComponent(package.pluginpackageId.Value, 10476, false);
            }
        }

        public void AddMissingPluginAssembly(PluginAssembly assm)
        {
            Initialize();
            if (solution != null)
            {
                AddSolutionComponent(assm.PluginAssemblyId.Value, 91, false);
            }
        }
        public void AddMissingPluginTypes(PluginType[] pluginTypes)
        {
            Initialize();
            if (solution != null)
            {
                foreach (var plugin in pluginTypes)
                {
                    AddSolutionComponent(plugin.PluginTypeId.Value, 90, false);
                }
            }
        }

        public void AddMissingPluginSteps(SdkMessageProcessingStep[] steps)
        {
            Initialize();
            if (solution != null)
            {
                foreach (var step in steps)
                {
                    AddSolutionComponent(step.SdkMessageProcessingStepId.Value, 92, true);
                }
            }
        }

        private void AddSolutionComponent(Guid id, int componentType, bool addRequiredComponents)
        {
            var query = Entities.SolutionComponent.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(Entities.SolutionComponent.Fields.SolutionId, solution.SolutionId.Value);
            query.Criteria.Equal(Entities.SolutionComponent.Fields.ObjectId, id);
            query.Criteria.Equal(Entities.SolutionComponent.Fields.ComponentType, componentType);

            var component = this.orgService.RetrieveMultiple(query).Entities.Select(r => new SolutionComponent(r)).SingleOrDefault();

            if (component == null)
            {
                var req = new Microsoft.Crm.Sdk.Messages.AddSolutionComponentRequest
                {
                    AddRequiredComponents = addRequiredComponents,
                    ComponentId = id,
                    ComponentType = componentType,
                    SolutionUniqueName = solution.UniqueName
                };
                this.orgService.Execute(req);
                messageService.Inform($"Added {id} of type {componentType} to solution: {solution.UniqueName}");
            }
        }

        private void Initialize()
        {
            if (!initialized)
            {
                var args = Environment.GetCommandLineArgs();

                var solutionName = (from a in args where a.ToLower().StartsWith("/solution:") select a).SingleOrDefault()?.Substring(10);

                if (solutionName == null && Models.Config.Instance != null)
                {
                    solutionName = Models.Config.Instance.Solution;
                }

                if (solutionName == null)
                {
                    messageService.Inform(NO_SOLUTION_MESSAGE);
                    initialized = true;
                    return;
                }

                solution = this.Get(solutionName);

                if (solution == null)
                {
                    messageService.Inform($"Solution with unique name {solutionName} was not found. Components will not be attached to a solution.");
                }

                initialized = true;

            }
        }

    }
}
