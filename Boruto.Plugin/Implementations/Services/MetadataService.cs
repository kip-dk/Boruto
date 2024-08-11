using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Implementations.Services
{
    internal class MetadataService : ServiceAPI.IMetadataService
    {
        private static Dictionary<string, Container> metadatas = new Dictionary<string, Container>();
        private readonly IOrganizationService orgService;

        internal MetadataService(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            this.orgService = orgService;
        }

        public EntityMetadata ForEntity(string logicalName)
        {
            if (metadatas.TryGetValue(logicalName, out Container c) && c.Timeout < System.DateTime.UtcNow)
            {
                return c.Meta;
            }

            var req = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = logicalName,
                RetrieveAsIfPublished = false
            };
            var response = (RetrieveEntityResponse)this.orgService.Execute(req);

            metadatas[logicalName] = new Container(response.EntityMetadata);
            return metadatas[logicalName].Meta;
        }

        internal class Container
        {
            internal Container(Microsoft.Xrm.Sdk.Metadata.EntityMetadata meta)
            {
                Meta = meta;
                this.Timeout = System.DateTime.UtcNow.AddMinutes(60);
            }

            internal EntityMetadata Meta { get; }
            internal DateTime Timeout { get; }
        }
    }
}
