using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Tests
{
    [TestClass]
    public class ReflectionTest
    {
        [TestMethod]
        public void RepositoryReflectionTest()
        {
            var type = typeof(Boruto.IRepository<Entities.Account>);

            Assert.IsTrue(type.IsInterface && type.IsGenericType && type.FullName.StartsWith("Boruto.IRepository"));

            Assert.AreEqual(type.GenericTypeArguments[0], typeof(Entities.Account)); 
        }
    }
}
