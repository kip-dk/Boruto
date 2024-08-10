using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Entities
{
    public partial class AccountStateChanged : Boruto.Plugin.Entities.Account, AccountStateChanged.IStatChanged
    {
        private readonly ITracingService traceService;

        public AccountStateChanged(Microsoft.Xrm.Sdk.ITracingService traceService) : base()
        {
            this.traceService = traceService;
        }

        void IStatChanged.OnStateChanged()
        {
            this.Description = $"{System.DateTime.UtcNow.ToString($"yyyy-MM-dd HH:mm:ss")} state changed to: { this.StateCode.Value.ToString() }";
            this.traceService.Trace(this.Description);
        }

        public interface IStatChanged : ITarget
        {
            Plugin.Entities.account_statecode? StateCode { get; }

            void OnStateChanged();
        }
     }
}
