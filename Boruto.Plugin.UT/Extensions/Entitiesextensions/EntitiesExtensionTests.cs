using Boruto.Extensions.Entities;
using Boruto.Extensions.TypeConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Extensions.Entitiesextensions
{
    [TestClass]
    public class EntitiesExtensionTests
    {
        [TestMethod]
        public void CleanupVirtualEntityTest()
        {
            var entity = new Microsoft.Xrm.Sdk.Entity();
            entity["c1"] = new Microsoft.Xrm.Sdk.EntityReference("c1", 1.ToGuid());
            entity["c2"] = new Microsoft.Xrm.Sdk.EntityReference("c2", 1.ToGuid());
            entity["c3"] = new Microsoft.Xrm.Sdk.EntityReference("c2", 2.ToGuid());
            Assert.AreEqual(3, entity.Attributes.Count);

            entity.CleanupVirtualEntity();
            Assert.AreEqual(2, entity.Attributes.Count);
        }
    }
}
