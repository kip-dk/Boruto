using Boruto.Extensions.Framework;
using Boruto.Extensions.SDK;
using Boruto.Implementations;
using Boruto.Reflection;
using Boruto.ServiceAPI;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;

namespace Boruto
{
    internal class PluginContext: IDisposable, ServiceAPI.IServiceContext 
    {
        private static Dictionary<System.Threading.Thread, List<PluginContext>> runnings = new Dictionary<System.Threading.Thread, List<PluginContext>>();
        private static Dictionary<Type, Reflection.PluginServiceResolver> serviceResolverIndex = new Dictionary<Type, Reflection.PluginServiceResolver>();

        private static readonly object locker = new object();

        internal static PluginContext Current
        {
            get
            {
                lock (locker)
                {
                    if (runnings.TryGetValue(System.Threading.Thread.CurrentThread, out List<PluginContext> ctxs))
                    {
                        return ctxs.LastOrDefault();
                    }
                }
                return null;
            }
        }

        private BasePlugin plugin;
        private string methodPattern;
        private List<string> onErrorLogs = new List<string>();

        internal PluginContext(BasePlugin plugin, IServiceProvider standardServiceProvider, Assembly[] assemblies, string unsecure, string secure, bool trace)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                throw new InvalidPluginExecutionException($"At least one assembly for service resolve should be provided");
            }

            this.plugin = plugin;
            this.Type = plugin.GetType();
            this.StandardServiceProvider = standardServiceProvider;
            this.ServiceAssemblies = assemblies;
            this.Unsecure = unsecure;
            this.Secure = secure;

            this.Message = this.PluginExecutionContext.MessageName;
            this.Stage = this.PluginExecutionContext.Stage;
            this.IsAsync = this.PluginExecutionContext.Mode > 0;
            this.PrimaryLogicalName = this.PluginExecutionContext.PrimaryEntityName;
            this.PrimaryEntityId = this.PluginExecutionContext.PrimaryEntityId;
            this._trace = trace;

            this.methodPattern = this.Stage.ToMethodName(this.Message, this.PluginExecutionContext.Mode);

