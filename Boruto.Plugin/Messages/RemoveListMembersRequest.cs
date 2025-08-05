using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Messages
{
    public class RemoveListMembersRequest : Microsoft.Xrm.Sdk.OrganizationRequest
    {
        public RemoveListMembersRequest(): base ("RemoveListMembers")
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

        public Guid[] MemberIds
        {
            get
            {
                return (Guid[])this.Parameters[nameof(MemberIds)];
            } set
            {
                this.Parameters[nameof(MemberIds)] = value;
            }
        }
    }
}
