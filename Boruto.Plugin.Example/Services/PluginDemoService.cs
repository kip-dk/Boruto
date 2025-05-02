using Boruto.Plugin.Entities;
using Boruto.Plugin.Example.Entities.bor_plugindemos;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Services
{
    public class PluginDemoService : ServiceAPI.IPluginDemoService
    {
        private readonly ITracingService traceService;
        private readonly Plugin.Entities.IRepository<Account> accountRepo;

        public PluginDemoService(Microsoft.Xrm.Sdk.ITracingService traceService,  Boruto.Plugin.Entities.IRepository<Boruto.Plugin.Entities.Account> accountRepo)
        {
            this.traceService = traceService;
            this.accountRepo = accountRepo;
        }

        public void OnCreate(NameChanged.INameChanged target)
        {
            var countAccounts = (from a in accountRepo.GetQuery()
                                 where a.StateCode == account_statecode.Active
                                 select a.AccountId.Value).ToArray();

            this.traceService.Trace($"Trace service was injected as expected, and we where able to fetch: { countAccounts.Length } accounts., now using repo.");
            target.bor_processmessage = $"{System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}: { target.bor_name }";
        }
    }
}
