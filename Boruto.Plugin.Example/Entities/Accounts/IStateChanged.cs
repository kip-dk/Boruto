using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Entities
{
    public partial class AccountStateChanged : Boruto.Plugin.Entities.Account, AccountStateChanged.IStatChanged
    {
        public AccountStateChanged() : base()
        {
        }

        public AccountStateChanged(Microsoft.Xrm.Sdk.ITracingService traceService) : this()
        {
        }

        void IStatChanged.OnStateChanged()
        {
            this.Description = $"{System.DateTime.UtcNow.ToString($"yyyy-MM-dd HH:mm:ss")} state changed to: { this.StateCode.Value.ToString() }";
        }

        public interface IStatChanged : ITarget
        {
            Plugin.Entities.account_statecode? StateCode { get; }

            void OnStateChanged();
        }
     }
}
