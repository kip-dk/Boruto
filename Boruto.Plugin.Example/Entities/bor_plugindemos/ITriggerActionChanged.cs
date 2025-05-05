using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Entities.bor_plugindemos
{
    public partial class TriggerActionChanged : Boruto.Plugin.Entities.bor_plugindemo, TriggerActionChanged.ITriggerActionChanged
    {
        private readonly IOrganizationService orgService;

        public TriggerActionChanged(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        void ITriggerActionChanged.DoStuff()
        {
            if (this.bor_triggeraction == true)
            {
                this.bor_triggeraction = false;

                var req = new Boruto.Plugin.Entities.bor_PlugindemoBoundmessageRequest
                {
                    Target = new EntityReference(this.LogicalName, this.Id),
                    Input = System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss") + " from trigger"
                };

                var res = (Boruto.Plugin.Entities.bor_PlugindemoBoundmessageResponse)this.orgService.Execute(req);
                this.bor_changelog = res.Output;
            }
        }

        public interface ITriggerActionChanged : ITarget
        {
            bool? bor_triggeraction { get; set; }

            void DoStuff();
        }
    }
}
