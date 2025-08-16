using Boruto.Extensions.Reflection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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

        private Dictionary<PropertyMirror, System.ComponentModel.INotifyPropertyChanged> PM = new Dictionary<PropertyMirror, System.ComponentModel.INotifyPropertyChanged>();
        internal void UnregistrePM()
        {
            foreach (var key in PM.Keys)
            {
                var no = PM[key];
                no.PropertyChanged -= key.MirrorpropertyChanged;
            }

            PM.Clear();
        }

        internal object Resolve(Reflection.Model.PluginMethodArgument argument)
        {
            if (argument.Name.ToLower() == "primaryentityid" && argument.FromType == typeof(Guid))
            {
                return this.ctx.PrimaryEntityId;
            }

            if (argument.Name.ToLower() == "primaryentityname" && argument.FromType == typeof(string))
            {
                return this.ctx.PrimaryLogicalName;
            }

            var result = this.DoResolve(argument.FromType, argument.IsTargetReference, argument.IsOrganizationRequest, argument.Admin, argument.EarlyBoundEntityType ?? argument.ToType);

            if (argument.EarlyBoundEntityType == null && argument.ToType == null)
            {
                argument.ToType = result.GetType();
            }

            if (argument.IsTarget)
            {
                var attrWasSet = false;
                if (result is Microsoft.Xrm.Sdk.Entity ent)
                {
                    ent.Attributes = this.ctx.Target.Attributes;
                    ent.LogicalName = this.ctx.TargetLogicalName;
                    ent.Id = this.ctx.TargetId;
                    attrWasSet = true;
                }

                if (!attrWasSet && result is ITarget target)
                {
                    target.Attributes = this.ctx.Target.Attributes;
                    attrWasSet = true;
                }

                var notifier = result as System.ComponentModel.INotifyPropertyChanged;
                if (notifier != null && ctx.Stage <= 20 && ctx.Message == "Update")
                {
                    var targetMirror = new PropertyMirror((Microsoft.Xrm.Sdk.Entity)this.ctx.Merged);
                    notifier.PropertyChanged += targetMirror.MirrorpropertyChanged;
                    this.PM.Add(targetMirror, notifier);
                }
                return result;
            }

            if (argument.IsPreImage)
            {
                if (result is Microsoft.Xrm.Sdk.Entity ent)
                {
                    ent.Attributes = this.ctx.PreImage.Attributes;
                    ent.LogicalName = this.ctx.TargetLogicalName;
                    ent.Id = this.ctx.TargetId;
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
                var attrWasSet = false;
                if (result is Microsoft.Xrm.Sdk.Entity ent)
                {
                    ent.Attributes = this.ctx.Merged.Attributes;
                    ent.LogicalName = this.ctx.TargetLogicalName;
                    ent.Id = this.ctx.TargetId;
                    attrWasSet = true;
                }

                if (!attrWasSet && result is IMerged merged)
                {
                    merged.Attributes = this.ctx.Merged.Attributes;
                    attrWasSet = true;
                }

                var notifier = result as System.ComponentModel.INotifyPropertyChanged;
                if (notifier != null && ctx.Stage <= 20 && ctx.Message == "Update")
                {
                    var tg = (Microsoft.Xrm.Sdk.Entity)this.ctx.Target;
                    var mergedimageMirror = new PropertyMirror(tg);
                    notifier.PropertyChanged += mergedimageMirror.MirrorpropertyChanged;
                    PM.Add(mergedimageMirror, notifier);
                }
                return result;
            }

            if (argument.IsPostImage)
            {
                if (result is Microsoft.Xrm.Sdk.Entity ent)
                {
                    ent.Attributes = this.ctx.PostImage.Attributes;
                    ent.LogicalName = this.ctx.TargetLogicalName;
                    ent.Id = this.ctx.TargetId;
                    return result;
                }

                if (result is IPreImage postimage)
                {
                    postimage.Attributes = this.ctx.PostImage.Attributes;
                }

                return result;
            }

            return result;
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

            #region queryable
            if (fromType == typeof(Microsoft.Xrm.Sdk.Query.QueryExpression))
            {
                return this.QueryExpression;
            }

            if (fromType == typeof(Microsoft.Xrm.Sdk.Query.FetchExpression))
            {
                return this.FetchExpression;
            }

            if (fromType == typeof(Microsoft.Xrm.Sdk.Query.ColumnSet))
            {
                return this.ColumnSet;
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
                // do not cache iqueryable,  each injection should have its own instance
                return queryMethd.Invoke(repo, null);
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

            #region custom service provider
            if (this.ctx.CustomServiceProvider != null)
            {
                object result = this.ResolveCustomService(fromType, admin);

                if (result != null)
                {
                    resolved[fromType] = result;
                    return result;
                }
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

            throw new Exceptions.UnresolveableTypeException(fromType);
        }

        private static Dictionary<Type, bool> customService = new Dictionary<Type, bool>();
        private object ResolveCustomService(Type fromType, bool admin)
        {
            var toBeReset = ctx._isAdmin;

            try
            {
                ctx._isAdmin = admin;

                if (customService.TryGetValue(fromType, out bool isCustom))
                {
                    if (isCustom)
                    {
                        return ctx.CustomServiceProvider.GetService(fromType);
                    }
                    else
                    {
                        return null;
                    }
                }

                var result = this.ctx.CustomServiceProvider.GetService(fromType);
                customService[fromType] = result != null;
                return result;
            } finally
            {
                ctx._isAdmin = toBeReset;
            }
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
                var orgService = this.ctx.PluginAdminService;
                var orgContext = this.ctx.AdminServiceContext;
                repositoryTypes[key] = Activator.CreateInstance(resultType, orgService, orgContext);
            }
            else
            {
                var orgService = this.ctx.PluginUserService;
                var orgContext = this.ctx.UserServiceContext;
                repositoryTypes[key] = Activator.CreateInstance(resultType, orgService, orgContext);
            }
            return repositoryTypes[key];
        }
        #endregion

        #region queries
        private Microsoft.Xrm.Sdk.Query.QueryExpression _queryExpression;
        private Microsoft.Xrm.Sdk.Query.QueryExpression QueryExpression
        {
            get
            {
                if (this._queryExpression != null)
                {
                    return this._queryExpression;
                }

                if (this.ctx.PluginExecutionContext.InputParameters.TryGetValue("Query", out object o))
                {
                    if (o is Microsoft.Xrm.Sdk.Query.QueryExpression qe)
                    {
                        this._queryExpression = qe;
                        return this._queryExpression;
                    }

                    if (o is Microsoft.Xrm.Sdk.Query.FetchExpression fe)
                    {
                        var resp = (Microsoft.Crm.Sdk.Messages.FetchXmlToQueryExpressionResponse)this.ctx.PluginAdminService.Execute(new Microsoft.Crm.Sdk.Messages.FetchXmlToQueryExpressionRequest
                        {
                            FetchXml = fe.Query
                        });
                        this._queryExpression = resp.Query;
                        return this._queryExpression;
                    }
                }
                return null;
            }
        }

        private Microsoft.Xrm.Sdk.Query.FetchExpression _fetchExpression;
        private  Microsoft.Xrm.Sdk.Query.FetchExpression FetchExpression
        {
            get
            {
                if (_fetchExpression != null)
                {
                    return _fetchExpression;
                }

                if (this.ctx.PluginExecutionContext.InputParameters.TryGetValue("Query", out object o))
                {
                    if (o is Microsoft.Xrm.Sdk.Query.FetchExpression qe)
                    {
                        this._fetchExpression = qe;
                        return this._fetchExpression;
                    }

                    if (o is Microsoft.Xrm.Sdk.Query.QueryExpression fe)
                    {
                        var resp = (Microsoft.Crm.Sdk.Messages.QueryExpressionToFetchXmlResponse)this.ctx.PluginAdminService.Execute(new Microsoft.Crm.Sdk.Messages.QueryExpressionToFetchXmlRequest
                        {
                            Query = fe
                            
                        });
                        this._fetchExpression = new Microsoft.Xrm.Sdk.Query.FetchExpression(resp.FetchXml);
                        return this._fetchExpression;
                    }
                }
                return null;
            }
        }

        private Microsoft.Xrm.Sdk.Query.ColumnSet _columnSet;

        private Microsoft.Xrm.Sdk.Query.ColumnSet ColumnSet
        {
            get
            {
                if (this._columnSet != null)
                {
                    return this._columnSet;
                }

                if (this.ctx.PluginExecutionContext.InputParameters.ContainsKey("ColumnSet")) 
                { 
                    this._columnSet = this.ctx.PluginExecutionContext.InputParameters["ColumnSet"] as Microsoft.Xrm.Sdk.Query.ColumnSet;
                    return this._columnSet;
                }

                var qe = this.QueryExpression;
                if (qe != null)
                {
                    return qe.ColumnSet;
                }

                return null;
            }
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
                args[i] = this.ResolveServiceInstance(con.Parameters[i].Parameter.ParameterType, con.Parameters[i].Admin);
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

        #region image helper classes
        private class PropertyMirror
        {
            private Microsoft.Xrm.Sdk.Entity mirrorTo;

            internal PropertyMirror(Microsoft.Xrm.Sdk.Entity mirrorTo)
            {
                this.mirrorTo = mirrorTo;
            }

            internal void MirrorpropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                var prop = sender.GetType().GetProperty(e.PropertyName);
                if (prop != null)
                {
                    var attr = (Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute)prop.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute), false).FirstOrDefault();
                    if (attr != null)
                    {
                        var source = sender as Microsoft.Xrm.Sdk.Entity;
                        if (source != null)
                        {
                            mirrorTo[attr.LogicalName] = source[attr.LogicalName];
                        }
                    }
                }
            }
        }
        #endregion

    }
}
