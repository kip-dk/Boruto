using Boruto.Extensions.SDK;
using Boruto.Extensions.TypeConverters;
using Microsoft.Xrm.Sdk;
using System.Web.UI.WebControls;

namespace Boruto.Extensions.Ctx
{
    /// <summary>
    /// Extension method to be used in a plugin execution context only
    /// If any of these methods are used outside a plugin execution context, an exception will be thrown
    /// 
    /// These extension methods servers as a shortcut for some of the extensionmethods of the SDK, to avoid having to inject the pluginexecutioncontext everywhere
    /// </summary>
    public static class CtxExtensions
    {
        public const string CreateMessage = "Create";
        public const string UpdateMessage = "Update";
        public const string DeleteMessage = "Delete";


        /// <summary>
        /// Determin if an attribute is part of the target payload
        /// If others has values, true will be returned if at attrName OR at least one of the attributes in others is part of target payload
        /// </summary>
        /// <param name="attrName">primary attribute name</param>
        /// <param name="others">other attributes</param>
        /// <returns>returns true if the attribute is part of the target payload</returns>
        [System.Diagnostics.DebuggerNonUserCode()]
        public static bool IsTargetAttribute(this string attrName, params string[] others)
        {
            var ctx = ThrowIfNotInPluginExecutionContext();
            return ctx.IsTargetAttribute(attrName, others);
        }


        /// <summary>
        /// Get the target value based on the attribute name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns>return the target value or default(T) if attribut is not within the target payload</returns>
        [System.Diagnostics.DebuggerNonUserCode()]
        public static T TargetValueOf<T>(this string name)
        {
            var ctx = ThrowIfNotInPluginExecutionContext();
            return ctx.TargetValueOf<T>(name);
        }

        /// <summary>
        /// Get the prevalue based on the attribute name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns>Returns the prevalue or default(T) if attribute is not within the preimage payload</returns>
        [System.Diagnostics.DebuggerNonUserCode()]
        public static T PreValueOf<T>(this string name)
        {
            var ctx = ThrowIfNotInPluginExecutionContext();
            return ctx.PreValueOf<T>(name);
        }

        /// <summary>
        /// Returns true if the message parsed match the message name of the current plugin context
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]
        public static bool IsCurrentMessage(this string message)
        {
            var ctx = ThrowIfNotInPluginExecutionContext();
            return ctx.MessageName == message;
        }

        /// <summary>
        /// Returns true if the attribute parsed was changed
        /// If the attribute is not part of the target payload, it will return false
        /// If the attribute is part of the target payload, and is also part of the preimage payload, false will be returned if the values are the same
        /// Otherwise true will be returned
        /// </summary>
        /// <param name="attrName"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]
        public static bool AttributeChanged(this string attrName)
        {
            var ctx = ThrowIfNotInPluginExecutionContext();

            if (attrName.IsTargetAttribute())
            {
                if (ctx.MessageName != UpdateMessage)
                {
                    return true;
                }
                var targetValue = ctx.TargetValueOf<object>(attrName);
                var preValue = ctx.PreValueOf<object>(attrName);

                return !targetValue.IsSame(preValue);
            }
            return false;
        }

        /// <summary>
        /// Returns true if at least one of the parsed attributes has changed value.
        /// </summary>
        /// <param name="attrName"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCode()]
        public static bool AttributesHasChanges(this string attrName, params string[] others)
        {
            var ctx = ThrowIfNotInPluginExecutionContext();

            var first = attrName.AttributeChanged();
            if (first)
            {
                return true;
            }

            if (others != null && others.Length > 0)
            {
                foreach (var other in others)
                {
                    var next = other.AttributeChanged(); 
                    if (next)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static Microsoft.Xrm.Sdk.IPluginExecutionContext ThrowIfNotInPluginExecutionContext()
        {
            if (Boruto.PluginContext.Current == null)
            {
                throw new InvalidPluginExecutionException("This extension method can only be called within a plugin execution context");
            }
            return Boruto.PluginContext.Current.PluginExecutionContext;
        }

        public static Boruto.ServiceAPI.IServiceContext CurrentSericeContext(this Boruto.BasePlugin plugin)
        {
            if (Boruto.PluginContext.Current == null)
            {
                throw new InvalidPluginExecutionException("This extension method can only be called within a plugin execution context");
            }

            return Boruto.PluginContext.Current;
        }

        public static bool IsParentMessage(this string message, params string[] logicalnames)
        {
            if (logicalnames == null || logicalnames.Length == 0)
            {
                return false;
            }

            var ctx = ThrowIfNotInPluginExecutionContext();

            foreach (var logicalname in logicalnames)
            {
                var r = ctx.IsChildOf(message, logicalname);
                if (r == true) return true;
            }
            return false;
        }
    }
}
