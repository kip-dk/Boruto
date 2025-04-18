using Boruto.Plugin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Actions.bor_PlugindemoBoundmessage
{
    public class bor_PlugindemoBoundmessagePlugin :Boruto.Plugin.Example.Plugins.BasePlugin
    {
        public bor_PlugindemoBoundmessageResponse OnPost(bor_PlugindemoBoundmessageRequest request)
        {
            return new bor_PlugindemoBoundmessageResponse
            {
                _output = $"{request.Input}: vi sætter noget output: {request.Target.Id}"
            };
        }
    }
}
