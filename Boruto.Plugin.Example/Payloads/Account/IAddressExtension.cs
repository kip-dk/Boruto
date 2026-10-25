using Boruto.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Payloads
{
    public partial class AccountAdr : Boruto.Plugin.Entities.Account, AccountAdr.IAddressExtension
    {
        [Microsoft.Xrm.Sdk.AttributeLogicalName(Boruto.Plugin.Entities.Account.Fields.Address1_Line1)]
        public string Line1 { get => this.Address1_Line1; set => this.Address1_Line1 = value; }

        [Microsoft.Xrm.Sdk.AttributeLogicalName(Boruto.Plugin.Entities.Account.Fields.Address1_City)]
        public string City { get => this.Address1_City; set => this.Address1_City = value; }

        [Microsoft.Xrm.Sdk.AttributeLogicalName(Boruto.Plugin.Entities.Account.Fields.Address1_Country)]
        public string Country { get => this.Address1_Country; set => this.Address1_Country = value; }

        public interface IAddressExtension : Model.IAddress
        {
            [TargetFilter]
            string AccountNumber { get; }
        }
    }
}
