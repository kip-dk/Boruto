using Boruto.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Boruto.Reflection.Model
{
    internal class Argument
    {
        private readonly Type pluginType;
        private readonly MethodInfo method;
        private readonly ParameterInfo parameterinfo;
        private readonly string primaryLogicalName;
        private Assembly[] assemblies;

        internal Argument(Type pluginType, System.Reflection.MethodInfo method, System.Reflection.ParameterInfo parameterinfo, string primaryLogicalName, Assembly[] assemblies)
        {
            this.Name = parameterinfo.Name;
            this.IsMethodArgument = true;
            this.pluginType = pluginType;
            this.method = method;
            this.parameterinfo = parameterinfo;
            this.primaryLogicalName = primaryLogicalName;
            this.assemblies = assemblies;
            this.Resolve();

            if (this.IsEntityMatch == null || this.IsEntityMatch == true)
            {
                var admin = method.GetCustomAttribute<Attributes.AdminAttribute>();

                if (admin != null)
                {
                    this.Admin = true;
                }
            }
        }

        internal bool IsTarget { get; private set; }
        internal bool IsPreImage { get; private set; }
        internal bool IsMergedImage { get; private set; }
        internal bool IsPostImage { get; private set; }
        internal bool IsOrganizationRequest { get; private set; }
        internal Type FromType { get; private set; }
        internal Type EarlyBoundEntityType { get; private set; }
        internal bool FilteredAllAttributes { get; private set; } = false;
        internal string[] FilteredAttributes { get; private set; }
        internal bool PreAllAttributes { get; private set; } = false;
        internal string[] PreAttributes { get; private set; }
        internal bool PostAllAttributes { get; private set; } = false;
        internal string[] PostAttributes { get; private set; }
        internal bool IsTargetReference { get; private set; }
        internal bool Admin { get; private set; }
        internal string Name { get; private set; }
        internal bool IsMethodArgument { get; private set; }
        internal bool? IsEntityMatch { get; private set; }

        private void Resolve()
        {
            this.FromType = this.parameterinfo.ParameterType;

            if (this.FromType.IsEntityType())
            {
                this.EarlyBoundEntityType = this.FromType.ResolveEntityType(this.primaryLogicalName, this.assemblies);

                if (this.EarlyBoundEntityType == null)
                {
                    this.IsEntityMatch = false;
                    return;
                }
                this.IsEntityMatch = true;
            }

            if (this.primaryLogicalName != null)
            {
                if (typeof(ITarget).IsAssignableFrom(this.FromType))
                {
                    this.IsTarget = true;
                    this.FilteredAttributes = this.FromType.ResolveAttributes(this.EarlyBoundEntityType, null, out bool all);
                    this.FilteredAllAttributes = all;
                    return;
                }

                if (typeof(IPreImage).IsAssignableFrom(this.FromType))
                {
                    this.IsPreImage = true;
                    this.PreAttributes = this.FromType.ResolveAttributes(this.EarlyBoundEntityType, null, out bool all);
                    this.PreAllAttributes = all;
                    return;
                }

                if (typeof(IMerged).IsAssignableFrom(this.FromType))
                {
                    {
                        this.IsMergedImage = true;
                        this.FilteredAttributes = this.FromType.ResolveAttributes(this.EarlyBoundEntityType, typeof(Boruto.Attributes.TargetFilterAttribute), out bool all);
                        this.FilteredAllAttributes = all;
                    }

                    {
                        this.PreAttributes = this.FromType.ResolveAttributes(this.EarlyBoundEntityType, null, out bool all);
                        this.PreAllAttributes = all;
                    }
                    return;
                }

                if (typeof(IPostImage).IsAssignableFrom(this.FromType))
                {
                    this.IsPostImage = true;
                    this.PostAttributes = this.FromType.ResolveAttributes(this.EarlyBoundEntityType, null, out bool all);
                    this.PostAllAttributes = all;
                    return;
                }

                if (this.FromType.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
                {
                    switch (this.parameterinfo.Name.ToLower())
                    {
                        case "target":
                            {
                                this.IsTarget = true;
                                this.FilteredAllAttributes = true;
                                return;
                            }
                        case "merged":
                            {
                                this.IsMergedImage = true;
                                this.PreAllAttributes = true;
                                return;
                            }
                        case "preimage":
                            {
                                this.IsPreImage = true;
                                this.PreAllAttributes = true;
                                return;
                            }
                        case "postimage":
                            {
                                this.IsPostImage = true;
                                this.PostAllAttributes = true;
                                return;
                            }
                        default:
                            throw new Exceptions.NamingConventionViolationException(this.parameterinfo.Name, "target", "merged", "preimage", "postimage");
                    }
                }

                if (typeof(ITargetReference).IsAssignableFrom(this.FromType))
                {
                    this.IsTargetReference = true;
                    return;
                }

                if (this.FromType == typeof(Microsoft.Xrm.Sdk.EntityReference))
                {
                    this.IsTargetReference = true;
                    return;
                }
            }

            if (typeof(Microsoft.Xrm.Sdk.OrganizationRequest).IsAssignableFrom(this.FromType))
            {
                this.IsOrganizationRequest = true;
                return;
            }
        }
    }
}
