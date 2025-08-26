using Boruto.Deployment.Services;
using Microsoft.Crm.Sdk.Messages;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.UI.WebControls.WebParts;

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

            if (typeof(IEntity).IsAssignableFrom(value))
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

        private static Dictionary<Type, string> typeToLogicalNameMap = new Dictionary<Type, string>();
        public static string ToIQueryableLogicalName(this Type type)
        {
            if (typeToLogicalNameMap.TryGetValue(type, out string s))
            {
                return s;
            }

            if (type.IsQueryable())
            {
                return type.GetGenericArguments().First().ToEntityLogicalName();
            }

            if (type.IsRepository())
            {

            }

            throw new Exceptions.TypeNotEntityType(type, true);
        }

        public static string ToEntityLogicalName(this Type type)
        {
            if (!type.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
                throw new Exceptions.TypeNotEntityType(type, true);
            }

            if (typeToLogicalNameMap.TryGetValue(type, out string s))
            {
                return s;
            }

            while (type.BaseType != typeof(Microsoft.Xrm.Sdk.Entity))
            {
                type = type.BaseType;
            }

            var ent = (Microsoft.Xrm.Sdk.Entity)System.Activator.CreateInstance(type);
            typeToLogicalNameMap[type] = ent.LogicalName;
            return typeToLogicalNameMap[type];
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


            var types = fromType.ResolveEntityTypes(assms);

            if (types != null && types.Length > 0)
            {
                foreach (var type in types)
                {
                    var ebType = type.ToEarlyBoundEntityType(assms);
                    var ln = ebType.GetEntity().LogicalName;

                    if (ln == logicalName)
                    {
                        resolvedEntityTypes[key] = type;
                        return resolvedEntityTypes[key];
                    }
                }
            }

            if (!fromType.IsInterface && !fromType.IsAbstract && fromType.HasPublicConstructor())
            {
                if (
                    typeof(ITarget).IsAssignableFrom(fromType) 
                    || typeof(IPreImage).IsAssignableFrom(fromType) 
                    || typeof(IMerged).IsAssignableFrom(fromType)
                    || typeof(IPostImage).IsAssignableFrom(fromType))
                {
                    resolvedEntityTypes[key] = fromType;
                    return resolvedEntityTypes[key];
                }
            }
            resolvedEntityTypes[key] = null;
            return null;
        }

        private static readonly Dictionary<System.Reflection.MethodInfo, Type[]> METHOD_ENTITY_TYPES = new Dictionary<MethodInfo, Type[]>();
        internal static Type[] ResolveEntityTypes(this System.Reflection.MethodInfo method, Assembly[] assms)
        {
            if (METHOD_ENTITY_TYPES.TryGetValue(method, out Type[] t))
            {
                return t;
            }

            var result = new List<Type>();

            var resolved = false;
            // resolve entity type from return parameter single entity
            if (method.ReturnType != null && method.ReturnType.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
            {
                result.Add(method.ReturnType);
                resolved = true;
            }

            if (!resolved)
            {
                // resolve entity type from return parameter. collection
                if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Boruto.EntityCollection<>))
                {
                    var gType = method.ReturnType.GetGenericArguments().First();
                    result.Add(gType);
                    resolved = true;
                }
            }

            if (!resolved)
            {
                // resolve entity type from entity type decorations
                var entityTypeAttrs = method.GetCustomAttributes<Boruto.Attributes.EntityTypeAttribute>()?.ToArray();
                if (entityTypeAttrs != null && entityTypeAttrs.Length > 0)
                {
                    result.AddRange(entityTypeAttrs.Select(r => r.Type));
                    resolved = true;
                }
            }

            if (!resolved)
            {
                var pms = method.GetParameters();
                if (pms != null && pms.Length > 0)
                {
                    foreach (var pm in pms)
                    {
                        var types = pm.ParameterType.ResolveEntityTypes(assms);
                        if (types != null && types.Length > 0)
                        {
                            result.AddRange(types);
                            resolved = true;
                            break;
                        }
                    }
                }
            }

            if (resolved && result.Count > 0)
            {
                METHOD_ENTITY_TYPES[method] = result.ToArray();
                return METHOD_ENTITY_TYPES[method];
            }

            return null;
        }

        private static readonly object locker = new object();
        public static Type ResolveEntityType(this System.Reflection.MethodInfo method, string logicalName, Assembly[] assms)
        {
            lock (locker)
            {
                var types = method.ResolveEntityTypes(assms);
                if (types != null && types.Length > 0)
                {
                    foreach (var type in types)
                    {
                        var ln = type.ToEarlyBoundEntityType(assms).GetEntity().LogicalName;
                        if (ln == logicalName)
                        {
                            return type;
                        }
                    }
                }
                return null;
            }
        }

        private static readonly Dictionary<Type, Type[]> TYPE_TO_ENTITYTYPE = new Dictionary<Type, Type[]>();
        private static Type[] ResolveEntityTypes(this Type type, Assembly[] assms)
        {
            if (!type.IsEntityType())
            {
                return null; 
            }

            if (TYPE_TO_ENTITYTYPE.TryGetValue(type, out Type[] ts))
            {
                return ts;
            }

            var result = new List<Type>();
            var resolved = false;

            if (type.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
                result.Add(type);
                resolved = true;
            }

            if (!resolved)
            {
                if (type.IsInterface)
                {
                    var types = type.GetEntityTypeImplementations(assms);
                    if (types != null && types.Length > 0)
                    {
                        result.AddRange(types);
                        resolved = true;
                    }
                }
            }

            TYPE_TO_ENTITYTYPE[type] = result.ToArray();
            return TYPE_TO_ENTITYTYPE[type];
        }

        public static Type ResolveImplementingType(this Type source, Assembly[] assemblies)
        {
            foreach (var ass in assemblies)
            {
                var can = (from t in ass.GetTypes()
                           where t.IsInterface == false
                             && t.IsAbstract == false
                             && source.IsAssignableFrom(t)
                             && t.HasPublicConstructor()
                           select t).FirstOrDefault();

                if (can != null)
                {
                    return can;
                }
            }
            return null;
        }

        public static Type ToEarlyBoundEntityType(this Type type, Assembly[] assms)
        {
            if (!type.IsEntityType())
            {
                throw new Exceptions.TypeNotEntityType(type);
            }

            if (type.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
                var t = type;

                while (t.BaseType != typeof(Microsoft.Xrm.Sdk.Entity))
                {
                    t = t.BaseType;
                }
                return t;
            }

            if (type.IsInterface || type.IsAbstract || !type.HasPublicConstructor())
            {
                throw new Exceptions.TypeNotEntityType(type);
            }

            if (typeof(IEntity).IsAssignableFrom(type))
            {
                var ti = type.CreateInstance<IEntity>();
                return ti.LogicalName.GetRootBoundEntityType(assms);
            }

            return null;
        }

        private static readonly Dictionary<string, Type> EBT = new Dictionary<string, Type>();
        public static Type GetRootBoundEntityType(this string logicalname, Assembly[] assms)
        {
            if (EBT.TryGetValue(logicalname, out Type t))
            {
                return t;
            }

            foreach (var assm in assms)
            {
                foreach (var type in assm.GetTypes())
                {
                    if (type.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                    {
                        EBT[logicalname] = type;
                        return EBT[logicalname];
                    }
                }
            }
            throw new Exceptions.UnresolveableEntityTypeException(logicalname);
        }

        public static T CreateInstance<T>(this Type type)
        {
            var con = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).OrderBy(r => r.GetParameters().Length).FirstOrDefault();
            if (con == null)
            {
                throw new Exceptions.MissingDefaultConstructorException(type);
            }
            var pms = con.GetParameters();
            var res = new object[pms.Length];
            for (var i=0;i<pms.Length;i++)
            {
                res[i] = pms[i].ParameterType.DefaultValue();
            }
            return (T)System.Activator.CreateInstance(type, res);
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
