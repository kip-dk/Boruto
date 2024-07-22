using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Implementations
{
    internal class Repository<T> : IRepository<T> where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        private readonly IOrganizationService orgService;
        private readonly OrganizationServiceContext ctx;
        private string logicalName;
        private static readonly Microsoft.Xrm.Sdk.Query.ColumnSet ALL = new Microsoft.Xrm.Sdk.Query.ColumnSet(true);

        internal Repository(
            Microsoft.Xrm.Sdk.IOrganizationService orgService,
            Microsoft.Xrm.Sdk.Client.OrganizationServiceContext ctx
            )
        {
            this.orgService = orgService;
            this.ctx = ctx;
            this.logicalName = new T().LogicalName;
        }

        public Guid Create(T entity)
        {
            return this.orgService.Create(entity);
        }

        public void Delete(T entity)
        {
            this.orgService.Delete(entity.LogicalName, entity.Id);
        }

        public void Delete(Guid id)
        {
            this.orgService.Delete(this.logicalName, id);
        }

        public T Get(Guid id)
        {
            return this.orgService.Retrieve(this.logicalName, id, ALL).ToEntity<T>();
        }

        public IQueryable<T> GetQuery()
        {
            this.ClearCache();
            return this.ctx.CreateQuery<T>(); 
        }

        public void Update(T entity)
        {
            this.orgService.Update(entity);
        }

        private void ClearCache()
        {
            var entities = this.ctx.GetAttachedEntities().Where(r => r.LogicalName == this.logicalName).ToArray();
            foreach (var ent in entities)
            {
                this.ctx.Detach(ent);
            }
         }
    }
}
