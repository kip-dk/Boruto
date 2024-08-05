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

        internal object Resolve(Reflection.Model.Argument argument)
        {
            if (resolved.TryGetValue(argument.FromType, out object o))
            {
                return o;
            }

            var result = this.DoResolve(argument.FromType);

            if (result is ITarget target)
            {
                target.Attributes = this.ctx.Target.Attributes;
                if (ctx.Stage <= 20 && ctx.Message == "Update")
                {
                    target.PropertyChanged += Target_PropertyChanged;
                }
                return result;
            }

            if (result is System.ComponentModel.INotifyPropertyChanged no)
            {

            }


            return result;
        }

        private void Target_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.ctx.Merged[e.PropertyName] = this.ctx.Target[e.PropertyName];
        }

        #region private helpers
        private object DoResolve(Type type)
        {
            object result = new object();



            resolved[type] = result;
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
