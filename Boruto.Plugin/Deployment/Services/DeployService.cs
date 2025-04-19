using Boruto.Deployment.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Services
{
    [Export(typeof(ServiceAPI.IDeployService))]
    public class DeployService : ServiceAPI.IDeployService
    {
        private readonly IPluginDeploymentService pluginDeployService;
        private readonly IPluginAssemblyService pluginAssmService;
        private readonly IPluginTypeService pluginTypeService;
        private readonly ISdkMessageProcessingStepService sdkMessageProcessingStepService;
        private readonly ISolutionService solutionService;
        private readonly IPublishereService pubService;
        private readonly IPluginPackagesService pacService;
        private readonly INugetService nugetService;
        private readonly Boruto.Deployment.Models.Config config;


        [ImportingConstructor]
        public DeployService(
            ServiceAPI.IPluginDeploymentService pluginDeployService,
            ServiceAPI.IPluginAssemblyService pluginAssmService,
            ServiceAPI.IPluginTypeService pluginTypeService,
            ServiceAPI.ISdkMessageProcessingStepService sdkMessageProcessingStepService,
            ServiceAPI.ISolutionService solutionService,
            ServiceAPI.IPublishereService pubService,
            ServiceAPI.IPluginPackagesService pacService,
            ServiceAPI.INugetService nugetService

            )
        {
            this.pluginDeployService = pluginDeployService;
            this.pluginAssmService = pluginAssmService;
            this.pluginTypeService = pluginTypeService;
            this.sdkMessageProcessingStepService = sdkMessageProcessingStepService;
            this.solutionService = solutionService;
            this.pubService = pubService;
            this.pacService = pacService;
            this.nugetService = nugetService;
            this.config = Boruto.Deployment.Models.Config.Instance;

        }
        public void Deploy()
        {
            if (this.Validate())
            {
                var prefix = this.pubService.ComponentPrefix;
                var nuget = this.nugetService.GetSpec();

                var pluginPackageName = $"{pubService.ComponentPrefix}_{nuget.Metadata.Id}";

                var pluginPackage = this.pacService.GetPluginPackage(pluginPackageName);

                var wasCreate = false;
                if (pluginPackage == null)
                {
                    var pid = pacService.Create(this.config.Plugin.Name, pluginPackageName, nuget.Metadata.Version, System.IO.File.ReadAllBytes(this.config.Plugin.Package.Replace("$version", nuget.Metadata.Version)));
                    Console.WriteLine("PluginPackage was uploaded");

                    this.solutionService.AddMissingPluginPackage(new Boruto.Deployment.Entities.pluginpackage { pluginpackageId = pid });
                    Console.WriteLine("New PluginPackage was added to solution");

                    pluginPackage = this.pacService.GetPluginPackage(pluginPackageName);
                    wasCreate = true;
                }

                var pluginAssemblies = this.pluginAssmService.ForPackage(pluginPackage.pluginpackageId.Value);

                if (pluginAssemblies.Length == 0 && wasCreate)
                {
                    throw new Exception($"The package uploaded did not create any PluginAssemblies");
                }

                var dlls = nugetService.GetLibNet64();


                var dyns = new Dictionary<string, Assembly>();
                System.Reflection.Assembly pluginAssm = null;


                List<Assembly> serviceAssemblies = new List<Assembly>();
                foreach (var dll in dlls)
                {
                    var code = System.AppDomain.CurrentDomain.Load(dll.Code);
                    Console.WriteLine($"Loaded: {code.FullName}");
                    dyns.Add(code.FullName, code);
                    if (config.Resolve(code))
                    {
                        serviceAssemblies.Add(code);
                    }
                }

                if (pluginAssm == null)
                {
                    if (wasCreate)
                    {
                        this.pacService.Delete(pluginPackage.pluginpackageId.Value);
                    }
                    throw new Exception($"Plugin assembly was not loaded, it is not possible to determin needed steps to be created: {pluginAssemblies[0].Name}");
                }

                Assembly DynamicResolver(object sender, ResolveEventArgs args)
                {
                    if (dyns.TryGetValue(args.Name, out Assembly asm))
                    {
                        return asm;
                    }
                    return null;
                }

                System.AppDomain.CurrentDomain.AssemblyResolve += DynamicResolver;


                // Then we find out the steps we needs according to the loaded assembly
                var upcommingPlugins = pluginDeployService.ForAssembly(serviceAssemblies.ToArray());

                // Then we find the plugin types we already have
                var existingplugins = pluginTypeService.ForPluginAssembly(pluginAssemblies[0].PluginAssemblyId.Value);

                // Then we cleanup plugin types in CRM that are no longer needed.
                pluginTypeService.JoinAndCleanup(existingplugins, upcommingPlugins);

                // Then we find the steps we already have (after cleanup on plugins that no longer exists in the assembly)
                var steps = sdkMessageProcessingStepService.ForPluginAssembly(pluginAssemblies[0].PluginAssemblyId.Value);

                // Then we cleanup steps that are no longer needed
                steps = sdkMessageProcessingStepService.Cleanup(steps, upcommingPlugins);

                if (!wasCreate)
                {
                    pacService.Update(pluginPackage.pluginpackageId.Value, nuget.Metadata.Version, System.IO.File.ReadAllBytes(this.config.Plugin.Package.Replace("$version", nuget.Metadata.Version)));
                }

                // now map existing plugin types with existing upcomming, and create new plugintypes on the fly and map to upcomming if applicable
                pluginTypeService.FindAndJoinMissing(pluginAssemblies[0].PluginAssemblyId.Value, upcommingPlugins);

                // Create/Update steps according to upcommingPlugins defacto code, and return the brutto list of steps we have after the create/update
                steps = sdkMessageProcessingStepService.CreateOrUpdateSteps(steps, upcommingPlugins);

                // update solution with components
                this.solutionService.AddMissingPluginSteps(steps);
            }
        }

        private bool Validate()
        {
            if (this.config == null)
            {
                Console.WriteLine($"Please provide config file to be used on the command line on form /config:filename");
                return false;
            }

            if (string.IsNullOrEmpty(this.config.Solution))
            {
                Console.WriteLine($"Please provide solution to be used in the config file");
                return false;
            }

            return this.config.validatePluginConfiguration();
        }
    }
}
