using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.ServiceAPI
{
    public interface ISolutionService
    {
        Entities.Solution Get(string unieuqName);
        void AddMissingPluginPackage(Entities.pluginpackage package);
        void AddMissingPluginAssembly(Entities.PluginAssembly assm);
        void AddMissingPluginTypes(Entities.PluginType[] pluginTypes);
        void AddMissingPluginSteps(Entities.SdkMessageProcessingStep[] steps);
    }
}
