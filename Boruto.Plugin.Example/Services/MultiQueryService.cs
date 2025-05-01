using Boruto.Plugin.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Services
{
    public class MultiQueryService : ServiceAPI.IMultiQueryService
    {
        private readonly ITracingService traceService;
        private readonly IQueryable<Account> acQuery;
        private readonly IQueryable<bor_plugindemo> demoQuery;

        public MultiQueryService(
            Microsoft.Xrm.Sdk.ITracingService traceService,
            IQueryable<Boruto.Plugin.Entities.Account> acQuery,
            IQueryable<Boruto.Plugin.Entities.bor_plugindemo> demoQuery)
        {
            this.traceService = traceService;
            this.acQuery = acQuery;
            this.demoQuery = demoQuery;
        }
        public void DoSomeQuery()
        {
            traceService.Trace($"In multi query"); 
            traceService.Trace($"{this.acQuery.GetType().FullName}");
            traceService.Trace($"{this.demoQuery.GetType().FullName}");

        }
    }
}
