using Boruto.Plugin.Entities;
using Boruto.ServiceAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.Example.Services
{
    public class ExampleServiceProvider : IServiceProvider
    {
        private readonly IServiceContext ctx;

        public ExampleServiceProvider(Boruto.ServiceAPI.IServiceContext ctx)
        {
            this.ctx = ctx;
        }

        public static void log(string message)
        {
            Boruto.Trace.Conditional(message);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(Boruto.Plugin.Entities.IUnitOfWork))
            {
                if (this.ctx.IsAdmin)
                {
                    log("Admin UOW");
                    return this.AdminUnitOfWork;
                }

                log("UOW");
                return this.UnitOfWork;
            }

            if (this.IsRepository(serviceType)) 
            {
                if (ctx.IsAdmin)
                {
                    log("Admin REPO");
                    return this.AdminUnitOfWork.GetRepoByType(serviceType.GetGenericArguments().First());
                }

                log("REPO");
                return this.UnitOfWork.GetRepoByType(serviceType.GetGenericArguments().First());
            }
            return null;
        }

        private Boruto.Plugin.Entities.IUnitOfWork _uow;
        private Boruto.Plugin.Entities.IUnitOfWork UnitOfWork
        {
            get
            {
                if (_uow == null)
                {
                    this._uow = new CrmUnitOfWork(this.ctx.UserOrganizationService);
                }
                return _uow;
            }
        }

        private Boruto.Plugin.Entities.IUnitOfWork _adminuow;
        private Boruto.Plugin.Entities.IUnitOfWork AdminUnitOfWork
        {
            get
            {
                if (_adminuow == null)
                {
                    this._adminuow = new CrmUnitOfWork(this.ctx.AdminOrganizationService);
                }
                return _adminuow;
            }
        }

        private static readonly Dictionary<Type, bool> isrepo = new Dictionary<Type, bool>();
        private bool IsRepository(Type type)
        {
            if (isrepo.TryGetValue(type, out bool v))
            {
                return v;
            }

            var isRepo =  type.IsInterface && type.IsGenericType && type.FullName.StartsWith("Boruto.Plugin.Entities.IRepository") && type.GetGenericArguments().First().IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity));
            isrepo[type] = isRepo;
            return isRepo;
        }
    }
}