            lock (locker)
            {
                if (!runnings.ContainsKey(System.Threading.Thread.CurrentThread))
                {
                    runnings[System.Threading.Thread.CurrentThread] = new List<PluginContext>();
                }

                runnings[System.Threading.Thread.CurrentThread].Add(this);
            }
        }

        internal void SetCustomServiceProvider(IServiceProvider customServiceProvider)
        {
            this.CustomServiceProvider = customServiceProvider;
        } 

        internal void Log(string message)
        {
            this.onErrorLogs.Add(message);
        }

        internal void FlushError()
        {
            if (this.onErrorLogs != null && this.onErrorLogs.Count > 0)
            {
                foreach (var l in onErrorLogs)
                {
                    this.TracingService.Trace(l);
                }
            }
        }

        #region constructor properties
        internal Type Type { get; }
        internal string Unsecure { get; }
        internal string Secure { get; }
        #endregion

        #region message properties
        internal string Message { get; }
        internal int Stage { get; }
        internal bool IsAsync { get; }
        internal string PrimaryLogicalName { get; }
        internal Guid PrimaryEntityId { get; }

        internal bool _isAdmin;
        internal bool _trace;

        #endregion

        #region entity properties
        internal string TargetLogicalName
        {
            get
            {
                {
                    var t = this.Target;
                    if (t != null)
                    {
                        return t.LogicalName;
                    }

                    {
                        var r = this.TargetReference;
                        if (r != null)
                        {
                            return r.LogicalName;
                        }
                    }
                    return this.PluginExecutionContext.PrimaryEntityName;
                }
            }
        }

        internal Guid TargetId
        {
            get
            {
                {
                    var t = this.Target;
                    if (t != null)
                    {
                        return t.Id;
                    }

                    {
                        var r = this.TargetReference;
                        if (r != null)
                        {
                            return r.Id;
                        }
                    }
                    return this.PluginExecutionContext.PrimaryEntityId;
                }
            }
        }

        private Microsoft.Xrm.Sdk.Entity _target;
        internal Microsoft.Xrm.Sdk.Entity Target
        {
            get
            {
                if (this._target == null && this.PluginExecutionContext.InputParameters.TryGetValue("Target", out Microsoft.Xrm.Sdk.Entity e))
                {
                    this._target = e;
                }
                return this._target;
            }
        }

        private Microsoft.Xrm.Sdk.Entity _preimage;
        internal Microsoft.Xrm.Sdk.Entity PreImage
        {
            get
            {
                return this.GetPreImage(true);
            }
        }

        private Microsoft.Xrm.Sdk.Entity GetPreImage(bool errorIfNotFound)
        {
            if (this._preimage == null)
            {
                this._preimage = new Entity();
                this._preimage.LogicalName = this.PrimaryLogicalName;
                this._preimage.Id = this.PrimaryEntityId;
                this.ResolveInfoFromDeleteMessage(_preimage);

                var key = Boruto.Deployment.Services.SdkMessageProcessingStepService.ImageName(1);

                if (this.PluginExecutionContext.PreEntityImages != null && this.PluginExecutionContext.PreEntityImages.ContainsKey(key))
                {
                    var pe = this.PluginExecutionContext.PreEntityImages[key];
                    foreach (var att in pe.Attributes)
                    {
                        this._preimage[att.Key] = att.Value;
                    }
                }
                else
                {
                    if (errorIfNotFound)
                    {
                        // Boruto.Trace.Error($"[{this.methodPattern}]:[{key}] was expected, but not found in pre entity images");
                    } else
                    {
                        this._preimage = null;
                        return null;
                    }
                }
            }
            return this._preimage;

        }

        private Microsoft.Xrm.Sdk.Entity _postimage;
        internal Microsoft.Xrm.Sdk.Entity PostImage
        {
            get
            {
                return this.GetPostImage(true);
            }
        }

        private Microsoft.Xrm.Sdk.Entity GetPostImage(bool throwIfNotFound)
        {
            if (this._postimage == null)
            {
                this._postimage = new Entity();
                this._postimage.LogicalName = this.PrimaryLogicalName;
                this._postimage.Id = this.PrimaryEntityId;
                this.ResolveInfoFromDeleteMessage(_postimage);

                var key = Boruto.Deployment.Services.SdkMessageProcessingStepService.ImageName(2);

                if (this.PluginExecutionContext.PostEntityImages != null && this.PluginExecutionContext.PostEntityImages.ContainsKey(key))
                {
                    var pe = this.PluginExecutionContext.PostEntityImages[key];
                    foreach (var att in pe.Attributes)
                    {
                        this._postimage[att.Key] = att.Value;
                    }
                }
                else
                {
                    if (throwIfNotFound)
                    {
                        Boruto.Trace.Error($"{key} was expected, but not found as post image");
                    } else
                    {
                        this._postimage = null;
                        return null;
                    }
                }
            }
            return this._postimage;
        }

        private Microsoft.Xrm.Sdk.Entity _merged;
        internal Microsoft.Xrm.Sdk.Entity Merged
        {
            get
            {
                return this.GetMerged(true);
            }
        }

        private Microsoft.Xrm.Sdk.Entity GetMerged(bool errorOnNoImage)
        {
            if (this.Message == "Delete")
            {
                return this.GetPreImage(errorOnNoImage);
            }

            if (this.Message != "Update")
            {
                return this.Target;
            }

            if (_merged == null)
            {
                var pre = this.GetPreImage(errorOnNoImage);

                if (pre == null && !errorOnNoImage)
                {
                    return null;
                }

                this._merged = new Entity();
                this._merged.LogicalName = this.PrimaryLogicalName;
                this._merged.Id = this.PrimaryEntityId;

                if (pre != null)
                {
                    foreach (var att in pre.Attributes)
                    {
                        this._merged[att.Key] = att.Value;
                        this._merged[$"preimage_{att.Key}"] = att.Value;
                    }

                    var tar = this.Target;

                    foreach (var att in tar.Attributes)
                    {
                        this._merged[att.Key] = att.Value;
                    }
                }
            }
            return _merged;
        }

        private Microsoft.Xrm.Sdk.EntityReference _targetReference;
        internal Microsoft.Xrm.Sdk.EntityReference TargetReference
        {
            get
            {
                if (this._targetReference == null)
                {
                    this._targetReference = this.PluginExecutionContext.InputParameters["Target"] as Microsoft.Xrm.Sdk.EntityReference;
                }
                return this._targetReference;
            }
        }

        private Microsoft.Xrm.Sdk.OrganizationRequest _orgRequest;

        internal Microsoft.Xrm.Sdk.OrganizationRequest OrganizationRequest
        {
            get
            {
                if (this._orgRequest == null)
                {
                    var req = new Microsoft.Xrm.Sdk.OrganizationRequest();
                    req.RequestName = this.Message;
                    req.Parameters = this.PluginExecutionContext.InputParameters;
                    this._orgRequest = req;
                }
                return this._orgRequest;
            }
        }

        private void ResolveInfoFromDeleteMessage(Microsoft.Xrm.Sdk.Entity entity)
        {
            if (this.PluginExecutionContext.MessageName == "Delete")
            {
                var target = this.TargetReference;
                entity.Id = target.Id;
                entity.LogicalName = target.LogicalName;
            }
        }

        #endregion

        #region microsoft service properties
        private IOrganizationService _InitiatingUserService;
        internal IOrganizationService InitiatingUserService
        {
            get
            {
                if (this._InitiatingUserService == null)
                {
                    this._InitiatingUserService = this.OrgSvcFactory.CreateOrganizationService(PluginExecutionContext.InitiatingUserId);
                }
                return this._InitiatingUserService;
            }
        }

        private IOrganizationService _PluginUserService;
        internal IOrganizationService PluginUserService
        {
            get
            {
                if (this._PluginUserService == null)
                {
                    var orgFac = this.OrgSvcFactory;
                    this._PluginUserService = orgFac.CreateOrganizationService(PluginExecutionContext.UserId); // User that the plugin is registered to run as, Could be same as current user.

                }
                return this._PluginUserService;
            }
        }

        private IOrganizationService _PluginAdminService;
        internal IOrganizationService PluginAdminService
        {
            get
            {
                if (this._PluginAdminService == null)
                {
                    this._PluginAdminService = this.OrgSvcFactory.CreateOrganizationService(null);
                }
                return this._PluginAdminService;
            }
        }

        private IPluginExecutionContext _PluginExecutionContext;
        internal IPluginExecutionContext PluginExecutionContext
        {
            get
            {
                if (this._PluginExecutionContext == null)
                {
                    this._PluginExecutionContext = StandardServiceProvider.Get<IPluginExecutionContext>();
                }
                return this._PluginExecutionContext;
            }
        }

        private IServiceEndpointNotificationService _NotificationService;
        internal IServiceEndpointNotificationService NotificationService
        {
            get
            {
                if (this._NotificationService == null)
                {
                    this._NotificationService = this.StandardServiceProvider.Get<IServiceEndpointNotificationService>();

                }
                return this._NotificationService;
            }
        }

        private ITracingService _TracingService;
        internal ITracingService TracingService
        {
            get
            {
                if (this._TracingService == null)
                {
                    this._TracingService = this.StandardServiceProvider.Get<ITracingService>();

                }
                return this._TracingService;
            }
        }

        internal IServiceProvider StandardServiceProvider { get; }
        internal IServiceProvider CustomServiceProvider { get; private set; }

        internal Assembly[] ServiceAssemblies { get; }

        private IOrganizationServiceFactory _OrgSvcFactory;
        internal IOrganizationServiceFactory OrgSvcFactory
        {
            get
            {
                if (this._OrgSvcFactory == null)
                {
                    this._OrgSvcFactory = this.StandardServiceProvider.Get<IOrganizationServiceFactory>();
                    var proxyProvider = _OrgSvcFactory as IProxyTypesAssemblyProvider;

                    if (proxyProvider != null)
                    {
                        var type = this.GetOrganizationServiceContextType();
                        proxyProvider.ProxyTypesAssembly = type.Assembly;
                    }
                }
                return this._OrgSvcFactory;
            }
        }

        private Microsoft.Xrm.Sdk.Client.OrganizationServiceContext _AdminServiceContext;
        internal Microsoft.Xrm.Sdk.Client.OrganizationServiceContext AdminServiceContext
        {
            get
            {
                if (this._AdminServiceContext == null)
                {
                    this._AdminServiceContext = this.GetOrganizationServiceContext(this.PluginAdminService);
                }
                return this._AdminServiceContext;
            }
        }

        private Microsoft.Xrm.Sdk.Client.OrganizationServiceContext _UserServiceContext;
        internal Microsoft.Xrm.Sdk.Client.OrganizationServiceContext UserServiceContext
        {
            get
            {
                if (this._UserServiceContext == null)
                {
                    this._UserServiceContext = this.GetOrganizationServiceContext(this.PluginUserService);
                }
                return this._UserServiceContext;
            }
        }
        #endregion

        #region context settings
        internal TraceLevel TraceLevel { get; set; } = TraceLevel.Error;

        private Reflection.ServiceFactory _serviceFactory;
        #endregion

        #region run plugin
        internal void Execute(bool filterTargetOnCreate, bool trace)
        {
            using (var fac = new Reflection.ServiceFactory(this))
            {
                this._serviceFactory = fac; 
                var resolver = this.GetPluginServiceResolver();

                var methods = resolver.GetMethods(this.methodPattern, this.PrimaryLogicalName, this.Message).ToList();

                this.Trace($"Number of methods: {methods.Count}");

                if (methods != null && methods.Count > 0) {
                    foreach (var method in methods)
                    {
                        if (((this.Message == "Create" && filterTargetOnCreate) || this.Message == "Update") && !method.AllTargetFilter)
                        {
                            if (!this.Target.Attributes.Keys.Where(r => method.TargetFilter.Contains(r)).Any())
                            {
                                this.Trace($"1: { method.method.Name } ignored");
                                this.Trace($"Target attributes: { string.Join(",", this.Target.Attributes.Keys) }");
                                this.Trace($"Filter attributes: { string.Join(",", method.TargetFilter) }");
                                continue;
                            }
                        }

                        if (!method.IsRelevant(this.PluginExecutionContext))
                        {
                            this.Trace($"2: {method.method.Name} ignored. !relevant");
                            continue;
                        }

                        #region resolve arguments
                        var args = new object[method.Arguments.Length];

                        var ix = 0;
                        foreach (var arg in method.Arguments)
                        {
                            this._isAdmin = false;
                            if (arg.Admin)
                            {
                                this._isAdmin = true;
                            }
                            args[ix] = fac.Resolve(arg);
                            ix++;
                        }
                        #endregion

                        #region invoke
                        object result = null;

                        try
                        {
                            if (this.CustomServiceProvider is IPipeline pePre)
                            {
                                pePre.PreInvokeMethod();
                            }

                            this.Trace($"Before: { method.method.Name }");
                            result = method.method.Invoke(this.plugin, args);
                            this.Trace($"After : {method.method.Name}");

                            this._AdminServiceContext.Clear();
                            this._UserServiceContext.Clear();

                            if (this.CustomServiceProvider is IPipeline pePost)
                            {
                                pePost.PostInvokeMethod();
                            }

                        } catch (System.Reflection.TargetInvocationException te)
                        {
                            var inner = te.InnerException;
                            while (inner != null)
                            {
                                if (inner is Microsoft.Xrm.Sdk.InvalidPluginExecutionException ip)
                                {
                                    throw ip;
                                }
                                inner = inner.InnerException;
                            }
                            throw te;
                        }

                        fac.UnregistrePM();

                        if (result != null)
                        {
                            if ((this.Stage == 30 || this.Stage == 40) && this.IsAsync == false && result is Microsoft.Xrm.Sdk.OrganizationResponse re && re.Results != null)
                            {
                                foreach (var p in re.Results)
                                {
                                    var value = p.Value;
                                    var done = false;
                                    if (value != null && value is Microsoft.Xrm.Sdk.EntityCollection ec && ec.Entities.Count > 0)
                                    {
                                        this.PluginExecutionContext.OutputParameters[p.Key] = new Microsoft.Xrm.Sdk.EntityCollection(ec.Entities.Select(r => r.ToEntityBase()).ToList());
                                        done = true;
                                    }

                                    if (value != null && value is Microsoft.Xrm.Sdk.Entity et)
                                    {
                                        this.PluginExecutionContext.OutputParameters[p.Key] = et.ToEntityBase();
                                        done = true;
                                    }

                                    if (!done)
                                    {
                                        this.PluginExecutionContext.OutputParameters[p.Key] = value;
                                        done = true;
                                    }
                                }
                            }

                            if (this.Stage == 30 && this.Message == "Create" && result is Guid g)
                            {
                                this.PluginExecutionContext.OutputParameters["id"] = g;
                            }

                            if (this.Stage == 30 && this.Message == "Retrieve" && result is Microsoft.Xrm.Sdk.Entity ent)
                            {
                                this.PluginExecutionContext.OutputParameters["BusinessEntity"] = ent.ToEntityBase();
                            }

                            if (this.Stage == 30 && this.Message == "RetrieveMultiple" && result is Microsoft.Xrm.Sdk.EntityCollection col)
                            {
                                var pub = new Microsoft.Xrm.Sdk.EntityCollection
                                {
                                    EntityName = col.EntityName,
                                    MinActiveRowVersion = col.MinActiveRowVersion,
                                    MoreRecords = col.MoreRecords,
                                    PagingCookie = col.PagingCookie,
                                    TotalRecordCount = col.TotalRecordCount,
                                    TotalRecordCountLimitExceeded = col.TotalRecordCountLimitExceeded
                                };

                                foreach (var e in col.Entities)
                                {
                                    pub.Entities.Add(e.ToEntityBase());
                                }

                                this.PluginExecutionContext.OutputParameters["BusinessEntityCollection"] = pub;
                            }

                            if (this.Stage == 30 && this.Message == "RetrieveMultiple" && result is Boruto.EntityCollection bCol)
                            {
                                this.PluginExecutionContext.OutputParameters["BusinessEntityCollection"] = bCol.ToEntityCollection();
                            }
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region public methods
        internal void Trace(string message, [CallerMemberName] string method = null)
        {
            this.TracingService.Trace(message);
        }
        #endregion

        #region private helpers
        private PluginServiceResolver GetPluginServiceResolver()
        {
            lock (locker)
            {
                if (serviceResolverIndex.TryGetValue(this.Type, out PluginServiceResolver pe))
                {
                    return pe;
                }

                serviceResolverIndex[this.Type] = new PluginServiceResolver(this.Type, this.ServiceAssemblies);
                return serviceResolverIndex[this.Type];
            }
        }

        private static readonly Type ORG_TYPE = typeof(Microsoft.Xrm.Sdk.IOrganizationService);
        private static readonly Type OSC_TYPE = typeof(Microsoft.Xrm.Sdk.Client.OrganizationServiceContext);
        private Microsoft.Xrm.Sdk.Client.OrganizationServiceContext GetOrganizationServiceContext(Microsoft.Xrm.Sdk.IOrganizationService orgService)
        {
            var orServiceType = this.GetOrganizationServiceContextType();
            return (Microsoft.Xrm.Sdk.Client.OrganizationServiceContext)System.Activator.CreateInstance(orServiceType, orgService);
        }

        private static Type _orgServiceContextType;
        private Type GetOrganizationServiceContextType()
        {
            if (_orgServiceContextType == null)
            {
                var count = this.ServiceAssemblies != null ? this.ServiceAssemblies.Length : 0;
                foreach (var asm in this.ServiceAssemblies)
                {
                    var local = (from type in asm.GetTypes()
                                 where type.BaseType == OSC_TYPE
                                 select type).FirstOrDefault();

                    if (local != null)
                    {
                        foreach (var con in local.GetConstructors())
                        {
                            if (con.IsPublic)
                            {
                                var pms = con.GetParameters();
                                if (pms != null && pms.Length == 1 && pms[0].ParameterType == ORG_TYPE)
                                {
                                    _orgServiceContextType = local;
                                }
                            }
                        }
                    }
                }
            }

            if (_orgServiceContextType == null)
            {
                throw new InvalidPluginExecutionException($"Unable to find an extending implementation of OrganizationServiceContext");
            }
            return _orgServiceContextType;
        }
        #endregion

        #region dispose
        public void Dispose()
        {
            if (this._AdminServiceContext != null)
            {
                this._AdminServiceContext.Dispose();
            }

            if (this._UserServiceContext != null)
            {
                this._UserServiceContext.Dispose();
            }

            lock (locker)
            {
                if (runnings.TryGetValue(System.Threading.Thread.CurrentThread, out List<PluginContext> list))
                {
                    if (list.Count > 0)
                    {
                        list.Remove(list.Last());
                    }

                    if (list.Count == 0)
                    {
                        runnings.Remove(System.Threading.Thread.CurrentThread);
                    }
                }
            }
        }
        #endregion

        #region iservicecontext
        IOrganizationService IServiceContext.UserOrganizationService => this.PluginUserService;
        IOrganizationService IServiceContext.InitiatingUserOrganizationService => this.InitiatingUserService;
        IOrganizationService IServiceContext.AdminOrganizationService => this.PluginAdminService;
        IOrganizationServiceFactory IServiceContext.OrganizationServiceFactory => this.OrgSvcFactory;
        ITracingService IServiceContext.TraceService => this.TracingService;
        IPluginExecutionContext IServiceContext.PluginExecutionContext => this.PluginExecutionContext;
        System.IServiceProvider IServiceContext.SdkServiceProvider => this.StandardServiceProvider;

        bool IServiceContext.IsAdmin => this._isAdmin;

        string IServiceContext.Message => this.Message;

        T IServiceContext.Target<T>()
        {
            var t = this.Target;
            if (t != null)
            {
                return t.ToEntity<T>();
            }
            return default(T);
        }

        T IServiceContext.Preimage<T>()
        {
            var p = this.GetPreImage(false);

            if (p != null)
            {
                return p.ToEntity<T>();
            }
            return default(T);
        }

        T IServiceContext.PostImage<T>()
        {
            var p = this.GetPostImage(false);
            if (p != null)
            {
                return p.ToEntity<T>();
            }
            return default(T);
        }

        T IServiceContext.MergedImage<T>()
        {
            var p = this.GetMerged(false);
            if (p != null)
            {
                return p.ToEntity<T>();
            }
            return default(T);
        }

        T IServiceContext.OrganizationRequest<T>()
        {
            var orgR = this.OrganizationRequest;
            var result = new T();
            result.RequestName = orgR.RequestName;
            result.Parameters = orgR.Parameters;
            return result;
        }
        #endregion

        #region private helpers
        private void Trace(string message)
        {
            if (this._trace)
            {
                Boruto.Trace.Info(message);
            }
        }
        #endregion
    }
}
