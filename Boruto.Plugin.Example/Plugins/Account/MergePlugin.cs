using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Plugins.Account
{
    public class MergePlugin : BasePlugin
    {
        public void OnPreMerge(Boruto.ITargetReference<Boruto.Plugin.Entities.Account> target, Guid SubordinateId, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            Boruto.Trace.Info($"Pre : { target.Id } -> { SubordinateId }");
        }

        public void OnPostMerge(Boruto.ITargetReference<Boruto.Plugin.Entities.Account> target, Guid SubordinateId, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            Boruto.Trace.Info($"Post: {target.Id} -> {SubordinateId}");
        }
    }
}
