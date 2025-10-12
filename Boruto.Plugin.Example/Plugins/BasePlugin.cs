using Boruto.Plugin.Example.Services;
using System;
using System.Reflection;

namespace Boruto.Plugin.Example.Plugins
{
    public abstract class BasePlugin : Boruto.BasePlugin
    {
        private static readonly Assembly[] assms = new Assembly[]
        {
            typeof(Boruto.Plugin.Entities.Account).Assembly,
            typeof(Boruto.Plugin.Example.Entities.AccountStateChanged).Assembly
        };

        public BasePlugin() : base()
        {
        }

        public BasePlugin(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override IServiceProvider ServiceProvider(Boruto.ServiceAPI.IServiceContext ctx) 
        {
            return new ExampleServiceProvider(ctx);
        }

        protected override Assembly[] ServiceAssemblies => assms;

    }
}
