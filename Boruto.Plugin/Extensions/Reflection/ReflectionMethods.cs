using System;
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
                               && t.HasPublicDefaultConstructor()
                             select t).ToArray();

                foreach (var sType in types)
                {
                    var instance = System.Activator.CreateInstance(sType);

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

        public static bool HasPublicDefaultConstructor(this Type type)
        {
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Where(r => { var pms = r.GetParameters(); return pms == null || pms.Length == 0; }).Any();
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
