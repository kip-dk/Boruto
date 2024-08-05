using Boruto.Extensions.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Reflection.Model
{
    internal class Argument
    {
        private readonly Type pluginType;
        private readonly MethodInfo method;
        private readonly ParameterInfo parameterinfo;
        private readonly string primaryLogicalName;

        internal Argument(Type pluginType, System.Reflection.MethodInfo method, System.Reflection.ParameterInfo parameterinfo, string primaryLogicalName)
        {
            this.Name = parameterinfo.Name;
            this.IsMethodArgument = true;
            this.pluginType = pluginType;
            this.method = method;
            this.parameterinfo = parameterinfo;
            this.primaryLogicalName = primaryLogicalName;
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
        internal string LogicalName { get; private set; }
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

        private void Resolve()
        {
            this.FromType = this.parameterinfo.ParameterType;

            if (this.primaryLogicalName != null)
            {
                if (typeof(ITarget).IsAssignableFrom(this.FromType))
                {
                    this.IsTarget = true;
                    this.ResolveLogicalName();
                    this.ResolveAttributes();
                    return;
                }

                if (typeof(IPreImage).IsAssignableFrom(this.FromType))
                {
                    this.IsPreImage = true;
                    this.ResolveLogicalName();
                    this.ResolveAttributes();
                    return;
                }

                if (typeof(IMerged).IsAssignableFrom(this.FromType))
                {
                    this.IsMergedImage = true;
                    this.ResolveLogicalName();
                    this.ResolveAttributes();
                    return;
                }

                if (typeof(IPostImage).IsAssignableFrom(this.FromType))
                {
                    this.IsPostImage = true;
                    this.ResolveLogicalName();
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
                                this.ResolveLogicalName();
                                return;
                            }
                        case "merged":
                            {
                                this.IsMergedImage = true;
                                this.PreAllAttributes = true;
                                this.ResolveLogicalName();
                                return;
                            }
                        case "preimage":
                            {
                                this.IsPreImage = true;
                                this.PreAllAttributes = true;
                                this.ResolveLogicalName();
                                this.ResolveLogicalName();
                                return;
                            }
                        case "postimage":
                            {
                                this.IsPostImage = true;
                                this.PostAllAttributes = true;
                                this.ResolveLogicalName();
                                return;
                            }
                    }
                }

                if (typeof(ITargetReference).IsAssignableFrom(this.FromType))
                {
                    this.IsTargetReference = true;
                    this.ResolveLogicalName();
                    return;
                }

                if (this.FromType == typeof(Microsoft.Xrm.Sdk.EntityReference))
                {
                    this.IsTargetReference = true;
                    this.ResolveLogicalName();
                    return;
                }
            }
        }

        private void ResolveLogicalName()
        {
            if (this.FromType.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
                var strongType = this.FromType.StrongTypeOf();
                this.LogicalName = strongType.LogicalName;
                this.EarlyBoundEntityType = strongType.GetType();
                return;
            }

            var etas = this.method.GetCustomAttributes<Attributes.EntityTypeAttribute>();
            if (etas != null)
            {
                foreach (var eta in etas)
                {
                    var stronType = eta.Type.StrongTypeOf();
                    if (stronType.LogicalName == this.primaryLogicalName)
                    {
                        this.LogicalName = stronType.LogicalName;
                        this.EarlyBoundEntityType = stronType.GetType();
                        return;
                    }
                }
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
