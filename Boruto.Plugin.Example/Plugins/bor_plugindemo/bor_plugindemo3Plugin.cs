using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Plugins.bor_plugindemo
{
    public class bor_plugindemo3Plugin : BasePlugin
    {
        public void OnPreUpdate(Payloads.bor_plugindemo.bor_plugindemo target, Payloads.bor_plugindemo.bor_plugindemo merged)
        {
            if (target.bor_number != null && target.bor_number == 123)
            {
                merged.bor_empty = $"settting empty from number: { target.bor_number }: { merged.bor_name }";
            }
        }
    }
}
