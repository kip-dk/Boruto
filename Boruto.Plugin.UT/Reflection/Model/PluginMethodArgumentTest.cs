using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Reflection.Model
{
    [TestClass]
    public class PluginMethodArgumentTest
    {

        [TestMethod]
        public void ArgumentConstructorTest()
        {
            {
                var pluginType = typeof(Boruto.Plugin.Example.Plugins.Account.AccountPlugin);

                {
                    var method = pluginType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(r => r.Name == "OnPreUpdate").First();
                    var parameter = method.GetParameters().Single();

                    var arg = new Boruto.Reflection.Model.PluginMethodArgument(pluginType, method, parameter, Boruto.Plugin.Entities.Account.EntityLogicalName, Consts.Assemblies);

                    Assert.IsTrue(arg.IsTarget);
                    Assert.IsFalse(arg.IsPreImage);
                    Assert.IsFalse(arg.IsMergedImage);
                    Assert.IsFalse(arg.IsPostImage);
                    Assert.IsTrue(arg.IsEntityMatch);
                    Assert.AreEqual(typeof(Boruto.Plugin.Example.Entities.AccountStateChanged), arg.EarlyBoundEntityType);
                    Assert.IsFalse(arg.FilteredAllAttributes);
                    Assert.AreEqual(1, arg.FilteredAttributes.Length);
                    Assert.AreEqual("statecode", arg.FilteredAttributes[0]);
                }

                {
                    var method = pluginType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(r => r.Name == "OnPreDelete").First();
                    var parameter = method.GetParameters().Single();

                    var arg = new Boruto.Reflection.Model.PluginMethodArgument(pluginType, method, parameter, Boruto.Plugin.Entities.Account.EntityLogicalName, Consts.Assemblies);

                    Assert.IsTrue(arg.IsTargetReference);
                    Assert.IsFalse(arg.IsTarget);
                    Assert.IsFalse(arg.IsPreImage);
                    Assert.IsFalse(arg.IsMergedImage);
                    Assert.IsFalse(arg.IsPostImage);
                    Assert.IsTrue(arg.IsEntityMatch);
                }
            }

            {
                var pluginType = typeof(Boruto.Plugin.Example.Plugins.Lead.LeadPlugin);
                var method = pluginType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(r => r.Name == "OnPost").First();
                var parameter = method.GetParameters().Single();

                var arg = new Boruto.Reflection.Model.PluginMethodArgument(pluginType, method, parameter, null, Consts.Assemblies);
                Assert.IsFalse(arg.IsTargetReference);
                Assert.IsFalse(arg.IsTarget);
                Assert.IsFalse(arg.IsPreImage);
                Assert.IsFalse(arg.IsMergedImage);
                Assert.IsFalse(arg.IsPostImage);
                Assert.IsTrue(arg.IsOrganizationRequest);
            }
        }
    }
}
