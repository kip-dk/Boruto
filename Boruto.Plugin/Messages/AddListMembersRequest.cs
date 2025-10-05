using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Boruto.Messages
{
    public class AddListMembersRequest : Microsoft.Xrm.Sdk.OrganizationRequest
    {
        public AddListMembersRequest(): base ("AddListMembers")
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
                if (this.Parameters.TryGetValue(nameof(MemberIds), out Guid[] mids))
                {
                    return mids;
                }

                if (Boruto.PluginContext.Current != null)
                {
                    var keys = this.Parameters.Keys;
                    foreach (var key in keys)
                    {
                        var vale = this.Parameters[key];
                        if (vale != null)
                        {
                            Boruto.Trace.Info($"{key} = {vale}, {vale.GetType().FullName}");
                        }
                        else
                        {
                            Boruto.Trace.Info($"{key} is nnull");
                        }
                    }
                }
                throw new InvalidPluginExecutionException($"Expected: { nameof(MemberIds) } to be in the request payload.");
            }
            set
            {
                this.Parameters[nameof(MemberIds)] = value;
            }
        }
    }
}
