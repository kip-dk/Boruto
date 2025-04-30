using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Deployment.Entities
{
    internal abstract class AbstractBaseEntity
    {
        public virtual Guid Id { get; set; }
        public string LogicalName { get; set; }
        internal Microsoft.Xrm.Sdk.AttributeCollection Attributes { get; set; }

        protected Microsoft.Xrm.Sdk.FormattedValueCollection FormattedValues = new Microsoft.Xrm.Sdk.FormattedValueCollection();

        internal AbstractBaseEntity(string logicalName)
        {
            this.LogicalName = logicalName;
            this.Attributes = new Microsoft.Xrm.Sdk.AttributeCollection();
        }

        internal AbstractBaseEntity(Microsoft.Xrm.Sdk.Entity entity)
        {
            this.Attributes = entity.Attributes;
            this.Id = entity.Id;
            this.LogicalName = entity.LogicalName;
        }

        internal Microsoft.Xrm.Sdk.Entity ToEntity()
        {
            return new Microsoft.Xrm.Sdk.Entity
            {
                Id = this.Id,
                LogicalName = this.LogicalName,
                Attributes = this.Attributes
            };
        }

        internal T GetAttributeValue<T>(string attribuName)
        {
            if (this.Attributes.ContainsKey(attribuName))
            {
                return (T)this.Attributes[attribuName];
            }
            return default(T);
        }

        internal void SetAttributeValue(string attribuName, object value)
        {
            this.Attributes[attribuName] = value;
        }

        internal Microsoft.Xrm.Sdk.EntityReference ToEntityReference()
        {
            return new Microsoft.Xrm.Sdk.EntityReference(this.LogicalName, this.Id);
        }
    }
}
