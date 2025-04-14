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

        public PluginDemoService(Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            this.traceService = traceService;
        }

        public void OnCreate(bor_plugindemo.INameChanged target)
        {
            this.traceService.Trace($"Trace service was injected as expected");
            target.bor_processmessage = $"{System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}: { target.bor_name }";
        }
    }
}
