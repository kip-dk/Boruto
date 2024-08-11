using Boruto.ServiceAPI;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Implementations.Services
{
    internal class NamingService : ServiceAPI.INamingService
    {
        private readonly IMetadataService metaService;
        private readonly IOrganizationService orgService;

        private static Dictionary<string, string> knownPrimaryNames = new Dictionary<string, string>();

        internal NamingService(Boruto.ServiceAPI.IMetadataService metaService, Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.metaService = metaService;
            this.orgService = orgService;
        }

        public string NameOf(EntityReference re)
        {
            if (re == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(re.Name))
            {
                return re.Name;
            }

            string primaryCol = null;

            if (knownPrimaryNames.TryGetValue(re.LogicalName, out primaryCol)) { };

            if (primaryCol == null)
            {
                var meta = this.metaService.ForEntity(re.LogicalName);
                primaryCol = meta.Attributes.Where(r => r.IsPrimaryName == true).Select(r => r.LogicalName).SingleOrDefault();
                if (!string.IsNullOrEmpty(primaryCol))
                {
                    knownPrimaryNames[re.LogicalName] = primaryCol;
                }
            }

            if (primaryCol != null)
            {
                var ent = this.orgService.Retrieve(re.LogicalName, re.Id, new ColumnSet(primaryCol));
                if (ent.Attributes.ContainsKey(primaryCol))
                {
                    re.Name = (string)ent[primaryCol];
                }
            }
            return re.Name;
        }
    }
}
