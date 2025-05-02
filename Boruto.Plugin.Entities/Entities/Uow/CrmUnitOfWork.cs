using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Entities
{
    public partial class CrmUnitOfWork : IUnitOfWork, IDisposable
    {
        #region Members
        private readonly ServiceContext context;


        private readonly IOrganizationService Service;

        #endregion

        #region Constructor


        public CrmUnitOfWork(IOrganizationService service)
        {
            this.Service = service;
            this.context = new ServiceContext(this.Service);
        }
        #endregion

        #region IDisposable

        public void Dispose()
        {
            context.Dispose();
        }

        #endregion

        #region IUnitOfWork

        #region generic methods
        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        /// <summary>
        /// Executes a CRM request.
        /// </summary>
        /// <typeparam name="R">Type of the response.</typeparam>
        /// <param name="request">Request to execute.</param>
        public R ExecuteRequest<R>(Microsoft.Xrm.Sdk.OrganizationRequest request) where R : OrganizationResponse
        {
            return (R)this.context.Execute(request);
        }

        public OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request)
        {
            return this.context.Execute(request);
        }

        public Guid Create(Microsoft.Xrm.Sdk.Entity entity)
        {
            return this.Service.Create(entity);
        }

        public void Update(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.Service.Update(entity);
        }

        public void Delete(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.Service.Delete(entity.LogicalName, entity.Id);
        }

        public void Delete(string logicalname, params Guid[] ids)
        {
            if (!string.IsNullOrEmpty(logicalname) && ids != null && ids.Length > 0)
            {
                foreach (var id in ids)
                {
                    this.Service.Delete(logicalname, id);
                }
            }
        }


        public Microsoft.Xrm.Sdk.EntityCollection RetrieveMultiple(Microsoft.Xrm.Sdk.Query.QueryExpression query)
        {
            return this.Service.RetrieveMultiple(query);
        }
        #endregion

        public void Detach(string logicalName, params Guid[] ids)
        {
            if (this.context != null)
            {
                var attached = this.context.GetAttachedEntities().Where(r => r.LogicalName == logicalName).ToArray();

                if (ids != null && ids.Length > 0)
                {
                    attached = attached.Where(r => ids.Contains(r.Id)).ToArray();
                }

                foreach (var att in attached)
                {
                    this.context.Detach(att);
                }
            }
        }

        public Microsoft.Xrm.Sdk.Entity Retrieve(string logicalName, Guid id, Microsoft.Xrm.Sdk.Query.ColumnSet columnSet)
        {
            return this.Service.Retrieve(logicalName, id, columnSet);
        }

        public void Detach(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.Detach(entity.LogicalName, entity.Id);
        }

        public void Detach(Microsoft.Xrm.Sdk.EntityReference rf)
        {
            this.Detach(rf.LogicalName, rf.Id);
        }

        public void ClearChanges()
        {
            if (this.context != null)
            {
                this.context.ClearChanges();
            }
        }

        public void ClearContext()
        {
            if (this.context != null)
            {
                this.context.ClearChanges();
                var attached = this.context.GetAttachedEntities().ToArray();
                foreach (var att in attached)
                {
                    this.context.Detach(att);
                }
            }
        }

        public void Associate(String entityName, Guid entityId, Relationship relationship, EntityReferenceCollection entities)
        {
            this.Service.Associate(entityName, entityId, relationship, entities);
        }

        public void Disassociate(String entityName, Guid entityId, Relationship relationship, EntityReferenceCollection entities)
        {
            this.Service.Disassociate(entityName, entityId, relationship, entities);
        }

        #endregion

        #region private helpers
        private Dictionary<Type, object> repros = new Dictionary<Type, object>();

        private IRepository<T> GetRepository<T>() where T : Microsoft.Xrm.Sdk.Entity, new()
        {
            var type = typeof(T);
            if (repros.TryGetValue(type, out object o))
            {
                return (IRepository<T>)o;
            }
            var r = new CrmRepository<T>(this.context);
            repros[type] = r;
            return r;
        }

        public object GetRepoByType(Type type)
        {
            if (repros.TryGetValue(type, out object o)) 
            {
                return o;
            }

            var dynType = typeof(CrmRepository<>).MakeGenericType(type);

            var result = System.Activator.CreateInstance(dynType, this.context);
            repros[type] = result;
            return result;
        }

        #endregion
    }
}
