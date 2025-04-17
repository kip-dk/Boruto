using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Entities.bor_plugindemos
{
    public partial class NameChanged : Boruto.Plugin.Entities.bor_plugindemo, NameChanged.INameChanged
    {
        public NameChanged(Microsoft.Xrm.Sdk.ITracingService traceService): base()
        {
            traceService.Trace($"trace service was injected into an entity instance.");
        }

        public interface INameChanged : Boruto.ITarget
        {
            string bor_name { get; }
            string bor_processmessage { set; }
        }
    }
}
