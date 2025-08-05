using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Messages
{
    public class AddMemberRequest : Microsoft.Xrm.Sdk.OrganizationRequest
    {
        public AddMemberRequest(): base ("AddMember")
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
