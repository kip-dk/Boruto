﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Boruto.Extensions.Reflection
{
    public static class ReflectionMethods
    {
        public static object DefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return System.Activator.CreateInstance(type);
            }
            return null;
        }

        public static bool IsEntityType(this Type value)
        {
            if (typeof(Microsoft.Xrm.Sdk.Entity).IsAssignableFrom(value))
            {
                return true;
            }

            if (typeof(ITarget).IsAssignableFrom(value))
            {
                return true;
            }

            if (typeof(IPreImage).IsAssignableFrom(value))
            {
                return true;
            }

            if (typeof(IMerged).IsAssignableFrom(value))
            {
                return true;
            }

            if (typeof(IPostImage).IsAssignableFrom(value))
            {
                return true;
            }

            return false;
        }

        public static bool IsRepository(this Type type)
        {
            return type.IsInterface && type.IsGenericType && type.FullName.StartsWith("Boruto.IRepository") && type.GetGenericArguments().First().IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity));
        }

        public static bool IsQueryable(this Type type)
        {
            return type.IsInterface && type.IsGenericType && type.FullName.StartsWith("System.Linq.IQueryable") && type.GetGenericArguments().First().IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity));
        }

        private static Dictionary<string, Type> resolvedEntityTypes = new Dictionary<string, Type>();
        public static Type ResolveEntityType(this Type fromType, string logicalName, Assembly[] assms)
        {
            var key = $"{fromType.FullName}:{logicalName}";

            {
                if (resolvedEntityTypes.TryGetValue(key, out Type type))
                {
                    return type;
                }
            }

            if (fromType == typeof(Microsoft.Xrm.Sdk.Entity))
            {
                resolvedEntityTypes[key] = fromType;
                return fromType;
            }

            if (fromType.IsInterface == false && fromType.IsAbstract == false)
            {
                var constructors = fromType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                if (constructors == null || constructors.Length == 0)
                {
                    return null;
                }

                var defaultConstructor = constructors.Where(r => { var pms = r.GetParameters(); return pms == null || pms.Length == 0; }).SingleOrDefault();
                if (defaultConstructor == null)
                {
                    return null;
                }

                if (fromType.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
                {
                    var entity = (Microsoft.Xrm.Sdk.Entity)System.Activator.CreateInstance(fromType);

                    if (entity.LogicalName == logicalName)
                    {
                        resolvedEntityTypes[key] = fromType;
                        return fromType;
                    }
                    return null;
                }
            }

            foreach (var assm in assms)
            {
                var types = (from t in assm.GetTypes()
                             where t.IsAbstract == false
                               && t.IsInterface == false
                               && fromType.IsAssignableFrom(t)
                               && t.HasPublicConstructor()
                             select t).ToArray();

                foreach (var sType in types)
                {
                    var useType = sType;

                    while (useType.BaseType.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
                    {
                        useType = sType.BaseType;
                    }

                    object instance = null;

                    if (!useType.HasPublicDefaultConstructor())
                    {
                        var constructor = sType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderBy(r => (r.GetParameters() ?? new ParameterInfo[0]).Length).FirstOrDefault();
                        if (constructor == null)
                        {
                            throw new Exceptions.UnresolveableEntityTypeException(sType);
                        }
                        var pms = constructor.GetParameters();
                        var args = new object[pms.Length];
                        var ix = 0;
                        foreach (var pm in pms)
                        {
                            args[ix] = pm.ParameterType.DefaultValue();
                        }
                        instance = constructor.Invoke(args);
                    }
                    else
                    {
                        instance = System.Activator.CreateInstance(useType);
                    }

                    {
                        if (instance is Microsoft.Xrm.Sdk.Entity ent)
                        {
                            if (ent.LogicalName == logicalName)
                            {
                                resolvedEntityTypes[key] = sType;
                                return sType;
                            }
                            continue;
                        }
                    }

                    {
                        if (instance is IEntity ent)
                        {
                            if (ent.LogicalName == logicalName)
                            {
                                resolvedEntityTypes[key] = sType;
                                return sType;
                            }
                            continue;
                        }
                    }
                }
            }

            resolvedEntityTypes[key] = null;
            return null;
        }

        public static Type ResolveImplementingType(this Type source, Assembly[] assemblies)
        {
            foreach (var ass in assemblies)
            {
                var can = (from t in ass.GetTypes()
                           where t.IsInterface == false
                             && t.IsAbstract == false
                             && t.IsAssignableFrom(source)
                             && t.HasPublicConstructor()
                           select t).FirstOrDefault();

                if (can != null)
                {
                    return can;
                }
            }
            return null;
        }

        public static bool HasPublicDefaultConstructor(this Type type)
        {
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Where(r => { var pms = r.GetParameters(); return pms == null || pms.Length == 0; }).Any();
        }

        public static bool HasPublicConstructor(this Type type)
        {
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Any();
        }

        public static string[] ResolveAttributes(this Type fromType, Type toType, Type decorator, out bool allAttributes)
        {
            if (fromType.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
                allAttributes = true;
                return null;
            }

            allAttributes = false;
            List<string> result = new List<string>();

            var interfaceProperties = fromType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(r => r.GetGetMethod(false) != null);

            if (decorator != null)
            {
                interfaceProperties = interfaceProperties.Where(r => r.GetCustomAttribute(decorator) != null);
            }

            var instanceProperties = toType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttribute<Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute>() != null);

            return (from interfaceProperty in interfaceProperties
                    join instanceProperty in instanceProperties on interfaceProperty.Name equals instanceProperty.Name
                    select instanceProperty.GetCustomAttribute<Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute>().LogicalName
                    ).Distinct().ToArray();
        }
    }
}
