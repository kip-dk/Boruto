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

        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
