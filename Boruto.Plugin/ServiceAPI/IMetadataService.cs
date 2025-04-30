using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.ServiceAPI
{
    public interface IMetadataService
    {
        Microsoft.Xrm.Sdk.Metadata.EntityMetadata ForEntity(string logicalName);
        string[] CanCreateAttributeName(string logicalName);
        string[] CanUpdateAttributeName(string logicalName);
        string PrimaryKey(string logicalName);
        string PrimaryName(string logicalName);
        Microsoft.Xrm.Sdk.Entity CloneForCreate(Microsoft.Xrm.Sdk.Entity source, params string[] ommit);
    }
}
