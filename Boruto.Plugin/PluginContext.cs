﻿using Boruto.Extensions.Framework;
using Boruto.Extensions.SDK;
using Boruto.Implementations;
using Boruto.Plugin;
using Boruto.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Boruto
{
    public class PluginContext : IDisposable
    {
        private static Dictionary<System.Threading.Thread, List<PluginContext>> runnings = new Dictionary<System.Threading.Thread, List<PluginContext>>();
        private static Dictionary<Type, Reflection.PluginServiceResolver> serviceResolverIndex = new Dictionary<Type, Reflection.PluginServiceResolver>();

        private static readonly object locker = new object();

        public static PluginContext Current
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
        private List<string> logs = new List<string>();

        internal PluginContext(BasePlugin plugin, IServiceProvider standardServiceProvider, IServiceProvider customServiceProvider, Assembly[] assemblies, string unsecure, string secure)
        {
            this.plugin = plugin;
            this.Type = plugin.GetType();
            this.StandardServiceProvider = standardServiceProvider;
            this.CustomServiceProvider = customServiceProvider;
            this.ServiceAssemblies = assemblies;
            this.Unsecure = unsecure;
            this.Secure = secure;

            this.Message = this.PluginExecutionContext.MessageName;
            this.Stage = this.PluginExecutionContext.Stage;
            this.IsAsync = this.PluginExecutionContext.Mode > 0;
            this.PrimaryLogicalName = this.PluginExecutionContext.PrimaryEntityName;
            this.PrimaryEntityId = this.PluginExecutionContext.PrimaryEntityId;

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
                    this._preimage.LogicalName = this.PrimaryLogicalName;
                    this._preimage.Id = this.PrimaryEntityId;

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
                    this._postimage.LogicalName = this.PrimaryLogicalName;
                    this._postimage.Id = this.PrimaryEntityId;
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
        public Microsoft.Xrm.Sdk.Entity Merged
        {
            get
            {
                if (this.Message == "Delete")
                {
                    return this.PreImage;
                }

                if (this.Message != "Update")
                {
                    return this.Target;
                }

                if (_merged == null)
                {
                    this._merged = new Entity();
                    this._merged.LogicalName = this.PrimaryLogicalName;
                    this._merged.Id = this.PrimaryEntityId;
                    var pre = this.PreImage;
                    foreach (var att in pre.Attributes)
                    {
                        this._merged[att.Key] = att.Value;
                    }

                    var tar = this.Target;

                    foreach (var att in tar.Attributes)
                    {
                        this._merged[att.Key] = att.Value;
                    }
                }
                return _merged;
            }
        }

        private Microsoft.Xrm.Sdk.EntityReference _targetReference;
        public Microsoft.Xrm.Sdk.EntityReference TargetReference
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

        public Microsoft.Xrm.Sdk.OrganizationRequest OrganizationRequest
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

        #endregion

        #region microsoft service properties
        private IOrganizationService _InitiatingUserService;
        public IOrganizationService InitiatingUserService 
        {
            get
            {
                if (this._InitiatingUserService == null)
                {
                    this._InitiatingUserService = this.StandardServiceProvider.GetOrganizationService(PluginExecutionContext.InitiatingUserId);
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
                    this._PluginUserService = this.StandardServiceProvider.GetOrganizationService(PluginExecutionContext.UserId); // User that the plugin is registered to run as, Could be same as current user.

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
                    this._PluginExecutionContext = StandardServiceProvider.Get<IPluginExecutionContext>();
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
                    this._NotificationService = this.StandardServiceProvider.Get<IServiceEndpointNotificationService>();

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
                    this._TracingService = this.StandardServiceProvider.Get<ITracingService>();

                }
                return this._TracingService;
            } 
        }

        public IServiceProvider StandardServiceProvider { get; }
        public IServiceProvider CustomServiceProvider { get; }

        public Assembly[] ServiceAssemblies { get; }

        private IOrganizationServiceFactory _OrgSvcFactory;
        public IOrganizationServiceFactory OrgSvcFactory 
        {
            get
            {
                if (this.OrgSvcFactory == null)
                {
                    this._OrgSvcFactory = this.StandardServiceProvider.Get<IOrganizationServiceFactory>();

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
                    this._AdminServiceContext = new Microsoft.Xrm.Sdk.Client.OrganizationServiceContext(this.PluginAdminService);
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
                    this._UserServiceContext = new Microsoft.Xrm.Sdk.Client.OrganizationServiceContext(this.PluginAdminService);
                }
                return this._UserServiceContext;
            }
        }
        #endregion

        #region context settings
        public TraceLevel TraceLevel { get; set; } = TraceLevel.Error;
        #endregion

        #region run plugin
        internal void Execute()
        {
            using (var fac = new Reflection.ServiceFactory(this))
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

                    #region resolve arguments
                    var args = new object[method.Arguments.Length];

                    var ix = 0;
                    foreach (var arg in method.Arguments)
                    {
                            args[ix] = fac.Resolve(arg);
                            ix++;
                    }
                    #endregion

                    #region invoke
                    var result = method.method.Invoke(this.plugin, args);

                    if (this.Stage == 40 && this.IsAsync == false && result is Microsoft.Xrm.Sdk.OrganizationResponse re && re.Results != null)
                    {
                        foreach (var p in re.Results)
                        {
                            this.PluginExecutionContext.OutputParameters[p.Key] = p.Value;
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region public methods
        public void Trace(string message, [CallerMemberName] string method = null)
        {
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
    }
}
