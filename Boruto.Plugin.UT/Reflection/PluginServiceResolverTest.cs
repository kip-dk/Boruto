using Boruto.Plugin.Example.Services;
using Boruto.Plugin.UT.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Boruto.Plugin.UT.Reflection
{
    [TestClass]
    public class PluginServiceResolverTest
    {
        private static readonly System.Reflection.Assembly[] ASSM = new System.Reflection.Assembly[]
        {
            typeof(Boruto.BasePlugin).Assembly,
            typeof(Boruto.Plugin.Entities.Account).Assembly,
            typeof(Boruto.Plugin.Example.Entities.bor_plugindemos.NameChanged).Assembly
        };

        [TestMethod]
        public void ResolveRepositoryTest()
        {
            var plugin = new Boruto.Plugin.Example.Plugins.bor_plugindemo.bor_plugindemoPlugin();
            var entity = new Entities.bor_plugindemo { bor_plugindemoId = Guid.NewGuid(), bor_number = 1 };

            using (var serviceProvider = new StandardServiceProvider(40, false, "Delete", Entities.bor_plugindemo.EntityLogicalName, entity.Id))
            {
                plugin.Execute(serviceProvider);
            }
        }
    }
}
