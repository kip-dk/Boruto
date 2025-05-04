using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.ServiceAPI
{
    public interface INamingService
    {
        /// <summary>
        /// Returns the name of the entity. The Name field in the entity will be used if populated, otherwise a lookup will be performed, and the Name field
        /// will be populated, and the result will be returned
        /// </summary>
        /// <param name="re"></param>
        /// <returns></returns>
        string NameOf(Microsoft.Xrm.Sdk.EntityReference re);

        /// <summary>
        /// Returns the concated string of the names, using  [, ] as separator
        /// </summary>
        /// <param name="refs"></param>
        /// <returns></returns>
        string Concat(params Microsoft.Xrm.Sdk.EntityReference[] refs);

        /// <summary>
        /// Returns the concated string of names, using sep as separator
        /// </summary>
        /// <param name="sep"></param>
        /// <param name="refs"></param>
        /// <returns></returns>
        string Concat(string sep, params Microsoft.Xrm.Sdk.EntityReference[] refs);

        /// <summary>
        /// Returns all found references, with Name property populated the the list of entity ids.
        /// The length of the result will not corresond to the length of ids in the case where some of the ids are not found
        /// </summary>
        /// <param name="entityLogicalName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Microsoft.Xrm.Sdk.EntityReference[] NameReferencesOff(string entityLogicalName, params Guid[] ids);
    }
}
