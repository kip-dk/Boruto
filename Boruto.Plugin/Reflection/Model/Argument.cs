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

            var admin = method.GetCustomAttribute<Attributes.AdminAttribute>();

            if (admin != null)
            {
                this.Admin = true;
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
                    this.ResolveAttributes();
                    return;
                }

                if (typeof(IPreImage).IsAssignableFrom(this.FromType))
                {
                    this.IsPreImage = true;
                    this.ResolveAttributes();
                    return;
                }

                if (typeof(IMerged).IsAssignableFrom(this.FromType))
                {
                    this.IsMergedImage = true;
                    this.ResolveAttributes();
                    return;
                }

                if (typeof(IPostImage).IsAssignableFrom(this.FromType))
                {
                    this.IsPostImage = true;
                    this.ResolveAttributes();
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

        private void ResolveAttributes()
        {
            if (this.IsTarget)
            {
                this.ResolveTargetAttributes();
                return;
            }

            if (this.IsPreImage || this.IsMergedImage)
            {
                this.ResolvePreAttributes();
                return;
            }

            if (this.IsPostImage)
            {
                this.ResolvePostAttributes();
                return;
            }
        }

        private void ResolveTargetAttributes()
        {
            List<string> result = new List<string>();
            var interfaceProperties = this.FromType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var instanceProperties = this.EarlyBoundEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(r => r.Name);

            foreach (var prop in interfaceProperties)
            {
                if (prop.GetGetMethod(false) != null)
                {
                    var targetAttr = prop.GetCustomAttribute<Attributes.TargetFilterAttribute>();
                    if (targetAttr != null && instanceProperties.TryGetValue(prop.Name, out PropertyInfo instanceProperty))
                    {
                        var ma = instanceProperty.PropertyType.GetCustomAttribute<Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute>();
                        if (ma != null)
                        {
                            result.Add(ma.LogicalName);
                        }
                    }
                }
            }
            this.FilteredAttributes = result.Distinct().ToArray();
        }

        private void ResolvePreAttributes()
        {
            List<string> result = new List<string>();
            var interfaceProperties = this.FromType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var instanceProperties = this.EarlyBoundEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(r => r.Name);

            foreach (var prop in interfaceProperties)
            {
                if (prop.GetGetMethod(false) != null)
                {
                    if (instanceProperties.TryGetValue(prop.Name, out PropertyInfo instanceProperty))
                    {
                        var ma = instanceProperty.PropertyType.GetCustomAttribute<Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute>();
                        if (ma != null)
                        {
                            result.Add(ma.LogicalName);
                        }
                    }
                }
            }
            this.PreAttributes = result.ToArray();
        }

        private void ResolvePostAttributes()
        {
            List<string> result = new List<string>();
            var interfaceProperties = this.FromType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var instanceProperties = this.EarlyBoundEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(r => r.Name);

            foreach (var prop in interfaceProperties)
            {
                if (prop.GetGetMethod(false) != null)
                {
                    if (instanceProperties.TryGetValue(prop.Name, out PropertyInfo instanceProperty))
                    {
                        var ma = instanceProperty.PropertyType.GetCustomAttribute<Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute>();
                        if (ma != null)
                        {
                            result.Add(ma.LogicalName);
                        }
                    }
                }
            }
            this.PostAttributes = result.ToArray();
        }
    }
}
