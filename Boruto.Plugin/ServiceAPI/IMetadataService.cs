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
    }
}
