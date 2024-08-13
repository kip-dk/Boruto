using Boruto.Extensions.Reflection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Reflection
{
    internal class ServiceFactory : IDisposable
    {
        private readonly PluginContext ctx;

        private Dictionary<Type, object> resolved = new Dictionary<Type, object>();

        internal ServiceFactory(PluginContext ctx)
        {
            this.ctx = ctx;
        }

        internal object Resolve(Reflection.Model.PluginMethodArgument argument)
        {
            var result = this.DoResolve(argument.FromType, argument.IsTargetReference, argument.IsOrganizationRequest, argument.Admin, argument.EarlyBoundEntityType ?? argument.ToType);

            if (argument.EarlyBoundEntityType == null && argument.ToType == null)
            {
                argument.ToType = result.GetType();
            }

            if (argument.IsTarget)
            {
                var notifier = result as System.ComponentModel.INotifyPropertyChanged;
                if (notifier != null && ctx.Stage <= 20 && ctx.Message == "Update")
                {
                    notifier.PropertyChanged += Target_PropertyChanged;
                }

                if (result is Microsoft.Xrm.Sdk.Entity ent)
                {
                    ent.Attributes = this.ctx.Target.Attributes;
                    ent.LogicalName = this.ctx.Target.LogicalName;
                    return result;
                }

                if (result is ITarget target)
                {
                    target.Attributes = this.ctx.Target.Attributes;
                }
                return result;
            }

            if (argument.IsPreImage)
            {
                if (result is Microsoft.Xrm.Sdk.Entity ent)
                {
                    ent.Attributes = this.ctx.PreImage.Attributes;
                    ent.LogicalName = this.ctx.PreImage.LogicalName;
                    return result;
                }

                if (result is IPreImage preimage)
                {
                    preimage.Attributes = this.ctx.PreImage.Attributes;
                }

                return result;
            }

            if (argument.IsMergedImage)
            {
                var notifier = result as System.ComponentModel.INotifyPropertyChanged;
                if (notifier != null && ctx.Stage <= 20 && ctx.Message == "Update")
                {
                    notifier.PropertyChanged += Merged_PropertyChanged;
                }

                if (result is Microsoft.Xrm.Sdk.Entity ent)
                {
                    ent.Attributes = this.ctx.Merged.Attributes;
                    return result;
                }

                if (result is IMerged merged)
                {
                    merged.Attributes = this.ctx.Merged.Attributes;
                }
                return result;
            }

            if (argument.IsPostImage)
            {
                if (result is Microsoft.Xrm.Sdk.Entity ent)
                {
                    ent.Attributes = this.ctx.PreImage.Attributes;
                    ent.LogicalName = this.ctx.PreImage.LogicalName;
                    return result;
                }

                if (result is IPreImage postimage)
                {
                    postimage.Attributes = this.ctx.PreImage.Attributes;
                }

                return result;
            }

            return result;
        }

        private void Merged_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.ctx.Target[e.PropertyName.ToLower()] = this.ctx.Merged[e.PropertyName.ToLower()];
        }

        private void Target_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.ctx.Merged[e.PropertyName.ToLower()] = this.ctx.Target[e.PropertyName.ToLower()];
        }

        #region private helpers
        private object DoResolve(Type fromType, bool isTargetReference, bool isOrgRequest ,bool admin, Type toType)
        {
            if (resolved.TryGetValue(fromType, out object o))
            {
                return o;
            }

            #region resolve standard services
            if (fromType == (typeof(Microsoft.Xrm.Sdk.ITracingService)))
            {
                return this.ctx.TracingService;
            }

            if (fromType == (typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext)))
            {
                return this.ctx.PluginExecutionContext;
            }

            if (fromType == typeof(IServiceEndpointNotificationService))
            {
                return this.ctx.NotificationService;
            }

            if (fromType == typeof(IServiceProvider))
            {
                return this.ctx.StandardServiceProvider;
            }

            if (fromType == typeof(IOrganizationServiceFactory))
            {
                return this.ctx.OrgSvcFactory;
            }
            #endregion

            #region resolve orgservice
            if (fromType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
            {
                if (admin)
                {
                    return this.ctx.PluginAdminService;
                }
                else
                {
                    return this.ctx.PluginUserService;
                }
            }
            #endregion

            #region resolve target reference
            if (isTargetReference)
            {
                if (fromType == typeof(Microsoft.Xrm.Sdk.EntityReference))
                {
                    return this.ctx.TargetReference;
                }

                var re = System.Activator.CreateInstance(toType, this.ctx.TargetReference);
                resolved[fromType] = re;
                return re;
            }
            #endregion

            #region organization request
            if (isOrgRequest)
            {
                var orgR = System.Activator.CreateInstance(fromType) as Microsoft.Xrm.Sdk.OrganizationRequest;
                orgR.RequestName = this.ctx.PluginExecutionContext.MessageName;
                orgR.Parameters = this.ctx.PluginExecutionContext.InputParameters;
                resolved[fromType] = orgR;
                return orgR;
            }
            #endregion

            #region resolve iqueryable
            if (fromType.IsQueryable())
            {
                var repo = this.ResolveRepository(fromType.GenericTypeArguments[0], admin);
                var queryMethd = repo.GetType().GetMethod("GetQuery", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                resolved[fromType] = queryMethd.Invoke(repo, null);
                return resolved[fromType];
            }
            #endregion

            #region resolve irepository
            if (fromType.IsRepository())
            {
                resolved[fromType] = this.ResolveRepository(fromType.GenericTypeArguments[0], admin);
                return resolved[fromType];
            }
            #endregion

            #region resolve boruto services
            if (fromType == typeof(Boruto.ServiceAPI.IMetadataService))
            {
                resolved[fromType] = new Implementations.Services.MetadataService(this.ctx.PluginAdminService);
                return resolved[fromType];
            }

            if (fromType == typeof(Boruto.ServiceAPI.INamingService))
            {
                var metaService = (Boruto.ServiceAPI.IMetadataService)DoResolve(typeof(Boruto.ServiceAPI.IMetadataService), false, false, false, null);
                resolved[fromType] = new Implementations.Services.NamingService(metaService, this.ctx.PluginAdminService);
                return resolved[fromType];
            }
            #endregion


            #region resolve types already mapped
            if (toType != null)
            {
                resolved[fromType] = this.CreateServiceInstance(toType);
                return resolved[fromType];
            }
            #endregion

            #region resolve from implementation
            if (fromType.IsInterface || fromType.IsAbstract)
            {
                var resolveToType = fromType.ResolveImplementingType(this.ctx.ServiceAssemblies);
                if (resolveToType != null)
                {
                    resolved[fromType] = this.CreateServiceInstance(resolveToType);
                    return resolved[fromType];
                }
            }
            #endregion

            #region resolve simply by it self
            if (!fromType.IsInterface && !fromType.IsAbstract && fromType.HasPublicConstructor())
            {
                resolved[fromType] = this.CreateServiceInstance(fromType);
                return resolved[fromType];
            }
            #endregion

            #region custom service provider
            if (this.ctx.CustomServiceProvider != null)
            {
                object result = this.ResolveCustomService(fromType);

                if (result != null)
                {
                    resolved[fromType] = result;
                    return result;
                }
            }
            #endregion

            #region resolve my service type search
            #endregion

            throw new Exceptions.UnresolveableEntityTypeException(fromType);
        }

        private static Dictionary<Type, bool> customService = new Dictionary<Type, bool>();
        private object ResolveCustomService(Type fromType)
        {
            if (customService.TryGetValue(fromType, out bool isCustom))
            {
                if (isCustom)
                {
                    return ctx.CustomServiceProvider.GetService(fromType);
                } else
                {
                    return null;
                }
            }

            var result = this.ctx.CustomServiceProvider.GetService(fromType);
            customService[fromType] = result != null;
            return result;
        }

        private Dictionary<string, object> repositoryTypes = new Dictionary<string, object>();
        private object ResolveRepository(Type entityType, bool admin)
        {
            var key = $"{entityType.FullName}:{admin}";
            if (repositoryTypes.TryGetValue(key, out object o))
            {
                return o;
            }
            Type resultType = typeof(Implementations.Repository<>).MakeGenericType(entityType);

            if (admin)
            {
                repositoryTypes[key] = Activator.CreateInstance(resultType, this.ctx.PluginAdminService, this.ctx.AdminServiceContext);
            }
            else
            {
                repositoryTypes[key] = Activator.CreateInstance(resultType, this.ctx.PluginUserService, this.ctx.UserServiceContext);
            }
            return repositoryTypes[key];
        }
        #endregion

        #region service constructor
        private List<Type> resolving = new List<Type>();
        private object CreateServiceInstance(Type type)
        {
            if (resolving.Contains(type))
            {
                throw new Exceptions.CyclicalDependencyException(type, resolving.ToArray());
            }

            resolving.Add(type);
            var con = Model.ServiceConstructor.ForType(type);

            var args = new object[con.Parameters.Length];

            for (var i = 0; i < args.Length; i++)
            {
                args[i] = this.ResolveServiceInstance(con.Parameters[0].Parameter.ParameterType, con.Parameters[0].Admin);
            }

            var result = con.Constructor.Invoke(args);
            resolving.Remove(type);
            return result;
        }

        private object ResolveServiceInstance(Type type, bool admin)
        {
            return this.DoResolve(type, false, false, admin, null);
        }
        #endregion

        #region disposeable
        public void Dispose()
        {
            foreach (var service in resolved.Values)
            {
                if (service is IDisposable di)
                {
                    di.Dispose();
                }
            }
        }
        #endregion
    }
}
