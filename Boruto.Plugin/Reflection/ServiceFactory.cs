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

            var result = this.DoResolve(argument);

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
        private object DoResolve(Reflection.Model.Argument argument)
        {
            if (argument.IsOrganizationRequest)
            {
                var orgR  = System.Activator.CreateInstance(argument.FromType) as Microsoft.Xrm.Sdk.OrganizationRequest;
                orgR.RequestName = this.ctx.PluginExecutionContext.MessageName;
                orgR.Parameters = this.ctx.PluginExecutionContext.InputParameters;
                resolved[argument.FromType] = orgR;
                return orgR;
            }

            if (argument.FromType.IsInterface && argument.IsTarget && typeof(ITarget).IsAssignableFrom(argument.FromType))
            {
            }

            if (argument.IsTarget && argument.FromType.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
            }

            object result = null;

            resolved[argument.FromType] = result;
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
