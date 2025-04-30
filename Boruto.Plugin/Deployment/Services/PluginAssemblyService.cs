using Boruto.Deployment.Entities;
using Boruto.Deployment.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boruto.Extensions.FilterExpression;
using Microsoft.Xrm.Sdk;
using System.Xml.Linq;
using Boruto.Extensions.QueryExpression;

namespace Boruto.Deployment.Services
{
    [Export(typeof(ServiceAPI.IPluginAssemblyService))]
    internal class PluginAssemblyService : ServiceAPI.IPluginAssemblyService
    {
        private readonly IOrganizationService orgService;
        private ServiceAPI.IMessageService messageService;
        private readonly IPluginTypeService typeService;
        private readonly ISdkMessageProcessingStepService stepService;

        public System.Reflection.Assembly Assembly { get; private set; }
        private Entities.PluginAssembly pluginAssembly;
        private byte[] code;
        private bool isNew = true;

        [ImportingConstructor]
        public PluginAssemblyService(
            Microsoft.Xrm.Sdk.IOrganizationService orgService,
            ServiceAPI.IMessageService messageService,
            ServiceAPI.IPluginTypeService typeService,
            ServiceAPI.ISdkMessageProcessingStepService stepService)
        {
            this.orgService = orgService;
            this.messageService = messageService;
            this.typeService = typeService;
            this.stepService = stepService;
        }

        public PluginAssembly FindOrCreate(string assemblyfilename)
        {
            this.code = System.IO.File.ReadAllBytes(assemblyfilename);
            this.Assembly = System.Reflection.Assembly.Load(code);
            var publickeytoken = this.GetPublicKeyTokenFromAssembly();

            var name = assemblyfilename.Split(new char[] { '\\', '/' }).Last();
            if (name.ToUpper().EndsWith(".DLL"))
            {
                name = name.Substring(0, name.Length - 4);
            }

            var res = this.GetPluginAssembly(name);

            if (res != null)
            {
                this.pluginAssembly = res;
                this.isNew = false;
                return this.pluginAssembly;
            }

            var r = new PluginAssembly
            {
                PluginAssemblyId = Guid.NewGuid(),
                Content = System.Convert.ToBase64String(code),
                Description = name,
                IsolationMode = pluginassembly_isolationmode.Sandbox,
                Name = name,
                SourceType = pluginassembly_sourcetype.Database,
                Culture = "neutral",
                PublicKeyToken = publickeytoken,
                Version = "1.0"
            };
            this.orgService.Create(r.ToEntity());
            this.messageService.Inform("Assembly code was created");

            this.pluginAssembly = r;
            this.isNew = true;

            return r;
        }


        public void UploadAssembly()
        {
            if (!isNew)
            {
                var clean = new Entities.PluginAssembly { PluginAssemblyId = this.pluginAssembly.PluginAssemblyId };
                clean.Content = System.Convert.ToBase64String(code);
                this.orgService.Update(clean.ToEntity());
                this.messageService.Inform("Assembly code updated");
            }
        }


        public Entities.PluginAssembly GetPluginAssembly(string name)
        {
            var query = Entities.PluginAssembly.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(Entities.PluginAssembly.Fields.Name, name);

            var res = this.orgService.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (res != null)
            {
                return new PluginAssembly(res);
            }
            return null;
        }

        public Entities.PluginAssembly[] ForPackage(Guid pluginPackageId)
        {
            var query = Entities.PluginAssembly.EntityLogicalName.ToQueryExpression();
            query.Criteria.Equal(Entities.PluginAssembly.Fields.PackageId, pluginPackageId);

            return this.orgService.RetrieveMultiple(query).Entities.Select(r => new Entities.PluginAssembly(r)).ToArray();
        }

        private string GetPublicKeyTokenFromAssembly()
        {
            var bytes = this.Assembly.GetName().GetPublicKeyToken();
            if (bytes == null || bytes.Length == 0)
                return "None";

            var publicKeyToken = string.Empty;
            for (int i = 0; i < bytes.GetLength(0); i++)
                publicKeyToken += string.Format("{0:x2}", bytes[i]);

            return publicKeyToken;
        }

    }
}
