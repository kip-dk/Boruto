using Boruto.Deployment.Services;
using Boruto.Exceptions;
using Boruto.Extensions.Reflection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    public class ServiceFactory : IDisposable
    {
        private readonly IOrganizationService orgService;
        private readonly IServiceProvider customProvider;
        private readonly Assembly[] assms;

        private Dictionary<Type, object> resolvedInstance = new Dictionary<Type, object>();
        private static Dictionary<Type, Type> resolvedTypes = new Dictionary<Type, Type>();

        public ServiceFactory(Microsoft.Xrm.Sdk.IOrganizationService orgService, System.IServiceProvider customProvider, params Assembly[] assms)
        {
            this.orgService = orgService;
            this.customProvider = customProvider;
            this.assms = assms;
        }


        public T Create<T>(string exportName)
        {
            var resolve_type = typeof(T);
            foreach (var assm in this.assms)
            {
                foreach (var type in assm.GetTypes())
                {
                    if (!type.IsAbstract && !type.IsInterface && type.HasPublicConstructor())
                    {
                        var ca = type.GetCustomAttribute<Boruto.Attributes.ExportAttribute>();
                        if (ca != null && ca.Name == exportName && ca.Type == resolve_type && resolve_type.IsAssignableFrom(type))
                        {
                           return (T)this.Create(type, false);
                        }
                    }
                }
            }
            throw new Exceptions.UnresolveableTypeException(exportName, resolve_type);
        } 

        public T Create<T>()
        {
            return (T)this.Create(typeof(T), false);
        }

        public T Get<T>()
        {
            return (T)this.Get(typeof(T)); 
        }

        public void Dispose()
        {
            foreach (var resolv in resolvingTypes)
            {
                if (resolv is IDisposable di)
                {
                    try
                    {
                        di.Dispose();
                    } catch (Exception)
                    {
                        // ignore exceptions thrown by the
                    }
                }
            }

            if (this._context != null)
            {
                this._context.Dispose();
            }
        }

        #region private properties
        Microsoft.Xrm.Sdk.Client.OrganizationServiceContext _context;
        private Microsoft.Xrm.Sdk.Client.OrganizationServiceContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new Microsoft.Xrm.Sdk.Client.OrganizationServiceContext(this.orgService);
                }
                return _context;
            }
        }

        private Boruto.ServiceAPI.IMetadataService _metaservice;
        private Boruto.ServiceAPI.IMetadataService Metaataservice
        {
            get
            {
                if (_metaservice == null)
                {
                    this._metaservice = new Implementations.Services.MetadataService(this.orgService);
                    resolvedInstance[typeof(Boruto.ServiceAPI.IMetadataService)] = this._metaservice;

                }
                return _metaservice;
            }
        }
        #endregion

        #region private helpers
        private object Get(Type type)
        {
            #region resolve from cache
            if (resolvedInstance.TryGetValue(type, out object o))
            {
                return o;
            }
            #endregion

            #region resolve SDK services
            if (type == typeof(Microsoft.Xrm.Sdk.IOrganizationService))
            {
                return this.orgService;
            }
            #endregion

            #region resolve query and repositories
            if (type.IsQueryable())
            {
                var repo = this.ResolveRepository(type.GenericTypeArguments[0]);
                var queryMethd = repo.GetType().GetMethod("GetQuery", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                return queryMethd.Invoke(repo, null);

            }

            if (type.IsRepository())
            {
                return this.ResolveRepository(type.GenericTypeArguments[0]);
            }
            #endregion

            #region resolve boruto services
            if (type == typeof(Boruto.ServiceAPI.IMetadataService))
            {
                return this.Metaataservice;
            }

            if (type == typeof(Boruto.ServiceAPI.INamingService))
            {
                resolvedInstance[type] = new Implementations.Services.NamingService(this.Metaataservice, this.orgService);
                return resolvedInstance[type];
            }
            #endregion

            #region customprovider
            var can = this.ResolveCustomService(type);
            if (can != null)
            {
                resolvedInstance[type] = can;
                return resolvedInstance[type];
            }
            #endregion

            return this.Create(type, true);

        }

        private object Create(Type type, bool shared)
        {
            if (resolvedTypes.TryGetValue(type, out Type t))
            {
                return this.DoCreate(t);
            }

            foreach (var assm in this.assms)
            {
                foreach (var can in assm.GetTypes())
                {
                    if (!can.IsAbstract && !can.IsInterface && can.HasPublicConstructor() && type.IsAssignableFrom(can))
                    {
                        resolvedTypes[type] = can;
                        return DoCreate(resolvedTypes[type]);
                    }
                }
            }
            throw new Exceptions.UnresolveableTypeException(type);
        }

        private List<Type> resolvingTypes = new List<Type>();

        /// <summary>
        /// Do create should only bee called with the final nonabtract not and interface, that should actually be created, by finding a constructor, and inject dependencies into that 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private object DoCreate(Type type)
        {
            // first we see if we can find a public multi argument constructor
            var cons = type.GetConstructors().Where(r => r.IsPublic && !r.IsStatic && (r.GetParameters()?.Length ?? 0) > 0).ToArray();
            if (cons.Length == 0)
            {
                // OK no public constructor with arguments,  lets see if we can find a default constructor
                cons = type.GetConstructors().Where(r => r.IsPublic && !r.IsStatic && (r.GetParameters()?.Length ?? 0) == 0).ToArray();
                if (cons.Length == 1)
                {
                    return cons[0].Invoke(null);
                }
                throw new MissingPublicConstructorException(type);
            }

            if (cons.Length > 1)
            {
                throw new Exceptions.MoreThanOnPlublicConstructorFoundException(type);
            }

            var pms = cons[0].GetParameters();
            var args = new object[pms.Length];

            for (var i=0;i<pms.Length;i++)
            {
                var toberesolved = pms[i].ParameterType;
                if (resolvingTypes.Contains(toberesolved))
                {
                    throw new Exceptions.CyclicalDependencyException(toberesolved, resolvingTypes.ToArray());
                }
                resolvingTypes.Add(toberesolved);
                args[i] = this.Get(toberesolved);
                resolvingTypes.Remove(toberesolved);
            }

            return cons[0].Invoke(args);
        }

        private Dictionary<string, object> repositoryTypes = new Dictionary<string, object>();

        private object ResolveRepository(Type entityType)
        {
            var key = $"{entityType.ToEntityLogicalName()}";
            if (repositoryTypes.TryGetValue(key, out object o))
            {
                return o;
            }
            Type resultType = typeof(Implementations.Repository<>).MakeGenericType(entityType);
            repositoryTypes[key] = Activator.CreateInstance(resultType, orgService, this.Context);
            return repositoryTypes[key];
        }

        private static Dictionary<Type, bool> customService = new Dictionary<Type, bool>();
        private object ResolveCustomService(Type fromType)
        {
            if (this.customProvider == null)
            {
                return null;

            }
            if (customService.TryGetValue(fromType, out bool isCustom))
            {
                if (isCustom)
                {
                    return this.customProvider.GetService(fromType);
                }
                else
                {
                    return null;
                }
            }

            var result = this.customProvider.GetService(fromType);
            customService[fromType] = result != null;
            return result;
        }
        #endregion
    }
}
