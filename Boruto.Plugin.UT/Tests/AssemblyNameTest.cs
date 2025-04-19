using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Tests
{
    [TestClass]
    public class AssemblyNameTest
    {
        [TestMethod]
        public void AssemblyName()
        {
            var name = typeof(Boruto.Plugin.Example.Entities.AccountStateChanged).Assembly.FullName;

            Assert.IsTrue(name.StartsWith($"Boruto.Plugin.Example,"));
        }
    }
}
