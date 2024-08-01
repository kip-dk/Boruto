using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Entities
{
    public partial class AccountStateChanged : Boruto.Plugin.Entities.Account, AccountStateChanged.IStatChanged
    {
        public AccountStateChanged(Microsoft.Xrm.Sdk.ITracingService traceService) : base()
        {
        }

        public interface IStatChanged : ITarget
        {
            Plugin.Entities.account_statecode? StateCode { get; }
        }
     }
}
