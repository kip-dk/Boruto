using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    public abstract class EntityCollection
    {
        public abstract Microsoft.Xrm.Sdk.EntityCollection ToEntityCollection();
    }

    public class EntityCollection<T> : EntityCollection where T : Microsoft.Xrm.Sdk.Entity, new()
    {
        public Microsoft.Xrm.Sdk.DataCollection<Microsoft.Xrm.Sdk.Entity> Entities { get; }
        public string EntityName { get;  }
        public string MinActiveRowVersion { get; }
        public bool MoreRecords { get; }
        public string PagingCookie { get; }

        public EntityCollection()
        {
            this.EntityName = new T().LogicalName;
            this.MoreRecords = false;
        }

        public EntityCollection(Microsoft.Xrm.Sdk.EntityCollection source) 
        {
            this.Entities = source.Entities;
            this.EntityName = source.EntityName;
            this.MinActiveRowVersion = source.MinActiveRowVersion;
            this.MoreRecords = source.MoreRecords;
            this.PagingCookie = source.PagingCookie;

        }

        public EntityCollection(T[] entities)
        {
            this.Entities = new Microsoft.Xrm.Sdk.DataCollection<Microsoft.Xrm.Sdk.Entity>(entities.Length);
            foreach (var e in entities)
            {
                this.Entities.Add(e);
            }
            this.EntityName = new T().LogicalName;
            this.MinActiveRowVersion = null;
            this.MoreRecords = false;
            this.PagingCookie = null;
        }

        public override Microsoft.Xrm.Sdk.EntityCollection ToEntityCollection()
        {
            var result = new Microsoft.Xrm.Sdk.EntityCollection();
            result.EntityName = this.EntityName;
            result.MinActiveRowVersion = this.MinActiveRowVersion;
            result.MoreRecords = this.MoreRecords;
            result.PagingCookie = this.PagingCookie;


            if (this.Entities != null)
            {
                var pt = new T();
                foreach (var e in this.Entities)
                {
                    var next = new Microsoft.Xrm.Sdk.Entity
                    {
                        Id = e.Id,
                        LogicalName = pt.LogicalName,
                        Attributes = e.Attributes
                    };
                    result.Entities.Add(next);
                }
            }
            return result;
        }
    }
}
