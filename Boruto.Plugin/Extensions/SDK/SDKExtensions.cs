using Microsoft.Xrm.Sdk;
using System;
using System.Drawing;
using System.Linq;
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
            if (!string.IsNullOrEmpty(attrName) && ctx.InputParameters != null && ctx.InputParameters.ContainsKey("Target") && ctx.InputParameters["Target"] is Microsoft.Xrm.Sdk.Entity target)
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
            if (!string.IsNullOrEmpty(attrName) && ctx.InputParameters != null && ctx.InputParameters != null && ctx.InputParameters.ContainsKey("Target") && ctx.InputParameters.ContainsKey("Target") && ctx.InputParameters["Target"] is Microsoft.Xrm.Sdk.Entity target)
            {
                var att = attrName.ToLower();
                return target.Attributes.ContainsKey(att) && target.Attributes[att] == null;
            }
            return false;
        }

        public static T TargetValueOf<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string attrName)
        {
            if (!string.IsNullOrEmpty(attrName) && ctx.InputParameters != null && ctx.InputParameters.ContainsKey("Target") && ctx.InputParameters["Target"] is Microsoft.Xrm.Sdk.Entity target)
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

        public static void Clear(this Microsoft.Xrm.Sdk.Client.OrganizationServiceContext ctx)
        {
            if (ctx != null)
            {
                var ats = ctx.GetAttachedEntities().ToArray();
                if (ats.Length > 0)
                {
                    foreach (var at in ats)
                    {
                        ctx.Detach(at);
                    }
                }
            }
        }

        public static bool IsOnlyTargetPayload(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, params string[] expectedAttributes)
        {
            if (ctx.InputParameters.TryGetValue<Microsoft.Xrm.Sdk.Entity>("Target", out Microsoft.Xrm.Sdk.Entity e))
            {
                expectedAttributes = expectedAttributes.Select(r => r.ToLower()).Distinct().ToArray();

                foreach (var key in e.Attributes.Keys)
                {
                    switch (key)
                    {
                        case "createdon":
                        case "createdby":
                        case "createdonbehalfby":
                        case "modifiedon":
                        case "modifiedby":
                        case "modifiedonbehalfby":
                            continue;
                        default:
                            {
                                var v = e[key];
                                if (v is Guid && key == "activityid" || key == $"{e.LogicalName}id") continue;

                                if (expectedAttributes.Contains(key))
                                {
                                    continue;
                                }
                                return false;
                            }
                    }
                }
                return true;
            }
            return false;
        }

        public static Microsoft.Xrm.Sdk.Entity TargetEntity(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx)
        {
            if (ctx.InputParameters.TryGetValue<Microsoft.Xrm.Sdk.Entity>("Target", out Microsoft.Xrm.Sdk.Entity e))
            {
                return e;
            }
            return null;
        }

        /// <summary>
        /// Return the target of the parent event. We can only find target for Create and Update parent events.
        /// *** This message might not give the correct answer, if create is triggered by and Action .. use with care and as little as possible ***
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <returns></returns
        [System.Diagnostics.DebuggerNonUserCode()]

        public static T ParentTarget<T>(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string message, Guid id) where T : Microsoft.Xrm.Sdk.Entity, new()
        {
            if (message != "Create" && message != "Update")
            {
                throw new Exceptions.BaseException("ParentTarget method only support search for Create and Update request");
            }

            var parent = ctx.ParentContext;
            if (parent == null)
            {
                return null;
            }

            var proto = new T();

            if (parent.MessageName == message && parent.PrimaryEntityName == proto.LogicalName && parent.PrimaryEntityId == id)
            {
                var result = (Microsoft.Xrm.Sdk.Entity)parent.InputParameters["Target"];
                return result.ToEntity<T>();
            }

            if (ctx.MessageName == "ExecuteTransaction")
            {
                if (ctx.InputParameters.Contains("Requests"))
                {
                    var requests = ctx.InputParameters["Requests"] as Microsoft.Xrm.Sdk.OrganizationRequestCollection;
                    if (requests != null)
                    {
                        foreach (var r in requests)
                        {
                            switch (message)
                            {
                                case "Create":
                                    {
                                        if (r is Microsoft.Xrm.Sdk.Messages.CreateRequest c)
                                        {
                                            if (c.Target.LogicalName == proto.LogicalName && c.Target.Id == id)
                                            {
                                                return c.Target.ToEntity<T>();
                                            }
                                        }
                                        break;
                                    }
                                case "Update":
                                    {
                                        if (r is Microsoft.Xrm.Sdk.Messages.UpdateRequest c)
                                        {
                                            if (c.Target.LogicalName == proto.LogicalName && c.Target.Id == id)
                                            {
                                                return c.Target.ToEntity<T>();
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            return parent.ParentTarget<T>(message, id);
        }

        /// <summary>
        /// Find out if the ctx is OR has a parent contect that match the filter
        /// *** This message might not give the correct answer, if it is triggered by and Action .. use with care and as little as possible ***
        /// </summary>
        /// <param name="ctx">The context</param>
        /// <param name="message">The message, ex Create, Update ..., required</param>
        /// <param name="entityLogicalName">The logical name of the key, if null any, optional</param>
        /// <param name="id">The id of the entity expected to be a parent event, if null any, optional</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static bool IsChildOf(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string message, string entityLogicalName = null, Guid? id = null)
        {
            if (ctx == null)
            {
                return false;
            }

            if (ctx.MessageName == message && (entityLogicalName == null || ctx.PrimaryEntityName == entityLogicalName) && (id == null || ctx.PrimaryEntityId == id))
            {
                return true;
            }

            if (ctx.MessageName == "ExecuteTransaction")
            {
                if (ctx.InputParameters.Contains("Requests"))
                {
                    try
                    {
                        var requests = ctx.InputParameters["Requests"] as Microsoft.Xrm.Sdk.DataCollection<Microsoft.Xrm.Sdk.OrganizationRequest>;
                        if (requests != null)
                        {
                            foreach (var r in requests)
                            {
                                try
                                {
                                    switch (message)
                                    {
                                        case "Create":
                                            {
                                                if (r is Microsoft.Xrm.Sdk.Messages.CreateRequest c)
                                                {
                                                    if ((entityLogicalName == null || c.Target.LogicalName == entityLogicalName) && (id == null || id == c.Target.Id)) return true;
                                                }
                                                break;
                                            }
                                        case "Update":
                                            {
                                                if (r is Microsoft.Xrm.Sdk.Messages.UpdateRequest c)
                                                {
                                                    if ((entityLogicalName == null || c.Target.LogicalName == entityLogicalName) && (id == null || id == c.Target.Id)) return true;
                                                }
                                                break;
                                            }
                                        case "Delete":
                                            {
                                                if (r is Microsoft.Xrm.Sdk.Messages.DeleteRequest c)
                                                {
                                                    if ((entityLogicalName == null || c.Target.LogicalName == entityLogicalName) && (id == null || id == c.Target.Id)) return true;
                                                }
                                                break;
                                            }
                                    }

                                    if (r.RequestName == message)
                                    {
                                        if (entityLogicalName == null && id == null)
                                        {
                                            return true;
                                        }

                                        if (r.Parameters.ContainsKey("Target") && r.Parameters["Target"] is Microsoft.Xrm.Sdk.EntityReference targetid)
                                        {
                                            if (targetid.LogicalName == entityLogicalName && (id == null || id == targetid.Id))
                                            {
                                                return true;
                                            }
                                        }

                                        if (r.Parameters.ContainsKey("Target") && r.Parameters["Target"] is Microsoft.Xrm.Sdk.Entity target)
                                        {
                                            if (target.LogicalName == entityLogicalName && (id == null || id == target.Id))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (ex.Message == null || !ex.Message.Contains("contains data from a type that maps to the name"))
                                    {
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == null || !ex.Message.Contains("contains data from a type that maps to the name"))
                        {
                            throw;
                        }
                    }
                }
            }
            return ctx.ParentContext.IsChildOf(message, entityLogicalName, id);
        }

        public static Microsoft.Xrm.Sdk.IPluginExecutionContext ParentContext(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, string logicalName, string message)
        {
            if (ctx.ParentContext != null)
            {
                if (ctx.ParentContext.PrimaryEntityName == logicalName && ctx.ParentContext.MessageName == message)
                {
                    return ctx.ParentContext;
                }

                if (logicalName == null && ctx.ParentContext.PrimaryEntityName == null && ctx.ParentContext.MessageName == message)
                {
                    return ctx.ParentContext;
                }

                return ctx.ParentContext.ParentContext(logicalName, message);
            }
            return null;
        }

        /// <summary>
        /// Find the Target object in the parent context
        /// *** This message might not give the correct answer, if it is triggered by and Action .. use with care and as little as possible ***
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="logicalName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static T ParentTarget<T>(this IPluginExecutionContext ctx, string logicalName, string message)
        {
            var parent = ctx.ParentContext(logicalName, message);
            if (parent != null && parent.InputParameters.TryGetValue("Target", out object targetValue) && targetValue != null)
            {
                var returnType = typeof(T);
                if (returnType.IsAssignableFrom(targetValue.GetType()))
                {
                    return (T)targetValue;
                }

                if (returnType.BaseType == typeof(Microsoft.Xrm.Sdk.Entity))
                {
                    if (targetValue is Microsoft.Xrm.Sdk.Entity e)
                    {
                        var result = System.Activator.CreateInstance(returnType) as Microsoft.Xrm.Sdk.Entity;
                        result.Attributes = e.Attributes;
                        return (T)(object)result;
                    }
                }

                if (returnType.BaseType == typeof(Microsoft.Xrm.Sdk.OrganizationRequest))
                {
                    if (targetValue is Microsoft.Xrm.Sdk.OrganizationRequest e)
                    {
                        var result = System.Activator.CreateInstance(returnType) as Microsoft.Xrm.Sdk.OrganizationRequest;
                        result.Parameters = e.Parameters;
                        return (T)(object)result;
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// Extension method to prevent a field is set to null
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="target"></param>
        /// <param name="field"></param>
        /// <exception cref="InvalidPluginExecutionException"></exception>
        public static void Required(this Microsoft.Xrm.Sdk.IPluginExecutionContext ctx, Microsoft.Xrm.Sdk.Entity target, string field)
        {
            field = field.ToLower();

            if (ctx.MessageName == "Create" && !target.Attributes.Contains(field))
            {
                throw new InvalidPluginExecutionException($"{field} on {target.LogicalName} is mandatory");
            }

            if (ctx.MessageName == "Create" && target[field] == null)
            {
                throw new InvalidPluginExecutionException($"{field} on {target.LogicalName} is mandatory");
            }

            if (ctx.MessageName == "Update" && ctx.IsTargetAttribute(field) && target[field] == null)
            {
                throw new InvalidPluginExecutionException($"{field} cannot be set to null");
            }
        }
    }
}
