using Boruto.Extensions.SDK;
using Microsoft.Xrm.Sdk;

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
        /// <summary>
        /// Determin if an attribute is part of the target payload
        /// </summary>
        /// <param name="attrName"></param>
        /// <returns>returns true if the attribute is part of the target payload</returns>
        [System.Diagnostics.DebuggerNonUserCode()]

        public static bool IsTargetAttribute(this string attrName)
        {
            var ctx = ThrowIfNotInPluginExecutionContext();
            return ctx.IsTargetAttribute(attrName);
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

        private static Microsoft.Xrm.Sdk.IPluginExecutionContext ThrowIfNotInPluginExecutionContext()
        {
            if (Boruto.PluginContext.Current == null)
            {
                throw new InvalidPluginExecutionException("This extension method can only be called within a plugin execution context");
            }
            return Boruto.PluginContext.Current.PluginExecutionContext;
        }
    }
}
