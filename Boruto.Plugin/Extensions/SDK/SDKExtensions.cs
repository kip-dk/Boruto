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
            result.Id = entity.Id;
            result.LogicalName = entity.LogicalName;
            return result;
        }

        public static T ToEntity<T>(this Microsoft.Xrm.Sdk.Entity entity, Type type) where T : Microsoft.Xrm.Sdk.Entity
        {
            var t = (T)System.Activator.CreateInstance(type);
            t.Attributes = entity.Attributes;
            t.Id = entity.Id;
            t.LogicalName = entity.LogicalName;
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
            next.Id = entity.Id;
            next.LogicalName = entity.LogicalName;
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


        /// <summary>
        /// return the strongly type value of an attribut by its name or default if not in the attributes collection, or null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributes"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
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

                return obj.ToValueType<T>();
            }

            return default(T);
        }

        /// <summary>
        /// Returns true if the attrName is part of the Target entity payload (assigned in the process), otherwise false
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static bool IsTargetAttribute(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string attrName)
        {
            if (!string.IsNullOrEmpty(attrName) && ctx.InputParameters["Target"] is Microsoft.Xrm.Sdk.Entity target)
            {
                return target.Attributes.ContainsKey(attrName.ToLower());
            }
            return false;
        }

        /// <summary>
        /// Returns true if the attrName is in the target payload, and it is assigned the null value
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static bool IsSetTargetNull(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string attrName)
        {
            if (!string.IsNullOrEmpty(attrName) && ctx.InputParameters["Target"] is Microsoft.Xrm.Sdk.Entity target)
            {
                var att = attrName.ToLower();
                return target.Attributes.ContainsKey(att) && target.Attributes[att] == null;
            }
            return false;
        }

        public static T TargetValueOf<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string attrName)
        {
            if (!string.IsNullOrEmpty(attrName) && ctx.InputParameters["Target"] is Microsoft.Xrm.Sdk.Entity target)
            {
                return target.Attributes.ValueOf<T>(attrName);
            }

            return default(T);
        }


        /// <summary>
        /// This method is intended for MERGED IMAGE only and will return the prevalue directly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributes"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static T PreValueOf<T>(this Microsoft.Xrm.Sdk.AttributeCollection attributes, string attrName)
        {
            return attributes.ValueOf<T>($"preimage_{attrName}");
        }

        /// <summary>
        /// find the attr value in the preentityimages payload, and if found, returns the value T, otherwise it returns default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>

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

        /// <summary>
        /// find the att value in the postentityimage payload and if found, returns the value T, otherwise it returns default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
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

        public static T ToValueType<T>(this object value)
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
