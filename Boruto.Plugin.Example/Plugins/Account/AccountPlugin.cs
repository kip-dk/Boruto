using Boruto.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Plugins.Account
{
    public class AccountPlugin : BasePlugin
    {
        public void OnPreUpdate(Boruto.Plugin.Example.Entities.AccountStateChanged.IStatChanged target)
        {
            target.OnStateChanged();
        }

        public void OnPreDelete(ITargetReference<Boruto.Plugin.Entities.Account> target, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            trace.Trace($"Deleting: { target.LogicalName } { target.Id }");
        }
    }

    public class AccountAddressPlugin : BasePlugin
    {
        [Sort(1)]
        public void OnPreCreate(Payloads.AccountAdr.IAddressExtension target)
        {

        }

        [Sort(1)]
        public void OnPreUpdate(Payloads.AccountAdr.IAddressExtension target)
        {

        }
    }
}
