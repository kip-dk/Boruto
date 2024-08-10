using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        protected override IServiceProvider ServiceProvider => new Services.ExampleServiceProvider();

        protected override Assembly[] ServiceAssemblies => assms;
    }
}
