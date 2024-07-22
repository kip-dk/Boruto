using Boruto.Extensions.SDK;
using Boruto.Plugin;
using Boruto.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Boruto
{
    public class PluginContext : IDisposable
    {
        private static Dictionary<int, PluginContext> runnings = new Dictionary<int, PluginContext>();

        private static Dictionary<Type, Reflection.PluginServiceResolver> serviceResolverIndex = new Dictionary<Type, Reflection.PluginServiceResolver>();

        public static PluginContext Current
        {
            get
            {
                return runnings[System.Threading.Thread.CurrentThread.ManagedThreadId];
            }
        }

        private BasePlugin plugin;
        private string methodPattern;
        private List<string> logs = new List<string>();

        internal PluginContext(BasePlugin plugin, IServiceProvider serviceProvider, string unsecure, string secure)
        {
            this.plugin = plugin;
            this.Type = plugin.GetType();
            this.ServiceProvider = serviceProvider;
            this.Unsecure = unsecure;
            this.Secure = secure;

            this.Message = this.PluginExecutionContext.MessageName;
            this.Stage = this.PluginExecutionContext.Stage;
            this.IsAsync = this.PluginExecutionContext.Mode > 0;
            this.PrimaryLogicalName = this.PluginExecutionContext.PrimaryEntityName;
            this.PrimaryEntityId = this.PluginExecutionContext.PrimaryEntityId;

            var methodName = "On";

            switch (this.Stage)
            {
                case 10: methodName += "Validate"; break;
                case 20: methodName += "Pre"; break;
                case 30: break; // virtual plugin
                case 40: methodName += "Post"; break;
            }

            switch (this.Message)
            {
                case "Create":
                case "Update":
                case "Delete":
                    {
                        methodName += this.Message;
                        break;
                    }
            }

            if (this.IsAsync)
            {
                methodName += "Async";
            }

            this.methodPattern = methodName;

            runnings[System.Threading.Thread.CurrentThread.ManagedThreadId] = this;
        }

        #region constructor properties
        public Type Type { get; }
        public string Unsecure { get; }
        public string Secure { get; }
        #endregion

        #region message properties
        public string Message { get; }
        public int Stage { get; }
        public bool IsAsync { get; }
        public string PrimaryLogicalName { get; }
        public Guid PrimaryEntityId { get; }

        #endregion

        #region entity properties
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
                if (this._preimage == null)
                {
                    this._preimage = new Entity();
                    if (this.PluginExecutionContext.PreEntityImages != null)
                    {
                        foreach (var pe in this.PluginExecutionContext.PreEntityImages.Values)
                        {
                            foreach (var att in pe.Attributes)
                            {
                                this._preimage[att.Key] = att.Value;
                            }
                        }
                    }
                }
                return this._target;
            }
        }

        private Microsoft.Xrm.Sdk.Entity _postimage;
        internal Microsoft.Xrm.Sdk.Entity PostImage
        {
            get
            {
                if (this._postimage == null)
                {
                    this._postimage = new Entity();
                    if (this.PluginExecutionContext.PostEntityImages != null)
                    {
                        foreach (var pe in this.PluginExecutionContext.PostEntityImages.Values)
                        {
                            foreach (var att in pe.Attributes)
                            {
                                this._postimage[att.Key] = att.Value;
                            }
                        }
                    }
                }
                return this._postimage;
            }
        }

        private Microsoft.Xrm.Sdk.Entity _merged;
        private Microsoft.Xrm.Sdk.Entity Merged
        {
            get
            {
                if (_merged == null)
                {
                    _merged = new Entity();
                    var pre = this.PreImage;
                    foreach (var att in pre.Attributes)
                    {
                        _merged[att.Key] = att.Value;
                    }

                    var tar = this.Target;

                    foreach (var att in tar.Attributes)
                    {
                        _merged[att.Key] = att.Value;
                    }
                }
                return _merged;
            }
        }

        private Microsoft.Xrm.Sdk.EntityReference _targetReference;
        private Microsoft.Xrm.Sdk.EntityReference TargetReference
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
        #endregion

        #region service properties
        private IOrganizationService _InitiatingUserService;
        public IOrganizationService InitiatingUserService 
        {
            get
            {
                if (this._InitiatingUserService == null)
                {
                    this._InitiatingUserService = this.ServiceProvider.GetOrganizationService(PluginExecutionContext.InitiatingUserId);
                }
                return this._InitiatingUserService;
            } 
        }

        private IOrganizationService _PluginUserService;
        public IOrganizationService PluginUserService 
        {
            get
            {
                if (this._PluginUserService == null)
                {
                    this._PluginUserService = this.ServiceProvider.GetOrganizationService(PluginExecutionContext.UserId); // User that the plugin is registered to run as, Could be same as current user.

                }
                return this._PluginUserService;
            }
        }

        private IOrganizationService _PluginAdminService;
        public IOrganizationService PluginAdminService 
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
        public IPluginExecutionContext PluginExecutionContext 
        {
            get
            {
                if (this._PluginExecutionContext == null)
                {
                    this._PluginExecutionContext = ServiceProvider.Get<IPluginExecutionContext>();
                }
                return this._PluginExecutionContext;
            }
        }

        private IServiceEndpointNotificationService _NotificationService;
        public IServiceEndpointNotificationService NotificationService 
        {
            get
            {
                if (this._NotificationService == null)
                {
                    this._NotificationService = this.ServiceProvider.Get<IServiceEndpointNotificationService>();

                }
                return this._NotificationService;
            }
        }

        private ITracingService _TracingService;
        public ITracingService TracingService 
        {
            get
            {
                if (this._TracingService == null)
                {
                    this._TracingService = this.ServiceProvider.Get<ITracingService>();

                }
                return this._TracingService;
            } 
        }
        public IServiceProvider ServiceProvider { get; }


        private IOrganizationServiceFactory _OrgSvcFactory;
        public IOrganizationServiceFactory OrgSvcFactory 
        {
            get
            {
                if (this.OrgSvcFactory == null)
                {
                    this._OrgSvcFactory = this.ServiceProvider.Get<IOrganizationServiceFactory>();

                }
                return this._OrgSvcFactory;
            } 
        }
        public void Trace(string message, [CallerMemberName] string method = null)
        {
        }
        #endregion

        #region context settings
        public TraceLevel TraceLevel { get; set; } = TraceLevel.Error;
        #endregion

        #region run plugin
        internal void Execute()
        {
            var resolver = this.GetPluginServiceResolver();
            foreach (var method in resolver.GetMethods(this.methodPattern, this.PrimaryLogicalName))
            {
                if ((this.Message == "Create" || this.Message == "Update") && !method.AllTargetFilter)
                {
                    if (!this.Target.Attributes.Keys.Where(r => method.TargetFilter.Contains(r)).Any())
                    {
                        continue;
                    }
                }

                if (!method.IsRelevant(this.PluginExecutionContext))
                {
                    continue;
                }

                var args = new object[method.Arguments.Length];

                Microsoft.Xrm.Sdk.Entity strongTypeTarget = null;
                Microsoft.Xrm.Sdk.Entity strongTypePre = null;
                Microsoft.Xrm.Sdk.Entity strongTypeMerged = null;
                Microsoft.Xrm.Sdk.Entity strongTypePost = null;

                var ix = 0;
                foreach (var arg in method.Arguments)
                {
                    try
                    {
                        #region resolve Entity parameters
                        if (arg.IsTarget)
                        {
                            if (strongTypeTarget == null)
                            {
                                strongTypeTarget = this.Target.StrongTypeOf(arg.ToType);
                            }
                            args[ix] = strongTypeTarget;
                            continue;
                        }

                        if (arg.IsPreImage)
                        {
                            if (strongTypePre == null)
                            {
                                strongTypePre = this.PreImage.StrongTypeOf(arg.ToType);
                            }
                            args[ix] = strongTypePre;
                            continue;
                         }

                        if (arg.IsMergedImage)
                        {
                            if (strongTypeMerged == null)
                            {
                                strongTypeMerged = this.Merged.StrongTypeOf(arg.ToType);
                            }
                            args[ix] = strongTypeMerged;
                        }

                        if (arg.IsPostImage)
                        {
                            if (strongTypePost == null)
                            {
                                strongTypePost = this.PostImage.StrongTypeOf(arg.ToType);
                            }
                            args[ix] = strongTypePost;
                        }
                        #endregion

                        #region resolve target reference
                        if (arg.IsTargetReference)
                        {
                            if (arg.ToType == typeof(Microsoft.Xrm.Sdk.EntityReference))
                            {
                                args[ix] = this.TargetReference;
                                continue;
                            }

                            if (arg.FromType == typeof(ITargetReference))
                            {
                                args[ix] = new Implementations.TargetReference(this.TargetReference);
                                continue;
                            }

                            if (arg.FromType.IsGenericType)
                            {
                                args[ix] = Implementations.TargetReference.CreateInstance(this.TargetReference, arg.FromType.GenericTypeArguments.First());
                                continue;
                            }
                        }
                        #endregion

                        #region resolve orgservice
                        if (arg.FromType == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
                        {
                            if (arg.Admin)
                            {
                                args[ix] = this.PluginAdminService;
                            } else
                            {
                                args[ix] = this.PluginUserService;
                            }
                            continue;
                        }
                        #endregion

                        #region resolve standard services
                        if (arg.FromType == (typeof(Microsoft.Xrm.Sdk.ITracingService))) 
                        {
                            args[ix] = this.TracingService;
                            continue;
                        }

                        if (arg.FromType == (typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext)))
                        {
                            args[ix] = this.PluginExecutionContext;
                            continue;
                        }

                        if (arg.FromType == typeof(IServiceEndpointNotificationService))
                        {
                            args[ix] = this.NotificationService;
                            continue;
                        }

                        if (arg.FromType == typeof(IServiceProvider))
                        {
                            args[ix] = this.ServiceProvider;
                            continue;
                        }

                        if (arg.FromType == typeof(IOrganizationServiceFactory))
                        {
                            args[ix] = this.OrgSvcFactory;
                            continue;
                        }
                        #endregion

                        #region resolve action request target
                        if (typeof(Microsoft.Xrm.Sdk.OrganizationRequest).IsAssignableFrom(arg.FromType))
                        {
                            var req = this._PluginExecutionContext.InputParameters["Target"] as Microsoft.Xrm.Sdk.OrganizationRequest;
                            if (req.GetType() == arg.FromType)
                            {
                                args[ix] = req;
                                continue;
                            }

                            var strongReq = (Microsoft.Xrm.Sdk.OrganizationRequest)System.Activator.CreateInstance(arg.FromType);
                            strongReq.Parameters = req.Parameters;
                            strongReq.RequestName = req.RequestName;
                            strongReq.RequestId = req.RequestId;
                            args[ix] = strongReq;
                            continue;
                        }
                        #endregion

                        #region resolve iqueryable
                        #endregion
                        #region resolve irepository
                        #endregion
                    }
                    finally
                    {
                        ix++;
                    }
                }
            }
        }
        #endregion

        #region private helpers
        private PluginServiceResolver GetPluginServiceResolver()
        {
            var type = this.plugin.GetType();
            if (serviceResolverIndex.TryGetValue(type, out PluginServiceResolver pe))
            {
                return pe;
            }

            serviceResolverIndex[type] = new PluginServiceResolver(type);
            return serviceResolverIndex[type];
        }
        #endregion

        #region dispose
        public void Dispose()
        {
            runnings.Remove(System.Threading.Thread.CurrentThread.ManagedThreadId);
        }
        #endregion
    }
}
