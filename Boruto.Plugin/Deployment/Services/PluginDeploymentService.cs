using Boruto.Deployment.Models;
using Boruto.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace Boruto.Deployment.Services
{
    [Export(typeof(ServiceAPI.IPluginDeploymentService))]
    internal class PluginDeploymentService : ServiceAPI.IPluginDeploymentService
    {
        private readonly ServiceAPI.IMessageService messageService;
        private int[] stages = new int[] { 10, 20, 40 };
        private string[] entityLogicalNames;

        private static readonly Type BORUTO_PLUGIN = typeof(Boruto.BasePlugin);

        [ImportingConstructor]
        public PluginDeploymentService(ServiceAPI.IMessageService messageService)
        {
            this.messageService = messageService;
        }

        public Models.Plugin[] ForAssembly(Assembly[] assemblies)
        {
            var result = new List<Models.Plugin>();

            foreach (var assm in assemblies)
            {
                foreach (var type in assm.GetTypes())
                {
                    if (!type.IsInterface && !type.IsAbstract && BORUTO_PLUGIN.IsAssignableFrom(type) && type.HasPublicDefaultConstructor())
                    {
                        var allLogicalSteps = new List<Step>();
                        var hasVirtualSteps = false;
                        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(r => r.Name.StartsWith("On")).ToArray();

                        foreach (var method in methods)
                        {
                            var stage = method.Name.ToStage();
                            if (stage == 30)
                            {
                                hasVirtualSteps = true;
                                continue;
                            }

                            var isAsync = method.Name.EndsWith("Async");
                            var message = method.ToMessage();
                            if (string.IsNullOrEmpty(message))
                            {
                                this.messageService.Warning($"Unable to resolve message for: {type.FullName}.{method.Name}. Method will never be invoked");
                                continue;
                            }

                            var logicalNames = method.ToLogicalName(assemblies);
                            if (logicalNames == null && message.RequireLogicalName())
                            {
                                this.messageService.Warning($"Message: {message} require logical name, but it could not be resolved from any parameters to the method: {type.FullName}. {method.Name}. Method will never be invoked");
                                continue;
                            }

                            if (logicalNames == null)
                            {
                                var step = new Models.Step
                                {
                                    ExecutionOrder = method.Sort(),
                                    TargetFilterAttributes = null,
                                    IsAsync = isAsync,
                                    Message = message,
                                    PostImage = null,
                                    PreImage = null,
                                    PrimaryEntityLogicalName = null,
                                    Stage = stage
                                };
                                allLogicalSteps.Add(step);
                                continue;
                            }

                            foreach (var logicalName in logicalNames)
                            {
                                var step = new Models.Step
                                {
                                    ExecutionOrder = method.Sort(),
                                    TargetFilterAttributes = method.TargetFilters(logicalName, assemblies),
                                    IsAsync = isAsync,
                                    Message = message,
                                    PreImage = method.PreImage(message, logicalName, assemblies),
                                    PostImage = method.PostImage(message, logicalName, assemblies),
                                    PrimaryEntityLogicalName = logicalName,
                                    Stage = stage
                                };

                                if (message == "Create" || message == "Delete" || step.TargetFilterAttributes != null || step.PostImage != null || step.PreImage != null)
                                {
                                    allLogicalSteps.Add(step);
                                }
                                continue;
                            }
                        }

                        if (allLogicalSteps.Count == 0 && !hasVirtualSteps)
                        {
                            this.messageService.Warning($"No steps was found for: { type.FullName }");
                            continue;
                        }

                        var steps = (from a in allLogicalSteps
                                     group a by a.PluginRegistrationGroup into grp
                                     select new Step
                                     {
                                         ExecutionOrder = grp.OrderBy(r => r.ExecutionOrder).First().ExecutionOrder,
                                         IsAsync = grp.First().IsAsync,
                                         Message = grp.First().Message,
                                         PrimaryEntityLogicalName = grp.First().PrimaryEntityLogicalName,
                                         Stage = grp.First().Stage,
                                         TargetFilterAttributes = grp.Select(r => r.TargetFilterAttributes).Accumulate(),
                                         PostImage = grp.Select(r => r.PostImage).Accumulate(),
                                         PreImage = grp.Select(r => r.PreImage).Accumulate()
                                     }).ToArray();

                        var next = new Models.Plugin(type, steps,hasVirtualSteps);
                        result.Add(next);
                    }
                }
            }
            return result.ToArray();
        }
    }

    internal static class PluginDeploymentServiceLocalExtensions
    {
        private static readonly Type ORG_REQ = typeof(Microsoft.Xrm.Sdk.OrganizationRequest);
        private static readonly Type ENTITY = typeof(Microsoft.Xrm.Sdk.Entity);
        private static readonly Type ITARGET = typeof(Boruto.ITarget);
        private static readonly Type IPREIMAGE = typeof(Boruto.IPreImage);
        private static readonly Type IMERGED = typeof(Boruto.IMerged);
        private static readonly Type IPOST = typeof(Boruto.IPostImage);

        public static int ToStage(this string name)
        {
            if (name.StartsWith("OnValidate")) return 10;
            if (name.StartsWith("OnPre")) return 20;
            if (name.StartsWith("OnPost")) return 40;
            return 30;
        }

        public static string ToMessage(this MethodInfo method)
        {
            if (method.Name.EndsWith("Create")) return "Create";
            if (method.Name.EndsWith("CreateAsync")) return "Create";
            if (method.Name.EndsWith("Update")) return "Update";
            if (method.Name.EndsWith("UpdateAsync")) return "Update";
            if (method.Name.EndsWith("Delete")) return "Delete";
            if (method.Name.EndsWith("DeleteAsync")) return "Delete";
            if (method.Name.EndsWith("Retrieve")) return "Retrieve";
            if (method.Name.EndsWith("RetrieveAsync")) return "Retrieve";
            if (method.Name.EndsWith("RetrieveMultiple")) return "RetrieveMultiple";
            if (method.Name.EndsWith("RetrieveMultipleAsync")) return "RetrieveMultiple";

            foreach (var arg in method.GetParameters())
            {
                if (ORG_REQ.IsAssignableFrom(arg.ParameterType) && arg.ParameterType.HasPublicDefaultConstructor())
                {
                    var req = (Microsoft.Xrm.Sdk.OrganizationRequest)System.Activator.CreateInstance(arg.ParameterType);
                    return req.RequestName;
                }
            }
            return null;
        }

        public static string[] ToLogicalName(this MethodInfo method, Assembly[] assms)
        {
#warning HER
            var entityTypeAttrs = method.GetCustomAttributes<Boruto.Attributes.EntityTypeAttribute>()?.ToArray();
            if (entityTypeAttrs != null && entityTypeAttrs.Length > 0)
            {
                return entityTypeAttrs.Select(r => r.LogicalName).ToArray();
            }

            foreach (var arg in method.GetParameters())
            {
                var type = arg.ParameterType;
                if (!type.IsInterface && !type.IsAbstract && type.IsSubclassOf(ENTITY))
                {
                    return new string[] { arg.ParameterType.ToLogicalName() };
                }

                if (type.IsInterface && ITARGET.IsAssignableFrom(arg.ParameterType))
                {
                    return type.ToLogicalNames(assms);
                }

                if (type.IsInterface && IPREIMAGE.IsAssignableFrom(arg.ParameterType))
                {
                    return type.ToLogicalNames(assms);
                }

                if (type.IsInterface && IMERGED.IsAssignableFrom(arg.ParameterType))
                {
                    return type.ToLogicalNames(assms);
                }

                if (type.IsInterface && IPOST.IsAssignableFrom(arg.ParameterType))
                {
                    return type.ToLogicalNames(assms);
                }

                if (type.IsInterface && type.IsGenericType && typeof(Boruto.ITargetReference).IsAssignableFrom(type))
                {
                    var entityType = type.GetGenericArguments().First();
                    return new string[] { entityType.ToLogicalName() };
                }
            }
            return null;
        }

        public static Microsoft.Xrm.Sdk.Entity GetEntity(this Type type)
        {
            if (!type.IsSubclassOf(ENTITY))
            {
                return null;
            }
            if (type.BaseType == ENTITY)
            {
                return ((Microsoft.Xrm.Sdk.Entity)System.Activator.CreateInstance(type));
            }
            return type.BaseType.GetEntity();

        }
        public static string ToLogicalName(this Type type)
        {
            return type.GetEntity().LogicalName;
        }

        public static string[] ToLogicalNames(this Type interfaceType, Assembly[] assms)
        {
            var list = new List<string>();
            foreach (var asm in assms)
            {
                foreach (var type in asm.GetTypes())
                {
                    if (!type.IsInterface && !type.IsAbstract && interfaceType.IsAssignableFrom(type) && type.IsSubclassOf(ENTITY))
                    {
                        list.Add(type.ToLogicalName());
                    }
                }
            }
            return list.ToArray();
        }

        public static bool RequireLogicalName(this string value)
        {
            if (value == "Create" || value == "Update" || value == "Delete" || value == "Retrieve" || value == "RetrieveMultiple") return true;
            return false;
        }

        public static int Sort(this MethodInfo method)
        {
            var sort = method.GetCustomAttribute<Boruto.Attributes.SortAttribute>();
            if (sort != null)
            {
                return sort.Value;
            }
            return 1;
        }

        internal static Image PostImage(this MethodInfo method, string message, string logicalName, Assembly[] assms)
        {
            if (message != "Create" && message != "Update" && message != "Delete")
            {
                return null;
            }

            var result = new List<string>();
            foreach (var arg in method.GetParameters())
            {
                var type = arg.ParameterType;
                if (arg.Name.ToLower() == "postimage" && type.IsSubclassOf(ENTITY) && logicalName == type.ToLogicalName())
                {
                    return new Image
                    {
                        AllAttributes = true
                    };
                }

                if (type.IsInterface && IPOST.IsAssignableFrom(type))
                {

                    var implType = type.GetEntityTypeImplementation(logicalName, assms);
                    if (implType != null)
                    {
                        var shared = type.GetEntityAttributes(implType, null);
                        if (shared != null && shared.Length > 0)
                        {
                            result.AddRange(shared);
                        }
                    }
                    continue;
                }
            }

            if (result.Count > 0)
            {
                return new Image
                {
                    AllAttributes = false,
                    FilteredAttributes = result.Distinct().ToArray()
                };
            }
            return null;
        }

        internal static Image PreImage(this MethodInfo method, string message, string logicalName, Assembly[] assms)
        {
            if (message != "Update" && message != "Delete") 
            {
                return null;
            }

            var result = new List<string>();
            foreach (var arg in method.GetParameters())
            {
                var type = arg.ParameterType;
                if (arg.Name.ToLower() == "preimage" && type.IsSubclassOf(ENTITY) && logicalName == type.ToLogicalName())
                {
                    return new Image
                    {
                        AllAttributes = true
                    };
                }

                if (arg.Name.ToLower() == "merged" && type.IsSubclassOf(ENTITY) && logicalName == type.ToLogicalName())
                {
                    return new Image
                    {
                        AllAttributes = true
                    };
                }

                if (arg.Name.ToLower() == "mergedimage" && type.IsSubclassOf(ENTITY) && logicalName == type.ToLogicalName())
                {
                    return new Image
                    {
                        AllAttributes = true
                    };
                }

                if (type.IsInterface && IPREIMAGE.IsAssignableFrom(type))
                {

                    var implType = type.GetEntityTypeImplementation(logicalName, assms);
                    if (implType != null)
                    {
                        var shared = type.GetEntityAttributes(implType, null);
                        if (shared != null && shared.Length > 0)
                        {
                            result.AddRange(shared);
                        }
                    }
                    continue;
                }

                if (type.IsInterface && IMERGED.IsAssignableFrom(type))
                {
                    var implType = type.GetEntityTypeImplementation(logicalName, assms);
                    if (implType != null)
                    {
                        var shared = type.GetEntityAttributes(implType, null);
                        if (shared != null && shared.Length > 0)
                        {
                            result.AddRange(shared);
                        }
                    }
                    continue;
                }
            }

            if (result.Count > 0)
            {
                return new Image
                {
                    AllAttributes =false,
                    FilteredAttributes = result.Distinct().ToArray()
                };
            }
            return null;
        }

        internal static Image TargetFilters(this MethodInfo method, string logicalName, Assembly[] assms)
        {
            var result = new List<string>();

            foreach (var arg in method.GetParameters())
            {
                var type = arg.ParameterType;
                if (arg.Name.ToLower() == "target" && type.IsSubclassOf(ENTITY) && logicalName == type.ToLogicalName())
                {
                    return new Image
                    {
                        AllAttributes = true
                    };
                }

                if (type.IsInterface && ITARGET.IsAssignableFrom(type))
                {

                    var implType = type.GetEntityTypeImplementation(logicalName, assms);
                    if (implType != null)
                    {
                        var shared = type.GetEntityAttributes(implType, null);
                        if (shared != null && shared.Length > 0)
                        {
                            result.AddRange(shared);
                        }
                    }
                    continue;
                }

                if (type.IsInterface && IMERGED.IsAssignableFrom(type))
                {
                    var implType = type.GetEntityTypeImplementation(logicalName, assms);
                    if (implType != null)
                    {
                        var shared = type.GetEntityAttributes(implType, p => p.GetCustomAttribute<Boruto.Attributes.TargetFilterAttribute>() != null);
                        if (shared != null && shared.Length > 0)
                        {
                            result.AddRange(shared);
                        }
                    }
                    continue;
                }
            }

            if (result.Count > 0)
            {
                return new Image
                {
                    AllAttributes = false,
                    FilteredAttributes = result.Distinct().ToArray()
                };
            }
            return null;
        }

        public static Type GetEntityTypeImplementation(this Type interfaceType, string logicalName, Assembly[] assms)
        {
            var types = interfaceType.GetEntityTypeImplementations(assms);
            if (types != null && types.Length > 0)
            {
                foreach (var type in types)
                {
                    var entity = type.GetEntity();
                    if (entity.LogicalName == logicalName)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        private static readonly Dictionary<Type, Type[]> ENTITY_TYPE_IMPL = new Dictionary<Type, Type[]>();
        public static Type[] GetEntityTypeImplementations(this Type interfaceType, Assembly[] assms)
        {
            if (ENTITY_TYPE_IMPL.TryGetValue(interfaceType, out Type[] types))
            {
                return types;
            }
            var result = new List<Type>();

            foreach (var asm in assms)
            {
                foreach (var type in asm.GetTypes())
                {
                    type.IsEntityType();
                    if (!type.IsAbstract && !type.IsInterface && type.IsEntityType() && interfaceType.IsAssignableFrom(type) && type.HasPublicConstructor())
                    {
                        result.Add(type);
                    }
                }
            }

            ENTITY_TYPE_IMPL[interfaceType] = result.ToArray();
            return ENTITY_TYPE_IMPL[interfaceType];
        }


        public static string[] GetEntityAttributes(this Type interfaceType, Type implType, Func<PropertyInfo, bool> interfaceTypeFilter)
        {
            var interfaceTypeProperties = interfaceType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(r => r.CanRead).ToArray();
            if (interfaceTypeFilter != null)
            {
                interfaceTypeProperties = interfaceTypeProperties.Where(r => interfaceTypeFilter(r)).ToArray();
            }

            var impTypeProperties = implType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return (from it in interfaceTypeProperties
                    join im in impTypeProperties on it.Name equals im.Name
                    select im.GetCustomAttribute<Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute>())
              .Where(r => r != null)
              .Select(r => r.LogicalName)
              .Distinct()
              .ToArray();
        }

        internal static Image Accumulate(this IEnumerable<Image> images)
        {
            if (images == null)
            {
                return null;
            }

            var fields = new List<string>();
            foreach (var image in images)
            {
                if (image != null)
                {
                    if (image.AllAttributes)
                    {
                        return image;
                    }

                    if (image.FilteredAttributes != null && image.FilteredAttributes.Length > 0)
                    {
                        fields.AddRange(image.FilteredAttributes);
                    }
                }
            }
            if (fields.Count > 0)
            {
                return new Image
                {
                    AllAttributes = false,
                    FilteredAttributes = fields.Distinct().ToArray()
                };
            }
            return null;
        }
    }
}
