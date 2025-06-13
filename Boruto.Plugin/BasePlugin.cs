using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Boruto
{
    public abstract class BasePlugin : IPlugin
    {
        private readonly string unsecure;
        private readonly string secureString;

        public BasePlugin()
        {
        }

        public BasePlugin(string unsecure, string secureString)
        {
            this.unsecure = unsecure;
            this.secureString = secureString;
        }

        public void Execute(IServiceProvider platformServiceProvider)
        {
            var assemblies = new List<Assembly>();
            if (this.ServiceAssemblies != null && this.ServiceAssemblies.Length > 0)
            {
                assemblies.AddRange(this.ServiceAssemblies);
            }

            var me = this.GetType().Assembly;

            if (!assemblies.Contains(me))
            {
                assemblies.Add(me);
            }

            using (var ctx = this.GetContext(platformServiceProvider))
            {
                try
                {
                    ctx.Execute();
                } catch (Exception ex)
                {
                    if (ex is Microsoft.Xrm.Sdk.InvalidPluginExecutionException)
                    {
                        throw;
                    }
                    Boruto.Trace.Error(ex.GetType().FullName);
                    Boruto.Trace.Error(ex.Message);
                    Boruto.Trace.Error(ex.StackTrace);
                    var inner = ex.InnerException;
                    while (inner != null)
                    {
                        Boruto.Trace.Error($"-> {inner.Message}");
                        Boruto.Trace.Error($"   {inner.StackTrace}");
                        inner = inner.InnerException;
                    }

                    var tobethrown = ex.InnerException;
                    while (tobethrown != null)
                    {
                        if (tobethrown is Microsoft.Xrm.Sdk.InvalidPluginExecutionException)
                        {
                            throw tobethrown;
                        }
                        tobethrown = tobethrown.InnerException;
                    }

                    throw new InvalidPluginExecutionException($"Unexpected exception: { ex.GetType().FullName }: {ex.Message}");
                }
                finally
                {
                    if (ctx.CustomServiceProvider != null && ctx.CustomServiceProvider is System.IDisposable dis)
                    {
                        dis.Dispose();
                    }
                }
            }
        }


        internal PluginContext GetContext(IServiceProvider platformServiceProvider)
        {
            var res =  new PluginContext(this, platformServiceProvider, this.ServiceAssemblies, unsecure, secureString);
            res.SetCustomServiceProvider(this.ServiceProvider(res));
            return res;
        }

        protected virtual IServiceProvider ServiceProvider(ServiceAPI.IServiceContext ServiceContext) 
        {
            return null;
        }

        protected abstract Assembly[] ServiceAssemblies { get; }
    }
}
