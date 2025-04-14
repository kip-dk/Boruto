using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Plugins.bor_plugindemo
{
    public class bor_plugindemoPlugin : BasePlugin
    {
        public void OnPreCreate(Entities.bor_plugindemos.bor_plugindemo target, ServiceAPI.IPluginDemoService service)
        {
            service.OnCreate(target);
        }
    }
}
