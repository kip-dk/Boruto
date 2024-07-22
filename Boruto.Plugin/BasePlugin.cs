using Microsoft.Xrm.Sdk;
using System;

namespace Boruto
{
    public class BasePlugin : IPlugin
    {
        private readonly string unsecure;
        private readonly string secureString;

        public BasePlugin()
        {
        }

        public BasePlugin(string unsecure, string secureString)
        {
            this.unsecure = unsecure;
            this.secureString = secureString;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            using (var ctx = new PluginContext(this, serviceProvider, unsecure, secureString))
            {
                ctx.Execute();
            }
        }
    }
}
