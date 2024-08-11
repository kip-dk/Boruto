using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Reflection.Model
{
    internal class ServiceConstructorArgument
    {
        internal System.Reflection.ParameterInfo Parameter { get; private set; }
        internal bool Admin { get; private set; }
        internal bool IsTargetReference { get; private set; }
        internal bool IsOrganizationRequest { get; private set; }

        internal ServiceConstructorArgument(System.Reflection.ParameterInfo parameter)
        {
            this.Parameter = parameter;
            var adminAttr = parameter.GetCustomAttribute<Boruto.Attributes.AdminAttribute>();
            this.Admin = adminAttr != null;
            this.IsTargetReference = parameter.ParameterType == typeof(Microsoft.Xrm.Sdk.EntityReference) || typeof(ITargetReference).IsAssignableFrom(parameter.ParameterType);
            this.IsOrganizationRequest = typeof(Microsoft.Xrm.Sdk.OrganizationRequest).IsAssignableFrom(parameter.ParameterType);
        }
    }
}
