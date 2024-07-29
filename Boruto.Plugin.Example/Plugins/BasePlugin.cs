using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Plugins
{
    public abstract class BasePlugin : Boruto.BasePlugin
    {
        protected override IServiceProvider ServiceProvider => new Services.ExampleServiceProvider();
    }
}
