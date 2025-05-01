using Boruto.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Plugins.bor_plugindemo
{
    public class bor_plugindemoPlugin : BasePlugin
    {
        [Sort(1)]
        public void OnPreCreate(Entities.bor_plugindemos.NameChanged.INameChanged target, ServiceAPI.IPluginDemoService service)
        {
            service.OnCreate(target);
        }

        [Sort(2)]
        public void OnPreCreate(Entities.bor_plugindemos.NumberChanged.INumberChanged target)
        {
            target.DoStuff();
        }

        [Sort(1)]
        public void OnPreUpdate(
            Entities.bor_plugindemos.NameChanged.INameChanged target, ServiceAPI.IPluginDemoService service,
            Microsoft.Xrm.Sdk.ITracingService traceService, ServiceAPI.IMultiQueryService multiQuery)
        {
            multiQuery.DoSomeQuery();
            service.OnCreate(target);
        }

        [Sort(2)]
        public void OnPreUpdate(Entities.bor_plugindemos.NumberChanged.INumberChanged mergedimage)
        {
            mergedimage.DoStuff();
        }

        public void OnPreUpdate(Entities.bor_plugindemos.TriggerActionChanged.ITriggerActionChanged target)
        {
            target.DoStuff();
        }

        public void OnPreDelete(Boruto.ITargetReference<Boruto.Plugin.Entities.bor_plugindemo> target, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            trace.Trace($"Pre bor_plugindemo: { target.Id } { target.LogicalName }");
        }

        public void OnPostDelete(
            Boruto.Plugin.Example.Entities.bor_plugindemos.PreNumber.IPreNumber preimage,
            IQueryable<Boruto.Plugin.Entities.Account> accountQuery,
            IRepository<Boruto.Plugin.Entities.bor_plugindemo> demoRepo
            )
        {
            if (preimage.bor_number != null)
            {
                var other = (from o in demoRepo.GetQuery()
                             where o.bor_number == preimage.bor_number
                               && o.bor_plugindemoId != preimage.Id
                             select o.bor_plugindemoId).FirstOrDefault();

                var accounts = (from a in accountQuery
                                where a.StateCode == Plugin.Entities.account_statecode.Active
                                select a.AccountId.Value).ToArray();

                if (other != null)
                {
                    var clean = new Boruto.Plugin.Entities.bor_plugindemo { bor_plugindemoId = other.Value };
                    clean.bor_changelog = $"delete: {preimage.Id}: { preimage.bor_number }: Accounts: { accounts.Length }";
                    demoRepo.Update(clean);
                }
            }
        }
    }
}
