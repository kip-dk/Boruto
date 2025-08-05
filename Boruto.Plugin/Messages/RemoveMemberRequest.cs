using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Messages
{
    public class RemoveMemberRequest : Microsoft.Xrm.Sdk.OrganizationRequest
    {
        public RemoveMemberRequest() : base("RemoveMember")
        {

        }

        public Guid ListId
        {
            get
            {
                return (Guid)this.Parameters[nameof(ListId)];
            }
            set
            {
                this.Parameters[nameof(ListId)] = value;
            }
        }

        public Guid EntityId
        {
            get
            {
                return (Guid)this.Parameters[nameof(EntityId)];
            }
            set
            {
                this.Parameters.Add(nameof(EntityId), value);
            }
        }
    }
}
