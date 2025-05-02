using Microsoft.Xrm.Sdk;
using System;
using System.Web;

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

        public static T ValueOf<T>(this Microsoft.Xrm.Sdk.AttributeCollection attributes, string attrName)
        {
            attrName = attrName.ToLower();

            if (attributes != null && attributes.ContainsKey(attrName))
            {
                var obj = attributes[attrName];

                if (obj == null)
                {
                    return default(T);
                }

                return obj.ToTValueType<T>();
            }

            return default(T);
        }

        public static T PreValueOf<T>(this Microsoft.Xrm.Sdk.AttributeCollection attributes, string attrName)
        {
            return attributes.ValueOf<T>($"preimage_{attrName}");
        }

        public static T TargetValueOf<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string attrName)
        {
            if (!string.IsNullOrEmpty(attrName) && ctx.InputParameters["Target"] is Microsoft.Xrm.Sdk.Entity target)
            {
                return target.Attributes.ValueOf<T>(attrName);
            }
            return default(T);
        }

        public static T PreValueOf<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string attrName)
        {
            if (!string.IsNullOrEmpty(attrName) && ctx.PreEntityImages != null)
            {
                foreach (var preimage in ctx.PreEntityImages)
                {
                    var entity = preimage.Value;
                    if (entity.Attributes.ContainsKey(attrName.ToLower()))
                    {
                        return entity.Attributes.ValueOf<T>(attrName);
                    }
                }
            }
            return default(T);
        }

        public static T PostValueOf<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string attrName)
        {
            if (!string.IsNullOrEmpty(attrName) && ctx.PostEntityImages != null)
            {
                foreach (var postimage in ctx.PostEntityImages)
                {
                    var entity = postimage.Value;
                    if (entity.Attributes.ContainsKey(attrName.ToLower()))
                    {
                        return entity.Attributes.ValueOf<T>(attrName);
                    }
                }
            }
            return default(T);
        }

        public static T ToTValueType<T>(this object value)
        {
            if (value is T t)
            {
                return t;
            }

            var type = typeof(T);
            var nullType = Nullable.GetUnderlyingType(type);
            if (nullType != null)
            {
                type = nullType;
            }

            if (type.IsAssignableFrom(value.GetType()))
            {
                return (T)value;
            }

            {
                if (type.IsEnum && value is Microsoft.Xrm.Sdk.OptionSetValue os)
                {
                    return (T)Enum.Parse(type, os.Value.ToString());
                }
            }

            if (value is Microsoft.Xrm.Sdk.OptionSetValueCollection oss)
            {
                if (type.IsArray)
                {
                    var arrayType = type.GetElementType();

                    if (arrayType.IsEnum)
                    {
                        var array = Array.CreateInstance(arrayType, oss.Count);
                        var ix = 0;
                        foreach (var v in oss)
                        {
                            array.SetValue(Enum.Parse(arrayType, v.Value.ToString()), ix);
                            ix++;
                        }
                        return (T)(object)array;
                    }
                }
            }
            throw new InvalidPluginExecutionException($"Unable to convert value of type {value.GetType().FullName} to {typeof(T).FullName}");
        }
    }
}
