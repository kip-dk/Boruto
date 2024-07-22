using Microsoft.Xrm.Sdk;
using System;

namespace Boruto.Extensions.SDK
{
    public static class SDKExtensions
    {
        public static T ToEntity<T>(this Microsoft.Xrm.Sdk.Entity entity) where T : Microsoft.Xrm.Sdk.Entity, new()
        {
            if (entity is T t)
            {
                return t;
            }

            var result = new T();
            result.Attributes = entity.Attributes;
            return result;
        }

        public static T ToEntity<T>(this Microsoft.Xrm.Sdk.Entity entity, Type type) where T : Microsoft.Xrm.Sdk.Entity
        {
            var t = (T)System.Activator.CreateInstance(type);
            t.Attributes = entity.Attributes;
            return t;
        }

        public static Microsoft.Xrm.Sdk.Entity StrongTypeOf(this Microsoft.Xrm.Sdk.Entity entity, Type type)
        {
            if (type.IsAssignableFrom(entity.GetType()))
            {
                return entity;
            }

            var next = type.StrongTypeOf();
            next.Attributes = entity.Attributes;
            return next;
        }

        public static Microsoft.Xrm.Sdk.Entity StrongTypeOf(this Type type)
        {
            if (type.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
                var t = (Microsoft.Xrm.Sdk.Entity)System.Activator.CreateInstance(type);
                t.Attributes = t.Attributes;
                return t;
            } else
            {
                throw new Exceptions.TypeNotEntityType(type);
            }
        }
    }
}
