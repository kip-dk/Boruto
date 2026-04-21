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
            lock (knownPrimaryNames)
            {
                if (re == null)
                {
                    return null;
                }

                if (re != null && re.Name != null && !string.IsNullOrEmpty(re.Name.Trim()))
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

        public Microsoft.Xrm.Sdk.EntityReference[] NameReferencesOff(string entityLogicalName, params Guid[] ids)
        {
            if (string.IsNullOrEmpty(entityLogicalName))
            {
                return null;
            }

            if (ids == null || ids.Length == 0)
            {
                return null;
            }

            var meta = this.metaService.ForEntity(entityLogicalName);


            var qe = new Microsoft.Xrm.Sdk.Query.QueryExpression(entityLogicalName);
            qe.ColumnSet.AddColumn(meta.PrimaryIdAttribute);
            qe.ColumnSet.AddColumn(meta.PrimaryNameAttribute);

            var filter = new FilterExpression(LogicalOperator.Or);
            foreach (var id in ids)
            {
                filter.AddCondition(new ConditionExpression(meta.PrimaryIdAttribute, Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, id));
            }
            qe.Criteria.Filters.Add(filter);

            var result = this.orgService.RetrieveMultiple(qe);

            return (from r in result.Entities
                    select new Microsoft.Xrm.Sdk.EntityReference
                    {
                        Id = (Guid)r[meta.PrimaryIdAttribute],
                        LogicalName = entityLogicalName,
                        Name = (string)r[meta.PrimaryNameAttribute]
                    }).ToArray();
        }

        public string Concat(params Microsoft.Xrm.Sdk.EntityReference[] refs)
        {
            return this.Concat(", ", refs);
        }
        public string Concat(string sep, params Microsoft.Xrm.Sdk.EntityReference[] refs)
        {
            if (refs == null || refs.Length == 0)
            {
                return null;
            }

            var comma = string.Empty;
            var sb = new StringBuilder();

            foreach (var re in refs)
            {
                if (re != null)
                {
                    var next = this.NameOf(re);
                    if (!string.IsNullOrEmpty(next))
                    {
                        sb.Append($"{comma}{next}");
                        comma = sep;
                    }
                }
            }
            return sb.ToString();
        }

    }
}
