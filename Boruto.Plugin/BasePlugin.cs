using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Boruto
{
    public abstract class BasePlugin : IPlugin
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
            var assemblies = new List<Assembly>();
            if (this.ServiceAssemblies != null && this.ServiceAssemblies.Length > 0)
            {
                assemblies.AddRange(this.ServiceAssemblies);
            }

            var me = this.GetType().Assembly;

            if (!assemblies.Contains(me))
            {
                assemblies.Add(me);
            }

            using (var ctx = new PluginContext(this, serviceProvider, this.ServiceProvider, this.ServiceAssemblies, unsecure, secureString))
            {
                ctx.Execute();
            }
        }

        protected virtual IServiceProvider ServiceProvider => null;
        protected virtual Assembly[] ServiceAssemblies => null;

    }
}
